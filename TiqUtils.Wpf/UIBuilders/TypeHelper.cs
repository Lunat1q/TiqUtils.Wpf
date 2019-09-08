using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TiqUtils.Wpf.UIBuilders
{
    internal static class TypeHelper
    {
        private static readonly Dictionary<string, Type> CachedTypes = new Dictionary<string, Type>();
        public static T CreateClassOfTypeByName<T>(string name, params object[] parameters)
        {
            if (!CachedTypes.ContainsKey(name))
            {
                var mainAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                if (!FindTypeInAssembly(name, mainAssembly))
                {
                    foreach (var assemblyName in mainAssembly.GetReferencedAssemblies())
                    {
                        var assembly = Assembly.Load(assemblyName);
                        if (FindTypeInAssembly(name, assembly))
                        {
                            break;
                        }
                    }
                }
            }

            if (!CachedTypes.TryGetValue(name, out var typeToCreate))
            {
                throw new ArgumentException($"Type name: {name} could not be resolved.");
            }

            return (T)Activator.CreateInstance(typeToCreate, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, parameters, null);
        }

        private static bool FindTypeInAssembly(string name, Assembly assembly)
        {
            var type = assembly.GetTypes().FirstOrDefault(x => x.Name.Equals(name));
            if (type != null)
            {
                CachedTypes.Add(name, type);
                return true;
            }

            return false;
        }
    }
}
