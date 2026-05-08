using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Runs;
using ThePerfect.Utils;

namespace ThePerfect.Messages;

public class MultiCardRemovedMessage : ICustomMessage {
    public int removeAmount;
    
    public void Serialize(PacketWriter writer) {
        writer.WriteInt(removeAmount);
    }

    public void Deserialize(PacketReader reader) {
        removeAmount = reader.ReadInt();
    }

    public void HandleMessage(ulong senderId) {
        Player player = RunManager.Instance.DebugOnlyGetState().GetPlayer(senderId);
        if (LocalContext.IsMe(player)) {
            throw new InvalidOperationException("MultiCardRemovedMessage should not be sent to the player removing the card!");
        }
        TaskHelper.RunSafely(RunManager.Instance.RewardSynchronizer.DoCardRemoval(player, removeAmount));
    }

    public bool ShouldBroadcast => true;

    public NetTransferMode Mode => NetTransferMode.Reliable;
    public LogLevel LogLevel => LogLevel.VeryDebug;
}
