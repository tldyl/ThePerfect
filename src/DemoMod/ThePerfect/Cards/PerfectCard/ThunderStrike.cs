using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class ThunderStrike : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move),
        new IntVar("DisplayOnly", 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromOrb<LightningOrb>()
    ];
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    public ThunderStrike() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy) {
        
    }

    public override async Task AfterOrbChanneled(
        PlayerChoiceContext choiceContext,
        Player player,
        OrbModel orb) {
        if (player == Owner && !Owner.Deck.Cards.Contains(this)) {
            DynamicVars["DisplayOnly"].BaseValue = CombatManager.Instance.History.Entries.OfType<OrbChanneledEntry>().Count(e => e.Actor.Player == Owner && e.Orb is LightningOrb);
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        AttackCommand _ = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(CombatManager.Instance.History.Entries.OfType<OrbChanneledEntry>().Count(e => e.Actor.Player == Owner && e.Orb is LightningOrb))
            .FromCard(this)
            .TargetingRandomOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}
