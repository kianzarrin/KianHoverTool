using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace Kian.HoverTool {
    using static Kian.Mod.ShortCuts;
    public sealed class KianTool : KianToolBase {
        ToolButton button;
        public KianTool() : base() {
            Log("KianTool.CTOR()");
            var uiView = UIView.GetAView();
            button = (ToolButton)uiView.AddUIComponent(typeof(ToolButton));
            button.eventClicked += (_, __) => {
                Log("Button pressed");
                ToggleTool();
            };
        }

        public static KianTool Create() {
            Log("KianTool.Create()");
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            var tool = toolModControl.GetComponent<KianTool>() ?? toolModControl.AddComponent<KianTool>();
            return tool;
        }

        public static void Remove() {
            Log("KianTool.Remove()");
            GameObject toolModControl = ToolsModifierControl.toolController?.gameObject;
            var tool = toolModControl?.GetComponent<KianTool>();
            if (tool != null)
                Destroy(tool);
        }

        protected override void OnDestroy() {
            Log("KianTool.OnDestroy()");
            Destroy(button);
            base.OnDestroy();
        }

        //public override void EnableTool() => ToolsModifierControl.SetTool<KianTool>();

        protected override void OnEnable() {
            Log("KianTool.OnEnable");
            base.OnEnable();
            button.Focus();
        }

        protected override void OnDisable() {
            Log("KianTool.OnDisable");
            button.Unfocus();
            base.OnDisable();
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            base.RenderOverlay(cameraInfo);
            OnDebugDraw();
            LogWait($" Application.version = {Application.version} Application.unityVersion = {Application.unityVersion}",4);
            if (HoveredSegmentId == 0 || HoveredNodeId == 0) return;
            NetNode.Flags nodeFlags = HoveredNodeId.ToNode().m_flags;
            Color color = GetToolColor(Input.GetMouseButton(0), false);
            NetTool.RenderOverlay(
                cameraInfo,
                ref HoveredSegmentId.ToSegment(),
                color,
                color);
        }

        protected override void OnPrimaryMouseClicked() {
            Log($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
            if(HoveredSegmentId == 0) {
                return;
            }
            drawpoints = true;
        }

        protected override void OnSecondaryMouseClicked() {
            throw new System.NotImplementedException();
        }

    } //end class
}
