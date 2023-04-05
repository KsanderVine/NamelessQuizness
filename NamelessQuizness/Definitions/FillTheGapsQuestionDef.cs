using NamelessQuizness.Workers;

namespace NamelessQuizness.Definitions
{
    public class FillTheGapsQuestionDef : IQuestionDef
    {
        public string DefKey { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int GapsCount { get; set; } = 1;
        public int DecoysCount { get; set; } = 1;
        public IDecoysListDef? DecoysListDef { get; set; }
        public IMessageDef? ConditionMessage { get; set; }
        public IMessageDef? InputMessage { get; set; }
        public Type? QuestionWorker { get; set; } = typeof(FillTheGapsQuestionWorker);
    }
}