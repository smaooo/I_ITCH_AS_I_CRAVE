using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatWhiteRoom : MonoBehaviour
{
    public enum State { CAT1, CAT2, CAT3}
    [SerializeField]
    private Sprite CAT1;
    [SerializeField]
    private Sprite CAT2;
    [SerializeField]
    private Sprite CAT3;

    public void ChangeSprite(State state)
    {
        switch (state)
        {
            case State.CAT1:
                this.GetComponent<SpriteRenderer>().sprite = CAT1;
                break;

            case State.CAT2:
                this.GetComponent<SpriteRenderer>().sprite = CAT2;
                break;

            case State.CAT3:
                this.GetComponent<SpriteRenderer>().sprite = CAT3;
                break;
        }
    }
}
