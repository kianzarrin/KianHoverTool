using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace Kian.Patch
{
    public static class LogOnce {
        private static List<string> listLogs = new List<string>();
        public static void Log(string m) {
            var st = new System.Diagnostics.StackTrace();
            var sf = st.GetFrame(2);
            string key = sf.GetMethod().Name + ": " + m;
            if(!listLogs.Contains(key))
                Debug.Log(key);
        }
    }

    public static class Hook
    {
        public static void HookAll() {
            GetSegmentColor.Hook();
            Debug.Log("hooked everything");
        }
        public static void UnHookAll() {
            GetSegmentColor.UnHook();
            Debug.Log("unhooked everything");
        }

        public class GetSegmentColor {
            private static MethodInfo From = null;
            private static RedirectCallsState State = null;
            public static void Hook() {
                UnHook();
                MethodInfo from = typeof(RoadAI).GetMethod("GetColor", new[] { typeof(ushort), typeof(global::NetSegment).MakeByRefType(), typeof(InfoManager.InfoMode) });
                MethodInfo to = typeof(ColorDetours).GetMethod("GetSegmentColor");
                LogOnce.Log($"Hooking {from}  to {to}");
                if (from == null || to == null) {
                    Debug.LogError("hooking failed.");
                    return;
                }
                State = RedirectionHelper.RedirectCalls(from, to);
                From = from;
            }
            public static void UnHook() {
                if (From != null) {
                    LogOnce.Log($"UnHooking {From}");
                    RedirectionHelper.RevertRedirect(From, State);
                    From = null;
                    State = null;
                }
            }
        }

        public class GetNodeColor {
            private static MethodInfo From = null;
            private static RedirectCallsState State = null;
            public static void Hook() {
                UnHook();
                MethodInfo from = typeof(RoadAI).GetMethod("GetColor", new[] { typeof(ushort), typeof(global::NetNode).MakeByRefType(), typeof(InfoManager.InfoMode) });
                MethodInfo to = typeof(ColorDetours).GetMethod("GetNodeColor");
                LogOnce.Log($"Hooking {from}  to {to}");
                if (from == null || to == null) {
                    Debug.LogError("hooking failed.");
                    return;
                }
                State = RedirectionHelper.RedirectCalls(from, to);
                From = from;
            }
            public static void UnHook() {
                if (From != null) {
                    LogOnce.Log($"UnHooking {From}");
                    RedirectionHelper.RevertRedirect(From, State);
                    From = null;
                    State = null;
                }
            }
        }
    }

    public static class ColorDetours
    {
        public static Color? [] SegmentSkins => Kian.HoverTool.SkinManager.SegmentSkins;
        public static Color?[] NodeSkins => Kian.HoverTool.SkinManager.NodeSkins;

        public static Color GetSegmentColor(NetAI netAI, ushort segmentID, ref global::NetSegment data, InfoManager.InfoMode infoMode)
        {
            Hook.GetSegmentColor.UnHook();
            var patcherState = Apply(netAI.m_info, SegmentSkins[segmentID]);
            var segmentColor = netAI.GetColor(segmentID, ref data, infoMode);
            Revert(netAI.m_info, patcherState);
            Hook.GetSegmentColor.Hook();
            if(SegmentSkins[segmentID]!= null) Debug.Log($"Detour returns segmentColor={segmentColor} for segmentID={segmentID}");
            return segmentColor;
        }

        public static Color GetNodeColor(NetAI netAI, ushort nodeID, ref global::NetNode data, InfoManager.InfoMode infoMode) {
            Hook.GetNodeColor.UnHook();
            var patcherState = Apply(netAI.m_info, NodeSkins[nodeID]);
            var nodeColor = netAI.GetColor(nodeID, ref data, infoMode);
            Revert(netAI.m_info, patcherState);
            Hook.GetNodeColor.Hook();
            if (NodeSkins[nodeID] != null) Debug.Log($"Detour returns nodeColor={nodeColor} for nodeID={nodeID}");
            return nodeColor;
        }

        public static Color? Apply(NetInfo info, Color? skinColor)
        {
            if (info == null || skinColor == null || info.m_color == skinColor)
            {
                return null;
            }

            var state = info.m_color;

            info.m_color = (Color)skinColor;

            return state;
        }

        public static void Revert(NetInfo info, Color? state)
        {
            if (info == null || state == null)
            {
                return;
            }

            info.m_color = state.Value;
        }
    }
}
