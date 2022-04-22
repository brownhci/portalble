using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamTextureUI : MonoBehaviour {

	public RawImage rawimage;
	void Start () 
	{
		WebCamDevice[] devices;
		devices = WebCamTexture.devices;

		WebCamTexture webcamTexture = new WebCamTexture();
		webcamTexture.deviceName = devices[0].name;
		rawimage.texture = webcamTexture;
		rawimage.material.mainTexture = webcamTexture;
		webcamTexture.Play();
	}
}
