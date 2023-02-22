using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundContact : MonoBehaviour {
    [HideInInspector] public CWCreatureController crawlerController;
    public bool agentDoneOnGroundContact;
    public bool penalizeGroundContact; // Whether to penalize on contact.
    public float groundContactPenalty; // Penalty amount (ex: -1).
    public bool touchingGround;

    void OnCollisionStay(Collision other) {

        this.touchingGround = true;

        return;
        if (this.penalizeGroundContact) {
            this.crawlerController.Fitness += this.groundContactPenalty;
        }

        if (this.agentDoneOnGroundContact) {
           // Debug.Log("groundDeath");
            this.crawlerController.Death = true;

        }
    }

    void OnCollisionExit(Collision other) {
        this.touchingGround = false;
    }
}
