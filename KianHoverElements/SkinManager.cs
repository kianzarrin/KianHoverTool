using System;
using System.Collections;
using UnityEngine;
using static Kian.Mod.ShortCuts;

namespace Kian.Skins
{
    public static class SkinManager {
        public static Hashtable Crosswalks = new Hashtable();

        private static string GetKey(ushort segmentID, ushort nodeID) => $"{segmentID}:{nodeID}";
        public static void Toggle(ushort segmentID, ushort nodeID) {
            if(segmentID == 0 || nodeID == 0) {
                return;
            }
            var key = GetKey(segmentID, nodeID);
            if (Crosswalks.Contains(key) && (bool)Crosswalks[key]) {
                Crosswalks[key] = false;
            } else {
                Crosswalks[key] = true;
            }
            Debug.Log($"crosswalks color toggled. new value = {key} : {Crosswalks[key]}");
        }

        public static bool ShowCrosswalks0(ushort segmentID, ushort nodeID, bool original=true) {
            var key = GetKey(segmentID, nodeID);
            bool ret = original;
            ret = Crosswalks.Contains(key);
            ret &= (bool)Crosswalks[key];
            return ret;
        }
        public static bool ShowCrosswalks(ushort segmentID, ushort nodeID) {
            var key = GetKey(segmentID, nodeID);
            bool ret = IsJunction(nodeID);
            ret &= Crosswalks.Contains(key);
            ret &= (bool)Crosswalks[key];
            return ret;
        }

    } // end class
}
