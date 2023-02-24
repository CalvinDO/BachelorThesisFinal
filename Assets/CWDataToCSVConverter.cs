using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CWDataToCSVConverter : MonoBehaviour {

    public TextAsset savedData;
    private CWTrainingDataResults deserializedTrainingResults;



    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            this.WriteCSV();
        }
    }

    private void WriteCSV() {

        this.deserializedTrainingResults = new CWTrainingDataResults();

        string serializedData = this.savedData.ToString();

        Debug.Log(serializedData);
        JsonUtility.FromJsonOverwrite(serializedData, this.deserializedTrainingResults);

        string generatedCSV = this.GetCSV();
        string currentPath = Application.dataPath + "/TrainingResults/" + "FormattedTrainingResults" + ".csv";

        File.WriteAllText(currentPath, generatedCSV);
    }

    private string GetCSV() {

        string output = "";

        foreach (CWTrainingConfigurationResultsData configResult in this.deserializedTrainingResults.configurationsResults) {
            foreach (CWTrainingCreatureResultsData creatureResult in configResult.creatureResults) {

                output += this.GetAvarageOfBatchWinners(creatureResult);
                output += ",";
            }

            output = output.Remove(output.Length - 1);
            output += "\n";
        }


        return output;
    }

    private float GetAvarageOfBatchWinners(CWTrainingCreatureResultsData creatureResult) {

        List<float> winners = new List<float>();

        foreach (CWTrainingBatchData batchResult in creatureResult.batchResults) {

            float currentWinner = batchResult.wavesMaxDistances.Max();
            Debug.Log(currentWinner);
            winners.Add(currentWinner);
        }

        return winners.Average();
    }
}
