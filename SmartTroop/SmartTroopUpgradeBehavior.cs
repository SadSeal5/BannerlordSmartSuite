using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

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
            if (party == MobileParty.MainParty)
            {
                if (Settings.Instance.ModEnabled && Settings.Instance.SmartTroopUpgradeEnabled)
                {
                    TroopUpgradeManager.RunUpgrade();
                }
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
