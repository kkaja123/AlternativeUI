using BepInEx.Configuration;
using KSP.Game;

namespace AUI
{
    /// <summary>Represents basic info for a ConfigEntry.</summary>
    public class ConfigEntryInfoAttribute<T> : System.Attribute
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
    public class AcceptableConfigRangeAttribute<T> : System.Attribute where T : System.IComparable
    {
        private AcceptableValueRange<T> _range;
        public AcceptableConfigRangeAttribute(T min, T max)
        {
            _range = new AcceptableValueRange<T>(min, max);
        }

        public AcceptableValueRange<T> GetRange()
        {
            return _range;
        }
    }

    public class AcceptableConfigListAttribute<T> : System.Attribute where T : System.IEquatable<T>
    {
        private AcceptableValueList<T> _list;
        public AcceptableConfigListAttribute(params T[] acceptableValues)
        {
            _list = new AcceptableValueList<T>(acceptableValues);
        }

        public AcceptableValueList<T> GetList()
        {
            return _list;
        }
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

        public void SetUpConfig()
        {
            _enableOABPartsPickerCollapseToggle = PluginConfig.Bind(
                _OABSectionString,  // Section name
                "Enable toggle button to collapse the parts picker drawer",  // Configuration key
                true,  // Default value
                "A toggle button is added that will expand and collapse the parts picker drawer.");  // Description
            Logger.LogDebug(_enableOABPartsPickerCollapseToggle.Value);

            _enableOABCenterToolbarsOnPartsPickerCollapse = PluginConfig.Bind(
                _OABSectionString,  // Section name
                "TestHeader",  // Configuration key
                true,  // Default value
                "This is just a test config option.");  // Description
            Logger.LogDebug(_enableOABCenterToolbarsOnPartsPickerCollapse.Value);
        }

        public void AddEntryWithAttribute<T>(ConfigEntry<T> entry)
        {
            bool isComparable = entry.Value is System.IComparable;
            bool isEquatable = entry.Value is System.IEquatable<T>;
            System.Attribute[] attributes = System.Attribute.GetCustomAttributes(entry.GetType());
            AcceptableValueBase acceptableValues = null;
            foreach (System.Attribute attr in attributes)
            {
                if (attr is ConfigEntryInfoAttribute<T>)
                {

                }

                if (isComparable)
                {
                    System.Type rangeType = typeof(T);
                    acceptableValues = GetEntryRangeFromAttribute(attr, rangeType);
                }

                if (isEquatable)
                {

                }
            }
        }
        public AcceptableValueBase GetEntryRangeFromAttribute(System.Attribute attribute, System.Type type)
        {
            AcceptableValueBase valueRange = null;
            System.Type test = typeof(AcceptableConfigRangeAttribute<int>);
            test.GetType();
            if (attribute is AcceptableConfigRangeAttribute<bool>)
            {
                AcceptableConfigRangeAttribute<T> rangeAttr = attribute as AcceptableConfigRangeAttribute<T>;
                valueRange = rangeAttr.GetRange();
            }
            return valueRange;
        }

        public AcceptableValueBase GetEntryListFromAttribute<T>(System.Attribute attribute) where T : System.IEquatable<T>
        {
            AcceptableValueBase valueList = null;
            if (attribute is AcceptableConfigListAttribute<T>)
            {
                AcceptableConfigListAttribute<T> listAttr = attribute as AcceptableConfigListAttribute<T>;
                valueList = listAttr.GetList();
            }
            return valueList;
        }

        const string _OABSectionString = "Vehicle Assembly Building (VAB/OAB)";
        [ConfigEntryInfo<bool>(
            _OABSectionString,
            "Enable toggle button to collapse the parts picker drawer",
            true,
            Description = "A toggle button is added that will expand and collapse the parts picker drawer.")]
        private ConfigEntry<bool> _enableOABPartsPickerCollapseToggle;

        [ConfigEntryInfo<bool>(
            _OABSectionString,
            "Keep vehicle toolbars centered in work area when parts picker drawer is collapsed",
            true,
            Description = "When the parts picker drawer is collapsed, this setting will automatically position the vehicle editing toolbars in the center of the working area.")]
        private ConfigEntry<bool> _enableOABCenterToolbarsOnPartsPickerCollapse;

        [ConfigEntryInfo<string>(
            _OABSectionString,
            "Tests a list of string options",
            "Default",
            Description = "This hopefully has a list of available string values.")]
        [AcceptableConfigList<string>("Default", "First", "Second", "Third")]
        private ConfigEntry<string> _testStringList;

    }
}