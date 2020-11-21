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
using HBS.Math;

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
        public int left = -99999; //-9999
        public int right = 99999; //9999
        public int top = -99999;  //-9999
        public int bottom = 99999; //9999
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
            codes[17].operand = 99999f; //9999f
            codes[19].operand = -99999f; //-9999f
            codes[21].operand = 99999f;  //9999f
            codes[23].operand = -99999f; //-9999f
                                        //codes.Insert(9, new CodeInstruction(OpCodes.Ldc_R4, 99999));
                                        //*****************************************************************************************
                                        //codes.RemoveRange(56, 28);            //removes map bondaries on init, practically purges the first loop in the method.
                                        //*****************************************************************************************
                                        //sets the mapoffset to 0,0
            /*codes[70].operand = 0;                //98
            codes.RemoveAt(71);                     //99  
            codes[71].operand = 0;                //99
            codes.RemoveAt(72);                  //100*/
            //*****************************************************************************************   
            //codes.RemoveRange(119, 9); //91       //removes normalized in one location breaks the game.  needs to be modified inside method itself

            //output log, this is going to leave debug code on your desktop as Harmony.log

            //codes.RemoveRange(76, 2);   //absolutes only one value 
            //codes.RemoveRange(78, 2);   //absolutes only one value
            //codes.Insert(109, new CodeInstruction(OpCodes.Mul));
            //codes.Insert(109, new CodeInstruction(OpCodes.Ldc_I4, 4));
            //codes.RemoveRange(105, 3);
            //codes.Insert(105, new CodeInstruction(OpCodes.Ldc_I4, 5));


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
    
    [HarmonyPatch(typeof(Starmap),"MapSize", MethodType.Getter)]
    public static class Change_map_Size
    {
        static void Postfix(ref Vector2 __result)
        {
            __result.x *= 1;
            __result.y *= 1;
        }
    }
  

    [HarmonyPatch(typeof(StarSystemNode), "NormalizedPosition", MethodType.Getter)]
    public static class Normalised_Position_Patch
    { 
        static bool Prefix(ref Vector2 __result, Starmap ___Map, StarSystemNode __instance)
        {
           float x = ___Map.MapSize.x;
           float y = ___Map.MapSize.y;

            Vector3 vector = __instance.Position;
            __result = new Vector2(((vector.x * 2) + ___Map.MapOffset.x) / x, ((vector.y * 2) + ___Map.MapOffset.y) / y);

            return false;
            /* Vector2 myVector = __result;
             float x = myVector.x;
             float y = myVector.y;
             x = x * (3814.794f);
             x = x - 1877.428f;
             x = ((x * 2) + 1877.428f) / 3814.794f ;

             y = y * (3916.079f);
             y = y - 1912.177f;
             y = ((y * 2) + 1912.177f) / 3916.079f;

             __result.x = x;
             __result.y = y;*/          
        }
        /*static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
         {
             var codes = new List<CodeInstruction>(instructions);

             //removes the math from both vectors, returning base values. currently broken

             CodeInstruction multiValue;
             multiValue = new CodeInstruction(OpCodes.Ldc_R4, 50);

             codes.Insert(3, multiValue);
             codes.Insert(6, new CodeInstruction(OpCodes.Mul));


             //codes.Insert(20, new CodeInstruction(OpCodes.Mul));
             //codes.Insert(20, new CodeInstruction(OpCodes.Ldc_R4,1));


             //codes.RemoveRange(5, 6);
             //codes[9].operand = -200;                          ///going to add value here
             //codes.RemoveRange(10, 6);            //9,13
             //codes[14].operand = -200;
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
         }*/

        //this code was obtained from the ISM3025 mod, as I had no idea what so ever on where this was done. Slightly modded to fit my messy coding.
        [HarmonyPatch(typeof(StarmapRenderer), "Update")]
        public static class Map_Size_patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                // this simply removes `this.starmapCamera.transform.position = position3;`
                var instructionList = new List<CodeInstruction>(instructions);

                var setTransformPosition = AccessTools.Property(typeof(Transform), nameof(Transform.position)).GetSetMethod();
                var setPositionIndex = instructionList.FindLastIndex(instruction =>
                    instruction.opcode == OpCodes.Callvirt && setTransformPosition.Equals(instruction.operand));

                for (var i = 0; i < 5; i++)
                    instructionList[setPositionIndex - i].opcode = OpCodes.Nop;

                return instructionList;
            }

            static void Postfix(StarmapRenderer __instance)
            {
                __instance.fovMin = 20;
                __instance.fovMax = 125;

                var starmapBorders = __instance.gameObject.GetComponentInChildren<StarmapBorders>();
                if (starmapBorders == null)
                    return;

                // set scale of region borders for our new size
                var borderTransform = starmapBorders.gameObject.transform;
                borderTransform.localScale = new Vector3(4f * 400, 4f * 400);

                // generate black texture to use for plusTex, so that we don't have a
                // stretched texture, it looked weird anyways
                const int textureSize = 64;
                var blackTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
                for (var x = 0; x < textureSize; x++)
                {
                    for (var y = 0; y < textureSize; y++)
                    {
                        blackTexture.SetPixel(x, y, Color.black);
                    }
                }
                blackTexture.Apply();
                starmapBorders.plusTex = blackTexture;

                // change the map border edges
                var edgeLine = GameObject.Find("Edges")?.GetComponent<LineRenderer>();
                if (edgeLine != null)
                {
                    var height = 400;
                    var width = 400;

                    edgeLine.positionCount = 4;
                    edgeLine.SetPositions(new[]
                    {
                    new Vector3(-height, -width),
                    new Vector3(-height, width),
                    new Vector3(height, width),
                    new Vector3(height, -width),
                });
                }

                var cameraPosition = __instance.starmapCamera.transform.position;
                var fov = __instance.starmapCamera.fieldOfView;
                var zPos = __instance.starmapCamera.transform.position.z;


                var clamped = cameraPosition;
                var xView = GetViewSize(Mathf.Abs(zPos), GetHorizontalFov(fov));
                var yView = GetViewSize(Mathf.Abs(zPos), fov);

                var totalWidth = 400 * 2f + 2f * 1;
                var totalHeight = 400 * 2f + 1 + 1;

                var leftBoundary = -400 - 1 + xView;
                var rightBoundary = 400 + 1 - xView;
                var bottomBoundary = -400 - 1 + yView;
                var topBoundary = 400 + 1 - yView;

                clamped.x = xView * 2f >= totalWidth ? 0
                    : Mathf.Clamp(cameraPosition.x, leftBoundary, rightBoundary);

                clamped.y = yView * 2f >= totalHeight ? 0
                    : Mathf.Clamp(cameraPosition.y, bottomBoundary, topBoundary);

                __instance.starmapCamera.transform.position = clamped;

            }
            private static float GetViewSize(float zPos, float fov)
            {
                return zPos * Mathf.Tan(fov * Mathf.Deg2Rad / 2.0f);
            }

            private static float GetHorizontalFov(float verticalFov)
            {
                return 2.0f * Mathf.Atan((16f / 9f) * Mathf.Tan(verticalFov * Mathf.Deg2Rad / 2.0f)) * Mathf.Rad2Deg;
            }
        }
    }
}

