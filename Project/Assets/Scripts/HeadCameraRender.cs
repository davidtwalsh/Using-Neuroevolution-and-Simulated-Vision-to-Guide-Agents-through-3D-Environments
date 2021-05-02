using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCameraRender : MonoBehaviour
{
    public bool takeScreenshotOnNextFrame = false;
    public Camera myCamera;
    public AgentMazeRunner agentMazeRunner;

    private void Awake()
    {
        myCamera = GetComponent<Camera>();
    }
    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame == true)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            //changes here
            TextureScale.Bilinear(renderResult, 16, 8);
            agentMazeRunner.renderResult = renderResult;
            //Color[] colors = renderResult.GetPixels();
            //end changes

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }
}
