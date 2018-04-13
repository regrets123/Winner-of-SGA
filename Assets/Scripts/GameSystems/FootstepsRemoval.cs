using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsRemoval : Object
{
    RenderTexture splatMap, temp;

    Vector4 coord;

    Material fadeMat;

    float startTime = Time.time;
    

    public FootstepsRemoval(RenderTexture splatMap, RenderTexture temp, Vector4 coord, Material fadeMat)
    {
        this.splatMap = splatMap;
        this.coord = coord;
        this.fadeMat = fadeMat;
        this.temp = temp;
    }

    void Update()
    {
        //if (Time.time < startTime + 15f)
        {
            fadeMat.SetVector("_Color", Vector4.Lerp(fadeMat.GetVector("_Color"), Color.black, 10f));
            Graphics.Blit(splatMap, temp);
            Graphics.Blit(temp, splatMap, fadeMat);
        }
        //else
            //Destroy(this);
    }
}
