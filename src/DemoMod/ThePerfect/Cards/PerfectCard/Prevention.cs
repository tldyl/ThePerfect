using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ThePerfect.Pools;
using ThePerfect.Powers;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Prevention : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),
        new CardsVar(1)
    ];

    public static SavedSpireField<Prevention, int[]> save = new SavedSpireField<Prevention, int[]>(() => [0, 0], "ThePerfect-PreventionCardSave");

    public Prevention() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) {
        
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState) {
        if (!Owner.Deck.Cards.Any(c => c is Prevention) || side != Owner.Creature.Side || combatState.RoundNumber > 1) {
            return;
        }
        Prevention prevention = (Prevention) Owner.Deck.Cards.FirstOrDefault(c => c is Prevention);
        if (save.Get(prevention)[0] > 0) {
            await PlayerCmd.GainEnergy(save.Get(prevention)[0], Owner);
            save.Get(prevention)[0] = 0;
        }
    }

    public override Decimal ModifyHandDraw(Player player, Decimal count) {
        if (!Owner.Deck.Cards.Any(c => c is Prevention)) {
            return count;
        }
        Prevention prevention = (Prevention) Owner.Deck.Cards.FirstOrDefault(c => c is Prevention);
        int amount = save.Get(prevention)[1];
        save.Get(prevention)[1] = 0;
        return player != Owner || player.Creature.CombatState.RoundNumber > 1 ? count : count + amount;
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        PreventionPower power = (PreventionPower) ModelDb.Power<PreventionPower>().ToMutable();
        power.DynamicVars.Energy.BaseValue = DynamicVars.Energy.BaseValue;
        await PowerCmd.Apply(power, Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature, this);
        foreach (CardModel card in Owner.Deck.Cards) {
            if (card is Prevention prevention) {
                save.Get(prevention)[0] += DynamicVars.Energy.IntValue;
                save.Get(prevention)[1] += DynamicVars.Cards.IntValue;
                break;
            }
        }
    }
    
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
