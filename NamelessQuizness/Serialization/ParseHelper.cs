using System.Globalization;

namespace NamelessQuizness.Serialization
{
    public static class ParseHelper
    {
        public static class Parser<T>
        {
            public static Func<string, T?>? Method { get; set; }
        }

        private static readonly Dictionary<Type, Func<string, object?>> parsers;

        static ParseHelper ()
        {
            parsers = new Dictionary<Type, Func<string, object?>>();

            Register(ParseInt);
            Register(ParseString);
            Register(ParseBool);
            Register(ParseLong);
            Register(ParseFloat);
            Register(ParseDouble);
            Register(ParseType);
            Register(ParseShort);

            static void Register<T>(Func<string, T?> method)
            {
                Parser<T>.Method = method;
                parsers.Add(typeof(T), (string str) => method(str));
            }
        }

        public static string ParseString(string str)
        {
            return str.Replace("\\n", "\n");
        }

        public static int ParseInt(string str)
        {
            return int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default;
        }

        public static float ParseFloat(string str)
        {
            return float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default;
        }

        public static bool ParseBool(string str)
        {
            return bool.TryParse(str, out var result) && result;
        }

        public static long ParseLong(string str)
        {
            return long.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default;
        }

        public static double ParseDouble(string str)
        {
            return double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default;
        }

        public static short ParseShort(string str)
        {
            return short.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : default;
        }

        public static Type? ParseType(string str)
        {
            if (str == "null" || str == "Null")
            {
                return null;
            }

            if (TypesDatabase.GetType(str) is Type type)
            {
                return type;
            }

            throw new NullReferenceException("Could not find a type named " + str);
        }

        public static T? FromString <T> (string str)
        {
            if (Parser<T>.Method is Func<string,T?> parser)
            {
                return parser(str);
            }
            return default;
        }
    }
}