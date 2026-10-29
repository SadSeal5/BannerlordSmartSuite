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
