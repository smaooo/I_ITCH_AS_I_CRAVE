using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public List<GameObject> buttons;
    private int index = 0;
    ColorBlock cb = new ColorBlock();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cb.colorMultiplier = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            index--;
            if (index < 0)
            {
                index = 1;
            }

            foreach (var g in buttons)
            {
                cb = new ColorBlock();
                cb.colorMultiplier = 1;
                cb.normalColor = new Color(1, 1, 1, 1);
                g.GetComponent<Button>().colors = cb;
            }
            cb.normalColor = new Color(1, 0.6352941f, 0, 1);
            buttons[index].GetComponent<Button>().colors = cb;
        }   
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            index++;
            if (index > 1)
            {
                index = 0;
            }
            foreach (var g in buttons)
            {
                cb = new ColorBlock();
                cb.colorMultiplier = 1;
                cb.normalColor = new Color(1, 1, 1, 1);
                g.GetComponent<Button>().colors = cb;
            }
            cb.normalColor = new Color(1, 0.6352941f, 0, 1);
            buttons[index].GetComponent<Button>().colors = cb;
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (index == 0)
            {
                cb.normalColor = new Color(1, 0.3758855f, 0, 1);
                buttons[index].GetComponent<Button>().colors = cb;
                Invoke("StartGame", 0.1f);
            }
            else
            {

                Invoke("QuitGame", 0.1f);
            }
        }
    }

    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);   
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
