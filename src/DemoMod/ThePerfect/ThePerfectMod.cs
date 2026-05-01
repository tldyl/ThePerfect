using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace ThePerfect;

[ModInitializer(nameof(initialize))]
public class ThePerfectMod {
    public static void initialize() {
        Harmony harmony = new Harmony("ThePerfectMod");
        harmony.PatchAll();

        ScriptManagerBridge.LookupScriptsInAssembly(typeof(ThePerfectMod).Assembly);
    }
}
