using UnityEngine;

public class PlayerActiveCar : MonoBehaviour
{
	[SerializeField] private WheelCollider frontLeftW, frontRightW, rearLeftW, rearRightW;
	[SerializeField] private Transform frontLeftT, frontRightT, rearLeftT, rearRightT;

	private float _motorForce;
	private float _maxSteerAngle;
	private float _restoreDirectionSpeed;

	private float _horizontalInput;
	private float _steeringAngle;

	private float _spring;
	private float _damper;
	private float _targetPos;

	private bool _isActive;

	public void Launch(PlayerAccountConfig playerAccountConfig)
	{
		_motorForce = playerAccountConfig.CarMotorForce;
		_maxSteerAngle = playerAccountConfig.MaxSteerAngle;
		_restoreDirectionSpeed = playerAccountConfig.RestoreDirectionSpeed;	
		GetComponent<Rigidbody>().mass = playerAccountConfig.CarMass;

		SetSuspension(playerAccountConfig);

		_isActive = true;
	}
	private void FixedUpdate()
	{
		if (!_isActive)
        {
			return;
        }

		GetInput();
		Steer();
		RestoreCarOrientation();
		AccelerateAuto();
		UpdateWheelPoses();
	}
	private void GetInput()
	{
		_horizontalInput = Input.GetAxis("Horizontal");
	}

	private void Steer()
	{
		_steeringAngle = _maxSteerAngle * _horizontalInput;
		frontLeftW.steerAngle = _steeringAngle;
		frontRightW.steerAngle = _steeringAngle;
	}
	private void RestoreCarOrientation()
	{
		if (_horizontalInput == 0)
		{
			var diffAngle = transform.rotation.y * _restoreDirectionSpeed;
			frontLeftW.steerAngle = -diffAngle;
			frontRightW.steerAngle = -diffAngle;
		}
	}

	private void AccelerateAuto()
	{
		frontLeftW.motorTorque = _motorForce;
		frontRightW.motorTorque = _motorForce;
	}

	private void UpdateWheelPoses()
	{
		UpdateWheelPose(frontLeftW, frontLeftT);
		UpdateWheelPose(frontRightW, frontRightT);
		UpdateWheelPose(rearLeftW, rearLeftT);
		UpdateWheelPose(rearRightW, rearRightT);
	}

	private void UpdateWheelPose(WheelCollider collider, Transform transform)
	{
		var pos = transform.position;
		var quat = transform.rotation;

		collider.GetWorldPose(out pos, out quat);

		transform.position = pos;
		transform.rotation = quat;
	}

	private void SetSuspension(PlayerAccountConfig playerConfig)
	{
		_spring = playerConfig.Spring;
		_damper = playerConfig.Damper;
		_targetPos = playerConfig.TargetPosition;

		var wheelColliders = new[] { frontLeftW, frontRightW, rearLeftW, rearRightW };
		var suspensionSpring = new JointSpring();
		suspensionSpring.spring = _spring;
		suspensionSpring.damper = _damper;
		suspensionSpring.targetPosition = _targetPos;

		foreach (var wheelCollider in wheelColliders)
		{
			wheelCollider.wheelDampingRate = playerConfig.WheelDampingRate;
			wheelCollider.suspensionDistance = playerConfig.SuspensionDistance;
			wheelCollider.suspensionSpring = suspensionSpring;
		}
	}
}
