using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;
using System.Reflection.Emit;
using ThePerfect.CombatHistoryEntries;

namespace ThePerfect.Patches;

public class OrbCmdPatch {
    [HarmonyPatch]
    public static class PatchEvoke {
        static Type? GetNestedType() {
            Type parentClass = typeof(OrbCmd);
            Type[] stateMachineTypes = parentClass.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
            foreach (Type type in stateMachineTypes) {
                if (type.Name.Contains("Evoke") && type.Name.Contains("d__6")) {
                    return type;
                }
            }
            return null;
        }
        
        static MethodBase TargetMethod() {
            Type? type = GetNestedType();
            if (type != null) {
                Log.Info("Find Evoke method.");
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
                if (instructionList[i].opcode == OpCodes.Callvirt && (MethodInfo)instructionList[i].operand == AccessTools.Method(typeof(OrbModel), nameof(OrbModel.RemoveInternal), [])) {
                    foundIndex = i;
                    break;
                }
            }
            if (foundIndex == -1) {
                throw new Exception("Could not find target line.");
            }
            List<CodeInstruction> insertedCode = [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(GetNestedType(), "evokedOrb")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchEvoke), nameof(Insert), [typeof(OrbModel)]))
            ];
            instructionList.InsertRange(foundIndex + 1, insertedCode);

            return instructionList;
        }

        public static void Insert(OrbModel evokedOrb) {
            OrbEvokedEntry orbEvokedEntry = new OrbEvokedEntry(evokedOrb, evokedOrb.Owner.Creature.CombatState.RoundNumber, evokedOrb.Owner.Creature.CombatState.CurrentSide, CombatManager.Instance.History);
            AccessTools.Method(typeof(CombatHistory), "Add", [typeof(CombatHistoryEntry)]).Invoke(CombatManager.Instance.History, [orbEvokedEntry]);
        }
    }
}
