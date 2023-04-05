namespace NamelessQuizness.Definitions
{
    public class SummaryDecoysListDef : IDecoysListDef
    {
        public string DefKey { get; set; } = string.Empty;
        public List<IDecoysListDef> BaseLists { get; set; } = new List<IDecoysListDef>();
        public List<string> Words
        {
            get
            {
                return GetAllWords().ToList();

                IEnumerable<string> GetAllWords()
                {
                    return BaseLists.SelectMany(decoysList => decoysList.Words);
                }
            }
        }
    }
}