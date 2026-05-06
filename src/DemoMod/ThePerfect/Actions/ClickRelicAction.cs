using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Threading.Tasks;
using ThePerfect.Relics;

namespace ThePerfect.Actions;

public class ClickRelicAction(RelicModel relicModel, GameActionType actionType, InputEvent inputEvent, Creature target) : GameAction {
    public override ulong OwnerId => relicModel.Owner.NetId;

    private RelicModel _relic = relicModel;

    public override GameActionType ActionType => actionType;
    public PlayerChoiceContext? PlayerChoiceContext { get; private set; }
    public Creature? TargetCreature => target;

    protected override async Task ExecuteAction() {
        ClickRelicAction action = this;

        if (inputEvent is not InputEventMouseButton eventMouseButton) {
            throw new InvalidOperationException("ClickRelicAction must have a input event!");
        }
        if (_relic is not IClickableRelic clickableRelic) {
            throw new InvalidOperationException("ClickRelicAction must have a clickable relic!");
        }
        
        action.PlayerChoiceContext = new GameActionPlayerChoiceContext(action);
        await clickableRelic.OnClickWrapper(eventMouseButton, PlayerChoiceContext);
        _relic = null;
    }

    public override INetAction ToNetAction() {
        return new NetClickRelicAction {
            inputEvent = inputEvent,
            actionType = ActionType,
            relicModel = _relic,
            _target = target
        };
    }
}
