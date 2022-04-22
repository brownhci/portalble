using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraProjectionRemap : MonoBehaviour {

	public Matrix4x4 originalProjection;
	private Camera cam;
	private Matrix4x4 m;

	void Start () {
		cam = Camera.main;

		m[0, 0] = 1993.81178F;
		m[0, 1] = 0F;
		m[0, 2] = 1601.56481F;
		m[0, 3] = 0F;
		m[1, 0] = 0F;
		m[1, 1] = 1989.69227F;
		m[1, 2] = 908.462534F;
		m[1, 3] = 0F;
		m[2, 0] = 0F;
		m[2, 1] = 0F;
		m[2, 2] = 1.00000000F;
		m[2, 3] = 0F;
		m[3, 0] = 0F;
		m[3, 1] = 0F;
		m[3, 2] = 0F;
		m[3, 3] = 0F;
	}
	
	// Update is called once per frame
	void Update () {
		cam.projectionMatrix = m;
	}
}
