using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using ThePerfect.Pools;
using ThePerfect.Powers;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Virus : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Orb", 0)
    ];
    private List<IHoverTip> _hoverTips = [HoverTipFactory.Static(StaticHoverTip.Channeling)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips;

    public Virus() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        if (IsUpgraded) {
            for (int i = 0; i < DynamicVars["Orb"].IntValue; i++) {
                await OrbCmd.Channel<DarkOrb>(choiceContext, Owner);
            }
        }
        await PowerCmd.Apply<VirusPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() {
        DynamicVars["Orb"].UpgradeValueBy(1);
        _hoverTips.Add(HoverTipFactory.FromOrb<DarkOrb>());
    }
}
