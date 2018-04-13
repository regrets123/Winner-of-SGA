using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDeformTracks : MonoBehaviour
{
    [SerializeField]
    Shader drawShader;
    [SerializeField]
    GameObject rightFoot, leftFoot;
    [SerializeField, Range(0, 500)]
    float brushSize;
    [SerializeField, Range(0, 5)]
    float brushStrength;

    private RenderTexture splatMap;
    private Material sandMaterial, drawMaterial;
    private RaycastHit hit;
    RenderTexture temp;

    // Use this for initialization
    void Start ()
    {
        drawMaterial = new Material(drawShader);
        drawMaterial.SetVector("_Color", Color.red);
	}

    void RightFootDeform()
    {
       
        if(Physics.Raycast(rightFoot.gameObject.transform.position, Vector3.down, out hit))
        {
            if (hit.transform.CompareTag("Sand"))
            {
                if (sandMaterial == null)
                {
                    sandMaterial = hit.transform.GetComponent<Terrain>().materialTemplate;
                    sandMaterial.SetTexture("_Splat", splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat));
                }
                drawMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_Strength", brushStrength);
                drawMaterial.SetFloat("_Size", brushSize);
                temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(splatMap, temp);
                Graphics.Blit(temp, splatMap, drawMaterial);
                Material fadeMat = drawMaterial;
                fadeMat.SetFloat("_Strength", drawMaterial.GetFloat("_Strength") * -0.001f);
                //fadeMat.SetVector("_Color", Color.black);
                FootstepsRemoval fader = new FootstepsRemoval(splatMap, temp, drawMaterial.GetVector("_Coordinate"), fadeMat);
                RenderTexture.ReleaseTemporary(temp);                
            }
        }
    }

    void LeftFootDeform()
    {
        if (Physics.Raycast(leftFoot.gameObject.transform.position, Vector3.down, out hit))
        {
            if (hit.transform.CompareTag("Sand"))
            {
                if (sandMaterial == null)
                {
                    sandMaterial = hit.transform.GetComponent<Terrain>().materialTemplate;
                    sandMaterial.SetTexture("_Splat", splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat));
                }
                drawMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_Strength", brushStrength);
                drawMaterial.SetFloat("_Size", brushSize);
                temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(splatMap, temp);
                Graphics.Blit(temp, splatMap, drawMaterial);
                Material fadeMat = drawMaterial;
                fadeMat.SetFloat("_Strength", drawMaterial.GetFloat("_Strength") * -0.001f);
                //fadeMat.SetVector("_Color", Color.black);
                FootstepsRemoval fader = new FootstepsRemoval(splatMap, temp, drawMaterial.GetVector("_Coordinate"), fadeMat);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }
}
