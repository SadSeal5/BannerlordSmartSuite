using TaleWorlds.CampaignSystem;
using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Plugins;

namespace BannerlordSmartSuite.SmartTroop
{
    public class SmartTroopPlugin : SmartSuitePluginBase
    {
        public override string Id => "SmartTroop";
        public override string Name => "Smart Troop Upgrade";
        public override bool IsEnabled => Settings.Instance.ModEnabled && Settings.Instance.SmartTroopUpgradeEnabled;

        public override void RegisterCampaignBehaviors(CampaignGameStarter starter)
        {
            if (starter != null)
            {
                starter.AddBehavior(new SmartTroopUpgradeBehavior());
            }
        }
    }
}
