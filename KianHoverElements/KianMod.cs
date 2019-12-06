using ICities;
using ColossalFramework;
using UnityEngine;

using Kian.HoverTool;
using Kian.Patch;
using System;

namespace Kian.Mod
{
    public class KianModInfo : IUserMod {
        public string Name => "Kian toggle color";
        public string Description => "kian hovering tool that toggles a single segmetns color";
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
            base.OnCreated(loading);
            try {
                //Hook.HookAll();
            }
            catch (Exception e) {
                Debug.Log("HookAll() threw exception");
            }

        }
        public override void OnReleased() {
            Debug.Log("OnReleased begins");
            Hook.UnHookAll();
            base.OnReleased();


        }
    }
    public static class ShortCuts {
        internal static ref NetNode Node(ushort ID) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[ID];
        internal static ref NetSegment Segment(ushort ID) => ref Singleton<NetManager>.instance.m_segments.m_buffer[ID];
        internal static NetManager netMan => Singleton<NetManager>.instance;
    }
} // end namesapce
