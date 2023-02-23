using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CWTrainingUIStats : MonoBehaviour {

    public TextMeshProUGUI time;
    public TextMeshProUGUI maxDistanceDisplay;

    public TextMeshProUGUI maxTimeDisplay;
    public Slider slider;

    ANNLearnByNEATInterface neatINterface;


    float timeSinceStart = 0;

    public float maxDistance = 0f;

    public float maxWaveTime = 5;

    public static CWTrainingUIStats instance;

    void Awake() {
        CWTrainingUIStats.instance = this;
    }

    void Start() {

        this.UpdateMaxTimeDisplay();

        this.SetCreaturesWaveTime();
    }


    public void Reset() {

        this.timeSinceStart = 0;

        this.maxDistance = 0;

        this.UpdateMaxDistanceDisplay();

        this.SetCreaturesWaveTime();
    }


    // Update is called once per frame
    void Update() {

        if (!this.neatINterface) {

            this.neatINterface = GameObject.FindObjectOfType<ANNLearnByNEATInterface>();

            return;
        }

        if (this.neatINterface.Learn) {
            this.timeSinceStart += Time.deltaTime;
        }


        int seconds = (int)this.timeSinceStart % 60;
        int minutes = (int)(this.timeSinceStart / 60);

        string minutesText = minutes < 10 ? "0" + minutes : "" + minutes;
        string secondsText = seconds < 10 ? "0" + seconds : "" + seconds;


        this.time.text = minutesText + ":" + secondsText;

        CWCreatureController[] creatureControllers = GameObject.FindObjectsOfType<CWCreatureController>();


        float currentMaxDistance = 0;

        foreach (CWCreatureController controller in creatureControllers) {
            float currentZ = controller.GetAvgPosition().z;
            if (currentZ > currentMaxDistance) {
                currentMaxDistance = currentZ;
            }
        }

        this.maxDistance = currentMaxDistance;

        this.UpdateMaxDistanceDisplay();
    }

    public void UpdateMaxDistanceDisplay() {
        this.maxDistanceDisplay.text = this.maxDistance.ToString("#0.000") + "m";
    }
    public void SliderOnValueChanged(Slider slider) {

        this.maxWaveTime = slider.value;

        this.UpdateMaxTimeDisplay();

        this.SetCreaturesWaveTime();
    }

    private void UpdateMaxTimeDisplay() {
        this.maxTimeDisplay.text = (int)this.maxWaveTime + "s";
    }

    void SetCreaturesWaveTime() {

        CWCreatureController[] creatureControllers = GameObject.FindObjectsOfType<CWCreatureController>();

        for (int i = 0; i < creatureControllers.Length; i++) {
            creatureControllers[i].maxWaveTime = this.maxWaveTime;
        }
    }
}
