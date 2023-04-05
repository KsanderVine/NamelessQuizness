using System.Reflection;

namespace NamelessQuizness.Serialization
{
    public static class DefsReferenceResolver
    {
        private interface IRequest
        {
            void Resolve();
        }

        private class PropertyRequest : IRequest
        {
            public readonly object targetObject;
            public readonly PropertyInfo propertyInfo;
            public readonly string defKeyReferenced;

            public PropertyRequest(object targetObject, PropertyInfo propertyInfo, string defKeyReferenced)
            {
                this.targetObject = targetObject;
                this.propertyInfo = propertyInfo;
                this.defKeyReferenced = defKeyReferenced;
            }

            public void Resolve()
            {
                if (DefDatabase.Get(defKeyReferenced) is IDef def)
                {
                    propertyInfo.SetValue(targetObject, def);
                }
                else
                {
                    throw new Exception($"Can not find any def with key \"{defKeyReferenced}\"");
                }
            }
        }

        private class ListRequest<T> : IRequest
        {
            public readonly List<T> targetList;
            public readonly List<string> defKeysReferenced;

            public ListRequest(List<T> targetList)
            {
                this.targetList = targetList;
                defKeysReferenced = new List<string>();
            }

            public void AddDefReferenced (string defKeyReferenced)
            {
                defKeysReferenced.Add(defKeyReferenced);
            }

            public void Resolve()
            {
                foreach(var defKey in defKeysReferenced)
                {
                    if(DefDatabase.Get(defKey) is IDef def)
                    {
                        targetList.Add((T)(object)def);
                    } 
                    else
                    {
                        throw new Exception($"Can not find any def with key \"{defKey}\"");
                    }
                }
                return;
            }
        }

        private static readonly List<IRequest> allRequests = new();
        private static readonly List<PropertyRequest> propertyRequests = new();
        private static readonly Dictionary<object, IRequest> listRequests = new();

        public static void AddPropertyRequest(object targetObject, PropertyInfo propertyInfo, string defKeyReferenced)
        {
            var request = new PropertyRequest(targetObject, propertyInfo, defKeyReferenced);
            propertyRequests.Add(request);
            allRequests.Add(request);
        }

        public static void AddListRequest<T> (List<T> targetList, string defKeyReferenced)
        {
            if(!listRequests.TryGetValue(targetList, out var list))
            {
                list = new ListRequest<T>(targetList);
                listRequests.Add(targetList, list);
                allRequests.Add(list);
            }

            if(list is ListRequest<T> listRequest)
            {
                listRequest.AddDefReferenced(defKeyReferenced);
            }
        }

        public static void ResolveAll ()
        {
            foreach(var request in allRequests)
            {
                try
                {
                    request.Resolve();
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}