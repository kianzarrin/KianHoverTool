using UnityEngine;
using static Kian.Mod.ShortCuts;

namespace KianHoverElements.DebugUtils {
    public class DrawTool : MonoBehaviour {
        public DebugDraw DebugDraw = new DebugDraw();
        public Vector3? c1;
        public Vector3? c2;

        public void Update() {
            LogWait("Update is called ", 1);
            if (c1 != null) {
                LogWait("Drawing debug sphere :)", 2);
                DebugDraw.DrawSphere(Vector3.zero, 1, Color.yellow);
                DebugDraw.DrawSphere(Vector3.zero, 10, Color.yellow);
                DebugDraw.DrawSphere(Vector3.zero, 100, Color.yellow);
                DebugDraw.DrawSphere(Vector3.zero, 1000, Color.yellow);
            }
            if (c2 != null) {
                DebugDraw.DrawSphere((Vector3)c2, 10, Color.red);
            }
            if (c1 != null && c2 != null) {
                Vector3 v1 = (Vector3)c1 + new Vector3(10f, 10f, 10f);
                Vector3 v2 = (Vector3)c2 + new Vector3(10f, 10f, 10f);

                LogWait("Drawing debug line :)", 3);
                Debug.DrawLine(v1, v2, Color.red, 10);
            }
        }

    }

}
