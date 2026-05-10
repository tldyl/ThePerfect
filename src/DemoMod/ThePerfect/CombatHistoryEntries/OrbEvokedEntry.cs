using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Models;

namespace ThePerfect.CombatHistoryEntries;

public class OrbEvokedEntry : CombatHistoryEntry {
    public OrbModel Orb { get; }
    
    public OrbEvokedEntry(OrbModel orb,
        int roundNumber,
        CombatSide currentSide,
        CombatHistory history)
        : base(orb.Owner.Creature, roundNumber, currentSide, history) {
        Orb = orb;
    }

    public override string Description => Actor.Player.Character.Id.Entry + " evoked " + Orb.Id.Entry;
}
