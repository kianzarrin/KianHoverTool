using ColossalFramework.UI;
using UnityEngine;
using static Kian.Utils.ShortCuts;

namespace Kian.HoverTool {
    public abstract class KianToolBase : DefaultTool
    {
        public bool ToolEnabled = false;

        protected abstract void OnPrimaryMouseClicked();
        protected abstract void OnSecondaryMouseClicked();
        public abstract void EnableTool();

        public void ToggleTool()
        {
            if (!ToolEnabled)
            {
                _EnableTool();
            }
            else
            {
                _DisableTool();
            }
        }

        private void _EnableTool()
        {
            Debug.Log("EnableTool: called");
            ToolsModifierControl.toolController.CurrentTool = this;
            ToolEnabled = true;
            EnableTool();
        }

        private void _DisableTool()
        {
            Debug.Log("DisableTool: called");
            ToolsModifierControl.toolController.CurrentTool = ToolsModifierControl.GetTool<DefaultTool>();
            ToolsModifierControl.SetTool<DefaultTool>();
            ToolEnabled = false;
        }



        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();
            DetermineHoveredElements();

            if (Input.GetMouseButtonDown(0))
            {
                OnPrimaryMouseClicked();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnSecondaryMouseClicked();
            }
        }

        public override void SimulationStep()
        {
            base.SimulationStep();
            DetermineHoveredElements();
        }

        public ushort HoveredNodeId { get; private set; } = 0;
        public ushort HoveredSegmentId { get; private set; } = 0;

        private bool DetermineHoveredElements()
        {
            if (UIView.IsInsideUI() || !Cursor.visible)
            {
                return false;
            }

            HoveredSegmentId = 0;
            HoveredNodeId = 0;

            // find currently hovered node
            RaycastInput nodeInput = new RaycastInput(m_mouseRay, m_mouseRayLength)
            {
                m_netService = {
                        // find road segments
                        m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                        m_service = ItemClass.Service.Road
                    },
                m_ignoreTerrain = true,
                m_ignoreNodeFlags = NetNode.Flags.None
            };

            if (RayCast(nodeInput, out RaycastOutput nodeOutput))
            {
                HoveredNodeId = nodeOutput.m_netNode;
            }

            // find currently hovered segment
            var segmentInput = new RaycastInput(m_mouseRay, m_mouseRayLength)
            {
                m_netService = {
                    // find road segments
                    m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                    m_service = ItemClass.Service.Road
                },
                m_ignoreTerrain = true,
                m_ignoreSegmentFlags = NetSegment.Flags.None
            };

            if (RayCast(segmentInput, out RaycastOutput segmentOutput))
            {
                HoveredSegmentId = segmentOutput.m_netSegment;
            }


            if (HoveredNodeId <= 0 && HoveredSegmentId > 0)
            {
                // alternative way to get a node hit: check distance to start and end nodes
                // of the segment
                ushort startNodeId = Segment(HoveredSegmentId).m_startNode;
                ushort endNodeId = Segment(HoveredSegmentId).m_endNode;

                var vStart = segmentOutput.m_hitPos - Node(startNodeId).m_position;
                var vEnd = segmentOutput.m_hitPos - Node(endNodeId).m_position;

                float startDist = vStart.magnitude;
                float endDist = vEnd.magnitude;

                if (startDist < endDist && startDist < 75f)
                {
                    HoveredNodeId = startNodeId;
                }
                else if (endDist < startDist && endDist < 75f)
                {
                    HoveredNodeId = endNodeId;
                }
            }
            return HoveredNodeId != 0 || HoveredSegmentId != 0;
        }
    }
}
