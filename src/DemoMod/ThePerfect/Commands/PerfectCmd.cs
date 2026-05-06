using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace ThePerfect.Commands;

public static class PerfectCmd {
    public static void ScaleUp(CardModel card) {
        card.EnergyCost.AddThisCombat(1);
        foreach (DynamicVar dynamicVar in card.DynamicVars.Values.Where(v => !v.Name.Contains("DisplayOnly"))) {
            dynamicVar.BaseValue *= 2;
        }
    }

    public static void ScaleDown(CardModel card) {
        card.EnergyCost.SetThisCombat(0);
        foreach (DynamicVar dynamicVar in card.DynamicVars.Values.Where(v => !v.Name.Contains("DisplayOnly"))) {
            dynamicVar.BaseValue = 1;
        }
    }

    public static async Task Spilt(CardModel card) {
        int halfCost = card.EnergyCost.GetResolved() / 2;
        if (halfCost <= 0) {
            halfCost = 1;
        }
        card.EnergyCost.SetThisCombat(halfCost);
        foreach (DynamicVar dynamicVar in card.DynamicVars.Values.Where(v => !v.Name.Contains("DisplayOnly"))) {
            dynamicVar.BaseValue /= 2;
            if (dynamicVar.BaseValue < 1) {
                dynamicVar.BaseValue = 1;
            }
        }
        CardModel cpy = card.CreateClone();
        await CardPileCmd.Add(cpy, card.Pile, card.Pile.Type == PileType.Draw ? CardPilePosition.Random : CardPilePosition.Bottom);
    }
}
