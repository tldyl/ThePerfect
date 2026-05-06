using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace ThePerfect.Cards.EventCards;

[Pool(typeof(EventCardPool))]
public class Aggregate : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Energy)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(4),
        new EnergyVar(1)
    ];

    public Aggregate() : base(1, CardType.Skill, CardRarity.Event, TargetType.None) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        int energyAmount = Owner.PlayerCombatState.DrawPile.Cards.Count / DynamicVars.Cards.IntValue;
        if (energyAmount > 0) {
            await PlayerCmd.GainEnergy(energyAmount, Owner);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(-1);
}
