using NamelessQuizness.Serialization;

namespace NamelessQuizness.Definitions
{
    public interface IDecoysListDef : IDef
    {
        List<string> Words { get; }
    }
}