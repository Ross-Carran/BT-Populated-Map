/*   <Changes the paramaters and somevariables in Setboundaries>
    Copyright (C) <year>  <name of author>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using BattleTech;

namespace BTPopulatedMap
{


    public static class main
    {
        public static void Init(string directory, string settingsJSON)
        {
            var harmony = HarmonyInstance.Create("ross.carran.BTPopulatedMap");
            
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

            /*for (int i = 0; i < codes.Count; i++) 
            {
                try
                {
                    FileLog.Log(i + ":  " + codes[i].operand.ToString());
                }
                catch(Exception e)
                {
                    FileLog.Log(e.ToString());
                }
  
            }*/
            return codes;
        }
    }

    [HarmonyPatch(typeof(StarSystemNode), "NormalizedPosition", MethodType.Getter)]

    public static class Normalized_Position_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            //This is a pre-emptive credit to the person on Discord ("name") who my brain is a siv will be added after this is made for giving awesome insite and 
            //the structure for the code that will be put in here. Im clueless with this stuff!

            for (int i = 0; i < codes.Count; i++)
            {
                try
                {
                    FileLog.Log(i + ":  " + codes[i].operand.ToString());
                }
                catch (Exception e)
                {
                    FileLog.Log(e.ToString());
                }

            }
            return codes;
        }

    }

}

