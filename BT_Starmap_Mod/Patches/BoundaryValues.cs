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
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            int[] fieldLoc = new int[4];
            int fieldLocCounter = 0;
            
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Contains("9999"))
                {
                    fieldLoc[fieldLocCounter] = i;
                    fieldLocCounter++;
                    
                    s_log.Log(codes[i].operand);
                    codes[i].operand = 200f;
                    s_log.Log((codes[i].operand));
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
            
            codes[fieldLoc[0]].operand = Settings.num1;
            codes[fieldLoc[1]].operand = Settings.num2;
            codes[fieldLoc[2]].operand = Settings.num3;
            codes[fieldLoc[3]].operand = Settings.num4;

            return codes;

        }
    }
}