using Sanabi.Framework.Game.Managers;
using Sanabi.Framework.Data;
using Sanabi.Framework.Patching;
using HarmonyLib;
using System.Reflection;

namespace Sanabi.Framework.Game.Patches;

public static class UserDataPatch
{
    private static readonly HashSet<object> StealthedDirProviders = [];
    public static bool Enabled => SanabiConfig.ProcessConfig.RunUserDataPatch;

    private static readonly Type WdpType = ReflectionManager.GetTypeByQualifiedName("Robust.Shared.ContentPack.WritableDirProvider", except: true);

    private static PropertyInfo ResManUserData = null!;

    [PatchEntry(PatchRunLevel.Engine)]
    public static void Patch()
    {
        if (!Enabled)
            return;

        var resMan = ReflectionManager.GetTypeByQualifiedName("Robust.Shared.ContentPack.ResourceManager", except: true);
        ResManUserData = resMan.GetProperty("UserData")!;

        var resourceManagerInitialize = PatchHelpers.ResolveMethod(
            resMan, "Initialize");

        PatchHelpers.PatchMethod(
            resourceManagerInitialize,
            ResManInitializePostfix,
            HarmonyPatchType.Postfix
        );
    }

    private static void ResManInitializePostfix(ref dynamic __instance, ref string? userData /* param */)
    {
        // if userdata is already null for irrelevant reasons, let it do its own thing
        if (userData == null)
            return;

        // Don do nuffin
        var userDataDirectory = userData;
        if (!Directory.Exists(userDataDirectory))
            return;

        userData = null;

        // WritableDirProvider(DirectoryInfo rootDir, bool hideRootDir)
        var wdp = Activator.CreateInstance(WdpType, [new DirectoryInfo(userDataDirectory), true])!;
        StealthedDirProviders.Add(wdp);

        ResManUserData.SetValue(__instance, wdp);
    }
}
