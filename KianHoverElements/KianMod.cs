using ICities;
using ColossalFramework;
using KianPatch;
using UnityEngine;

namespace KianHoverElements
{
    public class KianModInfo : IUserMod {
        public string Name => "Kian hovering tool";
        public string Description => "a basic tool with hover-click functionality";
    }

    public class LoadingExtention : LoadingExtensionBase {
        KianTool tool;
        public override void OnLevelLoaded(LoadMode mode) {
            base.OnLevelLoaded(mode);
            tool = new KianTool();
        }
        public override void OnLevelUnloading() {
            base.OnLevelUnloading();
            tool = null;
        }
        public override void OnCreated(ILoading loading) {
            Debug.Log("OnCreated begins");
            base.OnCreated(loading);
            Hook.UnhookAll();
            Hook.HookGetSegmentColor();
            Debug.Log("OnCreated ends");

        }
        public override void OnReleased() {
            Debug.Log("OnReleased begins");
            Hook.UnhookAll();
            base.OnReleased();
            Debug.Log("OnReleased ends");

        }
    }
    public static class ShortCuts {
        internal static ref NetNode Node(ushort ID) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[ID];
        internal static ref NetSegment Segment(ushort ID) => ref Singleton<NetManager>.instance.m_segments.m_buffer[ID];
        internal static NetManager netMan => Singleton<NetManager>.instance;
    }
} // end namesapce
