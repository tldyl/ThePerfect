using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ThePerfect.Pools;
using ThePerfect.Powers;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class ZERO : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override bool IsPlayable {
        get {
            foreach (CardModel card in Owner.PlayerCombatState.DrawPile.Cards) {
                if (card.EnergyCost.GetResolved() >= EnergyCost.GetResolved() && !card.EnergyCost.CostsX) {
                    return false;
                }
            }
            foreach (CardModel card in Owner.PlayerCombatState.DiscardPile.Cards) {
                if (card.EnergyCost.GetResolved() >= EnergyCost.GetResolved() && !card.EnergyCost.CostsX) {
                    return false;
                }
            }
            return true;
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("ReduceAmount", 1)
    ];

    public ZERO() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        await PowerCmd.Apply<ZeroPower>(Owner.Creature, DynamicVars["ReduceAmount"].BaseValue, Owner.Creature, this);
    }
    
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
