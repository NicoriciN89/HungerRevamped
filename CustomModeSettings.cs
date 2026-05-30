#nullable disable
using ModSettings;

namespace HungerRevamped {
	internal class CustomModeSettings : ModSettingsBase {

		[Name("HR.CUSTOM_CALORIES", Localize = true)]
		[Description("HR.DESC_CUSTOM_CALORIES", Localize = true)]
		[Slider(0f, 20_000f, 41, NumberFormat = "{0:F0}")]
		public float startingStoredCalories = 12_000f;

		internal static CustomModeSettings settings;

		internal static void Initialize() {
			settings = new CustomModeSettings();
			settings.AddToCustomModeMenu(Position.BelowGameStart);
		}
	}
}
