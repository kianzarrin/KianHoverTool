using System;
using System.Collections;
using UnityEngine;
using static Kian.Utils.ShortCuts;

namespace Kian.Skins
{
    public static class SkinManager {
        public static bool?[] Crosswalks = new bool?[NetManager.MAX_SEGMENT_COUNT*2];

        public static void Toggle(ushort segmentID, ushort nodeID) {
            if (segmentID == 0 || nodeID == 0) {
                return;
            }
            bool bStart = Segment(segmentID).m_startNode == nodeID;
            Toggle(segmentID, bStart);
        }

        private static int GetIndex(ushort segmentID, bool bStart) {
            int ret = segmentID * 2;
            if (!bStart) {
                ret++;
            }
            return ret;
        }

        public static void Toggle(ushort segmentID, bool bStart) {
            if (segmentID == 0) {
                return;
            }

            int idx = GetIndex(segmentID, bStart);
            if (Crosswalks[idx] ?? false) {
                Crosswalks[idx] = false;
            } else {
                Crosswalks[idx] = true;
            }

            Debug.Log($"crosswalks color toggled. {segmentID}*2 + {bStart} = {idx}. new value =  {Crosswalks[idx]}");
        }

        public static bool HasCrossingBan(ushort segmentID, ushort nodeID) {
            bool bStart = Segment(segmentID).m_startNode == nodeID;
            return HasCrossingBan(segmentID, bStart);
        }
        public static bool HasCrossingBan(ushort segmentID, bool bStart) {
            int idx = GetIndex(segmentID, bStart);
            bool ret = Crosswalks[idx] ?? false;
            return ret;
        }

    } // end class

    public static class TextureManger {
    }

}
