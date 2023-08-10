using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skynet
{
    [Serializable]
    abstract class PreferencesBase
    {
        protected readonly string prefPath = "Preferences";
        protected string serverID;
        protected string fileName = "Pref.json";

        public PreferencesBase(string serverID)
        {
            this.serverID = serverID;
        }

        protected string GetFilePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), prefPath, serverID, fileName);
        }

        public abstract void UpdatePreferences();
    }

    class MainModulePreferences : PreferencesBase
    {
        [Serializable]
        public struct Preference
        {
            public Preference(string serverID, string prefix)
            {
                ServerID = serverID;
                Prefix = prefix;
            }

            public string ServerID { get; private set; }
            public string Prefix { get; private set; }
        }

        public MainModulePreferences(string serverID) : base(serverID)
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), prefPath, serverID)))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), prefPath, serverID));
            }

            fileName = "MainModulePref.json";
            string filePath = GetFilePath();
            if (!File.Exists(filePath))
            {
                preference = new Preference(serverID, "!"); //Default Settings
                JSONUtility.Serialize(filePath, preference);
            }
            else
            {
                preference = JSONUtility.Deserialize<Preference>(filePath);
            }

            UpdatePreferences();
        }

        private Preference preference;

        public string ServerID { get => preference.ServerID; }
        public string Prefix { get => preference.Prefix; }

        public void UpdateServerPreference(Preference newPreference)
        {
            preference = newPreference;
            JSONUtility.Serialize(GetFilePath(), preference);
        }

        public override void UpdatePreferences()
        {
            Program.UpdatePreferences(this);
        }
    }
}
