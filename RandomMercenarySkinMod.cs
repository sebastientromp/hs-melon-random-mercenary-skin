using MelonLoader;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets;
using static DefLoader;

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

    // Implementation insired by https://github.com/Pik-4/HsMod/blob/master/HsMod/Patcher.cs#L1955
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
                    var randomIndex = UnityEngine.Random.Range(0, skin.Variations.Count);
                    var variation = skin.Variations[randomIndex];
                    __instance.SetTag(GAME_TAG.PREMIUM, variation.Premium);
                    __instance.SetRealTimePremium(variation.Premium);
                    cardId = GameUtils.TranslateDbIdToCardId(variation.ID);
                    RandomMercenarySkinMod.SharedLogger.Msg($"Getting random skin: {cardId} ({variation}) out of {skin.Variations.Count} skins (randomIndex: {randomIndex}): {string.Join(", ", skin.Variations)}");
                }
            }
            return true;
        }
    }
}
