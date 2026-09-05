using System;
using System.Reflection;

#if MELONLOADER
using MelonLoader;
#elif BEPINEX
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
#endif

#if MELONLOADER
[assembly: MelonInfo(typeof(SimpleSpawner.Core), "Simple Spawner", "3.9.1", "Tproplay")]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonOptionalDependencies("Magnetar Client")]
#endif

namespace SimpleSpawner
{
#if BEPINEX
    [BepInPlugin("com.tproplay.simplespawner", "Simple Spawner", "3.9.1")]
    [BepInProcess("PlantsVsZombiesRH.exe")]
    [BepInDependency("Magnetar_Client", BepInDependency.DependencyFlags.SoftDependency)]
    public class Core : BasePlugin
#elif MELONLOADER
    public class Core : MelonMod
#endif
    {
        public static Core Instance;
        public static bool MagnetarLoaded = false;

        static Core()
        {
            // Intercept assembly resolution to prevent TypeLoadException crashes in UniverseLib / HarmonyX
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var asmName = new AssemblyName(args.Name).Name;
                if (asmName == "Magnetar Client" || asmName == "Magnetar_Client")
                {
                    // Return null gracefully without bubbling fatal load exceptions
                    return null;
                }
                return null;
            };
        }

#if MELONLOADER
        public override void OnInitializeMelon()
        {
            Instance = this;
            Config.Initialize();

            MagnetarLoaded = MagnetarIntegration.IsMagnetarLoaded();
            if (MagnetarLoaded)
            {
                MagnetarIntegration.Init();
            }
        }

        public override void OnApplicationQuit()
        {
            if (MagnetarLoaded)
            {
                MagnetarIntegration.SyncIfDirty();
            }
        }

        public override void OnUpdate()
        {
            UpdateHandler.Update();
        }
#elif BEPINEX
        public override void Load()
        {
            Instance = this;
            SimpleSpawner.Config.Initialize();

            UpdateHandler.ApplyPatches();

            ClassInjector.RegisterTypeInIl2Cpp<CoreUpdater>();
            AddComponent<CoreUpdater>();

            MagnetarLoaded = MagnetarIntegration.IsMagnetarLoaded();
            if (MagnetarLoaded)
            {
                MagnetarIntegration.Init();
            }
        }

        public void OnApplicationQuit()
        {
            if (MagnetarLoaded)
            {
                MagnetarIntegration.SyncIfDirty();
            }
        }

        public class CoreUpdater : UnityEngine.MonoBehaviour
        {
            public CoreUpdater(System.IntPtr ptr) : base(ptr) { }
            private void Update() => UpdateHandler.Update();
        }
#endif
    }
}