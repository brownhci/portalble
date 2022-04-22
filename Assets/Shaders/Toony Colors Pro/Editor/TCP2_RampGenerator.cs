// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// Utility to generate ramp textures

public class TCP2_RampGenerator : EditorWindow
{
	[MenuItem(TCP2_Menu.MENU_PATH + "Ramp Generator", false, 500)]
	static void OpenTool()
	{
		GetWindowTCP2();
	}

	private static TCP2_RampGenerator GetWindowTCP2()
	{
		var window = GetWindow<TCP2_RampGenerator>(true, "TCP2 : Ramp Generator", true);
		window.editMode = false;
		window.linkedTexture = null;
		window.minSize = new Vector2(352f, 168f);
		window.maxSize = new Vector2(352f, 168f);
		return window;
	}

	public static void OpenForEditing(Texture2D texture, Object[] materials, bool openedFromMaterial)
	{
		var window = GetWindow<TCP2_RampGenerator>(true, "TCP2 : Ramp Generator", true);
		window.minSize = new Vector2(352f, 194f);
		window.maxSize = new Vector2(352f, 194f);
		var matList = new List<Material>();
		if(materials != null)
		{
			foreach(var o in materials)
				if(o is Material)
					matList.Add(o as Material);
		}
		window.editModeFromMaterial = openedFromMaterial;
		window.InitEditMode(texture, matList.ToArray());
	}

	//--------------------------------------------------------------------------------------------------
	// INTERFACE

#if UNITY_EDITOR_WIN
	private const string OUTPUT_FOLDER = "\\Textures\\Custom Ramps\\";
#else
	private const string OUTPUT_FOLDER = "/Textures/Custom Ramps/";
#endif

	[SerializeField]
	private Gradient mGradient;
	private int textureWidth = 256;
	private bool editMode;
	private bool textureEdited;
	private Texture2D linkedTexture;
	private AssetImporter linkedImporter;
	private Material[] linkedMaterials;
	private bool editModeFromMaterial;

	//--------------------------------------------------------------------------------------------------

	void OnEnable() { Init(); }

	void Init()
	{
		mGradient = new Gradient();
		mGradient.colorKeys = new[] { new GradientColorKey(Color.black, 0.49f), new GradientColorKey(Color.white, 0.51f) };
		mGradient.alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) };
	}

	void InitEditMode(Texture2D texture, Material[] materials)
	{
		textureEdited = false;
		editMode = true;
		linkedTexture = texture;
		linkedImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
		linkedMaterials = materials;
		TCP2_GradientManager.SetGradientFromUserData(linkedImporter.userData, mGradient);
		UpdateGradientPreview();
	}

	void OnDestroy()
	{
		if(textureEdited)
		{
			if(EditorUtility.DisplayDialog("Edited Ramp Texture", "There are pending edits on the following ramp texture:\n\n" + linkedTexture.name + "\n\nSave them?", "Yes", "Discard"))
			{
				SaveEditedTexture();
			}
			else
			{
				DiscardEditedTexture();
			}
		}
	}

	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		TCP2_GUI.HeaderBig(editMode ? "TCP 2 - RAMP EDITOR" : "TCP 2 - RAMP GENERATOR");
		TCP2_GUI.HelpButton("Ramp Generator");
		EditorGUILayout.EndHorizontal();
		TCP2_GUI.Separator();

		if(editMode)
		{
			var msg = "This will affect <b>all materials</b> that use this texture!" +
				(editModeFromMaterial ? "\n\nSave as a new texture first if you want to affect this material only." : "\n\nSave as a new texture if you want to keep the original ramp.");
			EditorGUILayout.LabelField(GUIContent.none, new GUIContent(msg, TCP2_GUI.GetHelpBoxIcon(MessageType.Warning)), TCP2_GUI.HelpBoxRichTextStyle);

			var rect = EditorGUILayout.GetControlRect(GUILayout.Height(16f));
			var lw = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 50f;
			var enabled = GUI.enabled;
			GUI.enabled = false;
			EditorGUI.ObjectField(rect, "Editing: ", linkedTexture, typeof(Texture2D), false);
			EditorGUIUtility.labelWidth = lw;
			GUI.enabled = enabled;
		}

		GUILayout.Label("Click on the gradient to edit it:");
		var so = new SerializedObject(this);
		var sp = so.FindProperty("mGradient");
		EditorGUILayout.PropertyField(sp, GUIContent.none);

		if(!editMode)
		{
			textureWidth = EditorGUILayout.IntField("TEXTURE SIZE:", textureWidth);
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("64", EditorStyles.miniButtonLeft)) textureWidth = 64;
			if(GUILayout.Button("128", EditorStyles.miniButtonMid)) textureWidth = 128;
			if(GUILayout.Button("256", EditorStyles.miniButtonMid)) textureWidth = 256;
			if(GUILayout.Button("512", EditorStyles.miniButtonMid)) textureWidth = 512;
			if(GUILayout.Button("1024", EditorStyles.miniButtonRight)) textureWidth = 1024;
			EditorGUILayout.EndHorizontal();
		}

		if (GUI.changed)
		{
			so.ApplyModifiedProperties();
			mGradient.alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) };

			if(editMode)
			{
				textureEdited = true;

				//Update linked texture
				var pixels = TCP2_GradientManager.GetPixelsFromGradient(mGradient, linkedTexture.width);
				linkedTexture.SetPixels(pixels);
				linkedTexture.Apply(true, false);
			}
		}

		GUILayout.Space(8f);
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(editMode)
		{
			if(GUILayout.Button("Discard", GUILayout.Width(90f), GUILayout.Height(20f)))
			{
				DiscardEditedTexture();
				if(editModeFromMaterial)
					Close();
				else
					OpenTool();
			}
			if(GUILayout.Button("Apply", GUILayout.Width(90f), GUILayout.Height(20f)))
			{
				SaveEditedTexture();
				if(editModeFromMaterial)
					Close();
				else
					OpenTool();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
		}

		var saveButton = false;
		if(editMode)
			saveButton = GUILayout.Button("Save as...", EditorStyles.miniButton, GUILayout.Width(120f), GUILayout.Height(16f));
		else
			saveButton = GUILayout.Button("GENERATE", GUILayout.Width(120f), GUILayout.Height(34f));
		if(saveButton)
		{
			var path = EditorUtility.SaveFilePanel("Save Generated Ramp", TCP2_GradientManager.LAST_SAVE_PATH, editMode ? linkedTexture.name : "TCP2_CustomRamp", "png");
			if(!string.IsNullOrEmpty(path))
			{
				TCP2_GradientManager.LAST_SAVE_PATH = Path.GetDirectoryName(path);
				var projectPath = path.Replace(Application.dataPath, "Assets");
				GenerateAndSaveTexture(projectPath);

				if(editMode)
				{
					var newtexture = AssetDatabase.LoadAssetAtPath<Texture2D>(projectPath);
					if(newtexture != null)
					{
						foreach(var mat in linkedMaterials)
						{
							mat.SetTexture("_Ramp", newtexture);
							EditorUtility.SetDirty(mat);
						}
					}

					//Reinitialize edit mode
					InitEditMode(newtexture, linkedMaterials);
				}
			}
		}
		EditorGUILayout.EndHorizontal();

		if(!editMode)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Load Texture", EditorStyles.miniButton, GUILayout.Width(120f)))
			{
				LoadTexture();
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	//--------------------------------------------------------------------------------------------------

	//Update Gradient preview through Reflection
	MethodInfo _ClearCacheMethod;
	MethodInfo ClearCacheMethod
	{
		get
		{
			if(_ClearCacheMethod == null)
			{
				var gpc = typeof(MonoScripts).Assembly.GetType("UnityEditorInternal.GradientPreviewCache");
				if(gpc != null)
					_ClearCacheMethod = gpc.GetMethod("ClearCache");
			}
			return _ClearCacheMethod;
		}
	}
	private void UpdateGradientPreview()
	{
		if(ClearCacheMethod != null)
			ClearCacheMethod.Invoke(null, null);
	}

	private void LoadTexture()
	{
		var path = EditorUtility.OpenFilePanel("TCP2 Gradient Texture", TCP2_GradientManager.LAST_SAVE_PATH, "png");
		if(!string.IsNullOrEmpty(path))
		{
			TCP2_GradientManager.LAST_SAVE_PATH = Path.GetDirectoryName(path);
			var assetPath = path.Replace(Application.dataPath, "Assets");
			var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
			if(texture != null)
			{
				OpenForEditing(texture, null, false);
			}
		}
	}

	private void GenerateAndSaveTexture(string path)
	{
		if(string.IsNullOrEmpty(path))
			return;

		TCP2_GradientManager.SaveGradientTexture(mGradient, textureWidth, path);
	}

	private void SaveEditedTexture()
	{
		if(textureEdited)
		{
			//Save data to file
			File.WriteAllBytes(Application.dataPath + AssetDatabase.GetAssetPath(linkedTexture).Substring(6), linkedTexture.EncodeToPNG());

			//Update linked texture userData
			linkedImporter.userData = TCP2_GradientManager.GradientToUserData(mGradient);
		}
		textureEdited = false;
	}

	private void DiscardEditedTexture()
	{
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(linkedTexture), ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
		textureEdited = false;
	}
}
