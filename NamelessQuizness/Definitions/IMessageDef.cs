using NamelessQuizness.Serialization;

namespace NamelessQuizness.Definitions
{
    public interface IMessageDef : IDef
    {
        string Text { get; }
    }
}