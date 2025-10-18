using MelonLoader;

[assembly: MelonInfo(typeof(AimLabsUltimateUnlocker_MelonLoader.Core), "AimLabsUltimateUnlocker-MelonLoader", "1.0.0", "accountrev", null)]
[assembly: MelonGame("Statespace", "aimlab_tb")]

namespace AimLabsUltimateUnlocker_MelonLoader
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("AimLabsUltimateUnlocker has been initialized.");
        }
    }
}