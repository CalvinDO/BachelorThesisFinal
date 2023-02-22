using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.MLAgentsExamples;
using System;

public enum CWCreatureControllerInputMode {

    Minimalistic = 1,
    Experimental = 2,
    ForcesApproach = 3
}

public class CWCreatureController : MonoBehaviour {

    [HideInInspector]
    public CWCreatureBrain crawlerBrain;

    //[HideInInspector]
    public float[] Outputs;
    [HideInInspector]
    public float[] Inputs;

    JointDriveController m_JdController;

    [Header("Body Parts")]
    [Space(10)]

    public Transform body;

    public Transform[] bodyParts;

    private int sensorIndex = 0;

    [HideInInspector]
    public bool Death = false;
    public float Fitness = 20;
    public float maxWaveTime = 20;

    [Header("Network Configuration")]
    [HideInInspector]
    public int inputs = 0;
    [HideInInspector]
    public int outputs = 0;

    private float timeSinceStart = 0f;

    public bool fitnessSetToZPosOnly = false;
    private bool useDelinearized = false;
    private float angleBodyForward;

    //private CWCreatureControllerInputMode inputMode = CWCreatureControllerInputMode.Minimalistic;
    //private CWCreatureControllerInputMode inputMode = CWCreatureControllerInputMode.Experimental;
    private CWCreatureControllerInputMode inputMode = CWCreatureControllerInputMode.ForcesApproach;


    private Vector3 bodyStartPos;
    private Vector3 totalCoM;


    private float maxDistanceToCOM;


    void Start() {

        this.Fitness = 20;

        this.inputs = 0;
        this.outputs = 0;

        m_JdController = GetComponent<JointDriveController>();

        SetupBodyParts();

        switch (this.inputMode) {

            case CWCreatureControllerInputMode.Minimalistic:

                this.inputs = (this.body == null ? 0 : 1) + this.bodyParts.Length * 1;
                break;

            case CWCreatureControllerInputMode.Experimental:

                this.inputs = (this.body == null ? 0 : 1) + this.bodyParts.Length * 1 + 4 + 3;
                break;
            case CWCreatureControllerInputMode.ForcesApproach:

                int amountMovableBodyParts = 0;

                for (int bodyIndex = 0; bodyIndex < this.bodyParts.Length; bodyIndex++) {
                    amountMovableBodyParts += this.m_JdController.bodyPartsDict[this.bodyParts[bodyIndex]].rotationalFreedom == CWRotationInterfaceRotationalFreedom.Nothing ? 0 : 1;
                }


                this.inputs = amountMovableBodyParts * 3 + 5;
                break;

            default:
                break; ;
        }


        this.Inputs = new float[this.inputs];

        //int outputs gets counted up in SetupBodyParts
        this.Outputs = new float[this.outputs];

        this.crawlerBrain = this.GetComponent<CWCreatureBrain>();

        this.crawlerBrain.Init();


        //this.m_JdController.bodyPartsDict[this.body].rb.inertiaTensor = new Vector3(0.1f, 0.01f, 0.1f);

        this.m_JdController.bodyPartsDict[this.body].rb.ResetInertiaTensor();

        this.bodyStartPos = this.body.position;

        if (this.maxDistanceToCOM == 0) {
            this.EstimateMaxDistanceToCOM();
        }

        //Debug.Log(Formulas.ActivationFunction(-1.419f, 1, true));
    }

    private void SetupBodyParts() {

        if (this.body) {
            this.m_JdController.SetupBodyPart(this.body);
        }

        ConfigurableJoint[] joints = this.transform.GetComponentsInChildren<ConfigurableJoint>();

        List<Transform> tempBodyParts = new List<Transform>();

        foreach (ConfigurableJoint joint in joints) {

            if (joint.transform != this.body) {
                tempBodyParts.Add(joint.transform);
                //+1 for strength!
                this.outputs += this.m_JdController.SetupBodyPart(joint.transform); // + 1 for strength
            }
        }

        this.bodyParts = tempBodyParts.ToArray();
    }

    void FixedUpdate() {
        this.OutputOutputs();
    }

    void Update() {

        this.timeSinceStart += Time.deltaTime;

        if (this.Death) {

            foreach (BodyPart bodyPart in this.m_JdController.bodyPartsList) {
                bodyPart.Reset(bodyPart);
            }

            this.timeSinceStart = 0f;
            this.Fitness = 20f;
            this.Death = false;
        }

        switch (this.inputMode) {
            case CWCreatureControllerInputMode.Minimalistic:
                this.InputInputsMinimalistic();
                break;
            case CWCreatureControllerInputMode.Experimental:
                this.InputInputsExperimental();
                break;
            case CWCreatureControllerInputMode.ForcesApproach:
                this.InputInputsForcesApproach();
                break;
            default:
                break;
        }


        if (this.Fitness < 0 || (this.timeSinceStart > this.maxWaveTime)) {
            this.Death = true;

        }

        CalculateFitness();
    }


    private void CalculateFitness() {

        if (!this.fitnessSetToZPosOnly) {
            this.Fitness += Time.deltaTime;
        }

        Vector3 avgPosXZ = Vector3.ProjectOnPlane(this.GetAvgPosition(), Vector3.up);
        Vector3 avgVelXZ = Vector3.ProjectOnPlane(this.GetAvgVelocity(), Vector3.up);


        if (!this.fitnessSetToZPosOnly) {
            //this.LifeTime += (avgPosXZ.z > 0 ? Mathf.Pow(avgPosXZ.z, 2) : 0) * 10 * Time.deltaTime;

            //this.LifeTime += (avgPosXZ.z > 0 ? avgPosXZ.z : 0) * 10 * Time.deltaTime;

            //this.LifeTime += avgVelXZ.z * Time.deltaTime;  /*> 0 ? this.GetAvgVelocity().z * 10 * Time.deltaTime : 0*/;

            float absAvgPosZ = Math.Abs(avgPosXZ.z);

            //this.LifeTime -= Math.Abs(avgPosXZ.x) * Mathf.Clamp(absAvgPosZ, 0, 5) * Time.deltaTime;

            // this.Fitness = 1 + absAvgPosZ - Math.Abs(avgPosXZ.x) * Mathf.Clamp(absAvgPosZ, 0, 10) * 0.1f;

            this.Fitness += avgVelXZ.z * Time.deltaTime;
            this.Fitness += absAvgPosZ * Time.deltaTime - Math.Abs(avgPosXZ.x) * Mathf.Clamp(absAvgPosZ, 0, 10) * 0.1f * Time.deltaTime;

            if (avgPosXZ.z < 0.2f) {
                this.Fitness -= 10 * Time.deltaTime;
            }

            this.Fitness -= (float)Math.Pow(Math.Abs(this.angleBodyForward), 2) * 10 * Time.deltaTime;

        }
        else {
            this.Fitness = 1 + avgPosXZ.z;
        }
    }

    //this.LifeTime -= Mathf.Pow(Math.Abs(this.GetAvgPosition().x / 10), 2) * Time.deltaTime;
    //this.LifeTime -= Math.Abs(this.GetAvgVelocity().x) * Time.deltaTime;



    private void InputInputsForcesApproach() {

        Vector3 avgVel = this.GetAvgVelocity();

        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVel.x);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVel.y);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVel.z);

        float angleTweenAvgVAndForward = Vector3.SignedAngle(avgVel, Vector3.forward, Vector3.forward);
        float normalizedAngle = angleTweenAvgVAndForward / 180f;

        this.Inputs[this.sensorIndex++] = normalizedAngle;

        float angleTweenBodyAndForward = Vector3.SignedAngle(this.body.transform.forward, Vector3.forward, Vector3.forward) / 180f;
        this.angleBodyForward = angleTweenBodyAndForward;

        this.Inputs[this.sensorIndex++] = angleTweenBodyAndForward;

        this.totalCoM = this.GetTotalCoM();


        this.CollectObservationBodyPartOptimized(this.m_JdController.bodyPartsDict[this.body]);

        for (int partIndex = 0; partIndex < this.bodyParts.Length; partIndex++) {
            this.CollectObservationBodyPartOptimized(this.m_JdController.bodyPartsDict[this.bodyParts[partIndex]]);
        }

        this.sensorIndex = 0;
    }



    private void InputInputsMinimalistic() {

        this.CollectObservationBodyPartMinimalistic(this.m_JdController.bodyPartsDict[this.body]);

        for (int partIndex = 0; partIndex < this.bodyParts.Length; partIndex++) {
            this.CollectObservationBodyPartMinimalistic(this.m_JdController.bodyPartsDict[this.bodyParts[partIndex]]);
        }

        this.sensorIndex = 0;
    }



    private void InputInputsExperimental() {

        //Quaternion localForwardToWorldForward = Quaternion.FromToRotation(this.body.forward, Vector3.forward);

        //this.Inputs[this.sensorIndex++] = localForwardToWorldForward.x;
        //this.Inputs[this.sensorIndex++] = localForwardToWorldForward.y;
        //this.Inputs[this.sensorIndex++] = localForwardToWorldForward.z;
        //this.Inputs[this.sensorIndex++] = localForwardToWorldForward.w;

        Vector3 avgVelocity = this.GetAvgVelocity();



        if (avgVelocity.magnitude > 10) {
            Debug.Log("avgVelocity.magnitude> 10");
        }

        //this.Inputs[this.sensorIndex++] = avgVelocity.magnitude;

        Vector3 avgPos = this.GetAvgPosition();

        Vector3 bodyDiff = this.body.position - avgPos;

        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(bodyDiff.x * 10);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(bodyDiff.y * 10);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(bodyDiff.z * 10);


        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVelocity.x);
        //this.Inputs[this.sensorIndex++] = avgVelocity.y;
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVelocity.z);

        if (!this.fitnessSetToZPosOnly) {
            //this.LifeTime -= Mathf.Abs(avgVelocity.x) * 2 * Time.deltaTime;
        }

        float angleTweenAvgVAndForward = Vector3.SignedAngle(avgVelocity, Vector3.forward, Vector3.forward);
        float normalizedAngle = angleTweenAvgVAndForward / 180f;

        this.Inputs[this.sensorIndex++] = normalizedAngle;

        if (!this.fitnessSetToZPosOnly) {
            //this.LifeTime -= Mathf.Abs(normalizedAngle) * Time.deltaTime;
        }

        float angleTweenBodyAndForward = Vector3.SignedAngle(this.body.transform.forward, Vector3.forward, Vector3.forward) / 180f;
        this.angleBodyForward = angleTweenBodyAndForward;
        this.Inputs[this.sensorIndex++] = angleTweenBodyAndForward;

        //this.LifeTime -= Mathf.Abs(angleTweenBodyAndForward) * 0.25f * Time.deltaTime;




        this.CollectObservationBodyPart(this.m_JdController.bodyPartsDict[this.body]);

        for (int partIndex = 0; partIndex < this.bodyParts.Length; partIndex++) {
            this.CollectObservationBodyPart(this.m_JdController.bodyPartsDict[this.bodyParts[partIndex]]);
        }

        this.sensorIndex = 0;
    }

    public Vector3 GetTotalCoM() {

        Vector3 massPosSum = Vector3.zero;
        Vector3 avgCoM = Vector3.zero;

        float totalMass = 0;

        foreach (var bodyPart in this.m_JdController.bodyPartsList) {

            totalMass += bodyPart.rb.mass;
            massPosSum += bodyPart.rb.worldCenterOfMass * bodyPart.rb.mass;
        }

        avgCoM = massPosSum / totalMass;

        return avgCoM;
    }

    private void EstimateMaxDistanceToCOM() {

        this.totalCoM = this.GetTotalCoM();

        this.maxDistanceToCOM = 0;

        foreach (var bodyPart in this.m_JdController.bodyPartsList) {

            float currentDistanceToCOM = Vector3.Magnitude(bodyPart.rb.transform.position - this.totalCoM);

            if (currentDistanceToCOM > this.maxDistanceToCOM) {
                this.maxDistanceToCOM = currentDistanceToCOM;
            }
        }

        Debug.Log("estimated maxdisToCOM: " + this.maxDistanceToCOM);
    }

    private void CollectObservationBodyPartOptimized(BodyPart bodyPart) {


        if (bodyPart.rb.transform == this.m_JdController.bodyPartsDict[this.body].rb.transform) {

            if (bodyPart.rb.transform.up.y < 0) {
                this.Fitness -= float.MaxValue;
            }

            return;
        }

        if (bodyPart.rotationalFreedom == CWRotationInterfaceRotationalFreedom.Nothing) {
            return;
        }

        //Vector3 avgVel = this.GetAvgVelocity();
        //Vector3 differenceVel = bodyPart.rb.velocity - this.m_JdController.bodyPartsDict[this.body].rb.velocity;

        Vector3 distanceToCoM = bodyPart.rb.transform.position - this.totalCoM;

        Vector3 sizeRelativeDistanceToCOM = distanceToCoM / this.maxDistanceToCOM;

        /*
        if (bodyPart.rb.transform == this.body) {

            this.Inputs[this.sensorIndex++] = (float)Math.Tanh(differenceVel.x);
            this.Inputs[this.sensorIndex++] = (float)Math.Tanh(differenceVel.y);
            this.Inputs[this.sensorIndex++] = (float)Math.Tanh(differenceVel.z);

        }
        */

        //Vector3 avgVel = this.GetAvgVelocity();


        //bodyPart.rb.velo

        //currently without tanH

        this.Inputs[this.sensorIndex++] = tanH(sizeRelativeDistanceToCOM.x);
        this.Inputs[this.sensorIndex++] = tanH(sizeRelativeDistanceToCOM.z);

        //this.Inputs[this.sensorIndex++] = tanH(bodyPart.currentXNormalizedRot);
        //this.Inputs[this.sensorIndex++] = tanH(bodyPart.currentYNormalizedRot);


        /*
         this.Inputs[this.sensorIndex++] = sizeRelativeDistanceToCOM.y;
         this.Inputs[this.sensorIndex++] = sizeRelativeDistanceToCOM.z;
          */
        this.Inputs[this.sensorIndex++] = tanH(bodyPart.rb.transform.position.y / this.maxDistanceToCOM);
        /*

        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(bodyPart.rb.angularVelocity.x);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(bodyPart.rb.angularVelocity.y);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(bodyPart.rb.angularVelocity.z);
       */
        //this.Inputs[this.sensorIndex++] = force.z * 100;
    }

    private float tanH(float value) {
        return (float)Math.Tanh(value);
    }


    private void CollectObservationBodyPartMinimalistic(BodyPart bodyPart) {

        if (this.useDelinearized) {
            this.Inputs[this.sensorIndex++] = Formulas.ActivationFunction((this.body.position.y - bodyPart.rb.transform.position.y) * 5, 1, true);
            return;
        }

        this.Inputs[this.sensorIndex++] = (bodyPart.rb.transform.position.y - 0.8f) * 2.5f;// (this.body.position.y - bodyPart.rb.transform.position.y) * 2;
    }


    public void CollectObservationBodyPart(BodyPart bodyPart) {

        //this.Inputs[this.sensorIndex++] = bodyPart.groundContact.touchingGround ? 1f : -1f;
        //this.Inputs[this.sensorIndex++] = bodyPart.groundContact.touchingGround ? -1f : -1f + bodyPart.rb.transform.position.y;
        /*
        if (this.Inputs[this.sensorIndex] == 0) {
            // Debug.Log(bodyPart.rb.gameObject.name);
            //Debug.Log(this.sensorIndex);
        }
        */

        //this.Inputs[this.sensorIndex++] = bodyPart.rb.angularVelocity.x / 180f;

        if (bodyPart.rb.transform == this.body) {

            if (this.useDelinearized) {

                this.Inputs[this.sensorIndex++] = (float)Math.Tanh((this.body.position.y) * 1.313);

                return;
            }

            this.Inputs[this.sensorIndex++] = (this.body.position.y);


            return;
        }



        if (this.useDelinearized) {

            this.Inputs[this.sensorIndex++] = (float)Math.Tanh((this.body.position.y - bodyPart.rb.transform.position.y) * 1.313);

            return;
        }

        this.Inputs[this.sensorIndex++] = (this.body.position.y - bodyPart.rb.transform.position.y);

        //this.Inputs[this.sensorIndex++] = bodyPart.rb.transform.position.y / 10f;

    }

    void OnDrawGizmos() {

        /*
        foreach (BodyPart bodyPart in this.m_JdController.bodyPartsList) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(bodyPart.rb.transform.position, 0.2f);
        }
        */

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.totalCoM, 0.3f);
    }

    //bad inputs:

    //this.Inputs[this.sensorIndex++] = bodyPart.currentXNormalizedRot;
    //this.Inputs[this.sensorIndex++] = bodyPart.currentYNormalizedRot;
    //this.Inputs[this.sensorIndex++] = bodyPart.rb.transform.position.y;
    //this.Inputs[this.sensorIndex++] = bodyPart.rb.velocity.x;
    //this.Inputs[this.sensorIndex++] = bodyPart.rb.velocity.y;
    //this.Inputs[this.sensorIndex++] = bodyPart.rb.velocity.z;


    // doesn't yield good results:
    //this.Inputs[this.sensorIndex++] = this.timeSinceStart / 10;

    //RaycastHit hit;
    //if (Physics.Raycast(this.body.position, Vector3.down, out hit, 10)) {
    //    this.Inputs[this.sensorIndex++] = hit.distance / 10;
    //}
    //else
    //    this.Inputs[this.sensorIndex++] = 1;

    //Vector3 avgVelocity = this.GetAvgVelocity();


    //this.Inputs[this.sensorIndex++] = avgVelocity.x;
    //this.Inputs[this.sensorIndex++] = avgVelocity.y;
    //this.Inputs[this.sensorIndex++] = avgVelocity.z;

    //if (bodyPart.rb.transform != this.body) {
    //    this.Inputs[this.sensorIndex++] = bodyPart.currentStrength / this.m_JdController.maxJointForceLimit;
    //}
    //this.Inputs[this.sensorIndex++] = bodyPart.currentZNormalizedRot;

    public Vector3 GetAvgPosition() {
        Vector3 posSum = Vector3.zero;
        Vector3 avgPos = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList) {
            numOfRb++;
            posSum += item.rb.position;
        }

        avgPos = posSum / numOfRb;
        return avgPos;
    }

    Vector3 GetAvgVelocity() {

        Vector3 velSum = Vector3.zero;
        Vector3 avgVel = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList) {
            numOfRb++;
            velSum += item.rb.velocity;
        }

        avgVel = velSum / numOfRb;
        return avgVel;
    }


    public void OutputOutputs() {

        var bpDict = this.m_JdController.bodyPartsDict;
        int i = 0;

        for (int partIndex = 0; partIndex < this.bodyParts.Length; partIndex++) {

            BodyPart currentBodyPart = bpDict[this.bodyParts[partIndex]];

            switch (currentBodyPart.rotationalFreedom) {

                case CWRotationInterfaceRotationalFreedom.X:

                    bpDict[this.bodyParts[partIndex]].SetJointTargetRotation(this.Outputs[i++], 0, 0);
                    break;

                case CWRotationInterfaceRotationalFreedom.Y:

                    bpDict[this.bodyParts[partIndex]].SetJointTargetRotation(0, this.Outputs[i++], 0);
                    break;

                case CWRotationInterfaceRotationalFreedom.XY:

                    bpDict[this.bodyParts[partIndex]].SetJointTargetRotation(this.Outputs[i++], this.Outputs[i++], 0);
                    break;

                default:
                    break;
            }


            //bpDict[this.bodyParts[partIndex]].SetJointStrength(this.Outputs[i++]);
            bpDict[this.bodyParts[partIndex]].SetJointStrength(0);

        }
    }
}
