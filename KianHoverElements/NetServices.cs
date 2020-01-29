using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using ColossalFramework;
using UnityEngine;
using System.Diagnostics;
using System.Timers;

namespace Kian.Utils {
    using Kian.Mod;
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
            ShortCuts.Log(m);
        }

        public static NetInfo _bInfo = null;
        public static NetInfo _bInfo2 = null;

        public static NetInfo PedestrianBridgeInfo =>
            _bInfo = _bInfo ?? GetInfo("Pedestrian Elevated");
        public static NetInfo GravelBridgeInfo =>
            _bInfo2 = _bInfo2 ?? GetInfo("Pedestrian Gravel Elevated");

        public static NetInfo GetInfo(string name) {
            int count = PrefabCollection<NetInfo>.LoadedCount();
            for (uint i = 0; i < count; ++i) {
                NetInfo info = PrefabCollection<NetInfo>.GetLoaded(i);
                if (info.name == name)
                    return info;
                //ShortCuts.Log(info.name);
            }
            throw new Exception("NetInfo not found!");
        }

        public class NetServiceException : Exception {
            public NetServiceException(string m) : base(m) { }
            public NetServiceException() : base() { }
            public NetServiceException(string m, Exception e) : base(m, e) { }
        }

        public static ushort CreateNode(Vector3 position, NetInfo info = null) {
            info = info ?? PedestrianBridgeInfo;
            ShortCuts.Log($"creating node for {info.name} at position {position.ToString("000.000")}");
            bool res = netMan.CreateNode(node: out ushort nodeID, randomizer: ref simMan.m_randomizer,
                info: info, position: position, buildIndex: simMan.m_currentBuildIndex);
            if (!res)
                throw new NetServiceException("Node creation failed");
            simMan.m_currentBuildIndex++;
            return nodeID;

        }

        public static ushort CreateSegment(
            ushort startNodeID, ushort endNodeID,
            Vector3 startDir, Vector3 endDir,
            NetInfo info = null) {
            ShortCuts.Log("CreateSegment input info is " + info?.name);
            info = info ?? startNodeID.ToNode().Info;
            ShortCuts.Log($"creating segment for {info.name} between nodes {startNodeID} {endNodeID}");
            var bi = simMan.m_currentBuildIndex;
            startDir.y = endDir.y = 0;
            startDir.Normalize(); endDir.Normalize();
            bool res = netMan.CreateSegment(
                segment: out ushort segmentID, randomizer: ref simMan.m_randomizer, info: info,
                startNode: startNodeID, endNode: endNodeID, startDirection: startDir, endDirection: endDir,
                buildIndex: bi, modifiedIndex: bi, invert: false);
            if (!res)
                throw new NetServiceException("Segment creation failed");
            simMan.m_currentBuildIndex++;
            return segmentID;
        }

        public static ushort CreateSegment(ushort startNodeID, ushort endNodeID, NetInfo info = null) {
            Vector3 startPos = startNodeID.ToNode().m_position;
            Vector3 endPos = endNodeID.ToNode().m_position;
            var dir = endPos - startPos;
            return CreateSegment(startNodeID, endNodeID, dir, -dir, info);
        }

        public static ushort CreateSegment(ushort startNodeID, ushort endNodeID, Vector2 middlePoint, NetInfo info = null) {
            Vector3 startPos = startNodeID.ToNode().m_position;
            Vector3 endPos = endNodeID.ToNode().m_position;
            Vector3 middlePos = middlePoint.ToPos();
            Vector3 startDir = middlePos - startPos;
            Vector3 endDir = middlePos - endPos;
            return CreateSegment(startNodeID, endNodeID, startDir, endDir,info);
        }


        public static float GetClosestHeight(this ushort segmentID, Vector3 Pos) =>
            segmentID.ToSegment().GetClosestPosition(Pos).Height();


        public static ushort CopyMove(ushort segmentID) {
            ShortCuts.Log("CopyMove");
            Vector3 move = new Vector3(70, 0, 70);
            var segment = segmentID.ToSegment();
            var startPos = segment.m_startNode.ToNode().m_position + move;
            var endPos = segment.m_endNode.ToNode().m_position + move;
            NetInfo info = GetInfo("Basic Road");
            ushort nodeID1 = CreateNode(startPos, info);
            ushort nodeID2 = CreateNode(endPos, info);
            return CreateSegment(nodeID1, nodeID2);
        }
    }

    public static class VectorUtils {
        public static float Angle(this Vector2 v) => Vector2.Angle(v, Vector2.right);

        public static Vector2 Rotate(this Vector2 v, float angle) => Vector2ByAgnle(v.magnitude, angle + v.Angle());
        public static Vector2 Rotate90CCW(this Vector2 v) => new Vector2(-v.y, +v.x);
        public static Vector2 PerpendicularCCW(this Vector2 v) => v.normalized.Rotate90CCW();
        public static Vector2 Rotate90CC(this Vector2 v) => new Vector2(+v.y, -v.x);
        public static Vector2 PerpendicularCC(this Vector2 v) => v.normalized.Rotate90CC();

        public static Vector2 Extend(this Vector2 v, float magnitude) => NewMagnitude(v, magnitude + v.magnitude);
        public static Vector2 NewMagnitude(this Vector2 v, float magnitude) => magnitude * v.normalized;

        /// <param name="angle">angle in degrees with Vector.right</param>
        public static Vector2 Vector2ByAgnle(float magnitude, float angle) {
            angle *= Mathf.Deg2Rad;
            return new Vector2(
                x: magnitude * Mathf.Cos(angle),
                y: magnitude * Mathf.Sin(angle)
                );
        }
        /// returns rotated vector counter clockwise
        ///
        public static Vector3 ToPos(this Vector2 v2, float h = 0) => new Vector3(v2.x, h, v2.y);
        public static Vector2 ToPoint(this Vector3 v3) => new Vector2(v3.x, v3.z);
        public static float Height(this Vector3 v3) => v3.y;
    }
}
