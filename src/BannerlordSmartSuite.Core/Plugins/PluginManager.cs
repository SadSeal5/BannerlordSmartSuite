using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using BannerlordSmartSuite.Core.Config;
using BannerlordSmartSuite.Core.Logging;

namespace BannerlordSmartSuite.Core.Plugins
{
    public sealed class PluginManager
    {
        private static readonly Lazy<PluginManager> _lazy = new Lazy<PluginManager>(() => new PluginManager());
        public static PluginManager Instance => _lazy.Value;

        private readonly List<ISmartSuitePlugin> _plugins = new List<ISmartSuitePlugin>();
        public IReadOnlyList<ISmartSuitePlugin> Plugins => _plugins.AsReadOnly();

        private IKernelContext _context;

        private PluginManager() { }

        public void DiscoverAndLoadPlugins(IKernelContext context)
        {
            _context = context;
            _plugins.Clear();

            try
            {
                string binDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (string.IsNullOrEmpty(binDir) || !Directory.Exists(binDir))
                {
                    SuiteLogger.Error("[PluginManager] Assembly directory not found.");
                    return;
                }

                SuiteLogger.Log($"[PluginManager] Searching for plugins in {binDir}...");
                string[] dllFiles = Directory.GetFiles(binDir, "BannerlordSmartSuite.*.dll");

                foreach (string dllPath in dllFiles)
                {
                    string fileName = Path.GetFileName(dllPath);
                    if (fileName.Equals("BannerlordSmartSuite.Core.dll", StringComparison.OrdinalIgnoreCase))
                        continue;

                    try
                    {
                        string asmName = Path.GetFileNameWithoutExtension(dllPath);
                        Assembly asm = AppDomain.CurrentDomain.GetAssemblies()
                            .FirstOrDefault(a => string.Equals(a.GetName().Name, asmName, StringComparison.OrdinalIgnoreCase));

                        if (asm == null)
                        {
                            asm = Assembly.LoadFrom(dllPath);
                        }

                        Type[] types;
                        try
                        {
                            types = asm.GetTypes();
                        }
                        catch (ReflectionTypeLoadException rtle)
                        {
                            types = rtle.Types.Where(t => t != null).ToArray();
                            SuiteLogger.Warn($"[PluginManager] Partial type load for {fileName}: {rtle.Message}");
                        }

                        foreach (Type type in types)
                        {
                            if (typeof(ISmartSuitePlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                            {
                                try
                                {
                                    var plugin = (ISmartSuitePlugin)Activator.CreateInstance(type);
                                    plugin.Initialize(context);
                                    _plugins.Add(plugin);
                                    SuiteLogger.Log($"[PluginManager] Successfully loaded plugin '{plugin.Name}' (v{plugin.Version}) from {fileName}.");
                                }
                                catch (Exception ex)
                                {
                                    SuiteLogger.Error($"[PluginManager] Failed to instantiate plugin '{type.FullName}' from {fileName}:", ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SuiteLogger.Error($"[PluginManager] Failed to load assembly from {dllPath}:", ex);
                    }
                }

                SuiteLogger.Log($"[PluginManager] Discovery finished. Loaded {_plugins.Count} plugins.");
            }
            catch (Exception ex)
            {
                SuiteLogger.Error("[PluginManager] Critical exception during plugin discovery:", ex);
            }
        }

        public void RegisterPatches(Harmony harmony)
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    SuiteLogger.Log($"[PluginManager] Registering Harmony patches for '{plugin.Name}'...");
                    plugin.RegisterPatches(harmony);
                }
                catch (Exception ex)
                {
                    SuiteLogger.Error($"[PluginManager] Error registering patches for '{plugin.Name}':", ex);
                }
            }
        }

        public void RegisterCampaignBehaviors(CampaignGameStarter starter)
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    SuiteLogger.Log($"[PluginManager] Registering campaign behaviors for '{plugin.Name}'...");
                    plugin.RegisterCampaignBehaviors(starter);
                }
                catch (Exception ex)
                {
                    SuiteLogger.Error($"[PluginManager] Error registering campaign behaviors for '{plugin.Name}':", ex);
                }
            }
        }

        public void OnMissionBehaviorInitialize(Mission mission)
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    plugin.OnMissionBehaviorInitialize(mission);
                }
                catch (Exception ex)
                {
                    SuiteLogger.Error($"[PluginManager] Error initializing mission behavior for '{plugin.Name}':", ex);
                }
            }
        }

        public void OnSubModuleUnloaded()
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    plugin.OnSubModuleUnloaded();
                }
                catch (Exception ex)
                {
                    SuiteLogger.Error($"[PluginManager] Error during OnSubModuleUnloaded for '{plugin.Name}':", ex);
                }
            }
            _plugins.Clear();
        }
    }

    public class KernelContext : IKernelContext
    {
        public Harmony Harmony { get; }
        public Settings Settings { get; }

        public KernelContext(Harmony harmony, Settings settings)
        {
            Harmony = harmony;
            Settings = settings;
        }
    }
}
