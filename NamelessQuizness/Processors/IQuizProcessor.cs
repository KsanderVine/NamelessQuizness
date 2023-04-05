using NamelessQuizness.Definitions;

namespace NamelessQuizness.Processors
{
    public interface IQuizProcessor
    {
        void StartQuiz(IQuizDef quizDef);
    }
}
