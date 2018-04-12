using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDeformTracks : MonoBehaviour
{
    [SerializeField]
    Shader drawShader;
    [SerializeField]
    GameObject rightFoot, leftFoot, terrain;
    [SerializeField, Range(0, 500)]
    float brushSize;
    [SerializeField, Range(0, 2)]
    float brushStrength;

    private RenderTexture splatMap;
    private Material sandMaterial, drawMaterial;
    private RaycastHit hit;

    // Use this for initialization
    void Start ()
    {
        drawMaterial = new Material(drawShader);
        drawMaterial.SetVector("_Color", Color.red);

        sandMaterial = terrain.GetComponent<Terrain>().materialTemplate;
        sandMaterial.SetTexture("_Splat", splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat));
	}
	
    void RightFootDeform()
    {
        if(Physics.Raycast(rightFoot.gameObject.transform.position, Vector3.down, out hit))
        {
            drawMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
            drawMaterial.SetFloat("_Strength", brushStrength);
            drawMaterial.SetFloat("_Size", brushSize);
            RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(splatMap, temp);
            Graphics.Blit(temp, splatMap, drawMaterial);
            RenderTexture.ReleaseTemporary(temp);
        }
    }

    void LeftFootDeform()
    {
        if (Physics.Raycast(leftFoot.gameObject.transform.position, Vector3.down, out hit))
        {
            drawMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
            drawMaterial.SetFloat("_Strength", brushStrength);
            drawMaterial.SetFloat("_Size", brushSize);
            RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(splatMap, temp);
            Graphics.Blit(temp, splatMap, drawMaterial);
            RenderTexture.ReleaseTemporary(temp);
        }
    }
}
