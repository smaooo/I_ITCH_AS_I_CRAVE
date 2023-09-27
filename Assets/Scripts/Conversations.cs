using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public enum Speaker { Character, Shadow }
[System.Serializable]
public struct Line
{

    public Sprite sprite;
    [TextArea(1, 10)]
    public string text;
    public Speaker speaker;

}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation")]
public class Conversations : ScriptableObject
{

    public string name;
    public List<Line> lines;
    public Conversations nextConversation;
}