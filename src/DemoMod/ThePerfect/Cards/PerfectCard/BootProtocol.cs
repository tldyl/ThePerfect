using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using ThePerfect.Pools;
using ThePerfect.Powers;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class BootProtocol : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<NextTurnFocusPower>(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FocusPower>()
    ];

    public BootProtocol() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        await PowerCmd.Apply<NextTurnFocusPower>(Owner.Creature, DynamicVars["NextTurnFocusPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}
