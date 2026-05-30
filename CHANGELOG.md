# Changelog

## [2.2.0] - 2026-05-30

### Bug Fixes
- **`GetRecipeCookability` was a no-op** — ruined food could still be added to cooking recipes; now correctly blocks it (`MissingIngredients` result)
- **Null reference in `PreventWarmingUpRuinedFood`** — `GetComponent<GearItem>()` result was used without null check; fixed with `gearItem == null || !gearItem.IsWornOut()`
- **Crash in `FoodPoisoningPatches`** — `HungerRevamped.Instance` was accessed without checking initialization; fixed with `HasInstance` guard (falls back to immediate food poisoning)
- **Null reference in `PreventEatingRuinedFood`** — `GetCurrentlySelectedItem()` return value and its `m_GearItem` were not null-checked
- **Division by zero in `ExploitableCookingSkillFix`** — `food.m_CaloriesRemaining / food.m_CaloriesTotal` could produce NaN/Infinity when `m_CaloriesTotal <= 0`; now guarded
- **UI panels showing empty labels instead of vanilla** — `Panel_BreakDown`, `Panel_IceFishing`, `Panel_SnowShelterBuild`, `Panel_SnowShelterInteract` Prefix patches returned `false` (skipping original) even when the mod was not initialized; now return `true` (run vanilla) in that case
- **`IncrementSkill` class not declared `static`** — renamed from `Incrementskill` to `IncrementSkill` and made `static`
- **csproj hardcoded to `D:\SteamLibrary`** — `TheLongDarkPath` and all HintPaths updated to use the property variable

### Null Safety (crash prevention)
- Added `HungerRevamped.HasInstance` property
- Added `HasInstance` guards across **12 patch sites** in `GameStatePatches`, `StatusBarPatches`, `SerializationPatches`, `UIPanelPatches`, `FoodPoisoningPatches`
- All **7 debug console commands** now check `HasInstance` and print `"HungerRevamped not active"` instead of crashing when called from the main menu

### Performance
- `HarvestFoodForCanPatch.SpawnGear` — `Resources.FindObjectsOfTypeAll<GearItemData>()` result is now cached; previously called on every button press
- `RecipeGiveCan.SpawnGear` — same caching applied
- `CookingConditionFix` — removed `logger.Msg()` calls that fired on every single cook action; replaced with `DebugHelper.Log()` (debug-only)

### Localization
- Added `Localization.cs` — `LocalizationManager` with embedded JSON + UserData override support, and two Harmony interceptors (`Localization.Get`, `ModSettings.DescriptionHolder.Text`)
- Added `localization.json` — **English** and **Russian** translations for all settings and UI strings
- `MenuSettings` — all `[Name]`/`[Description]` attributes now use `HR.*` keys with `Localize = true`; settings grouped into three sections: *Ruined Food*, *Food Poisoning*, *Cooking*
- `CustomModeSettings` — starting calories option localized
- `HarvestFoodForCanPatch` — "Harvest Can" button label and all HUD messages now localized

### Code Quality
- `RecipeGiveCan` — all classes changed from `public` to `internal`
- `HungerRevampedMod` — removed redundant `Debug.Log` + temporary `new MelonLogger.Instance(...)` double-logging; uses `LoggerInstance` now
- `HungerRevamped.csproj` — added `localization.json` as `EmbeddedResource`

---

## [2.1.0] - original release by BaltaZar
