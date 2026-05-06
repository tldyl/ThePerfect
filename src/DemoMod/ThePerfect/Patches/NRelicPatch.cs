using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Relics;
using ThePerfect.Relics;

namespace ThePerfect.Patches;

public class NRelicPatch {
    [HarmonyPatch(typeof(NRelic), "set_Model")]
    public static class PatchReload {
        public static void Postfix(NRelic __instance) {
            if (__instance.Model is IClickableRelic clickableRelic) {
                Log.Info("Checking clickable relic signal connection.");
                __instance.Connect(Control.SignalName.GuiInput, Callable.From(new Action<InputEvent>(clickableRelic.OnGuiInput)));
            }
        }
    }
}
