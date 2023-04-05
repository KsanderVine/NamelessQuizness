using NamelessQuizness.Serialization;
using System.Reflection;

namespace NamelessQuizness.Definitions
{
    public interface IQuizDef : IDef
    {
        string Title { get; }
        Type? QuizProcessor { get; }
        Type? QuizRenderer { get; }
        IQuestionsListDef? QuestionsList { get; }
    }
}