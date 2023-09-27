using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ApartmentManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Object;
    [SerializeField]
    private TextMeshProUGUI Note;
    public GameObject CurrentObject;
    private ApartmentCharacter character; 
    
    
    private void Start()
    {
        character = FindObjectOfType<ApartmentCharacter>();
        switch(ChosenDependency.chosenDependency)
        {
            case ChosenDependency.Dependency.Family:
                CurrentObject = Object.Where(obj => obj.name == "Photo").SingleOrDefault();
                CurrentObject.SetActive(true);
                Note.text = "stop by the office (be grateful)\n\nget pastry";
                break;
            case ChosenDependency.Dependency.Friend:
                CurrentObject = Object.Where(obj => obj.name == "Tickets").SingleOrDefault();
                CurrentObject.SetActive(true);
                Note.text = "stop by the office (be grateful)\n\nwithdraw money";
                break;
            case ChosenDependency.Dependency.Partner:
                CurrentObject = Object.Where(obj => obj.name == "Lock").SingleOrDefault();
                CurrentObject.SetActive(true);
                Note.text = "stop by the office (be grateful)\n\nget flowers";
                break;

            case ChosenDependency.Dependency.Home:
                CurrentObject = Object.Where(obj => obj.name == "Tank").SingleOrDefault();
                CurrentObject.SetActive(true);
                Note.text = "stop by the office (be grateful)\n\nget food for the crab";
                break;

            case ChosenDependency.Dependency.Knife:
                CurrentObject = Object.Where(obj => obj.name == "Knife").SingleOrDefault();
                CurrentObject.SetActive(true);
                Note.text = "stop by the office (be grateful)\n\nget a knife sharpener";
                break;

            case ChosenDependency.Dependency.Pet:
                CurrentObject = Object.Where(obj => obj.name == "Collar").SingleOrDefault();
                CurrentObject.SetActive(true);
                Note.text = "stop by the office (be grateful)\n\nget a vase";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ChosenDependency.chosenDependency != ChosenDependency.Dependency.Knife && ChosenDependency.chosenDependency != ChosenDependency.Dependency.Friend)
        {
            Quaternion rot = Quaternion.LookRotation(character.transform.position - CurrentObject.transform.position, CurrentObject.transform.up);
            CurrentObject.transform.localRotation = Quaternion.Euler(0, rot.eulerAngles.y + 90, 0);

        }
    }
}
