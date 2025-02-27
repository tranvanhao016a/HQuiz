using BlazorQuiz.Shared.DTO;

namespace BlazorQuiz.Web
{
    public class QuizState
    {
        public int StudentQuizId { get; private set; }
        public QuizListDto? Quiz { get; private set; }

        public void StartQuiz(QuizListDto? quiz, int studentQuizId)
        
         =>  (Quiz, StudentQuizId) = (quiz, studentQuizId);

    }
}
