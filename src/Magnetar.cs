using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using MelonLoader;


#if MELONLOADER
using Il2Cpp;
#endif

namespace SimpleSpawner;

public static class MagnetarIntegration
{
    private static bool? _isMagnetarLoaded;
    public static bool IsMagnetarLoaded
    {
        get
        {
            if (!_isMagnetarLoaded.HasValue)
            {
                try
                {
                    _isMagnetarLoaded = AppDomain.CurrentDomain.GetAssemblies()
                        .Any(a => string.Equals(a.GetName().Name, "Magnetar Client", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(a.GetName().Name, "Magnetar_Client", StringComparison.OrdinalIgnoreCase));
                }
                catch
                {
                    _isMagnetarLoaded = false;
                }
            }
            return _isMagnetarLoaded.Value;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Init()
    {
        if (!IsMagnetarLoaded) return;

        try
        {
            MagnetarRunner.Init();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[SimpleSpawner] Magnetar integration failed to initialize: {ex.Message}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void SyncPlantSelection(PlantType plantType)
    {
        if (!IsMagnetarLoaded) return;
        MagnetarRunner.SyncPlantSelection(plantType);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void SyncZombieSelection(ZombieType zombieType)
    {
        if (!IsMagnetarLoaded) return;
        MagnetarRunner.SyncZombieSelection(zombieType);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void LogWarning(string v)
    {
        if (IsMagnetarLoaded)
        {
            MagnetarRunner.LogWarning(v);
        }
        else
        {
#if MELONLOADER
            MelonLogger.Warning($"[SimpleSpawner] {v}");
#endif
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static bool GetKeyDown(string bindId, bool trueIfNone)
    {
        if (!IsMagnetarLoaded) return false;
        return MagnetarRunner.GetKeyDown(bindId, trueIfNone);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static bool GetKey(string bindId, bool trueIfNone)
    {
        if (!IsMagnetarLoaded) return false;
        return MagnetarRunner.GetKey(bindId, trueIfNone);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static bool Active()
    {
        if (!IsMagnetarLoaded) return true;
        return MagnetarRunner.Active();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void SyncIfDirty()
    {
        if (!IsMagnetarLoaded) return;
        MagnetarRunner.SyncIfDirty();
    }

    private static class MagnetarRunner
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void Init()
        {
            Magnetar_Client.Api.MagnetarApi.RegisterModule<SimpleSpawner>();
            Magnetar_Client.Api.MagnetarApi.LogMsg("Simple Spawner loaded");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void SyncPlantSelection(PlantType plantType)
        {
            if (SimpleSpawner.instance?.PlantSelectedSetting == null) return;

            var setting = SimpleSpawner.instance.PlantSelectedSetting;
            foreach (int val in setting.SelectedValues.ToList())
            {
                if (val != (int)plantType) setting.Deselect(val);
            }

            if (plantType != PlantType.Nothing)
            {
                setting.Select((int)plantType);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void SyncZombieSelection(ZombieType zombieType)
        {
            if (SimpleSpawner.instance?.ZombieSelectedSetting == null) return;

            var setting = SimpleSpawner.instance.ZombieSelectedSetting;
            foreach (int val in setting.SelectedValues.ToList())
            {
                if (val != (int)zombieType) setting.Deselect(val);
            }

            if (zombieType != ZombieType.Nothing)
            {
                setting.Select((int)zombieType);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void LogWarning(string v)
        {
            Magnetar_Client.Api.MagnetarApi.LogWarning(v);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static bool GetKeyDown(string bindId, bool trueIfNone)
        {
            if (SimpleSpawner.instance != null && SimpleSpawner.instance.Binds.TryGetValue(bindId, out var bind))
            {
                return Config.CheckChordDown(bind.BindKeys, trueIfNone);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static bool GetKey(string bindId, bool trueIfNone)
        {
            if (SimpleSpawner.instance != null && SimpleSpawner.instance.Binds.TryGetValue(bindId, out var bind))
            {
                return Config.CheckChordHeld(bind.BindKeys, trueIfNone);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static bool Active()
        {
            return SimpleSpawner.instance == null || SimpleSpawner.instance.Active;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void SyncIfDirty()
        {
            if (SimpleSpawner.instance == null) return;
            bool dirty = false;

            foreach (var bind in Config.AllBinds)
            {
                if (SimpleSpawner.instance.Binds.TryGetValue(bind.Id, out var magnetarBind))
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
    }

    public class SimpleSpawner : Magnetar_Client.Modules.Module
    {
        public override string Name { get; set; } = "Simple Spawner";
        public override string Description { get; set; } = "A <color=red>Simple Spawner</color> mod for Pvz Fusion.";
        public override string SearchHints { get; set; } = "simplespawner spawnmod pvzfusion spawner unitspawner spawnzombies spawnplants";

        public override Magnetar_Client.Modules.ModuleCategory Category { get; set; } = Magnetar_Client.Modules.ModuleCategory.Addon;
        public override bool Active { get; set; } = true;
        public override bool defaultActive { get; } = true;

        public static SimpleSpawner instance;

        public Magnetar_Client.UI.Setting.MultiSelectSetting PlantSelectedSetting;
        public Magnetar_Client.UI.Setting.MultiSelectSetting ZombieSelectedSetting;
        public Dictionary<string, Magnetar_Client.UI.Setting.BindSetting> Binds = new(StringComparer.OrdinalIgnoreCase);

        public SimpleSpawner()
        {
            instance = this;

            CreateCategory("Entities");

            PlantSelectedSetting = new Magnetar_Client.UI.Setting.MultiSelectSetting("Select Plant", typeof(PlantType))
            {
                MaxSelection = 1,
                CustomNames = Magnetar_Client.Utils.Translator.TranslateEnum(typeof(PlantType)),
                Blacklist = Banned.PlantTypeBanned,
                OnSelectionChanged = (id, selected) =>
                {
                    if (selected)
                    {
                        UpdateHandler.plantTypeselected = (PlantType)id;
                    }
                    else if ((int)UpdateHandler.plantTypeselected == id)
                    {
                        UpdateHandler.plantTypeselected = PlantType.Nothing;
                    }
                },
            };
            AddSettings(PlantSelectedSetting);

            ZombieSelectedSetting = new Magnetar_Client.UI.Setting.MultiSelectSetting("Select Zombie", typeof(ZombieType))
            {
                MaxSelection = 1,
                CustomNames = Magnetar_Client.Utils.Translator.TranslateEnum(typeof(ZombieType)),
                Blacklist = Banned.ZombieTypeBanned,
                OnSelectionChanged = (id, selected) =>
                {
                    if (selected)
                    {
                        UpdateHandler.zombieTypeselected = (ZombieType)id;
                    }
                    else if ((int)UpdateHandler.zombieTypeselected == id)
                    {
                        UpdateHandler.zombieTypeselected = ZombieType.Nothing;
                    }
                }
            };
            AddSettings(ZombieSelectedSetting);
            EndCategory();

            var categories = Config.AllBinds.GroupBy(b => b.Category);
            foreach (var group in categories)
            {
                CreateCategory(group.Key);
                foreach (var bindDef in group)
                {
                    var keyList = new List<KeyCode>(bindDef.Keys);
                    var bind = new Magnetar_Client.UI.Setting.BindSetting(bindDef.DisplayName, keyList);
                    Binds[bindDef.Id] = bind;
                    AddSettings(bind);
                }
                EndCategory();
            }
        }

        public override void OnLanguageChanged()
        {
            if (PlantSelectedSetting != null)
                PlantSelectedSetting.CustomNames = Magnetar_Client.Utils.Translator.TranslateEnum(typeof(PlantType));

            if (ZombieSelectedSetting != null)
                ZombieSelectedSetting.CustomNames = Magnetar_Client.Utils.Translator.TranslateEnum(typeof(ZombieType));
        }
    }
}