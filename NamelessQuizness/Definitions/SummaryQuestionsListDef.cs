using System.Linq;

namespace NamelessQuizness.Definitions
{
    public class SummaryQuestionsListDef : IQuestionsListDef
    {
        public string DefKey { get; set; } = string.Empty;
        public List<IQuestionsListDef> BaseLists { get; set; } = new List<IQuestionsListDef>();
        public List<IQuestionDef> Questions
        {
            get
            {
                return GetAllQuestions().ToList();

                IEnumerable<IQuestionDef> GetAllQuestions ()
                {
                    return BaseLists.SelectMany(questionsList => questionsList.Questions);
                }
            }
        }
    }
}