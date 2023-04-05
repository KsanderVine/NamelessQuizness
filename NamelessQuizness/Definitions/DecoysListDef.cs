using System.Linq;

namespace NamelessQuizness.Definitions
{
    public class DecoysListDef : IDecoysListDef
    {
        public string DefKey { get; set; } = string.Empty;
        public List<string> Words { get; set; } = new List<string>();
    }
}