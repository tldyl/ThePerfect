using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace ThePerfect.Powers;

public class SelfRepairPower : CustomPowerModel {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeApplied(
        Creature target,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource) {
        Removed += OnRemoved;
    }

    private void OnRemoved() {
        Log.Info($"SelfRepairPower removed. CombatManager.Instance.IsInProgress={CombatManager.Instance.IsInProgress}");
        if (!CombatManager.Instance.IsInProgress) {
            Flash();
            TaskHelper.RunSafely(CreatureCmd.Heal(Owner, Amount));
        }
    }
}
