using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtPiece : MonoBehaviour {
    public void SetArt(Texture2D UVTex, Mesh MeshToUse)
    {
        GetComponent<Renderer>().material.mainTexture = UVTex;
        GetComponent<MeshFilter>().mesh = MeshToUse;
    }

    public void UpdatePosition(Vector2 Axis)
    {
        Vector3 Pos = transform.position;
        Pos.x += Axis.x * Time.deltaTime;
        Pos.y += Axis.y * Time.deltaTime;
        transform.position = Pos;
    }

}
