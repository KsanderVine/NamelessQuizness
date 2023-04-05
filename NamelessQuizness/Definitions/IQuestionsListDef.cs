using NamelessQuizness.Serialization;

namespace NamelessQuizness.Definitions
{
    public interface IQuestionsListDef : IDef
    {
        List<IQuestionDef> Questions { get; }
    }
}