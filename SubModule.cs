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
            var harmony = new Harmony("com.sadseal5.bannerlord.smartsuite");

            Type[] patchClasses = new Type[]
            {
                typeof(OrderOfBattlePatch),
                typeof(OrderOfBattleAutoDeployPatch),
                typeof(PerkVMHasAlternativeAndSelectedPatch),
                typeof(HeroDeveloperAddPerkPatch)
            };

            foreach (Type patchClass in patchClasses)
            {
                try
                {
                    harmony.CreateClassProcessor(patchClass).Patch();
                    Debug.Print($"[BannerlordSmartSuite] Harmony patch '{patchClass.Name}' applied successfully.");
                }
                catch (Exception ex)
                {
                    Debug.Print($"[BannerlordSmartSuite] Error applying Harmony patch '{patchClass.Name}': {ex}");
                }
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
