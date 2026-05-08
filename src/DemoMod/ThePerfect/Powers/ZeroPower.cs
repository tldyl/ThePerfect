using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace ThePerfect.Powers;

public class ZeroPower : CustomPowerModel {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost) {
        if (card.Owner.Creature != Owner) {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = Math.Min(0, originalCost - Amount);
        return true;
    }
}
