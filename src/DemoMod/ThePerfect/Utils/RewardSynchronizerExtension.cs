using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using ThePerfect.Messages;

namespace ThePerfect.Utils;

public static class RewardSynchronizerExtension {
    public static async Task<IEnumerable<CardModel>> DoLocalCardRemoval(this RewardSynchronizer rewardSynchronizer, int removeAmount, Func<CardModel,bool>? filter = null) {
        INetGameService _gameService = AccessTools.Field(typeof(RewardSynchronizer), "_gameService").GetValue(rewardSynchronizer) as INetGameService;
        _gameService.SendMessage(new CustomMessageWrapper {
            Message = new MultiCardRemovedMessage {
                removeAmount = removeAmount
            }
        });
        Player player = (Player) AccessTools.PropertyGetter(typeof(RewardSynchronizer), "LocalPlayer").Invoke(rewardSynchronizer, []);
        return await rewardSynchronizer.DoCardRemoval(player, removeAmount, filter);
    }

    public static async Task<IEnumerable<CardModel>> DoCardRemoval(this RewardSynchronizer rewardSynchronizer, Player player, int removeAmount, Func<CardModel,bool>? filter = null) {
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromDeckForRemoval(player, new CardSelectorPrefs(new LocString("gameplay_ui", "COMBAT_REWARD_CARD_REMOVAL.selectionScreenPrompt"), removeAmount) {
            Cancelable = true,
            RequireManualConfirmation = true
        }, filter);
        if (selectedCards == null)
            return [];
        await CardPileCmd.RemoveFromDeck(selectedCards.ToList());
        return selectedCards;
    }
}
