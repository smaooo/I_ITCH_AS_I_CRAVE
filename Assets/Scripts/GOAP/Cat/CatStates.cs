using System.Collections.Generic;
using UnityEngine;
// Notion of this system has originated from https://learn.unity.com/tutorial/the-world-states?uv=2019.4&courseId=5dd851beedbc2a1bf7b72bed&projectId=5e0bc1a5edbc2a035d136397#5e0bc61aedbc2a0021fb58ee


[System.Serializable]
public class CatState
{
    public string key;
    public int value;
}
public class CatStates
{
    public Dictionary<string, int> states;
    
    public CatStates()
    {
        states = new Dictionary<string, int>();
    }

    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    public void ModifyState(string key, int value)
    {
        
        if (states.ContainsKey(key))
        {
            states[key] += value;
            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else
        {
            
            states.Add(key, value);
        }
    }
    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
        {
            
            states.Remove(key);
        }
    }

    public void SetState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            states[key] = value;
        }

        else
        {
            states.Add(key, value);
        }
    }

    public Dictionary<string, int> GetStates()
    {
        
        return states;
    }
}
