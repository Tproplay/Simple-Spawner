using System;
using System.Reflection;

#if MELONLOADER
using MelonLoader;
using SimpleSpawner;
#elif BEPINEX
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
#endif

#if MELONLOADER
[assembly: MelonInfo(typeof(SimpleSpawner.Core), AssemblyInfo.Name, AssemblyInfo.Version, AssemblyInfo.Author)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonOptionalDependencies("Magnetar Client")]
#endif

namespace SimpleSpawner
{
#if BEPINEX
    [BepInPlugin("com.tproplay.simplespawner", AssemblyInfo.Name, AssemblyInfo.Version)]
    [BepInProcess("PlantsVsZombiesRH.exe")]
    [BepInDependency("Magnetar_Client", BepInDependency.DependencyFlags.SoftDependency)]
    public class Core : BasePlugin
#elif MELONLOADER
    public class Core : MelonMod
#endif
    {
        public static Core Instance;


#if MELONLOADER
        public override void OnInitializeMelon()
        {
            Instance = this;
            Config.Initialize();

            MagnetarIntegration.Init();
        }

        public override void OnApplicationQuit()
        {
            MagnetarIntegration.SyncIfDirty();
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

            MagnetarLoaded = MagnetarIntegration.IsMagnetarLoaded;
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