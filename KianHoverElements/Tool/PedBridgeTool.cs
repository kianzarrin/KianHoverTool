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
            if (HoveredSegmentId == 0 || HoveredNodeId == 0)
                return;

            Color color1 = GetToolColor(Input.GetMouseButton(0), false);
            Color color2 = GetToolColor(Input.GetMouseButton(1), false);
            if (Condition())
                DrawNodeCircle(cameraInfo, HoveredNodeId, color1);
            else
                NetTool.RenderOverlay(
                    cameraInfo,
                    ref HoveredSegmentId.ToSegment(),
                    color2,
                    color2);
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
            if (HoveredSegmentId == 0 || HoveredNodeId == 0)
                return;
            if (Condition()) {
                BuildControler.CreateJunctionBridge(HoveredNodeId);
            }else {
                Utils.NetService.CopyMove(HoveredSegmentId);
            }
        }

        protected override void OnSecondaryMouseClicked() {
            throw new System.NotImplementedException();
        }

    } //end class
}
