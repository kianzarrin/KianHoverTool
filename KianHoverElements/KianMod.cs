using ICities;

namespace PedBridge.Mod
{
    using HoverTool;
    using static Utils.Helpers;


    public class KianModInfo : IUserMod {
        public string Name => "Pedestarian bridge";
        public string Description => "Automatically build pedestarian bridges over junctions or roundabouts.";

        public void OnEnabled() {
            System.IO.File.WriteAllText("mod.debug.log", ""); //clear file
            Log("IUserMod.OnEnabled");

            if (InGame)
                LoadTool.Load();
        }

        public void OnDisabled() {
            Log("IUserMod.OnDisabled");
            LoadTool.Release();
        }
    }

    public static class LoadTool {
        public static void Load() {
            PedBridgeTool.Create();
            Log("LoadTool:Created kian tool.");
        }
        public static void Release() {
            PedBridgeTool.Remove();
            Log("LoadTool:Removed kian tool.");
        }
    }

    public class LoadingExtention : LoadingExtensionBase {
        public override void OnLevelLoaded(LoadMode mode) {
            Log("LoadingExtention.OnLevelLoaded");
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                LoadTool.Load();
        }

        public override void OnLevelUnloading() {
            Log("LoadingExtention.OnLevelUnloading");
            LoadTool.Release();
        }
    }
} // end namesapce
