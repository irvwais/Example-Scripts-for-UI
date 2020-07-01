using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Means we can save it into a File
public class TrialEvent : MonoBehaviour
{
    public int trialNumber;
    public Transform startLocation;
    public Transform endLocation;
    public string taskOption;
    public bool correctChoiceStartAndEnd = false;

    public void GenerateTrialEvent(int newTrialsNumber, Transform newStartLocation, Transform newEndLocation, string newTaskOption, bool newCorrectChoiceStartAndEnd)
    {
        trialNumber = newTrialsNumber;
        startLocation = newStartLocation;
        endLocation = newEndLocation;
        taskOption = newTaskOption;
        correctChoiceStartAndEnd = newCorrectChoiceStartAndEnd;
    }
}
