using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ThePerfect.Powers;

public class NextTurnFocusPower : CustomPowerModel {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool thisTurnEnd = false;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) {
        if (side == CombatSide.Player) {
            if (thisTurnEnd) {
                await PowerCmd.Apply<FocusPower>(Owner, Amount, Owner, null);
                await PowerCmd.Remove(this);
            }
            thisTurnEnd = true;
        }
    }
}
