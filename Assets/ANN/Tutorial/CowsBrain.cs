using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowsBrain : MonoBehaviour {

    public TutorialCowControl tutCowCtrl;
    public ANN network = new ANN();
    private ANNInterface networkInterface;

    void Start() {

        this.network.Create(3, 2);

        this.networkInterface = this.gameObject.AddComponent<ANNInterface>();
        this.networkInterface.Ann = this.network;
    }

    // Update is called once per frame
    void Update() {

        this.network.Input[0] = this.tutCowCtrl.AngleToFood / 180f;
        this.network.Input[1] = Mathf.Sqrt(this.tutCowCtrl.DistanceToFood / 41f);
        this.network.Input[2] = this.tutCowCtrl.Satiety / 50F;

        this.network.Solution();

        this.tutCowCtrl.Turn = this.network.Output[0];
        this.tutCowCtrl.Move = this.network.Output[1];


    }
}
