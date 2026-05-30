#nullable disable
using ModSettings;

namespace HungerRevamped {
	internal class MenuSettings : JsonModSettings {

		[Section("HR.SEC_FOOD", Localize = true)]

		[Name("HR.RUINED_EAT", Localize = true)]
		[Description("HR.DESC_RUINED_EAT", Localize = true)]
		public bool canEatRuinedFood = false;

		[Name("HR.RUINED_COOK", Localize = true)]
		[Description("HR.DESC_RUINED_COOK", Localize = true)]
		public bool canCookRuinedFood = false;

		[Section("HR.SEC_POISON", Localize = true)]

		[Name("HR.DELAYED_POISON", Localize = true)]
		[Description("HR.DESC_DELAYED_POISON", Localize = true)]
		public bool delayedFoodPoisoning = true;

		[Name("HR.REALISTIC_POISON", Localize = true)]
		[Description("HR.DESC_REALISTIC_POISON", Localize = true)]
		public bool realisticFoodPoisoningChance = true;

		[Name("HR.REMOVE_IMMUNITY", Localize = true)]
		[Description("HR.DESC_REMOVE_IMMUNITY", Localize = true)]
		public bool removeFoodPoisonImmunity = false;

		[Section("HR.SEC_COOKING", Localize = true)]

		[Name("HR.FIX_EXPLOIT", Localize = true)]
		[Description("HR.DESC_FIX_EXPLOIT", Localize = true)]
		public bool fixCookingSkillExploit = true;

		[Name("HR.COOK_DOUBLES", Localize = true)]
		[Description("HR.DESC_COOK_DOUBLES", Localize = true)]
		public bool cookingDoublesCondition = true;

		internal static MenuSettings settings;

		internal static void Initialize() {
			settings = new MenuSettings();
			settings.AddToModSettings("Hunger Revamped");
		}
	}
}
