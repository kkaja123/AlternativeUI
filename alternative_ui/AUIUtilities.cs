using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx.Logging;

namespace AUI
{
    // public struct RectTransformData
    // {
    //     public Vector2 anchorMin;
    //     public Vector2 anchorMax;
    //     public Vector3 anchoredPosition3D;
    //     public Vector2 pivot;
    //     public Vector2 sizeDelta;
    //     public Quaternion rotation;
    //     public Vector3 position;
    //     public bool activeSelf;
    // }

    public class Utilities
    {
        /// <summary>
        /// Sets a RectTransform to the same layout as another.
        /// </summary>
        static public void CopyRectTransformProperties(RectTransform dest, RectTransform source)
        {
            if (dest == null || source == null)
            {
                logger.LogError($"Cannot perform CopyRectTransformProperties({dest}, {source})");
                return;
            }

            dest.anchorMin = source.anchorMin;
            dest.anchorMax = source.anchorMax;
            dest.anchoredPosition3D = source.anchoredPosition3D;
            dest.pivot = source.pivot;
            dest.sizeDelta = source.sizeDelta;
            dest.localScale = source.localScale;

            if (dest.gameObject != null && source.gameObject != null)
            {
                dest.gameObject.SetActive(source.gameObject.activeSelf);
            }
            else
            {
                logger.LogWarning($"Cannot setActive() in CopyRectTransformProperties({dest}, {source})");
            }
        }

        /// <summary>
        /// Extract RectTransform data from a real object.
        /// </summary>
        // static public void CopyRectTransformProperties(RectTransformData dest, RectTransform source)
        // {
        //     if (source == null)
        //     {
        //         logger.LogError($"Cannot perform CopyRectTransformProperties({dest}, {source})");
        //         return;
        //     }

        //     dest.position = source.position;
        //     dest.localScale = source.localScale;
        //     dest.sizeDelta = source.sizeDelta;
        //     dest.anchorMin = source.anchorMin;
        //     dest.anchorMax = source.anchorMax;
        //     dest.pivot = source.pivot;
        //     dest.rotation = source.rotation;

        //     if (source.gameObject != null)
        //     {
        //         dest.activeSelf = source.gameObject.activeSelf;
        //     }
        //     else
        //     {
        //         logger.LogWarning($"Cannot setActive() in CopyRectTransformProperties({dest}, {source})");
        //     }
        // }

        /// <summary>
        /// Project RectTransform data onto a real object.
        /// </summary>
        // static public void CopyRectTransformProperties(RectTransform dest, RectTransformData source)
        // {
        //     if (dest == null)
        //     {
        //         logger.LogError($"Cannot perform CopyRectTransformProperties({dest}, {source})");
        //         return;
        //     }

        //     dest.position = source.position;
        //     dest.localScale = source.localScale;
        //     dest.sizeDelta = source.sizeDelta;
        //     dest.anchorMin = source.anchorMin;
        //     dest.anchorMax = source.anchorMax;
        //     dest.pivot = source.pivot;
        //     dest.rotation = source.rotation;

        //     if (dest.gameObject != null)
        //     {
        //         dest.gameObject.SetActive(source.activeSelf);
        //     }
        //     else
        //     {
        //         logger.LogWarning($"Cannot setActive() in CopyRectTransformProperties({dest}, {source})");
        //     }
        // }

        internal static BepInEx.Logging.ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("AUI.Utilities");
    }
}
