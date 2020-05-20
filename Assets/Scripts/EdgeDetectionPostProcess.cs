using System;
using UnityEngine;
using UnityEngine.Rendering;
 
[Serializable]
public sealed class EdgeDetectionPostProcess : Volume
{
	[Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
	public FloatParameter blend = new FloatParameter { value = 0.5f, overrideState = false};
}
 
public sealed class GrayscaleRenderer : Volume<EdgeDetectionPostProcess>
{
	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Grayscale"));
		sheet.properties.SetFloat("_Blend", settings.blend);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
}