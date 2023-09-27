using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using Rand = System.Random;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour
{
    public enum OnStreetSide { LEFT, RIGHT} // camera side
    public OnStreetSide streetSide = OnStreetSide.LEFT;
    public enum WalkingState { ON_STREET, ON_INTERSECTION, DEADEND} // character state in walking state
    public WalkingState walkingState = WalkingState.ON_STREET;
    private float characterRotation = 0f; // character rotation along y axix
    [SerializeField]
    public float characterSpeed = 2f; // character movement speed    
    [SerializeField] 
    private float characterRotationSense = 100f; // character rotation sense
    [SerializeField]
    private float rotationSpeed = 2f; // character rotation speed
    private Animator characterAnim; // character animator
    [SerializeField]
    private CinemachineVirtualCamera cineMachine; // cinemachine virtual camera
    private Cinemachine3rdPersonFollow followPerson; // cinemachine 3rd person follow component
    private bool rotating = true; // is character currently rotating
    private Dictionary<string, Vector3> intersectionPositions = new Dictionary<string, Vector3>(); // possible positions of the current intersection
    private enum IntersectionDirection { Left, Middle, Right}
    private IntersectionDirection onIntersectionDirection = IntersectionDirection.Middle; // current character's direction toward a street on interction
    public bool canMove = true; // can character move
    private MainCat mainCat; // reference to maincat class
    [SerializeField]
    private int locationIndex = 0; // element index of the current path
    [SerializeField]
    public StreetIntersection intersection; // current instersection 
    private StreetIntersection nextIntersection; // next intersection
    public List<Vector3> path = new List<Vector3>(); // current path
    public List<Vector3> nextPath = new List<Vector3>(); // next path
    private bool onConnection = false; // is character walking on a connection street
    private float offsetAngle; // offset rotation angle 
    private bool deadEnd = false;
    private Dictionary<IntersectionDirection, Vector3> directions = new Dictionary<IntersectionDirection, Vector3>(); // to record left middle and right directions
    private Dictionary<IntersectionDirection, GameObject> dirObjects = new Dictionary<IntersectionDirection, GameObject>();
    private Rand rand = new Rand();
    public bool officeArea = false;
    private SceneManager sceneManager;
    public bool canMoveGeneral = false;
    [SerializeField]
    private RawImage fade;
    public bool badEnding = false;
    [SerializeField]
    private List<Thoughts> thoughts;
    [SerializeField]
    private List<ThoughtLine> thoughtLines;
    private Queue<Thought> thoughtWalkingDepend = new Queue<Thought>();
    private Queue<Thought> thoughtBadEnd = new Queue<Thought>();
    private Queue<Thought> thoughtGoodEnd = new Queue<Thought>();
    public bool inOfficeTrigger = false;
    [SerializeField]
    private GameObject characterCamera;
    [SerializeField]
    private GameObject initialW;
    [SerializeField]
    private GameObject actionW;
    [SerializeField]
    private GameObject officeW;
    public bool firstFade = false;
    private bool toOffice = true;
    public enum CurrentTask { Office, Stall, Dependency}
    public CurrentTask task = CurrentTask.Office;
    public GameObject stallTrigger;
    public GameObject badTrigger;
    public GameObject goodTrigger;
    public bool catSleep = false;
    [SerializeField]
    private List<GameObject> credits = new List<GameObject>();
    private bool ending = false;
    int maxEndingThoughts;

    private void Awake()
    {
        //Lock Cursor to screen
        Cursor.lockState = CursorLockMode.Locked;
        // Set character animator
        characterAnim = this.GetComponent<Animator>();
        // Set 3rd person follow component
        followPerson = cineMachine.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        // set main cat 
        mainCat = FindObjectOfType<MainCat>();
        sceneManager = FindObjectOfType<SceneManager>();
    }

   
    private void Start()
    {

        
        foreach (var v in intersection.Paths)
        {
            Debug.DrawRay(this.transform.position, v.Value - this.transform.position, Color.red, 10);
        }

        //sceneManager.ActivateMainScene();
        // set character position to the center of the intersection 
        this.transform.position = intersection.CenterPoint;
        
        // choose the middle path for the starting intersection
        ChoosePath(intersection.MiddleStreet);
        
        // calculate the offset angle
        offsetAngle = Vector3.Angle(path[(int)characterSpeed] - path[0], this.transform.forward);
        FindDirections(intersection.Paths);
        rotating = false;
        locationIndex++;

        ChooseThoughts();
        intersection.SetStreet();

        nextIntersection.SetGivenStreet(intersection.ConnectionObject);
        nextPath = intersection.CurrentConnection;

        

    }

    private void Update()
    {
        print(rotating);
        if (firstFade)
        {
            StartCoroutine(ShowWarning(initialW, 1));
            firstFade = false;
        }
        if (!badEnding)
        {
            CheckCatDistance();

        }
        if (Input.GetKeyDown(KeyCode.E) && !mainCat.acting)
        {

            if (officeArea)
            {
                mainCat.beleifs.ModifyState("tired", 0);
                catSleep = true;
            }

            else
            {
                if (rand.Next(0, 10) > 5)
                {
                    mainCat.beleifs.ModifyState("thirsty", 0);

                }
                else
                {
                    mainCat.beleifs.ModifyState("hungry", 0);

                }

            }

        }
        if (canMoveGeneral)
        {
            if (canMove)
            {
                // if player is not on an intersection and not rotating move
                if (walkingState == WalkingState.ON_INTERSECTION && !rotating)
                {
                    Move();

                }
                else
                {
                    Move();
                }

            }
            // if player cant move anymore because he has reached a intersection activate movement after the movement button is released
            else
            {
                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    canMove = true;
                }
            }

            // rotate character
            CharacterRotation();

            this.transform.localRotation = Quaternion.Euler(0, this.transform.localEulerAngles.y, 0);
        
        
           

        }
    }
    
    private void ChooseThoughts()
    {
        switch (ChosenDependency.chosenDependency)
        {
            case ChosenDependency.Dependency.Family:
                thoughtWalkingDepend = new Queue<Thought>(thoughts.Where(t => t.name == "familystall").SingleOrDefault().thoughts.ToList());
                thoughtGoodEnd = new Queue<Thought>(thoughts.Where(t => t.name == "goodendfamily").SingleOrDefault().thoughts.ToList());
                thoughtBadEnd = new Queue<Thought>(thoughts.Where(t => t.name == "badendfamily").SingleOrDefault().thoughts.ToList());
                break;

            case ChosenDependency.Dependency.Friend:
                thoughtWalkingDepend = new Queue<Thought>(thoughts.Where(t => t.name == "friendsstall").SingleOrDefault().thoughts.ToList());
                thoughtGoodEnd = new Queue<Thought>(thoughts.Where(t => t.name == "goodendfrineds").SingleOrDefault().thoughts.ToList());
                thoughtBadEnd = new Queue<Thought>(thoughts.Where(t => t.name == "badendfriends").SingleOrDefault().thoughts.ToList());
                break;

            case ChosenDependency.Dependency.Partner:
                thoughtWalkingDepend = new Queue<Thought>(thoughts.Where(t => t.name == "partnerstall").SingleOrDefault().thoughts.ToList());
                thoughtGoodEnd = new Queue<Thought>(thoughts.Where(t => t.name == "goodendpartner").SingleOrDefault().thoughts.ToList());
                thoughtBadEnd = new Queue<Thought>(thoughts.Where(t => t.name == "badendpartner").SingleOrDefault().thoughts.ToList());
                break;

            case ChosenDependency.Dependency.Home:
                thoughtWalkingDepend = new Queue<Thought>(thoughts.Where(t => t.name == "homestall").SingleOrDefault().thoughts.ToList());
                thoughtGoodEnd = new Queue<Thought>(thoughts.Where(t => t.name == "goodendhome").SingleOrDefault().thoughts.ToList());
                thoughtBadEnd = new Queue<Thought>(thoughts.Where(t => t.name == "badendhome").SingleOrDefault().thoughts.ToList());
                break;

            case ChosenDependency.Dependency.Knife:
                thoughtWalkingDepend = new Queue<Thought>(thoughts.Where(t => t.name == "knifestall").SingleOrDefault().thoughts.ToList());
                thoughtGoodEnd = new Queue<Thought>(thoughts.Where(t => t.name == "goodendknife").SingleOrDefault().thoughts.ToList());
                thoughtBadEnd = new Queue<Thought>(thoughts.Where(t => t.name == "badendknife").SingleOrDefault().thoughts.ToList());
                break;

            case ChosenDependency.Dependency.Pet:
                thoughtWalkingDepend = new Queue<Thought>(thoughts.Where(t => t.name == "petstall").SingleOrDefault().thoughts.ToList());
                thoughtGoodEnd = new Queue<Thought>(thoughts.Where(t => t.name == "goodendpet").SingleOrDefault().thoughts.ToList());
                thoughtBadEnd = new Queue<Thought>(thoughts.Where(t => t.name == "badendpet").SingleOrDefault().thoughts.ToList());
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Office") && !sceneManager.officeDone && !badEnding)
        {
            StartCoroutine(ShowWarning(officeW, 0));
            officeArea = true;
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        else if (other.CompareTag("OfficeInTrigger") && !sceneManager.officeDone && !badEnding)
        {
            Destroy(other.gameObject);
            inOfficeTrigger = true;
        }
        else if (other.gameObject == stallTrigger && !badEnding)
        {
            canMoveGeneral = false;
            characterAnim.SetFloat("MoveForward", 0);
            StartCoroutine(RotateToStall());
               
        }
        else if (other.gameObject == goodTrigger)
        {
            StartCoroutine(Ending("Good"));
        }
        else if (other.gameObject == badTrigger)
        {
            StartCoroutine(Ending("Bad"));
        }
    }
    
    private IEnumerator Ending(string tEnding)
    {
        yield return null;
        Color color = new Color(1, 1, 1, 1);
        float timer = 0;
        while (fade.color != color)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            fade.color = Color.Lerp(fade.color, color, timer/5);
        }
        Queue<Thought> end = new Queue<Thought>();
        ending = true;
        switch (tEnding)
        {
            case "Good":
                end = new Queue<Thought>(thoughtGoodEnd);
                break;

            case "Bad":
                end = new Queue<Thought>(thoughtBadEnd);
                break;

        }
        maxEndingThoughts = end.Count;
        foreach (var t in end)
        {
            StartCoroutine(BringThought(t.text));
        }
        yield return new WaitUntil(() => maxEndingThoughts < 3);
        yield return new WaitForSeconds(1);
        StartCoroutine(Credits());
    }

    private IEnumerator Credits()
    {
        foreach (var g in credits)
        {
            g.SetActive(true);
            yield return new WaitForSeconds(2);
            g.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    private IEnumerator RotateToStall()
    {
        yield return null;
        Quaternion prevRot = this.transform.localRotation;
        Vector3 target = stallTrigger.transform.parent.position - transform.position;

        Quaternion quaternion = Quaternion.LookRotation(target, this.transform.up);
        float timer = 0;
        while (this.transform.localRotation != quaternion)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, quaternion, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));

        }

        foreach (var t in thoughtWalkingDepend)
        {
            StartCoroutine(BringThought(t.text));
            yield return new WaitForSeconds(0.3f);
        }
        timer = 0;
        while(this.transform.localRotation != prevRot)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, prevRot, Mathf.SmoothStep(0, 1, Mathf.Log10(timer)));

        }
        canMoveGeneral = true;

    }
    private void OnTriggerExit(Collider other)
    {
    }

    private void CheckCatDistance()
    {
        if (mainCat.currentAction != null && (mainCat.currentAction.actionName != "Eat" || mainCat.currentAction.actionName != "Drink" || mainCat.currentAction.actionName != "Sleep"))
        {
            if (Vector3.Distance(this.transform.position, mainCat.transform.position) > 15f)
            {
                badEnding = true;
                mainCat.gameObject.SetActive(false);
                badTrigger.SetActive(true);

                //Destroy(mainCat);
            }

        }

        //else
        //{
        //    if (Vector3.Distance(this.transform.position, mainCat.transform.position) > 5f)
        //    {
        //        badEnding = true;
        //        mainCat.gameObject.SetActive(false);
        //        //Destroy(mainCat);
        //    }
        //}
    }
    // Calculate look direction vector of character
    public Vector3 LookDirection(bool inverse)
    {
        Vector3 lookDirection = Vector3.zero; // look direction vector
        // index offset for calculating the forward vector
        int offset = (locationIndex + (int)characterSpeed) <= path.Count - 1 ? locationIndex + (int)characterSpeed : (int)characterSpeed - (path.Count - locationIndex);
        List<Vector3> nextTemp = new List<Vector3>();
        if (!deadEnd)
        {
            // get connection street to the current street
            nextTemp = new List<Vector3>(nextPath);
            // reverse the order of the points in the connection street
            nextTemp.Reverse();

        }

        // Find streets on each side in the next intersection
        var paths = intersection.Paths;


        // find the starting point of the next street in the next intersection depending on character's side on street and the number of streets in the next intersection
        Vector3 nextPoint = Vector3.zero;
        if (onConnection)
        {
            switch (paths.Count)
            {
                case 2:
                    switch (streetSide)
                    {
                        case OnStreetSide.LEFT:
                            nextPoint = directions[onIntersectionDirection = IntersectionDirection.Left];
                            
                            break;

                        case OnStreetSide.RIGHT:
                            nextPoint = directions[onIntersectionDirection = IntersectionDirection.Right];
                            break;
                    }
                    break;

                case 3:
                    nextPoint = directions[onIntersectionDirection = IntersectionDirection.Middle];
                    break;
            }

        }

        // if current street is deadend 
        else if (deadEnd)
        {
            nextPoint = path.Last();
        }

        if (task == CurrentTask.Office&& onConnection)
        {
            nextPoint = GameObject.FindGameObjectWithTag("Office").transform.position;
        }
        
        // if inverse rotation is asked
        if (inverse)
        {
            if (streetSide == OnStreetSide.LEFT)
            {
                
                lookDirection = Quaternion.AngleAxis(-offsetAngle,
                    this.transform.up) * ((locationIndex + (int)characterSpeed >= path.Count ? nextTemp[offset] : path[offset]) - this.transform.position).normalized;

            }
            else
            {
                lookDirection = Quaternion.AngleAxis(offsetAngle,
                    this.transform.up) * ((locationIndex + (int)characterSpeed >= path.Count ? nextTemp[offset] : path[offset]) - this.transform.position).normalized;
                
            }
        }
        // if normal rotation is asked
        else
        {

            if (streetSide == OnStreetSide.LEFT)
            {
                if (nextTemp.Count > 0 && offset < nextTemp.Count)
                {

                    lookDirection = Quaternion.AngleAxis(offsetAngle,
                        this.transform.up) * ((locationIndex + (int)characterSpeed >= path.Count ? nextTemp[offset] : path[offset]) - this.transform.position).normalized;

                }

                else
                {
                    
                    
                    lookDirection = Quaternion.AngleAxis(offsetAngle,
                        this.transform.up) * ((locationIndex + (int)characterSpeed >= path.Count ? nextPoint: path[offset]) - this.transform.position).normalized;

                }


            }
            else
            {
                if (nextTemp.Count > 0 && offset < nextTemp.Count)
                {
                    lookDirection = Quaternion.AngleAxis(-offsetAngle,
                        this.transform.up) * ((locationIndex + (int)characterSpeed >= path.Count ? nextTemp[offset] : path[offset]) - this.transform.position).normalized;

                }

                else
                {


                    lookDirection = Quaternion.AngleAxis(-offsetAngle,
                        this.transform.up) * ((locationIndex + (int)characterSpeed >= path.Count ? nextPoint : path[offset]) - this.transform.position).normalized;

                }
            }
        }

        return lookDirection;
    }
    private void Move()
    {
        // Set the move forward parameter in character blend tree
        characterAnim.SetFloat("MoveForward", Input.GetAxisRaw("Vertical") >= 0 ? Input.GetAxisRaw("Vertical") : 0);
        // if character is reaching a deadend
        if (locationIndex == path.Count && deadEnd)
        {
            // set character animator to not moving
            characterAnim.SetFloat("MoveForward", 0);
            // disable movement
            canMove = false;
            // change walking state
            walkingState = WalkingState.DEADEND;
            return;
        }
        // if character is on a connection street and has reached end of the connection, it means he has reached on a new intersection
        else if (locationIndex == path.Count && onConnection)
        {
            
            // set character animator to not moving
            characterAnim.SetFloat("MoveForward", 0);
            // disable movement
            canMove = false;
            // change walking state to on intersection
            walkingState = WalkingState.ON_INTERSECTION;
            // character is not anymore on a connection street
            onConnection = false;
            if (inOfficeTrigger)
            {
                canMoveGeneral = false;
                StartCoroutine(Fade("Out"));
                ChoosePath(dirObjects[onIntersectionDirection]);
                
            }
            else
            {
                // find the proper street for character to rotate to when he reache the intersection
                if (dirObjects.ContainsKey(onIntersectionDirection))
                {
                    ChoosePath(dirObjects[onIntersectionDirection]);

                }
                else
                {
                    ChoosePath(dirObjects[dirObjects.Keys.ToList()[0]]);
                }

            }

            return;
        }
        // if character is not on a connection street and he has reached the end of the path this means it should switch to connection street
        else if (locationIndex == path.Count && !onConnection && canMoveGeneral)
        {
            if (nextPath != null)
            {
                // set next path as current path
                path = new List<Vector3>(nextPath);
                // reverse path elements order because its a connection street
                path.Reverse();
                // make next path empty
                nextPath = new List<Vector3>();
                // reset the location index
                locationIndex = 0;
                nextIntersection.Connected = intersection.ConnectionObject;
                // set next intersection as current intersection
                intersection = nextIntersection;
                nextIntersection = null;
            
                onConnection = true;




            }

            else
            {
                deadEnd = true;
            }
            // choose correct street on the intersection that character is arriving at based on the number of the street on the intersection and character walking side
            if (intersection.Paths.Count == 3)
            {
                onIntersectionDirection = IntersectionDirection.Middle;
                
            }
            else
            {
                if (streetSide == OnStreetSide.LEFT)
                {
                    onIntersectionDirection = IntersectionDirection.Right;
                }
                else
                {
                    onIntersectionDirection = IntersectionDirection.Left;
                }
            }
        }
        // if playe is pressing movement button
        if (Input.GetAxisRaw("Vertical") > 0)
        {

            FindDirections(intersection.Paths);

            // if he is on an intersection
            if (walkingState == WalkingState.ON_INTERSECTION)
            {
                // set the chosen street
                intersection.SetStreet();
                // change walking state to on street
                walkingState = WalkingState.ON_STREET;
                // if the street is deadend
                // set the connection street
                nextPath = intersection.CurrentConnection;
                if (nextPath == null)
                {
                    deadEnd = true;
                }
                else
                {
                    // set the next intersection
                    nextIntersection.SetGivenStreet(intersection.ConnectionObject);

                }
                // reset location index
                locationIndex = 0;
            }
            
            // move character to the point in the path
            this.transform.position = path[locationIndex];
            // if character is not rotating, rotate him based on the current point in the path rotation
            if (!rotating)
            {
                this.transform.localRotation = Quaternion.LookRotation(LookDirection(false), this.transform.up);
            }

            locationIndex++;

            if (toOffice && locationIndex == (int)((path.Count / 2)))
            {
                StartCoroutine(ShowWarning(actionW, 0));
                toOffice = false;
            }
            

        }
    }
    public void SetActiveCamera(bool state)
    {
        characterCamera.SetActive(state);   
    } 
    
    // managing character rotation based on different walking state
    private void CharacterRotation()
    {
        switch(walkingState)
        {
            case WalkingState.ON_STREET:
                // if character is not currently moving check for the mouseX axis and rotation
                if (!rotating)
                {
                    RotateOnStreet();
                }
                break;

            case WalkingState.ON_INTERSECTION:
                if (!rotating)
                {
                    RotateOnIntersection();
                }
                break;
        }
    }
    // If character is in walking on street
    private void RotateOnStreet()
    {
        // Set the character rotation 
        characterRotation += Input.GetAxis("MouseX") * characterRotationSense * Time.deltaTime;
        // if player has moved mouse to the right and if character is not currently in the right side rotate character to right
        if (characterRotation > 20)
        {
            switch(streetSide)
            {
                case OnStreetSide.LEFT:
                    StartCoroutine(OnStreetRotator(OnStreetSide.RIGHT));
                    characterRotation = 0;
                    break;
            }
            // reset character rotation value
            characterRotation = 0;
            return;
        }
        // if player has moved mouse to the left and character is not currently in the lefft side rotate character to left
        else if (characterRotation < -20)
        {
            switch(streetSide)
            {
                case OnStreetSide.RIGHT:
                    StartCoroutine(OnStreetRotator(OnStreetSide.LEFT));
                    break;
            }
            // reset character rotation value
            characterRotation = 0;
        }
    }

    private IEnumerator OnStreetRotator(OnStreetSide nextSide)
    {        
        // set rotating to true
        rotating = true;
        float timer = 0; // timer variable for lerp
        float camSide = 0; // camera side in cinemachine virtual camera
        Vector3 lookDir = LookDirection(true);
        // Set rotation angle and camera side based on the given next side
        switch (nextSide)
        {
            case OnStreetSide.LEFT:
                camSide = 1f;
                break;

            case OnStreetSide.RIGHT:
                
                camSide = 0f;

                break;
        }
        // create rotation
        Quaternion rotation = Quaternion.LookRotation(lookDir, this.transform.up);

        // rotate character and change camera side
        while (followPerson.CameraSide != camSide)
        {
            // increase timere by delta time
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            
            // rotate character
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, rotation, Mathf.SmoothStep(0, 1, Mathf.Log(timer) * rotationSpeed));
            // change camera side
            followPerson.CameraSide = Mathf.Lerp(followPerson.CameraSide, camSide, Mathf.SmoothStep(0, 1, Mathf.Log(timer) * rotationSpeed));
            // switch to back side for the transition if camera side is between 0.3 and 0.7

            switch (nextSide)
            {
                case OnStreetSide.LEFT:
                    if (followPerson.CameraSide > 0.3f)
                    {
                        characterAnim.SetFloat("Side", 0);
                        if (mainCat.gameObject.activeSelf)
                        {
                            mainCat.Rotate(OnStreetSide.LEFT);

                        }
                    }
                    if (followPerson.CameraSide > 0.7f)
                    {
                        characterAnim.SetFloat("Side", -1);
                    }
                    break;

                case OnStreetSide.RIGHT:
                    if (followPerson.CameraSide < 0.7f)
                    {
                        characterAnim.SetFloat("Side", 0);
                        if (mainCat.gameObject.activeSelf)
                        {
                            mainCat.Rotate(OnStreetSide.RIGHT);

                        }
                    }
                    if (followPerson.CameraSide < 0.3f)
                    {
                        characterAnim.SetFloat("Side", 1);

                    }
                    break;
            }

            // if camera side value is same as the target value break the loop
            if (nextSide == OnStreetSide.LEFT)
            {
                if (followPerson.CameraSide == camSide)
                {
                    rotating = false;
                    streetSide = nextSide;
                    this.transform.localRotation = rotation;
                    followPerson.CameraSide = camSide;
                    break;
                }
            }
            else if (nextSide == OnStreetSide.RIGHT)
            {
                if (followPerson.CameraSide == camSide)
                {
                    rotating = false;
                    streetSide = nextSide;
                    this.transform.localRotation = rotation;

                    followPerson.CameraSide = camSide;
                    break;
                }
            }
            
        }
    }

    private void RotateOnIntersection()
    {
        // Set the character rotation 
        characterRotation += Input.GetAxis("MouseX") * characterRotationSense * Time.deltaTime;
        // Get the paths in the intersection
        var paths = intersection.Paths;
        //FindDirections(intersection.Paths);

        // Rotate on the intersection based on the number of the streets
        switch (paths.Count)
        {
            case 2:
                switch (onIntersectionDirection)
                {
                    case IntersectionDirection.Left:
                        if (characterRotation > 20)
                        {
                            //print(dirObjects[IntersectionDirection.Right]);
                            StartCoroutine(OnIntersectionRotator(directions[IntersectionDirection.Left],directions[onIntersectionDirection = IntersectionDirection.Right]));
                            ChoosePath(dirObjects[onIntersectionDirection]);
                            characterRotation = 0;

                        }
                        break;

                    case IntersectionDirection.Right:
                        if (characterRotation < -20)
                        {
                            //print(dirObjects[IntersectionDirection.Left]);

                            StartCoroutine(OnIntersectionRotator(directions[IntersectionDirection.Right], directions[onIntersectionDirection = IntersectionDirection.Left]));
                            ChoosePath(dirObjects[onIntersectionDirection]);
                            characterRotation = 0;

                        }
                        break;
                }
                break;

            case 3:


                switch (onIntersectionDirection)
                {
                    case IntersectionDirection.Left:
                        if (characterRotation > 20)
                        {

                            StartCoroutine(OnIntersectionRotator(directions[IntersectionDirection.Left], directions[onIntersectionDirection = IntersectionDirection.Middle]));
                            //print(dirObjects[IntersectionDirection.Left] + " " + dirObjects[onIntersectionDirection]);
                            ChoosePath(dirObjects[onIntersectionDirection]);
                            characterRotation = 0;
                        }
                        break;

                    case IntersectionDirection.Middle:
                        if (characterRotation > 20)
                        {
                            StartCoroutine(OnIntersectionRotator(directions[IntersectionDirection.Middle], directions[onIntersectionDirection = IntersectionDirection.Right]));
                            //print(dirObjects[IntersectionDirection.Middle] + " " + dirObjects[onIntersectionDirection]);
                            ChoosePath(dirObjects[onIntersectionDirection]);
                            characterRotation = 0;
                        }
                        else if (characterRotation < -20)
                        {
                            StartCoroutine(OnIntersectionRotator(directions[IntersectionDirection.Middle], directions[onIntersectionDirection = IntersectionDirection.Left]));
                            //print(dirObjects[IntersectionDirection.Middle] + " " + dirObjects[onIntersectionDirection]);
                            ChoosePath(dirObjects[onIntersectionDirection]);
                            characterRotation = 0;
                        }
                        break;

                    case IntersectionDirection.Right:
                        if (characterRotation < -20)
                        {
                            StartCoroutine(OnIntersectionRotator(directions[IntersectionDirection.Right], directions[onIntersectionDirection = IntersectionDirection.Middle]));
                            //print(dirObjects[IntersectionDirection.Right] + " " + dirObjects[onIntersectionDirection]);
                            ChoosePath(dirObjects[onIntersectionDirection]);
                            characterRotation = 0;
                        }
                        break;
                }
                break;
        }

    }

    // return streets position in the intersection
    private void FindDirections(Dictionary<GameObject, Vector3> currentPath)
    {

        directions = new Dictionary<IntersectionDirection, Vector3>(); // to record left middle and right directions
        dirObjects = new Dictionary<IntersectionDirection, GameObject>();
        Dictionary<GameObject, float> dirs = new Dictionary<GameObject, float>(); // for calculating the location of each street regarding to character position

        
        // Determine each point position according to character
        foreach (KeyValuePair<GameObject, Vector3> kp in currentPath)
        {
            // calculate cross product
            Vector3 cross = Vector3.Cross(this.transform.forward, kp.Value - this.transform.position);
            
            // get the dot product to see where is the street in positioned
            float dir = Vector3.Dot(cross, this.transform.up);
            dirs.Add(kp.Key, dir);
            //print(kp.Key.name + " " + dir);
        }

        // find the left street by finding the minimum value 
        var left = from kvp in dirs where kvp.Value == dirs.Values.Min() select kvp;
        directions.Add(IntersectionDirection.Left, currentPath[left.ElementAt(0).Key]);
        dirObjects.Add(IntersectionDirection.Left, left.ElementAt(0).Key);
        //print(IntersectionDirection.Left + " " + left.ElementAt(0).Key);
        if (currentPath.Count > 1)
        {
            // find the right street by finding the maximum value
            var right = from kvp in dirs where kvp.Value == dirs.Values.Max() select kvp;
            directions.Add(IntersectionDirection.Right, currentPath[right.ElementAt(0).Key]);
            dirObjects.Add(IntersectionDirection.Right, right.ElementAt(0).Key);
            //print(IntersectionDirection.Right + " " + right.ElementAt(0).Key);
            dirs.Remove(right.ElementAt(0).Key);


        }
        dirs.Remove(left.ElementAt(0).Key);
        // remove left and right street from the dictionary to see if there's an middle street available

        // if still an element is present in the dictionary set it as the middle street
        if (dirs.Count > 0)
        {
            directions.Add(IntersectionDirection.Middle, currentPath[dirs.Keys.ToList()[0]]);
            dirObjects.Add(IntersectionDirection.Middle, dirs.Keys.ToList()[0]);
            //print(IntersectionDirection.Middle + " " + dirs.Keys.ToList()[0]);

        }
        
    }

    private void ChoosePath(GameObject street)
    {
        // Set current path
        path = new List<Vector3>(intersection.ChooseStreet(street));
        //nextPath = new List<Vector3>(intersection.CurrentConnection);
        // Find next intersection on the path
        nextIntersection = intersection.NextIntersection(street);


    }
    private IEnumerator OnIntersectionRotator(Vector3 prevDir, Vector3 target)
    {
        // Set rotating true
        rotating = true;
        // Set timer for rotation
        float timer = 0;
        // Set vector direction to the target
        Vector3 targetDirection = target - this.transform.position; // direction vector to target
        // Set previous vector direction
        Vector3 prevDirection = prevDir - this.transform.position;
        Vector3 lookDir = LookDirection(false); // new look direction
        Debug.DrawRay(transform.position, prevDirection, Color.blue, 10);
        Debug.DrawRay(transform.position, targetDirection, Color.green, 10);
        

        
        // get angle between previos street and the target one
        float angle = Vector3.SignedAngle(prevDirection, targetDirection, this.transform.up);
        //print(angle);
        // rotate look direction vector by angle
        lookDir = Quaternion.AngleAxis(angle, this.transform.up) * lookDir;
        Debug.DrawRay(transform.position, lookDir, Color.cyan, 10);
        // Set rotation 
        //Quaternion rotation = Quaternion.LookRotation(lookDir, this.transform.up);
        Quaternion rotation = Quaternion.LookRotation(Quaternion.Euler(0,offsetAngle,0) *  targetDirection, this.transform.up);
        // Rotate character
        while (this.transform.localRotation != rotation)
        {
            // update timer
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            //this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, rotation, Mathf.SmoothStep(0, 1, Mathf.Log(timer) * rotationSpeed));
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, rotation, Mathf.SmoothStep(0, 1, Mathf.Log(timer) * rotationSpeed));
            if (Vector3.Distance(this.transform.localEulerAngles, rotation.eulerAngles) <0.1f || timer > 2)
            {
                
                break;
            }
        }
        rotating = false;
     }

    public IEnumerator Fade(string to)
    {
        float timer = 0;
        Color target = new Color();
        switch (to)
        {
            case "In":
                if (!characterCamera.activeSelf)
                {
                    characterCamera.SetActive(true);
                }
                target = new Color(0, 0, 0, 0);
                
                break;

            case "Out":
                target = new Color(0, 0, 0, 1);
                
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
                canMoveGeneral = true;
                
                break;

            case "Out":
                if (inOfficeTrigger)
                {
                    sceneManager.ActivateOffice();

                }
                break;


        }
    }

    private IEnumerator BringThought(string text)
    {
        ThoughtLine line = null;

        yield return new WaitUntil(() => thoughtLines.Where(l => !l.inUse).First());
        line = thoughtLines.Where(l => !l.inUse).FirstOrDefault();
        if (line != null)
        {
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

            
            if (ending)
            {
                maxEndingThoughts--;
            }

        }
    }

    private IEnumerator ShowWarning(GameObject warning, float wait)
    {
        yield return new WaitForSeconds(wait);
        warning.SetActive(true);
        yield return new WaitForSeconds(2);
        warning.SetActive(false);
    }
}
