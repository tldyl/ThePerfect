using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThePerfect.Patches;

public class SavedPropertiesTypeCachePatch {
    //Save fields that annotated with SavedProperty. Ex: mod relic counter, etc.
    [HarmonyPatch(typeof(SavedPropertiesTypeCache), MethodType.StaticConstructor)]
    public static class PatchStaticBlock {
        public static void Postfix() {
            List<Type> list = (from t in Assembly.GetAssembly(typeof(ThePerfectMod)).GetTypes()
                where t.IsSubclassOf(typeof(AbstractModel)) && !t.IsAbstract
                select t).ToList();
            list.Sort((t1, t2) => string.CompareOrdinal(t1.Name, t2.Name));
            foreach (Type item in list) {
                Log.Info("Caching properties for type: " + item.Name);
                AccessTools.Method(typeof(SavedPropertiesTypeCache), "CachePropertiesForType", [typeof(Type)]).Invoke(null, [item]);
            }
            List<string> _netIdToPropertyNameMap = AccessTools.StaticFieldRefAccess<List<string>>(typeof(SavedPropertiesTypeCache), "_netIdToPropertyNameMap");
            AccessTools.PropertySetter(typeof(SavedPropertiesTypeCache), "NetIdBitSize").Invoke(null, [Mathf.CeilToInt(Math.Log2(_netIdToPropertyNameMap.Count))]);
        }
    }
}
