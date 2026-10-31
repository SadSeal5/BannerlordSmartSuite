using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Safe;
using BannerlordSmartSuite.Core.Logging;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Library;

namespace BannerlordSmartSuite.AutoSettlement
{
    public class AutoSettlementBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, OnDailyTickSettlement);
            CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, OnBuildingLevelChanged);
        }

        private void OnDailyTickSettlement(Settlement settlement)
        {
            if (settlement == null || !settlement.IsFortification || settlement.Town == null)
                return;

            Clan playerClan = CampaignSafe.PlayerClan;
            if (playerClan == null || settlement.OwnerClan != playerClan)
                return;

            if (!Settings.Instance.ModEnabled || !Settings.Instance.AutoSettlementBuilderEnabled)
                return;

            try
            {
                SettlementNeedEvaluator.EvaluateAndEnqueue(settlement.Town);
                DailyProjectEvaluator.EvaluateDailyProject(settlement.Town);
            }
            catch (Exception ex)
            {
                Debug.Print($"[BannerlordSmartSuite] Error in OnDailyTickSettlement for '{settlement.Name}': {ex.Message}");
            }
        }

        private void OnBuildingLevelChanged(Town town, Building building, int levelChange)
        {
            Clan playerClan = CampaignSafe.PlayerClan;
            if (town == null || playerClan == null || town.OwnerClan != playerClan)
                return;

            if (!Settings.Instance.ModEnabled || !Settings.Instance.AutoSettlementBuilderEnabled)
                return;

            try
            {
                SettlementNeedEvaluator.EvaluateAndEnqueue(town);
            }
            catch (Exception ex)
            {
                Debug.Print($"[BannerlordSmartSuite] Error in OnBuildingLevelChanged for '{town.Name}': {ex.Message}");
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            // Empty sync data ensures complete save game compatibility and vanilla safety.
        }
    }
}
