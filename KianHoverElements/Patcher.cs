using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace KianPatch
{
    public static class Hook
    {
        static private Dictionary<MethodInfo, RedirectCallsState> hooks = new Dictionary<MethodInfo, RedirectCallsState>();
        public static void HookGetSegmentColor()
        {
            MethodInfo from = typeof(NetAI).GetMethod("GetColor", new[] { typeof(ushort), typeof(global::NetSegment).MakeByRefType(), typeof(InfoManager.InfoMode) });
            MethodInfo to = typeof(ColorDetours).GetMethod("GetSegmentColor");
            RedirectCallsState hook = RedirectionHelper.RedirectCalls(from, to);
            hooks.Add(from, hook);
            Debug.Log("Hooked NetAI.GetColor to GetSegmentColor");
        }
        public static void UnhookAll()
        {
            foreach (var entry in hooks) {
                RedirectionHelper.RevertRedirect(entry.Key, entry.Value);
            }
            hooks.Clear();
            Debug.Log("Everything is unhooked.");
        }
    }

    public static class ColorDetours
    {
        public static Color? [] SegmentSkins => KianHoverElements.SkinManager.SegmentSkins;

        public static Color GetSegmentColor(NetAI netAI, ushort segmentID, ref global::NetSegment data, InfoManager.InfoMode infoMode)
        {
            Debug.Log($"Detour called");
            var patcherState = Apply(netAI.m_info, SegmentSkins[segmentID]);
            var segmentColor = netAI.GetColor(segmentID, ref data, infoMode);
            Revert(netAI.m_info, patcherState);
            Debug.Log($"returned color ={segmentColor}");
            return segmentColor;
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
