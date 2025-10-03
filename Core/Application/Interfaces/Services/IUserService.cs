using Habit_Battles.Models;
using Habit_Battles.Models.UserModel;

namespace Habit_Battles.Core.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponseModel>> GetUser(int id);
        Task<BaseResponse<ICollection<UserResponseModel>>> GetAllUsers();
        Task<BaseResponse> RemoveUser(int id);
        Task<BaseResponse> UpdateUser(int id, UserRequestModel request);
        Task<BaseResponse<LoginResponse>> Login(LoginRequest model);
        Task<BaseResponse<UserResponseModel>> CreateUser(UserRequestModel request);
        Task<BaseResponse<LoginResponse>> GoogleLogin(string tokenId);
        Task<BaseResponse<ProfileResponse>> GetProfile(int id);
    }
}
