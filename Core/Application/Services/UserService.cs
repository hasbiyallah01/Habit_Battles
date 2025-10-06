using Google.Apis.Auth;
using Habit_Battles.Core.Application.Interfaces.Repositories;
using Habit_Battles.Core.Domain.Entities;
using Habit_Battles.Models.UserModel;
using Habit_Battles.Models;
using System.Security.Claims;
using Habit_Battles.Core.Application.Interfaces.Services;
using Habit_Battles.Core.Domain.Enums;
using Habit_Battles.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Habit_Battles.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBattleRepository _battleRepository;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IConfiguration configuration, IBattleRepository battleRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _configuration = configuration;
            _battleRepository = battleRepository;
        }

        public async Task<BaseResponse<UserResponseModel>> CreateUser(UserRequestModel request)
        {

            if ( await _userRepository.ExistsAsync(request.Email))
            {
                return new BaseResponse<UserResponseModel>
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }
            if (await _userRepository.ExistsByNameAsync(request.UserName))
            {
                return new BaseResponse<UserResponseModel>
                {
                    Message = "Name already exists!!!",
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
                    Message = "User registered successfully.",
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
        public async Task<BaseResponse<UserResponseModel>> GetCurrentUserAsync()
        {
            var user = _httpContext.HttpContext?.User;
            if (user == null || !(user.Identity?.IsAuthenticated ?? false))
                return new BaseResponse<UserResponseModel>
                { 
                    IsSuccessful = false,
                    Message = "User not Authenticated"
                };

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = user.FindFirst(ClaimTypes.Email);
            var fullNameClaim = user.FindFirst(ClaimTypes.Name);

            if (idClaim == null || !int.TryParse(idClaim.Value, out int id))
                return null;

            var userEntity = await _userRepository.GetAsync(id);
            if (userEntity == null)
                return null;

            return new BaseResponse<UserResponseModel>
            {
                IsSuccessful = true,
                Message = "User Authenticated",
                Value = new UserResponseModel
                {
                    Id = id,
                    Email = emailClaim?.Value ?? string.Empty,
                    UserName = fullNameClaim?.Value ?? string.Empty,
                }
            };
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
            if(users == null )
            {
                return new BaseResponse<ICollection<UserResponseModel>>
                {
                    Message = "No users",
                    IsSuccessful = false,
                };
            }
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
            if (user == null)
            {
                return new BaseResponse<LoginResponse>
                {
                    Message = "User not found.",
                    IsSuccessful = false
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return new BaseResponse<LoginResponse>
                {
                    Message = "Invalid credentials.",
                    IsSuccessful = false
                };
            }

            var battles = await _battleRepository.GetByUserIdAsync(user.Id) ?? new List<Battle>();

            var activeBattles = battles.Any()
                ? battles.Select(b => new BattleSummaryDto
                {
                    BattleId = b.Id,
                    Habit = b.Habit,
                    MonsterType = b.MonsterType,
                    OpponentName = b.Creator.Id == user.Id ? b.Opponent?.Username ?? "Unknown" : b.Creator?.Username ?? "Unknown",
                    CreatorHealth = b.CreatorHealth,
                    OpponentHealth = b.OpponentHealth,
                    Status = b.Status
                }).ToList()
                : new List<BattleSummaryDto>();

            return new BaseResponse<LoginResponse>
            {
                IsSuccessful = true,
                Message = "Login successful.",
                Value = new LoginResponse
                {
                    Id = user.Id,
                    UserName = user.Username,
                    Email = user.Email,
                    CoinBalance = user.Coins,
                    ActiveBattles = activeBattles,
                    Streaks = user.Streaks,
                }
            };
        }

        public async Task<BaseResponse<ProfileResponse>> GetProfile(int id)
        {
            var user = await _userRepository.GetAsyncWithLogs(id);
            if (user == null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    IsSuccessful = false,
                    Message = "Theres no user with this id or user not authenticated"
                };
            }

            return new BaseResponse<ProfileResponse>
            {
                Message = "User profile retrieved successfully",
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
