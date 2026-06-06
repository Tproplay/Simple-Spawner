using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Il2Cpp;


[assembly: MelonInfo(typeof(SimpleSpawner.Core), "Simple Spawner", "3.7", "Tproplay")]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]


namespace SimpleSpawner
{
    public class Core : MelonMod
    {

        public static PlantType plantTypeselected = PlantType.Nothing;
        public static ZombieType zombieTypeselected = ZombieType.Nothing;

        public static bool ZombiespawnedByMod = false;

        // --- MelonPreferences Category and Entries ---
        public static MelonPreferences_Category spawnerCategory;

        // Base actions
        public static MelonPreferences_Entry<KeyCode> keyDeleteAllPlants;
        public static MelonPreferences_Entry<KeyCode> keyDeleteAllZombies;
        public static MelonPreferences_Entry<KeyCode> keyToggleTimeScale;
        public static MelonPreferences_Entry<KeyCode> keySpawnPlant;
        public static MelonPreferences_Entry<KeyCode> keySpawnZombie;

        // Modifiers and Bosses
        public static MelonPreferences_Entry<KeyCode> keyMindControlModifier;
        public static MelonPreferences_Entry<KeyCode> keyBossModifier;
        public static MelonPreferences_Entry<KeyCode> keySpawnBoss1;
        public static MelonPreferences_Entry<KeyCode> keySpawnBoss2;

        // Mini Pets
        public static MelonPreferences_Entry<KeyCode> keyPetGargantuar;
        public static MelonPreferences_Entry<KeyCode> keyPetFootball;
        public static MelonPreferences_Entry<KeyCode> keyPetDrown;
        public static MelonPreferences_Entry<KeyCode> keyPetJackbox;
        public static MelonPreferences_Entry<KeyCode> keyPetSnowBoss;

        // Items (Array for easy looping)
        public static MelonPreferences_Entry<KeyCode>[] keySpawnItems = new MelonPreferences_Entry<KeyCode>[9];

        // Note: OnInitializeMelon is the standard initialization method for modern MelonLoader
        public override void OnInitializeMelon()
        {
            spawnerCategory = MelonPreferences.CreateCategory("Simple Spawner");

            // --- Create Entries with Default Values ---
            keyDeleteAllPlants = spawnerCategory.CreateEntry("Key_DeleteAllPlants", KeyCode.Semicolon, "Delete All Plants");
            keyDeleteAllZombies = spawnerCategory.CreateEntry("Key_DeleteAllZombies", KeyCode.Quote, "Delete All Zombies");
            keyToggleTimeScale = spawnerCategory.CreateEntry("Key_ToggleTimeScale", KeyCode.Backslash, "Toggle Time Scale");
            keySpawnPlant = spawnerCategory.CreateEntry("Key_SpawnPlant", KeyCode.LeftBracket, "Spawn Plant");
            keySpawnZombie = spawnerCategory.CreateEntry("Key_SpawnZombie", KeyCode.RightBracket, "Spawn Zombie");

            keyMindControlModifier = spawnerCategory.CreateEntry("Key_MindControlModifier", KeyCode.RightControl, "Mind Control Modifier");
            keyBossModifier = spawnerCategory.CreateEntry("Key_BossModifier", KeyCode.Slash, "Boss Modifier");
            keySpawnBoss1 = spawnerCategory.CreateEntry("Key_SpawnBoss1", KeyCode.Comma, "Spawn Boss 1");
            keySpawnBoss2 = spawnerCategory.CreateEntry("Key_SpawnBoss2", KeyCode.Period, "Spawn Boss 2");

            keyPetGargantuar = spawnerCategory.CreateEntry("Key_PetGargantuar", KeyCode.Alpha6, "Spawn Pet Gargantuar");
            keyPetFootball = spawnerCategory.CreateEntry("Key_PetFootball", KeyCode.Alpha7, "Spawn Pet Football");
            keyPetDrown = spawnerCategory.CreateEntry("Key_PetDrown", KeyCode.Alpha8, "Spawn Pet Drown");
            keyPetJackbox = spawnerCategory.CreateEntry("Key_PetJackbox", KeyCode.Alpha9, "Spawn Pet Jackbox");
            keyPetSnowBoss = spawnerCategory.CreateEntry("Key_PetSnowBoss", KeyCode.Alpha0, "Spawn Pet Snow Boss");

            // Initialize item spawn keys (Keypad1 through Keypad9)
            for (int i = 0; i < 9; i++)
            {
                keySpawnItems[i] = spawnerCategory.CreateEntry($"Key_SpawnItem_{i + 1}", KeyCode.Keypad1 + i, $"Spawn Item {i + 1}");
            }
        }

        public override void OnUpdate()
        {
            if (Board.Instance == null) return;
            // --- Input handling for spawning plants and zombies ---

            // -- Delete all plants on the lawn ---
            if (Input.GetKeyDown(keyDeleteAllPlants.Value))
            {
                var allPlants = UnityEngine.Object.FindObjectsOfType<Plant>();

                foreach (var plant in allPlants)
                {
                    plant.Die(Plant.DieReason.BySelf);
                }
            }

            // -- Delete all zombies on the lawn ---
            else if (Input.GetKeyDown(keyDeleteAllZombies.Value))
            {
                var allZombies = UnityEngine.Object.FindObjectsOfType<Zombie>();

                foreach (var zombie in allZombies)
                {
                    zombie.theHealth = 0;
                }
            }

            // -- Toggle the game's time scale between paused and normal ---
            else if (Input.GetKeyDown(keyToggleTimeScale.Value))
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

            // -- Spawn a plant at the mouse position if a plant type is selected ---
            else if (Input.GetKeyDown(keySpawnPlant.Value))
            {
                if (plantTypeselected != PlantType.Nothing)
                {
                    CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn,
                        Mouse.Instance.theMouseRow, plantTypeselected);
                }
            }

            // -- Spawn a zombie with mind control at the mouse position if a zombie type is selected ---
            else if (Input.GetKey(keyMindControlModifier.Value) && Input.GetKeyDown(keySpawnZombie.Value))
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow,
                         zombieTypeselected, Mouse.Instance.mouseX);
                }
            }

            // -- Spawn a zombie at the mouse position if a zombie type is selected ---
            else if (!Input.GetKey(keyMindControlModifier.Value) && Input.GetKeyDown(keySpawnZombie.Value))
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow,
                         zombieTypeselected, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
            }

            // -- Spawn a Zombie Boss at the mouse position ---
            else if (Input.GetKey(keyBossModifier.Value))
            {
                if (Input.GetKeyDown(keySpawnBoss1.Value))
                {
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
                else if (Input.GetKeyDown(keySpawnBoss2.Value))
                {
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
            }

            // -- Spawn a Zombie Boss with mind control at the mouse position ---
            else if (Input.GetKey(keyMindControlModifier.Value))
            {
                if (Input.GetKeyDown(keySpawnBoss1.Value))
                {
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                }
                else if (Input.GetKeyDown(keySpawnBoss2.Value))
                {
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow,
                             ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                }
            }

            // -- Mini Pets spawning ---
            else if (Input.GetKeyDown(keyPetGargantuar.Value))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetGargantuar);
            }
            else if (Input.GetKeyDown(keyPetFootball.Value))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetFootball);
            }
            else if (Input.GetKeyDown(keyPetDrown.Value))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetDrown);
            }
            else if (Input.GetKeyDown(keyPetJackbox.Value))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetJackbox);
            }
            else if (Input.GetKeyDown(keyPetSnowBoss.Value))
            {
                MiniPet miniPet = MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetSnowBoss);
            }

            // -- Spawn specific items at the mouse position ---
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(keySpawnItems[i].Value))
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