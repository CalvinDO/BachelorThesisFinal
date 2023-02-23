using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CWTrainingSceneManager : MonoBehaviour {
    // Start is called before the first frame update

    public static CWCreatureController initialCreature;

    void Start() {

        CWTrainingSceneManager.initialCreature = GameObject.FindObjectOfType<CWCreatureController>();

        SceneManager.MoveGameObjectToScene(initialCreature.gameObject, SceneManager.GetActiveScene());
    }

    // Update is called once per frame
    void Update() {

    }
}
