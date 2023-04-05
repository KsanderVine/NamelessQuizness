namespace NamelessQuizness.Serialization
{
    public static class DefDatabase
    {
        private static List<IDef> Defs { get; set; } = new List<IDef>();

        public static IEnumerable<IDef> AllDefs => Defs;

        public static int DefCount => Defs.Count;

        public static void Add(IDef def)
        {
            if (Defs.Exists(d => d.DefKey.Equals(def.DefKey)))
            {
                throw new ArgumentException($"{typeof(IDef).Name} with key {def.DefKey} already exists");
            }
            Defs.Add(def);
        }

        public static IDef? Get(string defKey)
        {
            return Defs.FirstOrDefault(d => d.DefKey.Equals(defKey));
        }

        public static IEnumerable<T> GetAll<T> () where T : IDef
        {
            Type type = typeof(T);
            return AllDefs
                .Where(d => type.IsAssignableFrom(d.GetType()))
                .Select(d => (T)d)
                .ToList();
        } 
    }
}