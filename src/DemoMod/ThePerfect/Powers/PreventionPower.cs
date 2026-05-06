using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace ThePerfect.Powers;

public class PreventionPower : CustomPowerModel {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2)
    ];
    
    public override async Task AfterModifyingPowerAmountReceived(PowerModel power) {
        if (power is not PreventionPower) {
            return;
        }
        
        DynamicVars.Energy.UpgradeValueBy(power.DynamicVars.Energy.BaseValue);
    }
}
