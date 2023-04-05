using System.Xml;

namespace NamelessQuizness.Serialization
{
    public static class XmlDefsLoader
    {
        public static bool TryLoadAllDefs()
        {
            try
            {
                foreach (var file in GetAllXmlInResourcesFolder())
                {
                    XmlDocument dataDocument = new();
                    dataDocument.Load(file.FullName);

                    if (dataDocument.DocumentElement is XmlElement xRoot && string.Equals(xRoot.Name, "Content"))
                    {
                        LoadObjectsFromRootNode(xRoot);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            try
            {
                DefsReferenceResolver.ResolveAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return true;

            static IEnumerable<FileInfo> GetAllXmlInResourcesFolder()
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                string[] filePaths = Directory.GetFiles(path, "*.xml", new EnumerationOptions
                {
                    MaxRecursionDepth = int.MaxValue,
                    MatchCasing = MatchCasing.CaseInsensitive,
                    RecurseSubdirectories = true
                });

                foreach (var filePath in filePaths)
                {
                    if (File.Exists(filePath))
                    {
                        yield return new FileInfo(filePath);
                    }
                }
            }
        }

        private static void LoadObjectsFromRootNode(XmlElement xRoot)
        {
            foreach (XmlNode xNode in xRoot.ChildNodes)
            {
                if (xNode is not XmlElement xElement)
                {
                    continue;
                }

                string typeName = xElement.Name;

                if (xElement.HasAttribute("Class"))
                {
                    typeName = xElement.GetAttribute("Class");
                }

                Type? type = TypesDatabase.GetType(typeName);

                if (type == null)
                {
                    throw new Exception("Type not found");
                }

                if (type.IsAbstract || type.IsInterface)
                {
                    throw new Exception("Type can not be abstract or interface");
                }

                if (!TypesDatabase.IsDef(type))
                {
                    throw new Exception("Incorrect IDef implementation");
                }

                var method = XmlToObject.GetXmlToObjectMethod(type);
                var value = method(xElement);

                if (value is IDef def)
                {
                    DefDatabase.Add(def);
                }
            }
        }
    }
}