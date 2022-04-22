// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Graphical User Interface helper functions

public static class TCP2_GUI
{
	private static Dictionary<string, Texture2D> CustomEditorTextures = new Dictionary<string, Texture2D>();
	private static Texture2D GetCustomTexture(string name)
	{
		var uiName = name + (EditorGUIUtility.isProSkin ? "pro" : "");

		if(CustomEditorTextures.ContainsKey(uiName))
			return CustomEditorTextures[uiName];

		var rootPath = TCP2_Utils.FindReadmePath(true);

		Texture2D texture = null;

		//load pro version
		if(EditorGUIUtility.isProSkin)
			texture = AssetDatabase.LoadAssetAtPath("Assets" + rootPath + "/Editor/Icons/" + name + "_Pro.png", typeof(Texture2D)) as Texture2D;

		//load default version
		if(texture == null)
			texture = AssetDatabase.LoadAssetAtPath("Assets" + rootPath + "/Editor/Icons/" + name + ".png", typeof(Texture2D)) as Texture2D;

		if(texture != null)
		{
			CustomEditorTextures.Add(uiName, texture);
			return texture;
		}

		return null;
	}

	private static GUIStyle _EnabledLabel;
	private static GUIStyle EnabledLabel
	{
		get
		{
			if(_EnabledLabel == null)
			{
				_EnabledLabel = new GUIStyle(EditorStyles.label);
				_EnabledLabel.normal.background = GetCustomTexture("TCP2_EnabledBg");
			}
			return _EnabledLabel;
		}
	}

	private static GUIStyle _ShurikenFoldout;
	public static GUIStyle ShurikenFoldout
	{
		get
		{
			if(_ShurikenFoldout == null)
			{
				_ShurikenFoldout = new GUIStyle(EditorStyles.foldout);

				var shurikenLabel = (GUIStyle)"ShurikenLabel";
				_ShurikenFoldout.alignment = shurikenLabel.alignment;
				_ShurikenFoldout.font = shurikenLabel.font;
				_ShurikenFoldout.fontStyle = FontStyle.Bold;
				_ShurikenFoldout.margin = shurikenLabel.margin;
				_ShurikenFoldout.padding = new RectOffset(16, 0, 2, 0);
				_ShurikenFoldout.richText = shurikenLabel.richText;
				_ShurikenFoldout.stretchHeight = shurikenLabel.stretchHeight;
				_ShurikenFoldout.stretchWidth = shurikenLabel.stretchWidth;
				_ShurikenFoldout.fixedHeight = 0;
				_ShurikenFoldout.fixedWidth = 0;

				_ShurikenFoldout.normal.textColor = new Color(.4f, .4f, .4f);
				_ShurikenFoldout.focused.textColor = _ShurikenFoldout.normal.textColor;
				_ShurikenFoldout.onNormal.textColor = _ShurikenFoldout.normal.textColor;
				_ShurikenFoldout.onFocused.textColor = _ShurikenFoldout.normal.textColor;
				_ShurikenFoldout.active.textColor = new Color(.3f, .3f, .3f);
				_ShurikenFoldout.onActive.textColor = _ShurikenFoldout.active.textColor;

				_ShurikenFoldout.normal.background = GetCustomTexture("TCP2_FoldoutArrowRight");
				_ShurikenFoldout.active.background = _ShurikenFoldout.normal.background;
				_ShurikenFoldout.focused.background = _ShurikenFoldout.normal.background;

				_ShurikenFoldout.onNormal.background = GetCustomTexture("TCP2_FoldoutArrowDown");
				_ShurikenFoldout.onActive.background = _ShurikenFoldout.onNormal.background;
				_ShurikenFoldout.onFocused.background = _ShurikenFoldout.onNormal.background;
			}
			return _ShurikenFoldout;
		}
	}

	private static GUIStyle _ContextualHelpBox;
	public static GUIStyle ContextualHelpBox
	{
		get
		{
			if(_ContextualHelpBox == null)
			{
				_ContextualHelpBox = new GUIStyle(EditorStyles.helpBox);
				_ContextualHelpBox.normal.background = GetCustomTexture("TCP2_ContextualHelpBox");
				_ContextualHelpBox.normal.textColor = EditorGUIUtility.isProSkin ? new Color32(120, 130, 150, 255) : new Color32(80, 90, 100, 255);
				_ContextualHelpBox.richText = true;
			}
			return _ContextualHelpBox;
		}
	}

	private static GUIStyle _ContextualHelpBoxHover;
	public static GUIStyle ContextualHelpBoxHover
	{
		get
		{
			if(_ContextualHelpBoxHover == null)
			{
				_ContextualHelpBoxHover = new GUIStyle(ContextualHelpBox);
				_ContextualHelpBoxHover.hover.background = GetCustomTexture("TCP2_ContextualHelpBoxHover");
				_ContextualHelpBoxHover.hover.textColor = _ContextualHelpBoxHover.normal.textColor;
			}
			return _ContextualHelpBoxHover;
		}
	}

	private static GUIStyle _EnabledPropertyHelpBoxExp;
	public static GUIStyle EnabledPropertyHelpBoxExp
	{
		get
		{
			if(_EnabledPropertyHelpBoxExp == null)
			{
				_EnabledPropertyHelpBoxExp = new GUIStyle(EditorStyles.helpBox);
				_EnabledPropertyHelpBoxExp.normal.background = GetCustomTexture("TCP2_EnabledBgPropertyExpanded");
				var border = _EnabledPropertyHelpBoxExp.border;
				border.top += 24;
				_EnabledPropertyHelpBoxExp.border = border;
			}
			return _EnabledPropertyHelpBoxExp;
		}
	}

	private static GUIStyle _EnabledPropertyHelpBox;
	public static GUIStyle EnabledPropertyHelpBox
	{
		get
		{
			if(_EnabledPropertyHelpBox == null)
			{
				_EnabledPropertyHelpBox = new GUIStyle(EditorStyles.helpBox);
				_EnabledPropertyHelpBox.normal.background = GetCustomTexture("TCP2_EnabledBgProperty");
			}
			return _EnabledPropertyHelpBox;
		}
	}

	private static GUIStyle _ErrorPropertyHelpBoxExp;
	public static GUIStyle ErrorPropertyHelpBoxExp
	{
		get
		{
			if(_ErrorPropertyHelpBoxExp == null)
			{
				_ErrorPropertyHelpBoxExp = new GUIStyle(EditorStyles.helpBox);
				_ErrorPropertyHelpBoxExp.normal.background = GetCustomTexture("TCP2_ErrorBgPropertyExpanded");
				var border = _ErrorPropertyHelpBoxExp.border;
				border.top += 24;
				_ErrorPropertyHelpBoxExp.border = border;
			}
			return _ErrorPropertyHelpBoxExp;
		}
	}

	private static GUIStyle _ErrorPropertyHelpBox;
	public static GUIStyle ErrorPropertyHelpBox
	{
		get
		{
			if(_ErrorPropertyHelpBox == null)
			{
				_ErrorPropertyHelpBox = new GUIStyle(EditorStyles.helpBox);
				_ErrorPropertyHelpBox.normal.background = GetCustomTexture("TCP2_ErrorBgProperty");
			}
			return _ErrorPropertyHelpBox;
		}
	}

	private static GUIStyle _Tab;
	public static GUIStyle Tab
	{
		get
		{
			if(_Tab == null)
			{
				_Tab = new GUIStyle(EditorStyles.toolbarButton);
				_Tab.normal.background = GetCustomTexture("TCP2_TabOff");
				_Tab.focused.background = GetCustomTexture("TCP2_TabOff");
				_Tab.active.background = GetCustomTexture("TCP2_TabOff");

				_Tab.onNormal.background = GetCustomTexture("TCP2_Tab");
				_Tab.onFocused.background = GetCustomTexture("TCP2_Tab");
				_Tab.onActive.background = GetCustomTexture("TCP2_Tab");

				_Tab.margin = new RectOffset(4, 4, 0, 0);
			}
			return _Tab;
		}
	}

	private static GUIStyle _HelpIcon;
	private static GUIStyle _HelpIcon2;
	public static bool UseNewHelpIcon;
	public static GUIStyle HelpIcon
	{
		get
		{
			if(_HelpIcon == null)
			{
				_HelpIcon = new GUIStyle(EditorStyles.label);
				_HelpIcon.fixedWidth = 16;
				_HelpIcon.fixedHeight = 16;

				_HelpIcon.normal.background = GetCustomTexture("TCP2_HelpIcon");
				_HelpIcon.active.background = GetCustomTexture("TCP2_HelpIcon_Down");
			}

			if(_HelpIcon2 == null)
			{
				_HelpIcon2 = new GUIStyle(_HelpIcon);

				_HelpIcon2.normal.background = GetCustomTexture("TCP2_HelpIcon2");
				_HelpIcon2.active.background = GetCustomTexture("TCP2_HelpIcon2_Down");
			}

			return UseNewHelpIcon ? _HelpIcon2 : _HelpIcon;
		}
	}

	private static GUIStyle _CogIcon;
	public static GUIStyle CogIcon
	{
		get
		{
			if(_CogIcon == null)
			{
				_CogIcon = new GUIStyle(EditorStyles.label);
				_CogIcon.fixedWidth = 16;
				_CogIcon.fixedHeight = 16;

				_CogIcon.normal.background = GetCustomTexture("TCP2_CogIcon");
				_CogIcon.active.background = GetCustomTexture("TCP2_CogIcon_Down");
			}

			return _CogIcon;
		}
	}

	private static GUIStyle _HeaderLabel;
	private static GUIStyle HeaderLabel
	{
		get
		{
			if(_HeaderLabel == null)
			{
				_HeaderLabel = new GUIStyle(EditorStyles.label);
				_HeaderLabel.fontStyle = FontStyle.Bold;

				var gray1 = EditorGUIUtility.isProSkin ? 0.7f : 0.35f;
				_HeaderLabel.normal.textColor = new Color(gray1, gray1, gray1);
			}
			return _HeaderLabel;
		}
	}

	private static GUIStyle _HeaderDropDown;
	public static GUIStyle HeaderDropDown
	{
		get
		{
			if(_HeaderDropDown == null)
			{
				_HeaderDropDown = new GUIStyle(EditorStyles.foldout);
				_HeaderDropDown.fontStyle = FontStyle.Bold;

				_HeaderDropDown.focused.background = _HeaderDropDown.normal.background;
				_HeaderDropDown.active.background = _HeaderDropDown.normal.background;
				_HeaderDropDown.onFocused.background = _HeaderDropDown.onNormal.background;
				_HeaderDropDown.onActive.background = _HeaderDropDown.onNormal.background;

				var gray1 = EditorGUIUtility.isProSkin ? 0.6f : 0.35f;
				var gray2 = EditorGUIUtility.isProSkin ? 0.5f : 0.45f;

				var textColor = new Color(gray1, gray1, gray1);
				var textColorActive = new Color(gray2, gray2, gray2);
				_HeaderDropDown.normal.textColor = textColor;
				_HeaderDropDown.onNormal.textColor = textColor;
				_HeaderDropDown.focused.textColor = textColor;
				_HeaderDropDown.onFocused.textColor = textColor;
				_HeaderDropDown.active.textColor = textColorActive;
				_HeaderDropDown.onActive.textColor = textColorActive;
			}
			return _HeaderDropDown;
		}
	}

	private static GUIStyle _HeaderDropDownHighlighted;
	private static GUIStyle HeaderDropDownHighlighted
	{
		get
		{
			if(_HeaderDropDownHighlighted == null)
			{
				_HeaderDropDownHighlighted = new GUIStyle(HeaderDropDown);

				var textColor = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.574f, 0.488f) : new Color(0.0f, 0.5f, 0.43f);
				var textColorActive = EditorGUIUtility.isProSkin ? new Color(0.0f, 0.5f, 0.43f) : new Color(0.0f, 0.574f, 0.488f);
				_HeaderDropDownHighlighted.normal.textColor = textColor;
				_HeaderDropDownHighlighted.onNormal.textColor = textColor;
				_HeaderDropDownHighlighted.focused.textColor = textColor;
				_HeaderDropDownHighlighted.onFocused.textColor = textColor;
				_HeaderDropDownHighlighted.active.textColor = textColorActive;
				_HeaderDropDownHighlighted.onActive.textColor = textColorActive;
			}
			return _HeaderDropDownHighlighted;
		}
	}

	private static GUIStyle _SubHeaderLabel;
	private static GUIStyle SubHeaderLabel
	{
		get
		{
			if(_SubHeaderLabel == null)
			{
				_SubHeaderLabel = new GUIStyle(EditorStyles.label);
				_SubHeaderLabel.fontStyle = FontStyle.Normal;
				_SubHeaderLabel.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.35f, 0.35f, 0.35f);
			}
			return _SubHeaderLabel;
		}
	}

	private static GUIStyle _BigHeaderLabel;
	private static GUIStyle BigHeaderLabel
	{
		get
		{
			if(_BigHeaderLabel == null)
			{
				_BigHeaderLabel = new GUIStyle(EditorStyles.largeLabel);
				_BigHeaderLabel.fontStyle = FontStyle.Bold;
				_BigHeaderLabel.fixedHeight = 30;
			}
			return _BigHeaderLabel;
		}
	}

	private static GUIStyle _FoldoutBold;
	public static GUIStyle FoldoutBold
	{
		get
		{
			if(_FoldoutBold == null)
			{
				_FoldoutBold = new GUIStyle(EditorStyles.foldout);
				_FoldoutBold.fontStyle = FontStyle.Bold;
			}
			return _FoldoutBold;
		}
	}

	public static GUIStyle _LineStyle;
	public static GUIStyle LineStyle
	{
		get
		{
			if(_LineStyle == null)
			{
				_LineStyle = new GUIStyle();
				_LineStyle.normal.background = EditorGUIUtility.whiteTexture;
				_LineStyle.stretchWidth = true;
			}

			return _LineStyle;
		}
	}

	static GUIStyle _HelpBoxRichTextStyle;
	public static GUIStyle HelpBoxRichTextStyle
	{
		get
		{
			if(_HelpBoxRichTextStyle == null)
			{
				_HelpBoxRichTextStyle = new GUIStyle("HelpBox");
				_HelpBoxRichTextStyle.richText = true;
			}
			return _HelpBoxRichTextStyle;
		}
	}

	public static Texture2D GetHelpBoxIcon(MessageType msgType)
	{
		switch(msgType)
		{
			case MessageType.Error:
				return GetCustomTexture("TCP2_ErrorIcon");
			case MessageType.Warning:
				return GetCustomTexture("TCP2_WarningIcon");
			case MessageType.Info:
				return GetCustomTexture("TCP2_InfoIcon");
		}

		return null;
	}

	//--------------------------------------------------------------------------------------------------
	// HELP

	public static void HelpButton(Rect rect, string helpTopic, string helpAnchor = null)
	{
		if(Button(rect, HelpIcon, "?", "Help about:\n" + helpTopic))
		{
			OpenHelpFor(string.IsNullOrEmpty(helpAnchor) ? helpTopic : helpAnchor);
		}
	}
	public static void HelpButton(string helpTopic, string helpAnchor = null)
	{
		if(Button(HelpIcon, "?", "Help about:\n" + helpTopic))
		{
			OpenHelpFor(string.IsNullOrEmpty(helpAnchor) ? helpTopic : helpAnchor);
		}
	}

	public static void OpenHelpFor(string helpTopic)
	{
		var rootDir = TCP2_Utils.FindReadmePath();
		if(rootDir == null)
		{
			EditorUtility.DisplayDialog("TCP2 Documentation", "Couldn't find TCP2 root folder! (the readme file is missing)\nYou can still access the documentation manually in the Documentation folder.", "Ok");
		}
		else
		{
			var helpAnchor = helpTopic.Replace("/", "_").Replace(@"\", "_").Replace(" ", "_").ToLowerInvariant() + ".htm";
			var topicLink = "file:///" + rootDir.Replace(@"\", "/") + "/Documentation/Documentation Data/Anchors/" + helpAnchor;
			Application.OpenURL(topicLink);
		}
	}

	public static void OpenHelp()
	{
		var rootDir = TCP2_Utils.FindReadmePath();
		if(rootDir == null)
		{
			EditorUtility.DisplayDialog("TCP2 Documentation", "Couldn't find TCP2 root folder! (the readme file is missing)\nYou can still access the documentation manually in the Documentation folder.", "Ok");
		}
		else
		{
			var helpLink = "file:///" + rootDir.Replace(@"\", "/") + "/Documentation/TCP2 Documentation.html";
			Application.OpenURL(helpLink);
		}
	}

	//--------------------------------------------------------------------------------------------------
	//GUI Functions

	public static void SeparatorSimple()
	{
		var color = EditorGUIUtility.isProSkin ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.65f, 0.65f, 0.65f);
		GUILine(color, 1);
		GUILayout.Space(1);
	}

	public static void Separator()
	{
		var colorDark = EditorGUIUtility.isProSkin ? new Color(.1f, .1f, .1f) : new Color(.3f, .3f, .3f);
		var colorBright = EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f) : new Color(.9f, .9f, .9f);

		GUILayout.Space(4);
		GUILine(colorDark, 1);
		GUILine(colorBright, 1);
		GUILayout.Space(4);
	}

	public static void Separator(Rect position)
	{
		var colorDark = EditorGUIUtility.isProSkin ? new Color(.1f, .1f, .1f) : new Color(.3f, .3f, .3f);
		var colorBright = EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f) : new Color(.9f, .9f, .9f);

		var lineRect = position;
		lineRect.height = 1;
		GUILine(lineRect, colorDark, 1);
		lineRect.y += 1;
		GUILine(lineRect, colorBright, 1);
	}

	public static void SeparatorBig()
	{
		GUILayout.Space(10);
		GUILine(new Color(.3f, .3f, .3f), 2);
		GUILayout.Space(1);
		GUILine(new Color(.3f, .3f, .3f), 2);
		GUILine(new Color(.85f, .85f, .85f), 1);
		GUILayout.Space(2);
	}

	public static void GUILine(float height = 2f)
	{
		GUILine(Color.black, height);
	}
	public static void GUILine(Color color, float height = 2f)
	{
		var position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);

		if(Event.current.type == EventType.Repaint)
		{
			var orgColor = GUI.color;
			GUI.color = orgColor * color;
			LineStyle.Draw(position, false, false, false, false);
			GUI.color = orgColor;
		}
	}
	public static void GUILine(Rect position, Color color, float height = 2f)
	{
		if(Event.current.type == EventType.Repaint)
		{
			var orgColor = GUI.color;
			GUI.color = orgColor * color;
			LineStyle.Draw(position, false, false, false, false);
			GUI.color = orgColor;
		}
	}

	//----------------------

	public static void Header(string header, string tooltip = null, bool expandWidth = false)
	{
		if(tooltip != null)
			EditorGUILayout.LabelField(new GUIContent(header, tooltip), HeaderLabel, GUILayout.ExpandWidth(expandWidth));
		else
			EditorGUILayout.LabelField(header, HeaderLabel, GUILayout.ExpandWidth(expandWidth));
	}

	public static void Header(Rect position, string header, string tooltip = null, bool expandWidth = false)
	{
		if(tooltip != null)
			EditorGUI.LabelField(position, new GUIContent(header, tooltip), HeaderLabel);
		else
			EditorGUI.LabelField(position, header, HeaderLabel);
	}

	public static bool HeaderFoldout(bool foldout, GUIContent guiContent)
	{
		foldout = EditorGUILayout.Foldout(foldout, guiContent, true, HeaderDropDown);
		return foldout;
	}

	public static bool HeaderFoldoutHighlight(bool foldout, GUIContent guiContent, bool highlighted)
	{
		foldout = EditorGUILayout.Foldout(foldout, guiContent, true, highlighted ? HeaderDropDownHighlighted : HeaderDropDown);
		return foldout;
	}

	public static void SubHeaderGray(string header, string tooltip = null, bool expandWidth = false)
	{
		if(tooltip != null)
			EditorGUILayout.LabelField(new GUIContent(header, tooltip), SubHeaderLabel, GUILayout.ExpandWidth(expandWidth));
		else
			EditorGUILayout.LabelField(header, SubHeaderLabel, GUILayout.ExpandWidth(expandWidth));
	}

	public static void HeaderAndHelp(string header, string helpTopic)
	{
		HeaderAndHelp(header, null, helpTopic);
	}
	public static void HeaderAndHelp(string header, string tooltip, string helpTopic)
	{
		GUILayout.BeginHorizontal();
		var r = GUILayoutUtility.GetRect(new GUIContent(header, tooltip), EditorStyles.label, GUILayout.ExpandWidth(true));
		var btnRect = r;
		btnRect.width = 16;
		//Button
		if(GUI.Button(btnRect, new GUIContent("", "Help about:\n" + helpTopic), HelpIcon))
			OpenHelpFor(helpTopic);
		//Label
		r.x += 16;
		r.width -= 16;
		GUI.Label(r, new GUIContent(header, tooltip), EditorStyles.boldLabel);
		GUILayout.EndHorizontal();
	}
	public static void HeaderAndHelp(Rect position, string header, string tooltip, string helpTopic)
	{
		if(!string.IsNullOrEmpty(helpTopic))
		{
			var btnRect = position;
			btnRect.width = 16;
			//Button
			if(GUI.Button(btnRect, new GUIContent("", "Help about:\n" + helpTopic), HelpIcon))
				OpenHelpFor(helpTopic);
		}

		//Label
		position.x += 16;
		position.width -= 16;
		GUI.Label(position, new GUIContent(header, tooltip), EditorStyles.boldLabel);
	}

	public static void HeaderBig(string header, string tooltip = null)
	{
		if(tooltip != null)
			EditorGUILayout.LabelField(new GUIContent(header, tooltip), BigHeaderLabel);
		else
			EditorGUILayout.LabelField(header, BigHeaderLabel);
	}

	public static void SubHeader(string header, string tooltip = null, float width = 146f)
	{
		SubHeader(header, tooltip, false, width);
	}
	public static void SubHeader(string header, string tooltip, bool highlight, float width)
	{
		if(tooltip != null)
			GUILayout.Label(new GUIContent(header, tooltip), highlight ? EnabledLabel : EditorStyles.label, GUILayout.Width(width));
		else
			GUILayout.Label(header, highlight ? EnabledLabel : EditorStyles.label, GUILayout.Width(width));
	}

	public static void SubHeader(Rect position, string header, string tooltip, bool highlight)
	{
		SubHeader(position, new GUIContent(header, tooltip), highlight);
	}
	public static void SubHeader(Rect position, GUIContent content, bool highlight)
	{
		GUI.Label(position, content, highlight ? EnabledLabel : EditorStyles.label);
	}

	public static void HelpBox(Rect position, string message, MessageType msgType)
	{
		EditorGUI.LabelField(position, GUIContent.none, new GUIContent(message, GetHelpBoxIcon(msgType)), HelpBoxRichTextStyle);
	}

	public static void HelpBoxLayout(string message, MessageType msgType)
	{
		EditorGUILayout.LabelField(GUIContent.none, new GUIContent(message, GetHelpBoxIcon(msgType)), HelpBoxRichTextStyle);
	}

	public static void ContextualHelpBoxLayout(string message, bool canHover = false)
	{
		EditorGUILayout.LabelField(GUIContent.none, new GUIContent(message, GetCustomTexture("TCP2_ContextIcon")), canHover ? ContextualHelpBoxHover : ContextualHelpBox);
	}

	//----------------------

	public static bool Button(GUIStyle icon, string noIconText, string tooltip = null)
	{
		if(icon == null)
			return GUILayout.Button(new GUIContent(noIconText, tooltip), EditorStyles.miniButton);
		return GUILayout.Button(new GUIContent("", tooltip), icon);
	}

	public static bool Button(Rect rect, GUIStyle icon, string noIconText, string tooltip = null)
	{
		if(icon == null)
			return GUI.Button(rect, new GUIContent(noIconText, tooltip), EditorStyles.miniButton);
		return GUI.Button(rect, new GUIContent("", tooltip), icon);
	}

	public static int RadioChoice(int choice, bool horizontal, params string[] labels)
	{
		var guiContents = new GUIContent[labels.Length];
		for(var i = 0; i < guiContents.Length; i++)
		{
			guiContents[i] = new GUIContent(labels[i]);
		}
		return RadioChoice(choice, horizontal, guiContents);
	}
	public static int RadioChoice(int choice, bool horizontal, params GUIContent[] labels)
	{
		if(horizontal)
			EditorGUILayout.BeginHorizontal();

		for(var i = 0; i < labels.Length; i++)
		{
			var style = EditorStyles.miniButton;
			if(labels.Length > 1)
			{
				if(i == 0)
					style = EditorStyles.miniButtonLeft;
				else if(i == labels.Length-1)
					style = EditorStyles.miniButtonRight;
				else
					style = EditorStyles.miniButtonMid;
			}

			if(GUILayout.Toggle(i == choice, labels[i], style))
			{
				choice = i;
			}
		}

		if(horizontal)
			EditorGUILayout.EndHorizontal();

		return choice;
	}

	public static int RadioChoiceHorizontal(Rect position, int choice, params GUIContent[] labels)
	{
		for(var i = 0; i < labels.Length; i++)
		{
			var rI = position;
			rI.width /= labels.Length;
			rI.x += i * rI.width;
			if(GUI.Toggle(rI, choice == i, labels[i], (i == 0) ? EditorStyles.miniButtonLeft : (i == labels.Length - 1) ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid))
			{
				choice = i;
			}
		}

		return choice;
	}
}

//===================================================================================================================================================================
// Material Property Drawers

public class TCP2HeaderHelpDecorator : MaterialPropertyDrawer
{
	protected readonly string header;
	protected readonly string help;

	public TCP2HeaderHelpDecorator(string header)
	{
		this.header = header;
		help = null;
	}
	public TCP2HeaderHelpDecorator(string header, string help)
	{
		this.header = header;
		this.help = help;
	}

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		TCP2_GUI.HeaderAndHelp(position, header, null, help);
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		return 18f;
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2HelpBoxDecorator : MaterialPropertyDrawer
{
	protected readonly MessageType msgType;
	protected readonly string message;
	protected Texture2D icon;

	private float InspectorWidth;   //Workaround to detect vertical scrollbar in the inspector
	static string ParseMessage(string message)
	{
		//double space = line break
		message = message.Replace("  ", "\n");

		// __word__ = <b>word</b>
		var sb = new StringBuilder();
		var words = message.Split(' ');
		for(var i = 0; i < words.Length; i++)
		{
			var w = words[i];
			if(w.StartsWith("__") && w.EndsWith("__"))
			{
				var w2 = w.Replace("__", "");
				w = w.Replace("__" + w2 + "__", "<b>" + w2 + "</b>");
			}

			sb.Append(w + " ");
		}
		var str = sb.ToString();
		return str.TrimEnd();
	}

	public TCP2HelpBoxDecorator(string messageType, string msg)
	{
		msgType = (MessageType)Enum.Parse(typeof(MessageType), messageType);
		message = ParseMessage(msg);
		icon = TCP2_GUI.GetHelpBoxIcon(msgType);
	}

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		position.height -= 4f;
		EditorGUI.LabelField(position, GUIContent.none, new GUIContent(message, icon), TCP2_GUI.HelpBoxRichTextStyle);
		//EditorGUI.HelpBox(position, this.message, this.msgType);

		if(Event.current != null && Event.current.type == EventType.Repaint)
			InspectorWidth = position.width;
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		//Calculate help box height
		var scrollBar = (Screen.width - InspectorWidth) > 20;
		var height = TCP2_GUI.HelpBoxRichTextStyle.CalcHeight(new GUIContent(message, icon), Screen.width - (scrollBar ? 51 : 34));
		return height + 6f;
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2SeparatorDecorator : MaterialPropertyDrawer
{
	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		position.y += 4;
		position.height -= 4;
		TCP2_GUI.Separator(position);
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		return 12f;
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2OutlineNormalsGUIDrawer : MaterialPropertyDrawer
{
	readonly GUIContent[] labels = {
			new GUIContent("Regular", "Use regular vertex normals"),
			new GUIContent("Vertex Colors", "Use vertex colors as normals (with smoothed mesh)"),
			new GUIContent("Tangents", "Use tangents as normals (with smoothed mesh)"),
			new GUIContent("UV2", "Use second texture coordinates as normals (with smoothed mesh)")
	};
	readonly string[] keywords = { "TCP2_NONE", "TCP2_COLORS_AS_NORMALS", "TCP2_TANGENT_AS_NORMALS", "TCP2_UV2_AS_NORMALS" };

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		TCP2_GUI.Header("OUTLINE NORMALS", "Defines where to take the vertex normals from to draw the outline.\nChange this when using a smoothed mesh to fill the gaps shown in hard-edged meshes.");

		var r = EditorGUILayout.GetControlRect();
		r = EditorGUI.IndentedRect(r);
		var index = GetCurrentIndex(prop);
		EditorGUI.BeginChangeCheck();
		index = TCP2_GUI.RadioChoiceHorizontal(r, index, labels);
		if(EditorGUI.EndChangeCheck())
		{
			SetKeyword(prop, index);
		}
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		return 0f;
	}

	int GetCurrentIndex(MaterialProperty prop)
	{
		var index = 0;
		var targets = prop.targets;
		foreach(var t in targets)
		{
			var m = (Material)t;
			for(var i = 0; i < keywords.Length; i++)
			{
				if(m.IsKeywordEnabled(keywords[i]))
				{
					return i;
				}
			}
		}
		return index;
	}

	private void SetKeyword(MaterialProperty prop, int index)
	{
		var label = prop.targets.Length > 1 ? string.Format("modify Outline Normals of {0} Materials", prop.targets.Length) : string.Format("modify Outline Normals of {0}", prop.targets[0].name);
		Undo.RecordObjects(prop.targets, label);
		for(var i = 0; i < keywords.Length; i++)
		{
			var keywordName = keywords[i];
			var targets = prop.targets;
			for(var j = 0; j < targets.Length; j++)
			{
				var material = (Material)targets[j];
				if(index == i)
				{
					material.EnableKeyword(keywordName);
				}
				else
				{
					material.DisableKeyword(keywordName);
				}
			}
		}
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2Vector4FloatsDrawer : MaterialPropertyDrawer
{
	const float spacing = 2f;

	bool expanded;
	string groupLabel;
	string[] labels;
	float[] min;
	float[] max;
	bool useSlider;

	public TCP2Vector4FloatsDrawer(string groupLabel, string labelX, string labelY, string labelZ, string labelW)
	{
		this.groupLabel = groupLabel;
		labels = new[] { labelX, labelY, labelZ, labelW };
		min = new[] { float.MinValue, float.MinValue, float.MinValue, float.MinValue };
		max = new[] { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue };
		useSlider = false;
		expanded = false;
	}
	public TCP2Vector4FloatsDrawer(string groupLabel, string labelX, string labelY, string labelZ, string labelW, float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float minW, float maxW)
	{
		this.groupLabel = groupLabel;
		labels = new[] { labelX, labelY, labelZ, labelW };
		min = new[] { SignedValue(minX), SignedValue(minY), SignedValue(minZ), SignedValue(minW) };
		max = new[] { SignedValue(maxX), SignedValue(maxY), SignedValue(maxZ), SignedValue(maxW) };
		useSlider = true;
		expanded = false;
	}

	//hacky workaround because adding a minus sign in a material drawer argument will break the shader
	float SignedValue(float val)
	{
		return val > 90000 ? 90000 - val : val;
	}

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		var lineRect = position;
		lineRect.x += 12;
		lineRect.height = (lineRect.height-(expanded ? 5*spacing : 0)) / (expanded ? 5f : 1f);

		var values = prop.vectorValue;
		EditorGUI.BeginChangeCheck();
		expanded = EditorGUI.Foldout(lineRect, expanded, groupLabel);
		if(expanded)
		{
			lineRect.y += lineRect.height + spacing;

			var lw = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = position.width - 200f;

			values.x = useSlider ? EditorGUI.Slider(lineRect, labels[0], values.x, min[0], max[0]) : EditorGUI.FloatField(lineRect, labels[0], values.x);
			lineRect.y += lineRect.height + spacing;
			values.y = useSlider ? EditorGUI.Slider(lineRect, labels[1], values.y, min[1], max[1]) : EditorGUI.FloatField(lineRect, labels[1], values.y);
			lineRect.y += lineRect.height + spacing;
			values.z = useSlider ? EditorGUI.Slider(lineRect, labels[2], values.z, min[2], max[2]) : EditorGUI.FloatField(lineRect, labels[2], values.z);
			lineRect.y += lineRect.height + spacing;
			values.w = useSlider ? EditorGUI.Slider(lineRect, labels[3], values.w, min[3], max[3]) : EditorGUI.FloatField(lineRect, labels[3], values.w);

			EditorGUIUtility.labelWidth = lw;
		}
		if(EditorGUI.EndChangeCheck())
		{
			prop.vectorValue = values;
		}
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		return (EditorGUIUtility.singleLineHeight+spacing) * (expanded ? 5 : 1) - spacing;
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2ColorNoAlphaDrawer : MaterialPropertyDrawer
{
	public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
	{
		//Code from ColorPropertyInternal, but with alpha turned off
		EditorGUI.BeginChangeCheck();
		EditorGUI.showMixedValue = prop.hasMixedValue;
		bool hdr = (prop.flags & MaterialProperty.PropFlags.HDR) != MaterialProperty.PropFlags.None;
		bool showAlpha = false;
#if UNITY_2018_1_OR_NEWER
		Color colorValue = EditorGUI.ColorField(position, label, prop.colorValue, true, showAlpha, hdr);
#else
		Color colorValue = EditorGUI.ColorField(position, label, prop.colorValue, true, showAlpha, hdr, null);
#endif
		EditorGUI.showMixedValue = false;
		if(EditorGUI.EndChangeCheck())
		{
			prop.colorValue = colorValue;
		}
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2GradientDrawer : MaterialPropertyDrawer
{
	static Texture2D DefaultRampTexture;
	static bool DefaultTextureSearched;     //Avoid searching each update if texture isn't found

	private static GUIContent editButtonLabel = new GUIContent("Edit Gradient", "Edit the ramp texture using Unity's gradient editor");
	private static GUIContent editButtonDisabledLabel = new GUIContent("Edit Gradient", "Can't edit the ramp texture because it hasn't been generated with the Ramp Generator\n\n(Tools/Toony Colors Pro 2/Ramp Generator)");

	private AssetImporter assetImporter;

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		//Find default ramp texture if needed
		if(prop.textureValue == null)
		{
			if(DefaultRampTexture == null && !DefaultTextureSearched)
			{
				DefaultRampTexture = TryFindDefaultRampTexture();
				DefaultTextureSearched = true;
			}

			if(DefaultRampTexture != null)
			{
				prop.textureValue = DefaultRampTexture;
			}
		}

		float indent = EditorGUI.indentLevel * 16;

		//Label
		var labelRect = position;
		labelRect.height = 16f;
		var space = labelRect.height + 4;
		position.y += space - 3;
		position.height -= space;
		EditorGUI.PrefixLabel(labelRect, new GUIContent(label));

		//Texture object field
		position.height = 16f;
		var newTexture = (Texture)EditorGUI.ObjectField(position, prop.textureValue, typeof(Texture2D), false);
		if(newTexture != prop.textureValue)
		{
			prop.textureValue = newTexture;
			assetImporter = null;
		}

		//Preview texture override (larger preview, hides texture name)
		var previewRect = new Rect(position.x + indent - 1, position.y + 1, position.width - indent - 18, position.height - 2);
		if(prop.hasMixedValue)
		{
			var col = GUI.color;
			GUI.color = EditorGUIUtility.isProSkin ? new Color(.25f, .25f, .25f) : new Color(.85f, .85f, .85f);
			EditorGUI.DrawPreviewTexture(previewRect, Texture2D.whiteTexture);
			GUI.color = col;
			GUI.Label(previewRect, "―");
		}
		else if(prop.textureValue != null)
			EditorGUI.DrawPreviewTexture(previewRect, prop.textureValue);

		if(prop.textureValue != null)
		{
			assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(prop.textureValue));
		}

		//Edit button
		var buttonRect = labelRect;
		buttonRect.width = 70;
		buttonRect.x = labelRect.x + (labelRect.width-150);
		if(GUI.Button(buttonRect, "Create New", EditorStyles.miniButtonLeft))
		{
			var lastSavePath = TCP2_GradientManager.LAST_SAVE_PATH;
			if(!lastSavePath.Contains(Application.dataPath))
				lastSavePath = Application.dataPath;

			var path = EditorUtility.SaveFilePanel("Create New Ramp Texture", lastSavePath, "TCP2_CustomRamp", "png");
			if(!string.IsNullOrEmpty(path))
			{
				TCP2_GradientManager.LAST_SAVE_PATH = Path.GetDirectoryName(path);

				//Create texture and save PNG
				var projectPath = path.Replace(Application.dataPath, "Assets");
				TCP2_GradientManager.CreateAndSaveNewGradientTexture(256, projectPath);

				//Load created texture
				var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(projectPath);
				assetImporter = AssetImporter.GetAtPath(projectPath);

				//Assign to material(s)
				prop.textureValue = texture;

				//Open for editing
				TCP2_RampGenerator.OpenForEditing(texture, editor.targets, true);
			}
		}
		buttonRect.x += buttonRect.width;
		buttonRect.width = 80;
		var enabled = GUI.enabled;
		GUI.enabled = (assetImporter != null) && assetImporter.userData.StartsWith("GRADIENT") && !prop.hasMixedValue;
		if(GUI.Button(buttonRect, GUI.enabled ? editButtonLabel : editButtonDisabledLabel, EditorStyles.miniButtonRight))
		{
			TCP2_RampGenerator.OpenForEditing((Texture2D)prop.textureValue, editor.targets, true);
		}
		GUI.enabled = enabled;

	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		return 34f;
	}

	Texture2D TryFindDefaultRampTexture()
	{
		var rootPath = TCP2_Utils.FindReadmePath(true);
		if(!string.IsNullOrEmpty(rootPath))
		{
			var defaultTexPath = "Assets" + rootPath + "/Textures/TCP2_Ramp_3Levels.png";
			return AssetDatabase.LoadAssetAtPath<Texture2D>(defaultTexPath);
		}

		return null;
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2HeaderDecorator : MaterialPropertyDrawer
{
	protected readonly string header;

	public TCP2HeaderDecorator(string header)
	{
		this.header = header;
	}

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		position.y += 2;
		TCP2_GUI.Header(position, header);
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		return 18f;
	}
}

//---------------------------------------------------------------------------------------------------------------------

public class TCP2KeywordFilterDrawer : MaterialPropertyDrawer
{
	protected readonly string[] keywords;

	public TCP2KeywordFilterDrawer(string keyword)
	{
		keywords = keyword.Split(',');
	}

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		if(IsValid(editor))
		{
			EditorGUI.indentLevel++;
			editor.DefaultShaderProperty(prop, label);
			EditorGUI.indentLevel--;
		}
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		//There's still a small space if we return 0, -2 seems to get rid of that
		return -2f;
	}

	bool IsValid(MaterialEditor editor)
	{
		var valid = false;
		if(editor.target != null && editor.target is Material)
		{
			foreach(var kw in keywords)
				valid |= (editor.target as Material).IsKeywordEnabled(kw);
		}
		return valid;
	}
}

//----------------------------------------------------------------------------------------------------------------------------------------------------------------
// Same as Toggle drawer, but doesn't set any keyword
// This will avoid adding unnecessary shader keyword to the project

internal class TCP2ToggleNoKeywordDrawer : MaterialPropertyDrawer
{
	private static bool IsPropertyTypeSuitable(MaterialProperty prop)
	{
		return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		float result;
		if(!IsPropertyTypeSuitable(prop))
		{
			result = 40f;
		}
		else
		{
			result = base.GetPropertyHeight(prop, label, editor);
		}
		return result;
	}

	public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
	{
		if(!IsPropertyTypeSuitable(prop))
		{
			EditorGUI.HelpBox(position, "Toggle used on a non-float property: " + prop.name, MessageType.Warning);
		}
		else
		{
			EditorGUI.BeginChangeCheck();
			var flag = Mathf.Abs(prop.floatValue) > 0.001f;
			EditorGUI.showMixedValue = prop.hasMixedValue;
			flag = EditorGUI.Toggle(position, label, flag);
			EditorGUI.showMixedValue = false;
			if(EditorGUI.EndChangeCheck())
			{
				prop.floatValue = ((!flag) ? 0f : 1f);
			}
		}
	}
}
