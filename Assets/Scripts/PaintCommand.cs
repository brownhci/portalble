using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PaintCommand {

	void undo ();

	void redo ();

}
