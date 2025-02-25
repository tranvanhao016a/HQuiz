using BlazorQuiz.Api.Data;
using BlazorQuiz.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace BlazorQuiz.Api.Services
{
    public class StudentQuizService
    {
        private readonly QuizContext _context;
        public StudentQuizService(QuizContext context)
        {
            _context = context;
        }

        public async Task<QuizListDto[]> GetActiveQuizesAsync( int categoryId)
        {
            var query = _context.Quizzes.Where(q => q.IsActive);
            if (categoryId > 0)
            {
                query = query.Where(q => q.CategoryId == categoryId);
            } 
            var quizes = await _context.Quizzes
                .Where(q => q.CategoryId == categoryId && q.IsActive)
                .Select(q => new QuizListDto
                {
                    Id = q.Id,
                    Name = q.Name,
                    TimeInMinutes = q.TimeInMinutes,
                    TotalQuestions = q.Questions.Count,
                    CategoryId = q.CategoryId,
                    CategoryName = q.Category.Name
                })
                .ToArrayAsync();
            return quizes;
        }
    
     }
}
