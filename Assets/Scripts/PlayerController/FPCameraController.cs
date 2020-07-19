using UnityEngine;

namespace Controllers {
	public class FPCameraController : MonoBehaviour {
   
		[SerializeField] private float mouseSensitivity;
	
		[SerializeField] private float maxXTilt = 80f;
		[SerializeField] private float minXTilt = -80f;
		[SerializeField] private bool invertMouseYRot;
	
		private float _totalXRotCache;
		private Vector2 _inputCameraRotation;

		private void LateUpdate() {
			var mouseInput = ReadMouseInput();
			_inputCameraRotation = ConvertMouseInputToCameraAxisRotations(mouseInput);
			AccountForMouseYInversion();
			CacheTotalXRotation();
			ClampCameraXRotToTiltConstraints();
			ApplyRotation();
		}

		private Vector2 ReadMouseInput() {
			var xMouseInputRot = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
			var yMouseInputRot = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
			return new Vector2(xMouseInputRot, yMouseInputRot);
		}

		private Vector2 ConvertMouseInputToCameraAxisRotations(Vector2 mouseInput) {
			return new Vector2(mouseInput.y, mouseInput.x);
		}

		private void AccountForMouseYInversion() {
			_inputCameraRotation.x = invertMouseYRot ? _inputCameraRotation.x : -_inputCameraRotation.x;
		}

		private void CacheTotalXRotation() {
			_totalXRotCache += _inputCameraRotation.x;
		}
	
		private void ApplyRotation() {
			var localCameraRotation = transform.localEulerAngles;
			var totalXRot = localCameraRotation.x + _inputCameraRotation.x;
			var totalYRot = localCameraRotation.y + _inputCameraRotation.y;
			transform.localRotation = Quaternion.Euler(totalXRot,totalYRot,localCameraRotation.z);
		}

		private void ClampCameraXRotToTiltConstraints() {
			//camera wants to look too low
			if (_totalXRotCache > maxXTilt) {
				_inputCameraRotation.x -= _totalXRotCache - maxXTilt;
				_totalXRotCache = maxXTilt;
			}
			//camera wants to look too high
			if (_totalXRotCache < minXTilt) {
				_inputCameraRotation.x += minXTilt - _totalXRotCache;
				_totalXRotCache = minXTilt;
			}
		}

		private void OnValidate() {
			maxXTilt = Mathf.Clamp(maxXTilt, 0f, 90f);
			minXTilt = Mathf.Clamp(minXTilt, -90f, 0);
		}
	}
}
