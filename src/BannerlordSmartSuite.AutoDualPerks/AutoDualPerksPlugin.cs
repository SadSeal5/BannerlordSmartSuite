using HarmonyLib;
using TaleWorlds.CampaignSystem;
using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Plugins;

namespace BannerlordSmartSuite.AutoDualPerks
{
    public class AutoDualPerksPlugin : SmartSuitePluginBase
    {
        public override string Id => "AutoDualPerks";
        public override string Name => "Auto Dual Perks";
        public override bool IsEnabled => Settings.Instance.ModEnabled && Settings.Instance.AutoDualPerksEnabled;

        public override void RegisterPatches(Harmony harmony)
        {
            PatchClasses(harmony, typeof(PerkVMHasAlternativeAndSelectedPatch), typeof(HeroDeveloperAddPerkPatch));
        }

        public override void RegisterCampaignBehaviors(CampaignGameStarter starter)
        {
            if (starter != null)
            {
                starter.AddBehavior(new AutoDualPerksBehavior());
            }
        }
    }
}
