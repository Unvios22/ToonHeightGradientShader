using UnityEngine;

public class CameraDepthModeSetter : MonoBehaviour {
   [SerializeField] private DepthTextureMode cameraDepthMode;
   private void Start() {
      gameObject.GetComponent<Camera>().depthTextureMode = cameraDepthMode;
   }
}
