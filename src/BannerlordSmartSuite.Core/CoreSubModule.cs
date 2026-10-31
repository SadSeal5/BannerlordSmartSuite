using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Logging;
using BannerlordSmartSuite.Core.Plugins;

namespace BannerlordSmartSuite.Core
{
    public class CoreSubModule : MBSubModuleBase
    {
        private const string HarmonyId = "com.sadseal5.bannerlord.smartsuite";
        private Harmony _harmony;
        private KernelContext _kernelContext;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                SuiteLogger.Log("Initializing BannerlordSmartSuite Core Kernel v1.3.0...");
                _harmony = new Harmony(HarmonyId);
                _kernelContext = new KernelContext(_harmony, Settings.Instance);

                PluginManager.Instance.DiscoverAndLoadPlugins(_kernelContext);
                PluginManager.Instance.RegisterPatches(_harmony);
                SuiteLogger.Log("BannerlordSmartSuite Core Kernel initialization complete.");
            }
            catch (Exception ex)
            {
                SuiteLogger.Error("Critical error during CoreSubModule.OnSubModuleLoad:", ex);
            }
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            if (starterObject is CampaignGameStarter campaignStarter)
            {
                try
                {
                    PluginManager.Instance.RegisterCampaignBehaviors(campaignStarter);
                }
                catch (Exception ex)
                {
                    SuiteLogger.Error("Error during InitializeGameStarter campaign behavior registration:", ex);
                }
            }
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            try
            {
                PluginManager.Instance.OnMissionBehaviorInitialize(mission);
            }
            catch (Exception ex)
            {
                SuiteLogger.Error("Error during OnMissionBehaviorInitialize:", ex);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            try
            {
                PluginManager.Instance.OnSubModuleUnloaded();
            }
            catch (Exception ex)
            {
                SuiteLogger.Error("Error during CoreSubModule.OnSubModuleUnloaded:", ex);
            }
            base.OnSubModuleUnloaded();
        }
    }
}
