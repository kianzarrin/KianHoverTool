using System;
using UnityEngine;
using System.Reflection;
using static Kian.Mod.ShortCuts;

namespace Kian.Patch {
    public abstract class HookBase {
        public static HookBase instance;
        public HookBase() => instance = this;

        private RedirectCallsState State = null;
        public abstract MethodInfo From { get; }
        public abstract MethodInfo To { get; }

        public void Hook() {
            MethodInfo from = From;
            MethodInfo to = To;
            LogOnce($"Hooking {from}  to {To} ...");
            if (From == null || To == null) {
                Debug.LogError("hooking failed.");
                return;
            }
            State = RedirectionHelper.RedirectCalls(from, to);
        }
        public void UnHook() {
            if (State != null) {
                MethodInfo from = From;
                LogOnce($"UnHooking {from} ...");
                RedirectionHelper.RevertRedirect(from, State);
                State = null;
            }
        }
    }
}
