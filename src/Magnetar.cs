using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Magnetar_Client.Modules;
#if MELONLOADER
using Il2Cpp;
#endif

namespace SimpleSpawner
{
    public static class MagnetarIntegration
    {
        public static bool IsMagnetarLoaded()
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies()
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
        public static bool GetKeyDown(string id, bool trueIfNone)
        {
            if (SimpleSpawnerModule.instance != null && SimpleSpawnerModule.instance.Binds.TryGetValue(id, out var bind))
            {
                return Config.CheckChordDown(bind.BindKeys, trueIfNone);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool GetKey(string id, bool trueIfNone)
        {
            if (SimpleSpawnerModule.instance != null && SimpleSpawnerModule.instance.Binds.TryGetValue(id, out var bind))
            {
                return Config.CheckChordHeld(bind.BindKeys, trueIfNone);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Active()
        {
            return SimpleSpawnerModule.instance != null && SimpleSpawnerModule.instance.Active;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SyncPlantSelection(PlantType plantType)
        {
            var module = SimpleSpawnerModule.instance;
            if (module?.PlantSelectedSetting == null) return;

            module.isSyncing = true;
            try
            {
                foreach (int val in module.PlantSelectedSetting.SelectedValues.ToList())
                {
                    if (val != (int)plantType) module.PlantSelectedSetting.Deselect(val);
                }

                if (plantType != PlantType.Nothing)
                {
                    module.PlantSelectedSetting.Select((int)plantType);
                }
            }
            finally
            {
                module.isSyncing = false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SyncZombieSelection(ZombieType zombieType)
        {
            var module = SimpleSpawnerModule.instance;
            if (module?.ZombieSelectedSetting == null) return;

            module.isSyncing = true;
            try
            {
                foreach (int val in module.ZombieSelectedSetting.SelectedValues.ToList())
                {
                    if (val != (int)zombieType) module.ZombieSelectedSetting.Deselect(val);
                }

                if (zombieType != ZombieType.Nothing)
                {
                    module.ZombieSelectedSetting.Select((int)zombieType);
                }
            }
            finally
            {
                module.isSyncing = false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SyncIfDirty()
        {
            if (SimpleSpawnerModule.instance == null) return;
            bool dirty = false;

            foreach (var bind in Config.AllBinds)
            {
                if (SimpleSpawnerModule.instance.Binds.TryGetValue(bind.Id, out var magnetarBind))
                {
                    var magnetarKeys = magnetarBind.BindKeys ?? new List<KeyCode>();
                    string magnetarSerialized = Config.SerializeKeys(magnetarKeys);
                    string currentSerialized = Config.SerializeKeys(bind.Keys);

                    if (magnetarSerialized != currentSerialized)
                    {
                        bind.SetKeys(magnetarKeys);
                        dirty = true;
                    }
                }
            }

            if (dirty)
            {
#if MELONLOADER
                Config.spawnerCategory.SaveToFile(false);
#elif BEPINEX
                Core.Instance.Config.Save();
#endif
            }
        }

        public class SimpleSpawnerModule : Magnetar_Client.Modules.Module
        {
            public override string Name { get; set; } = "Simple Spawner";
            public override string Description { get; set; } = "A <color=red>Simple Spawner</color> mod for Pvz Fusion.";
            public override string Author { get; set; } = "Tproplay";
            public override string SearchHints { get; set; } = "simplespawner spawnmod pvzfusion spawner unitspawner spawnzombies spawnplants";
            public override ModuleCategory Category { get; set; } = ModuleCategory.Addon;
            public override bool Active { get; set; } = true;

            public static SimpleSpawnerModule instance;
            public Dictionary<string, BindSetting> Binds = new Dictionary<string, BindSetting>();

            public MultiSelectSetting PlantSelectedSetting;
            public MultiSelectSetting ZombieSelectedSetting;
            public bool isSyncing = false;

            public SimpleSpawnerModule()
            {
                instance = this;

                // --- 1. Entity Selection Category ---
                CreateCategory("Entities");

                PlantSelectedSetting = new MultiSelectSetting("Select Plant", typeof(PlantType))
                {
                    MaxSelection = 1,
                    CustomNames = TranslatedNames(typeof(PlantType)),
                    Blacklist = new HashSet<int>
                    {
                        (int)PlantType.Nothing,
                        257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268,
                        246, 247, 3000
                    }
                };
                PlantSelectedSetting.Blacklist.Add((int)PlantType.Nothing);
                PlantSelectedSetting.OnSelectionChanged = (id, selected) =>
                {
                    if (isSyncing) return;
                    if (selected)
                    {
                        UpdateHandler.plantTypeselected = (PlantType)id;
                        foreach (int val in PlantSelectedSetting.SelectedValues.ToList())
                        {
                            if (val != id) PlantSelectedSetting.Deselect(val);
                        }
                    }
                    else if ((int)UpdateHandler.plantTypeselected == id)
                    {
                        UpdateHandler.plantTypeselected = PlantType.Nothing;
                    }
                };
                AddSettings(PlantSelectedSetting);

                ZombieSelectedSetting = new MultiSelectSetting("Select Zombie", typeof(ZombieType))
                {
                    MaxSelection = 1,
                    CustomNames = TranslatedNames(typeof(ZombieType)),
                    Blacklist = new HashSet<int>
                    {
                        (int)ZombieType.Nothing
                    }
                };
                ZombieSelectedSetting.Blacklist.Add((int)ZombieType.Nothing);
                ZombieSelectedSetting.OnSelectionChanged = (id, selected) =>
                {
                    if (isSyncing) return;
                    if (selected)
                    {
                        UpdateHandler.zombieTypeselected = (ZombieType)id;
                        foreach (int val in ZombieSelectedSetting.SelectedValues.ToList())
                        {
                            if (val != id) ZombieSelectedSetting.Deselect(val);
                        }
                    }
                    else if ((int)UpdateHandler.zombieTypeselected == id)
                    {
                        UpdateHandler.zombieTypeselected = ZombieType.Nothing;
                    }
                };
                AddSettings(ZombieSelectedSetting);

                EndCategory();

                // --- 2. Dynamic Keybind Categories ---
                var categories = Config.AllBinds.GroupBy(b => b.Category);
                foreach (var group in categories)
                {
                    CreateCategory(group.Key);
                    foreach (var bindDef in group)
                    {
                        AddBind(bindDef);
                    }
                    EndCategory();
                }
            }

            public override void OnLanguageChanged()
            {
                base.OnLanguageChanged();
                if (PlantSelectedSetting != null)
                    PlantSelectedSetting.CustomNames = TranslatedNames(typeof(PlantType));
                if (ZombieSelectedSetting != null)
                    ZombieSelectedSetting.CustomNames = TranslatedNames(typeof(ZombieType));
            }

            private void AddBind(KeyBind keyBind)
            {
                var keyList = new List<KeyCode>(keyBind.Keys);
                var bind = new BindSetting(keyBind.DisplayName, keyList);
                Binds[keyBind.Id] = bind;
                AddSettings(bind);
            }
        }
    }
}