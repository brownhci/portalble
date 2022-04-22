using UnityEngine;

namespace Kalman {
	
	/// <summary>
	/// Simple kalman wrapper.
	/// </summary>
	public class SimpleKalmanWrapper : IKalmanWrapper {
		
		private KalmanFilterSimple1D kX;
		private KalmanFilterSimple1D kY;
		private KalmanFilterSimple1D kZ;
				
		public SimpleKalmanWrapper ()
		{
			/*
			X0 : predicted state
			P0 : predicted covariance
			
			F : factor of real value to previous real value
			Q : measurement noise
			H : factor of measured value to real value
			R : environment noise

			*/
			double q = 0.4;
			double r = 10;
			double f = 1.0;
			double h = 1.0;
			
			kX = makeKalmanFilter (q, r, f, h);
			kY = makeKalmanFilter (q, r, f, h);
			kZ = makeKalmanFilter (q, r, f, h);
		}
	
		
		public Vector3 Update (Vector3 current)
		{
			kX.Correct (current.x);
			kY.Correct (current.y);
			kZ.Correct (current.z);
			
			Vector3 filtered = new Vector3 (
				(float)kX.State,
				(float)kY.State,
				(float)kZ.State
			);
			return filtered;
		}
	
		public void Dispose ()
		{
		
		}
		
		#region Privates
		KalmanFilterSimple1D makeKalmanFilter (double q, double r, double f, double h)
		{
			var filter = new KalmanFilterSimple1D (q,r,f,h);
			// set initial value
			filter.SetState (500,5.0);
			return filter;
		}
		#endregion
		
		
	}
}