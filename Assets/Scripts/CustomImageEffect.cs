using System;
using UnityEngine;
 
[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour {
 
	public Material material;

	private void Start() {
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, material);
	}
}