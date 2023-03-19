using BepInEx.Configuration;
using KSP.Game;
using System;

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

    /// <summary>Represents basic info for a ConfigEntry.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigEntryInfoAttribute<T> : Attribute
    {
        public string Section;
        public string Key;
        public T Value;
        public string Description;
        public ConfigEntryInfoAttribute(string section, string key, T value)
        {
            Section = section;
            Key = key;
            Value = value;
            Description = "";
        }
    }

    // TODO Create an interface that has a common method to get an AcceptableValueBase from both types of attributes.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public abstract class AcceptableValueAttribute : Attribute
    {
        public abstract AcceptableValueBase GetAcceptableValues();
    }

    public class AcceptableConfigRangeAttribute<T> : AcceptableValueAttribute where T : IComparable
    {
        private AcceptableValueRange<T> _range;

        public AcceptableConfigRangeAttribute(T min, T max)
        {
            _range = new AcceptableValueRange<T>(min, max);
        }

        public override AcceptableValueBase GetAcceptableValues()
        {
            return _range;
        }
    }

    public class AcceptableConfigListAttribute<T> : AcceptableValueAttribute where T : IEquatable<T>
    {
        private AcceptableValueList<T> _list;

        public AcceptableConfigListAttribute(params T[] acceptableValues)
        {
            _list = new AcceptableValueList<T>(acceptableValues);
        }

        public override AcceptableValueBase GetAcceptableValues()
        {
            return _list;
        }
    }

    [Serializable]
    public class InvalidConfigurationAttributesException : Exception
    {
        public InvalidConfigurationAttributesException() { }
        public InvalidConfigurationAttributesException(string message) : base(message) { }
        public InvalidConfigurationAttributesException(string message, Exception inner) : base(message, inner) { }
        protected InvalidConfigurationAttributesException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
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

        // public void AddEntryWithAttribute<T>(SuperEntry<T> superEntry)
        // {
        //     Logger.LogError($"AddEntryWithAttribute: Start ({superEntry.ToString()})");
        //     // Attribute[] attributes = Attribute.GetCustomAttributes(superEntry.GetType());
        //     ConfigEntryInfoAttribute<T>[] attributes = (ConfigEntryInfoAttribute<T>[])superEntry.GetType().GetCustomAttributes(typeof(ConfigEntryInfoAttribute<T>), false);
        //     Logger.LogWarning($"AddEntryWithAttribute: Num ConfigEntryInfo Attributes: {attributes.Count()}");

        //     ConfigEntryInfoAttribute<T> entryInfo = null;
        //     AcceptableValueBase acceptableValues = null;

        //     foreach (Attribute attr in attributes)
        //     {
        //         Logger.LogWarning($"AddEntryWithAttribute:   Attribute ({attr})");
        //         if (attr is ConfigEntryInfoAttribute<T>)
        //         {
        //             if (entryInfo == null)
        //             {
        //                 entryInfo = attr as ConfigEntryInfoAttribute<T>;
        //                 Logger.LogInfo($"AddEntryWithAttribute:     attribute is ConfigEntryInfoAttribute ({entryInfo})");
        //             }
        //             else
        //             {
        //                 throw new InvalidConfigurationAttributesException("Multiple ConfigEntryInfo attributes detected. Only one ConfigEntryInfo attribute is allowed per ConfigEntry.");
        //             }
        //         }

        //         if (attr is AcceptableValueAttribute)
        //         {
        //             if (acceptableValues == null)
        //             {
        //                 acceptableValues = (attr as AcceptableValueAttribute).GetAcceptableValues();
        //                 Logger.LogInfo($"AddEntryWithAttribute:     attribute is AcceptableValueAttribute ({acceptableValues})");
        //             }
        //             else
        //             {
        //                 throw new InvalidConfigurationAttributesException("Multiple AcceptableValue attributes detected. Only one AcceptableValue attribute is allowed per ConfigEntry.");
        //             }
        //         }
        //     }

        //     if (entryInfo == null)
        //     {
        //         if (acceptableValues != null)
        //         {
        //             throw new InvalidConfigurationAttributesException("Detected a AcceptableValue attribute without a ConfigEntryInfo attribute. A ConfigEntryInfo attribute is required in order to use an AcceptableValue attribute.");
        //         }
        //         else
        //         {
        //             // No config-related attribute found. This entry must be manually bound later.
        //             Logger.LogWarning($"AddEntryWithAttribute<T>: Was provided with a ConfigEntry item without a ConfigEntryInfo attribute set! Make sure that this is intentional and that the ConfigEntry item will be bound to the ConfigFile at a later point! {superEntry.ToString()}");
        //         }
        //     }
        //     else
        //     {
        //         // A config info attribute is attached to this entry. Automatically bind it to the config file.
        //         ConfigDefinition entryDef = new ConfigDefinition(entryInfo.Section, entryInfo.Key);
        //         ConfigDescription entryDesc = new ConfigDescription(entryInfo.Description, acceptableValues);
        //         superEntry.Entry = PluginConfig.Bind(entryDef, entryInfo.Value, entryDesc);
        //         Logger.LogInfo($"AddEntryWithAttribute:     Binding to config file ({superEntry.Entry})");
        //     }
        //     Logger.LogWarning($"AddEntryWithAttribute: End");
        // }

        public void SetUpConfig()
        {
            Logger.LogInfo("Setting up AUI configuration settings.");
            // _enableOABPartsPickerCollapseToggle = PluginConfig.Bind(
            //     _OABSectionString,  // Section name
            //     "Enable toggle button to collapse the parts picker drawer",  // Configuration key
            //     true,  // Default value
            //     "A toggle button is added that will expand and collapse the parts picker drawer.");  // Description
            // Logger.LogDebug(_enableOABPartsPickerCollapseToggle.Value);

            // _enableOABCenterToolbarsOnPartsPickerCollapse = PluginConfig.Bind(
            //     _OABSectionString,  // Section name
            //     "TestHeader",  // Configuration key
            //     true,  // Default value
            //     "This is just a test config option.");  // Description
            // Logger.LogDebug(_enableOABCenterToolbarsOnPartsPickerCollapse.Value);
            AddSuperEntry(_enableOABPartsPickerCollapseToggle);
            AddSuperEntry(_enableOABCenterToolbarsOnPartsPickerCollapse);
            AddSuperEntry(_exampleTestStringList);
        }

        const string _OABSectionString = "Vehicle Assembly Building (VAB/OAB)";
        [ConfigEntryInfo<bool>(
            _OABSectionString,
            "Enable toggle button to collapse the parts picker drawer",
            true,
            Description = "A toggle button is added that will expand and collapse the parts picker drawer.")]
        private SuperEntry<bool> _enableOABPartsPickerCollapseToggle = new SuperEntry<bool>(
            _OABSectionString,
            "Enable toggle button to collapse the parts picker drawer",
            true,
            "A toggle button is added that will expand and collapse the parts picker drawer.");

        [ConfigEntryInfo<bool>(
            _OABSectionString,
            "Keep vehicle toolbars centered in work area when parts picker drawer is collapsed",
            true,
            Description = "When the parts picker drawer is collapsed, this setting will automatically position the vehicle editing toolbars in the center of the working area.")]
        private SuperEntry<bool> _enableOABCenterToolbarsOnPartsPickerCollapse = new SuperEntry<bool>(
            _OABSectionString,
            "Keep vehicle toolbars centered in work area when parts picker drawer is collapsed",
            true,
            "When the parts picker drawer is collapsed, this setting will automatically position the vehicle editing toolbars in the center of the working area.");

        [ConfigEntryInfo<string>(
            "Debug Settings",
            "Tests a list of string options",
            "Default",
            Description = "This hopefully has a list of available string values.")]
        [AcceptableConfigList<string>("Default", "First", "Second", "Third")]
        public SuperEntry<string> _exampleTestStringList = new SuperEntry<string>(
            "Debug Settings",
            "Tests a list of string options",
            "Default",
            "This hopefully has a list of available string values.",
            new AcceptableValueList<string>("Default", "First", "Second", "Third"));

    }
}