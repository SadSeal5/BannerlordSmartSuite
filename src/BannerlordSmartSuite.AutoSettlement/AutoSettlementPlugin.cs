using TaleWorlds.CampaignSystem;
using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Plugins;

namespace BannerlordSmartSuite.AutoSettlement
{
    public class AutoSettlementPlugin : SmartSuitePluginBase
    {
        public override string Id => "AutoSettlement";
        public override string Name => "Auto Settlement";
        public override bool IsEnabled => Settings.Instance.ModEnabled && Settings.Instance.AutoSettlementBuilderEnabled;

        public override void RegisterCampaignBehaviors(CampaignGameStarter starter)
        {
            if (starter != null)
            {
                starter.AddBehavior(new AutoSettlementBehavior());
            }
        }
    }
}
