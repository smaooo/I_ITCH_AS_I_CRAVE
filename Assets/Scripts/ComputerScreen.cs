using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = System.Random;
using System.Linq;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class ComputerScreen : MonoBehaviour
{

    [SerializeField]
    private Texture glitch;
    [SerializeField]
    private Shader shader;
    [SerializeField]
    private List<Texture> glitches = new List<Texture>();
    private Rand rand = new Rand();
    public bool changed = false;
    private TypeWriter typeWriter;
    private void Start()
    {
        typeWriter = FindObjectOfType<TypeWriter>();
        ChangeTexture();
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();

            ChangeTexture();
        }

#endif

    }
    private void ChangeTexture()
    {
        int index = rand.Next(glitches.Count);

        Texture texture = glitches[index];
        
        if (!Application.isPlaying)
        {
            this.GetComponent<MeshRenderer>().sharedMaterials.ToList().Where(mat => mat.shader == shader).SingleOrDefault().SetTexture("_Glitch", texture);

        }
        else
        {


            this.GetComponent<MeshRenderer>().materials.ToList().Where(mat => mat.shader == shader).SingleOrDefault().SetTexture("_Glitch", texture);
            this.GetComponent<MeshRenderer>().materials.ToList().Where(mat => mat.shader == shader).SingleOrDefault().SetTexture("_MainTex", glitch);
        }

        Invoke("ChangeTexture", 0.1f);
    }

    public void ChangeState()
    {
        
        StartCoroutine(Changing());
        
    }
    public void RemoveText()
    {
        this.transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
        this.GetComponent<MeshRenderer>().materials.ToList().Where(mat => mat.shader == shader).SingleOrDefault().SetInt("Change", 1);
        this.GetComponent<Animator>().SetTrigger("Glitch");
    }

    private IEnumerator Changing()
    {
        //this.transform.GetChild(0).GetComponent<TextMeshPro>().text = "TAKE THE CAT, SHE KNOWS THE HAPPY ENDING";
        string text = "TAKE THE CAT, SHE KNOWS THE HAPPY ENDING";
        this.transform.GetChild(0).GetComponent<TextMeshPro>().text = "";


        foreach (char c in text)
        {
            
            string tmp = this.transform.GetChild(0).GetComponent<TextMeshPro>().text;
            tmp = tmp + c.ToString();
            typeWriter.PlaySound();

            if (tmp.Length % 2 == 0)
            {
                //if (c != ' ')
                //{
                //    typeWriter.PlaySound();

                //}

            }
            this.transform.GetChild(0).GetComponent<TextMeshPro>().text = tmp;
            yield return new WaitForSeconds(0.16f);

            

        }
        //yield return new WaitForSeconds(0.5f);
        //this.GetComponent<MeshRenderer>().materials.ToList().Where(mat => mat.shader == shader).SingleOrDefault().SetInt("Change", 0);
        changed = true;
    }
}
