using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace BlobcatMod.Patches {
    [HarmonyPatch(typeof(BlockBehaviorCreatureContainer), "IsCatchable")]
    internal class Patcher {
        public static void Postfix(Entity entity, ref bool __result) {
            var basketGeneration = entity.Properties.Attributes?["basketCatchGeneration"].AsInt(-1) ?? -1;
            if (basketGeneration >= 0) {
                var altCond = entity.Properties.Attributes?.IsTrue("basketCatchable") == true && entity.WatchedAttributes.GetAsInt("generation") >= basketGeneration && entity.Alive;
                if (altCond) {
                    __result = true;
                    return;
                }
            }
        }
    }
}
