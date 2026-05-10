using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class UnstableTransition : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public UnstableTransition() : base(0, CardType.Skill, CardRarity.Rare, TargetType.None) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        await CardPileCmd.Draw(choiceContext, 10 - Owner.PlayerCombatState.Hand.Cards.Count, Owner);
        foreach (CardModel card in Owner.PlayerCombatState.DrawPile.Cards.ToList()) {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }
    
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
