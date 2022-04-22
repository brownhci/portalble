using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMatchGesture : IHandMatchItem {
	string[] _gesturenames;
	float _duration;
	Vector3 _palmNorm;

	bool _posLock;

	public HandMatchGesture (string gesturename, Vector3 palmNorm, float duration, bool posLock) {
		_gesturenames = gesturename.Split (new char[] {'|'});
		_duration = duration;
		_palmNorm = palmNorm.normalized;

		_posLock = posLock;
	}

	public bool isMatch(ref LinkedListNode<HandActionItem> currentAction) {
		if (currentAction == null || currentAction.Value == null)
			return false;

		HandActionItem action = currentAction.Value;

		// gesture position match
		if (_posLock) {
			// Get current palm pos
			Vector3 curPos = GameObject.Find("Hand_l").transform.Find("palm").position;
			if ((curPos - action.PalmPos).magnitude > 0.05f) {
				return false;
			}
		}

		// gesture name match
		int i = 0;
		for (i = 0; i < _gesturenames.Length; ++i) {
			if (action.Gesture == _gesturenames [i])
				break;
		}

		if (i >= _gesturenames.Length)
			return false;

		// duration time match
		if (_duration > .0f) {
			float lastTime = (currentAction.Next != null) ? currentAction.Next.Value.InsertTime : Time.time;
			if (lastTime - action.InsertTime < _duration)
				return false;
		}

		// palm normal vector match, here uses 30 degrees.
		if (_palmNorm != Vector3.zero) {
			if (Vector3.Dot (action.PalmNorm.normalized, _palmNorm) < 0.866f) {
				return false;
			}
		}

		return true;
	}
}