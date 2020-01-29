using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections;
using UnityEngine;
using static Kian.Mod.ShortCuts;
using static Kian.Utils.NetService;

namespace Kian.HoverTool {
    public abstract class KianToolBase : DefaultTool {
        #region DebugDraw
        public DebugDraw DebugDraw = new DebugDraw();
        public Vector3? c1;
        public Vector3? c2;
        public void OnDebugDraw() {
            if (c1 != null) {
                LogWait("Drawing debug sphere :)", 2);
                DebugDraw.DrawSphere(Vector3.zero, 1, Color.yellow);
                DebugDraw.DrawSphere(Vector3.zero, 10, Color.yellow);
                DebugDraw.DrawSphere(Vector3.zero, 100, Color.yellow);
                DebugDraw.DrawSphere(Vector3.zero, 1000, Color.yellow);
            }
            if (c2 != null) {
                DebugDraw.DrawSphere((Vector3)c2, 10, Color.red);
            }
            if (c1 != null && c2 != null) {
                Vector3 v1 = (Vector3)c1 + new Vector3(10f, 10f, 10f);
                Vector3 v2 = (Vector3)c2 + new Vector3(10f, 10f, 10f);

                LogWait("Drawing debug line :)", 1);
                Debug.DrawLine(v1, v2, Color.red, 10);
            }
        }

        void DebugSeg() {
            if (c1 != null && c2 != null) {
                Vector3 c12 = 0.5f * ((Vector3)c1 + (Vector3)c2);
                ushort nodeID1 = CreateNode((Vector3)c1, PedestrianBridgeInfo);
                ushort nodeID2 = CreateNode((Vector3)c2, GravelBridgeInfo);
                ushort nodeID12 = CreateNode(c12, PedestrianBridgeInfo);
                CreateSegment(nodeID1, nodeID12, PedestrianBridgeInfo); // mouse
                CreateSegment(nodeID2, nodeID12, GravelBridgeInfo); // hit
            }
        }

        public bool drawpoints = false;
        #endregion

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

        public ushort HoveredNodeId { get; private set; } = 0;
        public ushort HoveredSegmentId { get; private set; } = 0;


        public bool DoRayCast(RaycastInput input, out RaycastOutput output)
        {
            return RayCast(input, out output);
        }

        private bool DetermineHoveredElements()
        {
            HoveredSegmentId = 0;
            HoveredNodeId = 0;

            bool mouseRayValid = !UIView.IsInsideUI() && Cursor.visible;


            if (mouseRayValid)
            {
                // find currently hovered node
                var nodeInput = new RaycastInput(m_mouseRay, m_mouseRayLength)
                {
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

                if (RayCast(nodeInput, out RaycastOutput nodeOutput))
                {
                    HoveredNodeId = nodeOutput.m_netNode;
                }
                else
                {
                    // find train nodes
                    nodeInput.m_netService.m_itemLayers =
                        ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                    nodeInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                    nodeInput.m_netService.m_subService = ItemClass.SubService.PublicTransportTrain;
                    nodeInput.m_ignoreNodeFlags = NetNode.Flags.None;
                    // nodeInput.m_ignoreNodeFlags = NetNode.Flags.Untouchable;

                    if (RayCast(nodeInput, out nodeOutput))
                    {
                        HoveredNodeId = nodeOutput.m_netNode;
                    }
                    else
                    {
                        // find metro nodes
                        nodeInput.m_netService.m_itemLayers =
                            ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                        nodeInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                        nodeInput.m_netService.m_subService =
                            ItemClass.SubService.PublicTransportMetro;
                        nodeInput.m_ignoreNodeFlags = NetNode.Flags.None;
                        // nodeInput.m_ignoreNodeFlags = NetNode.Flags.Untouchable;

                        if (RayCast(nodeInput, out nodeOutput))
                        {
                            HoveredNodeId = nodeOutput.m_netNode;
                        }
                    }
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
                // segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.Untouchable;

                if (RayCast(segmentInput, out RaycastOutput segmentOutput))
                {
                    HoveredSegmentId = segmentOutput.m_netSegment;
                }
                else
                {
                    // find train segments
                    segmentInput.m_netService.m_itemLayers =
                        ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                    segmentInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                    segmentInput.m_netService.m_subService =
                        ItemClass.SubService.PublicTransportTrain;
                    segmentInput.m_ignoreTerrain = true;
                    segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
                    // segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.Untouchable;

                    if (RayCast(segmentInput, out segmentOutput))
                    {
                        HoveredSegmentId = segmentOutput.m_netSegment;
                    }
                    else
                    {
                        // find metro segments
                        segmentInput.m_netService.m_itemLayers =
                            ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                        segmentInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                        segmentInput.m_netService.m_subService =
                            ItemClass.SubService.PublicTransportMetro;
                        segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.None;
                        // segmentInput.m_ignoreSegmentFlags = NetSegment.Flags.Untouchable;

                        if (RayCast(segmentInput, out segmentOutput))
                        {
                            HoveredSegmentId = segmentOutput.m_netSegment;
                        }
                    }
                }

                if (HoveredNodeId <= 0 && HoveredSegmentId > 0)
                {
                    // alternative way to get a node hit: check distance to start and end nodes
                    // of the segment
                    ushort startNodeId = Singleton<NetManager>
                                         .instance.m_segments.m_buffer[HoveredSegmentId]
                                         .m_startNode;
                    ushort endNodeId = Singleton<NetManager>
                                       .instance.m_segments.m_buffer[HoveredSegmentId].m_endNode;

                    NetNode[] nodesBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
                    float startDist = (segmentOutput.m_hitPos - nodesBuffer[startNodeId]
                                                                .m_position).magnitude;
                    float endDist = (segmentOutput.m_hitPos - nodesBuffer[endNodeId]
                                                              .m_position).magnitude;
                    if (startDist < endDist && startDist < 75f)
                    {
                        HoveredNodeId = startNodeId;
                    }
                    else if (endDist < startDist && endDist < 75f)
                    {
                        HoveredNodeId = endNodeId;
                    }
                }

                if (HoveredNodeId != 0 && HoveredSegmentId != 0)
                {
                    ref NetNode node = ref HoveredNodeId.ToNode();
                    ref NetSegment segment = ref HoveredSegmentId.ToSegment();
                    ushort otherNodeID = segment.GetOtherNode(HoveredNodeId);
                    ref NetNode otherNode = ref otherNodeID.ToNode();

                    LogWait($"mouse = ${m_mousePosition.ToString("000.000")}" +
                        $" hit={segmentOutput.m_hitPos.ToString("000.000")}\n" +
                        $"node:{node.m_position.ToString("000.000")}" +
                        $"other node:{otherNode.m_position.ToString("000.000")}",
                        3
                        );
                    if (drawpoints) {
                        c1 = m_mousePosition;
                        c2 = segmentOutput.m_hitPos;
                        DebugSeg();
                        drawpoints = false;
                    }
                    //HoveredSegmentId = GetHoveredSegmentFromNode(segmentOutput.m_hitPos);
                    //HoveredSegmentId = GetHoveredSegmentFromNode(m_mousePosition);
                }
            }

            return HoveredNodeId != 0 || HoveredSegmentId != 0;
        }

        /// <summary>
        /// returns the node(HoveredNodeId) segment that is closest to the mouse pointer.
        /// </summary>
        internal ushort GetHoveredSegmentFromNode(Vector3 hitPos)
        {
            ushort minSegId = 0;
            ref NetNode node = ref NetManager.instance.m_nodes.m_buffer[HoveredNodeId];
            float minDistance = float.MaxValue;
            for (int i = 0; i < 8; ++i) {
                ushort segmentId = node.GetSegment(i);
                ref NetSegment segment = ref segmentId.ToSegment();


                Vector3 pos = segment.GetClosestPosition(hitPos);
                float distance = (m_mousePosition - pos).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minSegId = segmentId;
                }
            }
            return minSegId;
        }
    }
}
