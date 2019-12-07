using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;
using Kian.Skins;

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

            Debug.Log(" instanciating Kian Tool...");
        }


        public override void EnableTool() => ToolsModifierControl.SetTool<KianTool>();

        protected override void OnEnable() {
            Debug.Log("OnEnable Kian Tool");
            base.OnEnable();
            button.Focus();
        }
        protected override void OnDisable() {
            Debug.Log("OnDisable Kian Tool");
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

        bool hooked = false;
        protected override void OnPrimaryMouseClicked() {
            Debug.Log($"OnPrimaryMouseClicked: segment {HoveredSegmentId} node {HoveredNodeId}");
            if(HoveredSegmentId == 0) {
                return;
            }
            //if(!hooked) {
            //    Hook.HookAll();
            //}

            SkinManager.Toggle(HoveredSegmentId);

            //var seg = Segment(HoveredSegmentId);
            //Type t = seg.Info.m_netAI.GetType();
            //Debug.Log($"netAI type is {t}");
            //Color color = seg.Info.m_netAI.GetColor(HoveredSegmentId, ref seg, InfoManager.InfoMode.None);
            //Debug.Log($"get color returned {color}");

            //Refersh();
            Singleton<NetManager>.instance.UpdateSegmentColors(HoveredSegmentId);
            netMan.UpdateSegment(HoveredSegmentId);
            Segment(HoveredSegmentId).UpdateSegment(HoveredSegmentId);

        }

        protected override void OnSecondaryMouseClicked() {
            throw new System.NotImplementedException();
        }

        public void Refersh() {
            Singleton<NetManager>.instance.UpdateSegmentColors();
            Singleton<NetManager>.instance.UpdateNodeColors();
        }
    } //end class
}
