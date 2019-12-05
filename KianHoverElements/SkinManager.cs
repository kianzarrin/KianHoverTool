using UnityEngine;

namespace KianHoverElements
{
    public static class SkinManager {
        public static Color?[] SegmentSkins = new Color?[NetManager.MAX_SEGMENT_COUNT];

        public static void Toggle(ushort segmentID) {
            ref Color? skin = ref SegmentSkins[segmentID];
            if (skin == null) {
                skin = new Color(255, 0, 0);
            } else {
                skin = null;
            }
            Debug.Log($"skin color toggled. new color = {SegmentSkins[segmentID]}");
        }
    } // end class
}
