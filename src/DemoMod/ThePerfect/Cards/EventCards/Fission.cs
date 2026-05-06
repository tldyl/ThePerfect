using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using ThePerfect.Commands;

namespace ThePerfect.Cards.EventCards;

[Pool(typeof(EventCardPool))]
public class Fission : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    private List<IHoverTip> _hoverTips = [HoverTipFactory.Static(StaticHoverTip.Energy)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1)
    ];

    public Fission() : base(0, CardType.Skill, CardRarity.Event, TargetType.Self) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        int orbCount = Owner.PlayerCombatState.OrbQueue.Orbs.Count;
        if (IsUpgraded) {
            for (int i = 0; i < orbCount; ++i) {
                await OrbCmd.EvokeNext(choiceContext, Owner);
            }
        } else {
            for (int i = 0; i < orbCount; ++i) {
                await OrbCmdExtensions.RemoveNext(choiceContext, Owner);
            }
        }
        if (orbCount > 0) {
            await PlayerCmd.GainEnergy(orbCount, Owner);
            await CardPileCmd.Draw(choiceContext, orbCount, Owner);
        }
    }

    protected override void OnUpgrade() => _hoverTips.Add(HoverTipFactory.Static(StaticHoverTip.Evoke));
}
