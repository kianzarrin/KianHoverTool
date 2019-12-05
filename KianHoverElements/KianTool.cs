using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace KianHoverElements {
    using static ShortCuts;
    public sealed class KianTool : KianToolBase {
        public KianTool() : base() {
            var uiView = UIView.GetAView();
            ToolButton button = (ToolButton)uiView.AddUIComponent(typeof(ToolButton));

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
            SkinManager.Toggle(HoveredSegmentId);
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
