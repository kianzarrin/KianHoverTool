using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;
using Kian.Patch;

namespace Kian.HoverTool {
    using static Kian.Mod.ShortCuts;
    public sealed class KianTool : KianToolBase {
        ToolButton button;
        public KianTool() : base() {
            var uiView = UIView.GetAView();
            button = (ToolButton)uiView.AddUIComponent(typeof(ToolButton));

            Debug.Log("Initializing traffic Kian Tool...");
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            var tool = toolModControl.GetComponent<KianTool>() ?? toolModControl.AddComponent<KianTool>();
        }

        public static bool Toggle() {
            var tool = Singleton<KianTool>.instance;
            tool.ToggleTool();
            return tool.ToolEnabled;
        }

        public override void EnableTool() => ToolsModifierControl.SetTool<KianTool>();

        protected override void OnEnable() {
            Debug.Log("OnEnable");
            base.OnEnable();
            button.Focus();
        }
        protected override void OnDisable() {
            Debug.Log("OnDisable");
            base.OnDisable();
            button.Unfocus();
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            base.RenderOverlay(cameraInfo);
            if (HoveredSegmentId == 0 || HoveredNodeId == 0) return;
            NetNode.Flags nodeFlags = Node(HoveredNodeId).m_flags;
            Color color = GetToolColor(Input.GetMouseButton(0), false);
            NetTool.RenderOverlay(
                cameraInfo,
                ref Segment(HoveredSegmentId),
                color,
                color);
        }

        protected override void OnPrimaryMouseClicked() {
            Debug.Log($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
            if(HoveredSegmentId == 0) {
                return;
            }
            Hook.HookAll();


            SkinManager.Toggle(HoveredSegmentId);

            //var seg = Segment(HoveredSegmentId);
            //Type t = seg.Info.m_netAI.GetType();
            //Debug.Log($"netAI type is {t}");
            //Color color = seg.Info.m_netAI.GetColor(HoveredSegmentId, ref seg, InfoManager.InfoMode.None);
            //Debug.Log($"get color returned {color}");

            //Refersh();
            RefreshSegment(HoveredSegmentId);

        }

        protected override void OnSecondaryMouseClicked() {
            throw new System.NotImplementedException();
        }

        public void RefreshSegment(ushort ID) => Singleton<NetManager>.instance.UpdateSegmentColors(ID);
        public void Refersh() {
            Singleton<NetManager>.instance.UpdateSegmentColors();
            Singleton<NetManager>.instance.UpdateNodeColors();
        }
    } //end class
}
