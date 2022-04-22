using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMatchTransform : IHandMatchItem {
	string[] fromGestures;
	string[] toGestures;

	Vector3 fromNormal;
	Vector3 toNormal;

	float timeSpan;

	public HandMatchTransform (string fromgesture, string togesture, Vector3 fromNorm, Vector3 toNorm, float time) {
		fromGestures = fromgesture.Split (new char[]{ '|' });
		if (togesture == null || togesture == "")
			toGestures = fromGestures;
		else
			toGestures = togesture.Split (new char[]{ '|' });
		fromNormal = fromNorm.normalized;
		toNormal = toNorm.normalized;

		timeSpan = time;
	}

	public bool isMatch(ref LinkedListNode<HandActionItem> currentAction) {
		if (currentAction == null || currentAction.Value == null)
			return false;

		if (fromGestures == null || toGestures == null)
			return false;

		// try to fit this pattern
		HandActionItem action = currentAction.Value;
		int i = 0;
		for(i = 0; i < toGestures.Length; ++i) {
			if (action.Gesture == toGestures [i])
				break;
		}
		if (i >= toGestures.Length)
			return false;

		if(toNormal != Vector3.zero) {
			if (Vector3.Dot (toNormal, action.PalmNorm.normalized) < 0.866f)
				return false;
		}

		// try to jump over time to match pattern
		LinkedListNode<HandActionItem> prev = currentAction;
		while(prev != null && prev.Value != null) {
			prev = prev.Previous;
			if (prev == null || prev.Value == null)
				break;
			
			HandActionItem prevact = prev.Value;
			if (action.InsertTime - prev.Next.Value.InsertTime > timeSpan) {
				return false;
			}

			// try to match this
			for(i = 0; i < fromGestures.Length; ++i) {
				if (prevact.Gesture == fromGestures [i])
					break;
			}
			if (i >= fromGestures.Length)
				continue;

			if (fromNormal != Vector3.zero) {
				if (Vector3.Dot (fromNormal, prevact.PalmNorm.normalized) < 0.866f)
					continue;
			}

			// find it
			currentAction = prev.Next;
			return true;
		}
		return false;
	}
}
