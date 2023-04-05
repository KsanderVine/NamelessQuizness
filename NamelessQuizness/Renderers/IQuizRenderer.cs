using NamelessQuizness.Workers;

namespace NamelessQuizness.Renderers
{
    public interface IQuizRenderer : IRenderer
    {
        void DrawQuestion(int questionIndex, IQuestionWorker questionWorker);
        void DrawQuestionsCounter(int questionIndex, int totalQuestionsCount);
    }

    [Flags]
    public enum MessageFlags
    {
        None = 0,
        NoWait = 1,
        NoClear = 2
    }
}
