using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeErase : MonoBehaviour, PaintCommand {

	private LineRenderer m_target;

	public StrokeErase (LineRenderer target){
		m_target = target;
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}	

	public void undo(){
		m_target.gameObject.SetActive (true);
	}

	public void redo() {
		m_target.gameObject.SetActive (false);
	}
}
