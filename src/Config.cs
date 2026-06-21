#if MELONLOADER
using MelonLoader;
#elif BEPINEX
using BepInEx.Configuration;
#endif
using UnityEngine;

namespace SimpleSpawner
{
    public static class Config
    {
#if MELONLOADER
        public static MelonPreferences_Category spawnerCategory;
        public static MelonPreferences_Entry<KeyCode> keyDeleteAllPlants;
        public static MelonPreferences_Entry<KeyCode> keyDeleteAllZombies;
        public static MelonPreferences_Entry<KeyCode> keyToggleTimeScale;
        public static MelonPreferences_Entry<KeyCode> keySpawnPlant;
        public static MelonPreferences_Entry<KeyCode> keySpawnZombie;
        public static MelonPreferences_Entry<KeyCode> keyMindControlModifier;
        public static MelonPreferences_Entry<KeyCode> keyBossModifier;
        public static MelonPreferences_Entry<KeyCode> keySpawnBoss1;
        public static MelonPreferences_Entry<KeyCode> keySpawnBoss2;

        public static MelonPreferences_Entry<KeyCode> keyPetPre;
        public static MelonPreferences_Entry<KeyCode> keyPetGargantuar;
        public static MelonPreferences_Entry<KeyCode> keyPetFootball;
        public static MelonPreferences_Entry<KeyCode> keyPetDrown;
        public static MelonPreferences_Entry<KeyCode> keyPetJackbox;
        public static MelonPreferences_Entry<KeyCode> keyPetSnowBoss;
        public static MelonPreferences_Entry<KeyCode> keyPetHorse;
        public static MelonPreferences_Entry<KeyCode> keyPetImp;
        public static MelonPreferences_Entry<KeyCode> keyPetKirov;

        public static MelonPreferences_Entry<KeyCode> keySpawnFertilizer;
        public static MelonPreferences_Entry<KeyCode> keySpawnBucket;
        public static MelonPreferences_Entry<KeyCode> keySpawnHelmet;
        public static MelonPreferences_Entry<KeyCode> keySpawnJackbox;
        public static MelonPreferences_Entry<KeyCode> keySpawnPickaxe;
        public static MelonPreferences_Entry<KeyCode> keySpawnMachine;
        public static MelonPreferences_Entry<KeyCode> keySpawnSuperMachine;
        public static MelonPreferences_Entry<KeyCode> keySpawnPortalHeart;
        public static MelonPreferences_Entry<KeyCode> keySpawnSproutPotPrize;
#elif BEPINEX
        public static ConfigEntry<KeyCode> keyDeleteAllPlants;
        public static ConfigEntry<KeyCode> keyDeleteAllZombies;
        public static ConfigEntry<KeyCode> keyToggleTimeScale;
        public static ConfigEntry<KeyCode> keySpawnPlant;
        public static ConfigEntry<KeyCode> keySpawnZombie;
        public static ConfigEntry<KeyCode> keyMindControlModifier;
        public static ConfigEntry<KeyCode> keyBossModifier;
        public static ConfigEntry<KeyCode> keySpawnBoss1;
        public static ConfigEntry<KeyCode> keySpawnBoss2;

        public static ConfigEntry<KeyCode> keyPetPre;
        public static ConfigEntry<KeyCode> keyPetGargantuar;
        public static ConfigEntry<KeyCode> keyPetFootball;
        public static ConfigEntry<KeyCode> keyPetDrown;
        public static ConfigEntry<KeyCode> keyPetJackbox;
        public static ConfigEntry<KeyCode> keyPetSnowBoss;
        public static ConfigEntry<KeyCode> keyPetHorse;
        public static ConfigEntry<KeyCode> keyPetImp;
        public static ConfigEntry<KeyCode> keyPetKirov;

        public static ConfigEntry<KeyCode> keySpawnFertilizer;
        public static ConfigEntry<KeyCode> keySpawnBucket;
        public static ConfigEntry<KeyCode> keySpawnHelmet;
        public static ConfigEntry<KeyCode> keySpawnJackbox;
        public static ConfigEntry<KeyCode> keySpawnPickaxe;
        public static ConfigEntry<KeyCode> keySpawnMachine;
        public static ConfigEntry<KeyCode> keySpawnSuperMachine;
        public static ConfigEntry<KeyCode> keySpawnPortalHeart;
        public static ConfigEntry<KeyCode> keySpawnSproutPotPrize;
#endif

        public static void Initialize()
        {
#if MELONLOADER
            spawnerCategory = MelonPreferences.CreateCategory("Simple Spawner");

            keyDeleteAllPlants = spawnerCategory.CreateEntry("Key_DeleteAllPlants", KeyCode.Semicolon, "Delete All Plants");
            keyDeleteAllZombies = spawnerCategory.CreateEntry("Key_DeleteAllZombies", KeyCode.Quote, "Delete All Zombies");
            keyToggleTimeScale = spawnerCategory.CreateEntry("Key_ToggleTimeScale", KeyCode.Backslash, "Toggle Time Scale");
            keySpawnPlant = spawnerCategory.CreateEntry("Key_SpawnPlant", KeyCode.LeftBracket, "Spawn Plant");
            keySpawnZombie = spawnerCategory.CreateEntry("Key_SpawnZombie", KeyCode.RightBracket, "Spawn Zombie");
            keyMindControlModifier = spawnerCategory.CreateEntry("Key_MindControlModifier", KeyCode.RightControl, "Mind Control Modifier");
            keyBossModifier = spawnerCategory.CreateEntry("Key_BossModifier", KeyCode.Slash, "Boss Modifier");
            keySpawnBoss1 = spawnerCategory.CreateEntry("Key_SpawnBoss1", KeyCode.Comma, "Spawn Boss 1");
            keySpawnBoss2 = spawnerCategory.CreateEntry("Key_SpawnBoss2", KeyCode.Period, "Spawn Boss 2");

            keyPetPre = spawnerCategory.CreateEntry("Key_PetPre", KeyCode.RightControl, "Pet Spawn Prefix");
            keyPetGargantuar = spawnerCategory.CreateEntry("Key_PetGargantuar", KeyCode.Keypad1, "Spawn Pet Gargantuar");
            keyPetFootball = spawnerCategory.CreateEntry("Key_PetFootball", KeyCode.Keypad2, "Spawn Pet Football");
            keyPetDrown = spawnerCategory.CreateEntry("Key_PetDrown", KeyCode.Keypad3, "Spawn Pet Drown");
            keyPetJackbox = spawnerCategory.CreateEntry("Key_PetJackbox", KeyCode.Keypad4, "Spawn Pet Jackbox");
            keyPetSnowBoss = spawnerCategory.CreateEntry("Key_PetSnowBoss", KeyCode.Keypad5, "Spawn Pet Snow Boss");
            keyPetHorse = spawnerCategory.CreateEntry("Key_PetHorse", KeyCode.Keypad6, "Spawn Pet Horse");
            keyPetImp = spawnerCategory.CreateEntry("Key_PetImp", KeyCode.Keypad7, "Spawn Pet Imp");
            keyPetKirov = spawnerCategory.CreateEntry("Key_PetKirov", KeyCode.Keypad8, "Spawn Pet Kirov");

            keySpawnFertilizer = spawnerCategory.CreateEntry("Key_SpawnFertilizer", KeyCode.Keypad1, "Spawn Fertilizer");
            keySpawnBucket = spawnerCategory.CreateEntry("Key_SpawnBucket", KeyCode.Keypad2, "Spawn Bucket");
            keySpawnHelmet = spawnerCategory.CreateEntry("Key_SpawnHelmet", KeyCode.Keypad3, "Spawn Helmet");
            keySpawnJackbox = spawnerCategory.CreateEntry("Key_SpawnJackbox", KeyCode.Keypad4, "Spawn Jackbox");
            keySpawnPickaxe = spawnerCategory.CreateEntry("Key_SpawnPickaxe", KeyCode.Keypad5, "Spawn Pickaxe");
            keySpawnMachine = spawnerCategory.CreateEntry("Key_SpawnMachine", KeyCode.Keypad6, "Spawn Machine");
            keySpawnSuperMachine = spawnerCategory.CreateEntry("Key_SpawnSuperMachine", KeyCode.Keypad7, "Spawn Super Machine");
            keySpawnPortalHeart = spawnerCategory.CreateEntry("Key_SpawnPortalHeart", KeyCode.Keypad8, "Spawn Portal Heart");
            keySpawnSproutPotPrize = spawnerCategory.CreateEntry("Key_SpawnSproutPotPrize", KeyCode.Keypad9, "Spawn Sprout Pot Prize");
#elif BEPINEX
            var config = Core.Instance.Config;
            keyDeleteAllPlants = config.Bind("Simple Spawner", "Key_DeleteAllPlants", KeyCode.Semicolon, "Delete All Plants");
            keyDeleteAllZombies = config.Bind("Simple Spawner", "Key_DeleteAllZombies", KeyCode.Quote, "Delete All Zombies");
            keyToggleTimeScale = config.Bind("Simple Spawner", "Key_ToggleTimeScale", KeyCode.Backslash, "Toggle Time Scale");
            keySpawnPlant = config.Bind("Simple Spawner", "Key_SpawnPlant", KeyCode.LeftBracket, "Spawn Plant");
            keySpawnZombie = config.Bind("Simple Spawner", "Key_SpawnZombie", KeyCode.RightBracket, "Spawn Zombie");
            keyMindControlModifier = config.Bind("Simple Spawner", "Key_MindControlModifier", KeyCode.RightControl, "Mind Control Modifier");
            keyBossModifier = config.Bind("Simple Spawner", "Key_BossModifier", KeyCode.Slash, "Boss Modifier");
            keySpawnBoss1 = config.Bind("Simple Spawner", "Key_SpawnBoss1", KeyCode.Comma, "Spawn Boss 1");
            keySpawnBoss2 = config.Bind("Simple Spawner", "Key_SpawnBoss2", KeyCode.Period, "Spawn Boss 2");

            keyPetPre = config.Bind("Simple Spawner", "Key_PetPre", KeyCode.RightControl, "Pet Spawn Prefix");
            keyPetGargantuar = config.Bind("Simple Spawner", "Key_PetGargantuar", KeyCode.Alpha6, "Spawn Pet Gargantuar");
            keyPetFootball = config.Bind("Simple Spawner", "Key_PetFootball", KeyCode.Alpha7, "Spawn Pet Football");
            keyPetDrown = config.Bind("Simple Spawner", "Key_PetDrown", KeyCode.Alpha8, "Spawn Pet Drown");
            keyPetJackbox = config.Bind("Simple Spawner", "Key_PetJackbox", KeyCode.Alpha9, "Spawn Pet Jackbox");
            keyPetSnowBoss = config.Bind("Simple Spawner", "Key_PetSnowBoss", KeyCode.Alpha0, "Spawn Pet Snow Boss");
            keyPetHorse = config.Bind("Simple Spawner", "Key_PetHorse", KeyCode.U, "Spawn Pet Horse");
            keyPetImp = config.Bind("Simple Spawner", "Key_PetImp", KeyCode.I, "Spawn Pet Imp");
            keyPetKirov = config.Bind("Simple Spawner", "Key_PetKirov", KeyCode.O, "Spawn Pet Kirov");

            keySpawnFertilizer = config.Bind("Simple Spawner", "Key_SpawnFertilizer", KeyCode.Keypad1, "Spawn Fertilizer");
            keySpawnBucket = config.Bind("Simple Spawner", "Key_SpawnBucket", KeyCode.Keypad2, "Spawn Bucket");
            keySpawnHelmet = config.Bind("Simple Spawner", "Key_SpawnHelmet", KeyCode.Keypad3, "Spawn Helmet");
            keySpawnJackbox = config.Bind("Simple Spawner", "Key_SpawnJackbox", KeyCode.Keypad4, "Spawn Jackbox");
            keySpawnPickaxe = config.Bind("Simple Spawner", "Key_SpawnPickaxe", KeyCode.Keypad5, "Spawn Pickaxe");
            keySpawnMachine = config.Bind("Simple Spawner", "Key_SpawnMachine", KeyCode.Keypad6, "Spawn Machine");
            keySpawnSuperMachine = config.Bind("Simple Spawner", "Key_SpawnSuperMachine", KeyCode.Keypad7, "Spawn Super Machine");
            keySpawnPortalHeart = config.Bind("Simple Spawner", "Key_SpawnPortalHeart", KeyCode.Keypad8, "Spawn Portal Heart");
            keySpawnSproutPotPrize = config.Bind("Simple Spawner", "Key_SpawnSproutPotPrize", KeyCode.Keypad9, "Spawn Sprout Pot Prize");
#endif
        }

        public static bool CheckKeyDown(KeyCode fallbackKey, string bindId, bool true_if_none = false)
        {
            if (Core.MagnetarLoaded)
            {
                return MagnetarIntegration.GetKeyDown(bindId, true_if_none);
            }

            if (fallbackKey == KeyCode.None)
            {
                return true_if_none;
            }

            return Input.GetKeyDown(fallbackKey);
        }

        public static bool CheckKey(KeyCode fallbackKey, string bindId, bool true_if_none = false)
        {
            if (Core.MagnetarLoaded)
            {
                return MagnetarIntegration.GetKey(bindId, true_if_none);
            }

            if (fallbackKey == KeyCode.None)
            {
                return true_if_none;
            }

            return Input.GetKeyDown(fallbackKey);
        }

        public static bool CheckActive()
        {
            if (Core.MagnetarLoaded) return MagnetarIntegration.Active();
            return true;
        }
    }
}