using Godot;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace ThePerfect.Utils;

public static class PacketReaderExtension {
    public static T ReadLongEnum<T>(this PacketReader packetReader) where T : struct, Enum {
        if (!typeof (long).IsAssignableFrom(Enum.GetUnderlyingType(typeof (T)))) {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 1);
            interpolatedStringHandler.AppendLiteral("Trying to write enum type ");
            interpolatedStringHandler.AppendFormatted(typeof (T));
            interpolatedStringHandler.AppendLiteral(" that is not assignable to long!");
            throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
        }
        return (T) Enum.ToObject(typeof (T), packetReader.ReadLong(Mathf.CeilToInt(Math.Log2(MaxEnumValueCacheExtension.GetLong<T>()) + 1.0)));
    }
}
