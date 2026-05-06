using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ThePerfect.Utils;

public static class MaxEnumValueCacheExtension {
    private static readonly Dictionary<Type, long> _maxEnumValues = new Dictionary<Type, long>();

    public static long GetLong<T>() where T : struct, Enum {
        long num;
        if (!_maxEnumValues.TryGetValue(typeof(T), out num)) {
            if (!typeof(long).IsAssignableFrom(Enum.GetUnderlyingType(typeof (T)))) {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 1);
                interpolatedStringHandler.AppendLiteral("Trying to get max value for enum type ");
                interpolatedStringHandler.AppendFormatted(typeof (T));
                interpolatedStringHandler.AppendLiteral(" that is not assignable to long!");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            num = Enum.GetValues<T>().Select((Func<T, long>) (v => Convert.ToInt64(v))).Max();
            _maxEnumValues[typeof (T)] = num;
        }
        return num;
    }
}
