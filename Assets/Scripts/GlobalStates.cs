using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalStates {
    // some global states stored here
    public static bool isShift = false;
    // If the grid is set to be visible
    public static bool isGridVisible = true;
    // If the shadow is visible
    public static bool isShadowVisible = true;

    public static bool isIndicatorEnabled = true;

    public static Portalble.PortalbleConfig globalConfigFile = new Portalble.PortalbleConfig();

    private static Text debuglog;
    private static Text debuglog2;
    private static Text debuglog3;

    public static void DebugLog(string str)
    {
        if (debuglog)
            debuglog.text = str;
        else
            debuglog = GameObject.Find("DebugLog").GetComponent<Text>();
    }

    //Hightlight
    public static bool Highlight_on = true;

    public static bool resetFingerCount = false;
    /* Global States code copied from Throwable */
    //Actual hit point from user
    public static Vector3 groundImpulse = new Vector3(0, 0, 0);
    public static GameObject m_cross_position = new GameObject();

    public static List<double> threshold_0 = new List<double>();
    public static List<double> threshold_1 = new List<double>();
    public static List<double> threshold_2 = new List<double>();
    public static List<double> threshold_3 = new List<double>();

    public static List<Vector3> groundImpulseAry = new List<Vector3>();

    /* threshold model*/
    public static bool thresholdModel = false;

    /* adaptive model for no previous training*/
    public static bool adaptiveModelOn = true;

    public static float lastReleaseAngle = 0;

    public static float lastReleaseAngleAdapted = 0;

    public static float lastRawReleaseLateralSpeed = 0;

    public static bool speedCoefInitialized = false;

    public static Vector3 lastADPSpeed = new Vector3(0, 0, 0);

    public static List<float> errorY = new List<float>();

    public static List<float> SumErrorYList = new List<float>();

    public static List<float> AngleCoefList = new List<float>();

    public static List<float> SpeedSpeedCoefList = new List<float>();

    public static bool reachedAdaptionTarget = false;

    public static int AdaptionCoundIdx = 0;

    public static int errorYEpoch = 0;

    /* every n times, the error y will be calculated */
    public static int ErrorYListSize = 0;

    public static float SimualtedLateralSpeed = 0;

    public static float AutoAimAngle = 0;

    public static float AngleCoef = 1;

    public static float SpeedCoef = 1;

    public static float errorYFlatten = 0;

    public static GameObject latestManipulatedObj;

    }
