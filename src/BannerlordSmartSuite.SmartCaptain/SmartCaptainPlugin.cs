using HarmonyLib;
using TaleWorlds.MountAndBlade;
using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Plugins;

namespace BannerlordSmartSuite.SmartCaptain
{
    public class SmartCaptainPlugin : SmartSuitePluginBase
    {
        public override string Id => "SmartCaptain";
        public override string Name => "Smart Captain";
        public override bool IsEnabled => Settings.Instance.ModEnabled && Settings.Instance.SmartCaptainEnabled;

        public override void RegisterPatches(Harmony harmony)
        {
            PatchClasses(harmony, typeof(OrderOfBattlePatch), typeof(OrderOfBattleAutoDeployPatch));
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            if (IsEnabled)
            {
                mission.AddMissionBehavior(new SmartCaptainMissionBehavior());
            }
        }
    }
}
