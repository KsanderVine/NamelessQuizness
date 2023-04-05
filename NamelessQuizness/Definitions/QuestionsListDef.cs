namespace NamelessQuizness.Definitions
{
    public class QuestionsListDef : IQuestionsListDef
    {
        public string DefKey { get; set; } = string.Empty;
        public List<IQuestionDef> Questions { get; set; } = new List<IQuestionDef>();
    }
}