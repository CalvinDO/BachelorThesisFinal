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
    public CWCreatureBrain creatureBrain;

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
    public static int inputs = 0;
    [HideInInspector]
    public static int outputs = 0;

    private float timeSinceStart = 0f;

    public bool fitnessSetToZPosOnly = false;
    private bool useDelinearized = false;
    private float angleBodyForward;

    private Vector3 bodyStartPos;
    private Vector3 totalCoM;


    private float maxDistanceToCOM;


    void Start() {

        this.Fitness = 20;

        CWCreatureController.inputs = 0;
        CWCreatureController.outputs = 0;

        m_JdController = GetComponent<JointDriveController>();

        SetupBodyParts();

        int amountMovableBodyParts = 0;

        for (int bodyIndex = 0; bodyIndex < this.bodyParts.Length; bodyIndex++) {

            amountMovableBodyParts +=
                this.m_JdController.bodyPartsDict[this.bodyParts[bodyIndex]].rotationalFreedom
                == CWRotationInterfaceRotationalFreedom.Nothing
                ? 0 : 1;
        }

        CWCreatureController.inputs = amountMovableBodyParts * 3 + 7;


        this.Inputs = new float[CWCreatureController.inputs];
        //int outputs gets counted up in SetupBodyParts
        this.Outputs = new float[CWCreatureController.outputs];


        this.creatureBrain = this.GetComponent<CWCreatureBrain>();
        this.creatureBrain.Init();


        this.m_JdController.bodyPartsDict[this.body].rb.ResetInertiaTensor();

        this.bodyStartPos = this.body.position;

        if (this.maxDistanceToCOM == 0) {
            this.EstimateMaxDistanceToCOM();
        }
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

                //SetupBodyPart returns the amount of rotatable axis'
                CWCreatureController.outputs += this.m_JdController.SetupBodyPart(joint.transform);
            }
        }

        this.bodyParts = tempBodyParts.ToArray();
    }

    void FixedUpdate() {

        try {
            this.OutputOutputs();
        }
        catch (Exception e) {
            Debug.Log(e.Message);
        }
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


        this.InputInputs();

        if (this.Fitness < 0 || (this.timeSinceStart > this.maxWaveTime)) {
            this.Death = true;

        }

        CalculateFitness();
    }


    private void CalculateFitness() {

        if (CWTrainingManagerDataCollector.instance.GetCurrentTrainingConfiguration().fitnessFunctionType
            == CWTrainingConfiguration.CWTrainingFitnessFunctionType.alsoPunishX) {
            this.Fitness += Time.deltaTime;
        }

        Vector3 avgPosXZ = Vector3.ProjectOnPlane(this.GetAvgPosition(), Vector3.up);
        Vector3 avgVelXZ = Vector3.ProjectOnPlane(this.GetAvgVelocity(), Vector3.up);


        switch (CWTrainingManagerDataCollector.instance.GetCurrentTrainingConfiguration().fitnessFunctionType) {

            case CWTrainingConfiguration.CWTrainingFitnessFunctionType.zPosOnly:
                this.Fitness = 1 + avgPosXZ.z;
                break;

            case CWTrainingConfiguration.CWTrainingFitnessFunctionType.alsoPunishX:

                float absAvgPosZ = Math.Abs(avgPosXZ.z);

                this.Fitness += avgVelXZ.z * Time.deltaTime;
                this.Fitness += absAvgPosZ * Time.deltaTime;
                this.Fitness -= Math.Abs(avgPosXZ.x) * Mathf.Clamp(absAvgPosZ, 0, 10) * 0.1f * Time.deltaTime;

                if (avgPosXZ.z < -0.2f) {
                    this.Fitness -= 10 * Time.deltaTime;
                }

                this.Fitness -= (float)Math.Pow(Math.Abs(this.angleBodyForward), 2) * 10 * Time.deltaTime;

                break;
        }
    }



    private void InputInputs() {

        Vector3 avgVel = this.GetAvgVelocity();

        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVel.x);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVel.y);
        this.Inputs[this.sensorIndex++] = (float)Math.Tanh(avgVel.z);

        float angleTweenAvgVAndForward =
            Vector3.SignedAngle(avgVel, Vector3.forward, Vector3.forward);

        float normalizedAngle = angleTweenAvgVAndForward / 180f;
        this.Inputs[this.sensorIndex++] = normalizedAngle;

        float angleTweenBodyAndForward =
            Vector3.SignedAngle(this.body.transform.forward, Vector3.forward, Vector3.forward) / 180f;

        this.angleBodyForward = angleTweenBodyAndForward;
        this.Inputs[this.sensorIndex++] = angleTweenBodyAndForward;

        this.Inputs[this.sensorIndex++] = Vector3.Dot(Vector3.forward, body.forward);
        this.Inputs[this.sensorIndex++] = Vector3.Dot(Vector3.up, body.forward);


        this.totalCoM = this.GetTotalCoM();


        this.CollectObservationBodyPartOptimized(this.m_JdController.bodyPartsDict[this.body]);

        for (int partIndex = 0; partIndex < this.bodyParts.Length; partIndex++) {
            this.CollectObservationBodyPartOptimized(this.m_JdController.bodyPartsDict[this.bodyParts[partIndex]]);
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

        switch (CWTrainingManagerDataCollector.instance.GetCurrentTrainingConfiguration().inputType) {

            case CWTrainingConfiguration.CWTrainingInputType.comDistances:

                Vector3 distanceToCoM = bodyPart.rb.transform.position - this.totalCoM;
                Vector3 sizeRelativeDistanceToCOM = distanceToCoM / this.maxDistanceToCOM;

                this.Inputs[this.sensorIndex++] = tanH(sizeRelativeDistanceToCOM.x);
                this.Inputs[this.sensorIndex++] = tanH(sizeRelativeDistanceToCOM.z);

                this.Inputs[this.sensorIndex++] = tanH(bodyPart.rb.transform.position.y / this.maxDistanceToCOM);

                break;
            case CWTrainingConfiguration.CWTrainingInputType.rotationalFactor:

                this.Inputs[this.sensorIndex++] = tanH(bodyPart.currentXNormalizedRot);
                this.Inputs[this.sensorIndex++] = tanH(bodyPart.currentYNormalizedRot);

                this.Inputs[this.sensorIndex++] = tanH(bodyPart.rb.transform.position.y / this.maxDistanceToCOM);

                break;
        }
    }

    private float tanH(float value) {

        switch (CWTrainingManagerDataCollector.instance.GetCurrentTrainingConfiguration().delinearizationType) {

            case CWTrainingConfiguration.CWTrainingDelinearizationType.none:
                return value;

            case CWTrainingConfiguration.CWTrainingDelinearizationType.tanh:
                return (float)Math.Tanh(value);

            default:
                return value;
        }
    }


    public void CollectObservationBodyPart(BodyPart bodyPart) {

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
    }



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

            bpDict[this.bodyParts[partIndex]].SetJointStrength(0);
        }
    }
}
