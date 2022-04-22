using UnityEngine;
using System.Collections;


namespace Kalman {
	public interface IKalmanWrapper : System.IDisposable
	{
		Vector3 Update (Vector3 current);
	}
}
