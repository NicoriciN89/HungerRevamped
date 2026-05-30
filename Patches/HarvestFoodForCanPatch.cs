#nullable disable
using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.Gear;
using System;
using UnityEngine;

namespace HungerRevamped
{
    internal static class CannedFoodHarvestUI
    {
        private static GameObject harvestButton;
        private static GearItem currentShownGear;
        private static GearItemData _recycledCanData;

        // ================= PATCH =================

        [HarmonyPatch(typeof(ItemDescriptionPage), nameof(ItemDescriptionPage.UpdateGearItemDescription))]
        internal class ItemDescriptionPatch
        {
            private static void Postfix(ItemDescriptionPage __instance, GearItem gi)
            {
                InitializeButton(__instance);
                TrySetGear(gi);
            }
        }

        // ================= BUTTON INIT =================

        private static void InitializeButton(ItemDescriptionPage page)
        {
            if (page == null || harvestButton != null)
                return;

            GameObject equipButton = page.m_MouseButtonEquip;
            if (equipButton == null)
                return;

            harvestButton = UnityEngine.Object.Instantiate(
                equipButton,
                equipButton.transform.parent,
                true);

            harvestButton.transform.Translate(0f, -0.12f, 0f);

            UILabel label = Utils.GetComponentInChildren<UILabel>(harvestButton);
            if (label != null)
                label.text = LocalizationManager.Get("HR.HARVEST_CAN");

            UIButton uiBtn = harvestButton.GetComponent<UIButton>();
            if (uiBtn != null)
                uiBtn.isEnabled = true;

            AddAction(harvestButton, new Action(OnHarvestPressed));
            SetActive(false);
        }

        private static void AddAction(GameObject button, Action action)
        {
            Il2CppSystem.Collections.Generic.List<EventDelegate> list =
                new Il2CppSystem.Collections.Generic.List<EventDelegate>();

            list.Add(new EventDelegate(action));

            UIButton uiBtn = Utils.GetComponentInChildren<UIButton>(button);
            if (uiBtn != null)
                uiBtn.onClick = list;
        }

        // ================= GEAR CHECK =================

        private static void TrySetGear(GearItem gear)
        {
            currentShownGear = gear;

            bool isCannedFood = false;

            if (gear != null)
            {
                FoodItem food = gear.GetComponent<FoodItem>();
                if (food != null && food.m_GearPrefabHarvestAfterFinishEatingNormal != null)
                    isCannedFood = true;
            }

            SetActive(isCannedFood);
        }

        private static void SetActive(bool active)
        {
            if (harvestButton == null)
                return;

            NGUITools.SetActive(harvestButton, active);

            UIButton uiBtn = harvestButton.GetComponent<UIButton>();
            if (uiBtn != null)
                uiBtn.isEnabled = active;
        }

        // ================= HARVEST =================

        private static void OnHarvestPressed()
        {
            if (currentShownGear == null)
            {
                HUDMessage.AddMessage(LocalizationManager.Get("HR.NO_ITEM_SELECTED"));
                return;
            }

            InterfaceManager.GetPanel<Panel_GenericProgressBar>().Launch(
                LocalizationManager.Get("HR.HARVESTING"),
                5f,
                0f,
                0f,
                "Play_OpenCan",
                null,
                false,
                true,
                new Action<bool, bool, float>(OnHarvestFinished)
            );
        }

        private static void OnHarvestFinished(bool success, bool playerCancel, float progress)
        {
            if (!success || playerCancel)
            {
                HUDMessage.AddMessage(LocalizationManager.Get("HR.HARVEST_CANCELLED"));
                return;
            }

            PlayerManager pm = GameManager.GetPlayerManagerComponent();
            if (pm == null)
                return;

            SpawnGear(pm, "RecycledCan", 1);

            if (currentShownGear != null)
                GameManager.GetInventoryComponent().DestroyGear(currentShownGear);
        }

        // Cached lookup — Resources.FindObjectsOfTypeAll is expensive; only run once.
        private static void SpawnGear(PlayerManager pm, string nameFragment, int count)
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
                DebugHelper.Log("[HarvestCan] RecycledCan GearItemData not found.");
                return;
            }

            pm.InstantiateItemInPlayerInventory(_recycledCanData.PrefabReference, count);
        }
    }
}
