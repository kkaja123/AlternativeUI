using UnityEngine;
using UnityEngine.UI;

namespace AUI
{
    public class DebugUIVisualizer
    {
        public float OverlayTransparency = 0.25f;
        public void AttachToObject(GameObject target)
        {
            if (target.transform is not RectTransform)
            {
                throw new System.Exception("Object does not have a RectTransform (i.e. not a UI element)");
            }

            GameObject debugObject = new GameObject(
                "debug_ui",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(UnityEngine.UI.Image));
            RectTransform visualizer = debugObject.transform as RectTransform;

            visualizer.SetParent(target.transform);
            visualizer.anchorMin = new Vector2(0, 0);
            visualizer.anchorMax = new Vector2(1, 1);
            visualizer.offsetMin = new Vector2(0, 0);
            visualizer.offsetMax = new Vector2(0, 0);

            UnityEngine.UI.Image visualizerImg = debugObject.GetComponent<UnityEngine.UI.Image>();
            visualizerImg.color = Color.magenta.A(OverlayTransparency);
            visualizerImg.raycastTarget = false;  // Prevents intercepting clicks
        }
    }
}