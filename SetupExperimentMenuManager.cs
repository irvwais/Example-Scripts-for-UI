using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using System.Linq;
//using System.Collections.ObjectModel;

///////////////////////////////////////////////
// ALL SCRIPTS WRITTEN BY IRVING WAISMAN - 2020
///////////////////////////////////////////////
// This Script is the Setup Experiment Manager,
// the Participant ID and number of Trials is 
// decided here. As well as saving of Presets
// for number of Trials and their detailed 
// selections for each Trial is handeled here
///////////////////////////////////////////////

public class SetupExperimentMenuManager : MonoBehaviour 
{
    [Header("UI Elements")]
    public InputField NumberOfTrialsTextField;
    public InputField ParticipantIDTextField;
    public Text warningAllTrialsText;
    public Text WarningParIDText;
    bool warningParIDEmpty = false;
    public Button GenerateTrialListButton;
    public GameObject ConfirmMenu;
    public Button ConfirmButton;
    public Button StartTrailsButton;

    [Space(5)]
    [Header("Loading Screen Elements")]
    public GameObject loadingScreen;
	public Slider loadingSlider;
	public Text loadingText;

    [Space(5)]
    [Header("Saving and Loading Preset Elements and Info")]
    public Dropdown presetDropdown;
    public bool selectedPreset = false;
    //public int maxPresetCount = 10;
    int presetCount = 0;
    int presetLoadNum = 0;
    string presetTotalCountKey = "Preset_Total_Count";
    //bool reverseCheck = false;
    
    [Space(5)]
    [Header("Scroll Elements")]
    public ScrollRect trialListMenuRect;
    public GameObject trialScrollContent;
    public GameObject trialPrototypeOptionsPrefab;
    public GameObject trialYorkUOptionsPrefab;
    GameObject trialOptionsClone;
    //GameObject trialSelectedOptionsClone;
    
    [Space(5)]
    [Header("Trials List")]
    [SerializeField] int previousTrialCount = 0;
    [SerializeField] List<GameObject> trialsList = new List<GameObject>();
    [SerializeField] List<int> trialPresetList = new List<int>(); 
    //[SerializeField] LinkedList<int> trialPresetLinkedList = new LinkedList<int>();
    int sceneIndex = 0;
    int randomIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        // Check on Start if the Local Harddrive already has a previous save of Presets
        if (PlayerPrefs.HasKey(presetTotalCountKey))
        {
            LoadPresetListFromDrive(); // Load the Preset List if it does exist on the local harddrive
            SelectPresetDropdown(0); // Intial selection of the first preset 
            Debug.Log("Loaded Presets from Drive");
        }
        //trialPresetList.Capacity = maxPresetCount;

        ConfirmMenu.SetActive(false); // Confim Menu is set  to false on Start
        GameManager.Instance.setupScreenActive = true; // setup screen is active on start

        // Check if to load trial options for Prototype and load Prototype Scene
        if (GameManager.Instance.PrototypeSelected) 
        {
            trialOptionsClone = trialPrototypeOptionsPrefab;
            sceneIndex = 2;
        }

        // Check if to load trial options for York U and load York U Model Scene
        if (GameManager.Instance.YorkCampusSelected) 
        {
            trialOptionsClone = trialYorkUOptionsPrefab;
            sceneIndex = 3;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Add ParticipantID for Game Manager from the ParticipantID Text Field
        GameManager.Instance.participantID = ParticipantIDTextField.text;
        
        // Check if Number of Trials Text Field is not Empty
        if (NumberOfTrialsTextField.text != "")
            selectedPreset = false; // If Yes then no selected Preset has been selected 

        // Check if ParticipantID text field is empty
        if (ParticipantIDTextField.text == "")
        {   
            // If Yes then have the corresponding warnings be True  
            WarningParIDText.enabled = true;
            warningParIDEmpty = true;
        }
        
        // Else have these warnings be False
        else 
        {
            WarningParIDText.enabled = false;
            warningParIDEmpty = false;
        }

        // Check if the correct choices for all Trial Start and End Locations are not the same
        if (GameManager.Instance.correctChoiceForAllTrials)
            warningAllTrialsText.enabled = false; // If Yes then have the Warning be False
        
        // Else have the Warning be True
        else
            warningAllTrialsText.enabled = true;
 
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.ResetValues(); // Reset values in Game Manager
        SceneManager.LoadScene("MainMenuScene"); // Load Main Menu Scene
    }

    public void Randomize()
    {
        if (trialsList.Count > 1)
        {
            for (int i = 0; i < trialsList.Count; i++)
            {
                GameObject randomTemp = trialsList[i];
                randomIndex = Random.Range(0, trialsList.Count);
                trialsList[i] = trialsList[randomIndex];
                trialsList[randomIndex] = randomTemp;
            }

            foreach (GameObject trialsSelections in GameObject.FindGameObjectsWithTag("TrialOptionsPrefab"))
            {
                trialsSelections.transform.SetSiblingIndex(trialsList.IndexOf(trialsSelections));
       

                // for (int i = 0; i < trialsList.Count; i++)
                // {
                //     trialsSelections.name = "TrialPrototypeOptions(Clone)" + (i + 1);
                //     trialsSelections.GetComponent<TrialEvent>().trialNumber = i;
                // }
            }
        }
    }

    public void GenerateTrialList()
    {
        // If no Preset Save is selected
        if (!selectedPreset)
        {
            // Convert String from Number of Trials TextField into Number of Trials Integer from Game Manager
            int.TryParse(NumberOfTrialsTextField.text, out GameManager.Instance.numberOfTrials);  

            // Check that the number of Trials is greater than the previous number of Trials
            if (GameManager.Instance.numberOfTrials > previousTrialCount) 
            {
                // for each trial in the trial list count that is less than the number of trials
                // Insantiate the Trial selections for either Prototype options or York U model options
                // set them to the trial scroll content view and above any other UI displays
                // also add the TrialEvent Script to each Trial Selection Clone with the proper intial settings
                for (int i = trialsList.Count; i < GameManager.Instance.numberOfTrials; i++)
                {
                    GameObject trialSelectedOptionsClone = (GameObject) Instantiate(trialOptionsClone);
                    trialSelectedOptionsClone.transform.SetParent(trialScrollContent.transform, false);
                    trialSelectedOptionsClone.transform.Find("TrialText").gameObject.GetComponent<Text>().text = "Trial " + (i + 1).ToString();
                    
                    if (trialSelectedOptionsClone != null) 
                    {
                        if (GameManager.Instance.PrototypeSelected)
                        {
                            trialSelectedOptionsClone.name = "TrialPrototypeOptions(Clone)" + (i + 1);
                            trialSelectedOptionsClone.AddComponent<TrialEvent>().GenerateTrialEvent(i, GameManager.Instance.trialSpawnLocations[0].transform, GameManager.Instance.trialSpawnLocations[0].transform, "Control", false);
                        }

                        if (GameManager.Instance.YorkCampusSelected)
                        {
                            trialSelectedOptionsClone.name = "TrialYorkUOptions(Clone)" + (i + 1);
                            trialSelectedOptionsClone.AddComponent<TrialEvent>().GenerateTrialEvent(i, GameManager.Instance.trialYorkLocations[0].transform, GameManager.Instance.trialYorkLocations[0].transform, "Control", false);
                        }
                        trialsList.Add(trialSelectedOptionsClone);
                        
                    }
                }
                trialListMenuRect.verticalNormalizedPosition = 1;
            }

            // check if the number of trials is less than the previous number of trials
            if (GameManager.Instance.numberOfTrials < previousTrialCount)
            {
                int difference = previousTrialCount - GameManager.Instance.numberOfTrials; // get the difference between the previous number of trials and the current number of trials
                
                // for each trial option from the total trial list count minus the difference till the total trial list count
                for (int i = trialsList.Count - difference; i < trialsList.Count; i++)
                {
                    // check if each trial option is not null and then Destroy the object
                    if (trialsList[i] != null)
                    {
                        Destroy(trialsList[i]);        
                    }
                }
                trialsList.RemoveRange(trialsList.Count - difference, difference); // also remove from the trial list the toal minus the differnce starting at the difference index
            }
            previousTrialCount = GameManager.Instance.numberOfTrials; // set the previous number of trials as the current number of trials
            trialsList.Capacity = GameManager.Instance.numberOfTrials; // set the trial options list capacity as the current number of trials
        }

        // Else we have selected a Saved Preset to Generate the Trials List
        else 
        {
            ClearTrialList(); // so first clear all current displayed Trial options
            previousTrialCount = GameManager.Instance.numberOfTrials; // set the previous number of trials as the current number of trials
            trialsList.Capacity = GameManager.Instance.numberOfTrials; // set the trial options list capacity as the current number of trials
        
            // for each trial in the trial list count that is less than the number of trials
            // Insantiate the Trial selections for either Prototype options or York U model options
            // set them to the trial scroll content view and above any other UI displays
            // also add the TrialEvent Script to each Trial Selection Clone with the proper intial settings
            // the main difference here is that we are getting the correct saved presets data for each trial in the preset 
            for (int i = trialsList.Count; i < GameManager.Instance.numberOfTrials; i++)
            {
                GameObject trialSelectedOptionsClone = (GameObject) Instantiate(trialOptionsClone);
                trialSelectedOptionsClone.transform.SetParent(trialScrollContent.transform, false);
                trialSelectedOptionsClone.transform.Find("TrialText").gameObject.GetComponent<Text>().text = "Trial " + (i + 1).ToString();
                
                if (trialSelectedOptionsClone != null) 
                {
                    if (GameManager.Instance.PrototypeSelected)
                    {
                        trialSelectedOptionsClone.name = "TrialPrototypeOptions(Clone)" + (i + 1);
                        trialSelectedOptionsClone.AddComponent<TrialEvent>().GenerateTrialEvent(i, GameManager.Instance.trialSpawnLocations[0].transform, GameManager.Instance.trialSpawnLocations[0].transform, "Control", false);
                    }

                    if (GameManager.Instance.YorkCampusSelected)
                    {
                        trialSelectedOptionsClone.name = "TrialYorkUOptions(Clone)" + (i + 1);
                        trialSelectedOptionsClone.AddComponent<TrialEvent>().GenerateTrialEvent(i, GameManager.Instance.trialYorkLocations[0].transform, GameManager.Instance.trialYorkLocations[0].transform, "Control", false);
                    }
                    trialsList.Add(trialSelectedOptionsClone);

                    // for each element in the Trial List Get the Start and End Loaction dropdown values in each Trial's Trial Manager and assign the Saved Start and End values to their settings
                    trialsList[i].GetComponent<TrialManager>().trialStartLocationDropdown.value = PlayerPrefs.GetInt("Preset_" + presetLoadNum + "_Trial_" + i + "_StartValue");
                    trialsList[i].GetComponent<TrialManager>().trialEndLocationDropdown.value = PlayerPrefs.GetInt("Preset_" + presetLoadNum + "_Trial_" + i + "_EndValue");
                    
                }            
            }
            trialListMenuRect.verticalNormalizedPosition = 1;
        }
    }

    void ClearTrialList() 
    {
        for (int i = 0; i < trialsList.Count; ++i)
        {
            if (trialsList[i] != null)
                Destroy(trialsList[i]);
        }
        trialsList.Clear();
    }

    public void ClearPresets() // Called when you want to Clear current Presets
    {
        Debug.Log ("Clear Presets");
        presetCount = 0; // reset Preset Count
        PlayerPrefs.DeleteAll(); // Delete All Data on the Local Hardrive
        trialPresetList.Clear(); // Clear Trial Preset List
        presetDropdown.ClearOptions(); // Clear Preset Dropdown list
        presetDropdown.AddOptions(new List<string> {"Preset Empty"}); // Set it back to Default Text
    }

    public void SavePreset() // Called when Saving a Preset
    {
        presetCount++; // iterate the preset count 
        AddPresetToDropdown(); // call Add Preset to Preset Dropdown List 
    }

    void SavePresetListToDrive() // Save Preset Dropdown Data to Local Drive
    {
        // for each element in the Trial Preset List Count
        for (int i = 0; i < trialPresetList.Count; i++)
        {
            PlayerPrefs.SetInt("Preset_" + i + "_Trial_Count", trialPresetList[i]); // Save the number of Trials for Each Preset
        }
        PlayerPrefs.SetInt(presetTotalCountKey, trialPresetList.Count); // Also Save the Total Number of Presets
        PlayerPrefs.Save(); 
        // Finding Player Prefs Data on Local Drive do the following:
        // (Windows 10) Click start, search "regedit" and click "Registry Editor"
        // Go to the following address: Computer\HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\CompanyName\ProjectName
        // You will find all Player Prefs with their String and Int values.
    }

    void LoadPresetListFromDrive() // Load all saved Presets and add them to the dropdown list with the corresponding number of Trials for each Preset (This is called at Start)
    {
        presetDropdown.ClearOptions(); // Clear the current preset dropdown list
        int loadedTrialPresetListCount = PlayerPrefs.GetInt(presetTotalCountKey); // get saved preset list count and assign to loaded preset count 
        
        // for each Preset until the Loaded Preset Count
        for (int i = 0; i < loadedTrialPresetListCount; i++)
        {
            presetDropdown.AddOptions(new List<string> {"Preset " + (i + 1)}); // add the saved Preset to the dropdown
            trialPresetList.Add(PlayerPrefs.GetInt("Preset_" + i + "_Trial_Count")); // and load the number of Trials for each Preset
        }
        presetCount = loadedTrialPresetListCount; // makesure the current preset count is the same as what was loaded in
    }

    void AddPresetToDropdown()
    {
        // if (presetCount > maxPresetCount)
        // {
        //     Debug.Log("Reached Max Preset Count");
        // }

        //else 
        //{
            // if the number of trial presets is less than 1
            if (trialPresetList.Count < 1) 
                presetDropdown.ClearOptions(); // clear preset defaults

            // for each trial preset list count until the current preset count    
            for (int i = trialPresetList.Count; i < presetCount; i++)
            {
                presetDropdown.AddOptions(new List<string> {"Preset " + (i + 1)}); // add the Preset Option to the Preset Dropdown
                
                // for each trial in that preset 
                for (int j = 0; j < trialsList.Count; j++)
                {
                    // save each trials start and end loaction selection with that corresponding Preset 
                    PlayerPrefs.SetInt("Preset_" + i + "_Trial_" + j + "_StartValue", trialsList[j].GetComponent<TrialManager>().trialStartLocationDropdown.value);
                    PlayerPrefs.SetInt("Preset_" + i + "_Trial_" + j + "_EndValue", trialsList[j].GetComponent<TrialManager>().trialEndLocationDropdown.value);
                }
            }

            trialPresetList.Add(trialsList.Count); // add the number of trials to each preset in the preset list
            PlayerPrefs.Save();
            // Finding Player Prefs Data on Local Drive do the following:
            // (Windows 10) Click start, search "regedit" and click "Registry Editor"
            // Go to the following address: Computer\HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\CompanyName\ProjectName
            // You will find all Player Prefs with their String and Int values. 

        //}
    }

    public void SelectPresetDropdown (int index) // called when a Preset is selected from the dropdown 
    {
        NumberOfTrialsTextField.text = ""; // clear the number of trials text field
        GameManager.Instance.numberOfTrials = trialPresetList[index]; // assign the number of trials to the number that exists in the preset list index
        presetLoadNum = index; // assign the index to the preset load number for Key the correct Preset number when saving and loading later
        //GameManager.Instance.numberOfTrials = trialPresetLinkedList.ElementAt(index);
        selectedPreset = true; // selecting a preset is set to true
        Debug.Log("Selected Index of Trial Preset List " + trialPresetList[index].ToString());
        //Debug.Log("Selected Index of Trial Preset List " + trialPresetLinkedList.ElementAt(index).ToString());
    }

    public void BackToSelection()
    {
        GameManager.Instance.allTrialsStartLocations.Clear();
        GameManager.Instance.allTrialsEndLocations.Clear();
        GameManager.Instance.allTrialsTasks.Clear();
        GameManager.Instance.allCheckForStartAndEndLocations.Clear();

        //GameManager.Instance.setupScreenActive = true;
        GameManager.Instance.correctChoiceForAllTrials = false;
    }

    public void ConfirmSelections()
    {  
        // foreach (GameObject trialsSelections in GameObject.FindGameObjectsWithTag("TrialOptionsPrefab"))
        // {
        //     GameManager.Instance.allTrialsStartLocations.Add(trialsSelections.GetComponent<TrialEvent>().startLocation);
        //     GameManager.Instance.allTrialsEndLocations.Add(trialsSelections.GetComponent<TrialEvent>().endLocation);
        //     GameManager.Instance.allTrialsTasks.Add(trialsSelections.GetComponent<TrialEvent>().taskOption);
        //     GameManager.Instance.allCheckForStartAndEndLocations.Add(trialsSelections.GetComponent<TrialEvent>().correctChoiceStartAndEnd);
        // }

        for (int i = 0; i < trialsList.Count; i++)
        {
            GameObject trialsSelections = GameObject.Find("TrialPrototypeOptions(Clone)" + (i + 1));

            GameManager.Instance.allTrialsStartLocations.Add(trialsSelections.GetComponent<TrialEvent>().startLocation);
            GameManager.Instance.allTrialsEndLocations.Add(trialsSelections.GetComponent<TrialEvent>().endLocation);
            GameManager.Instance.allTrialsTasks.Add(trialsSelections.GetComponent<TrialEvent>().taskOption);
            GameManager.Instance.allCheckForStartAndEndLocations.Add(trialsSelections.GetComponent<TrialEvent>().correctChoiceStartAndEnd);
        }

        GameManager.Instance.runCheck = true;

        SavePresetListToDrive(); // save the preset list ot drive once the user has confirmed all their selections
    }

    public void StartTrials()
    {
        if (GameManager.Instance.correctChoiceForAllTrials && !warningParIDEmpty)
        {
            //SavePresetListToDrive();
            GameManager.Instance.setupScreenActive = false;
            GameManager.Instance.activateFirstTrial = true;
            StartCoroutine(LoadAsync(sceneIndex));

            // Creat Report New report with Assigned Paritcipetn ID
            CSVManager.CreateReportTrialTotals();
            CSVManager.CreateReportTrialPositionsPerFrame();
            CSVManager.CreateReportTrialPositionsPerSecond(); 

        }
    }

    IEnumerator LoadAsync (int sceneIndex) 
    {

		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

		while (!operation.isDone) {

			float progress = Mathf.Clamp01 (operation.progress / 0.9f);
			loadingSlider.value = progress;
			loadingText.text = "Loading: " + progress * 100f + "%";
            //Debug.Log(progress);
			yield return null;
		}
	}

}
