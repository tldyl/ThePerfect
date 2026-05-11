using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ThePerfect.Pools;

namespace ThePerfect.Cards.PerfectCard;

[Pool(typeof(PerfectCardPool))]
public class Arbitration : CustomCardModel {
    //public override string PortraitPath => $"res://ThePerfect/images/cards/{Id.Entry.ToLowerInvariant()}.png";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(15, ValueProp.Move)
    ];

    public Arbitration() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) {
        await using AttackContext context = await AttackCommand.CreateContextAsync(CombatState, this);
        IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, this);
        context.AddHit(damageResults);
        DamageResult damageResult = damageResults.FirstOrDefault();
        if (damageResult != null) {
            List<Creature> enemies = Owner.Creature.CombatState.HittableEnemies.ToList();
            int index = enemies.IndexOf(cardPlay.Target);
            if (index > 0) {
                context.AddHit(await CreatureCmd.Damage(choiceContext, enemies[index - 1], (damageResult.TotalDamage + damageResult.OverkillDamage) / 2.0M, ValueProp.Unpowered | ValueProp.Move, this));
            }
            if (enemies.Count > index + 1) {
                context.AddHit(await CreatureCmd.Damage(choiceContext, enemies[index + 1], (damageResult.TotalDamage + damageResult.OverkillDamage) / 2.0M, ValueProp.Unpowered | ValueProp.Move, this));
            }
        }
    }
}
