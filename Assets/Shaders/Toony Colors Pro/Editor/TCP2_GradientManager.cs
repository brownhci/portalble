// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// Manages the Gradient Textures created with the Ramp Generator

public class TCP2_GradientManager
{
	public static string LAST_SAVE_PATH
	{
		get { return EditorPrefs.GetString("TCP2_GradientSavePath", Application.dataPath); }
		set { EditorPrefs.SetString("TCP2_GradientSavePath", value); }
	}

	public static bool CreateAndSaveNewGradientTexture(int width, string unityPath)
	{
		var gradient = new Gradient();
		gradient.colorKeys = new[] { new GradientColorKey(Color.black, 0.45f), new GradientColorKey(Color.white, 0.55f) };
		gradient.alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) };

		return SaveGradientTexture(gradient, width, unityPath);
	}

	public static bool SaveGradientTexture(Gradient gradient, int width, string unityPath)
	{
		var ramp = CreateGradientTexture(gradient, width);
		var png = ramp.EncodeToPNG();
		Object.DestroyImmediate(ramp);

		var systemPath = Application.dataPath + "/" + unityPath.Substring(7);
		File.WriteAllBytes(systemPath, png);

		AssetDatabase.ImportAsset(unityPath);
		var ti = AssetImporter.GetAtPath(unityPath) as TextureImporter;
		ti.wrapMode = TextureWrapMode.Clamp;
		ti.isReadable = true;
#if UNITY_5_5_OR_NEWER
		ti.textureCompression = TextureImporterCompression.Uncompressed;
		ti.alphaSource = TextureImporterAlphaSource.None;
#else
		ti.textureFormat = TextureImporterFormat.RGB24;
#endif
		//Gradient data embedded in userData
		ti.userData = GradientToUserData(gradient);
		ti.SaveAndReimport();

		return true;
	}

	public static string GradientToUserData(Gradient gradient)
	{
		var output = "GRADIENT\n";
		for(var i = 0; i < gradient.colorKeys.Length; i++)
			output += ColorToHex(gradient.colorKeys[i].color) + "," + gradient.colorKeys[i].time + "#";
		output = output.TrimEnd('#');
		output += "\n";
		for(var i = 0; i < gradient.alphaKeys.Length; i++)
			output += gradient.alphaKeys[i].alpha + "," + gradient.alphaKeys[i].time + "#";
		output = output.TrimEnd('#');
#if UNITY_5_5_OR_NEWER
		output += "\n" + gradient.mode;
#endif

		return output;
	}

	public static void SetGradientFromUserData(string userData, Gradient gradient)
	{
		var keys = userData.Split('\n');
		if(keys == null || keys.Length < 3 || keys[0] != "GRADIENT")
		{
			EditorApplication.Beep();
			Debug.LogError("[TCP2_GradientManager] Invalid Gradient Texture\nMake sure the texture was created with the Ramp Generator.");
			return;
		}

		var ckData = keys[1].Split('#');
		var colorsKeys = new GradientColorKey[ckData.Length];
		for(var i = 0; i < ckData.Length; i++)
		{
			var data = ckData[i].Split(',');
			colorsKeys[i] = new GradientColorKey(HexToColor(data[0]), float.Parse(data[1]));
		}
		var akData = keys[2].Split('#');
		var alphaKeys = new GradientAlphaKey[akData.Length];
		for(var i = 0; i < akData.Length; i++)
		{
			var data = akData[i].Split(',');
			alphaKeys[i] = new GradientAlphaKey(float.Parse(data[0]), float.Parse(data[1]));
		}
		gradient.SetKeys(colorsKeys, alphaKeys);

#if UNITY_5_5_OR_NEWER
		if(keys.Length >= 4)
		{
			gradient.mode = (GradientMode)Enum.Parse(typeof(GradientMode), keys[3]);
		}
#endif
	}

	private static Texture2D CreateGradientTexture(Gradient gradient, int width)
	{
		var ramp = new Texture2D(width, 4, TextureFormat.RGB24, true, true);
		var colors = GetPixelsFromGradient(gradient, width);
		ramp.SetPixels(colors);
		ramp.Apply(true);
		return ramp;
	}

	public static Color[] GetPixelsFromGradient(Gradient gradient, int width)
	{
		var pixels = new Color[width*4];
		for(var x = 0; x < width; x++)
		{
			var delta = Mathf.Clamp01(x / (float)width);
			var col = gradient.Evaluate(delta);
			pixels[x+0*width] = col;
			pixels[x+1*width] = col;
			pixels[x+2*width] = col;
			pixels[x+3*width] = col;
		}
		return pixels;
	}

	public static string ColorToHex(Color32 color)
	{
		var hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	public static Color HexToColor(string hex)
	{
		var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(r, g, b, 255);
	}
}
