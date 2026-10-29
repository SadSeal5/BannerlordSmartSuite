using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Library;

namespace BannerlordSmartSuite
{
    public static class DailyProjectEvaluator
    {
        public static void EvaluateDailyProject(Town town)
        {
            if (town == null || !Settings.Instance.ModEnabled || !Settings.Instance.AutoSettlementBuilderEnabled)
                return;

            try
            {
                Building targetDaily = null;

                if (town.Loyalty < Settings.Instance.LoyaltyCriticalThreshold)
                {
                    targetDaily = town.Buildings.FirstOrDefault(b =>
                        b.BuildingType.IsDailyProject &&
                        b.BuildingType.HasEffect(BuildingEffectEnum.Loyalty));
                }
                else if (town.FoodStocks < Settings.Instance.FoodDeficitThreshold || (town.FoodChange < 0f && town.FoodStocks < 50f))
                {
                    targetDaily = town.Buildings.FirstOrDefault(b =>
                        b.BuildingType.IsDailyProject &&
                        (b.BuildingType.HasEffect(BuildingEffectEnum.FoodProduction) ||
                         b.BuildingType.HasEffect(BuildingEffectEnum.FoodStock)));
                }
                else
                {
                    string preferred = Settings.Instance.DefaultDailyProject?.SelectedValue ?? "Housing";
                    switch (preferred)
                    {
                        case "Housing":
                            targetDaily = town.Buildings.FirstOrDefault(b =>
                                b.BuildingType.IsDailyProject &&
                                (b.BuildingType == DefaultBuildingTypes.SettlementDailyHousing ||
                                 (b.BuildingType.StringId != null && b.BuildingType.StringId.IndexOf("housing", StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 b.BuildingType.HasEffect(BuildingEffectEnum.Prosperity)));
                            break;

                        case "Militia":
                            targetDaily = town.Buildings.FirstOrDefault(b =>
                                b.BuildingType.IsDailyProject &&
                                (b.BuildingType == DefaultBuildingTypes.SettlementDailyTrainMilitia ||
                                 b.BuildingType == DefaultBuildingTypes.CastleDailyRaiseTroops ||
                                 b.BuildingType == DefaultBuildingTypes.CastleDailyDrills ||
                                 (b.BuildingType.StringId != null && b.BuildingType.StringId.IndexOf("militia", StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 b.BuildingType.HasEffect(BuildingEffectEnum.Militia)));
                            break;

                        case "Irrigation":
                            targetDaily = town.Buildings.FirstOrDefault(b =>
                                b.BuildingType.IsDailyProject &&
                                (b.BuildingType == DefaultBuildingTypes.SettlementDailyIrrigation ||
                                 b.BuildingType == DefaultBuildingTypes.CastleDailyIrrigation ||
                                 (b.BuildingType.StringId != null && b.BuildingType.StringId.IndexOf("irrigation", StringComparison.OrdinalIgnoreCase) >= 0)));
                            break;

                        case "Festivals":
                            targetDaily = town.Buildings.FirstOrDefault(b =>
                                b.BuildingType.IsDailyProject &&
                                (b.BuildingType == DefaultBuildingTypes.SettlementDailyFestivalAndGames ||
                                 (b.BuildingType.StringId != null && b.BuildingType.StringId.IndexOf("festival", StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 b.BuildingType.HasEffect(BuildingEffectEnum.Loyalty)));
                            break;

                        case "KeepCurrent":
                        default:
                            targetDaily = null;
                            break;
                    }
                }

                if (targetDaily != null && town.CurrentDefaultBuilding != targetDaily)
                {
                    BuildingHelper.ChangeDefaultBuilding(targetDaily, town);

                    if (Settings.Instance.SettlementNotificationsEnabled)
                    {
                        NotificationHelper.NotifyDailyProjectChanged(town, targetDaily);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"[BannerlordSmartSuite] Error evaluating daily project for '{town.Name}': {ex.Message}");
            }
        }
    }
}
