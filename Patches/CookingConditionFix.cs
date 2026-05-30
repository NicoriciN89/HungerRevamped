#nullable disable
using HarmonyLib;
using UnityEngine;
using Il2Cpp;

namespace HungerRevamped
{

    [HarmonyPatch(typeof(CookingPotItem), "SetCookedGearProperties")]
    internal class CookingConditionFix
    {
        private static void Postfix(GearItem rawItem, GearItem cookedItem)
        {
            if (!MenuSettings.settings.cookingDoublesCondition || !rawItem || !cookedItem)
                return;

            float rawHp     = rawItem.CurrentHP;
            float rawMax    = rawItem.GearItemData.m_MaxHP;
            float cookedMax = cookedItem.GearItemData.m_MaxHP;
            float result    = cookedMax * Mathf.Clamp01(2f * rawHp / rawMax);

            DebugHelper.Log($"CookingConditionFix: raw={rawHp}/{rawMax} → cooked={result}/{cookedMax}");
            cookedItem.CurrentHP = result;
        }
    }
}
