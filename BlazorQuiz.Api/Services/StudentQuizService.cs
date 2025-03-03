﻿using BlazorQuiz.Api.Data;
using BlazorQuiz.Api.Data.Entities;
using BlazorQuiz.Shared.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
    
        public async Task<QuizApiResponse<int>> StartQuizAsync(int studentId, Guid quizId)
        {
            try
            {
                var studentQuiz = new StudentQuiz
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    Status = nameof(StudentQuizStats.Started),
                    StartedOn = DateTime.Now,
                };
                _context.StudentQuizzes.Add(studentQuiz);
                await _context.SaveChangesAsync();

                return QuizApiResponse<int>.Success(studentQuiz.Id);
            }
            catch (Exception ex)
            {
                return QuizApiResponse<int>.Fail(ex.Message);
            }
        }

        public async Task<QuizApiResponse<QuestionDto?>> GetNextQuestionForQuizAsync(int studentQuizId, int studentId)
        {
            var studentQuiz = await _context.StudentQuizzes
                .Include(s => s.StudentQuizQuestion)
                .FirstOrDefaultAsync(s => s.Id == studentQuizId);

            if (studentQuiz == null)
            {
                return QuizApiResponse<QuestionDto?>.Fail("Quiz does not exist");
            }

            if (studentQuiz.StudentId != studentId)
            {
                return QuizApiResponse<QuestionDto?>.Fail("Invalid request");
            }

            var questionsServed = studentQuiz.StudentQuizQuestion
                .Select(s => s.QuestionId)
                .ToArray();

            var nextQuestion = await _context.Questions
                .Where(q => q.QuizId == studentQuiz.QuizId)
                .Where(q => !questionsServed.Contains(q.Id))
                .OrderBy(q => Guid.NewGuid())
                .Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = q.Options.Select(o => new OptionDto
                    {
                        Id = o.Id,
                        Text = o.Text
                    }).ToList()
                })
                .Take(1).FirstOrDefaultAsync();

            if (nextQuestion == null)
            {
                return QuizApiResponse<QuestionDto?>.Fail("No more questions for this quiz");
            }

            try
            {
                var studentQuizQuestion = new StudentQuizQuestion
                {
                    StudentQuizId = studentQuizId,
                    QuestionId = nextQuestion.Id
                };
                _context.StudentQuestions.Add(studentQuizQuestion);
                await _context.SaveChangesAsync();
                return QuizApiResponse<QuestionDto?>.Success(nextQuestion);
            }
            catch (Exception ex)
            {
                return QuizApiResponse<QuestionDto?>.Fail(ex.Message);
            }
        }


        public async Task<QuizApiResponse> SaveQuestionResponseAsync(StudentQuizQuestionResponseDto dto, int studentId)
        {
            var studentQuiz = await _context.StudentQuizzes
                .AsTracking()
                .FirstOrDefaultAsync(s => s.Id == dto.StudentQuizId);
            if (studentQuiz == null)
            {
                return QuizApiResponse.Fail("Quiz does not exist");
            }
            if (studentQuiz.StudentId != studentId)
            {
                return QuizApiResponse.Fail("Invalid request");
            }
            var isSelectedOptionCorrect = await _context.Options
                .Where(o => o.QuestionId == dto.QuestionId && o.Id == dto.OptionId)
                .Select(o => o.IsCorrect)
                .FirstOrDefaultAsync();

            if (isSelectedOptionCorrect)
            {
                studentQuiz.Score++;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return QuizApiResponse.Fail(ex.Message);
                }
            }
            return QuizApiResponse.Success;
        }

        public async Task<QuizApiResponse> SubmitQuizAsync(int studentQuizId, int studentId)
      => await CompleteQuizAsync(studentQuizId, DateTime.Now, nameof(StudentQuizStats.Completed), studentId);

        public async Task<QuizApiResponse> ExitQuizAsync(int studentQuizId, int studentId)
            => await CompleteQuizAsync(studentQuizId, null, nameof(StudentQuizStats.Expired), studentId);

        public async Task<QuizApiResponse> AutoSubmitQuizAsync(int studentQuizId, int studentId)
            => await CompleteQuizAsync(studentQuizId, DateTime.Now, nameof(StudentQuizStats.AutoSubmitted), studentId);

        private async Task<QuizApiResponse> CompleteQuizAsync(int studentQuizId, DateTime? completeOn, string status, int studentId)
        {
            var studentQuiz = await _context.StudentQuizzes
                .AsTracking()
                .FirstOrDefaultAsync(s => s.Id == studentQuizId);
            if (studentQuiz == null)
            {
                return QuizApiResponse.Fail("Quiz does not exist");
            }
            if (studentQuiz.StudentId != studentId)
            {
                return QuizApiResponse.Fail("Invalid request");
            }
            if (studentQuiz.CompletedOn.HasValue || studentQuiz.Status == nameof(StudentQuizStats.Expired))
            {
                return QuizApiResponse.Fail("Quiz already submitted");
            }
            try
            {
                studentQuiz.Status = status;
                studentQuiz.CompletedOn = completeOn;
                var studentQuizQuestions = await _context.StudentQuestions
                    .Where(s => s.StudentQuizId == studentQuizId)
                    .ToListAsync();

                _context.StudentQuestions.RemoveRange(studentQuizQuestions);
                await _context.SaveChangesAsync();

                return QuizApiResponse.Success;
            }
            catch (Exception ex)
            {
                return QuizApiResponse.Fail(ex.Message);
            }
        }


        public async Task<PagedResult<StudentQuizDto>> GetStudentQuizAsync(int  studentId,int startIndex, int pageSize )
        {
            var query = _context.StudentQuizzes.Where(
                p=> p.StudentId == studentId);

            var count = await query.CountAsync();

            var quizes = await query.OrderByDescending(q => q.StartedOn)
                .Skip(startIndex)
                .Take(pageSize)
                .Select(q => new StudentQuizDto
                {
                    Id = q.Id,
                    QuizId = q.QuizId,
                    QuizName = q.Quiz.Name,
                    CategoryName = q.Quiz.Category.Name,
                    StartedOn = q.StartedOn,
                    CompletedOn = q.CompletedOn,
                    Status = q.Status,
                    Score = q.Score
                })
                .ToArrayAsync();

            return new PagedResult<StudentQuizDto>(quizes, count);
        }

    }
}
