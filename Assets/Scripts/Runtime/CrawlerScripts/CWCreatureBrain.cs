using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWCreatureBrain : MonoBehaviour {

    public CWCreatureController creatureController;

    [HideInInspector]
    public ANN Network = new ANN();

    private ANNInterface networkInterface;


    public void Init() {

        this.Network.Create(this.creatureController.inputs, this.creatureController.outputs);

        this.networkInterface = this.gameObject.AddComponent<ANNInterface>();
        this.networkInterface.Ann = this.Network;
    }

    void Update() {

        this.Network.Input = this.creatureController.Inputs;

        this.Network.Solution();

        this.creatureController.Outputs = this.Network.Output;
    }

}
