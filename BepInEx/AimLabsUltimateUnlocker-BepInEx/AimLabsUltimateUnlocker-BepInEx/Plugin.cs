using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace AimLabsUltimateUnlocker_BepInEx
{
    [BepInPlugin("org.accountrev.aluu", "AimLabsUltimateUnlocker-BepInEx", "1.0.0")]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;

        public override void Load()
        {
            // Plugin startup logic
            Log = base.Log;

            HarmonyLib.Harmony harmony = new (MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
