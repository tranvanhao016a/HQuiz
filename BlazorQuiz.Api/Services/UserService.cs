using BlazorQuiz.Api.Data;
using BlazorQuiz.Api.Data.Entities;
using BlazorQuiz.Shared;
using BlazorQuiz.Shared.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorQuiz.Api.Services
{
    public class UserService
    {
        private readonly QuizContext _context;

        public UserService(QuizContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(UserApprovedFilter filter, int startIndex, int pageSize)
        {
            var query = _context.Users.AsQueryable();
            if (filter != UserApprovedFilter.All)
            {
                if (filter == UserApprovedFilter.ApprovedOnly)
                {
                    query = query.Where(u => u.IsApproved);
                }
                else
                {
                    query = query.Where(u => !u.IsApproved);
                }
            }
            var total = await query.CountAsync();
            var users = await query.OrderByDescending(u => u.Id)
                .Skip(startIndex)
                .Take(pageSize)
                .Select(u => new UserDto(u.Id, u.Name, u.Email, u.Phone, u.IsApproved))
                .ToArrayAsync();

            foreach (var user in users)
            {
                Console.WriteLine($"User ID: {user.Id}, Name: {user.Name}");
            }

            return new PagedResult<UserDto>(users, total);
        }

        public async Task<QuizApiResponse> ToggleUserApprovedStatus(int userId)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (dbUser == null)
            {
                return QuizApiResponse.Fail("Not  ");
            }
            else
            {
                dbUser.IsApproved = !dbUser.IsApproved;
                await _context.SaveChangesAsync();
                return QuizApiResponse.Success;
            }
        }
    }
}
