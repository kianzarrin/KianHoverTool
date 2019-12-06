using UnityEngine;
using static Kian.Mod.ShortCuts;

namespace Kian.HoverTool
{
    public static class SkinManager {
        public static Color?[] SegmentSkins = new Color?[NetManager.MAX_SEGMENT_COUNT];
        public static Color?[] NodeSkins = new Color?[NetManager.MAX_NODE_COUNT];

        public static void Toggle(ushort segmentID) {
            if(segmentID == 0) {
                return;
            }
            ref Color? skin = ref SegmentSkins[segmentID];
            if (skin == null) {
                skin = new Color(255, 0, 0);
            } else {
                skin = null;
            }

            var seg = Segment(segmentID);
            NodeSkins[seg.m_startNode] = skin;
            NodeSkins[seg.m_endNode] = skin;
            Debug.Log($"skin color toggled. new color = {SegmentSkins[segmentID]}");
        }
   } // end class
}
