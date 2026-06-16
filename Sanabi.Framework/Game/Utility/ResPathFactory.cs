using System.Reflection;
using Sanabi.Framework.Game.Managers;
using Sanabi.Framework.Patching;

namespace Sanabi.Framework.Game.Utility;

public static class ResPathFactory
{
    public static readonly Type ResPathType = ReflectionManager.GetTypeByQualifiedName("Robust.Shared.Utility.ResPath", except: true);
    private static readonly ConstructorInfo ResPathConstructor = PatchHelpers.GetConstructor(ResPathType, [typeof(string)]);

    public static object Construct(string path)
        => ResPathConstructor.Invoke([path]);
}
