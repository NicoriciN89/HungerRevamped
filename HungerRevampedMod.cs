#nullable disable
using MelonLoader;

namespace HungerRevamped {
    internal sealed class HungerRevampedMod : MelonMod
    {
        internal static SaveDataManager sdm = new SaveDataManager();

        public override void OnInitializeMelon()
        {
            CustomModeSettings.Initialize();
            MenuSettings.Initialize();
            LoggerInstance.Msg($"Version {Info.Version} loaded");
            DebugHelper.Init();
        }
    }
}
