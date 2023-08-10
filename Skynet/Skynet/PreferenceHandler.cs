using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Skynet
{
    static class PreferenceHandler
    {
        private static List<PreferencesBase> preferences = new List<PreferencesBase>();

        public static List<PreferencesBase> Preferences { get => preferences; }

        public static IEnumerable<Type> GetEnumerableOfType<T>() where T : class
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(assembly => assembly.GetTypes())
                       .Where(type => type.IsSubclassOf(typeof(T)));
        }

        public static PreferencesBase GetPreference(Type prefType)
        {
            return preferences.Find((item) => item.GetType().IsAssignableFrom(prefType))!;
        }
    }
}
