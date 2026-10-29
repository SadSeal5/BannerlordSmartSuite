using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BannerlordSmartSuite
{
    public static class CaptainScoringEngine
    {
        public static float ScoreHeroOrAgent(Hero hero, Agent agent, FormationClass formationClass)
        {
            if (hero == null && agent == null)
                return -10000f;

            BasicCharacterObject character = hero?.CharacterObject ?? agent?.Character;
            if (character == null)
                return -10000f;

            FormationClass mappedClass = NormalizeFormationClass(formationClass);

            bool hasHorse = CheckHasHorse(hero, agent);

            // 1. Equipment constraints
            if (mappedClass == FormationClass.Cavalry || mappedClass == FormationClass.HorseArcher)
            {
                if (!hasHorse)
                    return -10000f;
            }

            if (mappedClass == FormationClass.Ranged || mappedClass == FormationClass.HorseArcher)
            {
                bool allowThrown = (mappedClass != FormationClass.HorseArcher);
                if (!CheckHasRangedWeapon(hero, agent, allowThrown))
                    return -5000f;
            }

            float score = 0f;

            if (mappedClass == FormationClass.Infantry)
            {
                if (hasHorse && !Settings.Instance.DismountMountedInfantryCaptains)
                {
                    score -= 50f;
                }
            }

            // 2. Combat skills
            int athletics = character.GetSkillValue(DefaultSkills.Athletics);
            int oneHanded = character.GetSkillValue(DefaultSkills.OneHanded);
            int twoHanded = character.GetSkillValue(DefaultSkills.TwoHanded);
            int polearm = character.GetSkillValue(DefaultSkills.Polearm);
            int bow = character.GetSkillValue(DefaultSkills.Bow);
            int crossbow = character.GetSkillValue(DefaultSkills.Crossbow);
            int throwing = character.GetSkillValue(DefaultSkills.Throwing);
            int riding = character.GetSkillValue(DefaultSkills.Riding);

            switch (mappedClass)
            {
                case FormationClass.Infantry:
                    score += athletics * 1.0f + oneHanded * 0.7f + twoHanded * 0.7f + polearm * 0.7f;
                    break;

                case FormationClass.Ranged:
                    score += bow * 1.2f + crossbow * 1.1f + athletics * 0.6f + throwing * 0.5f;
                    break;

                case FormationClass.Cavalry:
                    score += riding * 1.2f + polearm * 0.8f + oneHanded * 0.7f + twoHanded * 0.7f;
                    break;

                case FormationClass.HorseArcher:
                    score += riding * 1.0f + bow * 1.1f + crossbow * 0.9f;
                    break;
            }

            // 3. Command skills (Tactics + Leadership across all roles)
            int tactics = character.GetSkillValue(DefaultSkills.Tactics);
            int leadership = character.GetSkillValue(DefaultSkills.Leadership);
            score += tactics * 0.5f + leadership * 0.5f;

            // 4. Perks (+25f per applicable captain perk)
            if (hero != null)
            {
                try
                {
                    TroopUsageFlags flags = GetTroopUsageFlags(mappedClass);
                    BattleCaptainModel captainModel = Campaign.Current?.Models?.BattleCaptainModel;
                    if (captainModel == null && MissionGameModels.Current != null)
                    {
                        try
                        {
                            captainModel = HarmonyLib.Traverse.Create(MissionGameModels.Current).Property("BattleCaptainModel")?.GetValue<BattleCaptainModel>();
                        }
                        catch {}
                    }
                    captainModel ??= new DefaultBattleCaptainModel();

                    List<PerkObject> explanationList = null;
                    captainModel.GetCaptainRatingForTroopUsages(hero, flags, out explanationList);

                    if (explanationList != null)
                    {
                        score += explanationList.Count * 25f;
                    }
                }
                catch
                {
                    // Fallback gracefully if perk system is inaccessible
                }
            }

            return score;
        }

        public static float ScoreAgent(Agent agent, FormationClass formationClass)
        {
            if (agent == null) return -10000f;
            Hero hero = (agent.Character as CharacterObject)?.HeroObject;
            return ScoreHeroOrAgent(hero, agent, formationClass);
        }

        public static float ScoreHero(Hero hero, FormationClass formationClass)
        {
            if (hero == null) return -10000f;
            return ScoreHeroOrAgent(hero, null, formationClass);
        }

        public static FormationClass NormalizeFormationClass(FormationClass formationClass)
        {
            switch (formationClass)
            {
                case FormationClass.HeavyInfantry:
                case FormationClass.Infantry:
                    return FormationClass.Infantry;

                case FormationClass.Skirmisher:
                case FormationClass.Ranged:
                    return FormationClass.Ranged;

                case FormationClass.LightCavalry:
                case FormationClass.HeavyCavalry:
                case FormationClass.Cavalry:
                    return FormationClass.Cavalry;

                case FormationClass.HorseArcher:
                    return FormationClass.HorseArcher;

                default:
                    return FormationClass.Infantry;
            }
        }

        public static TroopUsageFlags GetTroopUsageFlags(FormationClass formationClass)
        {
            switch (NormalizeFormationClass(formationClass))
            {
                case FormationClass.Infantry:
                    return TroopUsageFlags.OnFoot | TroopUsageFlags.Melee | TroopUsageFlags.OneHandedUser | TroopUsageFlags.ShieldUser | TroopUsageFlags.TwoHandedUser | TroopUsageFlags.PolearmUser;

                case FormationClass.Ranged:
                    return TroopUsageFlags.OnFoot | TroopUsageFlags.Ranged | TroopUsageFlags.BowUser | TroopUsageFlags.CrossbowUser | TroopUsageFlags.ThrownUser;

                case FormationClass.Cavalry:
                    return TroopUsageFlags.Mounted | TroopUsageFlags.Melee | TroopUsageFlags.PolearmUser | TroopUsageFlags.OneHandedUser | TroopUsageFlags.ShieldUser | TroopUsageFlags.TwoHandedUser;

                case FormationClass.HorseArcher:
                    return TroopUsageFlags.Mounted | TroopUsageFlags.Ranged | TroopUsageFlags.BowUser | TroopUsageFlags.CrossbowUser;

                default:
                    return TroopUsageFlags.OnFoot | TroopUsageFlags.Melee;
            }
        }

        public static bool CheckHasHorse(Hero hero, Agent agent)
        {
            if (agent != null)
            {
                if (agent.HasMount || agent.MountAgent != null)
                    return true;

                if (agent.SpawnEquipment != null && agent.SpawnEquipment[EquipmentIndex.Horse].Item != null)
                    return true;
            }

            if (hero != null && hero.BattleEquipment != null)
            {
                if (hero.BattleEquipment[EquipmentIndex.Horse].Item != null)
                    return true;
            }

            return false;
        }

        public static bool CheckHasRangedWeapon(Hero hero, bool allowThrown = true)
        {
            return CheckHasRangedWeapon(hero, null, allowThrown);
        }

        public static bool CheckHasRangedWeapon(Hero hero, Agent agent, bool allowThrown = true)
        {
            if (agent != null)
            {
                if (allowThrown && agent.HasAnyRangedWeaponCached)
                    return true;

                if (agent.Equipment != null)
                {
                    for (EquipmentIndex i = EquipmentIndex.Weapon0; i <= EquipmentIndex.Weapon3; i++)
                    {
                        var item = agent.Equipment[i].Item;
                        if (item != null && (item.ItemType == ItemObject.ItemTypeEnum.Bow ||
                                             item.ItemType == ItemObject.ItemTypeEnum.Crossbow ||
                                             (allowThrown && item.ItemType == ItemObject.ItemTypeEnum.Thrown)))
                        {
                            return true;
                        }
                    }
                }

                if (agent.SpawnEquipment != null)
                {
                    for (EquipmentIndex i = EquipmentIndex.Weapon0; i <= EquipmentIndex.Weapon3; i++)
                    {
                        var item = agent.SpawnEquipment[i].Item;
                        if (item != null && (item.ItemType == ItemObject.ItemTypeEnum.Bow ||
                                             item.ItemType == ItemObject.ItemTypeEnum.Crossbow ||
                                             (allowThrown && item.ItemType == ItemObject.ItemTypeEnum.Thrown)))
                        {
                            return true;
                        }
                    }
                }
            }

            if (hero != null && hero.BattleEquipment != null)
            {
                for (EquipmentIndex i = EquipmentIndex.Weapon0; i <= EquipmentIndex.Weapon3; i++)
                {
                    var item = hero.BattleEquipment[i].Item;
                    if (item != null && (item.ItemType == ItemObject.ItemTypeEnum.Bow ||
                                         item.ItemType == ItemObject.ItemTypeEnum.Crossbow ||
                                         (allowThrown && item.ItemType == ItemObject.ItemTypeEnum.Thrown)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
