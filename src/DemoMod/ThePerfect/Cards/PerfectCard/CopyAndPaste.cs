using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class CopyAndPaste : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    public CopyAndPaste() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        IEnumerable<CardModel> cards = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
        CardModel card = cards.FirstOrDefault();
        if (card != null) {
            CardModel topCard = Owner.PlayerCombatState.DrawPile.Cards.FirstOrDefault();
            if (topCard != null) {
                CardModel cpy = card.CreateClone();
                await CardCmd.Transform(topCard, cpy);
            }
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
