using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

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

            foreach (PerkObject perk in PerkObject.All)
            {
                if (perk == null || perk.Skill == null) continue;
                if (hero.GetSkillValue(perk.Skill) >= perk.RequiredSkillValue)
                {
                    if (!hero.GetPerkValue(perk))
                    {
                        hero.HeroDeveloper.AddPerk(perk);
                    }
                    if (perk.AlternativePerk != null && !hero.GetPerkValue(perk.AlternativePerk))
                    {
                        hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                    }
                }
            }
        }

        public static void CheckAndGrantPerksForSkill(Hero hero, SkillObject skill)
        {
            if (hero == null || hero.HeroDeveloper == null || skill == null) return;
            if (PerkObject.All == null) return;

            int skillValue = hero.GetSkillValue(skill);
            foreach (PerkObject perk in PerkObject.All)
            {
                if (perk == null || perk.Skill != skill) continue;
                if (skillValue >= perk.RequiredSkillValue)
                {
                    if (!hero.GetPerkValue(perk))
                    {
                        hero.HeroDeveloper.AddPerk(perk);
                    }
                    if (perk.AlternativePerk != null && !hero.GetPerkValue(perk.AlternativePerk))
                    {
                        hero.HeroDeveloper.AddPerk(perk.AlternativePerk);
                    }
                }
            }
        }
    }
}
