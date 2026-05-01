using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using ThePerfect.Cards.PerfectCard;
using ThePerfect.Pools;

namespace ThePerfect.Characters;

public class ThePerfectCharacter : PlaceholderCharacterModel {
    // 角色名称颜色
    public override Color NameColor => StsColors.blue;
    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new Color("163E64FF");
    // 地图绘制颜色
    public override Color MapDrawingColor => new Color("0D638C");
    public override Color RemoteTargetingLineColor => new Color("70B6EDFF");
    public override Color RemoteTargetingLineOutline => new Color("163E64FF");
    public override Color DialogueColor => new Color("13446B");
    public override VfxColor SpeechBubbleColor => VfxColor.Blue;
    
    // 人物性别（男女中立）
    public override CharacterGender Gender => CharacterGender.Neutral;

    // 初始血量
    public override int StartingHp => 75;

    // 人物模型tscn路径。
    public override string CustomVisualPath => "res://scenes/creature_visuals/defect.tscn";
    // 卡牌拖尾场景。
    public override string CustomTrailPath => "res://scenes/vfx/card_trail_defect.tscn";
    // 人物头像路径。
    public override string CustomIconTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_defect.png");
    // 人物头像2号。
    public override string CustomIconPath => SceneHelper.GetScenePath("ui/character_icons/defect_icon");
    // 能量表盘tscn路径。
    public override string CustomEnergyCounterPath => "res://scenes/combat/energy_counters/defect_energy_counter.tscn";
    // 篝火休息场景。
    public override string CustomRestSiteAnimPath => "res://scenes/rest_site/characters/defect_rest_site.tscn";
    // 商店人物场景。
    public override string CustomMerchantAnimPath => "res://scenes/merchant/characters/defect_merchant.tscn";
    // 多人模式-手指。
    public override string CustomArmPointingTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_point.png");
    // 多人模式剪刀石头布-石头。
    public override string CustomArmRockTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_rock.png");
    // 多人模式剪刀石头布-布。
    public override string CustomArmPaperTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_paper.png");
    // 多人模式剪刀石头布-剪刀。
    public override string CustomArmScissorsTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_scissors.png");

    // 人物选择背景。
    public override string CustomCharacterSelectBg => SceneHelper.GetScenePath("screens/char_select/char_select_bg_defect");
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_defect.png");
    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_defect_locked.png");
    // 人物选择过渡动画。
    public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/defect_transition_mat.tres";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    public override string CustomMapMarkerPath => ImageHelper.GetImagePath("packed/map/icons/map_marker_defect.png");
    // 攻击音效
    public override string CustomAttackSfx => "event:/sfx/characters/defect/defect_attack";
    // 施法音效
    public override string CustomCastSfx => "event:/sfx/characters/defect/defect_cast";
    // 死亡音效
    // public override string CustomDeathSfx => null;
    // 角色选择音效
    public override string CharacterSelectSfx => "event:/sfx/characters/defect/defect_select";
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override CardPoolModel CardPool => ModelDb.CardPool<PerfectCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<PerfectRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<PerfectPotionPool>();

    public override int BaseOrbSlotCount => 3;
    
    // 初始卡组
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikePerfect>(),
        ModelDb.Card<StrikePerfect>(),
        ModelDb.Card<StrikePerfect>(),
        ModelDb.Card<StrikePerfect>(),
        ModelDb.Card<DefendPerfect>(),
        ModelDb.Card<DefendPerfect>(),
        ModelDb.Card<DefendPerfect>(),
        ModelDb.Card<DefendPerfect>(),
    ];

    // 初始遗物
    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<CrackedCore>(),
    ];

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}
