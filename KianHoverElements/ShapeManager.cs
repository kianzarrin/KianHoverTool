using System.Collections.Generic;
using System;

namespace PedBridge {
    using Utils;
    public static class ShapeManager {
        static float HWpb => NetCreation.PedastrianBridgeInfo().m_halfWidth;

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

        public static void CreateLBridge(ushort segID1, ushort segID2) {
            PosUtils.RLR rlr = new PosUtils.RLR {
                HWpb = HWpb,
                segID1 = segID1,
                segID2 = segID2
            };
            rlr.Calculate();
            ushort nodeID = segID1.ToSegment().GetSharedNode(segID2);
            NetNode node = nodeID.ToNode();
            float h = node.m_position.y;
            NetCreation.CreateL(rlr.Point1, rlr.PointL, rlr.Point2, h);
        }

        public static void CreateJunctionBridge(ushort nodeID) {
            List<ushort> segList = GetCWSegList(nodeID);
            if (segList.Count != 4)
                throw new Exception($"seglist count is ${segList.Count} expected 4");
            CreateLBridge(segList[0], segList[1]);
            CreateLBridge(segList[1], segList[2]);
            CreateLBridge(segList[2], segList[3]);
            CreateLBridge(segList[3], segList[0]);

            //TODO connect nodes.

        }
    }

}
