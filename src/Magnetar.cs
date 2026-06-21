using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Magnetar_Client.Modules;
#if MELONLOADER
using MelonLoader;
#elif BEPINEX
using BepInEx.Configuration;
#endif

namespace SimpleSpawner
{
    public static class MagnetarIntegration
    {
        public static bool IsMagnetarLoaded()
        {
            try
            {
                return System.AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.GetName().Name == "Magnetar Client" || a.GetName().Name == "Magnetar_Client");
            }
            catch { return false; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Init()
        {
            Magnetar_Client.Core.ModuleManager.showAddonCategory = true;
            Magnetar_Client.Core.ModuleManager.RegisterModule(typeof(SimpleSpawnerModule));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool GetKeyDown(string id, bool true_if_none)
        {
            if (SimpleSpawnerModule.instance != null && SimpleSpawnerModule.instance.Binds.TryGetValue(id, out var bind))
            {
                if (bind.BindKeys == null || bind.BindKeys.Count == 0 || bind.BindKeys[0] == KeyCode.None)
                {
                    return true_if_none;
                }

                foreach (var key in bind.BindKeys)
                {
                    if (Input.GetKeyDown(key)) return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool GetKey(string id, bool true_if_none)
        {
            if (SimpleSpawnerModule.instance != null && SimpleSpawnerModule.instance.Binds.TryGetValue(id, out var bind))
            {
                if (bind.BindKeys == null || bind.BindKeys.Count == 0 || bind.BindKeys[0] == KeyCode.None)
                {
                    return true_if_none;
                }

                foreach (var key in bind.BindKeys)
                {
                    if (Input.GetKey(key)) return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Active()
        {
            return SimpleSpawnerModule.instance != null && SimpleSpawnerModule.instance.Active;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SyncIfDirty()
        {
            if (SimpleSpawnerModule.instance == null) return;
            bool dirty = false;

            dirty |= UpdateKey(Config.keyDeleteAllPlants, "DeleteAllPlants");
            dirty |= UpdateKey(Config.keyDeleteAllZombies, "DeleteAllZombies");
            dirty |= UpdateKey(Config.keyToggleTimeScale, "ToggleTimeScale");
            dirty |= UpdateKey(Config.keySpawnPlant, "SpawnPlant");
            dirty |= UpdateKey(Config.keySpawnZombie, "SpawnZombie");
            dirty |= UpdateKey(Config.keyMindControlModifier, "MindControlModifier");
            dirty |= UpdateKey(Config.keyBossModifier, "BossModifier");
            dirty |= UpdateKey(Config.keySpawnBoss1, "SpawnBoss1");
            dirty |= UpdateKey(Config.keySpawnBoss2, "SpawnBoss2");

            dirty |= UpdateKey(Config.keyPetPre, "PetPre");
            dirty |= UpdateKey(Config.keyPetGargantuar, "PetGargantuar");
            dirty |= UpdateKey(Config.keyPetFootball, "PetFootball");
            dirty |= UpdateKey(Config.keyPetDrown, "PetDrown");
            dirty |= UpdateKey(Config.keyPetJackbox, "PetJackbox");
            dirty |= UpdateKey(Config.keyPetSnowBoss, "PetSnowBoss");
            dirty |= UpdateKey(Config.keyPetHorse, "PetHorse");
            dirty |= UpdateKey(Config.keyPetImp, "PetImp");
            dirty |= UpdateKey(Config.keyPetKirov, "PetKirov");

            dirty |= UpdateKey(Config.keySpawnFertilizer, "SpawnFertilizer");
            dirty |= UpdateKey(Config.keySpawnBucket, "SpawnBucket");
            dirty |= UpdateKey(Config.keySpawnHelmet, "SpawnHelmet");
            dirty |= UpdateKey(Config.keySpawnJackbox, "SpawnJackbox");
            dirty |= UpdateKey(Config.keySpawnPickaxe, "SpawnPickaxe");
            dirty |= UpdateKey(Config.keySpawnMachine, "SpawnMachine");
            dirty |= UpdateKey(Config.keySpawnSuperMachine, "SpawnSuperMachine");
            dirty |= UpdateKey(Config.keySpawnPortalHeart, "SpawnPortalHeart");
            dirty |= UpdateKey(Config.keySpawnSproutPotPrize, "SpawnSproutPotPrize");

            if (dirty)
            {
#if MELONLOADER
                Config.spawnerCategory.SaveToFile(false);
#elif BEPINEX
                Core.Instance.Config.Save();
#endif
            }
        }

#if MELONLOADER
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool UpdateKey(MelonPreferences_Entry<KeyCode> entry, string bindId)
        {
            if (SimpleSpawnerModule.instance.Binds.TryGetValue(bindId, out var bind))
            {
                KeyCode updatedKey = bind.BindKeys.Count > 0 ? bind.BindKeys[0] : KeyCode.None;
                if (entry.Value != updatedKey)
                {
                    entry.Value = updatedKey;
                    return true;
                }
            }
            return false;
        }
#elif BEPINEX
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool UpdateKey(ConfigEntry<KeyCode> entry, string bindId)
        {
            if (SimpleSpawnerModule.instance.Binds.TryGetValue(bindId, out var bind))
            {
                KeyCode updatedKey = bind.BindKeys.Count > 0 ? bind.BindKeys[0] : KeyCode.None;
                if (entry.Value != updatedKey)
                {
                    entry.Value = updatedKey;
                    return true;
                }
            }
            return false;
        }
#endif

        public class SimpleSpawnerModule : Magnetar_Client.Modules.Module
        {
            public override string Name { get; set; } = "Simple Spawner";
            public override string Description { get; set; } = "A <color=red>simple spawner</color> mod for Pvz fusion.\n" +
                "<b>Note:</b> Multi keybinds like (alt+ctrl+a) is not supported.";
            public override string Author { get; set; } = "Tproplay";
            public override string SearchHints { get; set; } = "simplespawner spawnmod pvzfusion spawner" +
                " spawnmenu spawnitems unitspawner spawnzombies spawnplants entityspawner spawncheat" +
                " itemspawner spawntool summoner summonsummon simple-spawner fusion-spawner spawnpvz " +
                "entitysummon spawnmanager summonmod spawnconfig spawnsettings spawncontroller";
            public override ModuleCategory Category { get; set; } = ModuleCategory.Addon;
            public override bool Active { get; set; } = true;

            public static SimpleSpawnerModule instance;
            public System.Collections.Generic.Dictionary<string, BindSetting> Binds = 
                new System.Collections.Generic.Dictionary<string, BindSetting>();

            public SimpleSpawnerModule()
            {
                instance = this;
                CreateCategory("General");
                AddBind("SpawnPlant", "Spawn plant", Config.keySpawnPlant.Value);
                AddBind("SpawnZombie", "Spawn zombie", Config.keySpawnZombie.Value);
                AddBind("DeleteAllPlants", "Delete all plants", Config.keyDeleteAllPlants.Value);
                AddBind("DeleteAllZombies", "Delete all zombies", Config.keyDeleteAllZombies.Value);
                EndCategory();

                CreateCategory("Pets");
                AddBind("PetPre", "Pet Spawn Prefix", Config.keyPetPre.Value);
                AddBind("PetGargantuar", "Spawn pet gargantuar", Config.keyPetGargantuar.Value);
                AddBind("PetFootball", "Spawn pet football", Config.keyPetFootball.Value);
                AddBind("PetDrown", "Spawn pet drown", Config.keyPetDrown.Value);
                AddBind("PetJackbox", "Spawn pet jackbox", Config.keyPetJackbox.Value);
                AddBind("PetSnowBoss", "Spawn pet snow boss", Config.keyPetSnowBoss.Value);
                AddBind("PetHorse", "Spawn pet horse", Config.keyPetHorse.Value);
                AddBind("PetImp", "Spawn pet imp", Config.keyPetImp.Value);
                AddBind("PetKirov", "Spawn pet kirov", Config.keyPetKirov.Value);
                EndCategory();

                CreateCategory("Extra");
                AddBind("ToggleTimeScale", "Toggle time scale", Config.keyToggleTimeScale.Value);
                AddBind("MindControlModifier", "Mind control modifier", Config.keyMindControlModifier.Value);
                AddBind("BossModifier", "Boss modifier", Config.keyBossModifier.Value);
                AddBind("SpawnBoss1", "Spawn boss 1", Config.keySpawnBoss1.Value);
                AddBind("SpawnBoss2", "Spawn boss 2", Config.keySpawnBoss2.Value);
                EndCategory();

                CreateCategory("Items");
                AddBind("SpawnFertilizer", "Spawn fertilizer", Config.keySpawnFertilizer.Value);
                AddBind("SpawnBucket", "Spawn bucket", Config.keySpawnBucket.Value);
                AddBind("SpawnHelmet", "Spawn helmet", Config.keySpawnHelmet.Value);
                AddBind("SpawnJackbox", "Spawn jackbox", Config.keySpawnJackbox.Value);
                AddBind("SpawnPickaxe", "Spawn pickaxe", Config.keySpawnPickaxe.Value);
                AddBind("SpawnMachine", "Spawn machine", Config.keySpawnMachine.Value);
                AddBind("SpawnSuperMachine", "Spawn super machine", Config.keySpawnSuperMachine.Value);
                AddBind("SpawnPortalHeart", "Spawn portal heart", Config.keySpawnPortalHeart.Value);
                AddBind("SpawnSproutPotPrize", "Spawn sprout pot prize", Config.keySpawnSproutPotPrize.Value);
                EndCategory();
            }

            private void AddBind(string id, string displayName, KeyCode defaultKey)
            {
                var keyList = defaultKey == KeyCode.None ?
                    new System.Collections.Generic.List<KeyCode>() :
                    new System.Collections.Generic.List<KeyCode> { defaultKey };

                var bind = new BindSetting(displayName, keyList);
                Binds[id] = bind;
                AddSettings(bind);
            }
        }
    }
}