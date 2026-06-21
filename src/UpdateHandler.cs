using UnityEngine;
using HarmonyLib;
#if MELONLOADER
using Il2Cpp;
#endif

namespace SimpleSpawner
{
    public static class UpdateHandler
    {
        public static PlantType plantTypeselected = PlantType.Nothing;
        public static ZombieType zombieTypeselected = ZombieType.Nothing;
        public static bool ZombiespawnedByMod = false;

#if BEPINEX
        public static void ApplyPatches()
        {
            var harmony = new Harmony("com.Tproplay.SimpleSpawner");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }
#endif

        public static void Update()
        {
            if (!Config.CheckActive() || Board.Instance == null) return;

            bool triggered = false;

            // -- Spawn Zombie with Mind Control ---
            if (!triggered && Config.CheckKey(Config.keyMindControlModifier.Value, "MindControlModifier") && Config.CheckKeyDown(Config.keySpawnZombie.Value, "SpawnZombie"))
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    triggered = true;
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow, zombieTypeselected, Mouse.Instance.mouseX);
                }
            }

            // -- Spawn Zombie Normal ---
            if (!triggered && !Config.CheckKey(Config.keyMindControlModifier.Value, "MindControlModifier") && Config.CheckKeyDown(Config.keySpawnZombie.Value, "SpawnZombie"))
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    triggered = true;
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, zombieTypeselected, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
            }

            // -- Spawn Bosses (Modifier) ---
            if (!triggered && Config.CheckKey(Config.keyBossModifier.Value, "BossModifier"))
            {
                if (Config.CheckKeyDown(Config.keySpawnBoss1.Value, "SpawnBoss1"))
                {
                    triggered = true;
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
                else if (Config.CheckKeyDown(Config.keySpawnBoss2.Value, "SpawnBoss2"))
                {
                    triggered = true;
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
            }

            // -- Spawn Bosses (Mind Control) ---
            if (!triggered && Config.CheckKey(Config.keyMindControlModifier.Value, "MindControlModifier"))
            {
                if (Config.CheckKeyDown(Config.keySpawnBoss1.Value, "SpawnBoss1"))
                {
                    triggered = true;
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                }
                else if (Config.CheckKeyDown(Config.keySpawnBoss2.Value, "SpawnBoss2"))
                {
                    triggered = true;
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                }
            }

            // -- Spawn Pets ---
            if (!triggered && Config.CheckKey(Config.keyPetPre.Value, "PetPre", true))
            {
                if (Config.CheckKeyDown(Config.keyPetGargantuar.Value, "PetGargantuar")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetGargantuar); triggered = true; }
                else if (Config.CheckKeyDown(Config.keyPetFootball.Value, "PetFootball")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetFootball); triggered = true; }
                else if (Config.CheckKeyDown(Config.keyPetSnowBoss.Value, "PetSnowBoss")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetSnowBoss); triggered = true; }
                else if (Config.CheckKeyDown(Config.keyPetJackbox.Value, "PetJackbox")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetJackbox); triggered = true; }
                else if (Config.CheckKeyDown(Config.keyPetDrown.Value, "PetDrown")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetDrown); triggered = true; }
                else if (Config.CheckKeyDown(Config.keyPetHorse.Value, "PetHorse")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetHorse); triggered = true; }
                else if (Config.CheckKeyDown(Config.keyPetImp.Value, "PetImp")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetImp); triggered = true; }
                else if (Config.CheckKeyDown(Config.keyPetKirov.Value, "PetKirov")) { MiniPet.SetPet(Board.Instance, Mouse.Instance.MousePosition, PetType.PetKirov); triggered = true; }
            }

            // -- Spawn Items ---
            if (!triggered && Config.CheckKeyDown(Config.keySpawnFertilizer.Value, "SpawnFertilizer")) { SpawnItem("Items/Fertilize/Ferilize"); triggered = true; }
            if (!triggered && Config.CheckKeyDown(Config.keySpawnBucket.Value, "SpawnBucket")) { SpawnItem("Items/Bucket"); triggered = true; }
            if (!triggered && Config.CheckKeyDown(Config.keySpawnHelmet.Value, "SpawnHelmet")) { SpawnItem("Items/Helmet"); triggered = true; }
            if (!triggered && Config.CheckKeyDown(Config.keySpawnJackbox.Value, "SpawnJackbox")) { SpawnItem("Items/Jackbox"); triggered = true; }
            if (!triggered && Config.CheckKeyDown(Config.keySpawnPickaxe.Value, "SpawnPickaxe")) { SpawnItem("Items/Pickaxe"); triggered = true; }
            if (!triggered && Config.CheckKeyDown(Config.keySpawnMachine.Value, "SpawnMachine")) SpawnItem("Items/Machine"); // Keep unassigned if intended or add triggered = true;
            if (!triggered && Config.CheckKeyDown(Config.keySpawnSuperMachine.Value, "SpawnSuperMachine")) { SpawnItem("Items/SuperMachine"); triggered = true; }
            if (!triggered && Config.CheckKeyDown(Config.keySpawnPortalHeart.Value, "SpawnPortalHeart")) { SpawnItem("Items/PortalHeart"); triggered = true; }
            if (!triggered && Config.CheckKeyDown(Config.keySpawnSproutPotPrize.Value, "SpawnSproutPotPrize")) { SpawnItem("Items/SproutPotPrize/SproutPotPrize"); triggered = true; }

            // -- Delete all plants ---
            if (!triggered && Config.CheckKeyDown(Config.keyDeleteAllPlants.Value, "DeleteAllPlants"))
            {
                triggered = true;
                var allPlants = UnityEngine.Object.FindObjectsOfType<Plant>();
                foreach (var plant in allPlants) plant.Die(Plant.DieReason.BySelf);
            }

            // -- Delete all zombies ---
            if (!triggered && Config.CheckKeyDown(Config.keyDeleteAllZombies.Value, "DeleteAllZombies"))
            {
                triggered = true;
                var allZombies = UnityEngine.Object.FindObjectsOfType<Zombie>();
                foreach (var zombie in allZombies) zombie.theHealth = 0;
            }

            // -- Toggle Time Scale ---
            if (!triggered && Config.CheckKeyDown(Config.keyToggleTimeScale.Value, "ToggleTimeScale"))
            {
                triggered = true;
                UnityEngine.Time.timeScale = UnityEngine.Time.timeScale != 0 ? 0 : 1;
            }

            // -- Spawn Plant ---
            if (!triggered && Config.CheckKeyDown(Config.keySpawnPlant.Value, "SpawnPlant"))
            {
                if (plantTypeselected != PlantType.Nothing)
                {
                    triggered = true;
                    CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow, plantTypeselected);
                }
            }
        }

        public static void SpawnItem(string resourcePath)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab != null)
            {
                GameObject item = UnityEngine.Object.Instantiate(prefab);
                item.transform.position = new Vector3(Mouse.Instance.mouseX, Mouse.Instance.mouseY, 0);
                item.transform.SetParent(GameAPP.board.transform);
            }
        }

        // --- Harmony Patches ---
        [HarmonyPatch(typeof(AlmanacPlantMenu), nameof(AlmanacPlantMenu.SelectCard))]
        public static class AlmanacPlantMenu_SelectCard_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(AlmanacCardUI card)
            {
                if (card != null) plantTypeselected = card.PlantType;
            }
        }

        [HarmonyPatch(typeof(AlmanacZombieMenu), nameof(AlmanacZombieMenu.SelectCard))]
        public static class AlmanacZombieMenu_SelectCard_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(AlmanacCardUI card)
            {
                if (card != null) zombieTypeselected = card.ZombieType;
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
                        (zombie.theZombieType == ZombieType.ZombieBoss2 && zombie.isMindControlled)) && theX < 5)
                    {
                        CreateZombie.Instance.SetZombieWithMindControl(theRow, theZombieType, theX);
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
                return !__instance.isMindControlled;
            }

            [HarmonyPatch(nameof(ZombieBoss.AnimBungi))]
            [HarmonyPrefix]
            public static bool Prefix_AnimBungi(ZombieBoss __instance)
            {
                return !__instance.isMindControlled;
            }
        }
    }
}