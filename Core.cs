using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Il2Cpp;

[assembly: MelonInfo(typeof(SimpleSpawner.Core), "Simple Spawner", "3.5", "Tproplay")]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]


namespace SimpleSpawner
{
    public class Core : MelonMod
    {

        public static PlantType plantTypeselected = PlantType.Nothing;
        public static ZombieType zombieTypeselected = ZombieType.Nothing;

        public override void OnUpdate()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                if (plantTypeselected != PlantType.Nothing)
                {
                    CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn,
                        Mouse.Instance.theMouseRow, plantTypeselected);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.O))
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow,
                         zombieTypeselected,Mouse.Instance.mouseX);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.I))
            {
                if(UnityEngine.Time.timeScale != 0)
                {
                    UnityEngine.Time.timeScale = 0;
                }

                else
                {
                    UnityEngine.Time.timeScale = 1;
                }
            }
        }

       


        [HarmonyPatch(typeof(AlmanacCard), "OnMouseDown")]

        public class AlmanacCard_OnMouseDown_Patch
        {
            public static void Postfix(AlmanacCard __instance)
            {
                plantTypeselected = (PlantType)(__instance.theSeedType);
            }

        }


        [HarmonyPatch(typeof(AlmanacCardZombie), "OnMouseDown")]

        public class AlmanacCardZombie_OnMouseDown_Patch
        {
            public static void Postfix(AlmanacCardZombie __instance)
            {
                zombieTypeselected = __instance.theZombieType;
            }

        }

        [HarmonyPatch(typeof(AlmanacPlantMenu), nameof(AlmanacPlantMenu.SelectCard))]
        public static class AlmanacPlantMenu_SelectCard_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(AlmanacCardUI card)
            {
                if (card == null) return;
                plantTypeselected = card.PlantType;
            }

        }


        [HarmonyPatch(typeof(AlmanacZombieMenu), nameof(AlmanacZombieMenu.SelectCard))]
        public static class AlmanacZombieMenu_SelectCard_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(AlmanacCardUI card)
            {
                if (card == null) return;
                zombieTypeselected = card.ZombieType;
            }

        }
    }
}