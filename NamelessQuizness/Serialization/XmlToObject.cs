using NamelessQuizness.Definitions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace NamelessQuizness.Serialization
{
    public static class XmlToObject
    {
        private static readonly Dictionary<Type, Func<XmlNode, object?>> cachedObjectFromXmlMethods = new();
        private static readonly Dictionary<Type, Func<XmlNode, object?>> cachedListFromXmlMethods = new();

        public static Func<XmlNode, object?> GetXmlToObjectMethod(Type objectType)
        {
            if (!cachedObjectFromXmlMethods.TryGetValue(objectType, out var value))
            {
                var methodName = nameof(ObjectFromXmlReflection);
                var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                MethodInfo? method = typeof(XmlToObject)?.GetMethod(methodName, bindingFlags);

                if (method is null)
                    throw new Exception("Failed to get method");

                var genericMethod = method.MakeGenericMethod(objectType);

                value = (Func<XmlNode, object?>)Delegate.CreateDelegate(typeof(Func<XmlNode, object?>), genericMethod);
                cachedObjectFromXmlMethods.Add(objectType, value);
            }
            return value;
        }

        private static object? ObjectFromXmlReflection<T>(XmlNode xmlRoot)
        {
            return ObjectFromXml<T>(xmlRoot);
        }

        private static T? ObjectFromXml<T>(XmlNode xmlRoot)
        {
            Type type = typeof(T);

            if (xmlRoot.ChildNodes.Count == 1 && xmlRoot.FirstChild?.NodeType == XmlNodeType.Text)
            {
                if(typeof(T).IsEnum)
                {
                    if(Enum.TryParse(typeof(T), ReplaceNewLines(xmlRoot.InnerText), out object? result))
                    {
                        return (T)result!;
                    }
                    else
                    {
                        return default;
                    }
                }

                try
                {
                    return ParseHelper.FromString<T>(ReplaceNewLines(xmlRoot.InnerText));
                }
                catch (Exception exception)
                {
                    throw new Exception($"Exception parsing {xmlRoot.OuterXml} to type {type}: {exception}");
                }
            }

            if (TypesDatabase.HasAttribute<FlagsAttribute>(type))
            {
                if (ListFromXml<T>(xmlRoot) is List<T> list)
                {
                    int flagsValue = 0;
                    foreach (T item in list)
                    {
                        flagsValue |= Convert.ToInt32(item);
                    }
                    return (T)(object)flagsValue;
                }
                return default;
            }

            if (TypesDatabase.IsList(type))
            {
                if (!cachedListFromXmlMethods.TryGetValue(type, out var value))
                {
                    var methodName = nameof(ListFromXmlReflection);
                    var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                    MethodInfo? method = typeof(XmlToObject)?.GetMethod(methodName, bindingFlags);
                    Type[] genericArguments = type.GetGenericArguments();

                    if (method is null)
                        throw new Exception("Failed to get method");

                    var genericMethod = method.MakeGenericMethod(genericArguments);

                    value = (Func<XmlNode, object?>)Delegate.CreateDelegate(typeof(Func<XmlNode, object?>), genericMethod);
                    cachedListFromXmlMethods.Add(type, value);
                }
                return (T?)value(xmlRoot);
            }

            if (!xmlRoot.HasChildNodes)
            {
                if (type == typeof(string))
                {
                    return (T)(object)"";
                }
            }

            var instance = Activator.CreateInstance<T>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            var childNodes = xmlRoot.ChildNodes;
            foreach(XmlNode xmlNode in childNodes)
            {
                if (xmlNode is XmlComment)
                {
                    continue;
                }

                foreach(var property in properties)
                {
                    if(xmlNode.Name.Equals(property.Name))
                    {
                        Type? propertyType = property.PropertyType;

                        if (xmlNode.Attributes?["Class"] is XmlAttribute classAttribute)
                        {
                            propertyType = TypesDatabase.GetType(classAttribute.Value);
                        }

                        if (propertyType != null)
                        {
                            if (TypesDatabase.IsDef(propertyType))
                            {
                                DefsReferenceResolver.AddPropertyRequest(
                                    instance!, property, 
                                    ReplaceNewLines(xmlNode.InnerText));
                            }
                            else
                            {
                                var innerObjectMethod = GetXmlToObjectMethod(propertyType);
                                var value = innerObjectMethod(xmlNode);

                                property.SetValue(instance, value);
                            }
                        }

                        break;
                    }
                }
            }

            return instance;
        }

        private static object? ListFromXmlReflection<T>(XmlNode xmlRoot)
        {
            return ListFromXml<T>(xmlRoot);
        }

        private static List<T>? ListFromXml<T>(XmlNode xmlRoot)
        {
            List<T> list = new();

            bool isDef = TypesDatabase.IsDef(typeof(T));

            var childNodes = xmlRoot.ChildNodes;
            foreach(XmlNode childNode in childNodes)
            {
                if(!ValidateListNode(childNode))
                {
                    continue;
                }

                if (isDef)
                {
                    DefsReferenceResolver.AddListRequest(list, ReplaceNewLines(childNode.InnerText));
                }
                else
                {
                    string? typeName = typeof(T).FullName;

                    if (childNode.Attributes?["Class"] is XmlAttribute classAttribute)
                    {
                        typeName = classAttribute.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(typeName))
                    {
                        if (TypesDatabase.GetType(typeName) is Type listObjectType)
                        {
                            var objectMethod = GetXmlToObjectMethod(listObjectType);
                            var listObject = objectMethod(childNode);

                            if (listObject != null)
                            {
                                list.Add((T)listObject);
                            }
                            else
                            {
                                throw new Exception($"Failed to create list object with type {typeName}");
                            }
                        }
                    }
                }
            }

            return list;

            static bool ValidateListNode(XmlNode listEntryNode)
            {
                return 
                    listEntryNode is not XmlComment && 
                    listEntryNode is not XmlText && 
                    listEntryNode.Name == "li";
            }
        }

        private static string ReplaceNewLines(string str)
        {
            return str
                .Trim()
                .Replace("\\n", "\n")
                .Replace("\r\n", "\n")
                .Replace("      ","")
                .Replace("\\t","");
        }
    }
}