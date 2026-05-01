using BaseLib.Abstracts;
using Godot;

namespace ThePerfect.Pools;

public class PerfectCardPool : CustomCardPoolModel {
    public override string Title => "perfect_card_pool";
    // 描述中使用的能量图标。大小为24x24。
    //public override string? TextEnergyIconPath => "res://test/images/energy_test.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    //public override string? BigEnergyIconPath => "res://test/images/energy_test_big.png";
    public override Color DeckEntryCardColor => new Color("3EB3ED");
    public override Color ShaderColor => new Color("3EB3ED");
    public override bool IsColorless => false;
}
