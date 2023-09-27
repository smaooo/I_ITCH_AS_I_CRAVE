using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
[InitializeOnLoadAttribute]
public static class EditorManager
{
    //public static BlitManager blit;
    //public static Material mat;
    //[SerializeField] public static ForwardRenderer feature;
    //static EditorManager()
    //{
    //    EditorApplication.playModeStateChanged += LogPlayModeState;
    //}
    //private static void LogPlayModeState(PlayModeStateChange state)
    //{
    //    if (SceneManager.GetActiveScene().name == "MG4 - Shape")
    //    {
    //        blit = GameObject.FindObjectOfType<BlitManager>();
        
    //        switch (state)
    //        {
    //            case PlayModeStateChange.EnteredEditMode:
    //                blit.SetActive(false);
    //                break;
    //            case PlayModeStateChange.EnteredPlayMode:
    //                blit.SetActive(true);
    //                break;
    //        }

    //    }
    //}
}