using BepInEx.Configuration;
using KSP.Game;

namespace AUI
{
    public class SuperEntry<T>
    {
        public string Section = "General";
        public string Key = "";
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                if (Entry != null)
                {
                    Entry.Value = value;
                }
            }
        }
        public string Description = "";
        public AcceptableValueBase AcceptableValues = null;
        public ConfigEntry<T> Entry
        {
            get;
            protected set;
        } = null;

        public SuperEntry(string section, string key, T value, string description, AcceptableValueBase acceptableValues = null)
        {
            Section = section;
            Key = key;
            Value = value;
            Description = description;
            AcceptableValues = acceptableValues;
        }

        public void BindMe(ConfigFile config)
        {
            ConfigDefinition entryDef = new ConfigDefinition(Section, Key);
            ConfigDescription entryDesc = new ConfigDescription(Description, AcceptableValues);
            Entry = config.Bind(entryDef, Value, entryDesc);
        }

        private T _value;
    }

    public class AUIConfigurationSettings : KerbalMonoBehaviour
    {
        public ConfigFile PluginConfig;
        public BepInEx.Logging.ManualLogSource Logger;
        public bool OABPartsPickerCollapseToggleIsEnabled
        {
            get => _enableOABPartsPickerCollapseToggle.Value;
            set => _enableOABPartsPickerCollapseToggle.Value = value;
        }

        public bool OABCenterToolbarsOnPartsPickerCollapseIsEnabled
        {
            get => _enableOABCenterToolbarsOnPartsPickerCollapse.Value;
            set => _enableOABCenterToolbarsOnPartsPickerCollapse.Value = value;
        }

        public void AddSuperEntry<T>(SuperEntry<T> superEntry)
        {
            superEntry.BindMe(PluginConfig);
        }

        public void SetUpConfig()
        {
            Logger.LogInfo("Setting up AUI configuration settings.");

            AddSuperEntry(_enableOABPartsPickerCollapseToggle);
            AddSuperEntry(_enableOABCenterToolbarsOnPartsPickerCollapse);
            // AddSuperEntry(_exampleTestStringList);
        }

        const string _OABSectionString = "Vehicle Assembly Building (VAB/OAB)";
        private SuperEntry<bool> _enableOABPartsPickerCollapseToggle = new SuperEntry<bool>(
            _OABSectionString,
            "Collapsable Parts Picker Button",
            true,
            "A toggle button is added that will expand and collapse the parts picker drawer.");

        private SuperEntry<bool> _enableOABCenterToolbarsOnPartsPickerCollapse = new SuperEntry<bool>(
            _OABSectionString,
            "Auto-center Editor Toolbars When Parts Picker Collapses",
            true,
            "When the parts picker drawer is collapsed, this setting will automatically position the vehicle editing toolbars in the center of the working area. If disabled, the toolbars will remain fixed in place.");

        public SuperEntry<string> _exampleTestStringList = new SuperEntry<string>(
            "Debug Settings",
            "Tests a list of string options",
            "Default",
            "This hopefully has a list of available string values.",
            new AcceptableValueList<string>("Default", "First", "Second", "Third"));

    }
}