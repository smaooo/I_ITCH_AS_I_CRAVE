using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    
    public IEnumerator ZoomCamera(Transform cam, Vector3 target, Vector3 Angle, float Speed)
    {
        float timer = 0;
        while (cam.localPosition != target)
        {

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            cam.localPosition = Vector3.Lerp(cam.localPosition, target, Mathf.SmoothStep(0, 1, Mathf.Log(timer)*Speed));
            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(Angle.x, Angle.y, Angle.z), Mathf.SmoothStep(0, 1, Mathf.Log(timer)*Speed));

            if (cam.localPosition == target)
            {
                break;
            }
        }
        
    }
}
