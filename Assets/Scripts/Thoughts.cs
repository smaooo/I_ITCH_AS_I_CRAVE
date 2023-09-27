using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Thought
{
 
    [TextArea(1, 10)]
    public string text;
    public Sprite sptite;

}

[CreateAssetMenu(fileName = "New Thought", menuName = "Thought")]
public class Thoughts : ScriptableObject
{
    public string name;
    public Thought[] thoughts;
    public Thoughts NextThought;
}
