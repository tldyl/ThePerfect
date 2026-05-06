using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace ThePerfect.Enums;

public class PerfectEnums {
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword ScaleUp;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword ScaleDown;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Spilt;
}
