/*
  This sample code is for demonstrating and testing the functionality
  of Unity Capture, and is placed in the public domain.
*/

using UnityEngine;

public class MultiCam : MonoBehaviour
{
    public int CaptureResolutionWidth = 1920, CaptureResolutionHeight = 1080;
    public Camera CaptureCamera1, CaptureCamera2;

    void Awake()
    {
        Camera RenderToScreenCamera = gameObject.GetComponent<Camera>();
        if (RenderToScreenCamera == null) gameObject.AddComponent<Camera>();
        RenderToScreenCamera.clearFlags = CameraClearFlags.Nothing;
        RenderToScreenCamera.cullingMask = 0;
        RenderToScreenCamera.depth = 100;
        RenderToScreenCamera.useOcclusionCulling = false;
        RenderToScreenCamera.allowHDR = false;
        RenderToScreenCamera.allowMSAA = false;
        RenderToScreenCamera.allowDynamicResolution = false;
        RenderToScreenCamera.stereoTargetEye = StereoTargetEyeMask.None;
        CaptureCamera1.targetTexture = new RenderTexture(CaptureResolutionWidth, CaptureResolutionHeight, 24);
        CaptureCamera2.targetTexture = new RenderTexture(CaptureResolutionWidth, CaptureResolutionHeight, 24);
    }

    void OnPostRender()
    {
        float w = Screen.width, whalf = w/2, h = Screen.height;
        GL.PushMatrix();
        GL.LoadPixelMatrix();
        Graphics.DrawTexture(new Rect(    0, h, whalf, -h), CaptureCamera1.targetTexture);
        Graphics.DrawTexture(new Rect(whalf, h, whalf, -h), CaptureCamera2.targetTexture);
        GL.PopMatrix();
    }
}
