using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CWDataToCSVConverter : MonoBehaviour {

    public TextAsset savedData;
    private CWTrainingDataResults deserializedTrainingResults;

    private string minMaxes = "";

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

        Debug.Log(this.minMaxes);
    }

    private string GetCSV() {

        string output = "";

        foreach (CWTrainingConfigurationResultsData configResult in this.deserializedTrainingResults.configurationsResults) {
            this.minMaxes += " \n" + configResult.configurationName + " \n";

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

        this.minMaxes += " "+ " ";

        List<float> winners = new List<float>();

        foreach (CWTrainingBatchData batchResult in creatureResult.batchResults) {


            float currentWinner = batchResult.wavesMaxDistances.Max();
            winners.Add(currentWinner);
        }

        this.minMaxes += " " + (winners.Max() - winners.Min()).ToString("#.000");

        return winners.Average();
    }
}
