using NamelessQuizness.Serialization;

namespace NamelessQuizness.Definitions
{
    public interface IQuestionDef : IDef
    {
        IMessageDef? InputMessage { get; }
        Type? QuestionWorker { get; }
    }
}