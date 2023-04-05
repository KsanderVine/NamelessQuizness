using NamelessQuizness.Workers;

namespace NamelessQuizness.Definitions
{
    public class TypingQuestionDef : IQuestionDef
    {
        public string DefKey { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public IMessageDef? InputMessage { get; set; }
        public Type? QuestionWorker { get; set; } = typeof(TypingQuestionWorker);
        public List<string> AcceptedAnswers { get; set; } = new List<string>();
    }
}