using ICities;
using ColossalFramework;
using UnityEngine;

using Kian.HoverTool;
using System;

namespace Kian.Mod
{
    public class KianModInfo : IUserMod {
        public string Name => "Kian better hover";
        public string Description => "kian hovering tool that is clever about node to segment hovering";
    }

    public class LoadingExtention : LoadingExtensionBase {
        KianTool tool;
        public override void OnLevelLoaded(LoadMode mode) {
            tool = new KianTool();
        }
        public override void OnLevelUnloading() {
            tool = null;
        }
        public override void OnCreated(ILoading loading) {

        }
        public override void OnReleased() {
            Debug.Log("OnReleased begins");
            base.OnReleased();
        }
    }
    public static class ShortCuts {
        internal static ref NetNode ToNode(this ushort ID) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[ID];
        internal static ref NetSegment ToSegment(this ushort ID) => ref Singleton<NetManager>.instance.m_segments.m_buffer[ID];
        internal static NetManager netMan => Singleton<NetManager>.instance;
    }
} // end namesapce
