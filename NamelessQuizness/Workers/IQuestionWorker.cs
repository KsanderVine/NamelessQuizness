using NamelessQuizness.Definitions;

namespace NamelessQuizness.Workers
{
    public interface IQuestionWorker
    {
        IQuestionDef QuestionDef { get; }

        string GetDescription();
        IEnumerable<string>? GetAnswersFormatted();
        string GetExpectedAnswer();
        bool IsAnswer(string? answer);
    }
}
