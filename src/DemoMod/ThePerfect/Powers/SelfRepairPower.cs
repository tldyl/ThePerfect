using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace ThePerfect.Powers;

public class SelfRepairPower : CustomPowerModel {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCombatVictory(CombatRoom room) {
        Flash();
        await CreatureCmd.Heal(Owner, Amount);
    }
}
