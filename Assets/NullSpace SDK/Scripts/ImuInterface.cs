/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://nullspacevr.com/?wpdmpro=nullspace-developer-agreement
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;
using System.Collections.Generic;


namespace NullSpace.SDK.Tracking
{
	using System;
	using Quaternion = UnityEngine.Quaternion;

	/// <summary>
	/// If you implement this interface and add your calibration script to the NSManager prefab object, 
	/// the SDK will 
	/// </summary>
	public interface IImuCalibrator
	{
		void ReceiveUpdate(TrackingUpdate update);
		Quaternion GetOrientation(Imu imu);
	}

	public class MockImuCalibrator : IImuCalibrator
	{
		public Quaternion GetOrientation(Imu imu)
		{
			return Quaternion.identity;
		}

		public void ReceiveUpdate(TrackingUpdate t)
		{
			//do nothing
		}
	}

	public class CalibratorWrapper : IImuCalibrator
	{
		private IImuCalibrator _calibrator;
		public void SetCalibrator(IImuCalibrator c)
		{
			_calibrator = c;
		}
		public CalibratorWrapper(IImuCalibrator c)
		{
			_calibrator = c;
		}
		public Quaternion GetOrientation(Imu imu)
		{
			return _calibrator.GetOrientation(imu);
		}

		public void ReceiveUpdate(TrackingUpdate update)
		{
			_calibrator.ReceiveUpdate(update);
		}
	}
	/// <summary>
	///	Container for a quaternion representing the rotation of an IMU
	/// </summary>
	public class ImuOrientation
	{
		private Quaternion _orientation;
		public Quaternion Orientation
		{
			get
			{
				return _orientation;
			}

			set
			{
				_orientation = value;
			}
		}

		public ImuOrientation(Quaternion q)
		{
			Orientation = q;
		}
	}

	
}