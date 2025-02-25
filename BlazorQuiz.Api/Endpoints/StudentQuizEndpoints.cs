using BlazorQuiz.Api.Services;
using BlazorQuiz.Shared;

namespace BlazorQuiz.Api.Endpoints
{
    public static class StudentQuizEndpoints
    {
        public static IEndpointRouteBuilder MapStudentQuizEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/student")
                .RequireAuthorization(p=>p.RequireRole(nameof(UserRole.Student)));
            group.MapGet("/available-quizes", async (int categoryId, StudentQuizService service) =>

            {
               return Results.Ok(await service.GetActiveQuizesAsync(categoryId));
            });
            

            return app;
        }
    }
}
