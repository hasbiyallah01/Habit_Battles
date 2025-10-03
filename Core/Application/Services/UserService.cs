using Google.Apis.Auth;
using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Models.UserModel;
using Habit_Battles.Models;
using System.Security.Claims;
using Habit_Battles.Core.Application.Interfaces.Services;

namespace Habit_Battles.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _configuration = configuration;
        }

        public async Task<BaseResponse<UserResponseModel>> CreateUser(UserRequestModel request)
        {
            int randomCode = new Random().Next(10000, 99999);

            if ( await _userRepository.ExistsAsync(request.Email))
            {
                return new BaseResponse<UserResponseModel>
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }
            else
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return new BaseResponse<UserResponseModel>
                    {
                        Message = "Password does not match",
                        IsSuccessful = false
                    };
                }


                var user = new User
                {
                    Email = request.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Username = request.UserName,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    CreatedBy = "ManualRegistration",
                };

                var newUser = await _userRepository.AddAsync(user);

                await _unitOfWork.SaveAsync();

                return new BaseResponse<UserResponseModel>
                {
                    Message = "Check your email and complete your registration",
                    IsSuccessful = true,
                    Value = new UserResponseModel
                    {
                        Id = user.Id,
                        UserName = user.Username,
                        Email = user.Email,
                    }
                };
            }
        }



        public async Task<BaseResponse<LoginResponse>> GoogleLogin(string tokenId)
        {
            if (!tokenId.Contains("."))
            {
                return new BaseResponse<LoginResponse>
                {
                    Message = "Invalid Google token format",
                    IsSuccessful = false
                };
            }

            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { "237085518851-jofqas7qcl591ba3kqcj09dl177ikbh1.apps.googleusercontent.com" }
            };

            try
            {
                var googlePayload = await GoogleJsonWebSignature.ValidateAsync(tokenId, validationSettings);

                if (googlePayload == null)
                {
                    return new BaseResponse<LoginResponse>
                    {
                        Message = "Invalid Google token",
                        IsSuccessful = false
                    };
                }

                var user = await _userRepository.GetAsync(u => u.Email == googlePayload.Email);

                if (user != null)
                {
                    var existingUserResponse = new LoginResponse
                    {
                        Id = user.Id,
                        UserName = user.Username,
                        Email = user.Email,
                    };

                    return new BaseResponse<LoginResponse>
                    {
                        Message = "User logged in successfully",
                        IsSuccessful = true,
                        Value = existingUserResponse
                    };
                }
                else
                {
                    user = new User
                    {
                        Username = googlePayload.GivenName,
                        Email = googlePayload.Email,
                        DateCreated = DateTime.UtcNow,
                        IsDeleted = false,
                        CreatedBy = "GoogleOAuth",
                    };

                    var newUser = await _userRepository.AddAsync(user);
                    await _unitOfWork.SaveAsync();

                    var newUserResponse = new LoginResponse
                    {
                        Id = user.Id,
                        UserName = user.Username,
                        Email = user.Email
                    };

                    return new BaseResponse<LoginResponse>
                    {
                        Message = "Google user created successfully",
                        IsSuccessful = true,
                        Value = newUserResponse
                    };
                }
            }
            catch (InvalidJwtException ex)
            {
                return new BaseResponse<LoginResponse>
                {
                    Message = $"JWT validation failed: {ex.Message}",
                    IsSuccessful = false
                };
            }
        }

        public async Task<BaseResponse<ICollection<UserResponseModel>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();

            return new BaseResponse<ICollection<UserResponseModel>>
            {
                Message = "List of users",
                IsSuccessful = true,
                Value = users.Select(user => new UserResponseModel
                {
                    Id = user.Id,
                    UserName = user.Username,
                    Email = user.Email
                }).ToList(),
            };
        }

        public async Task<BaseResponse<UserResponseModel>> GetUser(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse<UserResponseModel>
                {
                    Message = "User not found",
                    IsSuccessful = false
                };
            }
            return new BaseResponse<UserResponseModel>
            {
                Message = "User successfully found",
                IsSuccessful = true,
                Value = new UserResponseModel
                {
                    Id = user.Id,
                    UserName = user.Username,
                    Email = user.Email
                }
            };
        }

        public async Task<BaseResponse> RemoveUser(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }

            _userRepository.Remove(user);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User deleted successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> UpdateUser(int id, UserRequestModel request)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }


            var exists = await _userRepository.ExistsAsync(request.Email, id);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }


            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            user.Username = request.UserName;
            user.Email = request.Email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.IsDeleted = false;
            user.DateModified = DateTime.UtcNow;
            user.ModifiedBy = loginUserId;

            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User updated successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<LoginResponse>> Login(LoginRequest model)
        {
            var user = await _userRepository.GetAsync(model.Email);
            if (user.Email == model.Email && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return new BaseResponse<LoginResponse>
                {
                    Message = "Login Successfull",
                    IsSuccessful = true,
                    Value = new LoginResponse
                    {
                        Id = user.Id,
                        UserName = user.Username,
                        Email = user.Email
                    }
                };
            }
            return new BaseResponse<LoginResponse>
            {
                Message = "Invalid Credentials",
                IsSuccessful = false
            };
        }

        public async Task<BaseResponse<ProfileResponse>> GetProfile(int id)
        {
            var user = await _userRepository.GetAsyncWithLogs(id);
            if (user == null)
            {

            }

            return new BaseResponse<ProfileResponse>
            {
                Message = "",
                IsSuccessful = true,
                Value = new ProfileResponse
                {
                    Id = user.Id,
                    Coins = user.Coins,
                    Email = user.Email,
                    Streaks = user.Streaks,
                    UserName = user.Username,
                    Achievements = user.HabitLogs,
                }
            };
        }
    }
}
