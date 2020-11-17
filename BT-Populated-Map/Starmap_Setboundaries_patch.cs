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
using UnityEngine;

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
        public int left = -1000; //-9999
        public int right = 1000; //9999
        public int top = -1000;  //-9999
        public int bottom = 1000; //9999
    }    

    [HarmonyPatch(typeof(Starmap), "SetBoundaries", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) })]
    public static class Starmap_Setboundaries_patch
    {
        internal static ModSettings Settings = new ModSettings();
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
//****************************************************************************************          
            //changes the values of the methods paramaters
            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 4));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.bottom));
            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 3));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.top));
            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 2));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.right));
            codes.Insert(0, new CodeInstruction(OpCodes.Starg_S, 1));
            codes.Insert(0, new CodeInstruction(OpCodes.Ldc_I4, Settings.left));
//****************************************************************************************
            //changes the values of num,num2,num3,num4
            codes[17].operand = 1000f; //9999f
            codes[19].operand = -1000f; //-9999f
            codes[21].operand = 1000f;  //9999f
            codes[23].operand = -1000f; //9999f
            //codes.Insert(9, new CodeInstruction(OpCodes.Ldc_R4, 99999));
//*****************************************************************************************
            codes.RemoveRange(56, 28);            //removes map bondaries on init, practically purges the first loop in the method.
//*****************************************************************************************
            //sets the mapoffset to 0,0
            codes[70].operand = 0;                //98
            codes.RemoveAt(71);                     //99  
            codes[71].operand = 0;                //99
            codes.RemoveAt(72);                  //100
                                                 //*****************************************************************************************   
            /*next step is going to be removing the absolute value code when setting the map size.
               step after that is to finnaly pacisfy the normaliz code, as there are too many things that call these methods
               I'm making changes to them at the lowest point (I hope) so these changes will always be applied to anything that calls anything to do with the map
               by getting the program to generate the map as a 2d plane as it shouldve been anyway, the normalize code should theoretically not be needed anymore
               but as it stands, i will adjust it so it just passes a unmodified value back.
               There is a possibility Iv'e missed something in my thought process and this will not do what i think it will do.
               If so I've still learned a lot about IL code.
            */
            //codes.RemoveRange(119, 9); //91       //removes normalized in one location breaks the game.  needs to be modified inside method itself

            //output log, this is going to leave debug code on your desktop as Harmony.log

           codes.RemoveRange(76, 2);    //absolutes only one value 
            codes.RemoveRange(78, 2);   //absolutes only one value

            /*so as it stands my math was out -_- i now have to brain, all items on my list have now been modified or can be modified reasonably easily.
             * like most things though the last time i mathed was forever ago so guess im going to have to draw some graphs to visualise this process.
             * not to far to go hopefully.  I also noticed you can disable those backround stars on the starmap wonder if that will help peoples fps if i just purge
             * them from existence.
             * */

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

    [HarmonyPatch(typeof(StarSystemNode), "NormalizedPosition", MethodType.Getter)]
    public static class Normalised_Position_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            //removes the math from both vectors, returning base values.            
            codes.RemoveRange(5, 11);
            codes.RemoveRange(7, 11);

            /*for (int i = 0; i < codes.Count; i++)
            {
                try
                {
                    FileLog.Log(i + ":  " + codes[i].operand.ToString());
                }
                catch (Exception e)
                {
                    FileLog.Log(e.ToString());
                }

            }*/
            return codes;
        }
    }

}

