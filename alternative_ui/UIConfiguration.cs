// These UI configuration classes define a set of transforms that represent the
// UI layout for the respective group of elements. Helper functions are
// available to extract the current UI layout or to apply the set of transforms
// to the game. Changing values here will not change anything in the game until
// they are applied.

using UnityEngine;
using KSP.Game;

namespace AUI
{
    public interface IUIConfiguration
    {
        // Not yet sure how to implement the root object. Maybe a path to the object? protected UnityEngine.Transform Root;

        public abstract bool ApplyUIConfiguration();

        public abstract bool ApplyUIConfiguration(Transform UIRoot);

        public abstract bool RefreshConfigurationFromUI();

        public abstract bool RefreshConfigurationFromUI(Transform UIRoot);
    }

    public class OABUIConfiguration : IUIConfiguration
    {
        public RectTransform RootWidget;
        public PartsPickerUIConfiguration PartsPickerUIConfig { get; set; }

        public OABUIConfiguration(OABUIConfiguration other)
        {
            // PartsPickerUIConfig = new PartsPickerUIConfiguration(other.PartsPickerUIConfig);
        }

        public bool ApplyUIConfiguration()
        {

            return true;
        }

        public bool ApplyUIConfiguration(Transform UIRoot)
        {

            return true;
        }

        public bool RefreshConfigurationFromUI()
        {

            return true;
        }

        public bool RefreshConfigurationFromUI(Transform UIRoot)
        {

            return true;
        }
    }

    public class PartsPickerUIConfiguration : KerbalMonoBehaviour, IUIConfiguration
    {
        public RectTransform RootWidget;
        public RectTransform ExpandToggleTransform;
        public RectTransform HeaderTransform;
        public RectTransform SearchFilterSortTransform;
        public RectTransform BodyTransform;
        public RectTransform BackgroundPanelTransform;
        public RectTransform TrashBinTransform;
        public RectTransform TrashBinHitAreaTransform;

        public void Start()
        {
            RootWidget = new GameObject("RootWidget", typeof(RectTransform)).transform as RectTransform;
            RootWidget.SetParent(gameObject.transform);

            ExpandToggleTransform = new GameObject("ExpandToggleTransform", typeof(RectTransform)).transform as RectTransform;
            ExpandToggleTransform.SetParent(RootWidget.transform);

            HeaderTransform = new GameObject("HeaderTransform", typeof(RectTransform)).transform as RectTransform;
            HeaderTransform.SetParent(RootWidget.transform);

            SearchFilterSortTransform = new GameObject("SearchFilterSortTransform", typeof(RectTransform)).transform as RectTransform;
            SearchFilterSortTransform.SetParent(RootWidget.transform);

            BodyTransform = new GameObject("BodyTransform", typeof(RectTransform)).transform as RectTransform;
            BodyTransform.SetParent(RootWidget.transform);

            BackgroundPanelTransform = new GameObject("BackgroundPanelTransform", typeof(RectTransform)).transform as RectTransform;
            BackgroundPanelTransform.SetParent(RootWidget.transform);

            TrashBinTransform = new GameObject("TrashBinTransform", typeof(RectTransform)).transform as RectTransform;
            TrashBinTransform.SetParent(RootWidget.transform);

            TrashBinHitAreaTransform = new GameObject("TrashBinHitAreaTransform", typeof(RectTransform)).transform as RectTransform;
            TrashBinHitAreaTransform.SetParent(RootWidget.transform);
        }

        public bool ApplyUIConfiguration()
        {
            Transform UIRoot = Game.OAB.Current.OABHUD.GetCurrentPartsPicker().transform.Find("mask_PartsPicker")?.transform;

            if (UIRoot == null)
            {
                return false;
            }

            return ApplyUIConfiguration(UIRoot);
        }

        public bool ApplyUIConfiguration(Transform UIRoot)
        {
            if (UIRoot == false)
            {
                return false;
            }

            RectTransform gameRootWidget = UIRoot as RectTransform;
            RectTransform gameExpandToggleTransform = UIRoot.Find("GRP-ExpandCollapse") as RectTransform;
            RectTransform gameHeaderTransform = UIRoot.Find("GRP-Header") as RectTransform;
            RectTransform gameSearchFilterSortTransform = UIRoot.Find("GRP-Search-Filter-Sort") as RectTransform;
            RectTransform gameBodyTransform = UIRoot.Find("GRP-Body") as RectTransform;
            RectTransform gameBackgroundPanelTransform = UIRoot.Find("BG-panel") as RectTransform;
            RectTransform gameTrashBinTransform = UIRoot.Find("GRP-DragDrop-TrashBin") as RectTransform;
            RectTransform gameTrashBinHitAreaTransform = UIRoot.Find("TrashBin-HitArea") as RectTransform;

            Utilities.CopyRectTransformProperties(gameRootWidget, RootWidget);
            Utilities.CopyRectTransformProperties(gameExpandToggleTransform, ExpandToggleTransform);
            Utilities.CopyRectTransformProperties(gameHeaderTransform, HeaderTransform);
            Utilities.CopyRectTransformProperties(gameSearchFilterSortTransform, SearchFilterSortTransform);
            Utilities.CopyRectTransformProperties(gameBodyTransform, BodyTransform);
            Utilities.CopyRectTransformProperties(gameBackgroundPanelTransform, BackgroundPanelTransform);
            Utilities.CopyRectTransformProperties(gameTrashBinTransform, TrashBinTransform);
            Utilities.CopyRectTransformProperties(gameTrashBinHitAreaTransform, TrashBinHitAreaTransform);

            return true;
        }

        public bool RefreshConfigurationFromUI()
        {
            Transform UIRoot = Game.OAB.Current.OABHUD.GetCurrentPartsPicker().transform.Find("mask_PartsPicker")?.transform;

            if (UIRoot == null)
            {
                return false;
            }

            return RefreshConfigurationFromUI(UIRoot);
        }

        public bool RefreshConfigurationFromUI(Transform UIRoot)
        {
            if (UIRoot == false)
            {
                return false;
            }

            RectTransform gameRootWidget = UIRoot as RectTransform;
            RectTransform gameExpandToggleTransform = UIRoot.Find("GRP-ExpandCollapse") as RectTransform;
            RectTransform gameHeaderTransform = UIRoot.Find("GRP-Header") as RectTransform;
            RectTransform gameSearchFilterSortTransform = UIRoot.Find("GRP-Search-Filter-Sort") as RectTransform;
            RectTransform gameBodyTransform = UIRoot.Find("GRP-Body") as RectTransform;
            RectTransform gameBackgroundPanelTransform = UIRoot.Find("BG-panel") as RectTransform;
            RectTransform gameTrashBinTransform = UIRoot.Find("GRP-DragDrop-TrashBin") as RectTransform;
            RectTransform gameTrashBinHitAreaTransform = UIRoot.Find("TrashBin-HitArea") as RectTransform;

            Utilities.CopyRectTransformProperties(RootWidget, gameRootWidget);
            Utilities.CopyRectTransformProperties(ExpandToggleTransform, gameExpandToggleTransform);
            Utilities.CopyRectTransformProperties(HeaderTransform, gameHeaderTransform);
            Utilities.CopyRectTransformProperties(SearchFilterSortTransform, gameSearchFilterSortTransform);
            Utilities.CopyRectTransformProperties(BodyTransform, gameBodyTransform);
            Utilities.CopyRectTransformProperties(BackgroundPanelTransform, gameBackgroundPanelTransform);
            Utilities.CopyRectTransformProperties(TrashBinTransform, gameTrashBinTransform);
            Utilities.CopyRectTransformProperties(TrashBinHitAreaTransform, gameTrashBinHitAreaTransform);

            return true;
        }
    }

    public class OABToolbarsUIConfiguration : KerbalMonoBehaviour, IUIConfiguration
    {
        public RectTransform RootWidget;
        public RectTransform UndoRedoTransform;
        public RectTransform ToggleOrientationTransform;
        public RectTransform InfoOverlaysTransform;
        public RectTransform SymmetrySnapTransform;
        public RectTransform ToolsTransform;
        public RectTransform OrientationCubeTransform;

        public void Start()
        {
            RootWidget = new GameObject("RootWidget", typeof(RectTransform)).transform as RectTransform;
            RootWidget.SetParent(gameObject.transform);

            UndoRedoTransform = new GameObject("UndoRedoTransform", typeof(RectTransform)).transform as RectTransform;
            UndoRedoTransform.SetParent(RootWidget.transform);

            ToggleOrientationTransform = new GameObject("ToggleOrientationTransform", typeof(RectTransform)).transform as RectTransform;
            ToggleOrientationTransform.SetParent(RootWidget.transform);

            InfoOverlaysTransform = new GameObject("InfoOverlaysTransform", typeof(RectTransform)).transform as RectTransform;
            InfoOverlaysTransform.SetParent(RootWidget.transform);

            SymmetrySnapTransform = new GameObject("SymmetrySnapTransform", typeof(RectTransform)).transform as RectTransform;
            SymmetrySnapTransform.SetParent(RootWidget.transform);

            ToolsTransform = new GameObject("ToolsTransform", typeof(RectTransform)).transform as RectTransform;
            ToolsTransform.SetParent(RootWidget.transform);

            OrientationCubeTransform = new GameObject("OrientationCubeTransform", typeof(RectTransform)).transform as RectTransform;
            OrientationCubeTransform.SetParent(RootWidget.transform);
        }

        public bool ApplyUIConfiguration()
        {
            Transform UIRoot = Game.OAB.Current.OABHUD.toolbar.transform;

            if (UIRoot == null)
            {
                return false;
            }

            return ApplyUIConfiguration(UIRoot);
        }

        public bool ApplyUIConfiguration(Transform UIRoot)
        {
            if (UIRoot == false)
            {
                return false;
            }

            RectTransform gameRootWidget = UIRoot as RectTransform;
            RectTransform gameUndoRedoTransformTransform = UIRoot.Find("GRP-Undo-Redo") as RectTransform;
            RectTransform gameToggleOrientationTransform = UIRoot.Find("GRP-Toggle Orientation") as RectTransform;
            RectTransform gameInfoOverlaysTransform = UIRoot.Find("GRP-Info-Overlays") as RectTransform;
            RectTransform gameSymmetrySnapTransform = UIRoot.Find("GRP-Symmetry-Snap") as RectTransform;
            RectTransform gameToolsTransform = UIRoot.Find("GRP-Tools") as RectTransform;
            RectTransform gameOrientationCubeTransform = UIRoot.Find("orientation_Cube") as RectTransform;

            Utilities.CopyRectTransformProperties(gameRootWidget, RootWidget);
            Utilities.CopyRectTransformProperties(gameUndoRedoTransformTransform, UndoRedoTransform);
            Utilities.CopyRectTransformProperties(gameToggleOrientationTransform, ToggleOrientationTransform);
            Utilities.CopyRectTransformProperties(gameInfoOverlaysTransform, InfoOverlaysTransform);
            Utilities.CopyRectTransformProperties(gameSymmetrySnapTransform, SymmetrySnapTransform);
            Utilities.CopyRectTransformProperties(gameToolsTransform, ToolsTransform);
            if (gameOrientationCubeTransform != null)  // This class expects the orientation cube to have been moved to the toolbars group
            {
                Utilities.CopyRectTransformProperties(gameOrientationCubeTransform, OrientationCubeTransform);
            }

            return true;
        }

        public bool RefreshConfigurationFromUI()
        {
            Transform UIRoot = Game.OAB.Current.OABHUD.toolbar.transform;

            if (UIRoot == null)
            {
                return false;
            }

            return RefreshConfigurationFromUI(UIRoot);
        }

        public bool RefreshConfigurationFromUI(Transform UIRoot)
        {
            if (UIRoot == false)
            {
                return false;
            }

            RectTransform gameRootWidget = UIRoot as RectTransform;
            RectTransform gameUndoRedoTransformTransform = UIRoot.Find("GRP-Undo-Redo") as RectTransform;
            RectTransform gameToggleOrientationTransform = UIRoot.Find("GRP-Toggle Orientation") as RectTransform;
            RectTransform gameInfoOverlaysTransform = UIRoot.Find("GRP-Info-Overlays") as RectTransform;
            RectTransform gameSymmetrySnapTransform = UIRoot.Find("GRP-Symmetry-Snap") as RectTransform;
            RectTransform gameToolsTransform = UIRoot.Find("GRP-Tools") as RectTransform;
            RectTransform gameOrientationCubeTransform = UIRoot.Find("orientation_Cube") as RectTransform;

            Utilities.CopyRectTransformProperties(RootWidget, gameRootWidget);
            Utilities.CopyRectTransformProperties(UndoRedoTransform, gameUndoRedoTransformTransform);
            Utilities.CopyRectTransformProperties(ToggleOrientationTransform, gameToggleOrientationTransform);
            Utilities.CopyRectTransformProperties(InfoOverlaysTransform, gameInfoOverlaysTransform);
            Utilities.CopyRectTransformProperties(SymmetrySnapTransform, gameSymmetrySnapTransform);
            Utilities.CopyRectTransformProperties(ToolsTransform, gameToolsTransform);
            if (gameOrientationCubeTransform != null)  // This class expects the orientation cube to have been moved to the toolbars group
            {
                Utilities.CopyRectTransformProperties(OrientationCubeTransform, gameOrientationCubeTransform);
            }

            return true;
        }

        // private RectTransform _undoRedoTransform;
        // private RectTransform _toggleOrientationTransform;
        // private RectTransform _infoOverlaysTransform;
        // private RectTransform _symmetrySnapTransform;
        // private RectTransform _toolsTransform;
    }
}