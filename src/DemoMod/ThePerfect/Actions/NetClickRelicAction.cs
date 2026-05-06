using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using ThePerfect.Utils;

namespace ThePerfect.Actions;

public struct NetClickRelicAction : INetAction {
    public RelicModel relicModel;
    public GameActionType actionType;
    public InputEvent inputEvent;
    public Creature _target;
    
    public void Serialize(PacketWriter writer) {
        writer.WriteModel(ModelDb.GetById<RelicModel>(relicModel.Id));
        writer.WriteEnum(actionType);
        writer.WriteLongEnum(((InputEventMouseButton) inputEvent).ButtonIndex);
        CombatState combatState = relicModel.Owner.Creature.CombatState;
        writer.WriteInt(_target == null ? -1 : combatState.Enemies.IndexOf(_target));
    }

    public void Deserialize(PacketReader reader) {
        relicModel = reader.ReadModel<RelicModel>();
        actionType = reader.ReadEnum<GameActionType>();
        MouseButton mouseButton = reader.ReadLongEnum<MouseButton>();
        inputEvent = new InputEventMouseButton();
        ((InputEventMouseButton) inputEvent).ButtonIndex = mouseButton;
        ((InputEventMouseButton)inputEvent).Pressed = true;
        CombatState combatState = relicModel.Owner.Creature.CombatState;
        int targetIndex = reader.ReadInt();
        _target = targetIndex > 0 ? combatState.Enemies[reader.ReadInt()] : null;
    }

    public GameAction ToGameAction(Player player) {
        return new ClickRelicAction(relicModel, actionType, inputEvent, _target);
    }
}
