using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using ColossalFramework;
using UnityEngine;

namespace PedBridge.Utils {
    public static class NetCreation {
        public static NetManager netMan => Singleton<NetManager>.instance;
        public static SimulationManager simMan => Singleton<SimulationManager>.instance;

        public static NetInfo PedastrianBridgeInfo()  {
            int count = PrefabCollection<NetInfo>.LoadedCount();
            for(uint i = 0; i < count; ++i) {
                NetInfo info = PrefabCollection<NetInfo>.GetLoaded(i);
                if (info.m_netAI is PedestrianBridgeAI)
                    return info;
            }
            return null;
        }

        public static bool CreateNode(out ushort node, Vector3 pos) {
            NetInfo info = PedastrianBridgeInfo();
            bool ret = netMan.CreateNode(out node, ref simMan.m_randomizer, info, pos, simMan.m_currentBuildIndex);
            if (ret)
                simMan.m_currentBuildIndex++;
            return ret;
        }

        public static bool CreateSegment(out ushort segmentID, ushort startNodeID, ushort endNodeID, Vector3 startDir, Vector3 endDir) {
            NetInfo info = PedastrianBridgeInfo();
            bool ret = netMan.CreateSegment(out segmentID, ref simMan.m_randomizer, info,
                startNodeID, endNodeID, startDir, endDir,
                simMan.m_currentBuildIndex, simMan.m_currentBuildIndex, false);
            if(ret)simMan.m_currentBuildIndex++;
            return ret;
        }

        public static ushort CreateL(Vector2 point1, Vector2 point0, Vector2 point2, float h) {
            NetInfo info = PedastrianBridgeInfo();
            Vector3 pos1 = point1.ToVector3(h);
            Vector3 pos2 = point2.ToVector3(h);
            Vector3 pos0 = point0.ToVector3(h + 10);
            Vector3 dir1 = (point1 - point0).ToVector3();
            Vector3 dir2 = (point2 - point0).ToVector3();

            bool b = CreateNode(out ushort nodeID0, pos0);
            b &= CreateNode(out ushort nodeID1, pos1);
            b &= CreateNode(out ushort nodeID2, pos2);

            b &= CreateSegment(out ushort segmentID1, nodeID0, nodeID1, dir1, -dir1);
            b &= CreateSegment(out ushort segmentID2, nodeID0, nodeID2, dir2, -dir2);
            if (b)
                return nodeID0;
            return 0;
        }
    }
}
