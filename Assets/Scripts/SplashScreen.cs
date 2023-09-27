using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private Animator splash;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        splash = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (splash.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //Invoke("SwitchScene", 1);
            SwitchScene();
        }
    }

    private void SwitchScene()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
