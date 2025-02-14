using BlazorQuiz.Api.Services;
using BlazorQuiz.Shared.DTO;

namespace BlazorQuiz.Api.Endpoints
{
    public static class AuthEnpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/login", async (AuthService authService, LoginDto dto) =>

            Results.Ok(await authService.LoginAsync(dto))
            );
            return app;
        }
    }
}
