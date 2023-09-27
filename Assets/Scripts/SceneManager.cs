using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject office;
    [SerializeField]
    private OfficeCharacter officeCharacter;
    [SerializeField]
    private GameObject apartment;
    [SerializeField]
    private ApartmentCharacter apartmentCharacter;
    [SerializeField]
    private MainCat mainCat;
    [SerializeField]
    private Character mainCharacter;
    [SerializeField]
    private List<GameObject> paths = new List<GameObject>();
    [SerializeField]
    private GameObject peopleSpawner;
    public bool officeDone = false;
    [SerializeField]
    private GameObject mainScenePost;
    [SerializeField]
    private GameObject apartmentPost;
    [SerializeField]
    private GameObject officePost;
    [SerializeField]
    private List<GameObject> stallTriggers = new List<GameObject>();
    [SerializeField]
    private List<GameObject> BadTriggers = new List<GameObject>();
    [SerializeField]
    private List<GameObject> goodTriggers = new List<GameObject>();
    private bool catGoToStall = false;
    private bool catGoToDepend = false;
    private bool stallDone = false;
    [SerializeField]
    private List<GameObject> familyGoodBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> familyBadBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> friendGoodBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> friendBadBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> partnerGoodBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> partnerBadBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> homeGoodBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> homeBadBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> knifeGoodBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> knifeBadBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> petGoodBlock = new List<GameObject>();
    [SerializeField]
    private List<GameObject> petBadBlock = new List<GameObject>();

    private List<GameObject> badBlock = new List<GameObject>();
    private List<GameObject> goodBlock = new List<GameObject>();
    
    private Dictionary<StreetIntersection, List<GameObject>> badIntersections = new Dictionary<StreetIntersection, List<GameObject>>();
    private bool badEndingSet = false;
    [SerializeField]
    private List<StreetIntersection> intersections = new List<StreetIntersection>();
    [SerializeField]
    private ReflectionProbe refProbe;
    [System.Serializable]
    public struct OmitStreet
    {
        public StreetIntersection intersection;
        public GameObject street;

    }

    [System.Serializable]
    public struct DependencyBlocks
    {
        public ChosenDependency.Dependency dependency;
        public List<OmitStreet> streets;        
    }
    [SerializeField]
    private List<DependencyBlocks> badStreets = new List<DependencyBlocks>();
    private void Awake()
    {
        foreach (GameObject g in paths)
        {
            g.SetActive(true);
        }
        
    }
    void Start()
    {
        refProbe.RenderProbe();
        apartmentPost.SetActive(true);
        peopleSpawner.SetActive(true);
        mainCharacter.gameObject.SetActive(true);
        DependencyBlocks block;
        switch (ChosenDependency.chosenDependency)
        {
            case ChosenDependency.Dependency.Family:
                mainCharacter.stallTrigger = stallTriggers.Where(obj => obj.name == "Family").SingleOrDefault();
                mainCharacter.badTrigger = BadTriggers.Where(obj => obj.name == "Family").SingleOrDefault();
                mainCharacter.goodTrigger = goodTriggers.Where(obj => obj.name == "Friends/Family/Partner").SingleOrDefault();
                goodBlock = familyGoodBlock;
                badBlock = familyBadBlock;
                block = badStreets.Where(obj => obj.dependency == ChosenDependency.Dependency.Family).SingleOrDefault();

                foreach (var b in block.streets)
                {
                    if (!badIntersections.ContainsKey(b.intersection))
                    {
                        badIntersections.Add(b.intersection, new List<GameObject> { b.street });

                    }
                    else
                    {
                        badIntersections[b.intersection].Add(b.street);
                    }
                }
                break;

            case ChosenDependency.Dependency.Friend:
                mainCharacter.stallTrigger = stallTriggers.Where(obj => obj.name == "Friends").SingleOrDefault();
                mainCharacter.badTrigger = BadTriggers.Where(obj => obj.name == "Friends").SingleOrDefault();
                mainCharacter.goodTrigger = goodTriggers.Where(obj => obj.name == "Friends/Family/Partner").SingleOrDefault();
                goodBlock = friendGoodBlock;
                badBlock = friendBadBlock;
                block = badStreets.Where(obj => obj.dependency == ChosenDependency.Dependency.Friend).SingleOrDefault();

                foreach (var b in block.streets)
                {
                    if (!badIntersections.ContainsKey(b.intersection))
                    {
                        badIntersections.Add(b.intersection, new List<GameObject> { b.street });

                    }
                    else
                    {
                        badIntersections[b.intersection].Add(b.street);
                    }
                }
                break;

            case ChosenDependency.Dependency.Partner:
                mainCharacter.stallTrigger = stallTriggers.Where(obj => obj.name == "Partner").SingleOrDefault();
                mainCharacter.badTrigger = BadTriggers.Where(obj => obj.name == "Partner").SingleOrDefault();
                mainCharacter.goodTrigger = goodTriggers.Where(obj => obj.name == "Friends/Family/Partner").SingleOrDefault();
                goodBlock = partnerGoodBlock;
                badBlock = partnerBadBlock;
                block = badStreets.Where(obj => obj.dependency == ChosenDependency.Dependency.Partner).SingleOrDefault();

                foreach (var b in block.streets)
                {
                    if (!badIntersections.ContainsKey(b.intersection))
                    {
                        badIntersections.Add(b.intersection, new List<GameObject> { b.street });

                    }
                    else
                    {
                        badIntersections[b.intersection].Add(b.street);
                    }
                }
                break;

            case ChosenDependency.Dependency.Home:
                mainCharacter.stallTrigger = stallTriggers.Where(obj => obj.name == "Home").SingleOrDefault();
                mainCharacter.badTrigger = BadTriggers.Where(obj => obj.name == "Home").SingleOrDefault();
                mainCharacter.goodTrigger = goodTriggers.Where(obj => obj.name == "Home/Knife").SingleOrDefault();
                goodBlock = homeGoodBlock;
                badBlock = homeBadBlock;
                block = badStreets.Where(obj => obj.dependency == ChosenDependency.Dependency.Home).SingleOrDefault();

                foreach (var b in block.streets)
                {
                    if (!badIntersections.ContainsKey(b.intersection))
                    {
                        badIntersections.Add(b.intersection, new List<GameObject> { b.street });

                    }
                    else
                    {
                        badIntersections[b.intersection].Add(b.street);
                    }
                }
                break;

            case ChosenDependency.Dependency.Knife:
                mainCharacter.stallTrigger = stallTriggers.Where(obj => obj.name == "Knife").SingleOrDefault();
                mainCharacter.badTrigger = BadTriggers.Where(obj => obj.name == "Knife").SingleOrDefault();
                mainCharacter.goodTrigger = goodTriggers.Where(obj => obj.name == "Home/Knife").SingleOrDefault();
                goodBlock = knifeGoodBlock;
                badBlock = knifeBadBlock;
                block = badStreets.Where(obj => obj.dependency == ChosenDependency.Dependency.Knife).SingleOrDefault();

                foreach (var b in block.streets)
                {
                    if (!badIntersections.ContainsKey(b.intersection))
                    {
                        badIntersections.Add(b.intersection, new List<GameObject> { b.street });

                    }
                    else
                    {
                        badIntersections[b.intersection].Add(b.street);
                    }
                }
                break;

            case ChosenDependency.Dependency.Pet:
                mainCharacter.stallTrigger = stallTriggers.Where(obj => obj.name == "Toy").SingleOrDefault();
                mainCharacter.badTrigger = BadTriggers.Where(obj => obj.name == "Toy").SingleOrDefault();
                mainCharacter.goodTrigger = BadTriggers.Where(obj => obj.name == "Toy").SingleOrDefault();
                goodBlock = petGoodBlock;
                badBlock = petBadBlock;
                block = badStreets.Where(obj => obj.dependency == ChosenDependency.Dependency.Pet).SingleOrDefault();

                foreach (var b in block.streets)
                {
                    if (!badIntersections.ContainsKey(b.intersection))
                    {
                        badIntersections.Add(b.intersection, new List<GameObject> { b.street });

                    }
                    else
                    {
                        badIntersections[b.intersection].Add(b.street);
                    }
                }
                break;
        }

        foreach (var v in badBlock)
        {
            v.SetActive(true);
        }

        
    }

    private void Update()
    {
        if (mainCharacter.badEnding && !badEndingSet)
        {
            foreach(var v in badBlock)
            {
                v.SetActive(false);
            }
            foreach (var v in goodBlock)
            {
                v.SetActive(true);
            }
            foreach (var v in badIntersections)
            {
                foreach (var s in v.Value)
                {
                    var st = intersections.Where(obj => obj.gameObject == v.Key.gameObject).SingleOrDefault();
                    var curStreet = st.Streets;
                    int indexSt = curStreet.IndexOf(curStreet.Where(obj => obj == s).SingleOrDefault());
                    st.Streets.RemoveAt(indexSt);
                    st.Connections.RemoveAt(indexSt);
                    //curStreet.Remove(curStreet.Where(obj => obj == s).SingleOrDefault());
                    
                    //curStreet[curStreet.IndexOf(curStreet.Where(obj => obj == s).SingleOrDefault())] = null;
                    //curStreet.IndexOf(curStreet.Where(obj => obj == s).SingleOrDefault()) = null;
                    
                }
            }
            badEndingSet = true;
            
        }

        if (!officeDone && mainCat.pathIndex == mainCat.currentPath.Count - 1 && !catGoToStall)
        {
            
            print("SDASDSAF");
            mainCat.CompleteAction();
            mainCat.currentPath = mainCat.actions.Where(obj => obj.actionName == "GoToStall").SingleOrDefault().path;
            mainCat.pathIndex = 0;
            catGoToStall = true;
            mainCat.currentAction.running = true;

        }
        //if (officeDone && main)
    }
    public void ActivateOffice()
    {

        if (!mainCharacter.catSleep)
        {
            Destroy(mainCat.gameObject);
            mainCharacter.badEnding = true;
        }
        RenderSettings.fog = false;
        refProbe.RenderProbe();

        mainCharacter.SetActiveCamera(false);
        office.SetActive(true);
        StartCoroutine(officeCharacter.Fade("In"));
        officePost.SetActive(true);
        mainScenePost.SetActive(false);
        //officeCharacter.canMove = true;
    }
    public void ActivateMainAfterOffice()
    {
        mainCharacter.inOfficeTrigger = false;
        mainCharacter.officeArea = false;
        mainCharacter.stallTrigger.SetActive(true);
        print(mainCharacter.stallTrigger);
        RenderSettings.fog = true;
        refProbe.RenderProbe();
        StartCoroutine(mainCharacter.Fade("In"));
        officePost.SetActive(false);
        mainScenePost.SetActive(true);
        Destroy(office);
        officeDone = true;
        if (!mainCharacter.badEnding)
        {

            mainCat.CompleteAction();
            mainCat.currentPath = new List<Vector3>(mainCat.actions.Where(obj => obj.actionName == "GoToStall").SingleOrDefault().path);
            List<Vector3> tmp = new List<Vector3>(mainCat.actions.Where(obj => obj.actionName == "GoToDependency").SingleOrDefault().path);
            print(tmp.Count);
            mainCat.currentPath.AddRange(tmp);
            mainCat.pathIndex = 20;
            mainCat.currentAction.running = true;
            catGoToStall = true;
            


        }

        


    }

    public void AfterStall()
    {
        mainCharacter.goodTrigger.SetActive(true);
    }
    public void DisableOffice()
    {
        office.SetActive(false);
    }

    public void ActivateMainScene()
    {

        mainCat.currentPath = mainCat.actions.Where(obj => obj.actionName == "GoToOffice").SingleOrDefault().path;
        RenderSettings.fog = true;
        refProbe.RenderProbe();

        StartCoroutine(mainCharacter.Fade("In"));
        apartmentPost.SetActive(false);
        mainScenePost.SetActive(true);
        Destroy(apartment);
        mainCharacter.firstFade = true;
        //mainCat.gameObject.SetActive(true);
    }

}
