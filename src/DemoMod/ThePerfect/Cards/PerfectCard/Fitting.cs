using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Fitting : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(0, ValueProp.Move)
    ];

    public Fitting() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource) {
        if (cardSource != this || target?.Monster == null) {
            return 0;
        }
        if (target.Monster.IntendsToAttack) {
            List<AbstractIntent> intents = target.Monster.NextMove.Intents.Where(intent => intent.IntentType == IntentType.Attack || intent.IntentType == IntentType.DeathBlow).ToList();
            int sum = 0;
            foreach (AbstractIntent intent in intents) {
                if (intent is AttackIntent attackIntent) {
                    sum += attackIntent.GetTotalDamage(Owner.Creature.CombatState.PlayerCreatures, target);
                }
            }
            return sum;
        }
        return 0;
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
