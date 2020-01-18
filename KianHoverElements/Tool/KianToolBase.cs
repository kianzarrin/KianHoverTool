using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;
using static Kian.Mod.ShortCuts;

namespace Kian.HoverTool {
    public abstract class KianToolBase : DefaultTool {
        public bool ToolEnabled => ToolsModifierControl.toolController.CurrentTool == this;

        protected override void OnDestroy() {
            DisableTool();
            base.OnDestroy();
        }

        protected abstract void OnPrimaryMouseClicked();
        protected abstract void OnSecondaryMouseClicked();

        public void ToggleTool() {
            if (!ToolEnabled)
                EnableTool();
            else
                DisableTool();
        }

        private void EnableTool() {
            Log("EnableTool: called");
            WorldInfoPanel.HideAllWorldInfoPanels();
            GameAreaInfoPanel.Hide();
            ToolsModifierControl.toolController.CurrentTool = this;
        }

        private void DisableTool() {
            Log("DisableTool: called");
            ToolsModifierControl.SetTool<DefaultTool>();
        }

        protected override void OnToolUpdate() {
            base.OnToolUpdate();
            DetermineHoveredElements();

            if (Input.GetMouseButtonDown(0)) {
                OnPrimaryMouseClicked();
            } else if (Input.GetMouseButtonDown(1)) {
                OnSecondaryMouseClicked();
            }
        }

        public override void SimulationStep() {
            base.SimulationStep();
            DetermineHoveredElements();
        }

        public ushort HoveredNodeId { get; private set; } = 0;
        public ushort HoveredSegmentId { get; private set; } = 0;


        private void FindHoveredNode() {
            var nodeInput = new RaycastInput(m_mouseRay, m_mouseRayLength) {
                m_netService = {
                        // find road nodes
                        m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                        m_service = ItemClass.Service.Road
                    },
                m_ignoreTerrain = true,
                m_ignoreNodeFlags = NetNode.Flags.None
            };

            // nodeInput.m_netService2.m_itemLayers = ItemClass.Layer.Default
            //     | ItemClass.Layer.PublicTransport | ItemClass.Layer.MetroTunnels;
            // nodeInput.m_netService2.m_service = ItemClass.Service.PublicTransport;
            // nodeInput.m_netService2.m_subService = ItemClass.SubService.PublicTransportTrain;
            // nodeInput.m_ignoreNodeFlags = NetNode.Flags.Untouchable;

            if (RayCast(nodeInput, out RaycastOutput nodeOutput)) {
                HoveredNodeId = nodeOutput.m_netNode;
            } else {
                // find train nodes
                nodeInput.m_netService.m_itemLayers =
                    ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                nodeInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                nodeInput.m_netService.m_subService = ItemClass.SubService.PublicTransportTrain;
                nodeInput.m_ignoreNodeFlags = NetNode.Flags.None;
                // nodeInput.m_ignoreNodeFlags = NetNode.Flags.Untouchable;

                if (RayCast(nodeInput, out nodeOutput)) {
                    HoveredNodeId = nodeOutput.m_netNode;
                } else {
                    // find metro nodes
                    nodeInput.m_netService.m_itemLayers =
                        ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                    nodeInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                    nodeInput.m_netService.m_subService =
                        ItemClass.SubService.PublicTransportMetro;
                    nodeInput.m_ignoreNodeFlags = NetNode.Flags.None;
                    // nodeInput.m_ignoreNodeFlags = NetNode.Flags.Untouchable;

                    if (RayCast(nodeInput, out nodeOutput)) {
                        HoveredNodeId = nodeOutput.m_netNode;
                    }
                }
            }

        }

        private void FindHoveredSegment() {
            var segmentInput = new RaycastInput(m_mouseRay, m_mouseRayLength) {
                m_netService = {
                        // find road segments
                        m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                        m_service = ItemClass.Service.Road
                    },
                m_ignoreTerrain = true,
                m_ignoreSegmentFlags = NetSegment.Flags.None
            };
            // segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.Untouchable;

            if (RayCast(segmentInput, out RaycastOutput segmentOutput)) {
                HoveredSegmentId = segmentOutput.m_netSegment;
            } else {
                // find train segments
                segmentInput.m_netService.m_itemLayers =
                    ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                segmentInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                segmentInput.m_netService.m_subService =
                    ItemClass.SubService.PublicTransportTrain;
                segmentInput.m_ignoreTerrain = true;
                segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
                // segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.Untouchable;

                if (RayCast(segmentInput, out segmentOutput)) {
                    HoveredSegmentId = segmentOutput.m_netSegment;
                } else {
                    // find metro segments
                    segmentInput.m_netService.m_itemLayers =
                        ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                    segmentInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                    segmentInput.m_netService.m_subService =
                        ItemClass.SubService.PublicTransportMetro;
                    segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
                    // segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.Untouchable;

                    if (RayCast(segmentInput, out segmentOutput)) {
                        HoveredSegmentId = segmentOutput.m_netSegment;
                    }
                }
            }
        }

        private void GetHoveredNodeFromSegment() {
            ushort startNodeId = Singleton<NetManager>
                                    .instance.m_segments.m_buffer[HoveredSegmentId]
                                    .m_startNode;
            ushort endNodeId = Singleton<NetManager>
                                .instance.m_segments.m_buffer[HoveredSegmentId].m_endNode;

            NetNode[] nodesBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
            float startDist = (m_mousePosition - nodesBuffer[startNodeId]
                                                        .m_position).magnitude;
            float endDist = (m_mousePosition - nodesBuffer[endNodeId]
                                                        .m_position).magnitude;
            if (startDist < endDist && startDist < 75f) {
                HoveredNodeId = startNodeId;
            } else if (endDist < startDist && endDist < 75f) {
                HoveredNodeId = endNodeId;
            }
        }

        /// <summary>
        /// returns the node segment that is closest to the mouse pointer based on angle.
        /// </summary>
        internal ushort GetHoveredSegmentFromNode() {
            bool considerSegmentLenght = true;
            ushort minSegId = 0;
            NetNode node = HoveredNodeId.ToNode();
            Vector3 dir0 = node.m_position - m_mousePosition;
            float min_angle = float.MaxValue;
            for (int i = 0; i < 8; ++i) {
                ushort segmentId = node.GetSegment(i);
                if (segmentId == 0)
                    continue;
                NetSegment segment = segmentId.ToSegment();
                Vector3 dir = segment.m_startNode == HoveredNodeId ?
                    segment.m_startDirection :
                    segment.m_endDirection;
                float angle = GetAgnele(-dir, dir0);
                if (considerSegmentLenght)
                    angle *= segment.m_averageLength;
                if (angle < min_angle) {
                    min_angle = angle;
                    minSegId = segmentId;
                }
            }
            return minSegId;
        }

        /// <summary>
        /// returns the angle between v1 and v2.
        /// input order does not matter.
        /// The return value is between 0 to 180.
        /// </summary>
        private static float GetAgnele(Vector3 v1, Vector3 v2) {
            float ret = Vector3.Angle(v1, v2); // -180 to 180 degree
            if (ret > 180) ret -= 180; // future proofing.
            ret = Math.Abs(ret);
            return ret;
        }

        private bool DetermineHoveredElements() {
            bool mouseRayValid = !UIView.IsInsideUI() && Cursor.visible;

            if (mouseRayValid) {
                HoveredSegmentId = 0;
                HoveredNodeId = 0;

                FindHoveredNode();

                FindHoveredSegment();
                if (HoveredNodeId <= 0 && HoveredSegmentId > 0) {
                    // alternative way to get a node hit: check distance to start and end nodes of the segment
                    GetHoveredNodeFromSegment();
                }

                if (HoveredNodeId != 0) {
                    HoveredSegmentId = GetHoveredSegmentFromNode();
                }

                return HoveredNodeId != 0 || HoveredSegmentId != 0;
            }

            return false; // mouseRayValid=false here
        }

#if false
        private bool DetermineHoveredElementsSimple() {
            if (UIView.IsInsideUI() || !Cursor.visible) {
                return false;
            }

            HoveredSegmentId = 0;
            HoveredNodeId = 0;

            find currently hovered node
            RaycastInput nodeInput = new RaycastInput(m_mouseRay, m_mouseRayLength) {
                m_netService = {
                         find road segments
                        m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                        m_service = ItemClass.Service.Road
                    },
                m_ignoreTerrain = true,
                m_ignoreNodeFlags = NetNode.Flags.None
            };

            if (RayCast(nodeInput, out RaycastOutput nodeOutput)) {
                HoveredNodeId = nodeOutput.m_netNode;
            }

            HoveredSegmentId = GetSegmentFromNode();

            if (HoveredSegmentId != 0) {
                Debug.Assert(HoveredNodeId != 0, "unexpected: HoveredNodeId == 0");
                return true;
            }

            find currently hovered segment
            var segmentInput = new RaycastInput(m_mouseRay, m_mouseRayLength) {
                m_netService = {
                     find road segments
                    m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                    m_service = ItemClass.Service.Road
                },
                m_ignoreTerrain = true,
                m_ignoreSegmentFlags = NetSegment.Flags.None
            };

            if (RayCast(segmentInput, out RaycastOutput segmentOutput)) {
                HoveredSegmentId = segmentOutput.m_netSegment;
            }


            if (HoveredNodeId <= 0 && HoveredSegmentId > 0) {
                alternative way to get a node hit: check distance to start and end nodes
                of the segment
                ushort startNodeId = HoveredSegmentId.ToSegment().m_startNode;
                ushort endNodeId = HoveredSegmentId.ToSegment().m_endNode;

                var vStart = segmentOutput.m_hitPos - startNodeId.ToNode().m_position;
                var vEnd = segmentOutput.m_hitPos - endNodeId.ToNode().m_position;

                float startDist = vStart.magnitude;
                float endDist = vEnd.magnitude;

                if (startDist < endDist && startDist < 75f) {
                    HoveredNodeId = startNodeId;
                } else if (endDist < startDist && endDist < 75f) {
                    HoveredNodeId = endNodeId;
                }
            }
            return HoveredNodeId != 0 || HoveredSegmentId != 0;
        }
#endif


    }
}
