using ICities;
using ColossalFramework;
using UnityEngine;

using Kian.HoverTool;
using Kian.Patch;
using System;
using System.Collections.Generic;

namespace Kian.Mod
{
    public class KianModInfo : IUserMod {
        public string Name => "Kian toggle color";
        public string Description => "kian hovering tool that toggles a single segmetns color";
    }

    public class LoadingExtention : LoadingExtensionBase {
        public override void OnLevelLoaded(LoadMode mode) {
            base.OnLevelLoaded(mode);
            Debug.Log("OnLevelLoaded kian mod");
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            toolModControl.AddComponent<KianTool>();
        }
        public override void OnLevelUnloading() {
            Debug.Log("OnLevelUnloading kian mod");
            base.OnLevelUnloading();
            GameObject toolModControl = ToolsModifierControl.toolController.gameObject;
            var tool = toolModControl.GetComponent<KianTool>();
            GameObject.DestroyObject(tool, 10);
            Hook.UnHookAll();
        }
        public override void OnCreated(ILoading loading) {
            Debug.Log("OnCreated kian mod");
            base.OnCreated(loading);
            Hook.Create();
            Hook.HookAll();
        }

        public override void OnReleased() {
            Debug.Log("OnReleased kian mod");
            Hook.Release();
            base.OnReleased();
        }
    }
    public static class ShortCuts {
        internal static ref NetNode Node(ushort ID) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[ID];
        internal static ref NetSegment Segment(ushort ID) => ref Singleton<NetManager>.instance.m_segments.m_buffer[ID];
        internal static NetManager netMan => Singleton<NetManager>.instance;
    }

    public static class LogOnceT {
        private static List<string> listLogs = new List<string>();
        public static void LogOnce(string m) {
            Debug.Log(m);
            return;
            var st = new System.Diagnostics.StackTrace();
            var sf = st.GetFrame(2);
            string key = sf.GetMethod().Name + ": " + m;
            //if (!listLogs.Contains(key))
            {
                Debug.Log(key);
                //listLogs.Add(key);
            }
        }
    }
} // end namesapce
