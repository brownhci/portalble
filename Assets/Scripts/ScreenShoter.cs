using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Static Tools Class for Screen Shots
/// It supports screenshot on Android.
/// Written in 9/10/2018
/// </summary>
public class ScreenShoter {
    private static string dcim_root;
    private static string absolute_path;
    private static string folder_name;

    static ScreenShoter() {
        // Needs Android Classes to get dcim root
        var android_environment = new AndroidJavaClass("android.os.Environment");
        string dcim = android_environment.GetStatic<string>("DIRECTORY_DCIM");
        var galleryfile = android_environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", dcim);
        dcim_root = galleryfile.Call<string>("getAbsolutePath");
        folder_name = "Portalble";
        absolute_path = System.IO.Path.Combine(dcim_root, folder_name);
    }

    /// <summary>
    /// Get or Set folder name
    /// </summary>
    public static string FolderName {
        set {
            folder_name = value;
            absolute_path = System.IO.Path.Combine(dcim_root, folder_name);
        }
        get {
            return folder_name;
        }
    }

    public static void CaptureScreenShot(string name, bool saveInGallery = true) {
        if (saveInGallery) {
            // try png first
            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            // Check file type
            byte[] bytes = screenshot.EncodeToPNG();
            if (System.IO.Path.GetExtension(name) == ".jpg")
                bytes = screenshot.EncodeToJPG();
            else if (System.IO.Path.GetExtension(name) != ".png")
                name += ".png";

            string filepath = System.IO.Path.Combine(absolute_path, name);


            // Check if exist
            if (!Directory.Exists(absolute_path)) {
                Directory.CreateDirectory(absolute_path);
            }

            File.WriteAllBytes(filepath, bytes);

            // Notify Gallery Update
            AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + filepath) });
            objActivity.Call("sendBroadcast", objIntent);
        }
        else {
            ScreenCapture.CaptureScreenshot(name);
        }
    }
}
