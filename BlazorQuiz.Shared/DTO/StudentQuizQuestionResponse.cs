using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorQuiz.Shared.DTO
{
   public record StudentQuizQuestionResponseDto(
       int StudentQuizId, int QuestionId, int OptionId);

}
