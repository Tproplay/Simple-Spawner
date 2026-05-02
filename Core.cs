using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Il2Cpp;


[assembly: MelonInfo(typeof(SimpleSpawner.Core), "Simple Spawner", "3.6", "Tproplay")]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]


namespace SimpleSpawner
{
    public class Core : MelonMod
    {

        public static PlantType plantTypeselected = PlantType.Nothing;
        public static ZombieType zombieTypeselected = ZombieType.Nothing;

        public static bool ZombiespawnedByMod = false;
        public override void OnUpdate()
        {

            // --- Input handling for spawning plants and zombies ---

            // -- Press 'Semicolon' to delete all plants on the lawn ---
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                var allPlants = UnityEngine.Object.FindObjectsOfType<Plant>();

                foreach (var plant in allPlants)
                {
                    plant.Die(Plant.DieReason.BySelf);
                }
            }

            // -- Press 'Quote' to delete all zombies on the lawn ---
            else if (Input.GetKeyDown(KeyCode.Quote))
            {
                var allZombies = UnityEngine.Object.FindObjectsOfType<Zombie>();

                foreach (var zombie in allZombies)
                {
                    zombie.theHealth = 0;
                }
            }


            // -- Press 'Backslash' to toggle the game's time scale between paused and normal ---
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Backslash))
            {
                if (UnityEngine.Time.timeScale != 0)
                {
                    UnityEngine.Time.timeScale = 0;
                }

                else
                {
                    UnityEngine.Time.timeScale = 1;
                }
            }

            // -- Press 'Left Bracket' to spawn a plant at the mouse position if a plant type is selected ---
            else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftBracket))
            {
                if (plantTypeselected != PlantType.Nothing)
                {
                    CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn,
                        Mouse.Instance.theMouseRow, plantTypeselected);
                }
            }

            // -- Press 'Left Control + O' to spawn a zombie with mind control at the mouse position if a zombie type is selected ---
            else if (UnityEngine.Input.GetKey(KeyCode.RightControl) && UnityEngine.Input.GetKeyDown(KeyCode.RightBracket))
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow,
                         zombieTypeselected, Mouse.Instance.mouseX);
                }
            }


            // -- Press 'Right Control + Right Bracket' to spawn a zombie at the mouse position if a zombie type is selected ---
            else if (!UnityEngine.Input.GetKey(KeyCode.RightControl) && UnityEngine.Input.GetKeyDown(KeyCode.RightBracket))
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow,
                         zombieTypeselected, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
            }

            // -- Press 'Slash + Comma' or 'Slash + Period' to spawn a Zombie Boss at the mouse position ---

            else if (Input.GetKey(KeyCode.Slash))
            {
                if (Input.GetKeyDown(KeyCode.Comma))
                {
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }

                else if (Input.GetKeyDown(KeyCode.Period))
                {
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
            }

            // -- Press 'Right Control + Comma' or 'Right Control + Period' to spawn a Zombie Boss with mind control at the mouse position ---

            else if (Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.Comma))
                {
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                }
                else if (Input.GetKeyDown(KeyCode.Period))
                {
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                }
            }






            // -- Mini Pets spawning with number keys 7-0 ---

            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetGargantuar);
            }

            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetFootball);
            }

            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetJackbox);
            }

            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetSnowBoss);
            }

            // -- Press number keys 1-9 to spawn specific items at the mouse position ---
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    SpawnItem(i);
                }

            }




        }

        // Method to spawn an item based on the provided index, using the paths defined in the code
        public void SpawnItem(int index)
        {
            string[] paths = new string[] {
                "Items/Fertilize/Ferilize",
                "Items/Bucket",
                "Items/Helmet",
                "Items/Jackbox",
                "Items/Pickaxe",
                "Items/Machine",
                "Items/SuperMachine",
                "Items/PortalHeart",
                "Items/SproutPotPrize/SproutPotPrize",
            };

            if (index >= 0 && index < paths.Length)
            {
                GameObject prefab = Resources.Load<GameObject>(paths[index]);
                GameObject item = UnityEngine.Object.Instantiate(prefab);
                item.transform.position = new Vector3(Mouse.Instance.mouseX, Mouse.Instance.mouseY, 0);
                item.transform.SetParent(GameAPP.board.transform);
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


        [HarmonyPatch(typeof(CreateZombie), nameof(CreateZombie.SetZombie))]

        public static class CreateZombie_SetZombie_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(int theRow, ZombieType theZombieType, float theX)
            {
                if (ZombiespawnedByMod) return true;
                var zombies = UnityEngine.Object.FindObjectsOfType<Zombie>();
                foreach (var zombie in zombies)
                {
                    if (((zombie.theZombieType == ZombieType.ZombieBoss && zombie.isMindControlled) ||
                        (zombie.theZombieType == ZombieType.ZombieBoss2 && zombie.isMindControlled))
                        && theX < 5
                        )
                    {
                        CreateZombie.Instance.SetZombieWithMindControl(
                            theRow, theZombieType, theX);
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ZombieBoss))]
        public static class ZombieBoss_Patch
        {
            [HarmonyPatch(nameof(ZombieBoss.AnimPutBall))]
            [HarmonyPrefix]
            public static bool Prefix_AnimBall(ZombieBoss __instance)
            {
                if (__instance.isMindControlled)
                {
                    return false;
                }
                return true;
            }

            [HarmonyPatch(nameof(ZombieBoss.AnimBungi))]
            [HarmonyPrefix]
            public static bool Prefix_AnimBungi(ZombieBoss __instance)
            {
                if (__instance.isMindControlled)
                {
                    return false;
                }
                return true;
            }
        }

        
    }
}