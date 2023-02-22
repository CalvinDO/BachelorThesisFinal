using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



[Serializable]
public class CWTrainingDataResults {

    public CWTrainingConfigurationResultsData[] configurationsResults;
}


[Serializable]
public class CWTrainingConfigurationResultsData {

    public CWTrainingConfiguration configuration;

    public CWTrainingCreatureResultsData[] creatureResults;
}


[Serializable]
public class CWTrainingCreatureResultsData {

    public string name;

    public CWTrainingBatchData[] batchResults;
}


[Serializable]
public class CWTrainingBatchData {

    public int index;

    public List<float> wavesMaxDistances;
}


public class CWTrainingManagerDataCollector : MonoBehaviour {

    public CWTrainingConfiguration[] trainingConfigurations;
    public TextAsset[] creatures;
    public int amountBatchesPerCreature;
    public float batchTime;


    private CWTrainingDataResults trainingDataResults;


    public int currentConfigurationIndex = 0;
    public int currentCreatureIndex = 0;
    public int currentBatchIndex = 0;


    void Start() {

        this.trainingDataResults = new CWTrainingDataResults();

        StartCoroutine(this.RunConfigurations());
    }

    void WriteResultsToFile() {


        string json = JsonUtility.ToJson(this.trainingDataResults);
        Debug.Log(json);

        string currentPath = Application.dataPath + "/TrainingResults/" + "TrainingResults" + ".json";

        File.WriteAllText(currentPath, json);

        /*
        if (!File.Exists(currentPath) || File.ReadAllText(currentPath) == "") {
        }
        */
    }

    IEnumerator RunConfigurations() {

        this.trainingDataResults.configurationsResults = new CWTrainingConfigurationResultsData[this.trainingConfigurations.Length];


        while (this.currentConfigurationIndex < this.trainingConfigurations.Length) {

            yield return this.RunConfiguration();

            this.currentConfigurationIndex++;
            this.currentCreatureIndex = 0;
        }

        this.WriteResultsToFile();
    }

    IEnumerator RunConfiguration() {

        Debug.Log("testing configuration: " + this.currentConfigurationIndex);


        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex] = new CWTrainingConfigurationResultsData();
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].configuration = this.trainingConfigurations[this.currentConfigurationIndex];
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults = new CWTrainingCreatureResultsData[this.creatures.Length];

        while (this.currentCreatureIndex < this.creatures.Length) {

            yield return this.RunCreature();

            this.currentCreatureIndex++;
            this.currentBatchIndex = 0;
        }

        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator RunCreature() {

        Debug.Log("running Creature: " + this.currentCreatureIndex);

        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex] = new CWTrainingCreatureResultsData();
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].name = this.creatures[this.currentCreatureIndex].name;
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].batchResults = new CWTrainingBatchData[this.amountBatchesPerCreature];


        while (this.currentBatchIndex < this.amountBatchesPerCreature) {

            yield return this.RunBatch();

            this.currentBatchIndex++;
        }

        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator RunBatch() {

        Debug.Log("running batch: " + this.currentBatchIndex);

        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].batchResults[this.currentBatchIndex] = new CWTrainingBatchData();
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].batchResults[this.currentBatchIndex].index = this.currentBatchIndex;



        this.trainingDataResults
            .configurationsResults[this.currentConfigurationIndex]
            .creatureResults[this.currentCreatureIndex]
            .batchResults[this.currentBatchIndex]
            .wavesMaxDistances
            = new List<float>() { 0, 2, 30, 2, 11, 0 };


        yield return new WaitForSeconds(this.batchTime);
    }
}
