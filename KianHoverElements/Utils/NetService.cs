using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using ColossalFramework;
using UnityEngine;

namespace PedBridge.Utils {
    public static class NetService {
        public static NetManager netMan => Singleton<NetManager>.instance;
        public static SimulationManager simMan => Singleton<SimulationManager>.instance;
        public static NetTool netTool = Singleton<NetTool>.instance;

        public static void LogSegmentDetails(ushort segmentID) {
            var segment = segmentID.ToSegment();
            var startPos = segment.m_startNode.ToNode().m_position;
            var endPos = segment.m_endNode.ToNode().m_position;
            var startDir = segment.m_startDirection;
            var endDir = segment.m_endDirection;
            var end2start = endPos - startPos;
            var end2start_normal = end2start; end2start_normal.y = 0; end2start_normal.Normalize();
            var f = "000.000";
            string m = $"segment:{segmentID},\n" +
                $"startPos:{startPos.ToString(f)}, endPos:{endPos.ToString(f)},\n" +
                $"startDir:{startDir.ToString(f)}, endDir:{endDir.ToString(f)},\n" +
                $"end2start={end2start.ToString(f)}, end2start_normal={end2start_normal.ToString(f)}";
            Helpers.Log(m);
        }


        public static NetInfo _bInfo = null;
        public static NetInfo _pInfo = null;

        public static NetInfo PedestrianBridgeInfo =>
            _bInfo = _bInfo ?? GetInfo("Pedestrian Elevated");

        public static NetInfo PedestrianPathInfo =>
            _pInfo = _pInfo ?? GetInfo("Pedestrian Pavement");

        public static NetInfo GetInfo(string name) {
            int count = PrefabCollection<NetInfo>.LoadedCount();
            for (uint i = 0; i < count; ++i) {
                NetInfo info = PrefabCollection<NetInfo>.GetLoaded(i);
                if (info.name == name)
                    return info;
                //Helpers.Log(info.name);
            }
            throw new Exception("NetInfo not found!");
        }


        public static ushort CreateNode(Vector3 position, NetInfo info = null) {
            info = info ?? PedestrianBridgeInfo;
            Helpers.Log($"creating node for {info.name} at position {position.ToString("000.000")}");
            bool res = netMan.CreateNode(node: out ushort nodeID, randomizer: ref simMan.m_randomizer,
                info: info, position: position, buildIndex: simMan.m_currentBuildIndex);
            if (!res)
                throw new Exception("Node creation failed");
            simMan.m_currentBuildIndex++;
            return nodeID;

        }

        public static ushort CreateSegment(
            ushort startNodeID, ushort endNodeID,
            Vector3 startDir, Vector3 endDir,
            NetInfo info = null)
        {
            info = info ?? startNodeID.ToNode().Info;
            Helpers.Log($"creating segment for {info.name} between nodes {startNodeID} {endNodeID}");
            var bi = simMan.m_currentBuildIndex;
            bool res = netMan.CreateSegment(
                segment: out ushort segmentID, randomizer: ref simMan.m_randomizer, info: info,
                startNode: startNodeID, endNode: endNodeID,
                startDirection: startDir.normalized, endDirection: endDir.normalized,
                buildIndex: bi, modifiedIndex: bi, invert: false);
            if (!res)
                throw new Exception("Segment creation failed");
            simMan.m_currentBuildIndex++;
            return segmentID;
        }

        public static ushort CreateSegment(ushort startNodeID, ushort endNodeID) {
            Vector3 startPos = startNodeID.ToNode().m_position;
            Vector3 endPos = endNodeID.ToNode().m_position;
            var dir = endPos - startPos;
            dir.y = 0;
            dir.Normalize();
            return CreateSegment(startNodeID, endNodeID, dir, -dir);
        }

        public static ushort CreateSegment(ushort startNodeID, ushort endNodeID, Vector2 middlePoint) {
            Vector3 startPos = startNodeID.ToNode().m_position;
            Vector3 endPos = endNodeID.ToNode().m_position;
            Vector3 middlePos = middlePoint.ToPos();
            Vector3 startDir = middlePos - startPos;
            Vector3 endDir = middlePos - endPos;
            startDir.y = endDir.y = 0;
            startDir.Normalize();
            endDir.Normalize();
            return CreateSegment(startNodeID, endNodeID,startDir,endDir);
        }


        public static float GetClosestHeight(this ushort segmentID, Vector3 Pos) =>
            segmentID.ToSegment().GetClosestPosition(Pos).Height();

        public static void SetGroundNode(ushort nodeID) {
            NetNode node = nodeID.ToNode();
            node.m_elevation = 0;
            node.m_building = 0;
            node.m_flags &= ~NetNode.Flags.Moveable;
            node.m_flags |= NetNode.Flags.Transition | NetNode.Flags.OnGround;
        }

        public static ushort CreateL(Vector2 point1, Vector2 pointL, Vector2 point2, float h) {
            NetInfo info = PedestrianBridgeInfo;
            Vector3 pos1 = point1.ToPos(h);
            Vector3 pos2 = point2.ToPos(h);
            Vector3 posL = pointL.ToPos(h + 10);

            ushort nodeIDL = CreateNode(posL);
            ushort nodeID1 = CreateNode(pos1);
            ushort nodeID2 = CreateNode(pos2);
            SetGroundNode(nodeID1);
            SetGroundNode(nodeID2);
            CreateSegment(nodeIDL, nodeID1);
            CreateSegment(nodeIDL, nodeID2);
            SetGroundNode(nodeID1);
            SetGroundNode(nodeID2);

            return nodeIDL;
        }
     }
}
