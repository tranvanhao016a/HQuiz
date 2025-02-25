using Microsoft.AspNetCore.Mvc;
using BlazorQuiz.Api.Services;
using BlazorQuiz.Shared;
using System.Threading.Tasks;
using BlazorQuiz.Api.Data.Entities;

namespace BlazorQuiz.Api.Endpoints
{
    public static class UserEndPoints
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/users")
                .RequireAuthorization(p => p.RequireRole(nameof(UserRole.Admin)));

            group.MapGet("", async ([FromQuery] UserApprovedFilter filter, [FromQuery] int startIndex, [FromQuery] int pageSize, UserService userService) =>
            {
                return Results.Ok(await userService.GetUsersAsync(filter, startIndex, pageSize));
            });

            group.MapPatch("/toggle-status", async ([FromQuery] int userId, UserService userService) =>
            {
                Console.WriteLine($"Endpoint Toggling approval status for User ID: {userId}");
                var response = await userService.ToggleUserApprovedStatus(userId);
                return Results.Ok(response);
            });

            return app;
        }
    }
}
