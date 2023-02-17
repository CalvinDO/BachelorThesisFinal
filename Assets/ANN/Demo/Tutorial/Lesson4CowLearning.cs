using UnityEngine;

public class Lesson4CowLearning : MonoBehaviour
{
    public TutorialCowControl tutCowCtrl;
    public CowsBrain cowsBrain;

    private ANNLearnByNEATInterface networkLearnInterface;

    void Start()
    {
        networkLearnInterface = gameObject.AddComponent<ANNLearnByNEATInterface>();
        networkLearnInterface.Ann = cowsBrain.network;
        //Debug.Log(networkLearnInterface.s)
        networkLearnInterface.NL.AmountOfChildren = 100;
        networkLearnInterface.NL.ChildrenByWave = false;
        networkLearnInterface.NL.ChildrenInWave = 10;
        networkLearnInterface.NL.StudentData(cowsBrain.gameObject, cowsBrain, /*nameof(this.cowsBrain.network)*/ "network", tutCowCtrl, /*nameof(this.tutCowCtrl.Death)*/ "Death", /*nameof(this.tutCowCtrl.LifeTime*/ "LifeTime");
    }
}
