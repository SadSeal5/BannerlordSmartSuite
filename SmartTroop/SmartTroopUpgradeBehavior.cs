using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace BannerlordSmartSuite
{
    public class SmartTroopUpgradeBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, OnHourlyTickParty);
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, OnMissionEnded);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // Empty - Mod state is driven by MCM settings and game state
        }

        private void OnHourlyTickParty(MobileParty party)
        {
            if (party == null || !Settings.Instance.ModEnabled || !Settings.Instance.SmartTroopUpgradeEnabled)
                return;

            if (GameStateManager.Current?.ActiveState is PartyState ||
                GameStateManager.Current?.ActiveState is InventoryState ||
                (ScreenManager.TopScreen != null && ScreenManager.TopScreen.GetType().Name.Contains("Party")))
            {
                return;
            }

            Clan playerClan = CampaignSafe.PlayerClan;
            if (party == MobileParty.MainParty)
            {
                TroopUpgradeManager.RunUpgradeForParty(party);
            }
            else if (Settings.Instance.UpgradeClanParties && playerClan != null && party.ActualClan == playerClan && !party.IsGarrison)
            {
                TroopUpgradeManager.RunUpgradeForParty(party);
            }
            else if (Settings.Instance.UpgradeGarrisons && party.IsGarrison && playerClan != null && (party.ActualClan == playerClan || party.CurrentSettlement?.OwnerClan == playerClan))
            {
                TroopUpgradeManager.RunUpgradeForParty(party);
            }
        }

        private void OnMissionEnded(IMission mission)
        {
            if (Settings.Instance.ModEnabled && Settings.Instance.SmartTroopUpgradeEnabled)
            {
                TroopUpgradeManager.RunUpgrade();
            }
        }

        private void OnGameLoaded(CampaignGameStarter starter)
        {
            if (Settings.Instance.ModEnabled && Settings.Instance.SmartTroopUpgradeEnabled)
            {
                TroopUpgradeManager.RunUpgrade();
            }
        }

        private void OnTick(float dt)
        {
            if (!Settings.Instance.ModEnabled || !Settings.Instance.SmartTroopUpgradeEnabled)
                return;

            if (GameStateManager.Current?.ActiveState is PartyState ||
                GameStateManager.Current?.ActiveState is InventoryState ||
                (ScreenManager.TopScreen != null && ScreenManager.TopScreen.GetType().Name.Contains("Party")))
            {
                return;
            }

            if (Input.IsKeyDown(InputKey.LeftControl) &&
                Input.IsKeyDown(InputKey.LeftShift) &&
                Input.IsKeyPressed(InputKey.U))
            {
                int upgraded = TroopUpgradeManager.RunUpgrade();
                InformationManager.DisplayMessage(new InformationMessage($"[Smart Troop Upgrade] Manual upgrade processed: {upgraded} troop(s) upgraded."));
            }
        }
    }
}
