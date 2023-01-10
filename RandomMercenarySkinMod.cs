using MelonLoader;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace RandomMercenarySkin
{
    public class RandomMercenarySkinMod : MelonMod
    {
        public static MelonLogger.Instance SharedLogger;

        public override void OnInitializeMelon()
        {
            RandomMercenarySkinMod.SharedLogger = LoggerInstance;
            var harmony = this.HarmonyInstance;
            harmony.PatchAll(typeof(EntityPatcher));
        }
    }

    // Implementation from https://github.com/Pik-4/HsMod/blob/master/HsMod/Patcher.cs#L1955
    public static class EntityPatcher
    {
        [HarmonyPatch(typeof(Entity), "LoadCard", new Type[] { typeof(string), typeof(Entity.LoadCardData) })]
        [HarmonyPrefix]
        public static bool Prefix(Entity __instance, ref string cardId)
        {
            if (cardId != null && Utils.IsMercenarySkin(cardId, out Utils.MercenarySkin skin))
            {
                if (__instance.GetCard().GetControllerSide() == Player.Side.FRIENDLY)
                {
                    List<int> dbids = new List<int>();
                    dbids.AddRange(skin.Id);
                    dbids.Remove(skin.Diamond);
                    var dbid = dbids[UnityEngine.Random.Range(0, dbids.Count)];
                    RandomMercenarySkinMod.SharedLogger.Msg($"Getting random skin: {dbid} out of {dbids.Count} skins");
                    cardId = GameUtils.TranslateDbIdToCardId(dbid);
                }
            }
            return true;
        }
    }
}
