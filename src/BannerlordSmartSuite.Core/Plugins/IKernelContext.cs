using HarmonyLib;
using BannerlordSmartSuite.Core.Config;

namespace BannerlordSmartSuite.Core.Plugins
{
    public interface IKernelContext
    {
        Harmony Harmony { get; }
        Settings Settings { get; }
    }
}
