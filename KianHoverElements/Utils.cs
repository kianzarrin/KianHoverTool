using ICities;
using ColossalFramework;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Kian.Utils {
    public static class TextureUtils {
        //Texture flipping script from https://gist.github.com/Cgameworld/f22cfe649a222faf8226e1d65a0782e1
        public static void FlipRoadNodeTextures() {
            string road = "Basic Road";
            var nodeMaterial = PrefabCollection<NetInfo>.FindLoaded(road).m_nodes[0].m_nodeMaterial;
            Flip(nodeMaterial, "_MainTex");
            //Flip(nodeMaterial, "_APRMap");
        }

        public static void Flip(Material material, string name) {
            var nodeTextureMain = material.GetTexture(name) as Texture2D;
            byte[] bytes = nodeTextureMain.MakeReadable().EncodeToPNG();
            Texture2D newTexture = new Texture2D(1, 1);
            newTexture.LoadImage(bytes);
            newTexture.anisoLevel = 16;
            newTexture = FlipTexture(newTexture);
            newTexture.Compress(true);
            material.SetTexture(name, newTexture);
        }


        //Texture flipping script from https://stackoverflow.com/questions/35950660/unity-180-rotation-for-a-texture2d-or-maybe-flip-both
        static Texture2D FlipTexture(Texture2D original, bool upSideDown = true) {
            Texture2D flipped = new Texture2D(original.width, original.height);

            int xN = original.width;
            int yN = original.height;


            for (int i = 0; i < xN; i++) {
                for (int j = 0; j < yN; j++) {
                    if (upSideDown) {
                        flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
                    } else {
                        flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
                    }
                }
            }
            flipped.Apply();

            return flipped;
        }

    }


    public static class ShortCuts {
        internal static ref NetNode Node(ushort ID) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[ID];
        internal static ref NetSegment Segment(ushort ID) => ref Singleton<NetManager>.instance.m_segments.m_buffer[ID];
        internal static NetManager netMan => Singleton<NetManager>.instance;
        internal static bool IsJunction(ushort nodeID) => (Node(nodeID).m_flags & NetNode.Flags.Junction) != 0;
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

    public static class Bin {
        //Texture flipping script from https://gist.github.com/boformer/6524899363e97cedf45b
        public static void ReplaceTextures() {
            var networkName = "Basic Road"; // replace with the one you want to edit

            var segmentMaterial = PrefabCollection<NetInfo>.FindLoaded(networkName).m_segments[0].m_segmentMaterial;
            Texture2D texture2D;
            if (File.Exists("tt/" + networkName + "_D.png")) {
                texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(File.ReadAllBytes("tt/" + networkName + "_D.png"));
                texture2D.anisoLevel = 0;
                segmentMaterial.SetTexture("_MainTex", texture2D);
            }
            if (File.Exists("tt/" + networkName + "_APR.png")) {
                texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(File.ReadAllBytes("tt/" + networkName + "_APR.png"));
                texture2D.anisoLevel = 0;
                segmentMaterial.SetTexture("_APRMap", texture2D);
            }
            if (File.Exists("tt/" + networkName + "_XYS.png")) {
                texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(File.ReadAllBytes("tt/" + networkName + "_XYS.png"));
                texture2D.anisoLevel = 0;
                segmentMaterial.SetTexture("_XYSMap", texture2D);
            }
        }

        //Texture flipping script from https://gist.github.com/Cgameworld/f22cfe649a222faf8226e1d65a0782e1
        public static void FlipRoadNodeTextures() {
            string[] roads = { "Basic Road", "Large Road Decoration Grass", "Large Oneway Decoration Grass" };

            for (int i = 0; i < roads.Length; i++) {
                var nodeMaterial = PrefabCollection<NetInfo>.FindLoaded(roads[i]).m_nodes[0].m_nodeMaterial;
                TextureUtils.Flip(nodeMaterial, "_MainTex");
                //Flip(nodeMaterial, "_APRMap");
            }
        }
    }


}
