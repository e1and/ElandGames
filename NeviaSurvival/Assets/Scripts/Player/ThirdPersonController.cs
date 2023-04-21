using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
public class ThirdPersonController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 2.0f;
	public float tiredSwimSpeed = 1f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 5.335f;
	public float SprintInWaterSpeed = 4f;
	[Tooltip("How fast the character turns to face movement direction")]
	[Range(0.0f, 0.3f)]
	public float RotationSmoothTime = 0.12f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.50f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.28f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 70.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -30.0f;
	[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
	public float CameraAngleOverride = 0.0f;
	[Tooltip("For locking the camera position on all axis")]
	public bool LockCameraPosition = false;

	// cinemachine
	private float _cinemachineTargetYaw;
	private float _cinemachineTargetPitch;

	// player
	private float _speed;
	private float _animationBlend;
	private float _targetRotation = 0.0f;
	private float _rotationVelocity;
	[SerializeField] private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDGrounded;
	private int _animIDJump;
	private int _animIDFreeFall;
	private int _animIDMotionSpeed;

	public Animator _animator;
	private CharacterController _controller;
	private StarterAssetsInputs _input;
	private Player _player;
	private Swimming _swimming;
	private GameObject _mainCamera;

	private const float _threshold = 0.01f;

	public bool _hasAnimator;
	[SerializeField] bool isLook;
	[SerializeField] bool isLookLocked;

	Vector3 lastDirection;
	Vector3 targetDirection;
	public Vector3 slopingVelocity;
	float lastSpeed;

	float targetSpeed = 0;

	Links links;

	private void Awake()
	{
		// get a reference to our main camera
		if (_mainCamera == null)
		{
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}
		links = FindObjectOfType<Links>();	

	}

	private void Start()
	{
		//_hasAnimator = TryGetComponent(out _animator);
		_controller = GetComponent<CharacterController>();
		_input = GetComponent<StarterAssetsInputs>();
		_player = GetComponent<Player>();
		_swimming = _player.GetComponentInChildren<Swimming>();

		AssignAnimationIDs();

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
		Cursor.visible = true;
	}

	private void Update()
	{
		if (!_player.isSwim)
		{
			JumpAndGravity();
			GroundedCheck();
		}
		else
		{
			_verticalVelocity = 0;
			Grounded = true;
		}

		Move();

		if (Input.GetMouseButton(1))
		{
			Cursor.visible = false;
			isLook = true;
		}
		else 
		{ 
			Cursor.visible = true; 
			isLook = false; 
		}
	}

	private void LateUpdate()
	{
		CameraRotation();
	}

	private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	}

	public void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetBool(_animIDGrounded, Grounded);
		}
	}

	private void CameraRotation()
	{
		// if there is an input and camera position is not fixed
		if (isLook)
		{ 
			if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
			{
				_cinemachineTargetYaw += _input.look.x * Time.deltaTime;
				_cinemachineTargetPitch += _input.look.y * Time.deltaTime;
			}
		}

		// clamp our rotations so our values are limited 360 degrees
		_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		// Cinemachine will follow this target
		CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);

	}

	public void CustomRotateCamera(float y, float x)
	{
		_cinemachineTargetYaw = y;
		_cinemachineTargetPitch = x;
	}

	Vector3 swimVerticalVelocity;
	Vector3 swimUp;
	Vector3 swimDown;
	private void MoveInWater()
	{
		slopingVelocity = Vector3.zero;

		if (_input.jump && _player.gameObject.transform.position.y < _swimming.waterY - 1.3f)
		{
			swimUp = Vector3.up * MoveSpeed * Time.deltaTime;
		}
		else { _input.jump = false; swimUp = Vector3.zero; }

		if (_input.crouch) swimDown = Vector3.down * MoveSpeed * Time.deltaTime;
		else swimDown = Vector3.zero;

		swimVerticalVelocity = swimUp + swimDown;
	}

	private void Move()
	{
		if (_player.isSwim) MoveInWater();

		// Определение скорости движения (ходьба, бег, плавание, усталость)
		if (Grounded)
		{
			if (_input.sprint && !_player.isAbleRun) links.mousePoint.Comment("Я слишком устал, чтобы бегать!");
			if (_input.sprint && _player.runCoolDown > 0 && !_player.isSwim) links.mousePoint.Comment("Не могу бежать - надо отдышаться!");


			if (_player.isAbleRun && _input.sprint && !links.mousePoint.isCarry && _player.Stamina > 0 && _player.runCoolDown <= 0)
			{
				targetSpeed = SprintSpeed;
				if (_player.isSwim) targetSpeed = SprintInWaterSpeed;
				if (_speed > 0.1f) _player.isRun = true;
				else _player.isRun = false;
			}
			else
			{
				targetSpeed = MoveSpeed;
				_player.isRun = false;
			}
			if (_player.isSwim && _player.Stamina <= 0) targetSpeed = tiredSwimSpeed;
		}

		if (links.mousePoint.isCarry)
		{
			float carryIndex = (_player.maxCarryWeight - links.mousePoint.carryWeight) * 0.01f;
			if (carryIndex < 0) carryIndex = 0; else if (carryIndex < 0.3f) carryIndex = 0.3f; else if (carryIndex > 1) carryIndex = 1;
			targetSpeed *= carryIndex;
			_animator.SetFloat("CarrySpeed", carryIndex);
		}

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_input.move == Vector2.zero) targetSpeed = 0.0f;

		float currentHorizontalSpeed = 0;
		float speedOffset = 0;
		float inputMagnitude = 0;

		// a reference to the players current horizontal velocity
		currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		speedOffset = 0.1f;
		inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}
		_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);


		// normalise input direction
		Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_input.move != Vector2.zero && !_player.isLay && !_player.isSit && _player.isControl)
		{
			_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}

		targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
		if (isLookLocked) targetDirection = lastDirection;

		// move the player
		if (!isLookLocked) lastSpeed = _speed;
		_controller.Move((targetDirection.normalized * lastSpeed + slopingVelocity) * Time.deltaTime + 
			new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime + swimVerticalVelocity);

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetFloat(_animIDSpeed, _animationBlend);
			_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
		}


		Crouch();
	}

	public void Crouch()
    {
		if (_input.crouch)
		{
			_animator.SetBool("Crouch", true);
			_player.GetComponent<CharacterController>().height = 0.8f;
			_player.GetComponent<CharacterController>().center = new Vector3(0, 0.5f, 0);
		}
		else
		{
			_animator.SetBool("Crouch", false);
			_player.GetComponent<CharacterController>().center = new Vector3(0, 1f, 0);
			_player.GetComponent<CharacterController>().height = 1.8f;
		}
	}

	float SphereCastVerticalOffset;
	Vector3 CastOrigin;
	Action OnNextDrawGizmos;
	void OnDrawGizmos()
    {
		OnNextDrawGizmos?.Invoke();
		OnNextDrawGizmos = null;
    }

	float sphereOffsetZ = 0.2f; // Смещение сферы вперед, чтобы при спуске со ступенек она не цеплялась за них
	void SlopeSliding()
    {
		var SphereCastVerticalOffset = _controller.height / 2 - _controller.radius * 5;
		var CastOrigin = transform.position - new Vector3(0, SphereCastVerticalOffset, 0) + transform.forward.normalized * sphereOffsetZ;

		if (Physics.SphereCast(CastOrigin, _controller.radius - .21f, Vector3.down, out var hit, .8f, ~LayerMask.GetMask("Player"),
				QueryTriggerInteraction.Ignore))
        {
			var collider = hit.collider;
			var angle = Vector3.Angle(Vector3.up, hit.normal);

			Debug.DrawLine(hit.point, hit.point + hit.normal, Color.black, 3f);
			OnNextDrawGizmos += () =>
			{
				GUI.color = Color.black;
				//Handles.Label(transform.position + new Vector3(0, 2, 0), "Угол: " + angle.ToString());
			};

			if (angle > _controller.slopeLimit && !_player.isSwim)
			{
				//_player.PlayerControl(false);
				var normal = hit.normal;
				// Скольжкние
				slopingVelocity += Vector3.Reflect(Vector3.down, normal) * .04f;

				//Отскок от стены
				if (hit.collider.gameObject.layer == 9 && angle > 60)
				{
					slopingVelocity += normal * 0.5f;
					StartCoroutine(NoControlInAir());
				}
			}
			else
			{
				slopingVelocity = Vector3.zero;
				//_player.PlayerControl(true);
			}
        }
	}

	IEnumerator JumpSpeed(bool isSprint)
	{
		lastDirection = targetDirection;
		//if (isSprint) targetSpeed = SprintSpeed;
		//else targetSpeed = MoveSpeed;
		targetSpeed = _speed;
		lastSpeed = targetSpeed;
		isLookLocked = true;
		//_player.Stamina -= _player.staminaForJump;
		Debug.Log("Coroutine JumpSpeed");

		isLook = false;

		while (_input.jump || !Grounded)
		{
			yield return null;
		}

		isLookLocked = false;
	}

	// Корутина для отключения управления в полете, отскоке и т.д.
	IEnumerator NoControlInAir()
	{
		_player.isAbleRun = false;
		_player.isAbleJump = false;
		isLookLocked = true;

		isLook = false;
		float timer = 0; // Таймер задержки, чтобы корутина запустилась

		while (!Grounded || timer < 0.5f)
		{
			timer += Time.deltaTime;
			yield return null;
		}

		isLookLocked = false;
		_player.isAbleRun = true;
		_player.isAbleJump = true;
	}

	private void JumpAndGravity()
	{
		if (Grounded && !_player.isSwim)
		{
			
			SlopeSliding();
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDJump, false);
				_animator.SetBool(_animIDFreeFall, false);
			}

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (_input.jump && _jumpTimeoutDelta <= 0.0f && _player.isAbleJump && _player.isControl)
			{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

					StartCoroutine(JumpSpeed(_input.sprint));

					// update animator if using character
					if (_hasAnimator)
					{
						_animator.SetBool(_animIDJump, true);
					}
			}

			if (_input.jump && _player.isControl && !_player.isAbleJump) links.mousePoint.Comment("У меня нет сил прыгать!");

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				// update animator if using character
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDFreeFall, true);
				}
			}

			// if we are not grounded, do not jump
			_input.jump = false;
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
}
