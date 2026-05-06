using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using ThePerfect.Cards.EventCards;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Dejavu : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    public Dejavu() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        List<CardModel> pool = [
            ModelDb.Card<Aggregate>(),
            ModelDb.Card<SelfRepair>(),
            ModelDb.Card<HelloWorld>(),
            ModelDb.Card<Fission>(),
            ModelDb.Card<Recycle>(),
            ModelDb.Card<Electrodynamics>(),
            ModelDb.Card<Equilibrium>()
        ];
        List<CardModel> choices = [];
        while (choices.Count < 3) {
            int index = Owner.RunState.Rng.CombatCardGeneration.NextInt(pool.Count);
            choices.Add(pool[index].ToMutable());
            pool.RemoveAt(index);
        }
        foreach (CardModel card in choices) {
            if (IsUpgraded) {
                card.UpgradeInternal();
                card.FinalizeUpgradeInternal();
            }
            CombatState.AddCard(card, Owner);
        }
        CardModel chosenCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, choices, Owner);
        if (chosenCard == null) {
            return;
        }
        await CardPileCmd.AddGeneratedCardToCombat(chosenCard, PileType.Hand, true);
    }
}
