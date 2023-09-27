
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueSystem : MonoBehaviour
{

    
    public Conversations provider;
    public GameObject canvas;
    private int index;
    void Start()
    {
        Run(index = 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Run(index++);
        }

        
    }

    private void Run(int i)
    {
            
        
            var l = provider.lines[i];
            if (l.sprite != null)
            {
                canvas.transform.Find("SP").GetComponent<Image>().sprite = l.sprite;
            canvas.transform.Find("SP").GetComponent<Image>().SetNativeSize();
            }

            canvas.transform.Find("BackText").Find("Text").GetComponent<TextMeshProUGUI>().text = l.text;
    }
}
