using HarmonyLib;
using Il2Cpp;
using Il2CppAimLab.Models.Economy;
using Il2CppAimLab.Utility;
using Il2CppAimLabUtilities;
using MelonLoader;

namespace AimLabsUltimateUnlocker_MelonLoader
{
    #region IdentityService Patches
    // IdentityService patches

    [HarmonyPatch(typeof(IdentityService), "IsUserSubscribed", new Type[] { typeof(AimLabPlusSubscriptionTiers) })]
    internal static class AimLabsPlusSubscriptionPatch
    {
        private static bool Prefix(ref bool __result)
        {
            var old_result = __result;
            __result = true;

            Melon<Core>.Logger.Msg($"Intercepted request for AimLabs+ subscription check ({old_result} -> {__result})");

            return false;
        }
    }

    #endregion

    #region CosmeticSettingsUtility Patches
    // CosmeticSettingsUtility patches

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "IsOwnedItem", new Type[] { typeof(string) })]
    internal static class CosmeticsIsOwnedItemPatchV1
    {
        private static bool Prefix(ref SkinOwnedResult __result)
        {
            __result = SkinOwnedResult.Owned;

            Melon<Core>.Logger.Msg($"Intercepted request for CosmeticSettingsUtility IsOwnedItem check");

            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "IsOwnedItem", new Type[] { typeof(CatalogItem), typeof(bool) })]
    internal static class CosmeticsIsOwnedItemPatchV2
    {
        private static bool Prefix(ref SkinOwnedResult __result)
        {
            __result = SkinOwnedResult.Owned;

            Melon<Core>.Logger.Msg($"Intercepted request for CosmeticSettingsUtility IsOwnedItem CatalogItem variant check");

            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "OwnsWeaponSkin")]
    internal static class CosmeticsOwnsWeaponSkinPatch
    {
        private static bool Prefix(ref SkinOwnedResult __result)
        {
            __result = SkinOwnedResult.Owned;

            Melon<Core>.Logger.Msg($"Intercepted request for CosmeticSettingsUtility OwnsWeaponSkin check");

            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "HasSkin")]
    internal static class CosmeticsHasSkinPatch
    {
        private static bool Prefix(ref SkinOwnedResult __result)
        {
            __result = SkinOwnedResult.Owned;

            Melon<Core>.Logger.Msg($"Intercepted request for CosmeticSettingsUtility HasSkin check");

            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "IsOwnedSkin")]
    internal static class CosmeticsIsOwnedSkinPatch
    {
        private static bool Prefix(ref SkinOwnedResult __result)
        {
            __result = SkinOwnedResult.Owned;

            Melon<Core>.Logger.Msg($"Intercepted request for CosmeticSettingsUtility IsOwnedSkin check");

            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "OwnsArmSkin")]
    internal static class CosmeticsOwnsArmSkinPatch
    {
        private static bool Prefix(ref SkinOwnedResult __result)
        {
            __result = SkinOwnedResult.Owned;

            Melon<Core>.Logger.Msg($"Intercepted request for CosmeticSettingsUtility OwnsArmSkin check");

            return false;
        }
    }

    #endregion

    #region ProfileCustomizationImageSelectionBinding Patches
    // ProfileCustomizationImageSelectionBinding patches

    [HarmonyPatch(typeof(ProfileCustomizationImageSelectionBinding), "LoadAssetsData")]
    internal static class AvatarBannerUnlockPatch
    {
        private static void Postfix(Il2Cpp.ProfileCustomizationImageSelectionBinding __instance, Il2CppSystem.Threading.Tasks.Task __result)
        {
            foreach (var item in __instance.unownedAssetsData)
            {
                __instance.ownedAssetsData.Add(item);
            }

            __instance.unownedAssetsData.Clear();

        }
    }

    #endregion
}
