using MelonLoader;

[assembly: MelonInfo(typeof(AimLabsUltimateUnlocker_MelonLoader.Core), "AimLabsUltimateUnlocker", "1.1.0", "accountrev", "https://github.com/accountrev/AimLabsUltimateUnlocker/releases")]
[assembly: MelonGame("Statespace", "aimlab_tb")]

namespace AimLabsUltimateUnlocker_MelonLoader
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("AimLabsUltimateUnlocker has been initialized.");
        }

        public override void OnLateUpdate()
        {
            // Show user that the patch has been applied 
            ProTabLabel.Apply();
        }
    }
}