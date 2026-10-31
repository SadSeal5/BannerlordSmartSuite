using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Safe;
using BannerlordSmartSuite.Core.Logging;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordSmartSuite.AutoDualPerks
{
    public class AutoDualPerksBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
            CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, OnHeroGainedSkill);
            CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, OnCompanionAdded);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // Empty sync data ensures complete save game compatibility and vanilla safety.
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            CheckAndGrantEligibleHeroes();
        }

        private void OnGameLoaded(CampaignGameStarter starter)
        {
            CheckAndGrantEligibleHeroes();
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            CheckAndGrantEligibleHeroes();
        }

        private void OnCompanionAdded(Hero newCompanion)
        {
            if (newCompanion == null || !Settings.Instance.ModEnabled || !Settings.Instance.AutoDualPerksEnabled)
                return;

            if (Settings.Instance.DualPerksApplyToCompanions)
            {
                CheckAndGrantAllPerks(newCompanion);
            }
        }

        private void OnHeroGainedSkill(Hero hero, SkillObject skill, int change, bool shouldNotify)
        {
            if (hero == null || skill == null || !Settings.Instance.ModEnabled || !Settings.Instance.AutoDualPerksEnabled)
                return;

            try
            {
                bool isPlayer = CampaignSafe.IsPlayer(hero);
                bool isCompanionOrClan = CampaignSafe.IsCompanionOrClan(hero);

                if ((isPlayer && Settings.Instance.DualPerksApplyToPlayer) ||
                    (isCompanionOrClan && Settings.Instance.DualPerksApplyToCompanions))
                {
                    CheckAndGrantPerksForSkill(hero, skill);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("[BannerlordSmartSuite] Error in OnHeroGainedSkill: " + ex.Message);
            }
        }

        private void CheckAndGrantEligibleHeroes()
        {
            if (!Settings.Instance.ModEnabled || !Settings.Instance.AutoDualPerksEnabled)
                return;

            try
            {
                Hero mainHero = CampaignSafe.MainHero;
                if (Settings.Instance.DualPerksApplyToPlayer && mainHero != null)
                {
                    CheckAndGrantAllPerks(mainHero);
                }

                Clan playerClan = CampaignSafe.PlayerClan;
                if (Settings.Instance.DualPerksApplyToCompanions && playerClan != null)
                {
                    if (playerClan.Heroes != null)
                    {
                        foreach (Hero hero in playerClan.Heroes)
                        {
                            if (hero != null && hero != mainHero && hero.IsAlive)
                            {
                                CheckAndGrantAllPerks(hero);
                            }
                        }
                    }

                    if (playerClan.Companions != null)
                    {
                        foreach (Hero companion in playerClan.Companions)
                        {
                            if (companion != null && companion.IsAlive)
                            {
                                CheckAndGrantAllPerks(companion);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("[BannerlordSmartSuite] Error in CheckAndGrantEligibleHeroes: " + ex.Message);
            }
        }

        public static void CheckAndGrantAllPerks(Hero hero)
        {
            if (hero == null || hero.HeroDeveloper == null) return;
            if (PerkObject.All == null) return;

            int grantMode = Settings.Instance.PerkGrantMode?.SelectedIndex ?? 0;
            int perksGranted = 0;

            if (grantMode == 0)
            {
                // Paired Choice (When Picked)
                foreach (PerkObject perk in PerkObject.All)
                {
                    if (perk == null || perk.AlternativePerk == null || perk.AlternativePerk == perk) continue;

                    if (hero.GetPerkValue(perk) && !hero.GetPerkValue(perk.AlternativePerk))
                    {
                        hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                        perksGranted++;
                    }
                }
            }
            else
            {
                // Full Auto Unlock (By Skill Level)
                foreach (PerkObject perk in PerkObject.All)
                {
                    if (perk == null || perk.Skill == null) continue;

                    if (hero.GetSkillValue(perk.Skill) >= perk.RequiredSkillValue)
                    {
                        if (!hero.GetPerkValue(perk))
                        {
                            hero.HeroDeveloper.AddPerk(perk);
                            perksGranted++;
                        }
                        if (perk.AlternativePerk != null && perk.AlternativePerk != perk && !hero.GetPerkValue(perk.AlternativePerk))
                        {
                            hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                            perksGranted++;
                        }
                    }
                }
            }

            if (perksGranted > 0 && Settings.Instance.PerkNotificationsEnabled)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[Auto Dual Perks] Granted {perksGranted} perk{(perksGranted > 1 ? "s" : "")} to {hero.Name}.",
                    Color.FromUint(0x40E0D0FF)
                ));
            }
        }

        public static void CheckAndGrantPerksForSkill(Hero hero, SkillObject skill)
        {
            if (hero == null || hero.HeroDeveloper == null || skill == null) return;
            if (PerkObject.All == null) return;

            int grantMode = Settings.Instance.PerkGrantMode?.SelectedIndex ?? 0;
            int skillValue = hero.GetSkillValue(skill);
            int perksGranted = 0;

            if (grantMode == 0)
            {
                // Paired Choice (When Picked)
                foreach (PerkObject perk in PerkObject.All)
                {
                    if (perk == null || perk.Skill != skill || perk.AlternativePerk == null || perk.AlternativePerk == perk) continue;

                    if (hero.GetPerkValue(perk) && !hero.GetPerkValue(perk.AlternativePerk))
                    {
                        hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                        perksGranted++;
                    }
                }
            }
            else
            {
                // Full Auto Unlock (By Skill Level)
                foreach (PerkObject perk in PerkObject.All)
                {
                    if (perk == null || perk.Skill != skill) continue;

                    if (skillValue >= perk.RequiredSkillValue)
                    {
                        if (!hero.GetPerkValue(perk))
                        {
                            hero.HeroDeveloper.AddPerk(perk);
                            perksGranted++;
                        }
                        if (perk.AlternativePerk != null && perk.AlternativePerk != perk && !hero.GetPerkValue(perk.AlternativePerk))
                        {
                            hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                            perksGranted++;
                        }
                    }
                }
            }

            if (perksGranted > 0 && Settings.Instance.PerkNotificationsEnabled)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[Auto Dual Perks] Granted {perksGranted} perk{(perksGranted > 1 ? "s" : "")} to {hero.Name}.",
                    Color.FromUint(0x40E0D0FF)
                ));
            }
        }
    }
}
