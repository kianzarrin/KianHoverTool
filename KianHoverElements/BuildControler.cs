using System.Collections.Generic;
using System;

namespace PedBridge {
    using Utils;
    public static class BuildControler {
        //static float HWpb => info.GetElevated().m_halfWidth;
        static float HWpb => info.m_halfWidth; // TODO use elevated.
        static NetInfo info => PrefabUtils.Value;

        public static List<ushort> GetCWSegList(ushort nodeID) {
            NetNode node = nodeID.ToNode();
            ushort segmentID = 0;
            List<ushort> segList = new List<ushort>();
            int i;
            for (i = 0; i < 8; ++i) {
                segmentID = node.GetSegment(i);
                if (segmentID != 0)
                    break;
            }

            segList.Add(segmentID);

            while (true) {
                segmentID = segmentID.ToSegment().GetLeftSegment(nodeID);
                if (segmentID == segList[0])
                    break;
                else
                    segList.Add(segmentID);

            }
            return segList;
        }

        public static ushort CreateLBridge(ushort segID1, ushort segID2) {
            PosUtils.RLR rlr = new PosUtils.RLR {
                HWpb = HWpb,
                segID1 = segID1,
                segID2 = segID2
            };
            rlr.Calculate();
            ushort nodeID = segID1.ToSegment().GetSharedNode(segID2);
            NetNode node = nodeID.ToNode();
            float h = node.m_position.y;

            //NetService.LogSegmentDetails(segID1);
            Helpers.Log($"creating L from segments: {segID1} {segID2}");
            return NetService.CreateL(rlr.Point1, rlr.PointL, rlr.Point2, h, info);
        }

        public static void CreateJunctionBridge(ushort nodeID) {
            if (nodeID.ToNode().CountSegments() != 4)
                throw new NotImplementedException("number of segments is not 4");
            List<ushort> segList = GetCWSegList(nodeID);
            if (segList.Count != 4)
                throw new Exception($"seglist count is ${segList.Count} expected 4");
            int n = segList.Count;
            var nodeList = new List<ushort>();
            for (int i = 0; i < n; ++i) {
                ushort newNodeID = CreateLBridge(segList[i], segList[(i + 1) % n]);
                nodeList.Add(newNodeID);
            }
            for (int i = 0; i < n; ++i) {
                NetService.CreateSegment(nodeList[i], nodeList[(i + 1) % n]);
            } // end for
        } // end method

    } // end calss
} // end namespace
