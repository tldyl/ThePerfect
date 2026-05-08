using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Suspend : CustomCardModel {
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Orb", 1),
        new BlockVar(10, ValueProp.Move)
    ];
    public override bool GainsBlock => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<FrostOrb>()
    ];

    public Suspend() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        for (int i = 0; i < DynamicVars["Orb"].IntValue; i++) {
            await OrbCmd.Channel<FrostOrb>(choiceContext, Owner);
        }
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        PlayerCmd.EndTurn(Owner, false);
    }
}
