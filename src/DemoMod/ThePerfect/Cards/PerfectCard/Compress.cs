using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using ThePerfect.Pools;
using ThePerfect.Utils;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Compress : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    public override bool CanBeGeneratedInCombat => false;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
    private List<IHoverTip> _hoverTips = [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("Card1"),
        new StringVar("Card2"),
        new IntVar("Compressed", 0)
    ];

    private List<SerializableCard>? _cardSave = [];
    
    [SavedProperty]
    private List<SerializableCard>? CardSave {
        get => _cardSave;
        set {
            _cardSave = value;
            if (value != null) {
                foreach (SerializableCard serializableCard in _cardSave) {
                    CardModel card1 = FromSerializable(serializableCard);
                    _hoverTips.Add(HoverTipFactory.FromCard(card1));
                }
                ((StringVar)DynamicVars["Card1"]).StringValue = ModelDb.GetById<CardModel>(_cardSave[0].Id).Title;
                if (_cardSave.Count > 1) {
                    ((StringVar)DynamicVars["Card2"]).StringValue = ModelDb.GetById<CardModel>(_cardSave[1].Id).Title;
                }
                DynamicVars["Compressed"].BaseValue = 1;
            }
        }
    }

    public Compress() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.None) {
        
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source) {
        if (card == this && Owner.Deck.Cards.Contains(this)) {
            IEnumerable<CardModel> removedCards = await RunManager.Instance.RewardSynchronizer.DoLocalCardRemoval(2, c => c is not Compress);
            CardSave.AddRange(removedCards.Select(c => c.ToSerializable()));
            foreach (CardModel removedCard in removedCards) {
                _hoverTips.Add(HoverTipFactory.FromCard(removedCard));
            }
            ((StringVar)DynamicVars["Card1"]).StringValue = ModelDb.GetById<CardModel>(CardSave[0].Id).Title;
            if (CardSave.Count > 1) {
                ((StringVar)DynamicVars["Card2"]).StringValue = ModelDb.GetById<CardModel>(CardSave[1].Id).Title;
            }
            DynamicVars["Compressed"].BaseValue = 1;
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
        if (card != this) {
            return;
        }
        await CardCmd.Exhaust(choiceContext, this);
        foreach (SerializableCard serializableCard in CardSave) {
            CardModel card1 = FromSerializable(serializableCard);
            CombatState.AddCard(card1, Owner);
            await CardPileCmd.AddGeneratedCardsToCombat([card1], PileType.Hand, true);
        }
    }
    
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);

    protected override void DeepCloneFields() {
        base.DeepCloneFields();
        List<IHoverTip> hoverTips = [];
        hoverTips.AddRange(_hoverTips);
        _hoverTips = hoverTips;
        if (_cardSave != null) {
            List<SerializableCard> cardSave = [];
            cardSave.AddRange(_cardSave);
            _cardSave = cardSave;
        }
    }
}
