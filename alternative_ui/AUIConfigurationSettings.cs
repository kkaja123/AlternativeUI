using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace AUI
{
    public abstract class SuperEntryBase
    {
        public abstract void Flush();
    }

    public class SuperEntry<T> : SuperEntryBase
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
            Flush();
        }

        public override void Flush()
        {
            if (Entry != null)
            {
                _value = Entry.Value;
            }
        }

        private T _value;
    }

    public class AUIConfigurationSettings
    {
        public ConfigFile PluginConfig;
        public ManualLogSource Logger;
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

        public AUIConfigurationSettings(ConfigFile config, ManualLogSource logger)
        {
            PluginConfig = config;
            Logger = logger;
        }

        public void AddSuperEntry<T>(SuperEntry<T> superEntry)
        {
            superEntry.BindMe(PluginConfig);
            _entries.Add(superEntry.Key, superEntry);
        }

        public void SetUpConfig()
        {
            Logger.LogInfo("Setting up AUI configuration settings.");

            AddSuperEntry(_enableOABPartsPickerCollapseToggle);
            AddSuperEntry(_enableOABCenterToolbarsOnPartsPickerCollapse);
            // AddSuperEntry(_exampleTestStringList);
            PluginConfig.SettingChanged += HandlePluginSettingsChanged;
        }

        private void HandlePluginSettingsChanged(object sender, SettingChangedEventArgs eventArgs)
        {
            try
            {
                _entries[eventArgs.ChangedSetting.Definition.Key].Flush();
            }
            catch (KeyNotFoundException)
            {
                // The changed setting does not seem to be one from this object.
            }
        }

        private Dictionary<string, SuperEntryBase> _entries = new Dictionary<string, SuperEntryBase>();
        public const string _OABSectionString = "Vehicle Assembly Building (VAB/OAB)";
        public readonly SuperEntry<bool> _enableOABPartsPickerCollapseToggle = new SuperEntry<bool>(
            _OABSectionString,
            "Collapsable Parts Picker Button",
            true,
            "A toggle button is added that will expand and collapse the parts picker drawer.");

        public readonly SuperEntry<bool> _enableOABCenterToolbarsOnPartsPickerCollapse = new SuperEntry<bool>(
            _OABSectionString,
            "Auto-center Editor Toolbars When Parts Picker Collapses",
            true,
            "When the parts picker drawer is collapsed, this setting will automatically position the vehicle editing toolbars in the center of the working area. If disabled, the toolbars will remain fixed in place.");

        public readonly SuperEntry<string> _exampleTestStringList = new SuperEntry<string>(
            "Debug Settings",
            "Tests a list of string options",
            "Default",
            "This hopefully has a list of available string values.",
            new AcceptableValueList<string>("Default", "First", "Second", "Third"));

    }
}