using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System;

using static Kian.Mod.ShortCuts;

namespace Kian.HoverTool {
    public abstract class KianToolBase : DefaultTool
    {
        public bool ToolEnabled = false;
        public virtual void Release() {
            if (ToolEnabled)
                ToggleTool();
            Destroy(this.gameObject);
        }

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

            HoveredSegmentId = GetSegmentFromNode();

            if (HoveredSegmentId != 0) {
                Debug.Assert(HoveredNodeId != 0, "unexpected: HoveredNodeId == 0");
                return true;
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
                ushort startNodeId = HoveredSegmentId.ToSegment().m_startNode;
                ushort endNodeId = HoveredSegmentId.ToSegment().m_endNode;

                var vStart = segmentOutput.m_hitPos - startNodeId.ToNode().m_position;
                var vEnd = segmentOutput.m_hitPos - endNodeId.ToNode().m_position;

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


        internal ushort GetSegmentFromNode() {
            ushort minSegId = 0;
            if (HoveredNodeId != 0) {
                NetNode node = HoveredNodeId.ToNode();
                //TODO does this actually find the mouse direction vector?
                Vector3 dir0 = m_mouseRay.origin - node.m_position;
                float min_angle = float.MaxValue;
                for (int i = 0; i < 8; ++i) {
                    ushort segmentId = node.GetSegment(i);
                    if (segmentId == 0)
                        continue;
                    NetSegment segment = segmentId.ToSegment();
                    Vector3 dir;
                    if (segment.m_startNode == HoveredNodeId) {
                        dir = -segment.m_startDirection;

                    } else {
                        dir = segment.m_endDirection;
                    }
                    float angle = Vector3.AngleBetween(dir0, dir);
                    if (angle < 0) angle += (float)(2 * Math.PI);
                    if (angle < min_angle) {
                        min_angle = angle;
                        minSegId = segmentId;
                    }

                    string m = $"m_mouseRay.origin:{m_mouseRay.origin} - node.m_position:{node.m_position} = dir0:{dir0}\n";
                    m += $"segment:{segmentId} dir:{dir} angle({dir0},{dir})={angle}";
                    Debug.Log(m);
                    Debug.Log($"{dir.x} {dir.y} {dir.z}");
                }
            }
            return minSegId;
        }
    }
}
