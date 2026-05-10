using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ThePerfect.Powers;

public class ResetPower : CustomPowerModel {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) {
        if (Owner != player.Creature) {
            return;
        }
        await PowerCmd.TickDownDuration(this);
    }
    
    public override bool ShouldTakeExtraTurn(Player player) {
        return Owner == player.Creature;
    }
}
