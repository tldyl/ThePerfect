using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ThePerfect.CombatHistoryEntries;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Descent : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.Static(StaticHoverTip.Evoke),
        HoverTipFactory.FromPower<FocusPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
        new IntVar("ReduceAmount", 1),
        new PowerVar<FocusPower>(1)
    ];

    public Descent() : base(20, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) {
        
    }

    public override async Task AfterOrbChanneled(
        PlayerChoiceContext choiceContext,
        Player player,
        OrbModel orb) {
        if (player != Owner) {
            return;
        }
        EnergyCost.AddThisCombat(-DynamicVars["ReduceAmount"].IntValue);
    }

    public override async Task AfterOrbEvoked(
        PlayerChoiceContext choiceContext,
        OrbModel orb,
        IEnumerable<Creature> targets) {
        if (orb.Owner != Owner) {
            return;
        }
        EnergyCost.AddThisCombat(-DynamicVars["ReduceAmount"].IntValue);
    }

    public override async Task AfterCardEnteredCombat(CardModel card) {
        if (card != this) {
            return;
        }
        int count = CombatManager.Instance.History.Entries.OfType<OrbChanneledEntry>().Count() + CombatManager.Instance.History.Entries.OfType<OrbEvokedEntry>().Count();
        EnergyCost.AddThisCombat(-count);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        await PowerCmd.Apply<FocusPower>(Owner.Creature, DynamicVars["FocusPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5);
}
