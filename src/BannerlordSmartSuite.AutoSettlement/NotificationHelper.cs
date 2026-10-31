using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Safe;
using BannerlordSmartSuite.Core.Logging;
using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Library;

namespace BannerlordSmartSuite.AutoSettlement
{
    public static class NotificationHelper
    {
        private static readonly Color MessageColor = Color.FromUint(0xFFE5A93C); // Amber / Gold

        public static void NotifyAutoQueued(Town town, Building building)
        {
            if (town == null || building == null) return;

            string townName = town.Name?.ToString() ?? "Settlement";
            string buildingName = building.Name?.ToString() ?? "Building";
            int nextTier = building.CurrentLevel + 1;

            string text = $"[Smart Suite] Auto-queued '{buildingName} (Tier {nextTier})' in {townName} based on settlement needs.";
            DisplayMessage(text);
        }

        public static void NotifyDailyProjectChanged(Town town, Building dailyBuilding)
        {
            if (town == null || dailyBuilding == null) return;

            string townName = town.Name?.ToString() ?? "Settlement";
            string dailyName = dailyBuilding.Name?.ToString() ?? "Daily Project";

            string text = $"[Smart Suite] Set daily project to '{dailyName}' in {townName} based on settlement needs.";
            DisplayMessage(text);
        }

        private static void DisplayMessage(string text)
        {
            try
            {
                InformationManager.DisplayMessage(new InformationMessage(text, MessageColor));
            }
            catch (Exception ex)
            {
                Debug.Print($"[BannerlordSmartSuite] Could not display message: {ex.Message}");
            }
        }
    }
}
