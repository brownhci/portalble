using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandActionItem {
	public string Gesture;
	public float InsertTime;
	public Vector3 PalmNorm;
	public Vector3 PalmPos;

	public HandActionItem(string _gesture, float _insertTime, Vector3 _palmNorm, Vector3 _palmPos) {
		Gesture = _gesture;
		InsertTime = _insertTime;
		PalmNorm = _palmNorm;
		PalmPos = _palmPos;
	}

	public static float CompareTwoHandActionItem(HandActionItem a, HandActionItem b) {
		float palmNormalScore = Vector3.Dot (a.PalmNorm, b.PalmNorm);

		// use a quatratic function as a score function for the dot of palm normals. (x - 1)^2
		palmNormalScore = (palmNormalScore - 1) * (palmNormalScore - 1);

		float palmPosScore = (a.PalmPos - b.PalmPos).magnitude;

		// gesture score, a constant
		float gestureScore = 0;
		if (a.Gesture != b.Gesture)
			gestureScore = 5;

		return gestureScore + palmNormalScore + palmPosScore;
	}
}
