using UnityEngine;

namespace Kalman {
	
	/// <summary>
	/// Matrix kalman wrapper.
	/// </summary>
	public class MatrixKalmanWrapper : IKalmanWrapper
	{
		private KalmanFilter kX;
		private KalmanFilter kY;
		private KalmanFilter kZ;
		
		public MatrixKalmanWrapper ()
		{
			/*
			X0 : predicted state
			P0 : predicted covariance
			
			F : factor of real value to previous real value
			B : the control-input model which is applied to the control vector uk;
			U : the control-input model which is applied to the control vector uk;
			Q : measurement noise
			H : factor of measured value to real value
			R : environment noise
			*/
			var f = new Matrix (new[,] {
				{1.0,	2.5},
				{0,		1.0}
			});
			var b = new Matrix (new[,] {{0.0}, {0}});
			var u = new Matrix (new[,] {{0.0}, {0}});
			var r = Matrix.CreateVector (10);
			var q = new Matrix (new[,] {
				{0.01,	0.4},
				{0.1,	0.02}
			});
			var h = new Matrix (new[,] {{1.0 , 0}});
			
			kX = makeKalmanFilter (f, b, u, q, h, r);
			kY = makeKalmanFilter (f, b, u, q, h, r);
			kZ = makeKalmanFilter (f, b, u, q, h, r);
		}
		
		public Vector3 Update (Vector3 current)
		{
			kX.Correct (new Matrix (new double[,] {{current.x}}));
			kY.Correct (new Matrix (new double[,] {{current.y}}));
			kZ.Correct (new Matrix (new double[,] {{current.z}}));
			
			// rashod
			// kX.State [1,0];
			// kY.State [1,0];
			// kZ.State [1,0];
			
			Vector3 filtered = new Vector3 (
				(float)kX.State [0, 0],
				(float)kY.State [0, 0],
				(float)kZ.State [0, 0]
			);
			return filtered;
		}
	
		public void Dispose ()
		{
		
		}
		
		#region Privates
		KalmanFilter makeKalmanFilter (Matrix f, Matrix b, Matrix u, Matrix q, Matrix h, Matrix r)
		{
			var filter = new KalmanFilter (
				f.Duplicate (),
				b.Duplicate (),
				u.Duplicate (),
				q.Duplicate (),
				h.Duplicate (),
				r.Duplicate ()
			);
			// set initial value
			filter.SetState (
				Matrix.CreateVector (500, 0), 
				new Matrix (new [,] {{10.0, 0}, {0, 5.0}})
			);
			return filter;
		}
		#endregion
		
		
		
	}

}
