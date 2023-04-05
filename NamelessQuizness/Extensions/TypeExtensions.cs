using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NamelessQuizness.Extensions
{
    public static class TypeExtensions
    {
		public static Type GetFirstGenericArgument(this Type type)
		{
			if (!type.IsGenericType)
			{
				throw new ArgumentException("The {type} needs to be a GenericTypeDefinition", nameof(type));
			}
			return type.GetGenericArguments().ToList().First();
		}

		public static bool HasGenericDefinition(this Type type, Type definition)
		{
			return GetTypeWithGenericDefinition(type, definition) != null;
		}

		public static Type? GetTypeWithGenericDefinition(this Type type, Type definition)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			if (definition == null)
			{
				throw new ArgumentNullException(nameof(definition));
			}
			if (!definition.IsGenericTypeDefinition)
			{
				throw new ArgumentException("The {definition} needs to be a GenericTypeDefinition", nameof(definition));
			}

			if (definition.IsInterface)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type interfaceType in interfaces)
				{
					if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == definition)
					{
						return interfaceType;
					}
				}
			}

			Type? tempType = type;
			while (tempType != null)
			{
				if (tempType.IsGenericType && tempType.GetGenericTypeDefinition() == definition)
				{
					return tempType;
				}
				tempType = tempType.BaseType;
			}

			return null;
		}
	}
}
