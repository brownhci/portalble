using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It defines an interface that is used for hand motion matching. Each Item should contain a method called match() to tell the manager if this part of motion is matched.
/// </summary>
public interface IHandMatchItem {
	bool isMatch (ref LinkedListNode<HandActionItem> currentAction);
}