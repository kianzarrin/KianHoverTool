using ColossalFramework.Plugins;
using System.Linq;
using static PedBridge.Utils.Helpers;
using ICities;

namespace PedBridge.Utils {
    public static class PluginUtils {
        public static bool FineRoadToolDetected => PluginDetected("FineRoadTool");
        static bool PluginDetected(string name) {
            try {
                foreach (PluginManager.PluginInfo current in PluginManager.instance.GetPluginsInfo()) {
                    IUserMod mod = current.userModInstance as IUserMod;

                    Log("checking plugin ... " + current.name);
                    Log("checking mod ... " + mod.Name);

                    if (current.isEnabled && mod.GetType().Assembly.FullName.Contains(name)) {
                        Log("found plugin" + name);
                        return true;
                    }
                }
            }
            catch { }
            Log("PLUGING NOT FOUND: " + name);
            return false;
        } // end FineRoadToolDetected
    } // end PluginUtils
} // end namesapce

