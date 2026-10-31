using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Logging;

namespace BannerlordSmartSuite.Core.Plugins
{
    public abstract class SmartSuitePluginBase : ISmartSuitePlugin
    {
        public abstract string Id { get; }
        public abstract string Name { get; }
        public virtual string Version => "1.3.0";
        public virtual bool IsEnabled => Settings.Instance.ModEnabled;

        protected IKernelContext Context { get; private set; }
        protected Harmony Harmony => Context?.Harmony;
        protected Settings Settings => Context?.Settings ?? Settings.Instance;

        public virtual void Initialize(IKernelContext context)
        {
            Context = context;
        }

        public virtual void RegisterPatches(Harmony harmony)
        {
        }

        public virtual void RegisterCampaignBehaviors(CampaignGameStarter starter)
        {
        }

        public virtual void OnMissionBehaviorInitialize(Mission mission)
        {
        }

        public virtual void OnSubModuleUnloaded()
        {
        }

        protected void PatchClasses(Harmony harmony, params Type[] patchTypes)
        {
            if (harmony == null || patchTypes == null) return;
            foreach (var patchType in patchTypes)
            {
                try
                {
                    harmony.CreateClassProcessor(patchType).Patch();
                    SuiteLogger.Log($"[{Name}] Harmony patch '{patchType.Name}' applied successfully.");
                }
                catch (Exception ex)
                {
                    SuiteLogger.Error($"[{Name}] Error applying Harmony patch '{patchType.Name}':", ex);
                }
            }
        }
    }
}
