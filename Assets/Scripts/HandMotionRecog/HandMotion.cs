using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hand motion represents a motion of hand. It maintains a List of HandMatchItem and will check them if they are satisfied.
/// 
/// </summary>
public class HandMotion {
	public List<IHandMatchItem> _actionList;
	private string _motionName;

	public HandMotion(string motionname) {
		_actionList = new List<IHandMatchItem> ();
		_motionName = motionname;
	}

	public void AddMatchItem(IHandMatchItem item) {
		_actionList.Add (item);
	}

	public bool Match(LinkedList<HandActionItem> list) {
		LinkedListNode<HandActionItem> cur = list.Last;

		for(int i = _actionList.Count - 1; i >= 0; --i) {
            if (_actionList[i].isMatch(ref cur) == false) {
                return false;
            }
            else {
                if (cur != null)
                    cur = cur.Previous;
            }
		}

		return true;
	}

	public string MotionName {
		get {
			return _motionName;
		}
	}
}
