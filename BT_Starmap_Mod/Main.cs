using System.Reflection;
using HarmonyLib;
using HBS.Logging;

namespace BT_Starmap_Mod;

public static class Main
{
    private static readonly ILog s_log = Logger.GetLogger(nameof(Main));
    public static void Start()
    {
        s_log.Log("Starting");
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "io.github.ross-carran.BT_Starmap_Mod");
        s_log.Log("Started");
    }
}