using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using ThePerfect.Cards.PerfectCard;
using ThePerfect.Enums;
using ThePerfect.Pools;

namespace ThePerfect.Relics;

[Pool(typeof(PerfectRelicPool))]
public class UpgradedCore : CustomRelicModel, IClickableRelic {
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    private int _counter = 3;
    public override int DisplayAmount => _counter;
    public override bool IsUsedUp => _counter <= 0;
    public GameActionType GameActionType => GameActionType.CombatPlayPhaseOnly;
    public TargetType TargetType => TargetType.None;
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<PerfectCore>().ToMutable();
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<ScaleUpBeam>(),
        HoverTipFactory.FromCard<ScaleDownBeam>(),
        HoverTipFactory.FromKeyword(PerfectEnums.ScaleUp),
        HoverTipFactory.FromKeyword(PerfectEnums.ScaleDown)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Counter", 3)
    ];
    
    [SavedProperty]
    public int Counter {
        get => _counter;
        set {
            AssertMutable();
            Log.Info("Saved counter: " + value);
            _counter = Math.Min(value, 3);
            DynamicVars["Counter"].BaseValue = _counter;
            InvokeDisplayAmountChanged();
        }
    }

    public async Task OnLeftClick(PlayerChoiceContext playerChoiceContext, InputEventMouseButton eventMouseButton) {

    }

    public async Task OnRightClick(PlayerChoiceContext playerChoiceContext, InputEventMouseButton eventMouseButton) {
        if (eventMouseButton.IsPressed() && _counter > 0) {
            Flash();
            List<CardModel> optionCards = [ModelDb.Card<ScaleUpBeam>().ToMutable(), ModelDb.Card<ScaleDownBeam>().ToMutable()];
            foreach (CardModel card in optionCards) {
                Owner.Creature.CombatState.AddCard(card, Owner);
            }
            CardModel chosenCard = await CardSelectCmd.FromChooseACardScreen(playerChoiceContext, optionCards, Owner);
            if (chosenCard == null) {
                return;
            }
            chosenCard.EnergyCost.SetThisTurn(0);
            await CardPileCmd.AddGeneratedCardToCombat(chosenCard, PileType.Hand, true);
            _counter--;
            DynamicVars["Counter"].BaseValue = _counter;
            InvokeDisplayAmountChanged();
        }
    }
}
