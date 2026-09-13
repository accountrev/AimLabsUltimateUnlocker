using HarmonyLib;
using Il2Cpp;
using Il2CppAimLab.Economy;
using Il2CppAimLab.GraphQL;
using Il2CppAimLab.Inventory;
using Il2CppAimLab.Models.Economy;
using Il2CppAimLab.UI.Popups;
using Il2CppAimLab.Utility;
using Il2CppAimlabs.Navigation;
using Il2CppAimLabUtilities;
using Il2CppServices.AchievementBanners;
using MelonLoader;

namespace AimLabsUltimateUnlocker_MelonLoader
{
    #region Error Handling
    // Error handler/logger for patches: handles and logs any errors that can happen between AimLabs updates.
    
    internal static class PatchErrorHandler 
    {
        // Errors get stored into a HashSet to prevent constant error spam in the console
        private static readonly HashSet<string> reported = new HashSet<string>();

        internal static void Report(string patch, Exception exception, bool noNotification = false)
        {
            if (!reported.Add(patch)) return;

            Melon<Core>.Logger.Error($"{patch} patch failed and has been skipped. Returning this function back to AimLabs to run originally.\n\nEXCEPTION: {exception}");
            
            if (noNotification)
                PlayerNotification.Show($"{Melon<Core>.Instance.Info.Name} has encountered an exception. Please check the console for more details.", ToastPopupPreset.Error, 5.0f);
        }
    }
    
    #endregion

    #region IdentityService Patches
    // IdentityService patches

    [HarmonyPatch(typeof(IdentityService), "IsUserSubscribed", new Type[] { typeof(AimLabPlusSubscriptionTiers) })]
    internal static class AimLabsPlusSubscriptionPatch
    {
        private static bool Prefix(ref bool __result)
        {
            __result = true;

            Melon<Core>.Logger.Msg($"Intercepted request for AimLabs Pro subscription check");

            return false;
        }
    }

    #endregion

    #region CosmeticSettingsUtility Patches
    // CosmeticSettingsUtility patches

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "IsOwnedItem", new Type[] { typeof(CatalogItem), typeof(bool) })]
    internal static class CosmeticsIsOwnedItemPatch
    {
        private static bool Prefix(ref SkinOwnedResult __result)
        {
            __result = SkinOwnedResult.Owned;

            Melon<Core>.Logger.Msg($"Intercepted request for CosmeticSettingsUtility IsOwnedItem check");

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

    #endregion

    #region Inventory Patches
    // Inventory patches for AimLabs "2.0" update

    [HarmonyPatch(typeof(InventorySnapshot), "IsOwned", new Type[] { typeof(CatalogItem) })]
    internal static class InventorySnapshotIsOwnedPatch
    {
        private static bool Prefix(ref bool __result)
        {
            __result = true;

            Melon<Core>.Logger.Msg($"Intercepted request for InventorySnapshot IsOwned check");

            return false;
        }
    }

    [HarmonyPatch(typeof(UserInventoryModel), "IsItemOwned", new Type[] { typeof(string) })]
    internal static class UserInventoryModelIsItemOwnedPatch
    {
        private static bool Prefix(ref bool __result)
        {
            __result = true;

            Melon<Core>.Logger.Msg($"Intercepted request for UserInventoryModel IsItemOwned check");

            return false;
        }
    }

    // This patch handles the new inventory's "owned" filter, and
    // sets all items in the inventory as "owned".
    [HarmonyPatch(typeof(InventoryService), "BuildOwnedIds")]
    internal static class InventoryServiceBuildOwnedIdsPatch
    {
        private static void Postfix(Il2CppSystem.Collections.Generic.HashSet<string> __result)
        {
            try
            {
                var catalog = ItemCatalogModel.Instance;

                if (__result == null || catalog == null) return;

                var before = __result.Count;
                var types = catalog.GetItemTypes();

                for (var i = 0; types != null && i < types.Count; i++)
                {
                    var items = catalog.GetItemOfType(types[i]);

                    for (var j = 0; items != null && j < items.Count; j++)
                    {
                        var item = items[j];

                        if (item == null) continue;

                        if (!string.IsNullOrEmpty(item.ID)) __result.Add(item.ID);
                        if (!string.IsNullOrEmpty(item.PlayFabID)) __result.Add(item.PlayFabID);
                    }
                }

                Melon<Core>.Logger.Msg($"Intercepted request for InventoryService BuildOwnedIds ({before} -> {__result.Count} owned ids)");
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(InventoryServiceBuildOwnedIdsPatch), e);
            }
        }
    }

    // Activates any items with IsActive set to false (unreleased/hidden items)
    // Helper functions
    internal static class CosmeticActivation
    {
        private static readonly HashSet<string> reported = new HashSet<string>();

        internal static void Activate(string kind, Il2CppAimLab.Models.Standard item)
        {
            if (item == null || item.IsActive) return;

            item.IsActive = true;

            if (reported.Add(kind + "/" + item.name))
                Melon<Core>.Logger.Msg($"Activated inactive {kind} '{item.name}'");
        }

        internal static void ActivateWeaponSkins()
        {
            var skins = WeaponSkinUtility.All;

            for (var i = 0; skins != null && i < skins.Count; i++)
                Activate("Weapon skin", skins[i]);
        }

        internal static void ActivateCatalog()
        {
            var catalog = ItemCatalogModel.Instance;
            var classes = catalog?.GetItemTypes();

            for (var i = 0; classes != null && i < classes.Count; i++)
            {
                var items = catalog.GetItemOfType(classes[i]);

                for (var j = 0; items != null && j < items.Count; j++)
                {
                    var item = items[j];

                    if (item != null && IsCosmetic(item))
                        Activate("Catalog item", item);
                }
            }
        }

        internal static bool IsCosmetic(CatalogItem item)
        {
            var type = InventoryUtility.GetCatalogItemType(item);

            return type != InventoryUtility.CatalogItemType.NONE && type != InventoryUtility.CatalogItemType.BUNDLE;
        }
    }

    
    // Activates any avatars with IsActive set to false (unreleased/hidden avatars)
    // Uses CosmeticActivation to do so
    [HarmonyPatch(typeof(ProfileCustomizationAvatarTabView), "CollectItems")]
    internal static class AvatarActivationPatch
    {
        private static void Prefix(Il2CppSystem.Collections.Generic.List<Il2CppAimLab.Models.AvatarSkin> __0)
        {
            try
            {
                for (var i = 0; __0 != null && i < __0.Count; i++)
                    CosmeticActivation.Activate("avatar", __0[i]);
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(AvatarActivationPatch), e);
            }
        }
    }

    // Activates any targets with IsActive set to false (unreleased/hidden avatars)
    // Uses CosmeticActivation to do so, but we need to add the targets to a list
    // for the argument compared to other functions

    [HarmonyPatch(typeof(TargetCosmeticUtility), "get_ActiveTargetCosmetics")]
    internal static class TargetActivationPatch
    {
        private static void Postfix(Il2CppSystem.Collections.Generic.List<TargetCosmetic> __result)
        {
            try
            {
                var all = TargetCosmeticUtility.All;

                if (__result == null || all == null || __result.Count >= all.Count) return;

                for (var i = 0; i < all.Count; i++)
                {
                    var target = all[i];

                    if (target == null) continue;

                    CosmeticActivation.Activate("target", target);

                    if (!__result.Contains(target)) __result.Add(target);
                }
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(TargetActivationPatch), e);
            }
        }
    }

    // Activates any weapon skins with IsActive set to false (unreleased/hidden weapon skins)
    // Uses CosmeticActivation to do so
    [HarmonyPatch(typeof(WeaponSkinUtility), "GetForWeaponAsync")]
    internal static class WeaponSkinActivationPatch
    {
        private static void Prefix()
        {
            try
            {
                CosmeticActivation.ActivateWeaponSkins();
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(WeaponSkinActivationPatch), e);
            }
        }
    }

    // Sometimes, GetForWeaponAsync function above never gets called. This
    // can happen if the player never visits the weapon skin menu during their
    // session. This patch below ensures this won't cause an issue.

    [HarmonyPatch(typeof(WeaponService), "GetSkinForWeapon")]
    internal static class WeaponServiceSkinActivationPatch
    {
        private static void Prefix()
        {
            try
            {
                CosmeticActivation.ActivateWeaponSkins();
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(WeaponServiceSkinActivationPatch), e);
            }
        }
    }
    
    [HarmonyPatch(typeof(InventoryService), "ReloadAsync")]
    internal static class InventoryCatalogActivationPatch
    {
        private static void Prefix()
        {
            try
            {
                CosmeticActivation.ActivateCatalog();
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(InventoryCatalogActivationPatch), e);
            }
        }
    }

    // There is a target in the game attached to the KFC Secret Menu x AimLabs collab,
    // but never exists as a selectable item within the game. This collection of patches
    // is dedicated to bringing this back by reconstructing the CatalogItem.
    //  If you're curious: the "burger" is called The Triple Double Sandwich. Awesome
    internal static class KfcBurgerCatalogEntry
    {
        private const string TargetName = "KFCBurger_Target";
        private const string AssetGuid = "8a25470e28f80ca498ce337dbe16bdc0";
        private const string TargetItemClass = "Target";

        private static bool registered;

        internal static void Register()
        {
            if (registered) return;

            var catalog = ItemCatalogModel.Instance;
            var targets = catalog?.GetItemOfType(TargetItemClass);
            var burger = FindBurger();

            // Not loaded yet, the next inventory reload tries again
            if (targets == null || targets.Count == 0 || burger == null) return;

            // The inventory preview checks the entry ID against the asset's own PlayFabID, and the
            // equipped badge depends on the two matching as well
            var id = burger.PlayFabID;

            if (string.IsNullOrEmpty(id)) return;

            CatalogItem source = null;

            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (target == null) continue;

                if (target.ID == id)
                {
                    registered = true;
                    return;
                }

                if (source == null && target.HasItem && !target.IsBundle)
                    source = target;
            }

            if (source == null) return;

            // One attempt only from here
            // A failure part way through would otherwise leave an unregistered clone behind on every inventory reload
            registered = true;

            var entry = UnityEngine.Object.Instantiate(source);
            
            entry.hideFlags = UnityEngine.HideFlags.DontUnloadUnusedAsset;
            entry.name = id;
            entry.ID = id;
            entry.Item = new UnityEngine.AddressableAssets.AssetReference(AssetGuid);
            entry.Title = burger.Title;
            entry.Description = burger.Title; // Contains no description? Reuse title
            entry.Image = null;

            var before = targets.Count;

            catalog.AddItem(entry);

            //Melon<Core>.Logger.Msg($"Registered inventory entry '{id}' for {TargetName}, cloned from '{source.ID}' " + $"(Target entries {before} -> {catalog.GetItemOfType(TargetItemClass)?.Count}, valid asset key: {entry.HasItem})");
        }

        private static TargetCosmetic FindBurger()
        {
            var all = TargetCosmeticUtility.All;

            for (var i = 0; all != null && i < all.Count; i++)
            {
                if (all[i] != null && all[i].name == TargetName)
                    return all[i];
            }

            return null;
        }
    }

    [HarmonyPatch(typeof(InventoryService), "ReloadAsync")]
    internal static class KfcBurgerCatalogEntryPatch
    {
        private static void Prefix()
        {
            try
            {
                KfcBurgerCatalogEntry.Register();
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(KfcBurgerCatalogEntryPatch), e);
            }
        }
    }

    // Some avatars that are unreleased/hidden are not allowed to be shown server-side,
    // so this patch allows actually being able to equip the avatars clientside instead.
    [HarmonyPatch(typeof(Il2CppAimLab.Content.Shop.CatalogItemExtensionMethods), "IsAvailable", new Type[] { typeof(EconomyItem) })]
    internal static class AvatarAvailabilityPatch
    {
        private static readonly HashSet<string> reported = new HashSet<string>();

        private static void Postfix(EconomyItem __0, ref bool __result)
        {
            try
            {
                if (__result || __0 == null || __0.TryCast<Il2CppAimLab.Models.AvatarSkin>() == null)
                    return;

                __result = true;

                if (reported.Add(__0.name))
                    Melon<Core>.Logger.Msg($"Made unavailable avatar '{__0.name}' available");
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(AvatarAvailabilityPatch), e);
            }
        }
    }

    // Forces all items to show up in the inventory by modifying IsListable to true on all items
    [HarmonyPatch(typeof(InventoryService), "IsListable")]
    internal static class InventoryListAllItemsPatch
    {
        private static readonly HashSet<string> reported = new HashSet<string>();

        private static void Postfix(CatalogItem __0, InventoryUtility.CatalogItemType __1, ref bool __result)
        {
            try
            {
                if (__result || __0 == null || __0.IsBundle || InventoryUtility.IsBundleItem(__0))
                    return;

                if (InventoryUtility.GetCatalogItemType(__0) != __1) return;

                __result = true;

                if (reported.Add(__0.ID ?? ""))
                    Melon<Core>.Logger.Msg($"Listed hidden {__1} '{__0.ID}' in the inventory");
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(InventoryListAllItemsPatch), e);
            }
        }
    }

    #endregion

    #region Achievement Banner Patches
    // Achievement banner patches for AimLabs "2.0" update
    // If you've never heard of this in-game, it probably hasn't released yet
    // Therefore, this patch is client-sided (for now?)

    [HarmonyPatch(typeof(AchievementBannerService), "IsUnlocked")]
    internal static class AchievementBannerIsUnlockedPatch
    {
        private static bool Prefix(ref bool __result)
        {
            __result = true;

            Melon<Core>.Logger.Msg("Intercepted request for AchievementBannerService IsUnlocked check");

            return false;
        }
    }

    [HarmonyPatch(typeof(AchievementBannerService), "IsAchievableHere")]
    internal static class AchievementBannerIsAchievableHerePatch
    {
        private static bool Prefix(ref bool __result)
        {
            __result = true;

            Melon<Core>.Logger.Msg("Intercepted request for AchievementBannerService IsAchievableHere check");

            return false;
        }
    }

    // Handles achievement banners that require certain stats to unlock
    // Since we can't (and shouldn't) manipulate the statistics of a player, we can
    // change the "check" it looks for by replacing the banner group and key
    [HarmonyPatch(typeof(ProfileCustomizationAchievementBannerTabView), "CollectItems")]
    internal static class AchievementBannerUngatePatch
    {
        private const string PlaceholderKey = "AIMLABS_ULTIMATE_UNLOCKER_PLACEHOLDER_KEY_LOL";

        private static void Prefix()
        {
            try
            {
                var banners = AchievementBannerUtility.All;
                var split = 0;
                var keyed = 0;

                for (var i = 0; banners != null && i < banners.Count; i++)
                {
                    var banner = banners[i];

                    if (banner == null) continue;

                    var name = banner.name;

                    if (string.IsNullOrEmpty(name)) continue;

                    if (banner.tierGroup != name)
                    {
                        banner.tierGroup = name;
                        split++;
                    }

                    if (string.IsNullOrWhiteSpace(banner.achievementKey))
                    {
                        banner.achievementKey = PlaceholderKey;
                        keyed++;
                    }
                }

                Melon<Core>.Logger.Msg($"Ungated achievement banners - regrouped: {split}, keyed: {keyed}");
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(AchievementBannerUngatePatch), e);
            }
        }
    }

    // Achievements are not meant to be accessed yet by the public, but the devs were
    // willing enough to leave the tab invisible in the game. We force enable it.
    // This patch actually enables all hidden tabs as for future proofing. 
    [HarmonyPatch(typeof(ProfileCustomizationModalController), "OnEnable")]
    internal static class AchievementBannerTabRevealPatch
    {
        private const string TabsContainerPath = "Content/ModalBody/CustomizationSelector/TabBar/StandardTabSwitcher/TabsContainer";

        private static void Postfix(ProfileCustomizationModalController __instance)
        {
            try
            {
                var container = __instance.transform.Find(TabsContainerPath);

                if (container == null)
                {
                    Melon<Core>.Logger.Msg($"Could not resolve '{TabsContainerPath}' under '{__instance.name}'");
                    return;
                }

                for (var i = 0; i < container.childCount; i++)
                {
                    var tab = container.GetChild(i);

                    if (tab.gameObject.activeSelf) continue;

                    tab.gameObject.SetActive(true);

                    Melon<Core>.Logger.Msg($"Enabled navigation tab '{tab.name}'");
                }
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(AchievementBannerTabRevealPatch), e);
            }
        }
    }

    #endregion

    #region Profile Avatar and Banner Fix Patches
    // A fix for client-side avatar and banner customization for AimLabs "2.0" update

    [HarmonyPatch(typeof(CosmeticSettingsUtility), "StageProfileImages", new Type[] { typeof(UpdateAimlabProfileCosmeticsInput), typeof(ProfileImageElementData), typeof(ProfileImageElementData) })]
    internal static class CosmeticsStageProfileImagesPatch
    {
        private static void Postfix(UpdateAimlabProfileCosmeticsInput __0, ProfileImageElementData __1, ProfileImageElementData __2)
        {
            try
            {
                var profile = IdentityService.AimlabProfile;
                var cosmetics = profile?.profileCosmetics;

                if (cosmetics == null) return;

                ApplyElement(cosmetics.profileIcon, __0?.profileIconSlug, __1);
                ApplyElement(cosmetics.profileBanner, __0?.profileBannerSlug, __2);

                // AimLabs does more server checks now and tries to update all profile
                // items to sync them, regardless if it's being edited. Therefore, we
                // force replace it back to our changes instead
                ApplyElement(cosmetics.avatar, __0?.avatarSlug, null);
                ApplyElement(cosmetics.title, __0?.titleSlug, null);

                CosmeticSettingsUtility.ApplyProfileCosmetics(profile, cosmetics);

                Melon<Core>.Logger.Msg("Applied staged profile images to the local profile");
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(CosmeticsStageProfileImagesPatch), e);
            }
        }

        private static void ApplyElement(ProfileCosmetic cosmetic, string slug, ProfileImageElementData element)
        {
            if (cosmetic == null || string.IsNullOrEmpty(slug)) return;

            cosmetic.slug = slug;
            var url = element?.Image?.URL;

            if (string.IsNullOrEmpty(url)) url = element?.LegacyImageUrl;

            if (!string.IsNullOrEmpty(url)) cosmetic.url = url;
        }
    }

    #endregion

    #region Topbar Indication Patches
    // Renames the "AimLabs Pro" button to "AimLabsUltimateUnlocker v#.##"
    // Helps with verifying the patch has been applied
    
    internal static class ProTabLabel
    {
        private const string LabelPath = "Lobby/Canvas/Overlay/TopNav/TopNav/TopNavContainer/LeftTopNav/Pro/Label";
        private const float CheckInterval = 0.25f;

        private static Il2CppTMPro.TMP_Text label;
        private static string text;
        private static float nextCheck;
        private static bool alreadySet;

        internal static void Apply()
        {
            if (UnityEngine.Time.unscaledTime < nextCheck)
                return;

            nextCheck = UnityEngine.Time.unscaledTime + CheckInterval;
            

            try
            {
                if (label == null)
                {
                    label = UnityEngine.GameObject.Find(LabelPath)?.GetComponent<Il2CppTMPro.TMP_Text>();

                    if (label == null) return;
                }

                text ??= $"{Melon<Core>.Instance.Info.Name} v{Melon<Core>.Instance.Info.Version}";

                if (label.text == text) return;

                label.text = text;

                if (alreadySet) return;

                alreadySet = true;

                Melon<Core>.Logger.Msg($"Renamed Pro tab to '{text}'");

                MelonCoroutines.Start(ShowAfter($"{text} successfully loaded!", ToastPopupPreset.Saved, 3f));
            }
            catch (Exception e)
            {
                // Look the label up again next time rather than reusing a reference that failed
                label = null;

                PatchErrorHandler.Report(nameof(ProTabLabel), e);
            }
        }
        
        internal static System.Collections.IEnumerator ShowAfter(string message, ToastPopupPreset icon, float seconds)
        {
            yield return new UnityEngine.WaitForSeconds(seconds);
            PlayerNotification.Show(message, icon);
        }
    }

    #endregion

    #region Player Notifications
    // Shows a message to the player using AimLabs' own toast popup, which slides in and dismisses itself.
    // PopupManager's Show methods are static and await the "Popup" scene themselves,
    // so no PopupManager instance is needed.
    internal static class PlayerNotification
    {
        internal static void Show(string message, ToastPopupPreset icon, float seconds = 2.0f)
        {
            try
            {
                var text = new Il2CppAimLab.Common.TextData();
                text.Text = message;
                
                PopupManager.ShowGenericToastPopup(icon, text, new Il2CppSystem.Nullable<float>(seconds), null);
            }
            catch (Exception e)
            {
                PatchErrorHandler.Report(nameof(PlayerNotification), e, true);
            }
        }
    }

    #endregion
}
