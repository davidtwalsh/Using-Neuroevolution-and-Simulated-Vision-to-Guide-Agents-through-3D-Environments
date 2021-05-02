using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTextureRenderer : MonoBehaviour
{
    private Camera myCamera;
    private bool takeScreenshotOnNextFrame;
    public AgentRedLight agent;


    private void Awake()
    {
        myCamera = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeScreenshot(8,8);
        }
        */
        if (agent.renderResult == null)
        {
            TakeScreenshot(4, 4);
        }
    }

    private void TakeScreenshot(int width, int height)
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
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

            agent.renderResult = renderResult;
            //changes here
            //TextureScale.Bilinear(renderResult, 32, 16);
            //Color[] colors = renderResult.GetPixels();
            //Debug.Log(colors.Length);
            //end changes
            /*
            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenshots/screenshot" + fileCounter + ".png", byteArray);
            Debug.Log("Saved Camera Screenshot");
            fileCounter++;
            */

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }

}
