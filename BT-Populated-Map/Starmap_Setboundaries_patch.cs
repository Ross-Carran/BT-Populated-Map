using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using BattleTech;

namespace BTStarmap
{


    public static class main
    {
        public static void Init(string directory, string settingsJSON)
        {
            var harmony = HarmonyInstance.Create("ross.carran.BTStarmap");
            
            harmony.PatchAll();
        }
    }

    internal class ModSettings
    {
        public int left = -99999;
        public int right = 99999;
        public int top = -99999;
        public int bottom = 99999;
    }    

    [HarmonyPatch(typeof(Starmap), "SetBoundaries", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) })]
    public static class Starmap_Setboundaries_patch
    {
        internal static ModSettings Settings = new ModSettings();
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 4));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.bottom));
            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 3));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.top));
            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 2));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.right));
            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 1));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.left));

            codes[17].operand = 99999f;
            codes[19].operand = -99999f;
            codes[21].operand = 99999f;
            codes[23].operand = -99999f;
            //codes.Insert(9, new CodeInstruction(OpCodes.Ldc_R4, 99999));

            for (int i = 0; i < codes.Count; i++) 
            {
                try
                {
                    FileLog.Log(i + ":  " + codes[i].operand.ToString());
                }
                catch(Exception e)
                {
                    FileLog.Log(e.ToString());
                }
  
            }
            return codes;
        }
    }

}

