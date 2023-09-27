using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Rand = System.Random;
using UnityEngine.UI;

public class OfficeCharacter : MonoBehaviour
{
    private Rigidbody characterRb;
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private float speed = 2;
    public bool canMove = false;
    private Animator characterAnim;
    [SerializeField]
    private List<Thoughts> thoughts = new List<Thoughts>();
    private float distance = 0;
    [SerializeField]
    private Transform reception;
    [SerializeField]
    private TextMeshProUGUI thoughtText;
    private Queue<Thought> tText = new Queue<Thought>();
    private Rand rand = new Rand();
    private bool pop = true;
    [SerializeField]
    private Conversations convo;
    [SerializeField]
    private GameObject convoCanvas;
    [SerializeField]
    private TextMeshProUGUI convoText;
    [SerializeField]
    private Image characterSprite;
    [SerializeField]
    private Image shadowSprite;
    private int convoIndex = 0;
    private bool stop = false;
    private bool printing = false;
    private bool inConvo = false;
    private SceneManager sceneManager;
    [SerializeField]
    private RawImage fade;
    [SerializeField]
    private List<GameObject> people;
    [SerializeField]
    private List<ThoughtLine> thoughtLines;


    void Start()
    {
        characterRb = this.GetComponent<Rigidbody>();
        sceneManager = FindObjectOfType<SceneManager>();
        characterRb.Sleep();
        characterAnim = this.GetComponent<Animator>();
        tText = new Queue<Thought>(thoughts.Where(t => t.name == "walk").SingleOrDefault().thoughts.ToList());
        foreach (GameObject g in people)
        {
            int ran = rand.Next(0, 4);
            if (ran == 0)
            {
                g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BackLeft");
                g.transform.GetChild(0).GetComponent<Animator>().speed = Random.Range(0.6f, 1.2f);

            }
            else if (ran == 1)
            {
                g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("BackRight");
                g.transform.GetChild(0).GetComponent<Animator>().speed = Random.Range(0.6f, 1.2f);

            }
            else if (ran == 2)
            {
                g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("FrontLeft");
                g.transform.GetChild(0).GetComponent<Animator>().speed = Random.Range(0.6f, 1.2f);

            }
            else
            {
                g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("FrontRight");
                g.transform.GetChild(0).GetComponent<Animator>().speed = Random.Range(0.6f, 1.2f);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            foreach (GameObject g in people)
            {
                Quaternion rotation = Quaternion.LookRotation(this.transform.position - g.transform.position, g.transform.up);
                g.transform.localRotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (inConvo && !printing)
            {
                convoIndex++;
                if (convoIndex < convo.lines.Count)
                {
                    RunConvo();
                }
                else
                {
                    convoCanvas.SetActive(false);
                    StartCoroutine(Fade("Out"));
                }

            }
            else if (printing)
            {
                stop = true;
            }

        }

    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();

        }
    }

    private void ClearText()
    {
        thoughtText.text = "";
    }
    private void Move()
    {
        velocity = this.transform.forward * (Input.GetAxisRaw("Vertical") > 0 ? Input.GetAxisRaw("Vertical") : 0) * speed;

        if (velocity != Vector3.zero)
        {
            distance = Vector3.Distance(this.transform.position, reception.position);
            //print(distance);
            if ((int)(distance * 3.5) % 13 == 0 && pop)
            {

                if (tText.Count > 0)
                {

                    //thoughtText.text += string.Concat(Enumerable.Repeat(" ", rand.Next(10))) + tText.Dequeue().text + "\n";
                    StartCoroutine(BringThought(tText.Dequeue().text));
                    pop = false;
                    //if (tText.Count == 0)
                    //{
                    //    Invoke("ClearText", 0.5f);
                    //}

                }

            }

            if ((int)(distance * 3.5) % 11 == 0 && !pop)
            {
                pop = true;
            }
            characterAnim.SetTrigger("BackWalk");
            characterAnim.ResetTrigger("Idle");
            characterRb.WakeUp();
            characterRb.velocity = velocity;
        }
        else
        {
            characterAnim.SetTrigger("Idle");
            characterAnim.ResetTrigger("BackWalk");
            characterRb.Sleep();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "ReceptionTrigger")
        {
            tText = new Queue<Thought>(thoughts.Where(t => t.name == "convo").SingleOrDefault().thoughts.ToList());
            RunConvo();
            velocity = Vector3.zero;
            characterRb.Sleep();
            characterAnim.ResetTrigger("BackWalk");
            characterAnim.SetTrigger("Idle");
            inConvo = true;
            canMove = false;

        }
    }

    private void RunConvo()
    {
        convoText.text = "";
        if (!convoCanvas.activeSelf)
        {
            convoCanvas.SetActive(true);

        }


        switch (convo.lines[convoIndex].speaker)
        {
            case Speaker.Character:
                characterSprite.color = new Color(1, 1, 1, 1);
                shadowSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                characterSprite.sprite = convo.lines[convoIndex].sprite;
                if (tText.Count > 0)
                {
                    //thoughtText.text += string.Concat(Enumerable.Repeat(" ", rand.Next(10))) + tText.Dequeue().text + "\n";
                    StartCoroutine(BringThought(tText.Dequeue().text));
                }

                break;
            case Speaker.Shadow:
                shadowSprite.color = new Color(1, 1, 1, 1);
                characterSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                shadowSprite.sprite = convo.lines[convoIndex].sprite;
                break;
        }
        StartCoroutine(TypeLetters(convo.lines[convoIndex].text));

    }


    private IEnumerator TypeLetters(string text)
    {
        printing = true;
        foreach (char c in text)
        {
            if (stop)
            {
                stop = false;
                convoText.text = text;
                break;
            }
            FindObjectOfType<TypeWriter>().PlaySound();
            convoText.text += c.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        printing = false;

    }

    public IEnumerator Fade(string to)
    {
        float timer = 0;
        Color target = new Color();
        switch (to)
        {
            case "In":
                target = new Color(1, 1, 1, 0);

                break;

            case "Out":
                target = new Color(1, 1, 1, 1);

                break;
        }

        while (fade.color != target)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            fade.color = Color.Lerp(fade.color, target, timer);
        }
        switch (to)
        {
            case "In":
                canMove = true;
                break;

            case "Out":
                sceneManager.ActivateMainAfterOffice();
                break;


        }

    }

    private IEnumerator BringThought(string text)
    {
        ThoughtLine line = null;

        yield return new WaitUntil(() => thoughtLines.Where(l => !l.inUse).FirstOrDefault());
        line = thoughtLines.Where(l => !l.inUse).FirstOrDefault();
        line.inUse = true;
        Color color = line.GetComponent<TextMeshProUGUI>().color;
        line.GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, Random.Range(0.5f, 1.1f));
        Vector3 target = Vector3.zero;
        Transform lineTrans = line.transform;
        switch (line.side)
        {
            case ThoughtLine.Side.Left:
                if (!line.reverse)
                {
                    target = new Vector3(1374f, lineTrans.localPosition.y, 0);

                }
                else
                {
                    target = new Vector3(-4000f, lineTrans.localPosition.y, 0);

                }
                break;

            case ThoughtLine.Side.Right:
                if (!line.reverse)
                {
                    target = new Vector3(-4000, lineTrans.localPosition.y, 0);

                }
                else
                {
                    target = new Vector3(1374f, lineTrans.localPosition.y, 0);

                }
                break;
        }
        line.GetComponent<TextMeshProUGUI>().text = text;
        while (Mathf.Abs(lineTrans.localPosition.x - target.x) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            lineTrans.localPosition = Vector3.MoveTowards(lineTrans.localPosition, target, 300 * Time.deltaTime);
        }

        line.reverse = !line.reverse;
        line.inUse = false;
        line.GetComponent<TextMeshProUGUI>().text = "";
        //GameObject tt = Instantiate(thoughtTextCanvas, canvas);
        //Color color = tt.GetComponent<TextMeshProUGUI>().color;

        //tt.GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, Random.Range(0.5f, 1.1f));
        //float textTarget = 0;
        //int ran = rand.Next(0, 2);
        //if (ran == 0)
        //{
        //    textTarget = 1374f;
        //    do
        //    {
        //        tt.transform.localPosition = new Vector3(tt.transform.localPosition.x, Random.Range(-1, 2) * Random.Range(0, 336), 0);

        //    }
        //    while (tt.transform.localPosition.y == 0);
        //}
        //else
        //{
        //    textTarget = tt.transform.localPosition.x;
        //    do
        //    {

        //        tt.transform.localPosition = new Vector3(1374f, Random.Range(-1, 2) * Random.Range(0, 336), 0);
        //    }
        //    while (tt.transform.localPosition.y == 0);
        //}
        //tt.GetComponent<TextMeshProUGUI>().text = text;
        //yield return null;

        //float timer = 0;
        //Vector3 tPos = new Vector3(textTarget, tt.transform.localPosition.y, 0);
        //while(Mathf.Abs(tt.transform.localPosition.x - textTarget) > 0.1f)
        //{
        //    yield return new WaitForEndOfFrame();
        //    timer += Time.deltaTime / 100;
        //    //t.transform.localPosition = Vector3.Lerp(t.transform.localPosition, tPos, timer);
        //    tt.transform.localPosition = Vector3.MoveTowards(tt.transform.localPosition, tPos, 150 *Time.deltaTime);
        //}

        //Destroy(tt);
    }
}
