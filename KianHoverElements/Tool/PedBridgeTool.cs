using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace PedBridge.HoverTool {
    using static PedBridge.Utils.Helpers;
    public sealed class PedBridgeTool : KianToolBase {
        ToolButton button;
        public PedBridgeTool() : base() {
            Log("PedBridgeTool.CTOR()");
            var uiView = UIView.GetAView();
            button = (ToolButton)uiView.AddUIComponent(typeof(ToolButton));
            button.eventClicked += (_, __) => {
                Log("Button pressed");
                ToggleTool();
            };
        }

        public static PedBridgeTool Create() {
            Log("PedBridgeTool.Create()");
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            var tool = toolModControl.GetComponent<PedBridgeTool>() ?? toolModControl.AddComponent<PedBridgeTool>();
            return tool;
        }

        public static void Remove() {
            Log("PedBridgeTool.Remove()");
            GameObject toolModControl = ToolsModifierControl.toolController?.gameObject;
            var tool = toolModControl?.GetComponent<PedBridgeTool>();
            if (tool != null)
                Destroy(tool);
        }

        protected override void OnDestroy() {
            Log("PedBridgeTool.OnDestroy()\n" + Environment.StackTrace);
            Destroy(button);
            base.OnDestroy();
        }

        //public override void EnableTool() => ToolsModifierControl.SetTool<PedBridgeTool>();

        protected override void OnEnable() {
            Log("PedBridgeTool.OnEnable");
            base.OnEnable();
            button.Focus();
        }

        protected override void OnDisable() {
            Log("PedBridgeTool.OnDisable");
            button.Unfocus();
            base.OnDisable();
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            base.RenderOverlay(cameraInfo);
            if (!Condition())
                return;
            Color color = GetToolColor(Input.GetMouseButton(0), false);
            //NetTool.RenderOverlay(
            //    cameraInfo,
            //    ref HoveredSegmentId.ToSegment(),
            //    color,
            //    color);
            DrawNodeCircle(cameraInfo, HoveredNodeId, color);
        }

        bool Condition() {
            if (HoveredSegmentId == 0 || HoveredNodeId == 0)
                return false;
            NetNode.Flags nodeFlags = HoveredNodeId.ToNode().m_flags;
            NetNode node = HoveredNodeId.ToNode();
            if (node.CountSegments() != 4)
                return false;
            return true;
        }

        protected override void OnPrimaryMouseClicked() {
            Log($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
            if (!Condition())
                return;
            BuildControler.CreateJunctionBridge(HoveredNodeId);
        }

        protected override void OnSecondaryMouseClicked() {
            //if(HoveredSegmentId!=0)
            //    Utils.NetService.CopyMove(HoveredSegmentId);
            throw new System.NotImplementedException();
        }

    } //end class
}
