using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Orbs;
using System.Reflection;
using System.Reflection.Emit;
using ThePerfect.Powers;

namespace ThePerfect.Patches;
public class LightningOrbPatch {
    [HarmonyPatch]
    public static class PatchApplyLightningDamage {
        static Type? GetNestedType() {
            Type parentClass = typeof(LightningOrb);
            Type[] stateMachineTypes = parentClass.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
            foreach (Type type in stateMachineTypes) {
                if (type.Name.Contains("ApplyLightningDamage") && type.Name.Contains("d__15")) {
                    return type;
                }
            }
            return null;
        }
        
        static MethodBase TargetMethod() {
            Type? type = GetNestedType();
            if (type != null) {
                Log.Info("Find ApplyLightningDamage method.");
                MethodInfo moveNext = type.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (moveNext != null) {
                    return moveNext;
                }
            }
            return null;
        }
        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            List<CodeInstruction> instructionList = instructions.ToList();
        
            int foundIndex = -1;
            for (int i = 0; i < instructionList.Count; i++) {
                if (instructionList[i].opcode == OpCodes.Ldarg_0 && instructionList[i + 1].opcode == OpCodes.Ldfld && instructionList[i + 2].opcode == OpCodes.Brfalse_S) {
                    foundIndex = i;
                    break;
                }
            }
            if (foundIndex == -1) {
                throw new Exception("Could not find target line.");
            }
            int pos = foundIndex;
            instructionList[pos++].opcode = OpCodes.Ldarg_0;
            instructionList[pos++].opcode = OpCodes.Ldloc_1;
            instructionList[pos++].opcode = OpCodes.Ldarg_0;
            instructionList[pos++] = new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(GetNestedType(), "target"));
            instructionList[pos++].opcode = OpCodes.Ldloc_3;
            instructionList[pos++] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchApplyLightningDamage), nameof(Insert), [typeof(LightningOrb), typeof(Creature), typeof(List<Creature>)]));
            for (int i = pos; i < foundIndex + 18; i++) {
                instructionList[i].opcode = OpCodes.Nop;
            }
        
            return instructionList;
        }
        
        public static IReadOnlyList<Creature> Insert(LightningOrb __instance, Creature? target, List<Creature> list) {
            IReadOnlyList<Creature> targets = [];
            if (__instance.Owner.HasPower<ElectrodynamicsPower>()) {
                targets = list;
            } else {
                targets = target == null ? [__instance.Owner.RunState.Rng.CombatTargets.NextItem(list)] : [target];
            }
            return targets;
        }
    }
}
