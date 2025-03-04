using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorQuiz.Shared.DTO
{
    public record AdminHomeDateDto(int TotalCategories, int TotalStudents, int ApprovedStudents, int TotalQuizes, int ActiveQuizes);
    
    
}
