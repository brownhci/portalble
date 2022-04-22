using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawLine : MonoBehaviour {
	private LineRenderer lineRenderer;
	private DataManager dataManager;
	private float counter;
	private float dist;
	private Vector3 destPositon;
	public float lineSpeed = 6f;

	private Transform dest;
	private Transform origin;

	// Use this for initialization
	void Start () {
		
		dataManager = GameObject.Find ("gDataManager").GetComponent<DataManager> ();
		lineRenderer = GetComponent<LineRenderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		origin = this.transform;
		destPositon = dataManager.getLeftHandPosition ();

		dist = Vector3.Distance (origin.position, destPositon);
		if (dist < 0.25f && !dataManager.checkLeftHandBusy()) {
			lineRenderer.enabled = true;
			lineRenderer.SetPosition (0, origin.position);
			lineRenderer.SetPosition (1, destPositon);	
		} else {
			lineRenderer.enabled = false;
		}
	}
}
