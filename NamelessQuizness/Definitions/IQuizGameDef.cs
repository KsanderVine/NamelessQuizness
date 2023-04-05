using NamelessQuizness.Serialization;

namespace NamelessQuizness.Definitions
{
    public interface IQuizGameDef : IDef
    {
        string Title { get; }
        Type QuizGame { get; }
    }
}