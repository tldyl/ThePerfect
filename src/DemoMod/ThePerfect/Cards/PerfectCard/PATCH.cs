using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class PATCH : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Fuel>(IsUpgraded)
    ];

    public PATCH() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        CardModel statusCard = Owner.PlayerCombatState.DrawPile.Cards.Where(c => c.Type == CardType.Status).ToList().StableShuffle(Owner.RunState.Rng.CombatCardGeneration).Take(1).FirstOrDefault();
        if (statusCard != null) {
            CardModel fuel = ModelDb.Card<Fuel>().ToMutable();
            if (IsUpgraded) {
                fuel.UpgradeInternal();
                fuel.FinalizeUpgradeInternal();
            }
            await CardCmd.Transform(statusCard, fuel);
        }
        statusCard = Owner.PlayerCombatState.DiscardPile.Cards.Where(c => c.Type == CardType.Status).ToList().StableShuffle(Owner.RunState.Rng.CombatCardGeneration).Take(1).FirstOrDefault();
        if (statusCard != null) {
            CardModel fuel = ModelDb.Card<Fuel>().ToMutable();
            if (IsUpgraded) {
                fuel.UpgradeInternal();
                fuel.FinalizeUpgradeInternal();
            }
            await CardCmd.Transform(statusCard, fuel);
        }
    }
}
