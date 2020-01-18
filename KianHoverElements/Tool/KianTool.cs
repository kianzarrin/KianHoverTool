using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace Kian.HoverTool {
    using static Kian.Mod.ShortCuts;
    public sealed class KianTool : KianToolBase {
        ToolButton button;
        public KianTool() : base() {
            var uiView = UIView.GetAView();
            button = (ToolButton)uiView.AddUIComponent(typeof(ToolButton));
            button.eventClicked += (_, __) => {
                Debug.Log("Button pressed");
                ToggleTool();
            };

            Debug.Log("Initializing Kian Tool...");
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            var tool = toolModControl.GetComponent<KianTool>() ?? toolModControl.AddComponent<KianTool>();
        }

        public override void Release() {
            Destroy(button.gameObject);
            base.Release();
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
            NetNode.Flags nodeFlags = HoveredNodeId.ToNode().m_flags;
            Color color = GetToolColor(Input.GetMouseButton(0), false);
            NetTool.RenderOverlay(
                cameraInfo,
                ref HoveredSegmentId.ToSegment(),
                color,
                color);
        }

        protected override void OnPrimaryMouseClicked() {
            Debug.Log($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
            if(HoveredSegmentId == 0) {
                return;
            }
        }

        protected override void OnSecondaryMouseClicked() {
            throw new System.NotImplementedException();
        }

    } //end class
}
