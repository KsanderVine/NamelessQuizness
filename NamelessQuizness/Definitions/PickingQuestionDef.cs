using NamelessQuizness.Workers;

namespace NamelessQuizness.Definitions
{
    public class PickingQuestionDef : IQuestionDef
    {
        public class PickableAnswer
        {
            public string Text { get; set; } = string.Empty;
            public bool IsCorrectAnswer { get; set; }
        }

        public string DefKey { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public IMessageDef? InputMessage { get; set; }
        public Type? QuestionWorker { get; set; } = typeof(PickingQuestionDef);
        public List<PickableAnswer> Answers { get; set; } = new List<PickableAnswer>();
    }
}