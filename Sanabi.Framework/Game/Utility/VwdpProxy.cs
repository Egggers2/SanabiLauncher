using System.Reflection;
using Sanabi.Framework.Game.Managers;
using Sanabi.Framework.Patching;

namespace Sanabi.Framework.Game.Utility;

/// <summary>
///     Shit for VirtualWritableDirProvider
/// </summary>
public static class VwdpProxy
{
    private static readonly Type VwdpType = ReflectionManager.GetTypeByQualifiedName("Robust.Shared.ContentPack.VirtualWritableDirProvider", except: true);
    private static readonly MethodInfo VwdpTryGetNodeAt = PatchHelpers.GetMethod(VwdpType, "TryGetNodeAt")!;

    private static readonly Type BaseNodeType = ReflectionManager.GetTypeByQualifiedName("Robust.Shared.ContentPack.VirtualWritableDirProvider+INode", except: true);

    private static readonly Type DirectoryNodeType = ReflectionManager.GetTypeByQualifiedName("Robust.Shared.ContentPack.VirtualWritableDirProvider+DirectoryNode", except: true);
    private static readonly PropertyInfo DirectoryNodeChildren = DirectoryNodeType.GetProperty("Children")!;
    /// <summary>
    ///     Equivalent to Dictionary<string, INode>
    /// </summary>
    private static readonly Type DirectoryNodeChildrenType = typeof(Dictionary<,>).MakeGenericType(typeof(string), BaseNodeType);
    private static readonly MethodInfo DirectoryNodeChildrenAdd = PatchHelpers.GetMethod(DirectoryNodeChildrenType, "Add", [typeof(string), BaseNodeType])!;

    private static readonly Type FileNodeType = ReflectionManager.GetTypeByQualifiedName("Robust.Shared.ContentPack.VirtualWritableDirProvider+FileNode", except: true);
    private static readonly PropertyInfo FileNodeContents = FileNodeType.GetProperty("Contents")!;

    private static readonly PropertyInfo ResPathDirectory = ResPathFactory.ResPathType.GetProperty("Directory")!;
    private static readonly PropertyInfo ResPathFilename = ResPathFactory.ResPathType.GetProperty("Filename")!;

    /// <summary>
    ///     Returns the backing stream for the filenode
    /// </summary>
    public static Stream CreateNewUnsafe(object rpPath, object vwdp)
    {
        var filename = ResPathFilename.GetValue(rpPath);
        var parentPath = ResPathDirectory.GetValue(rpPath);

        var parameters = new object?[] { parentPath, null };
        if (!(bool)VwdpTryGetNodeAt.Invoke(vwdp, parameters)! || !DirectoryNodeType.IsAssignableFrom(parameters[1]!.GetType()))
            throw new InvalidOperationException($"No directory at {rpPath}");

        var parentDirectoryNode = Convert.ChangeType(parameters[1], DirectoryNodeType);
        var children = Convert.ChangeType(DirectoryNodeChildren.GetValue(parentDirectoryNode)!, DirectoryNodeChildrenType);

        var fileNode = Activator.CreateInstance(FileNodeType);
        DirectoryNodeChildrenAdd.Invoke(children, [filename, fileNode]);

        var virtualFileStream = (Stream)FileNodeContents.GetValue(fileNode)!;
        return virtualFileStream;
    }
}
