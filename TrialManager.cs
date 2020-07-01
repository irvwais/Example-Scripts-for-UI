using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] // Means we can save it into a File
public class TrialManager : MonoBehaviour
{
    public Text TrialWarningText;
    public Dropdown trialStartLocationDropdown;
    public Dropdown trialEndLocationDropdown;
    public Dropdown trialTaskDropdown;
    public GameObject taskOptionsDisplay;
    public Dropdown numberOfTasksDropdown;
    
    void Awake()
    {   
        if (GameManager.Instance.PrototypeSelected) 
        {
            for (int i = 0; i < GameManager.Instance.trialSpawnLocations.Length; i++)
            { 
                trialStartLocationDropdown.AddOptions(new List<string> {GameManager.Instance.trialSpawnLocations[i].name});
                trialEndLocationDropdown.AddOptions(new List<string> {GameManager.Instance.trialSpawnLocations[i].name}); 
            }
        }

        if (GameManager.Instance.YorkCampusSelected) 
        {
            for (int i = 0; i < GameManager.Instance.trialYorkLocations.Length; i++)
            { 
                trialStartLocationDropdown.AddOptions(new List<string> {GameManager.Instance.trialYorkLocations[i].name});
                trialEndLocationDropdown.AddOptions(new List<string> {GameManager.Instance.trialYorkLocations[i].name});
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //trialStartLocationDropdown.ClearOptions(); // Clear all Start Location Dropdown Options just incase
        
        
        //SelectTrialStartLocationDropdown(0);
        //SelectTrialEndLocationDropdown(0);
        SelectTrialTaskOptionDropdown(0);
        SelectNumberOfTasks(0);

    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<TrialEvent>().startLocation == 
            this.gameObject.GetComponent<TrialEvent>().endLocation) 
        {
            TrialWarningText.enabled = true;
            this.gameObject.GetComponent<TrialEvent>().correctChoiceStartAndEnd = false;
        }
        else
        {
            TrialWarningText.enabled = false;
            this.gameObject.GetComponent<TrialEvent>().correctChoiceStartAndEnd = true;
        }
    }

    public void SelectTrialStartLocationDropdown (int index)
    {   
        if (GameManager.Instance.PrototypeSelected)
            this.gameObject.GetComponent<TrialEvent>().startLocation = GameManager.Instance.trialSpawnLocations[index].transform;

        if (GameManager.Instance.YorkCampusSelected)
            this.gameObject.GetComponent<TrialEvent>().startLocation = GameManager.Instance.trialYorkLocations[index].transform;
    }  

    public void SelectTrialEndLocationDropdown (int index)
    {
        if (GameManager.Instance.PrototypeSelected)
            this.gameObject.GetComponent<TrialEvent>().endLocation = GameManager.Instance.trialSpawnLocations[index].transform;

        if (GameManager.Instance.YorkCampusSelected)
            this.gameObject.GetComponent<TrialEvent>().endLocation = GameManager.Instance.trialYorkLocations[index].transform;
    }

    public void SelectTrialTaskOptionDropdown (int index)
    {
        if (index == 0)
            this.gameObject.GetComponent<TrialEvent>().taskOption = "Control";
        
        if (index == 1)
        {
            this.gameObject.GetComponent<TrialEvent>().taskOption = "Task";
            taskOptionsDisplay.SetActive(true);
            
        }
        
        else
        {
            taskOptionsDisplay.SetActive(false);
        }

        if (index == 2)
            this.gameObject.GetComponent<TrialEvent>().taskOption = "Modified";
    }
}
