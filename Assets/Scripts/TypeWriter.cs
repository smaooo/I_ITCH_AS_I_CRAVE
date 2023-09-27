using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = System.Random;
public class TypeWriter : MonoBehaviour
{
    public List<AudioClip> clips;
    private Rand rand = new Rand();

    public void PlaySound()
    {
        AudioClip clip = clips[rand.Next(clips.Count)];
        
        StartCoroutine(Play(clip));
    }

    private IEnumerator Play(AudioClip clip)
    {
        

        GameObject source = new GameObject();
        source.transform.SetParent(transform);
        source.AddComponent<AudioSource>();
        source.GetComponent<AudioSource>().loop = false;
        source.GetComponent<AudioSource>().playOnAwake = false;
        source.GetComponent<AudioSource>().volume = 0.5f;

        source.GetComponent<AudioSource>().clip = clip;
        source.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(clip.length);
        Destroy(source);

        
    }
}
