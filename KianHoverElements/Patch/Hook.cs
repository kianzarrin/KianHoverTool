using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
//using ColossalFramework;

namespace Kian.Patch {
    public static class Hook {
        public static List<HookBase> hooks = new List<HookBase>();

        public static void HookAll() {
            hooks.Add(new GetSegmentColor());
            hooks.Add(new GetNodeColor());
            hooks.Add(new RenderInstance());
            Debug.Log("hooked everything");
        }
        public static void UnHookAll() {
            foreach (var h in hooks) {
                h.UnHook();
            }
            Debug.Log("unhooked everything");
        }

        public class GetSegmentColor : HookBase {
            private Type[] args => new[] { typeof(ushort), typeof(NetSegment).MakeByRefType(), typeof(InfoManager.InfoMode) };
            public override MethodInfo From => typeof(RoadAI).GetMethod("GetColor", args);
            public override MethodInfo To => typeof(ColorDetours).GetMethod("GetSegmentColor");
        }

        public class GetNodeColor : HookBase {
            private Type[] args => new[] { typeof(ushort), typeof(NetNode).MakeByRefType(), typeof(InfoManager.InfoMode) };
            public override MethodInfo From => typeof(RoadAI).GetMethod("GetColor", args);
            public override MethodInfo To => typeof(ColorDetours).GetMethod("GetNodeColor");
        }

        /*private void NetSegment.RenderInstance(
         * RenderManager.CameraInfo cameraInfo,
         * ushort segmentID,
         * int layerMask,
         * NetInfo info,
         * ref RenderManager.Instance data) */
        public class RenderInstance : HookBase {
            private Type[] args => new[] {
                typeof(RenderManager.CameraInfo),
                typeof(ushort),
                typeof(int),
                typeof(NetInfo),
                typeof(RenderManager.Instance).MakeByRefType()
            };
            public override MethodInfo From => typeof(NetSegment).GetMethod("RenderInstance", args);
            public override MethodInfo To => typeof(ColorDetours).GetMethod("RenderInstance");
        }

        // // private static void NetTool.RenderSegment(NetInfo info, NetSegment.Flags flags, Vector3 startPosition, Vector3 endPosition, Vector3 startDirection, Vector3 endDirection, bool smoothStart, bool smoothEnd)
        //public class RenderSegment : HookBase {
        //    public override MethodInfo From => typeof(NetTool).GetMethod("RenderSegment");
        //    public override MethodInfo To => typeof(ColorDetours).GetMethod("RenderSegment");
        //}

        // //private static void NetTool.RenderNode(NetInfo info, Vector3 position, Vector3 direction)
        //public class RenderNode : HookBase {
        //    public override MethodInfo From => typeof(NetTool).GetMethod("RenderNode");
        //    public override MethodInfo To => typeof(ColorDetours).GetMethod("RenderNode");
        //}

    } // end class Hook
}