#nullable disable
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace HungerRevamped
{
    internal static class LocalizationManager
    {
        private static Dictionary<string, Dictionary<string, string>> _data;

        private static Dictionary<string, Dictionary<string, string>> Data =>
            _data ?? (_data = Load());

        private const string EmbeddedResource = "HungerRevamped.localization.json";

        private static Dictionary<string, Dictionary<string, string>> Load()
        {
            string dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string userPath = Path.Combine(
                Path.GetDirectoryName(dllDir) ?? dllDir,
                "UserData", "HungerRevamped", "localization.json");

            if (File.Exists(userPath))
            {
                var fromFile = TryLoad(File.ReadAllText(userPath, Encoding.UTF8));
                if (fromFile != null)
                {
                    MelonLogger.Msg("[HungerRevamped] Using localization override: " + userPath);
                    return fromFile;
                }
            }

            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(EmbeddedResource);
            if (stream != null)
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var embedded = TryLoad(reader.ReadToEnd());
                if (embedded != null)
                    return embedded;
            }

            MelonLogger.Warning("[HungerRevamped] Could not load localization — using fallback strings.");
            return Fallback;
        }

        private static Dictionary<string, Dictionary<string, string>> TryLoad(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[HungerRevamped] Localization parse error: " + ex.Message);
                return null;
            }
        }

        internal static string Get(string key)
        {
            string lang = Localization.Language ?? "English";
            var data = Data;
            if (data.TryGetValue(lang, out var dict) && dict.TryGetValue(key, out string val))
                return val;
            if (data.TryGetValue("English", out var en) && en.TryGetValue(key, out string enVal))
                return enVal;
            return key;
        }

        private static readonly Dictionary<string, Dictionary<string, string>> Fallback = new()
        {
            ["English"] = new()
            {
                ["HR.SEC_FOOD"]              = "Ruined Food",
                ["HR.RUINED_EAT"]            = "Can eat ruined food",
                ["HR.DESC_RUINED_EAT"]       = "All ruined food is inedible. Ruined canned food can still be harvested for a recycled can.",
                ["HR.RUINED_COOK"]           = "Can cook ruined food",
                ["HR.DESC_RUINED_COOK"]      = "If disabled, placing ruined food on a fire will not improve its condition.",
                ["HR.SEC_POISON"]            = "Food Poisoning",
                ["HR.DELAYED_POISON"]        = "Delayed Food Poisoning",
                ["HR.DESC_DELAYED_POISON"]   = "Adds an incubation period of 4–16 hours before food poisoning takes effect.",
                ["HR.REALISTIC_POISON"]      = "Gradual Poisoning Probability",
                ["HR.DESC_REALISTIC_POISON"] = "Risk scales with condition and proportion eaten.",
                ["HR.REMOVE_IMMUNITY"]       = "Remove Cooking Lv.5 Immunity",
                ["HR.DESC_REMOVE_IMMUNITY"]  = "Level 5 cooking no longer grants full immunity to food poisoning.",
                ["HR.SEC_COOKING"]           = "Cooking",
                ["HR.FIX_EXPLOIT"]           = "Fix Cooking Skill Exploit",
                ["HR.DESC_FIX_EXPLOIT"]      = "Skill points proportional to amount of meat cooked. Ruined food gives no points.",
                ["HR.COOK_DOUBLES"]          = "Cooking Doubles Condition",
                ["HR.DESC_COOK_DOUBLES"]     = "Cooked item gets exactly double the raw item's condition.",
                ["HR.CUSTOM_CALORIES"]       = "Starting Stored Calories",
                ["HR.DESC_CUSTOM_CALORIES"]  = "Body-fat calories at the start of a Custom Mode game.",
                ["HR.HARVEST_CAN"]           = "Harvest Can",
                ["HR.HARVESTING"]            = "Harvesting Can...",
                ["HR.HARVEST_CANCELLED"]     = "Harvest cancelled.",
                ["HR.NO_ITEM_SELECTED"]      = "No item selected.",
            }
        };
    }

    [HarmonyPatch(typeof(Localization), nameof(Localization.Get))]
    internal static class Patch_LocalizationGet
    {
        private static void Postfix(string __0, ref string __result)
        {
            if (__0 == null || !__0.StartsWith("HR."))
                return;
            __result = LocalizationManager.Get(__0);
        }
    }

    // ModSettings 2.x passes description strings through DescriptionHolder.Text — intercept here.
    [HarmonyPatch]
    internal static class Patch_DescriptionText
    {
        private static MethodBase TargetMethod() =>
            AccessTools.PropertyGetter(
                AccessTools.TypeByName("ModSettings.DescriptionHolder"), "Text");

        private static void Postfix(ref string __result)
        {
            if (__result == null || !__result.StartsWith("HR."))
                return;
            __result = LocalizationManager.Get(__result);
        }
    }
}
