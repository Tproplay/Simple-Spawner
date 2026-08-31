using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if MELONLOADER
using MelonLoader;
#elif BEPINEX
using BepInEx.Configuration;
#endif

namespace SimpleSpawner
{
    public class KeyBind
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string Category { get; }
        public List<KeyCode> DefaultKeys { get; }
        public List<KeyCode> Keys { get; private set; }

#if MELONLOADER
        public MelonPreferences_Entry<string> MelonEntry { get; internal set; }
#elif BEPINEX
        public ConfigEntry<string> BepInExEntry { get; internal set; }
#endif

        public KeyBind(string id, string displayName, string category, params KeyCode[] defaultKeys)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            DefaultKeys = defaultKeys != null && defaultKeys.Length > 0
                ? new List<KeyCode>(defaultKeys)
                : new List<KeyCode> { KeyCode.None };
            Keys = new List<KeyCode>(DefaultKeys);
        }

        public void SetKeys(IEnumerable<KeyCode> newKeys)
        {
            Keys = newKeys != null ? newKeys.Where(k => k != KeyCode.None).ToList() : new List<KeyCode>();
            if (Keys.Count == 0) Keys.Add(KeyCode.None);

            string serialized = Config.SerializeKeys(Keys);
#if MELONLOADER
            if (MelonEntry != null) MelonEntry.Value = serialized;
#elif BEPINEX
            if (BepInExEntry != null) BepInExEntry.Value = serialized;
#endif
        }

        public void LoadFromString(string raw)
        {
            Keys = Config.ParseKeys(raw);
            if (Keys.Count == 0) Keys.AddRange(DefaultKeys);
        }

        public bool IsDown(bool trueIfNone = false) => Config.CheckKeyDown(Keys, Id, trueIfNone);
        public bool IsPressed(bool trueIfNone = false) => Config.CheckKey(Keys, Id, trueIfNone);
    }

    public static class Config
    {
        public static readonly List<KeyBind> AllBinds = new List<KeyBind>();

#if MELONLOADER
        public static MelonPreferences_Category spawnerCategory;
#endif

        // =========================================================================
        // SINGLE PLACE TO CONFIGURE KEYBINDS (SUPPORTS MULTI-KEY CHORDS)
        // =========================================================================

        // --- 1. Base Spawners & Controls ---
        public static readonly KeyBind KeySpawnPlant = Register("SpawnPlant", "Spawn Plant", "General", KeyCode.LeftBracket);
        public static readonly KeyBind KeySpawnZombie = Register("SpawnZombie", "Spawn Zombie", "General", KeyCode.RightBracket);
        public static readonly KeyBind KeyToggleTimeScale = Register("ToggleTimeScale", "Freeze/Unfreeze Time", "General", KeyCode.Backslash);

        // --- 2. Deletion ---
        public static readonly KeyBind KeyDeleteAllPlants = Register("DeleteAllPlants", "Delete All Plants", "General", KeyCode.LeftControl, KeyCode.LeftBracket);
        public static readonly KeyBind KeyDeleteAllZombies = Register("DeleteAllZombies", "Delete All Zombies", "General", KeyCode.LeftControl, KeyCode.RightBracket);

        // --- 3. Hypnotized Spawns ---
        public static readonly KeyBind KeySpawnHypnoZombie = Register("SpawnHypnoZombie", "Spawn Hypnotized Zombie", "Zombies", KeyCode.Slash, KeyCode.RightBracket);
        public static readonly KeyBind KeySpawnBoss1 = Register("SpawnBoss1", "Spawn Zomboss 1", "Zombies", KeyCode.Period, KeyCode.Semicolon);
        public static readonly KeyBind KeySpawnBoss2 = Register("SpawnBoss2", "Spawn Zomboss 2", "Zombies", KeyCode.Period, KeyCode.Quote);
        public static readonly KeyBind KeySpawnHypnoBoss1 = Register("SpawnHypnoBoss1", "Spawn Hypno Zomboss 1", "Zombies", KeyCode.Slash, KeyCode.Semicolon);
        public static readonly KeyBind KeySpawnHypnoBoss2 = Register("SpawnHypnoBoss2", "Spawn Hypno Zomboss 2", "Zombies", KeyCode.Slash, KeyCode.Quote);

        // --- 4. Pot Spawns ---
        public static readonly KeyBind KeySpawnPlantPot = Register("SpawnPlantPot", "Spawn Plant Pot", "Pots", KeyCode.Comma, KeyCode.LeftBracket);
        public static readonly KeyBind KeySpawnZombiePot = Register("SpawnZombiePot", "Spawn Zombie Pot", "Pots", KeyCode.Comma, KeyCode.RightBracket);
        public static readonly KeyBind KeySpawnHypnoZombiePot = Register("SpawnHypnoZombiePot", "Spawn Hypno Zombie Pot", "Pots", KeyCode.Comma, KeyCode.Slash, KeyCode.RightBracket);
        public static readonly KeyBind KeySpawnBoss1Pot = Register("SpawnBoss1Pot", "Spawn Zomboss 1 Pot", "Pots", KeyCode.Comma, KeyCode.Semicolon);
        public static readonly KeyBind KeySpawnBoss2Pot = Register("SpawnBoss2Pot", "Spawn Zomboss 2 Pot", "Pots", KeyCode.Comma, KeyCode.Quote);
        public static readonly KeyBind KeySpawnHypnoBoss1Pot = Register("SpawnHypnoBoss1Pot", "Spawn Hypno Zomboss 1 Pot", "Pots", KeyCode.Comma, KeyCode.Slash, KeyCode.Semicolon);
        public static readonly KeyBind KeySpawnHypnoBoss2Pot = Register("SpawnHypnoBoss2Pot", "Spawn Hypno Zomboss 2 Pot", "Pots", KeyCode.Comma, KeyCode.Slash, KeyCode.Quote);

        // --- 5. Pets (Native Multi-Key Chords) ---
        public static readonly KeyBind KeyPetGargantuar = Register("PetGargantuar", "Spawn Pet Gargantuar", "Pets", KeyCode.RightControl, KeyCode.Keypad1);
        public static readonly KeyBind KeyPetFootball = Register("PetFootball", "Spawn Pet Football", "Pets", KeyCode.RightControl, KeyCode.Keypad2);
        public static readonly KeyBind KeyPetDrown = Register("PetDrown", "Spawn Pet Drown", "Pets", KeyCode.RightControl, KeyCode.Keypad3);
        public static readonly KeyBind KeyPetJackbox = Register("PetJackbox", "Spawn Pet Jackbox", "Pets", KeyCode.RightControl, KeyCode.Keypad4);
        public static readonly KeyBind KeyPetSnowBoss = Register("PetSnowBoss", "Spawn Pet Snow Boss", "Pets", KeyCode.RightControl, KeyCode.Keypad5);
        public static readonly KeyBind KeyPetHorse = Register("PetHorse", "Spawn Pet Horse", "Pets", KeyCode.RightControl, KeyCode.Keypad6);
        public static readonly KeyBind KeyPetImp = Register("PetImp", "Spawn Pet Imp", "Pets", KeyCode.RightControl, KeyCode.Keypad7);
        public static readonly KeyBind KeyPetKirov = Register("PetKirov", "Spawn Pet Kirov", "Pets", KeyCode.RightControl, KeyCode.Keypad8);

        // --- 6. Items ---
        public static readonly KeyBind KeySpawnFertilizer = Register("SpawnFertilizer", "Spawn Fertilizer", "Items", KeyCode.Keypad1);
        public static readonly KeyBind KeySpawnBucket = Register("SpawnBucket", "Spawn Bucket", "Items", KeyCode.Keypad2);
        public static readonly KeyBind KeySpawnHelmet = Register("SpawnHelmet", "Spawn Helmet", "Items", KeyCode.Keypad3);
        public static readonly KeyBind KeySpawnJackbox = Register("SpawnJackbox", "Spawn Jackbox", "Items", KeyCode.Keypad4);
        public static readonly KeyBind KeySpawnPickaxe = Register("SpawnPickaxe", "Spawn Pickaxe", "Items", KeyCode.Keypad5);
        public static readonly KeyBind KeySpawnMachine = Register("SpawnMachine", "Spawn Machine", "Items", KeyCode.Keypad6);
        public static readonly KeyBind KeySpawnSuperMachine = Register("SpawnSuperMachine", "Spawn Super Machine", "Items", KeyCode.Keypad7);
        public static readonly KeyBind KeySpawnPortalHeart = Register("SpawnPortalHeart", "Spawn Portal Heart", "Items", KeyCode.Keypad8);
        public static readonly KeyBind KeySpawnSproutPotPrize = Register("SpawnSproutPotPrize", "Spawn Sprout Pot Prize", "Items", KeyCode.Keypad9);
        public static readonly KeyBind KeySpawnZombieFertilizer = Register("SpawnZombieFertilizer", "Spawn Zombie Fertilizer", "Items", KeyCode.None);

        private static KeyBind Register(string id, string displayName, string category, params KeyCode[] defaultKeys)
        {
            var bind = new KeyBind(id, displayName, category, defaultKeys);
            AllBinds.Add(bind);
            return bind;
        }

        public static void Initialize()
        {
#if MELONLOADER
            spawnerCategory = MelonPreferences.CreateCategory("Simple Spawner");
            foreach (var bind in AllBinds)
            {
                bind.MelonEntry = spawnerCategory.CreateEntry(bind.Id, SerializeKeys(bind.DefaultKeys), bind.DisplayName);
                bind.LoadFromString(bind.MelonEntry.Value);
            }
#elif BEPINEX
            var config = Core.Instance.Config;
            foreach (var bind in AllBinds)
            {
                bind.BepInExEntry = config.Bind("Simple Spawner - " + bind.Category, bind.Id, SerializeKeys(bind.DefaultKeys), bind.DisplayName);
                bind.LoadFromString(bind.BepInExEntry.Value);
            }
#endif
        }

        public static bool CheckKeyDown(List<KeyCode> keys, string bindId, bool trueIfNone = false)
        {
            if (Core.MagnetarLoaded) return MagnetarIntegration.GetKeyDown(bindId, trueIfNone);
            return CheckChordDown(keys, trueIfNone);
        }

        public static bool CheckKey(List<KeyCode> keys, string bindId, bool trueIfNone = false)
        {
            if (Core.MagnetarLoaded) return MagnetarIntegration.GetKey(bindId, trueIfNone);
            return CheckChordHeld(keys, trueIfNone);
        }

        public static bool CheckChordDown(IList<KeyCode> keys, bool trueIfNone = false)
        {
            if (keys == null || keys.Count == 0 || (keys.Count == 1 && keys[0] == KeyCode.None))
                return trueIfNone;

            bool anyKeyDown = false;
            for (int i = 0; i < keys.Count; i++)
            {
                KeyCode k = keys[i];
                if (k == KeyCode.None) continue;

                bool isDown = IsKeyOrModifierDown(k);
                bool isHeld = IsKeyOrModifierHeld(k);

                if (isDown) anyKeyDown = true;
                else if (!isHeld) return false;
            }
            return anyKeyDown;
        }

        public static bool CheckChordHeld(IList<KeyCode> keys, bool trueIfNone = false)
        {
            if (keys == null || keys.Count == 0 || (keys.Count == 1 && keys[0] == KeyCode.None))
                return trueIfNone;

            for (int i = 0; i < keys.Count; i++)
            {
                KeyCode k = keys[i];
                if (k == KeyCode.None) continue;
                if (!IsKeyOrModifierHeld(k)) return false;
            }
            return true;
        }

        private static bool IsKeyOrModifierDown(KeyCode k)
        {
            if (Input.GetKeyDown(k)) return true;
            if (k == KeyCode.LeftControl || k == KeyCode.RightControl) return Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
            if (k == KeyCode.LeftShift || k == KeyCode.RightShift) return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
            if (k == KeyCode.LeftAlt || k == KeyCode.RightAlt) return Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
            return false;
        }

        private static bool IsKeyOrModifierHeld(KeyCode k)
        {
            if (Input.GetKey(k)) return true;
            if (k == KeyCode.LeftControl || k == KeyCode.RightControl) return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (k == KeyCode.LeftShift || k == KeyCode.RightShift) return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (k == KeyCode.LeftAlt || k == KeyCode.RightAlt) return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            return false;
        }

        public static bool CheckActive()
        {
            if (Core.MagnetarLoaded) return MagnetarIntegration.Active();
            return true;
        }

        public static List<KeyCode> ParseKeys(string raw)
        {
            var list = new List<KeyCode>();
            if (string.IsNullOrWhiteSpace(raw)) return list;

            var parts = raw.Split(new[] { '+', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                if (Enum.TryParse<KeyCode>(p.Trim(), true, out var kc) && kc != KeyCode.None)
                {
                    list.Add(kc);
                }
            }
            return list;
        }

        public static string SerializeKeys(IEnumerable<KeyCode> keys)
        {
            if (keys == null) return string.Empty;
            var valid = keys.Where(k => k != KeyCode.None).Select(k => k.ToString()).ToArray();
            return string.Join("+", valid);
        }
    }
}