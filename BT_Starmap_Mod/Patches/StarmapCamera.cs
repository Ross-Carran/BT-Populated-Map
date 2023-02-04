using System.Collections.Generic;
using BattleTech;
using BT_Starmap_Mod;
using HarmonyLib;
using HBS.Logging;

namespace BattleTech_Populated_Map.Patches;

public class StarmapCamera
{
    [HarmonyDebug]
    [HarmonyPatch(typeof(StarmapRenderer), nameof(StarmapRenderer.Update))]
    public class CameraBoundariesPatch
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(StarmapCamera));
        static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count;i++)
            {
                s_log.Log(codes[i].ToString());
            }

            return codes;
        }
    }
}