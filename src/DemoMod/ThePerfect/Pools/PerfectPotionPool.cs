using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Helpers;

namespace ThePerfect.Pools;

public class PerfectPotionPool : CustomPotionPoolModel {
    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => "res://images/packed/sprite_fonts/defect_energy_icon.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/energy_defect.tres");
}
