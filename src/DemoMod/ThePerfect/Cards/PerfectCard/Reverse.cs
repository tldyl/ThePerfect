using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Reverse : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    public Reverse() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        List<CardModel> _cards = AccessTools.Field(typeof(CardPile), "_cards").GetValue(Owner.PlayerCombatState.DrawPile) as List<CardModel>;
        _cards.Sort((c1, c2) => c2.EnergyCost.GetResolved().CompareTo(c1.EnergyCost.GetResolved()));
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
