using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BannerlordSmartSuite
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                new Harmony("com.sadseal5.bannerlord.smartsuite").PatchAll();
                Debug.Print("[BannerlordSmartSuite] Harmony patches initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.Print("[BannerlordSmartSuite] Error initializing Harmony patches: " + ex);
            }
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            if (starterObject is CampaignGameStarter campaignStarter)
            {
                try
                {
                    campaignStarter.AddBehavior(new AutoSettlementBehavior());
                    campaignStarter.AddBehavior(new SmartTroopUpgradeBehavior());
                    campaignStarter.AddBehavior(new AutoDualPerksBehavior());
                    Debug.Print("[BannerlordSmartSuite] Campaign behaviors registered successfully.");
                }
                catch (Exception ex)
                {
                    Debug.Print("[BannerlordSmartSuite] Error registering campaign behaviors: " + ex);
                }
            }
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            try
            {
                mission.AddMissionBehavior(new SmartCaptainMissionBehavior());
            }
            catch (Exception ex)
            {
                Debug.Print("[BannerlordSmartSuite] Error adding SmartCaptainMissionBehavior: " + ex);
            }
        }
    }
}
