// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Menu Options for Toony Colors Pro 2

public static class TCP2_Menu
{
	//Change this path if you want the Toony Colors Pro 2 menu to appear elsewhere in the menu bar
	public const string MENU_PATH = @"Tools/Toony Colors Pro 2/";

	//--------------------------------------------------------------------------------------------------
	// DOCUMENTATION

	[MenuItem(MENU_PATH + "Documentation", false, 100)]
	static void OpenDocumentation()
	{
		TCP2_GUI.OpenHelp();
	}

	//--------------------------------------------------------------------------------------------------
	// UNPACK SHADERS

	[MenuItem(MENU_PATH + "Unpack Shaders/Rim (Desktop)", false, 800)]
	static void UnpackRim() { UnpackShaders("rim desktop"); }
	[MenuItem(MENU_PATH + "Unpack Shaders/Rim (Mobile)", false, 800)]
	static void UnpackRimMobile() { UnpackShaders("rim mobile"); }
	[MenuItem(MENU_PATH + "Unpack Shaders/Reflection (Desktop)", false, 800)]
	static void UnpackReflectionDesktop() { UnpackShaders("reflection desktop"); }
	[MenuItem(MENU_PATH + "Unpack Shaders/Matcap (Mobile)", false, 800)]
	static void UnpackMatcapMobile() { UnpackShaders("matcap mobile"); }
	[MenuItem(MENU_PATH + "Unpack Shaders/All Shaders (Mobile)", false, 800)]
	static void UnpackAllMobile() { UnpackShaders("mobile"); }
	[MenuItem(MENU_PATH + "Unpack Shaders/All Shaders (Desktop)", false, 800)]
	static void UnpackAllDesktop() { UnpackShaders("desktop"); }
	[MenuItem(MENU_PATH + "Unpack Shaders/All Shaders", false, 800)]
	static void UnpackAll() { UnpackShaders(""); }

	private static void UnpackShaders(string filter)
	{
		var archFiles = Directory.GetFiles( TCP2_Utils.UnityToSystemPath(Application.dataPath), "TCP2 Packed Shaders.tcp2data", SearchOption.AllDirectories );
		if(archFiles == null || archFiles.Length == 0)
		{
			EditorApplication.Beep();
			Debug.LogError("[TCP2 Unpack Shaders] Couldn't find file: \"TCP2 Packed Shaders.tcp2data\"\nPlease reimport Toony Colors Pro 2.");
			return;
		}
		var archivePath = archFiles[0];
		if(archivePath.EndsWith(".tcp2data"))
		{
			var files = TCP2_Utils.ExtractArchive(archivePath, filter);

			var @continue = 0;
			if(files.Length > 8)
			{
				do
				{
					@continue = EditorUtility.DisplayDialogComplex("TCP2 : Unpack Shaders", "You are about to import " + files.Length + " shaders in Unity.\nIt could take a few minutes!\nContinue?", "Yes", "No", "Help");
					if(@continue == 2)
					{
						TCP2_GUI.OpenHelpFor("Unpack Shaders");
					}
				}
				while(@continue == 2);
			}

			if(@continue == 0 && files.Length > 0)
			{
				var tcpRoot = TCP2_Utils.FindReadmePath();
				foreach(var f in files)
				{
					var filePath = tcpRoot + f.path;
					var fileDir = Path.GetDirectoryName(filePath);
					if(!Directory.Exists(fileDir))
					{
						Directory.CreateDirectory(fileDir);
					}
					File.WriteAllText(filePath, f.content);
				}
				
				Debug.Log("Toony Colors Pro 2 - Unpack Shaders:\n" + files.Length + (files.Length > 1 ? " shaders extracted." : " shader extracted."));
				AssetDatabase.Refresh();
			}

			if(files.Length == 0)
			{
				Debug.Log("Toony Colors Pro 2 - Unpack Shaders:\nNothing to unpack. Shaders are probably already unpacked!");
			}
		}
	}

	//--------------------------------------------------------------------------------------------------
	// RESET MATERIAL

	[MenuItem(MENU_PATH + "Reset Selected Material(s)", false, 900)]
	static void ResetSelectedMaterials()
	{
		foreach(var o in Selection.objects)
		{
			if(o is Material)
			{
				var user = false;
				var keywordsList = new List<string>((o as Material).shaderKeywords);
				if(keywordsList.Contains("USER"))
					user = true;
				(o as Material).shaderKeywords = user ? new[]{"USER"} : new string[0];
				if((o as Material).shader != null && (o as Material).shader.name.Contains("Mobile"))
					(o as Material).shader = Shader.Find("Toony Colors Pro 2/Mobile");
				else
					(o as Material).shader = Shader.Find("Toony Colors Pro 2/Desktop");
				Debug.Log("[TCP2] Keywords reset for " + o.name);
			}
		}
	}

	[MenuItem(MENU_PATH + "Reset Selected Material(s)", true, 900)]
	static bool ResetSelectedMaterials_Validate()
	{
		foreach(var o in Selection.objects)
		{
			if(o is Material)
			{
				return true;
			}
		}

		return false;
	}

	//--------------------------------------------------------------------------------------------------
	// CONVERT TCP1 MATERIALS TO TCP2
	// Commented: this shouldn't be needed anymore, TCP1 is now quite old

	/*
	[MenuItem(MENU_PATH + "Convert Selected Material(s) from TCP1 to TCP2", false, 900)]
	static void ConvertSelectedMaterials()
	{
		foreach(Object o in Selection.objects)
		{
			if(o is Material)
			{
				ConvertMaterial(o as Material);
			}
		}
	}

	[MenuItem(MENU_PATH + "Convert Selected Material(s) from TCP1 to TCP2", true, 900)]
	static bool ConvertSelectedMaterials_Validate()
	{
		foreach(Object o in Selection.objects)
		{
			if(o is Material)
			{
				return true;
			}
		}
		
		return false;
	}

	//Convert one Material from TCP1 to TCP2
	static private void ConvertMaterial(Material m)
	{
		if(m == null)
			return;

		if(m.shader == null)
			return;

		string originalShader = m.shader.name;

		if(originalShader.Contains("Toony Colors Pro 2"))
		{
			Debug.LogWarning("Skipping '" + m.name + "'\nMaterial already has TCP2 shader");
			return;
		}
		else if(!originalShader.Contains("Toony Colors Pro/"))
		{
			Debug.LogWarning("Skipping '" + m.name + "'\nMaterial doesn't have TCP1 shader");
			return;
		}

		// Keyword list
		List<string> tcp2Keywords = new List<string>();
		tcp2Keywords.Add("TCP2_RAMPTEXT");	//TCP1 only supports textured ramps

		// Analyze shader name to determine its features
		string[] nameParts = originalShader.Split(new string[]{"/"}, System.StringSplitOptions.RemoveEmptyEntries);

		if(nameParts.Length != 4)
		{
			Debug.LogWarning("Skipping '" + m.name + "'\nIconrrect number of parts in the name");
			return;
		}

		// 0 = Toony Colors Pro
		// 1 = Normal, Outline, OutlineConst, Outline Z-Correct, OutlineConst Z-Correct, Rim Outline
		// 2 = OneDirLight, MultipleLights
		// 3 = Basic, Basic Rim, Bumped, Bumped Rim, Bumped Specular, Bumped Specular Rim, Specular, Specular Rim

		switch(nameParts[1])
		{
			case "Normal":					break;
			case "Outline":					tcp2Keywords.Add("OUTLINES");	break;
			case "OutlineConst":			tcp2Keywords.Add("OUTLINES"); tcp2Keywords.Add("TCP2_OUTLINE_CONST_SIZE");	break;
			case "Outline Z-Correct":		tcp2Keywords.Add("OUTLINES"); tcp2Keywords.Add("TCP2_ZSMOOTH_ON");	break;
			case "OutlineConst Z-Correct":	tcp2Keywords.Add("OUTLINES"); tcp2Keywords.Add("TCP2_OUTLINE_CONST_SIZE"); tcp2Keywords.Add("TCP2_ZSMOOTH_ON");	break;
			case "Rim Outline":				tcp2Keywords.Add("TCP2_RIMO"); break;

			default: Debug.LogWarning("Material: " + m.name + ", Shader: " + originalShader + "\nUnrecognized nameParts[1]: '" + nameParts[1] + "'"); break;
		}

		bool isMobile = false;
		switch(nameParts[2])
		{
			case "OneDirLight":		isMobile = true; break;
			case "MultipleLights":	isMobile = false; break;
			
			default: Debug.LogWarning("Material: " + m.name + ", Shader: " + originalShader + "\nUnrecognized nameParts[2]: '" + nameParts[2] + "'"); break;
		}

		if(nameParts[3].Contains("Bumped"))			tcp2Keywords.Add("TCP2_BUMP");
		if(nameParts[3].Contains("Rim"))			tcp2Keywords.Add("TCP2_RIM");
		if(nameParts[3].Contains("Specular"))		tcp2Keywords.Add("TCP2_SPEC");

		string features = originalShader + "\nMOBILE: " + isMobile.ToString();
		foreach(string kw in tcp2Keywords)
		{
			features += "\n" + kw;
		}

		// Variables conversion
		Color highlightColor = m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white;
		float rimMin = m.HasProperty("_RimPower") ? (m.GetFloat("_RimPower") / 4f) : 0.5f;
		float outline = m.HasProperty("_Outline") ? (m.GetFloat("_Outline") * 100f) : 1.0f;

		//Find TCP2 Shader
		string[] keywords = tcp2Keywords.ToArray();
		string tcp2ShaderName = GetShaderFromKeywords(isMobile, keywords);

		Shader tcp2Shader = Shader.Find(tcp2ShaderName);
		if(tcp2Shader == null)
		{
			Debug.LogWarning("Skipping '" + m.name + "'\nCouldn't find corresponding TCP2 shader\nGenerated shader name: " + tcp2ShaderName);
			return;
		}
		else
		{
			// Valid TCP2 Shader found
			Undo.RecordObject(m, string.Format("Material TCP2 conversion for {0}", m.name));
			m.shader = tcp2Shader;
			m.shaderKeywords = keywords;

			// Set converted variables
			//Highlight Color
			if(m.HasProperty("_HColor"))
			{
				m.SetColor("_HColor", highlightColor);
				m.SetColor("_Color", Color.white);
			}
			else
			{
				Debug.LogWarning("Property _HColor not found in resulting TCP2 shader:\n" + tcp2ShaderName);
			}
			//Rim power
			if(m.HasProperty("_RimMin"))
			{
				m.SetFloat("_RimMin", rimMin);
			}
			//Outline width
			if(m.HasProperty("_Outline"))
			{
				m.SetFloat("_Outline", outline);
			}

			EditorUtility.SetDirty(m);
		}
	}

	static private string GetShaderFromKeywords(bool isMobile, string[] keywords)
	{
		string shaderName = isMobile ? "Mobile" : "Desktop";

		bool isHidden = false;
		if(System.Array.IndexOf<string>(keywords, "TCP2_SPEC") >= 0)
		{
			isHidden = true;
			shaderName += " Specular";
		}
		if(System.Array.IndexOf<string>(keywords, "TCP2_RIM") >= 0)
		{
			isHidden = true;
			shaderName += " Rim";
		}
		if(System.Array.IndexOf<string>(keywords, "TCP2_RIMO") >= 0)
		{
			isHidden = true;
			shaderName += " RimOutline";
		}
		if(System.Array.IndexOf<string>(keywords, "OUTLINES") >= 0)
		{
			isHidden = true;
			shaderName += " Outline";
		}
		shaderName = isHidden ? "Hidden/Toony Colors Pro 2/Variants/" + shaderName : "Toony Colors Pro 2/" + shaderName;

		return shaderName;
	}
	*/
}
