using BlazorQuiz.Shared.DTO;
using Refit;

namespace BlazorQuiz.Web.Apis
{
    [Headers("Authorization: Bearer")]
    public interface IStudentQuizApi
    {
        [Get("/api/student/available-quizes")]
        Task<QuizListDto[]> GetActiveQuizesAsync(int categoryId);
    }
}
