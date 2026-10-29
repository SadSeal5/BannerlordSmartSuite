using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BannerlordSmartSuite
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

            bool isPlayer = (hero == Hero.MainHero);
            bool isCompanionOrClan = (hero != Hero.MainHero && (hero.IsPlayerCompanion || hero.Clan == Clan.PlayerClan));

            if ((isPlayer && Settings.Instance.DualPerksApplyToPlayer) ||
                (isCompanionOrClan && Settings.Instance.DualPerksApplyToCompanions))
            {
                CheckAndGrantPerksForSkill(hero, skill);
            }
        }

        private void CheckAndGrantEligibleHeroes()
        {
            if (!Settings.Instance.ModEnabled || !Settings.Instance.AutoDualPerksEnabled)
                return;

            if (Settings.Instance.DualPerksApplyToPlayer && Hero.MainHero != null)
            {
                CheckAndGrantAllPerks(Hero.MainHero);
            }

            if (Settings.Instance.DualPerksApplyToCompanions && Clan.PlayerClan != null)
            {
                if (Clan.PlayerClan.Heroes != null)
                {
                    foreach (Hero hero in Clan.PlayerClan.Heroes)
                    {
                        if (hero != null && hero != Hero.MainHero && hero.IsAlive)
                        {
                            CheckAndGrantAllPerks(hero);
                        }
                    }
                }

                if (Clan.PlayerClan.Companions != null)
                {
                    foreach (Hero companion in Clan.PlayerClan.Companions)
                    {
                        if (companion != null && companion.IsAlive)
                        {
                            CheckAndGrantAllPerks(companion);
                        }
                    }
                }
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
