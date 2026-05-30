#nullable disable
using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.Cooking;
using Il2CppTLD.Gear;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

// Returns a recycled can after cooking a recipe that consumed a canned food item.

namespace HungerRevamped.Patches
{
    internal static class CookingStateTracker
    {
        internal static RecipeData PendingRecipe;
    }

    [HarmonyPatch(typeof(Panel_Cooking), "OnCookRecipe")]
    internal static class PanelCooking_OnCookRecipe_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(RecipeData recipe)
        {
            CookingStateTracker.PendingRecipe = recipe;
        }
    }

    [HarmonyPatch(typeof(Panel_Cooking), "OnRecipePrepSuccess")]
    internal static class PanelCooking_OnRecipePrepSuccess_Patch
    {
        private static readonly List<string> RecipesThatReturnCan = new List<string>
        {
            "RECIPE_PiePeach",
            "RECIPE_PorridgeFruit",
            "RECIPE_PancakeAcorn",
            "RECIPE_StewVegetables"
        };

        private static GearItemData _recycledCanData;

        [HarmonyPostfix]
        internal static void Postfix()
        {
            try
            {
                RecipeData recipe = CookingStateTracker.PendingRecipe;
                CookingStateTracker.PendingRecipe = null;

                if (recipe == null || string.IsNullOrEmpty(recipe.name))
                    return;

                bool shouldSpawn = false;
                foreach (string allowed in RecipesThatReturnCan)
                {
                    if (recipe.name.Equals(allowed, StringComparison.OrdinalIgnoreCase))
                    {
                        shouldSpawn = true;
                        break;
                    }
                }

                if (!shouldSpawn)
                    return;

                PlayerManager playerManager = GameManager.GetPlayerManagerComponent();
                if (playerManager == null)
                    return;

                SpawnGear(playerManager, "RecycledCan", 1);
                DebugHelper.Log("RecycledCan granted after recipe: " + recipe.name);
            }
            catch (Exception e)
            {
                MelonLogger.Error("Error in OnRecipePrepSuccess patch: " + e);
            }
        }

        // Cached lookup — Resources.FindObjectsOfTypeAll is expensive; only run once.
        private static void SpawnGear(PlayerManager playerManager, string nameFragment, int count)
        {
            if (_recycledCanData == null)
            {
                foreach (var g in Resources.FindObjectsOfTypeAll<GearItemData>())
                {
                    if (g != null && g.name.IndexOf(nameFragment, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _recycledCanData = g;
                        break;
                    }
                }
            }

            if (_recycledCanData == null)
            {
                DebugHelper.Log("[RecipeGiveCan] RecycledCan GearItemData not found.");
                return;
            }

            playerManager.InstantiateItemInPlayerInventory(_recycledCanData.PrefabReference, count);
        }
    }

    [HarmonyPatch(typeof(Panel_Cooking), "OnCancelRecipePreparation")]
    internal static class PanelCooking_OnCancelRecipePreparation_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {
            CookingStateTracker.PendingRecipe = null;
        }
    }
}
