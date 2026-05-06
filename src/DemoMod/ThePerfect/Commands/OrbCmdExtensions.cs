using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ThePerfect.Commands;

public static class OrbCmdExtensions {
    public static async Task RemoveNext(
        PlayerChoiceContext choiceContext,
        Player player) {
        if (CombatManager.Instance.IsOverOrEnding)
            return;
        OrbQueue orbQueue = player.PlayerCombatState.OrbQueue;
        OrbModel removedOrb = orbQueue.Orbs.First();
        if (orbQueue.Orbs.Count <= 0)
            return;
        bool removed = orbQueue.Remove(removedOrb);
        if (player.Creature.CombatState == null || !removed)
            return;
        removedOrb.RemoveInternal();
    }
}
