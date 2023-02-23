using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWCreatureBrain : MonoBehaviour {

    public CWCreatureController creatureController;

    [HideInInspector]
    public ANN Network = new ANN();

    private ANNInterface networkInterface;


    public void Reset() {

        if (this.networkInterface) {

            Destroy(this.networkInterface);
            this.Network = new ANN();
        }

    }

    public void Init() {


        this.Network.Create(CWCreatureController.inputs, CWCreatureController.outputs);

        this.networkInterface = this.gameObject.AddComponent<ANNInterface>();
        this.networkInterface.Ann = this.Network;
    }

    void Update() {

        try {

            this.Network.Input = this.creatureController.Inputs;

            this.Network.Solution();

            this.creatureController.Outputs = this.Network.Output;
        }
        catch (Exception e) {

            this.Network = null;
            Debug.Log(e.Message);
        }
    }

}
