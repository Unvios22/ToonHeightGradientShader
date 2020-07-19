using UnityEngine;

namespace Controllers {
	public class PlayerController : MonoBehaviour {
		[SerializeField] private float moveSpeed = 10;
		[SerializeField] private float jumpForce = 5;
		[SerializeField] private float groundCheckDistance;
		[SerializeField] private FPCameraController playerCamera;
		[SerializeField] private Transform cameraAttachPoint;
		[SerializeField] private Transform groundChecker;
	
		[SerializeField] private bool drawGroundCheckSphere;
	
		private Rigidbody _rigidbody;
		private Transform _playerCameraTransform;
		private bool _isGrounded;
	
		private Vector3 _moveVector;
	
		private void Start() {
			_rigidbody = gameObject.GetComponent<Rigidbody>();
			_playerCameraTransform = playerCamera.transform;
		}
	
		private void Update() {
			ReadMovementInput();
			CheckIfPlayerIsGrounded();
			RealignPlayerToCameraForward();
			MoveCameraWithPlayer();
		}

		private void ReadMovementInput() {
			var userInput = new Vector2 {
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			};
			//clamping so that diagonal move isn't faster than forward/sideways move by themselves
			userInput = Vector2.ClampMagnitude(userInput, 1f);
			userInput *= moveSpeed * Time.deltaTime;

			var moveToApplyThisFrame = ConvertPlayerInputToWorldspace(userInput);
			_moveVector += moveToApplyThisFrame;
			//todo: method name obfuscates function. The method not only reads input, but parses to it to worldspace and add to input cache (in between fixedUpdate calls)
		}

		private Vector3 ConvertPlayerInputToWorldspace(Vector2 playerInput) {
			var playerInputVector3 = new Vector3(playerInput.x, 0f, playerInput.y);
			var worldspaceMove = transform.TransformDirection(playerInputVector3);
			return worldspaceMove;
		}

		private void CheckIfPlayerIsGrounded() {
			_isGrounded = Physics
				.CheckSphere(groundChecker.position, groundCheckDistance, LayerMask.GetMask("Ground"));
		}
	
		private void RealignPlayerToCameraForward() {
			//todo change into generic usage Extension Method
			//realignes player up to upwardsDirection (exactly) and player forward to camera forward (as close as possible)
			var playerCameraForward = _playerCameraTransform.forward;
			var newPlayerRot = Quaternion.LookRotation(Vector3.up, -playerCameraForward)
			                   * Quaternion.AngleAxis(90f, Vector3.right);
			transform.localRotation = newPlayerRot;
		}
	
		private void MoveCameraWithPlayer() {
			_playerCameraTransform.position = cameraAttachPoint.position;
		}
	
		private void FixedUpdate() {
			ApplyPlayerMovement();
			if (Input.GetKey(KeyCode.Space) && _isGrounded) {
				ApplyPlayerJump();
			}
		}

		private void ApplyPlayerMovement() {
			_rigidbody.MovePosition(_rigidbody.position + _moveVector);
			_moveVector = Vector2.zero;
		}

		private void ApplyPlayerJump() {
			_rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
		}
	
		private void OnDrawGizmos() {
			Gizmos.color = Color.red;
			if (drawGroundCheckSphere) {
				Gizmos.DrawWireSphere(groundChecker.position,groundCheckDistance);
			}
		}
	}
}
