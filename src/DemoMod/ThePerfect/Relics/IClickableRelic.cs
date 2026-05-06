using ThePerfect.Actions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace ThePerfect.Relics;

public interface IClickableRelic {
    GameActionType GameActionType { get; }
    TargetType TargetType { get; }
    MouseButton MouseButton => MouseButton.Right;
    bool Activated => true;
    
    Task OnLeftClick(PlayerChoiceContext playerChoiceContext, InputEventMouseButton eventMouseButton);
    
    Task OnRightClick(PlayerChoiceContext playerChoiceContext, InputEventMouseButton eventMouseButton);

    async Task OnClickWrapper(InputEventMouseButton eventMouseButton, PlayerChoiceContext playerChoiceContext) {
        RelicModel relicModel = (RelicModel)this;
        playerChoiceContext.PushModel(relicModel);
        await CombatManager.Instance.WaitForUnpause();
        
        switch (eventMouseButton.ButtonIndex) {
            case MouseButton.Left:
                await OnLeftClick(playerChoiceContext, eventMouseButton);
                break;
            case MouseButton.Right:
                await OnRightClick(playerChoiceContext, eventMouseButton);
                break;
            case MouseButton.None:
            case MouseButton.Middle:
            case MouseButton.WheelUp:
            case MouseButton.WheelDown:
            case MouseButton.WheelLeft:
            case MouseButton.WheelRight:
            case MouseButton.Xbutton1:
            case MouseButton.Xbutton2:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        relicModel.InvokeExecutionFinished();
        await CombatManager.Instance.CheckForEmptyHand(playerChoiceContext, relicModel.Owner);
        playerChoiceContext.PopModel(relicModel);
    }
    
    void OnGuiInput(InputEvent inputEvent) {
        if (inputEvent is not InputEventMouseButton inputEventMouseButton || !Activated) {
            return;
        }
        if (inputEventMouseButton.ButtonIndex != MouseButton) {
            return;
        }
        bool flag;
        switch (TargetType) {
            case TargetType.AnyEnemy:
            case TargetType.TargetedNoCreature:
                flag = true;
                break;
            default:
                flag = false;
                break;
        }
        if (flag || CanThrowAtAlly()) {
            TaskHelper.RunSafely(SelectTarget(inputEvent));
        } else {
            if (GameActionType == GameActionType.Combat || GameActionType == GameActionType.CombatPlayPhaseOnly || GameActionType == GameActionType.Any) {
                if (RunManager.Instance.ActionQueueSynchronizer.CombatState == ActionSynchronizerCombatState.PlayPhase ||
                    RunManager.Instance.ActionQueueSynchronizer.CombatState == ActionSynchronizerCombatState.EndTurnPhaseOne ||
                    RunManager.Instance.ActionQueueSynchronizer.CombatState == ActionSynchronizerCombatState.NotPlayPhase || GameActionType == GameActionType.Any) {
                    RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new ClickRelicAction((RelicModel) this, GameActionType, inputEvent, null));
                }
            } else {
                if (RunManager.Instance.ActionQueueSynchronizer.CombatState == ActionSynchronizerCombatState.NotInCombat) {
                    RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new ClickRelicAction((RelicModel) this, GameActionType, inputEvent, null));
                }
            }
        }
    }

    public bool CanThrowAtAlly() {
        return TargetType == TargetType.AnyPlayer && ((RelicModel)this).Owner.RunState.Players.Count > 1 && CombatManager.Instance.IsInProgress;
    }
    
    private async Task SelectTarget(InputEvent inputEvent) {
        RunManager.Instance.HoveredModelTracker.OnLocalRelicHovered((RelicModel) this);
        await TargetNode(inputEvent);
        RunManager.Instance.HoveredModelTracker.OnLocalRelicUnhovered();
    }
    
    private async Task TargetNode(InputEvent inputEvent) {
        RelicModel relicModel = (RelicModel) this;
        IReadOnlyList<NRelicInventoryHolder> relicNodes = NRun.Instance.GlobalUi.RelicInventory.RelicNodes;
        Vector2 startPosition = new Vector2();
        NRelicInventoryHolder relicInventoryHolder = null;
        foreach (NRelicInventoryHolder relicNode in relicNodes) {
            if (relicModel.GetType().IsInstanceOfType(relicNode.Relic.Model)) {
                startPosition = relicNode.Relic.GlobalPosition + Vector2.Right * relicNode.Relic.Size.X * 0.5f + Vector2.Down * relicNode.Relic.Size.Y * 0.5f;
                relicInventoryHolder = relicNode;
                break;
            }
        }
        NTargetManager instance = NTargetManager.Instance;
        instance.StartTargeting(TargetType, startPosition, TargetMode.ClickMouseToTarget, ShouldCancelTargeting, null);
        Node actualValue = await instance.SelectionFinished();
        NCombatRoom.Instance?.EnableControllerNavigation();
        NRun.Instance.GlobalUi.MultiplayerPlayerContainer.UnlockNavigation();
        Creature creature2;
        switch (actualValue) {
            case null:
                goto label_25;
            case NCreature ncreature:
                creature2 = ncreature.Entity;
                break;
            case NMultiplayerPlayerState nmultiplayerPlayerState:
                creature2 = nmultiplayerPlayerState.Player.Creature;
                break;
            case NMerchantButton:
                creature2 = null;
                break;
            default:
                throw new ArgumentOutOfRangeException("targetNode", actualValue, null);
        }
        Creature target = creature2;
        Log.Info("target == null ? " + (target == null));
        if (target != null) {
            Log.Info("target type: " + target.GetType().FullName);
        }
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new ClickRelicAction((RelicModel) this, GameActionType, inputEvent, target));
        label_25: 
        relicInventoryHolder?.TryGrabFocus();
    }

    private bool ShouldCancelTargeting() {
        if (!CombatManager.Instance.IsInProgress)
            return false;
        return NOverlayStack.Instance.ScreenCount > 0 || NCapstoneContainer.Instance.InUse;
    }
}