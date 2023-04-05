using NamelessQuizness.Definitions;

namespace NamelessQuizness
{
    public interface IQuizGame
    {
        IQuizGameDef QuizGameDef { get; }

        void Play();
    }
}