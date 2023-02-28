using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class CWTrainingDataResults {

    public CWTrainingConfigurationResultsData[] configurationsResults;
}


[Serializable]
public class CWTrainingConfigurationResultsData {

    public string configurationName;

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

    public static bool copyConfigIndexFromPreset = false;
    public static int presetConfigIndex;

    public int currentConfigurationIndex = 0;
    public int currentCreatureIndex = 0;
    public int currentBatchIndex = 0;

    public static CWTrainingManagerDataCollector instance;

    public List<float> tempWavesMaxDistances;

    public bool testConfigurations;

    void Awake() {
        CWTrainingManagerDataCollector.instance = this;

        if (CWTrainingManagerDataCollector.copyConfigIndexFromPreset) {
            this.currentConfigurationIndex = CWTrainingManagerDataCollector.presetConfigIndex;
        }
    }

    void Update() {

        if (Input.GetKeyUp(KeyCode.Space)) {
            this.WriteResultsToFile();
        }
    }


    public CWTrainingConfiguration GetCurrentTrainingConfiguration() {
        try {
            return this.trainingConfigurations[this.currentConfigurationIndex];
        }
        catch (Exception e) {
            return this.trainingConfigurations[0];
        }
    }

    void Start() {

        this.trainingDataResults = new CWTrainingDataResults();

        if (this.testConfigurations) {
            this.StartTesting();
        }
    }

    public void StartTesting() {
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

        Debug.Log("testing configuration: " + this.currentConfigurationIndex + "  config name: " + this.trainingConfigurations[this.currentConfigurationIndex]);


        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex] = new CWTrainingConfigurationResultsData();
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].configurationName = this.trainingConfigurations[this.currentConfigurationIndex].name;
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults = new CWTrainingCreatureResultsData[this.creatures.Length];

        while (this.currentCreatureIndex < this.creatures.Length) {

            yield return this.RunCreature();

            this.currentCreatureIndex++;
            this.currentBatchIndex = 0;
        }

        yield return null;//new WaitForSeconds(0.1f);
    }

    IEnumerator RunCreature() {

        Debug.Log("running Creature: " + this.currentCreatureIndex);

        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex] = new CWTrainingCreatureResultsData();
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].name = this.creatures[this.currentCreatureIndex].name;
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].batchResults = new CWTrainingBatchData[this.amountBatchesPerCreature];


        while (this.currentBatchIndex < this.amountBatchesPerCreature) {

            this.ResetScene();

            yield return this.RunBatch();

            this.WriteTempBatchDataToResults();

            this.WriteResultsToFile();

            this.SaveCreatureBrain();

            this.currentBatchIndex++;
        }

        yield return null;
    }

    private void SaveCreatureBrain() {
        ANNLearnByNEATInterface.instance.Ann.Save("Config" + this.GetCurrentTrainingConfiguration().name + "Creature" + this.currentCreatureIndex + "Batch" + this.currentBatchIndex);
    }

    public void WriteTempBatchDataToResults() {

        this.trainingDataResults
        .configurationsResults[this.currentConfigurationIndex]
        .creatureResults[this.currentCreatureIndex]
        .batchResults[this.currentBatchIndex]
        .wavesMaxDistances = new List<float>();

        this.trainingDataResults
         .configurationsResults[this.currentConfigurationIndex]
         .creatureResults[this.currentCreatureIndex]
         .batchResults[this.currentBatchIndex]
         .wavesMaxDistances.AddRange(this.tempWavesMaxDistances);
    }

    IEnumerator RunBatch() {

        Debug.Log("running batch: " + this.currentBatchIndex);

        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].batchResults[this.currentBatchIndex] = new CWTrainingBatchData();
        this.trainingDataResults.configurationsResults[this.currentConfigurationIndex].creatureResults[this.currentCreatureIndex].batchResults[this.currentBatchIndex].index = this.currentBatchIndex;

        this.tempWavesMaxDistances = new List<float>();


        yield return new WaitForSeconds(this.batchTime);
    }

    public void ResetScene() {

        ANNLearnByNEAT.instance.Reset();


        Destroy(CWTrainingSceneManager.initialCreature.gameObject);
        Destroy(CrawlerLearner.instance.networkLearnInterface);

        CWEditorLimb temporaryEditorLimb = CWEditorController.GetDeserializedLimbFromPath
            (Application.dataPath + "/SavedCreatures/" + this.currentCreatureIndex + ".json");

        CWTrainingSceneManager.initialCreature
            = CWEditorPhysicalCreatureFactory.GetPhysicalCreature(temporaryEditorLimb)
            .GetComponent<CWCreatureController>();

        Destroy(temporaryEditorLimb.gameObject);

        CrawlerLearner.instance.Start();

        CWTrainingUIStats.instance.Reset();

        ANNLearnByNEATInterface.instance.Learn = true;
    }
}
