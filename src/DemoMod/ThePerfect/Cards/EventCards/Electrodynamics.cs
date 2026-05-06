using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using ThePerfect.Powers;

namespace ThePerfect.Cards.EventCards;

[Pool(typeof(EventCardPool))]
public class Electrodynamics : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Amount", 2)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromOrb<LightningOrb>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling)
    ];

    public Electrodynamics() : base(2, CardType.Power, CardRarity.Event, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        await PowerCmd.Apply<ElectrodynamicsPower>(Owner.Creature, 1, Owner.Creature, this);
        for (int i = 0; i < DynamicVars["Amount"].IntValue; i++) {
            await OrbCmd.Channel<LightningOrb>(choiceContext, Owner);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Amount"].UpgradeValueBy(1);
}
