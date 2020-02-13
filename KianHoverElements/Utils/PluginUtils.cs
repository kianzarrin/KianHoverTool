using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PedBridge.Utils {
    public static class PluginUtils {
        public static bool FineRoadToolDetected {
            get {
                try {
                    return null != Assembly.Load("FineRoadTool");
                }
                catch {
                    return false;
                } // end try .. catch
            } // end get
        } // end FineRoadToolDetected
    } // end PluginUtils
} // end namesapce

