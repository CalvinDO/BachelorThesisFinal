using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerLearner : MonoBehaviour {

    public CWCreatureController creatureController;
    public CWCreatureBrain creatureBrain;

    [HideInInspector]
    public ANNLearnByNEATInterface networkLearnInterface;

    public static CrawlerLearner instance;

    void Awake() {
        CrawlerLearner.instance = this;
    }

    public void Start() {

        this.creatureController = FindObjectOfType<CWCreatureController>();
        this.creatureBrain = FindObjectOfType<CWCreatureBrain>();

        this.networkLearnInterface = gameObject.AddComponent<ANNLearnByNEATInterface>();
        this.networkLearnInterface.Ann = creatureBrain.Network;
        this.networkLearnInterface.NL.AmountOfChildren = 13;
        this.networkLearnInterface.NL.ChildrenInWave = 13;
        this.networkLearnInterface.NL.ChanceCoefficient = 0.04f;
        this.networkLearnInterface.NL.ChangeWeightSign = 0.25f;

        this.networkLearnInterface.NL.Cross = false;
        this.SyncCrossingWithDataCollector();

        this.networkLearnInterface.NL.PerceptronStart = true;
        this.networkLearnInterface.NL.ChildrenDifference = 1.26f;
        this.networkLearnInterface.NL.AddingWeightsCount = 2;
        this.networkLearnInterface.NL.MutationAddWeight = 17.94f / 1.25f;
        this.networkLearnInterface.NL.MutationChangeOneWeight = 27.51f / 1.25f;
        this.networkLearnInterface.NL.MutationChangeWeights = 22.73f / 1.25f;
        this.networkLearnInterface.NL.MutationAddNeuron = 31.82f / 1.25f;

        this.networkLearnInterface.NL.StudentData(creatureBrain.gameObject, creatureBrain, nameof(this.creatureBrain.Network), creatureController, nameof(this.creatureController.Death), nameof(this.creatureController.Fitness));
    }

    void Update() {
        this.SyncCrossingWithDataCollector();
    }

    void SyncCrossingWithDataCollector() {

        switch (CWTrainingManagerDataCollector.instance.GetCurrentTrainingConfiguration().hyperparameterType) {

            case CWTrainingConfiguration.CWTrainingHyperparameterType.crossingOff:
                this.networkLearnInterface.NL.Cross = false;
                break;

            case CWTrainingConfiguration.CWTrainingHyperparameterType.crossingOn:
                this.networkLearnInterface.NL.Cross = true;
                break;

            default:
                this.networkLearnInterface.NL.Cross = false;
                break;
        }
    }
}
