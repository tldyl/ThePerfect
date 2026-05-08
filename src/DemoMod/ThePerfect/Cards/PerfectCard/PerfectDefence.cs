using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class PerfectDefence : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Orbs", 3),
        new IntVar("Focus", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<FrostOrb>(),
        HoverTipFactory.FromPower<FocusPower>()
    ];

    public PerfectDefence() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        for (int i = 0; i < DynamicVars["Orbs"].IntValue; i++) {
            await OrbCmd.Channel<FrostOrb>(choiceContext, Owner);
        }
        await PowerCmd.Apply<HotfixPower>(Owner.Creature, DynamicVars["Focus"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
