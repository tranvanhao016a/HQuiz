using BlazorQuiz.Shared.DTO;
using BlazorQuiz.Shared;
using Refit;

[Headers("Authorization: Bearer")]
public interface IUserApi
{
    [Get("/api/users")]
    Task<PagedResult<UserDto>> GetUsersAsync(UserApprovedFilter filter, int startIndex, int pageSize);

    [Patch("/api/users/toggle-status")]
    Task ToggleUserApprovedStatus([Query] int userId);

}
