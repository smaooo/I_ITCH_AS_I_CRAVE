using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Rand = System.Random;
using UnityEngine.UI;

public class ApartmentCharacter : MonoBehaviour
{
    private Rigidbody characterRb;
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private Transform animCam;
    private Animator animaCamAnimator;
    [SerializeField]
    private float speed = 2;
    private float rotX = 0;
    private float rotY = 180;
    [SerializeField]
    private float CameraRotationSense = 20;
    [SerializeField]
    private Vector2 MinMaxAngle = new Vector2(-50f, 50f);
    private bool canMove = false;
    private Vector3 lastCamPos = Vector3.zero;
    private Quaternion lastCamRot = Quaternion.identity;
    private ApartmentManager manager;
    [SerializeField]
    private Transform taskZoom;
    [SerializeField]
    private List<Sprite> mirrorSprites;
    [SerializeField]
    private List<Thoughts> thoughts;
    [SerializeField]
    private Transform canvas;
    private Rand rand = new Rand();
    [SerializeField]
    private Transform mirror;
    [SerializeField]
    private Shader mirrorShader;
    [SerializeField]
    private Transform mirrorZoom;
    [SerializeField]
    private Transform mirrorZoom2;
    [SerializeField]
    private Texture originalMirrorText;
    private bool[] triggers = new bool[4];
    [SerializeField]
    private GameObject cat;
    [SerializeField]
    private RawImage fade;
    [SerializeField]
    private GameObject nextCamera;
    private SceneManager sceneManager;
    private bool set = false;
    private GameObject collisionObject = null;
    [SerializeField]
    private GameObject tableTrigger;
    [SerializeField]
    private GameObject fridgeTrigger;
    [SerializeField]
    private GameObject mirrorTrigger;
    [SerializeField]
    private List<ThoughtLine> thoughtLines;


    void Start()
    {
        RenderSettings.fog = false;
        Cursor.lockState = CursorLockMode.Locked;
        characterRb = this.GetComponent<Rigidbody>();
        characterRb.Sleep();
        animaCamAnimator = animCam.GetComponent<Animator>();
        manager = FindObjectOfType<ApartmentManager>();
        sceneManager = FindObjectOfType<SceneManager>();
        animaCamAnimator.SetTrigger("Roll");
    }


    void Update()
    {
        if (!set &&animaCamAnimator.GetCurrentAnimatorStateInfo(0).IsName("ApartmentCamAfter"))
        {
            
            animaCamAnimator.gameObject.SetActive(false);
            this.transform.Find("Camera").gameObject.SetActive(true);
            set = true;
            canMove = true;
        }
        
        
        if (triggers[0] && triggers[1] && triggers[2] && !cat.activeSelf)
        {
            cat.SetActive(true);
        }

        RayCastObjects();
        if (cat.activeSelf)
        {
            Quaternion rotation = Quaternion.LookRotation(this.transform.position - cat.transform.position, cat.transform.up);
            cat.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
            
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
            RotateCamera();

        }
    }
    private void Move()
    {
        velocity = Input.GetAxisRaw("Vertical") * this.transform.forward + Input.GetAxisRaw("Horizontal") * this.transform.right;

        if (velocity != Vector3.zero)
        {
            characterRb.WakeUp();
            characterRb.velocity = velocity * speed;

        }
        else
        {
            characterRb.Sleep();
        }
    }

    private void RotateCamera()
    {
        rotX += -Input.GetAxis("MouseY") * CameraRotationSense * Time.deltaTime;

        rotX = ClampAngle(rotX, (float)MinMaxAngle.x, (float)MinMaxAngle.y);

        rotY += Input.GetAxis("MouseX") * CameraRotationSense * Time.deltaTime;

        Camera.main.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0, rotY, 0);


    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.name)
        {
            case "FridgeTrigger":
                collisionObject = other.gameObject;
                break;

            case "TableTrigger":
                collisionObject = other.gameObject;
                break;

            case "MirrorTrigger":
                collisionObject = other.gameObject;
                break;

            case "DoorTrigger":
                if (triggers.All(trigger => trigger))
                {
                    StartCoroutine(ExitHouse());
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collisionObject = null;
    }
    private IEnumerator ZoomCam(string zoom)
    {
        canMove = false;
        velocity = Vector3.zero;
        characterRb.Sleep();
        yield return null;
        Transform cam = Camera.main.transform;
        lastCamPos = cam.position;
        lastCamRot = cam.localRotation;
        float timer = 0;
        Vector3 target = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        List<string> objectThoughts = new List<string>();
        List<Thought> t = new List<Thought>();
        switch (zoom)
        {
            case "Table":

                triggers[1] = true;
                tableTrigger.gameObject.SetActive(false);
                switch (ChosenDependency.chosenDependency)
                {
                    case ChosenDependency.Dependency.Family:
                        t = thoughts.Where(obj => obj.name == "family").SingleOrDefault().thoughts.ToList();

                        break;

                    case ChosenDependency.Dependency.Friend:
                        t = thoughts.Where(obj => obj.name == "friends").SingleOrDefault().thoughts.ToList();

                        break;

                    case ChosenDependency.Dependency.Partner:
                        t = thoughts.Where(obj => obj.name == "partner").SingleOrDefault().thoughts.ToList();

                        break;

                    case ChosenDependency.Dependency.Home:
                        t = thoughts.Where(obj => obj.name == "home").SingleOrDefault().thoughts.ToList();

                        break;

                    case ChosenDependency.Dependency.Knife:
                        t = thoughts.Where(obj => obj.name == "knife").SingleOrDefault().thoughts.ToList();

                        break;

                    case ChosenDependency.Dependency.Pet:
                        t = thoughts.Where(obj => obj.name == "pet").SingleOrDefault().thoughts.ToList();

                        break;
                }
                Queue<Thought> tQ = new Queue<Thought>(t);

                target = manager.CurrentObject.transform.Find("Zoom").transform.position;
                rotation = manager.CurrentObject.transform.Find("Zoom").transform.rotation;
                while (Vector3.Distance(cam.position, target) > 0.1f)
                {
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                    cam.position = Vector3.Lerp(cam.position, target, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
                    cam.rotation = Quaternion.Lerp(cam.rotation, rotation, Mathf.Log10(timer));
                    if ((int)(Vector3.Distance(cam.position, target)*10) % 2 == 0)
                    {
                        if (tQ.Count > 0)
                        {
                            StartCoroutine(BringThought(tQ.Dequeue().text));

                        }

                    }
                }
                
                foreach (Thought text in tQ)
                {
                    
                    StartCoroutine(BringThought(text.text));
                    //yield return new WaitForSeconds(Random.Range(0f, 0.5f));
                }
                yield return new WaitForSeconds(10f);
                //yield return new WaitForSeconds(1);
                timer = 0;
                while (Vector3.Distance(cam.position, lastCamPos) > 0.1f)
                {
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                    cam.position = Vector3.Lerp(cam.position, lastCamPos, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
                    cam.localRotation = Quaternion.Lerp(cam.localRotation, lastCamRot, Mathf.Log10(timer));
                    if (Vector3.Distance(cam.position, lastCamPos) < 0.1f)
                    {
                        cam.position = lastCamPos;
                        cam.localRotation = lastCamRot;
                        break;
                    }
                }
                break;

            case "Fridge":
                triggers[0] = true;
                fridgeTrigger.gameObject.SetActive(false);

                target = taskZoom.position;
                t = thoughts.Where(obj => obj.name == "fridge").SingleOrDefault().thoughts.ToList();
                Queue<Thought> tQ2 = new Queue<Thought>(t);
                while (Vector3.Distance(cam.position, target) > 0.1f)
                {
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                    cam.position = Vector3.Lerp(cam.position, target, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
                    cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(0, 90, 0), Mathf.Log10(timer));
                    if ((int)(Vector3.Distance(cam.position, target) * 10) % 2 == 0)
                    {
                        if (tQ2.Count > 0)
                        {
                            StartCoroutine(BringThought(tQ2.Dequeue().text));

                        }

                    }
                }
                //yield return new WaitForSeconds(0.5f);


                foreach (Thought text in tQ2)
                {
                    //thoughtText.text += string.Concat(Enumerable.Repeat(" ", rand.Next(10))) + text.text + "\n";
                    StartCoroutine(BringThought(text.text));

                    //yield return new WaitForSeconds(Random.Range(0f, 0.5f));
                }
                yield return new WaitForSeconds(10f);
                timer = 0;
                while (Vector3.Distance(cam.position, lastCamPos) > 0.1f)
                {
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                    cam.position = Vector3.Lerp(cam.position, lastCamPos, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
                    cam.localRotation = Quaternion.Lerp(cam.localRotation, lastCamRot, Mathf.Log10(timer));
                    if (Vector3.Distance(cam.position, lastCamPos) < 0.1f)
                    {
                        cam.position = lastCamPos;
                        cam.localRotation = lastCamRot;
                        break;
                    }
                }
                break;

            case "Cat":
                
                target = cam.position + ((cat.transform.position - cam.position) / 2);
                t = thoughts.Where(obj => obj.name == "door").SingleOrDefault().thoughts.ToList();
                Queue<Thought> tQ3 = new Queue<Thought>(t);


                while (Vector3.Distance(cam.position, target)> 0.1f)
                {
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                    cam.position = Vector3.Lerp(cam.position, target, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
                    if ((int)(Vector3.Distance(cam.position, target) * 10) % 2 == 0)
                    {
                        if (tQ3.Count > 0)
                        {
                            StartCoroutine(BringThought(tQ3.Dequeue().text));

                        }

                    }
                }

                foreach (Thought text in tQ3)
                {
                    StartCoroutine(BringThought(text.text));

                    //yield return new WaitForSeconds(Random.Range(0f, 0.5f));
                }
                yield return new WaitForSeconds(10f);
                timer = 0;
                while (Vector3.Distance(cam.position, lastCamPos) > 0.1f)
                {
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;
                    cam.position = Vector3.Lerp(cam.position, lastCamPos, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
                    if (Vector3.Distance(cam.position, lastCamPos) < 0.1f)
                    {
                        cam.position = lastCamPos;
                        break;
                    }

                }
                break;
        }

        canMove = true;
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
                    target = new Vector3(-1111f, lineTrans.localPosition.y, 0);

                }
                break;

            case ThoughtLine.Side.Right:
                if (!line.reverse)
                {
                    target = new Vector3(-1111f, lineTrans.localPosition.y, 0);

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
    private IEnumerator ZoomMirror()
    {
        mirrorTrigger.gameObject.SetActive(false);

        triggers[2] = true;
        canMove = false;
        velocity = Vector3.zero;
        characterRb.Sleep();
        yield return null;
        Transform cam = Camera.main.transform;
        lastCamPos = cam.position;
        lastCamRot = cam.rotation;
        float timer = 0;
        Vector3 target = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        List<string> objectThoughts = new List<string>();
        List<Thought> t = new List<Thought>();
        while (cam.rotation != rotation)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            cam.position = Vector3.Lerp(cam.position, mirrorZoom.position, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
            cam.rotation = Quaternion.Lerp(cam.rotation, rotation, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
        }

        t = thoughts.Where(obj => obj.name == "mirror").SingleOrDefault().thoughts.ToList();
        bool secondZoom = false;
        timer = 0;
        Material mirrorMat = mirror.GetComponent<MeshRenderer>().materials.Where(mat => mat.shader == mirrorShader).SingleOrDefault();
        foreach (Thought text in t)
        {
            //thoughtText.text += string.Concat(Enumerable.Repeat(" ", rand.Next(10))) + text.text + "\n";
            StartCoroutine(BringThought(text.text));

            //yield return new WaitForSeconds(Random.Range(0f, 0.5f));
            if (text.sptite != null && secondZoom)
            {
                mirrorMat.SetTexture("Sprite", text.sptite.texture);
            }
            if (secondZoom)
            {
                yield return new WaitForSeconds(0.5f);

            }
            if (text.sptite != null)
            {
                if (!secondZoom)
                {
                    while (Vector3.Distance(cam.position, mirrorZoom2.position) > 0.1f)
                    {
                        yield return new WaitForEndOfFrame();
                        timer += Time.deltaTime;
                        cam.position = Vector3.Lerp(cam.position, mirrorZoom2.position, Mathf.SmoothStep(0, 1, Mathf.Log10(timer*3)));
                        if (Vector3.Distance(cam.position, mirrorZoom2.position) > 0.4f && Vector3.Distance(cam.position, mirrorZoom2.position) < 0.5f)
                        {
                            mirrorMat.SetTexture("Sprite", text.sptite.texture);

                        }

                    }
                        secondZoom = true;
                }

            }
        }
        this.transform.DetachChildren();
        timer = 0;
        this.transform.position = new Vector3(cam.position.x, this.transform.position.y, this.transform.position.z);
        this.transform.rotation = Quaternion.Euler(0, -90, 0);
        rotX = 0;
        rotY = -90;
        lastCamPos = new Vector3(cam.position.x, lastCamPos.y, lastCamPos.z);
        while (Vector3.Distance(cam.position, lastCamPos) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            cam.position = Vector3.Lerp(cam.position, lastCamPos, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));
            if (Vector3.Distance(cam.position, lastCamPos) > 0.4f && Vector3.Distance(cam.position, lastCamPos) < 0.6f)
            {
                
                mirrorMat.SetTexture("Sprite", originalMirrorText);
            }

            //if (Vector3.Distance(cam.position, lastCamPos) < 0.1f)
            //{
            //    cam.position = lastCamPos;
            //}
        }
        cam.SetParent(this.transform);
        canMove = true;

    }
    private IEnumerator ExitHouse()
    {
        yield return null;
        float timer = 0;
        Color color = new Color(0, 0, 0, 1);
        while (fade.color != color)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            fade.color = Color.Lerp(fade.color, color, timer);
        }
        RenderSettings.fog = false;
        yield return new WaitForSeconds(1);
        Camera.main.gameObject.SetActive(false);
        nextCamera.SetActive(true);
        sceneManager.ActivateMainScene();
    }

    private void RayCastObjects()
    {

        // set raycast layer mask
        LayerMask layerMask = LayerMask.GetMask("Objects");
        // Create ray from center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        // Check if character is looking at any of the objects
        bool hitObject = Physics.Raycast(ray, out RaycastHit lookObject, 120, layerMask);

        // if one of the objects in the layer is ray casted
        if (hitObject && lookObject.transform.CompareTag("Cat"))
        {
            StartCoroutine(ZoomCam("Cat"));
            cat.GetComponent<BoxCollider>().enabled = false;
            triggers[3] = true;
        }
        else if (hitObject && lookObject.transform.CompareTag("Objects") && collisionObject != null && collisionObject == tableTrigger)
        {
            collisionObject = null;
            StartCoroutine(ZoomCam("Table"));

        }
        else if (hitObject && lookObject.transform.CompareTag("Note") && collisionObject != null && collisionObject == fridgeTrigger)
        {
            StartCoroutine(ZoomCam("Fridge"));
            collisionObject = null;

        }

        else if (hitObject && lookObject.transform.CompareTag("Mirror") && collisionObject != null && collisionObject == mirrorTrigger)
        {
            StartCoroutine(ZoomMirror());
            collisionObject = null;

        }
    }

   
    
}
