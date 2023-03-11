using System;
using BepInEx;
using KSP.Messages;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;

namespace AUI
{
    [BepInPlugin("com.arcticninja73.AUI", "AlternativeUI (AUI)", "0.1.0")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class AlternativeUIPlugin : BaseSpaceWarpPlugin
    {
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

            if (_pendingOABToolbarsUpdate)
            {
                if (Game.OAB.Current.OABHUD.GetCurrentPartsPicker().expandCollapseToggle.isOn)
                {
                    DefaultOABToolbarsUIConfiguration.ApplyUIConfiguration();
                }
                else
                {
                    CollapsedOABToolbarsUIConfiguration.ApplyUIConfiguration();
                }

                _pendingOABToolbarsUpdate = false;
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

        public void SetActivePartsPickerToggleButton(bool enableToggleButton = true)
        {
            if (enableToggleButton == Game.OAB.Current.OABHUD.GetCurrentPartsPicker().expandCollapseToggle.IsActive())
            {
                // Toggle button already matches the target active state. Nothing to do.
                return;
            }

            // Force the toggle to be on, since the toggle is being enabled or disabled. (i.e. revert to a known state after a setting change)
            // This will trigger event handlers for the Toggle, including AUI's.
            Game.OAB.Current.OABHUD.GetCurrentPartsPicker().expandCollapseToggle.isOn = true;

            if (enableToggleButton)
            {
                Logger.LogDebug($"Enabling the OAB Parts Picker Toggle button.");

                // Force AUI config on
                AlternativePartsPickerUIConfiguration.ApplyUIConfiguration();
                _partsPickerToggleButtonIsEnabled = true;
            }
            else
            {
                Logger.LogDebug("Disabling the OAB Parts Picker Toggle button.");

                // Force AUI config off
                DefaultPartsPickerUIConfiguration.ApplyUIConfiguration();
                _partsPickerToggleButtonIsEnabled = false;
            }
        }

        public void SetPartsPickerToggleButtonPosition(float xPos, float yPos, float zPos)
        {
            Logger.LogDebug($"Moving the Toggle button to ({xPos}, {yPos}, {zPos})");
            UnityEngine.Transform PPToggleTransform = Game.OAB.Current.OABHUD.GetCurrentPartsPicker().transform.Find("mask_PartsPicker")?.Find("GRP-ExpandCollapse");
            PPToggleTransform.position = new Vector3(xPos, yPos, zPos);
        }

        public void SetPartsPickerToggleButtonScale(float xScale, float yScale)
        {
            Logger.LogDebug($"Scaling the Toggle button to ({xScale}, {yScale})");
            UnityEngine.Transform PPToggleTransform = Game.OAB.Current.OABHUD.GetCurrentPartsPicker().transform.Find("mask_PartsPicker")?.Find("GRP-ExpandCollapse");
            PPToggleTransform.localScale = new Vector3(xScale, yScale, PPToggleTransform.localScale.z);
        }

        public void SetPartsPickerToggleButtonScale(float scalingFactor)
        {
            Logger.LogDebug($"Scaling the Toggle button to {scalingFactor * 100}% of original size.)");
            UnityEngine.Transform PPToggleTransform = Game.OAB.Current.OABHUD.GetCurrentPartsPicker().transform.Find("mask_PartsPicker")?.Find("GRP-ExpandCollapse");
            PPToggleTransform.localScale = new Vector3(scalingFactor, scalingFactor, PPToggleTransform.localScale.z);
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

                if (!_uiConfigurationsAreInitialized)
                {
                    // Initialize the configurations to the standard UI layout.
                    DefaultPartsPickerUIConfiguration.RefreshConfigurationFromUI();
                    DefaultOABToolbarsUIConfiguration.RefreshConfigurationFromUI();

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

            _pendingOABToolbarsUpdate = true;  // OAB toolbars have to be updated later, since the AssemblyPartsPicker tries to automatically adjust them, but fails.
            if (isExpanded)
            {
                AlternativePartsPickerUIConfiguration.ApplyUIConfiguration();
            }
            else
            {
                CollapsedPartsPickerUIConfiguration.ApplyUIConfiguration();
            }
        }

        private void SetUpAltPartsPickerUIConfig()
        {
            // Set up the normally-hidden parts picker toggle button.
            AlternativePartsPickerUIConfiguration.ExpandToggleTransform.localScale = _defaultPartsPickerToggleScale;
            AlternativePartsPickerUIConfiguration.ExpandToggleTransform.position = _defaultPartsPickerTogglePosition;
            AlternativePartsPickerUIConfiguration.ExpandToggleTransform.gameObject.SetActive(true);

            // Set up the header to make room for the addition of the toggle button.
            AlternativePartsPickerUIConfiguration.HeaderTransform.sizeDelta = new Vector2(-40, 25);  // Adds room for the toggle button in top-left of parts picker window.
            AlternativePartsPickerUIConfiguration.HeaderTransform.position = new Vector3(-663, 471);  // Right-aligns the header.
        }

        private void SetUpCollapsedPartsPickerUIConfig()
        {
            CollapsedPartsPickerUIConfiguration.ExpandToggleTransform.localScale = _defaultPartsPickerToggleScale;
            CollapsedPartsPickerUIConfiguration.ExpandToggleTransform.position = _defaultPartsPickerTogglePosition;
            CollapsedPartsPickerUIConfiguration.ExpandToggleTransform.gameObject.SetActive(true);

            // Moves the body to be snug with the BG panel and the toggle button and collapsed in size without obscuring the category buttons.
            CollapsedPartsPickerUIConfiguration.BodyTransform.position = new Vector3(-895.0518f, 185.8370f, -4900.0f);
            CollapsedPartsPickerUIConfiguration.BodyTransform.sizeDelta = new Vector2(-415, -492);
            CollapsedPartsPickerUIConfiguration.BodyTransform.pivot = new Vector2(0, .5f);

            CollapsedPartsPickerUIConfiguration.HeaderTransform.gameObject.SetActive(false);
            CollapsedPartsPickerUIConfiguration.TrashBinHitAreaTransform.gameObject.SetActive(false);

            CollapsedPartsPickerUIConfiguration.BackgroundPanelTransform.sizeDelta = new Vector2(40, 570);
        }

        private void SetUpCollapsedOABToolbarsUIConfig()
        {
            CollapsedOABToolbarsUIConfiguration.RootWidget.position = DefaultOABToolbarsUIConfiguration.RootWidget.position with {x =
                DefaultOABToolbarsUIConfiguration.RootWidget.position.x - AlternativePartsPickerUIConfiguration.RootWidget.rect.width / 2};
        }

        protected bool _uiConfigurationsAreInitialized = false;

        private bool _partsPickerToggleButtonIsEnabled = true;
        private PartsPickerUIConfiguration _defaultPartsPickerUIConfiguration;
        private PartsPickerUIConfiguration _alternativePartsPickerUIConfiguration;
        private PartsPickerUIConfiguration _collapsedPartsPickerUIConfiguration;
        private Vector3 _defaultPartsPickerTogglePosition = new Vector3(-893, 439, 0);  // Move up to the header row.
        private Vector3 _defaultPartsPickerToggleScale = new Vector3(0.8f, 0.8f, 1.0f);  // Fits nicer next to the header when smaller than default.

        private OABToolbarsUIConfiguration _defaultOABToolbarsUIConfiguration;
        private OABToolbarsUIConfiguration _collapsedOABToolbarsUIConfiguration;
        private bool _pendingOABToolbarsUpdate = false;
    }
}
