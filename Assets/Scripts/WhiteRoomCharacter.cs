using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Unity.Mathematics.math;
using Rand = System.Random;

public class WhiteRoomCharacter : MonoBehaviour
{
    private Rigidbody characterRB; // character rigidbody
    private Vector3 velocity = Vector3.zero; // character velocity
    [SerializeField]
    private float CharacterSpeed = 2f; // character movement speed
    private float[] rotationAngle = new float[2] { 20, 0 }; // character and camera rotation angle
    [SerializeField]
    private float RotationSense = 10f; // character and camera rotation sense
    [SerializeField]
    private Vector2 MinMaxAngle = new Vector2(-80f, 80f); // camera rotation min and max angle
    private Transform characterCamera; // camera attached to the character
    [SerializeField]
    private Stack<KeyCode> inputs = new Stack<KeyCode>(); // to record the last input
    private KeyCode prevLastKey; // last key before current one
    [SerializeField]
    private GameObject sphere; // walking sphere
    [SerializeField]
    private Transform WhiteRoom; // whiteroom transform
    [SerializeField]
    private Volume DoFpostProcessing;
    [SerializeField]
    private Volume postProcessing;
    private float moveDistance = 0; // character's distance from its last position
    private Vector3 lastPos = Vector3.zero; // character's last position
    private Rand rand = new Rand(); // random class for generating integer
    private Transform lastHitObject; // latest raycasted object
    private bool inArea = false; // is player in the area
    [SerializeField]
    private List<GameObject> highlighters = new List<GameObject>(); // Board picture highlighters
    private bool dependencyChosen = false; // if dependency has been chosen
    private CatWhiteRoom cat; // reference to cat class
    private bool transitioning = false;
    private bool canMove = true;
    public Transform catObj;
    public AudioSource source;
    void Start()
    {
        // Lock cursor to screen
        Cursor.lockState = CursorLockMode.Locked;
        // Set character rigidbody
        characterRB = this.GetComponent<Rigidbody>();
        // Set rigidbody to sleep
        characterRB.Sleep();
        // Set camera
        characterCamera = this.transform.GetChild(0);
        // Set character's last position
        lastPos = this.transform.position;

    }




    private void Update()
    {
        if (canMove)
        {

            // Move character
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputs.Push(KeyCode.W);
                Move();

            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                inputs.Push(KeyCode.D);
                Move();

            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                inputs.Push(KeyCode.S);
                Move();

            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                inputs.Push(KeyCode.A);
                Move();

            }


            if (inputs.Count > 0)
            {
                Move();

                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    if (inputs.Contains(KeyCode.W))
                    {
                        KeyCode[] tmp = new KeyCode[inputs.Count];
                        inputs.CopyTo(tmp, 0);

                        inputs.Clear();
                        foreach (KeyCode k in tmp)
                        {
                            if (k != KeyCode.W)
                            {
                                inputs.Push(k);
                            }
                        }
                    }
                }
                else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
                {
                    if (inputs.Contains(KeyCode.D))
                    {
                        KeyCode[] tmp = new KeyCode[inputs.Count];
                        inputs.CopyTo(tmp, 0);
                        inputs.Clear();

                        foreach (KeyCode k in tmp)
                        {
                            if (k != KeyCode.D)
                            {
                                inputs.Push(k);
                            }
                        }
                    }
                }
                else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
                {
                    if (inputs.Contains(KeyCode.S))
                    {
                        KeyCode[] tmp = new KeyCode[inputs.Count];
                        inputs.CopyTo(tmp, 0);
                        inputs.Clear();
                        foreach (KeyCode k in tmp)
                        {
                            if (k != KeyCode.S)
                            {
                                inputs.Push(k);
                            }
                        }

                    }
                }
                else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    if (inputs.Contains(KeyCode.A))
                    {
                        KeyCode[] tmp = new KeyCode[inputs.Count];
                        inputs.CopyTo(tmp, 0);
                        inputs.Clear();
                        foreach (KeyCode k in tmp)
                        {
                            if (k != KeyCode.A)
                            {
                                inputs.Push(k);
                            }
                        }
                    }

                }

            }
            if (!Input.anyKey)
            {
                inputs.Clear();

            }

            // Rotate character and its camera
            if (Input.GetAxis("MouseX") != 0 || Input.GetAxis("MouseY") != 0)
            {
                RotateCharacter();

            }

            //Look at objects
            RayCastObjects();

            // if an object is raycasted, when player hit enter button, select that object as the dependency
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (lastHitObject != null && !dependencyChosen)
                {
                    SelectDependency();
                }
            }
        }
    }
    private void LateUpdate()
    {
        ChangePost();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if player has entered int the white room area decrease the minimum camera rotaion angle
        if (other.CompareTag("WhiteRoomArea"))
        {
            MinMaxAngle = new Vector2(-20, MinMaxAngle.y);
            inArea = true;
        }
    }
    // Move character
    private void Move()
    {

        // the basic notion of movement is from https://answers.unity.com/questions/1350307/how-to-move-on-the-surface-of-a-sphere-using-spher.html

        if (!transitioning)
        {

            KeyCode lastKey = inputs.Peek();


            switch (lastKey)
            {
                case KeyCode.W:
                    prevLastKey = lastKey;

                    break;

                case KeyCode.D:
                    prevLastKey = lastKey;

                    break;

                case KeyCode.S:
                    prevLastKey = lastKey;

                    break;

                case KeyCode.A:
                    prevLastKey = lastKey;

                    break;
            }

            // Set velocity
            if (prevLastKey == KeyCode.D || prevLastKey == KeyCode.A)
            {
                velocity = this.transform.right * Input.GetAxisRaw("Horizontal");
            }
            else if (prevLastKey == KeyCode.W || prevLastKey == KeyCode.S)
            {
                velocity = this.transform.forward* Input.GetAxisRaw("Vertical");
            }

            // calculate vector from sphere origin to character
            //Vector3 worldToCharacter = this.transform.position - sphere.transform.position;
            Vector3 worldToCharacter = sphere.transform.position - this.transform.position;
            // calculate prependicular vector to both worldtocharacter and velocity vector
            Vector3 prepVector = Vector3.Cross(worldToCharacter, velocity);

            Vector3 rawNewPos = this.transform.position + velocity;
            Vector3 worldToCharacterNewPos = sphere.transform.position - rawNewPos;
            // keep the new pos distance from the sphere center same as previous
            worldToCharacterNewPos = worldToCharacterNewPos.normalized * worldToCharacter.magnitude;

            // calculate tangent vector with finding the prendiculat vector to both prepvector and worldtoCharacter vectors
            Vector3 tangent = Vector3.Cross(prepVector, worldToCharacterNewPos);

            // set velocity by normalized tangent vector and magnitude of original velocity
            velocity = tangent.normalized * velocity.magnitude;
            
            // If player is pressing moving keys then move
            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            {
                // wake up rigidbody
                characterRB.WakeUp();
                // Add velocity to the rigidbody
                characterRB.velocity = velocity * CharacterSpeed;
            
                // update wold to character vector after movement
                worldToCharacter = this.transform.position - sphere.transform.position;
            
                // rotate character in a way that it is prependicular to the standing point on the sphere surface
                if (prevLastKey == KeyCode.D )
                {
                    transform.rotation = Quaternion.LookRotation(this.transform.forward, worldToCharacter);
                }
                else if (prevLastKey == KeyCode.A)
                {
                    transform.rotation = Quaternion.LookRotation(this.transform.forward, worldToCharacter);
                }
                else if (prevLastKey == KeyCode.W)
                {
                    transform.rotation = Quaternion.LookRotation(tangent, worldToCharacter);
                
                }
                else if (prevLastKey == KeyCode.S)
                {
                    transform.rotation = Quaternion.LookRotation(-tangent, worldToCharacter);
                }

                //float noise = Mathf.PerlinNoise(Random.Range(0f, 1f), Random.Range(0f, 1f));
                //Vector3 upNoise = this.transform.up * (noise + 1) * rand.Next(-1,2) * 2;
                //print(upNoise);
                //characterRB.AddForce(upNoise, ForceMode.Impulse);
            }

            else
            {
                // Set velocity to zero
                characterRB.velocity = Vector2.zero;
                // Sleep character
                characterRB.Sleep();

            }
        }

        else
        {
            StartCoroutine(CutScene());
        }

    }

    private void RotateCharacter()
    {
        // camera rotation around X axis
        rotationAngle[0] += -Input.GetAxis("MouseY") * RotationSense * Time.deltaTime;
        // clamp camera rotation between min and max angle
        rotationAngle[0] = ClampAngle(rotationAngle[0], MinMaxAngle.x, MinMaxAngle.y);
        // character rotation around Y axis
        rotationAngle[1] = Input.GetAxis("MouseX") * RotationSense * Time.deltaTime; 
        //rotationAngle[1] = ClampAngle(rotationAngle[1], -180, 180);
        // apply camera rotation
        characterCamera.localRotation = Quaternion.Euler(rotationAngle[0], characterCamera.localEulerAngles.y, characterCamera.localEulerAngles.z);
        // apply character rotation to rotate around the prependicular vector on surface
        this.transform.RotateAround(transform.position, (transform.position - sphere.transform.position), rotationAngle[1]);
        //this.transform.Rotate(0, rotationAngle[1], 0, Space.Self);
        
    }

    // clamp euler angle between min and max
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void ChangePost()
    {
        if (DoFpostProcessing.profile.TryGet<DepthOfField>(out DepthOfField DoF))
        {
            
            
            //DoF.focalLength.value = Mathf.Clamp(Mathf.Log(Vector3.Distance(this.transform.position, WhiteRoom.position)) * 60, 138,300);
        }

        if (postProcessing.profile.TryGet<LensDistortion>(out LensDistortion Lens))
        {            
            Lens.intensity.value = remap(-1f,-0.5f, -1f, 0f, Mathf.Clamp(Mathf.Log10(Vector3.Distance(this.transform.position, WhiteRoom.position)) / -3, -1, 0));
            if (Lens.scale.value < 0.99f)
            {

                Lens.scale.value = remap(0.5f, 0f, 0.5f, 1f, Vector3.Distance(this.transform.position, WhiteRoom.position) / 3000);
            }
            else
            {
                Lens.scale.value = 1;
            }
            
        }
        
    }

    private void RayCastObjects()
    {
        if (inArea)
        {
            // set raycast layer mask
            LayerMask layerMask = LayerMask.GetMask("WhiteRoom");
            // Create ray from center of the screen
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            // Check if character is looking at any of the objects
            bool hitObject = Physics.Raycast(ray, out RaycastHit lookObject, 120, layerMask);

            // if one of the objects in the layer is ray casted
            if (hitObject && lookObject.transform.CompareTag("WhiteRoomObject"))
            {
                // Make all highlighters disabled
                foreach (GameObject g in highlighters)
                {
                    lastHitObject = null;
                    g.SetActive(false);
                }
                // Highlight raycasted object            
                lookObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                lastHitObject = lookObject.transform;
            }
            if (!hitObject || !lookObject.transform.CompareTag("WhiteRoomObject"))
            {
                // Make all highlighters disabled
                foreach (GameObject g in highlighters)
                {
                    lastHitObject = null;
                    g.SetActive(false);
                }
            }

            //// if there is no raycasted object
            //else if (!hitObject && lastHitObject != null)
            //{
            //    // remove highlight from last hit object
            //    lastHitObject.GetChild(0).GetChild(0).gameObject.SetActive(false);   
            //    lastHitObject = null;
            //}

        }   

    }

    private void SelectDependency()
    {
        canMove = false;

        characterRB.Sleep();
        switch(lastHitObject.name)
        {
            case "Knife":
                ChosenDependency.chosenDependency = ChosenDependency.Dependency.Knife;
                break;

            case "Family":
                ChosenDependency.chosenDependency = ChosenDependency.Dependency.Family;
                break;

            case "Friend":
                ChosenDependency.chosenDependency = ChosenDependency.Dependency.Friend;
                break;

            case "Home":
                ChosenDependency.chosenDependency = ChosenDependency.Dependency.Home;
                break;

            case "Partner":
                ChosenDependency.chosenDependency = ChosenDependency.Dependency.Partner;
                break;

            case "Pet":
                ChosenDependency.chosenDependency = ChosenDependency.Dependency.Pet;
                break;
        }
        dependencyChosen = true;
        print(ChosenDependency.chosenDependency);
        
        StartCoroutine(CutScene());
    }

  

    private IEnumerator CutScene()
    {
        catObj.gameObject.SetActive(true);
        cat = FindObjectOfType<CatWhiteRoom>();

        ComputerScreen screen = FindObjectOfType<ComputerScreen>();
        screen.RemoveText();
        Transform camera = Camera.main.transform;
        transitioning = true;
        float timer = 0;
        Vector3 zoomIn = screen.transform.Find("ZoomIn").position;
        while (camera.position != zoomIn)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            camera.position = Vector3.Lerp(camera.position, zoomIn, Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
            camera.rotation = Quaternion.Lerp(camera.rotation, Quaternion.Euler(0, 0, 0), Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
            if (Vector3.Distance(camera.position, zoomIn) < 0.1f)
            {
                camera.position = zoomIn;
                camera.rotation = Quaternion.Euler(0, 0, 0);
                break;
            }
        }

        yield return new WaitForSeconds(1f);
        screen.ChangeState();

        characterRB.Sleep();
        yield return new WaitUntil(() => screen.changed);

        yield return new WaitForSeconds(2f);

        Quaternion rotation = Quaternion.LookRotation(catObj.position - camera.position, this.transform.up);
        timer = 0;
        while (camera.rotation != rotation)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;

            camera.rotation = Quaternion.Lerp(camera.rotation, rotation, Mathf.SmoothStep(0, 1, Mathf.Log10(timer))/15);

            if (timer > 6)
            {
                camera.rotation = rotation;
                break;
            }
        }
        //yield return new WaitForSeconds(0.4f);
        timer = 0;
        float tTime = 0;
        bool change = true;
        bool change2 = true;
        
        while (camera.position != cat.transform.position)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            tTime += Time.deltaTime;
            if (timer > 7.8f && change)
            {
                cat.ChangeSprite(CatWhiteRoom.State.CAT2);
                change = false;
            }

            if (timer > 8.8f && change2)
            {
                cat.ChangeSprite(CatWhiteRoom.State.CAT3);
                change2 = false;
            }
            camera.position = Vector3.Lerp(camera.position, catObj.Find("ZoomIn").position, timer / 1500f);
            source.volume = Mathf.Lerp(source.volume, 0, timer / 1500f);
            if (tTime > 14)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(3);
                break;
            }
        }
    }
}
