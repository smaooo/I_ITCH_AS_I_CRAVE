using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShadowPeopleAnim : MonoBehaviour
{
    [SerializeField]
    private Texture texture;

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("Sprite", texture);

        }
        else
        {

            this.GetComponent<MeshRenderer>().material.SetTexture("Sprite", texture);
        }
    }
}
