using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ThePerfect.Powers;

public class ElectrodynamicsPower : CustomPowerModel {
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
