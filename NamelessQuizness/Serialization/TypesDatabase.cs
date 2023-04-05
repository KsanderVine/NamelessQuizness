using NamelessQuizness.Extensions;
using System.Reflection;

namespace NamelessQuizness.Serialization
{
    public static class TypesDatabase
    {
        public static class AttributeUsage<TAttribute> where TAttribute : Attribute
        {
            private static readonly Dictionary<Type, bool> cachedIsTypeHasAttribute = new();

            public static bool HasAttribute (Type type)
            {
                if(!cachedIsTypeHasAttribute.TryGetValue(type, out bool value))
                {
                    value = Attribute.IsDefined(type, typeof(TAttribute));
                    cachedIsTypeHasAttribute.Add(type, value);
                }
                return value;
            }
        }

        private static readonly List<string?> defaultNamespaces;

        private static readonly Dictionary<Type, bool> cachedIsDef = new();
        private static readonly Dictionary<Type, bool> cachedIsList = new();

        private static List<Type> cachedTypes = new();
        public static List<Type> AllTypes
        {
            get
            {
                if (!cachedTypes.Any())
                {
                    cachedTypes = GetAllTypes().ToList();
                }
                return cachedTypes;

                static IEnumerable<Type> GetAllTypes ()
                {
                    foreach (var assembly in AllActiveAssemblies)
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        private static IEnumerable<Assembly> AllActiveAssemblies
        {
            get 
            {
                return AppDomain.CurrentDomain.GetAssemblies().ToList();
            }
        }

        static TypesDatabase ()
        {
            defaultNamespaces = typeof(TypesDatabase)
                .Assembly.GetTypes()
                .Select(t => t.Namespace)
                .Where(t => t != null)
                .Where(t => t!.Contains("NamelessQuizness"))
                .Distinct()
                .ToList();
        }

        public static List<Type> AllSubclasses(this Type baseType)
        {
            var list = new List<Type>();
            foreach (var x in AllTypes.AsParallel())
            {
                if (x.IsSubclassOf(baseType))
                {
                    list.Add(x);
                }
            }

            return list;
        }

        public static IEnumerable<Type> AllLeafSubclasses(this Type baseType)
        {
            foreach (var type in baseType.AllSubclasses())
            {
                if (!type.AllSubclasses().Any())
                {
                    yield return type;
                }
            }

            yield break;
        }

        public static bool HasAttribute <TAttribute> (Type type) where TAttribute : Attribute
        {
            return AttributeUsage<TAttribute>.HasAttribute(type);
        }

        public static bool IsList(Type type)
        {
            if (!cachedIsList.TryGetValue(type, out bool isList))
            {
                isList = type.HasGenericDefinition(typeof(List<>));
                cachedIsList.Add(type, isList);

            }
            return isList;
        }

        public static bool IsList<TType>()
        {
            return IsList(typeof(TType));
        }

        public static bool IsDef(Type type)
        {
            if (!cachedIsDef.TryGetValue(type, out bool isDef))
            {
                isDef = typeof(IDef).IsAssignableFrom(type);
                cachedIsDef.Add(type, isDef);

            }
            return isDef;
        }

        public static bool IsDef<TType>()
        {
            return IsDef(typeof(TType));
        }

        public static Type? GetType(string typeName, string? @namespace = null)
        {
            if(@namespace is null)
            {
                foreach(var defaultNamespace in defaultNamespaces)
                {
                    Type? defaultType = SearchAllAssemblies(typeName, defaultNamespace);

                    if (defaultType != null)
                    {
                        return defaultType;
                    }
                }
            }

            Type? type = SearchAllAssemblies(typeName, @namespace);

            if (type != null)
            {
                return type;
            }

            return Type.GetType(typeName, throwOnError: false, ignoreCase: true);

            static Type? SearchAllAssemblies (string typeName, string? @namespace = null)
            {
                string fullName = @namespace != null ? $"{@namespace}.{typeName}" : typeName;

                foreach (var assembly in AllActiveAssemblies)
                {
                    if (assembly.GetType(fullName, throwOnError: false, ignoreCase: true) is Type type)
                    {
                        return type;
                    }
                }
                return default;
            }
        }
    }
}