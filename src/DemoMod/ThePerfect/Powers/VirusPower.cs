using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ThePerfect.Hooks;

namespace ThePerfect.Powers;

public class VirusPower : CustomPowerModel, IBeforeOrbChanneled {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public bool BeforeOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb) {
        if (Owner.IsPlayer && player == Owner.Player) {
            Flash();
            OrbModel leftOrb = player.PlayerCombatState.OrbQueue.Orbs.Last();
            if (leftOrb != null) {
                for (int i = 0; i < Amount; i++) {
                    leftOrb.Passive(choiceContext, null);
                }
            }
            OrbModel rightOrb = player.PlayerCombatState.OrbQueue.Orbs.First();
            if (rightOrb != null) {
                for (int i = 0; i < Amount; i++) {
                    rightOrb.Passive(choiceContext, null);
                }
            }
        }
        return Owner.IsPlayer && player == Owner.Player;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) {
        if (side != Owner.Side) {
            return;
        }
        await PowerCmd.Remove(this);
    }
}
