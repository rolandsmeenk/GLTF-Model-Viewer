#define OPEN_ALL_FILES_IN_ORDER
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Storage.Pickers;
#endif 

internal static class FileDialogHelper
{
    internal static async Task<string> PickGLTFFileAsync()
    {
#if ENABLE_WINMD_SUPPORT

#if !OPEN_ALL_FILES_IN_ORDER
        var pickCompleted = new TaskCompletionSource<string>();

        UnityEngine.WSA.Application.InvokeOnUIThread(
            async () =>
            {
                Stream stream = null;
                FileOpenPicker picker = new FileOpenPicker();
                picker.SuggestedStartLocation = PickerLocationId.Objects3D;
                picker.FileTypeFilter.Add(".glb");
                picker.FileTypeFilter.Add(".gltf");
                picker.FileTypeFilter.Add("*");
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.CommitButtonText = "Select Model";

                var file = await picker.PickSingleFileAsync();
                string filePath = null;

                if (file != null)
                {
                    filePath = file.Path;
                }
                pickCompleted.SetResult(filePath);
            },
            true
        );

        await pickCompleted.Task;

        return (pickCompleted.Task.Result);

#else
        string file = null;

        if (fileList == null)
        {
            fileList = new List<string>();

            var folder = KnownFolders.Objects3D;
            var folderFiles = await folder.GetFilesAsync();

            foreach (var folderFile in folderFiles)
            {
                if (IsGltfFile(folderFile.Path))
                {
                    fileList.Add(file);
                }
            }
        }
        currentFileIndex++;

        if (currentFileIndex >= fileList.Count)
        {
            currentFileIndex = 0;
        }
        if (fileList.Count > 0)
        {
            file = fileList[currentFileIndex];
        }
        return (file);

#endif // OPEN_ALL_FILES_IN_ORDER

#else
        throw new InvalidOperationException(
            "Sorry, no file dialog support for other platforms here");
#endif // ENABLE_WINMD_SUPPORT
    }

#if OPEN_ALL_FILES_IN_ORDER

    static bool IsGltfFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        return (
            (string.Compare(extension, GLTF_EXTENSION, false) == 0) ||
            (string.Compare(extension, GLB_EXTENSION, false) == 0));
    }

    static int currentFileIndex;
    static List<string> fileList;
    static readonly string GLTF_EXTENSION = ".gltf";
    static readonly string GLB_EXTENSION = ".glb";

#endif // OPEN_ALL_FILES_IN_FOLDER
}