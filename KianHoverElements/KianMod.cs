using ICities;
using ColossalFramework;
using UnityEngine;
using System;

using Kian.HoverTool;


namespace Kian.Mod
{
    public class KianModInfo : IUserMod {
        public string Name => "Kian better hover";
        public string Description => "kian hovering tool that is clever about node to segment hovering";

        public void OnEnable() {
            if (ShortCuts.InGame)
                LoadTool.Load();
        }
        public void OnDisable() {
            LoadTool.Release();
        }
    }

    public static class LoadTool {
        public static KianTool tool;
        public static void Load() {
            Release();
            tool = new KianTool();
        }
        public static void Release() {
            tool?.Release();
            tool = null;
        }
    }

    public class LoadingExtention : LoadingExtensionBase {

        public override void OnLevelLoaded(LoadMode mode) {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                LoadTool.Load();
        }
        public override void OnLevelUnloading() {
            LoadTool.Release();
        }
    }
    public static class ShortCuts {
        internal static ref NetNode ToNode(this ushort ID) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[ID];
        internal static ref NetSegment ToSegment(this ushort ID) => ref Singleton<NetManager>.instance.m_segments.m_buffer[ID];
        internal static NetManager netMan => Singleton<NetManager>.instance;


        internal static AppMode currentMode => SimulationManager.instance.m_ManagersWrapper.loading.currentMode;
        internal static bool CheckGameMode(AppMode mode) => CheckGameMode(new[] { mode });
        internal static bool CheckGameMode(AppMode[] modes) {
            try {
                foreach (var mode in modes) {
                    if (currentMode == mode)
                        return true;
                }
            }
            catch { }
            return false;
        }
        internal static bool InGame => CheckGameMode(AppMode.Game);
        internal static bool InAssetEditor => CheckGameMode(AppMode.AssetEditor);
    }
} // end namesapce
