using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ThePerfect.Hooks;

public interface IBeforeOrbChanneled {
    bool BeforeOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb);
}
