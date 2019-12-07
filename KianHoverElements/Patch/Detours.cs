using UnityEngine;
using ColossalFramework;
using static Kian.Mod.ShortCuts;

namespace Kian.Patch {
    public class Detours {
        public static Color?[] SegmentSkins => Kian.HoverTool.SkinManager.SegmentSkins;
        public static Color?[] NodeSkins => Kian.HoverTool.SkinManager.NodeSkins;

        public Color GetSegmentColor(ushort segmentID, ref global::NetSegment data, InfoManager.InfoMode infoMode) {
            NetAI netAI = Segment(segmentID).Info.m_netAI; //this
            Singleton<Hook.GetSegmentColor>.instance.UnHook();

            var patcherState = Apply(netAI.m_info, SegmentSkins[segmentID]);
            var segmentColor = netAI.GetColor(segmentID, ref data, infoMode);
            Revert(netAI.m_info, patcherState);

            Singleton<Hook.GetSegmentColor>.instance.Hook();
            if (SegmentSkins[segmentID] != null) Debug.Log($"Detour returns segmentColor={segmentColor} for segmentID={segmentID}");
            return segmentColor;
        }

        public Color GetNodeColor(ushort nodeID, ref global::NetNode data, InfoManager.InfoMode infoMode) {
            NetAI netAI = Node(nodeID).Info.m_netAI;//this
            Singleton<Hook.GetNodeColor>.instance.UnHook();

            var patcherState = Apply(netAI.m_info, NodeSkins[nodeID]);
            var nodeColor = netAI.GetColor(nodeID, ref data, infoMode);
            Revert(netAI.m_info, patcherState);

            Singleton<Hook.GetNodeColor>.instance.Hook();
            if (NodeSkins[nodeID] != null) Debug.Log($"Detour returns nodeColor={nodeColor} for nodeID={nodeID}");
            return nodeColor;
        }

        /*private void NetSegment.RenderInstance(
         * RenderManager.CameraInfo cameraInfo,
         * ushort segmentID,
         * int layerMask,
         * NetInfo info,
         * ref RenderManager.Instance data) */
        public static void RenderInstance(
            RenderManager.CameraInfo cameraInfo,
            ushort segmentID,
            int layerMask,
            NetInfo info,
            ref RenderManager.Instance data)
        {
            NetSegment segment = Segment(segmentID);//this
            Singleton<Hook.RenderInstance>.instance.UnHook();

            var patcherState = Apply(info, NodeSkins[segmentID]);
            Singleton<Hook.RenderInstance>.instance.From.Invoke(segment, new object[] { cameraInfo, segmentID, layerMask, info, data });
            Revert(info, patcherState);

            Singleton<Hook.GetNodeColor>.instance.Hook();
            if (SegmentSkins[segmentID] != null) Debug.Log($"Detour uses SegmentSkins[segmentID]={SegmentSkins[segmentID]} for segmentID={segmentID}");
        }

        // // private static void NetTool.RenderSegment(NetInfo info, NetSegment.Flags flags, Vector3 startPosition, Vector3 endPosition, Vector3 startDirection, Vector3 endDirection, bool smoothStart, bool smoothEnd)
        //public static void RenderSegment(
        //    NetTool netTool,
        //    NetInfo info,
        //    NetSegment.Flags flags,
        //    Vector3 startPosition,
        //    Vector3 endPosition,
        //    Vector3 startDirection,
        //    Vector3 endDirection,
        //    bool smoothStart,
        //    bool smoothEnd){}


        // //private static void RenderNode(NetInfo info, Vector3 position, Vector3 direction)
        //public static void RenderNode(NetTool netTool, NetInfo info, Vector3 position, Vector3 direction) {
        //}


        public static Color? Apply(NetInfo info, Color? skinColor) {
            if (info == null || skinColor == null || info.m_color == skinColor) {
                return null;
            }

            var state = info.m_color;

            info.m_color = (Color)skinColor;

            return state;
        }

        public static void Revert(NetInfo info, Color? state) {
            if (info == null || state == null) {
                return;
            }

            info.m_color = state.Value;
        }
    }
}
