using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Safe;
using BannerlordSmartSuite.Core.Logging;
using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;

namespace BannerlordSmartSuite.AutoDualPerks
{
    [HarmonyPatch(typeof(PerkVM), "get__hasAlternativeAndSelected")]
    public static class PerkVMHasAlternativeAndSelectedPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PerkVM __instance, ref bool __result)
        {
            if (!Settings.Instance.ModEnabled || !Settings.Instance.AutoDualPerksEnabled)
                return true;

            // Safe handling for capstone perks or perks without alternative
            if (__instance == null || __instance.Perk == null || __instance.Perk.AlternativePerk == null || __instance.Perk.AlternativePerk == __instance.Perk)
                return true;

            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HeroDeveloper), "AddPerk")]
    public static class HeroDeveloperAddPerkPatch
    {
        [ThreadStatic]
        private static bool _isGrantingAlternative;

        [HarmonyPostfix]
        public static void Postfix(HeroDeveloper __instance, PerkObject perk)
        {
            if (_isGrantingAlternative)
            {
                return;
            }

            if (!Settings.Instance.ModEnabled || !Settings.Instance.AutoDualPerksEnabled)
            {
                return;
            }

            try
            {
                Hero hero = __instance?.Hero;
                if (hero == null || perk == null || perk.AlternativePerk == null || perk.AlternativePerk == perk)
                {
                    return;
                }

                bool isPlayer = CampaignSafe.IsPlayer(hero);
                bool isCompanionOrClan = CampaignSafe.IsCompanionOrClan(hero);

                bool shouldApply = (isPlayer && Settings.Instance.DualPerksApplyToPlayer) ||
                                   (isCompanionOrClan && Settings.Instance.DualPerksApplyToCompanions);

                if (shouldApply && !hero.GetPerkValue(perk.AlternativePerk))
                {
                    try
                    {
                        _isGrantingAlternative = true;
                        __instance.AddPerk(perk.AlternativePerk);
                    }
                    finally
                    {
                        _isGrantingAlternative = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[BannerlordSmartSuite] Error in HeroDeveloperAddPerkPatch: " + ex.Message);
            }
        }
    }
}
