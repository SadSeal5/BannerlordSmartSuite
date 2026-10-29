using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;

namespace BannerlordSmartSuite
{
    [HarmonyPatch(typeof(PerkVM), "get__hasAlternativeAndSelected")]
    public static class PerkVMHasAlternativeAndSelectedPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            if (!Settings.Instance.ModEnabled || !Settings.Instance.AutoDualPerksEnabled)
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

            Hero hero = __instance?.Hero;
            if (hero == null || perk?.AlternativePerk == null)
            {
                return;
            }

            bool isPlayer = (hero == Hero.MainHero);
            bool isCompanionOrClan = (hero != Hero.MainHero && (hero.IsPlayerCompanion || hero.Clan == Clan.PlayerClan));

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
    }
}
