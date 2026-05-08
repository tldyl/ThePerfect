using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Relocate : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar("EnergyDisplayOnly", 1),
        new IntVar("Cost", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    public Relocate() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        int energyAmount = 0;
        foreach (CardModel card in Owner.PlayerCombatState.Hand.Cards) {
            if (card.EnergyCost.GetResolved() > DynamicVars["Cost"].BaseValue) {
                energyAmount++;
            } else if (card.EnergyCost.GetResolved() < DynamicVars["Cost"].BaseValue) {
                energyAmount--;
            }
        }
        if (energyAmount > 0) {
            await PlayerCmd.GainEnergy(energyAmount, Owner);
        } else {
            await PlayerCmd.LoseEnergy(-energyAmount, Owner);
        }
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
