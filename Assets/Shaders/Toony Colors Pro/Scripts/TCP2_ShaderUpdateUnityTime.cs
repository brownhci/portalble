// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

using UnityEngine;

// Script that will update the custom time value for relevant water materials (using "Custom Time" from the Shader Generator)

// This allows:
// - getting the world height position of the wave with the TCP2_GetPosOnWater script
// - syncing to Unity's Time.timeScale value

public class TCP2_ShaderUpdateUnityTime : MonoBehaviour
{
	void LateUpdate()
	{
		Shader.SetGlobalFloat("unityTime", Time.time);
	}
}
