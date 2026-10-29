using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Library;

namespace BannerlordSmartSuite
{
    public static class SettlementNeedEvaluator
    {
        public static void EvaluateAndEnqueue(Town town)
        {
            if (town == null || !Settings.Instance.ModEnabled || !Settings.Instance.AutoSettlementBuilderEnabled)
                return;

            if (town.BuildingsInProgress.Count > 0)
                return;

            try
            {
                var candidates = town.Buildings
                    .Where(b => !b.BuildingType.IsDailyProject && b.CurrentLevel < 3 && !town.BuildingsInProgress.Contains(b))
                    .ToList();

                if (candidates.Count == 0)
                    return;

                Building bestBuilding = null;
                float highestScore = float.MinValue;

                foreach (var candidate in candidates)
                {
                    float score = CalculateScore(candidate, town);
                    if (score > highestScore)
                    {
                        highestScore = score;
                        bestBuilding = candidate;
                    }
                }

                if (bestBuilding != null)
                {
                    town.BuildingsInProgress.Enqueue(bestBuilding);

                    if (Settings.Instance.SettlementNotificationsEnabled)
                    {
                        NotificationHelper.NotifyAutoQueued(town, bestBuilding);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"[BannerlordSmartSuite] Error evaluating settlement needs for '{town.Name}': {ex.Message}");
            }
        }

        public static float CalculateScore(Building candidate, Town town)
        {
            float score = 100f;
            var bType = candidate.BuildingType;

            // 1. Loyalty crisis
            if (town.Loyalty < Settings.Instance.LoyaltyCriticalThreshold && bType.HasEffect(BuildingEffectEnum.Loyalty))
            {
                score += 1000f * (Settings.Instance.LoyaltyCriticalThreshold - town.Loyalty + 5f);
            }
            if (town.LoyaltyChange < 0f && town.Loyalty < 50f && bType.HasEffect(BuildingEffectEnum.Loyalty))
            {
                score += 400f;
            }

            // 2. Food crisis
            bool hasFoodEffect = bType.HasEffect(BuildingEffectEnum.FoodProduction) || bType.HasEffect(BuildingEffectEnum.FoodStock);
            if (town.FoodStocks < Settings.Instance.FoodDeficitThreshold && hasFoodEffect)
            {
                score += 600f * (Settings.Instance.FoodDeficitThreshold - town.FoodStocks + 5f);
            }
            if (town.FoodChange < 0f && hasFoodEffect)
            {
                score += 350f;
            }

            // 3. Security crisis
            if (town.Security < Settings.Instance.SecurityCriticalThreshold && bType.HasEffect(BuildingEffectEnum.SecurityPerDay))
            {
                score += 300f * (Settings.Instance.SecurityCriticalThreshold - town.Security + 5f);
            }

            // 4. Construction lever (Mason / Workshop)
            if (town.Loyalty >= 40f && town.FoodChange >= 0f && bType.HasEffect(BuildingEffectEnum.ConstructionPerDay))
            {
                score += 250f;
            }

            // 5. Fortifications (Walls)
            if (IsFortificationBuilding(candidate))
            {
                if (IsAtWar(town))
                {
                    score += 200f;
                }
                if (candidate.CurrentLevel == 0)
                {
                    score += 150f;
                }
            }

            // 6. Economy & Prosperity
            if (bType.HasEffect(BuildingEffectEnum.Prosperity) || bType.HasEffect(BuildingEffectEnum.TaxPerDay))
            {
                score += 100f;
            }

            // 7. Garrison & Training
            if (bType.HasEffect(BuildingEffectEnum.GarrisonCapacity) || bType.HasEffect(BuildingEffectEnum.ExperiencePerDay))
            {
                score += 80f;
            }

            return score;
        }

        private static bool IsFortificationBuilding(Building building)
        {
            if (building == null || building.BuildingType == null)
                return false;

            if (building.BuildingType.HasEffect(BuildingEffectEnum.WallRepairSpeed))
                return true;

            if (building.BuildingType == DefaultBuildingTypes.SettlementFortifications ||
                building.BuildingType == DefaultBuildingTypes.CastleFortifications)
                return true;

            string id = building.BuildingType.StringId;
            return !string.IsNullOrEmpty(id) && id.IndexOf("fortification", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsAtWar(Town town)
        {
            var mapFaction = town?.Settlement?.MapFaction;
            if (mapFaction == null)
                return false;

            return mapFaction.FactionsAtWarWith != null && mapFaction.FactionsAtWarWith.Count > 0;
        }
    }
}
