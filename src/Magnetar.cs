using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using Magnetar_Client.Modules;

#if MELONLOADER
using Il2Cpp;
#endif

namespace SimpleSpawner
{
    public static class MagnetarIntegration
    {
        public static object ModuleInstance;
        public static Dictionary<string, BindSetting> Binds = new Dictionary<string, BindSetting>();
        public static MultiSelectSetting PlantSelectedSetting;
        public static MultiSelectSetting ZombieSelectedSetting;
        public static bool isSyncing = false;

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
            try
            {
                Type dynamicModuleType = CreateDynamicModuleType();
                Magnetar_Client.Core.ModuleManager.showAddonCategory = true;
                Magnetar_Client.Core.ModuleManager.RegisterModule(dynamicModuleType);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SimpleSpawner] Failed to create dynamic Magnetar module: {ex}");
            }
        }

        private static Type CreateDynamicModuleType()
        {
            var asmName = new AssemblyName("SimpleSpawner_DynamicMagnetar");
            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("Main");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                "SimpleSpawner.DynamicSpawnerModule",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(Magnetar_Client.Modules.Module)
            );

            var implementedMethods = new HashSet<MethodInfo>();

            // 1. Automatically implement all abstract properties (Name, Description, Author, etc.)
            var properties = typeof(Magnetar_Client.Modules.Module).GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            foreach (var prop in properties)
            {
                MethodInfo getMethod = prop.GetGetMethod(true);
                MethodInfo setMethod = prop.GetSetMethod(true);

                bool isGetAbstract = getMethod != null && getMethod.IsAbstract;
                bool isSetAbstract = setMethod != null && setMethod.IsAbstract;

                if (isGetAbstract || isSetAbstract)
                {
                    // Create a private backing field for the property
                    FieldBuilder field = typeBuilder.DefineField($"_{prop.Name}", prop.PropertyType, FieldAttributes.Private);

                    // Implement Getter
                    if (getMethod != null && isGetAbstract)
                    {
                        MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                        MethodBuilder mbGet = typeBuilder.DefineMethod(getMethod.Name, attrs, prop.PropertyType, Type.EmptyTypes);

                        ILGenerator il = mbGet.GetILGenerator();
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Ret);

                        typeBuilder.DefineMethodOverride(mbGet, getMethod);
                        implementedMethods.Add(getMethod);
                    }

                    // Implement Setter
                    if (setMethod != null && isSetAbstract)
                    {
                        MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                        MethodBuilder mbSet = typeBuilder.DefineMethod(setMethod.Name, attrs, typeof(void), new[] { prop.PropertyType });

                        ILGenerator il = mbSet.GetILGenerator();
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Stfld, field);
                        il.Emit(OpCodes.Ret);

                        typeBuilder.DefineMethodOverride(mbSet, setMethod);
                        implementedMethods.Add(setMethod);
                    }
                }
            }

            // 2. Implement any remaining abstract methods on Module with default returns
            var methods = typeof(Magnetar_Client.Modules.Module).GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            foreach (var method in methods)
            {
                if (method.IsAbstract && !implementedMethods.Contains(method))
                {
                    var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                    MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig;
                    MethodBuilder mb = typeBuilder.DefineMethod(method.Name, attrs, method.ReturnType, paramTypes);
                    ILGenerator il = mb.GetILGenerator();

                    if (method.ReturnType != typeof(void))
                    {
                        if (method.ReturnType.IsValueType)
                        {
                            LocalBuilder local = il.DeclareLocal(method.ReturnType);
                            il.Emit(OpCodes.Ldloca_S, local);
                            il.Emit(OpCodes.Initobj, method.ReturnType);
                            il.Emit(OpCodes.Ldloc, local);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldnull);
                        }
                    }
                    il.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(mb, method);
                }
            }

            // 3. Define Constructor & Invoke SetupModule(this)
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes
            );

            ILGenerator ctorIL = ctorBuilder.GetILGenerator();
            ConstructorInfo baseCtor = typeof(Magnetar_Client.Modules.Module).GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null
            );

            if (baseCtor != null)
            {
                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Call, baseCtor);
            }

            MethodInfo setupMethod = typeof(MagnetarIntegration).GetMethod(nameof(SetupModule), BindingFlags.Public | BindingFlags.Static);
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, setupMethod);
            ctorIL.Emit(OpCodes.Ret);

            // 4. Override OnLanguageChanged
            MethodInfo onLangMethod = typeof(Magnetar_Client.Modules.Module).GetMethod("OnLanguageChanged", BindingFlags.Public | BindingFlags.Instance);
            if (onLangMethod != null)
            {
                MethodBuilder overrideMethod = typeBuilder.DefineMethod(
                    "OnLanguageChanged",
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    typeof(void),
                    Type.EmptyTypes
                );

                ILGenerator il = overrideMethod.GetILGenerator();
                if (!onLangMethod.IsAbstract)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Call, onLangMethod);
                }
                il.Emit(OpCodes.Call, typeof(MagnetarIntegration).GetMethod(nameof(OnLanguageChangedHook), BindingFlags.Public | BindingFlags.Static));
                il.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(overrideMethod, onLangMethod);
            }

            return typeBuilder.CreateType();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SetupModule(object instanceObj)
        {
            var module = (Magnetar_Client.Modules.Module)instanceObj;
            ModuleInstance = module;

            module.Name = "Simple Spawner";
            module.Description = "A <color=red>Simple Spawner</color> mod for Pvz Fusion.";
            module.Author = "Tproplay";
            module.SearchHints = "simplespawner spawnmod pvzfusion spawner unitspawner spawnzombies spawnplants";
            module.Category = ModuleCategory.Addon;
            module.Active = true;

            // --- 1. Entity Selection Category ---
            module.CreateCategory("Entities");

            PlantSelectedSetting = new MultiSelectSetting("Select Plant", typeof(PlantType))
            {
                MaxSelection = 1,
                CustomNames = Magnetar_Client.Modules.Module.TranslatedNames(typeof(PlantType)),
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
            module.AddSettings(PlantSelectedSetting);

            ZombieSelectedSetting = new MultiSelectSetting("Select Zombie", typeof(ZombieType))
            {
                MaxSelection = 1,
                CustomNames = Magnetar_Client.Modules.Module.TranslatedNames(typeof(ZombieType)),
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
            module.AddSettings(ZombieSelectedSetting);
            module.EndCategory();

            // --- 2. Dynamic Keybind Categories ---
            var categories = Config.AllBinds.GroupBy(b => b.Category);
            foreach (var group in categories)
            {
                module.CreateCategory(group.Key);
                foreach (var bindDef in group)
                {
                    var keyList = new List<KeyCode>(bindDef.Keys);
                    var bind = new BindSetting(bindDef.DisplayName, keyList);
                    Binds[bindDef.Id] = bind;
                    module.AddSettings(bind);
                }
                module.EndCategory();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnLanguageChangedHook()
        {
            if (ModuleInstance != null)
            {
                if (PlantSelectedSetting != null)
                    PlantSelectedSetting.CustomNames = Magnetar_Client.Modules.Module.TranslatedNames(typeof(PlantType));
                if (ZombieSelectedSetting != null)
                    ZombieSelectedSetting.CustomNames = Magnetar_Client.Modules.Module.TranslatedNames(typeof(ZombieType));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool GetKeyDown(string id, bool trueIfNone)
        {
            if (ModuleInstance != null && Binds.TryGetValue(id, out var bind))
            {
                return Config.CheckChordDown(bind.BindKeys, trueIfNone);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool GetKey(string id, bool trueIfNone)
        {
            if (ModuleInstance != null && Binds.TryGetValue(id, out var bind))
            {
                return Config.CheckChordHeld(bind.BindKeys, trueIfNone);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Active()
        {
            if (ModuleInstance is Magnetar_Client.Modules.Module mod)
            {
                return mod.Active;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SyncPlantSelection(PlantType plantType)
        {
            if (PlantSelectedSetting == null) return;

            isSyncing = true;
            try
            {
                foreach (int val in PlantSelectedSetting.SelectedValues.ToList())
                {
                    if (val != (int)plantType) PlantSelectedSetting.Deselect(val);
                }

                if (plantType != PlantType.Nothing)
                {
                    PlantSelectedSetting.Select((int)plantType);
                }
            }
            finally
            {
                isSyncing = false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SyncZombieSelection(ZombieType zombieType)
        {
            if (ZombieSelectedSetting == null) return;

            isSyncing = true;
            try
            {
                foreach (int val in ZombieSelectedSetting.SelectedValues.ToList())
                {
                    if (val != (int)zombieType) ZombieSelectedSetting.Deselect(val);
                }

                if (zombieType != ZombieType.Nothing)
                {
                    ZombieSelectedSetting.Select((int)zombieType);
                }
            }
            finally
            {
                isSyncing = false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SyncIfDirty()
        {
            if (ModuleInstance == null) return;
            bool dirty = false;

            foreach (var bind in Config.AllBinds)
            {
                if (Binds.TryGetValue(bind.Id, out var magnetarBind))
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogDebug(string message)
        {
            Magnetar_Client.Utils.Magnetar_Logger.DebugLogger.Msg(message);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogError(string message)
        {
            Magnetar_Client.Utils.Magnetar_Logger.DebugLogger.Error(message);
        }
    }
}