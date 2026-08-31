using System.Collections.Generic;
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
        private static bool spawnedByBoss = false;

        private struct ZombieData
        {
            public ZombieType ZombieType;
            public int Column;
            public int Row;

            public ZombieData(ZombieType zombieType, int column, int row)
            {
                ZombieType = zombieType;
                Column = column;
                Row = row;
            }
        }

        private static readonly List<ZombieData> hypnoZombiesData = new List<ZombieData>();

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

            // ========================================================
            // 1. 3-KEY COMBINATIONS
            // ========================================================
            if (Config.KeySpawnHypnoZombiePot.IsDown())
            {
                SpawnHypnoZombiePot();
                return;
            }
            if (Config.KeySpawnHypnoBoss1Pot.IsDown())
            {
                SpawnHypnoBossPot(ZombieType.ZombieBoss);
                return;
            }
            if (Config.KeySpawnHypnoBoss2Pot.IsDown())
            {
                SpawnHypnoBossPot(ZombieType.ZombieBoss2);
                return;
            }

            // ========================================================
            // 2. 2-KEY POT COMBINATIONS
            // ========================================================
            if (Config.KeySpawnPlantPot.IsDown())
            {
                SpawnPlantPot();
                return;
            }
            if (Config.KeySpawnZombiePot.IsDown())
            {
                SpawnZombiePot();
                return;
            }
            if (Config.KeySpawnBoss1Pot.IsDown())
            {
                SpawnBossPot(ZombieType.ZombieBoss);
                return;
            }
            if (Config.KeySpawnBoss2Pot.IsDown())
            {
                SpawnBossPot(ZombieType.ZombieBoss2);
                return;
            }

            // ========================================================
            // 3. 2-KEY DELETION COMBINATIONS
            // ========================================================
            if (Config.KeyDeleteAllPlants.IsDown())
            {
                var allPlants = Object.FindObjectsOfType<Plant>();
                for (int i = 0; i < allPlants.Length; i++) allPlants[i].Die(Plant.DieReason.BySelf);
                return;
            }
            if (Config.KeyDeleteAllZombies.IsDown())
            {
                var allZombies = Object.FindObjectsOfType<Zombie>();
                for (int i = 0; i < allZombies.Length; i++) allZombies[i].theHealth = 0;
                return;
            }

            // ========================================================
            // 4. 2-KEY HYPNOTIZED & BOSS COMBINATIONS
            // ========================================================
            if (Config.KeySpawnHypnoZombie.IsDown())
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow, zombieTypeselected, Mouse.Instance.mouseX);
                }
                return;
            }
            if (Config.KeySpawnBoss1.IsDown())
            {
                ZombiespawnedByMod = true;
                CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                ZombiespawnedByMod = false;
                return;
            }
            if (Config.KeySpawnBoss2.IsDown())
            {
                ZombiespawnedByMod = true;
                CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                ZombiespawnedByMod = false;
                return;
            }
            if (Config.KeySpawnHypnoBoss1.IsDown())
            {
                CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss, Mouse.Instance.mouseX);
                return;
            }
            if (Config.KeySpawnHypnoBoss2.IsDown())
            {
                CreateZombie.Instance.SetZombieWithMindControl(Mouse.Instance.theMouseRow, ZombieType.ZombieBoss2, Mouse.Instance.mouseX);
                return;
            }

            // ========================================================
            // 5. PETS (NATIVE MULTI-KEY CHORDS)
            // ========================================================
            var mousePos = Mouse.Instance.MousePosition;
            var board = Board.Instance;

            if (Config.KeyPetGargantuar.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetGargantuar); return; }
            if (Config.KeyPetFootball.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetFootball); return; }
            if (Config.KeyPetSnowBoss.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetSnowBoss); return; }
            if (Config.KeyPetJackbox.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetJackbox); return; }
            if (Config.KeyPetDrown.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetDrown); return; }
            if (Config.KeyPetHorse.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetHorse); return; }
            if (Config.KeyPetImp.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetImp); return; }
            if (Config.KeyPetKirov.IsDown()) { MiniPet.SetPet(board, mousePos, PetType.PetKirov); return; }

            // ========================================================
            // 6. 1-KEY BASE SPAWNS & CONTROLS
            // ========================================================
            if (Config.KeySpawnPlant.IsDown())
            {
                if (plantTypeselected != PlantType.Nothing)
                {
                    CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow, plantTypeselected);
                }
                return;
            }
            if (Config.KeySpawnZombie.IsDown())
            {
                if (zombieTypeselected != ZombieType.Nothing)
                {
                    ZombiespawnedByMod = true;
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, zombieTypeselected, Mouse.Instance.mouseX);
                    ZombiespawnedByMod = false;
                }
                return;
            }
            if (Config.KeyToggleTimeScale.IsDown())
            {
                Time.timeScale = Time.timeScale != 0 ? 0 : 1;
                return;
            }

            // ========================================================
            // 7. ITEMS
            // ========================================================
            if (Config.KeySpawnFertilizer.IsDown()) { SpawnItem("Items/Fertilize/Ferilize"); return; }
            if (Config.KeySpawnBucket.IsDown()) { SpawnItem("Items/Bucket"); return; }
            if (Config.KeySpawnHelmet.IsDown()) { SpawnItem("Items/Helmet"); return; }
            if (Config.KeySpawnJackbox.IsDown()) { SpawnItem("Items/Jackbox"); return; }
            if (Config.KeySpawnPickaxe.IsDown()) { SpawnItem("Items/Pickaxe"); return; }
            if (Config.KeySpawnMachine.IsDown()) { SpawnItem("Items/Machine"); return; }
            if (Config.KeySpawnSuperMachine.IsDown()) { SpawnItem("Items/SuperMachine"); return; }
            if (Config.KeySpawnPortalHeart.IsDown()) { SpawnItem("Items/PortalHeart"); return; }
            if (Config.KeySpawnSproutPotPrize.IsDown()) { SpawnItem("Items/SproutPotPrize/SproutPotPrize"); return; }
        }

        // ==========================================
        // POT SPAWNING
        // ==========================================
        private static void SpawnPlantPot()
        {
            if (plantTypeselected != PlantType.Nothing)
            {
                ScaryPotManager.CreateScaryPot(plantTypeselected, Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow);
            }
        }

        private static void SpawnZombiePot()
        {
            if (zombieTypeselected != ZombieType.Nothing)
            {
                ScaryPotManager.CreateScaryPot(zombieTypeselected, Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow);
            }
        }

        private static void SpawnHypnoZombiePot()
        {
            if (zombieTypeselected != ZombieType.Nothing)
            {
                int col = Mouse.Instance.theMouseColumn;
                int row = Mouse.Instance.theMouseRow;
                ScaryPotManager.CreateScaryPot(zombieTypeselected, col, row);
                hypnoZombiesData.Add(new ZombieData(zombieTypeselected, col, row));
            }
        }

        private static void SpawnBossPot(ZombieType bossType)
        {
            ScaryPotManager.CreateScaryPot(bossType, Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow);
        }

        private static void SpawnHypnoBossPot(ZombieType bossType)
        {
            int col = Mouse.Instance.theMouseColumn;
            int row = Mouse.Instance.theMouseRow;
            ScaryPotManager.CreateScaryPot(bossType, col, row);
            hypnoZombiesData.Add(new ZombieData(bossType, col, row));
        }

        public static void SpawnItem(string resourcePath)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab != null)
            {
                GameObject item = Object.Instantiate(prefab);
                item.transform.position = new Vector3(Mouse.Instance.mouseX, Mouse.Instance.mouseY, 0);
                item.transform.SetParent(GameAPP.board.transform);
            }
            else if (Core.MagnetarLoaded)
            {
                Magnetar_Client.Utils.Magnetar_Logger.DebugLogger.Msg($"[Simple Spawner] No prefab at path: {resourcePath}");
            }
        }

        // ==========================================
        // HARMONY PATCHES
        // ==========================================
        [HarmonyPatch(typeof(AlmanacPlantMenu), nameof(AlmanacPlantMenu.SelectCard))]
        public static class AlmanacPlantMenu_SelectCard_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(AlmanacCardUI card)
            {
                if (card != null)
                {
                    plantTypeselected = card.PlantType;
                    if (Core.MagnetarLoaded)
                    {
                        MagnetarIntegration.SyncPlantSelection(card.PlantType);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AlmanacZombieMenu), nameof(AlmanacZombieMenu.SelectCard))]
        public static class AlmanacZombieMenu_SelectCard_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(AlmanacCardUI card)
            {
                if (card != null)
                {
                    zombieTypeselected = card.ZombieType;
                    if (Core.MagnetarLoaded)
                    {
                        MagnetarIntegration.SyncZombieSelection(card.ZombieType);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CreateZombie), nameof(CreateZombie.SetZombie))]
        public static class CreateZombie_SetZombie_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(int theRow, ZombieType theZombieType, float theX)
            {
                if (ZombiespawnedByMod) return true;

                int col = Lawnf.GetColumnFromX(theX);

                for (int i = hypnoZombiesData.Count - 1; i >= 0; i--)
                {
                    var data = hypnoZombiesData[i];
                    if (data.ZombieType == theZombieType && data.Row == theRow && data.Column == col)
                    {
                        hypnoZombiesData.RemoveAt(i);
                        CreateZombie.Instance.SetZombieWithMindControl(theRow, theZombieType, theX);
                        return false;
                    }
                }

                if (spawnedByBoss && theX < 5f)
                {
                    var zombies = Object.FindObjectsOfType<Zombie>();
                    for (int i = 0; i < zombies.Length; i++)
                    {
                        var z = zombies[i];
                        if ((z.theZombieType == ZombieType.ZombieBoss || z.theZombieType == ZombieType.ZombieBoss2) && z.isMindControlled)
                        {
                            CreateZombie.Instance.SetZombieWithMindControl(theRow, theZombieType, theX);
                            return false;
                        }
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
            public static bool Prefix_AnimBall(ZombieBoss __instance) => !__instance.isMindControlled;

            [HarmonyPatch(nameof(ZombieBoss.AnimBungi))]
            [HarmonyPrefix]
            public static bool Prefix_AnimBungi(ZombieBoss __instance) => !__instance.isMindControlled;

            [HarmonyPatch(nameof(ZombieBoss.AnimSpawn))]
            [HarmonyPrefix]
            public static void Prefix_AnimSpawn() => spawnedByBoss = true;

            [HarmonyPatch(nameof(ZombieBoss.AnimSpawn))]
            [HarmonyPostfix]
            public static void Postfix_AnimSpawn() => spawnedByBoss = false;
        }
    }
}