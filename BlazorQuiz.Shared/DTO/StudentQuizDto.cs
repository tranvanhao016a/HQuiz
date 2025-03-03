using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorQuiz.Shared.DTO
{
 

    public class StudentQuizDto
    {
        public int Id { get; set; }
        public Guid QuizId { get; set; }
        public string QuizName { get; set; }
        public string CategoryName { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Status { get; set; }
        public int Score { get; set; }
    }
}
