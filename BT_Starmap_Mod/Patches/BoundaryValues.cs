using System.Collections.Generic;
using BattleTech;
using BT_Starmap_Mod;
using HarmonyLib;
using HBS.Logging;

namespace BattleTech_Populated_Map.Patches;

public class BoundaryValues
{
    [HarmonyDebug]
    [HarmonyPatch(typeof(Starmap), nameof(Starmap.SetBoundaries))]
    public class ChangeBoundaryValues
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(BoundaryValues));
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            foreach (var code in codes)
            {
                
                if (code.ToString().Contains("9999"))
                {
                    s_log.Log(code.operand);
                    code.operand = 200f;
                    s_log.Log((code.operand));
                }
            }
            
            var startIndex = -1;
            var endIndex = -1;
            
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Contains("bgt Label5"))
                {
                    startIndex = i + 1;

                    for (var j = startIndex; j < codes.Count; j++)
                    {
                        if (codes[j].ToString().Contains("ldarg.0 NULL [Label9]"))
                        {
                            endIndex = j;
                        }
                    }
                }
            }
            
            if (startIndex > -1 && endIndex > -1)
            {
                codes.RemoveRange(startIndex,endIndex - startIndex);
            }

            return codes;
        }
    }
}