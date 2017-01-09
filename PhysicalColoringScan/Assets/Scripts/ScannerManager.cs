using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerManager : MonoBehaviour {
    [SerializeField]
    private RenderTextureCamera rtc;
    [SerializeField]
    private RawImage OutputImage;

    public int testPlayerID;
    public int testObjectID;
    Texture2D FrameTexture;

    void Start () {
	}

    public void OnClickScan()
    {
        RenderTexture RendTex = rtc.GetRenderTexture();

        FrameTexture = new Texture2D(RendTex.width, RendTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = RendTex;
        FrameTexture.ReadPixels(new Rect(0, 0, RendTex.width, RendTex.height), 0, 0);
        RenderTexture.active = null;

        FrameTexture.Apply();


    }
    public void OnClickSend()
    {

        ObjectToTransfer obj = new ObjectToTransfer(testPlayerID, testObjectID, FrameTexture);
        OutputImage.texture = obj.GetTexture();
        ClientSocketAsync.singlton.SendObject(obj);
    }
}
