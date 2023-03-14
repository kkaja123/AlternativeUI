using System;
using BepInEx;
using BepInEx.Configuration;
using KSP.Messages;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;

namespace AUI
{
    [BepInPlugin("com.kkaja123." + MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class AlternativeUIPlugin : BaseSpaceWarpPlugin
    {
        public AUIConfigurationSettings ConfigSettings;
        public bool EnablePartsPickerToggleButton
        {
            get => _partsPickerToggleButtonIsEnabled;
            set => SetActivePartsPickerToggleButton(value);
        }

        new protected BepInEx.Logging.ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("AUI");

        public KSP.OAB.ObjectAssemblyBuilderHUD OABHUD
        {
            get;
            private set;
        }

        public KSP.OAB.AssemblyPartsPicker OABPartsPicker
        {
            get;
            private set;
        }

        public PartsPickerUIConfiguration DefaultPartsPickerUIConfiguration
        {
            get => _defaultPartsPickerUIConfiguration;
            protected set => _defaultPartsPickerUIConfiguration = value;  // TODO: probably set this to public once a copy operation has been implemented.
        }
        public PartsPickerUIConfiguration AlternativePartsPickerUIConfiguration
        {
            get => _alternativePartsPickerUIConfiguration;
            protected set => _alternativePartsPickerUIConfiguration = value;
        }
        public PartsPickerUIConfiguration CollapsedPartsPickerUIConfiguration
        {
            get => _collapsedPartsPickerUIConfiguration;
            protected set => _collapsedPartsPickerUIConfiguration = value;
        }

        public OABToolbarsUIConfiguration DefaultOABToolbarsUIConfiguration
        {
            get => _defaultOABToolbarsUIConfiguration;
            protected set => _defaultOABToolbarsUIConfiguration = value;  // TODO: probably set this to public once a copy operation has been implemented.
        }
        public OABToolbarsUIConfiguration CollapsedOABToolbarsUIConfiguration
        {
            get => _collapsedOABToolbarsUIConfiguration;
            protected set => _collapsedOABToolbarsUIConfiguration = value;
        }

        private void Awake()
        {
            ConfigSettings = new AUIConfigurationSettings();
            ConfigSettings.PluginConfig = Config;  // Delegate plugin settings to other class.
            ConfigSettings.Logger = Logger;
            ConfigSettings.SetUpConfig();
        }

        public override void OnInitialized()
        {
            base.OnInitialized();

            Game.Messages.Subscribe<OABLoadedMessage>(new Action<MessageCenterMessage>(this.OnOABLoaded));
            Game.Messages.Subscribe<OABUnloadedMessage>(new Action<MessageCenterMessage>(this.OnOABUnloaded));
        }

        public void Start()
        {
            Logger.LogInfo("AUI.Start()");

            // AUI Manager setup
            GameObject AUIManager = new GameObject("AUIManager");
            AUIManager.transform.SetParent(gameObject.transform);

            // OAB Parts Picker setup
            GameObject config = new GameObject("DefaultPartsPickerUIConfig");
            config.transform.SetParent(AUIManager.transform);
            _defaultPartsPickerUIConfiguration = config.AddComponent<PartsPickerUIConfiguration>() as PartsPickerUIConfiguration;

            config = new GameObject("AltPartsPickerUIConfig");
            config.transform.SetParent(AUIManager.transform);
            _alternativePartsPickerUIConfiguration = config.AddComponent<PartsPickerUIConfiguration>() as PartsPickerUIConfiguration;

            config = new GameObject("CollapsedPartsPickerUIConfig");
            config.transform.SetParent(AUIManager.transform);
            _collapsedPartsPickerUIConfiguration = config.AddComponent<PartsPickerUIConfiguration>() as PartsPickerUIConfiguration;

            // OAB Toolbar setup
            config = new GameObject("DefaultOABToolbarsUIConfig");
            config.transform.SetParent(AUIManager.transform);
            _defaultOABToolbarsUIConfiguration = config.AddComponent<OABToolbarsUIConfiguration>() as OABToolbarsUIConfiguration;

            config = new GameObject("CollapsedOABToolbarsUIConfig");
            config.transform.SetParent(AUIManager.transform);
            _collapsedOABToolbarsUIConfiguration = config.AddComponent<OABToolbarsUIConfiguration>() as OABToolbarsUIConfiguration;
        }

        public void LateUpdate()
        {
            if (IsGameShuttingDown)
            {
                return;
            }

            if (_uiConfigurationsAreInitialized && _pendingPartsPickerExpandUpdate)
            {
                if (!_partsPickerToggleButtonIsEnabled)
                {
                    // Return to default UI config
                    DefaultPartsPickerUIConfiguration.ApplyUIConfiguration();
                    DefaultOABToolbarsUIConfiguration.ApplyUIConfiguration();
                }
                else if (Game.OAB.Current.OABHUD.GetCurrentPartsPicker().expandCollapseToggle.isOn)
                {
                    AlternativePartsPickerUIConfiguration.ApplyUIConfiguration();
                    DefaultOABToolbarsUIConfiguration.ApplyUIConfiguration();
                }
                else
                {
                    CollapsedPartsPickerUIConfiguration.ApplyUIConfiguration();
                    CollapsedOABToolbarsUIConfiguration.ApplyUIConfiguration();
                }

                _pendingPartsPickerExpandUpdate = false;
            }
        }

        public void DebugFunction()
        {
            Logger.LogDebug("AUI.DebugFunction(): Start");

            Logger.LogDebug($"AUI.DebugFunction(): Game = {Game}");
            Logger.LogDebug($"AUI.DebugFunction(): Game.OAB = {Game.OAB}");
            Logger.LogDebug($"AUI.DebugFunction(): Game.OAB.Current = {Game.OAB.Current}");
            if (Game.OAB.Current != null)
            {
                Logger.LogDebug($"AUI.DebugFunction(): Game.OAB.Current.ActiveBuilderVariant = {Game.OAB.Current.ActiveBuilderVariant}");

                Logger.LogDebug($"AUI.DebugFunction(): Game.OAB.Current.OABHUD = {Game.OAB.Current.OABHUD}");
                Logger.LogDebug($"AUI.DebugFunction(): Game.OAB.Current.OABHUD.GetCurrentPartsPicker() = {Game.OAB.Current.OABHUD.GetCurrentPartsPicker()}");
            }

            Logger.LogDebug("AUI.DebugFunction(): End");
        }

        public void ForceToggleButtonListener()
        {
            OABPartsPicker = OABHUD.GetCurrentPartsPicker();
            OABPartsPicker.expandCollapseToggle.onValueChanged.AddListener(this.OnPartsPickerToggled);
        }

        public void SetActivePartsPickerToggleButton(bool enableToggleButton)
        {
            _partsPickerToggleButtonIsEnabled = enableToggleButton;
            if (!_uiConfigurationsAreInitialized)
            {
                return;
            }

            if (enableToggleButton == Game.OAB.Current.OABHUD.GetCurrentPartsPicker().expandCollapseToggle.IsActive())
            {
                // Toggle button already matches the target active state. Nothing to do.
                return;
            }

            _pendingPartsPickerExpandUpdate = true;

            if (enableToggleButton)
            {
                Logger.LogDebug($"Enabling the OAB Parts Picker Toggle button.");
            }
            else
            {
                Logger.LogDebug("Disabling the OAB Parts Picker Toggle button.");
            }
        }

        private void OnOABLoaded(MessageCenterMessage msg)
        {
            Logger.LogDebug($"AUI.OnOABLoaded: The OAB has been loaded!");
            KSP.OAB.ObjectAssemblyUIEvents eventsUI = Game.OAB.Current.eventsUI;
            eventsUI.OnInitialVABUILoaded += new Action(OnVABUIReady);
        }

        private void OnVABUIReady()
        {
            Logger.LogWarning("AUI.OnVABUIReady: OAB UI Ready message received");
            OABHUD = Game.OAB.Current.OABHUD;
            if (OABHUD != null)
            {
                OABPartsPicker = OABHUD.GetCurrentPartsPicker();
                OABPartsPicker.expandCollapseToggle.onValueChanged.AddListener(this.OnPartsPickerToggled);

                if (OABHUD.builder.ActiveBuilderVariant == KSP.OAB.OABVariant.VAB)
                {
                    Logger.LogDebug($"AUI.OnVABUIReady: The Vehicle Assembly Building (VAB) variant has been loaded");
                }
                else if (OABHUD.builder.ActiveBuilderVariant == KSP.OAB.OABVariant.BAE)
                {
                    Logger.LogDebug($"AUI.OnVABUIReady: The Base Assembly Editor (BAE) variant has been loaded");
                }

                // Move the orientation (ortho view) cube to the toolbar widget and position it relative to the toggle orientation button
                OABPartsPicker.viewCubeRT.SetParent(OABHUD.toolbar.transform);
                RectTransform orientationToggleTransform = OABHUD.toolbar.transform.Find("GRP-Toggle Orientation") as RectTransform;
                OABPartsPicker.viewCubeRT.anchorMin = orientationToggleTransform.anchorMin;
                OABPartsPicker.viewCubeRT.anchorMax = orientationToggleTransform.anchorMax;
                OABPartsPicker.viewCubeRT.pivot = orientationToggleTransform.pivot;
                OABPartsPicker.viewCubeRT.anchoredPosition = new Vector2(orientationToggleTransform.anchoredPosition.x - 95, 36);

                if (!_uiConfigurationsAreInitialized)
                {
                    // Initialize the configurations to the standard UI layout.
                    DefaultPartsPickerUIConfiguration.RefreshConfigurationFromUI();
                    SetUpDefaultPartsPickerUIConfig();

                    DefaultOABToolbarsUIConfiguration.RefreshConfigurationFromUI();
                    SetUpDefaultOABToolbarsUIConfig();

                    AlternativePartsPickerUIConfiguration.RefreshConfigurationFromUI();
                    SetUpAltPartsPickerUIConfig();

                    CollapsedPartsPickerUIConfiguration.RefreshConfigurationFromUI();
                    SetUpCollapsedPartsPickerUIConfig();

                    CollapsedOABToolbarsUIConfiguration.RefreshConfigurationFromUI();
                    SetUpCollapsedOABToolbarsUIConfig();

                    // Now that the configurations are saved, keep them cached to avoid having to reset them on every VAB load. Hopefully, no one decides to change the standard UI layout over the course of the game 🙄.
                    _uiConfigurationsAreInitialized = true;
                }

                if (EnablePartsPickerToggleButton)
                {
                    SetActivePartsPickerToggleButton(true);
                }
            }
            else
            {
                Logger.LogWarning("AUI.OnVABUIReady: The OAB UI is expected to be ready, but the HUD is null.");
            }
        }

        private void OnOABUnloaded(MessageCenterMessage msg)
        {
            Logger.LogDebug($"AUI.OnOABUnloaded");
            KSP.OAB.ObjectAssemblyUIEvents eventsUI = Game.OAB.Current.eventsUI;
            eventsUI.OnInitialVABUILoaded -= new Action(OnVABUIReady);
        }

        private void OnPartsPickerToggled(bool isExpanded)
        {
            // TODO: Fix auto-adjustments of toolbars.
            Logger.LogDebug($"AUI.OnPartsPickerToggled: isExpanded = {isExpanded}");

            _pendingPartsPickerExpandUpdate = true;  // The OAB HUD elements have to be updated later, since the AssemblyPartsPicker tries to automatically adjust them.
        }

        private void SetUpDefaultPartsPickerUIConfig()
        {
            // Fix the root to better behave with widget_PartsPicker
            DefaultPartsPickerUIConfiguration.RootWidget.anchorMin = new Vector2(0, 0);
            DefaultPartsPickerUIConfiguration.RootWidget.anchorMax = new Vector2(1, 1);
            DefaultPartsPickerUIConfiguration.RootWidget.anchoredPosition = new Vector2(0, 10);  // Position moves up because widget_PartsPicker seems to have a bad offset value.
            DefaultPartsPickerUIConfiguration.RootWidget.sizeDelta = new Vector2(0, 0);
            DefaultPartsPickerUIConfiguration.RootWidget.pivot = new Vector2(0, 1);

            DefaultPartsPickerUIConfiguration.ApplyUIConfiguration();  // Since this is supposed to be the default, apply these changes right away.
        }

        private void SetUpAltPartsPickerUIConfig()
        {
            // Set up the normally-hidden parts picker toggle button.
            AlternativePartsPickerUIConfiguration.ExpandToggleTransform.localScale = _defaultPartsPickerToggleScale;
            AlternativePartsPickerUIConfiguration.ExpandToggleTransform.anchoredPosition = new Vector2(22, -18);
            AlternativePartsPickerUIConfiguration.ExpandToggleTransform.sizeDelta = new Vector2(48, 48);
            AlternativePartsPickerUIConfiguration.ExpandToggleTransform.gameObject.SetActive(true);

            // Set up the header to make room for the addition of the toggle button.
            AlternativePartsPickerUIConfiguration.HeaderTransform.sizeDelta = new Vector2(-40, 30);  // Adds room for the toggle button in top-left of parts picker window.
            AlternativePartsPickerUIConfiguration.HeaderTransform.pivot = new Vector2(1, 1);  // Right-aligns the header.
            AlternativePartsPickerUIConfiguration.HeaderTransform.anchoredPosition = new Vector2(0, -2);  // Provides some margin on the top edge.
        }

        private void SetUpCollapsedPartsPickerUIConfig()
        {
            CollapsedPartsPickerUIConfiguration.ExpandToggleTransform.localScale = AlternativePartsPickerUIConfiguration.ExpandToggleTransform.localScale;
            CollapsedPartsPickerUIConfiguration.ExpandToggleTransform.anchoredPosition = AlternativePartsPickerUIConfiguration.ExpandToggleTransform.anchoredPosition;
            CollapsedPartsPickerUIConfiguration.ExpandToggleTransform.sizeDelta = AlternativePartsPickerUIConfiguration.ExpandToggleTransform.sizeDelta;
            CollapsedPartsPickerUIConfiguration.ExpandToggleTransform.gameObject.SetActive(true);

            float panelWidth = 40f;

            // Shrink the parts picker width-wise
            CollapsedPartsPickerUIConfiguration.RootWidget.anchorMin = new Vector2(0, 1);  // Top-left corner
            CollapsedPartsPickerUIConfiguration.RootWidget.anchorMax = new Vector2(0, 1);  // Top-left corner
            CollapsedPartsPickerUIConfiguration.RootWidget.sizeDelta = new Vector2(panelWidth, 570);  // TODO: base the vertical size off of the preferred min height of the categories vertical layer group + room for the toggle button
            CollapsedPartsPickerUIConfiguration.RootWidget.pivot = new Vector2(0, 1);  // Top-left corner
            CollapsedPartsPickerUIConfiguration.RootWidget.anchoredPosition = new Vector2(0, 0);  // Puts it a little bit above the parent widget, which matches the height
            RectTransform originalRoot = AlternativePartsPickerUIConfiguration.RootWidget;
            float distanceToMatchOriginalTop = (1 - originalRoot.pivot.y) * originalRoot.rect.height + originalRoot.anchoredPosition.y;  // Derive the anchor vertical offset needed to match the same header position as the original
            Logger.LogDebug($"SetUpCollapsedPartsPickerUIConfig(): Calculated dY as {distanceToMatchOriginalTop}");
            CollapsedPartsPickerUIConfiguration.RootWidget.anchoredPosition = new Vector2(0, distanceToMatchOriginalTop);  // Puts it a little bit above the parent widget, which matches the height

            // Moves the body to be snug with the BG panel and the toggle button and collapsed in size without obscuring the category buttons.
            CollapsedPartsPickerUIConfiguration.BodyTransform.pivot = new Vector2(0.5f, 1f);  // Place pivot top-center
            CollapsedPartsPickerUIConfiguration.BodyTransform.anchoredPosition = new Vector2(0, -38);  // Move body down to make room for the toggle button
            CollapsedPartsPickerUIConfiguration.BodyTransform.sizeDelta = new Vector2(0, -38);  // Trim off the bottom that got pushed off from the anchor offset

            // Resize background panel to 100% of root
            CollapsedPartsPickerUIConfiguration.BackgroundPanelTransform.anchorMin = new Vector2(0, 0);
            CollapsedPartsPickerUIConfiguration.BackgroundPanelTransform.anchorMax = new Vector2(1, 1);
            CollapsedPartsPickerUIConfiguration.BackgroundPanelTransform.anchoredPosition = new Vector2(0, 0);
            CollapsedPartsPickerUIConfiguration.BackgroundPanelTransform.sizeDelta = new Vector2(0, 0);

            CollapsedPartsPickerUIConfiguration.HeaderTransform.gameObject.SetActive(false);
            CollapsedPartsPickerUIConfiguration.SearchFilterSortTransform.gameObject.SetActive(false);
            CollapsedPartsPickerUIConfiguration.TrashBinHitAreaTransform.gameObject.SetActive(false);
            CollapsedPartsPickerUIConfiguration.TrashBinTransform.gameObject.SetActive(false);
        }

        private void SetUpDefaultOABToolbarsUIConfig()
        {
            // By setting up a right-aligned toolbar UI area, the collapse logic just needs to adjust size delta to get everything re-positioned.
            DefaultOABToolbarsUIConfiguration.RootWidget.anchorMin = new Vector2(1, 0);
            DefaultOABToolbarsUIConfiguration.RootWidget.anchorMax = new Vector2(1, 1);
            DefaultOABToolbarsUIConfiguration.RootWidget.anchoredPosition = new Vector2(-455, -15);  // Large x offset is for all of the buttons on the right and the "staging" UI elements
            DefaultOABToolbarsUIConfiguration.RootWidget.pivot = new Vector2(1f, 0.5f);

            // Calculate distance between the root's pivot anchor and the right side of the parts picker.
            // We'll have to apply the default configurations right now so we can calculate the distance from live objects.
            DefaultPartsPickerUIConfiguration.ApplyUIConfiguration();
            DefaultOABToolbarsUIConfiguration.ApplyUIConfiguration();
            if (OABPartsPicker == null)
            {
                OABPartsPicker = Game.OAB.Current.OABHUD.GetCurrentPartsPicker();
            }

            RectTransform ppRoot = OABPartsPicker.transform as RectTransform;
            RectTransform myRoot = Game.OAB.Current.OABHUD.toolbar.transform as RectTransform;
            float margin = 16f;
            float deltaX = myRoot.localPosition.x - (ppRoot.localPosition.x + ppRoot.rect.xMax + margin);  // Uses local position, since absolute position is subject to scaling from the HUD canvas. rect is in local unit scale, so it's technically accurate too. You just have to hope that both roots have the same parent.
            // The borders of the UI elements need to be pixel perfect to prevent artifacts. That means deltaX needs to be a integer multiple of two.
            int pixelDeltaX = (int)deltaX;
            pixelDeltaX = pixelDeltaX / 2;  // Get the multiple of two by using integer division to truncate any remaining decimal part.
            pixelDeltaX = pixelDeltaX * 2;  // Return to base value
            DefaultOABToolbarsUIConfiguration.RootWidget.sizeDelta = new Vector2(pixelDeltaX, -62);  // X size gets the left side of the widget near to the right side of parts picker. Y size gives some margin to top and bottom of view area.

            DefaultOABToolbarsUIConfiguration.UndoRedoTransform.anchorMin = new Vector2(0, 1);
            DefaultOABToolbarsUIConfiguration.UndoRedoTransform.anchorMax = new Vector2(0, 1);
            DefaultOABToolbarsUIConfiguration.UndoRedoTransform.anchoredPosition = new Vector2(0, 0);
            DefaultOABToolbarsUIConfiguration.UndoRedoTransform.pivot = new Vector2(0, 1);
            DefaultOABToolbarsUIConfiguration.ApplyUIConfiguration();  // Since this is supposed to be the default, apply these changes right away.
        }

        private void SetUpCollapsedOABToolbarsUIConfig()
        {
            // Calculate distance between the root's pivot anchor and the right side of the parts picker.
            RectTransform ppRoot = OABPartsPicker.transform as RectTransform;
            RectTransform myRoot = Game.OAB.Current.OABHUD.toolbar.transform as RectTransform;
            float margin = 16f;
            float deltaX = myRoot.localPosition.x - (ppRoot.localPosition.x + CollapsedPartsPickerUIConfiguration.RootWidget.rect.xMax + margin);
            // The borders of the UI elements need to be pixel perfect to prevent artifacts. That means deltaX needs to be a integer multiple of two.
            int pixelDeltaX = (int)deltaX;
            pixelDeltaX = pixelDeltaX / 2;  // Get the multiple of two by using integer division to truncate any remaining decimal part.
            pixelDeltaX = pixelDeltaX * 2;  // Return to base value
            CollapsedOABToolbarsUIConfiguration.RootWidget.sizeDelta = DefaultOABToolbarsUIConfiguration.RootWidget.sizeDelta with { x = pixelDeltaX };  // X size gets the left side of the widget near to the right side of parts picker.
        }


        protected bool _uiConfigurationsAreInitialized = false;

        private bool _partsPickerToggleButtonIsEnabled = true;
        private PartsPickerUIConfiguration _defaultPartsPickerUIConfiguration;
        private PartsPickerUIConfiguration _alternativePartsPickerUIConfiguration;
        private PartsPickerUIConfiguration _collapsedPartsPickerUIConfiguration;
        private Vector3 _defaultPartsPickerTogglePosition = new Vector3(-893, 439, 0);  // Move up to the header row.
        private Vector3 _defaultPartsPickerToggleScale = new Vector3(0.8f, 0.8f, 1.0f);  // Fits nicer next to the header when smaller than default.
        private bool _pendingPartsPickerExpandUpdate = false;

        private OABToolbarsUIConfiguration _defaultOABToolbarsUIConfiguration;
        private OABToolbarsUIConfiguration _collapsedOABToolbarsUIConfiguration;
    }
}
