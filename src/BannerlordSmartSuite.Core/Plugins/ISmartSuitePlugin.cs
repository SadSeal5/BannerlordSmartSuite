using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace BannerlordSmartSuite.Core.Plugins
{
    public interface ISmartSuitePlugin
    {
        string Id { get; }
        string Name { get; }
        string Version { get; }
        bool IsEnabled { get; }

        void Initialize(IKernelContext context);
        void RegisterPatches(Harmony harmony);
        void RegisterCampaignBehaviors(CampaignGameStarter starter);
        void OnMissionBehaviorInitialize(Mission mission);
        void OnSubModuleUnloaded();
    }
}
