using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;

public class ChildChainEndException : Exception {
    public ChildChainEndException() : base($" | CWEditorPhysicalCreatureFactory | childLimb Count = 0") {

    }
}
public class CWEditorPhysicalCreatureFactory {

    public static CWEditorLimb initialEditorLimb;
    //public static CrawlerController crawlerController;
    public static GameObject physicalCreature;

    private const float collider_Radius = 0.076f * 2;


    public static GameObject GetPhysicalCreature(CWEditorLimb editorLimb) {

        physicalCreature = new GameObject("CustomCreature");

        CWLimb physicalBody = new GameObject("Body").AddComponent<CWLimb>();

        physicalBody.transform.parent = physicalCreature.transform;
        physicalBody.transform.SetPositionAndRotation(editorLimb.transform.position, editorLimb.transform.rotation);

        physicalBody.childLimbs = new List<CWLimb>();

        physicalBody.endCapCenter = editorLimb.endCapCenter;

        SetupLayer(physicalBody);
        SetupMeshes(physicalBody, editorLimb);
        SetupOptimizedColliders(physicalBody, editorLimb);
        SetupRigidybody(physicalBody, 1.5f);
        SetupGroundContact(physicalBody, true, true);

        CreateAttachChildLimbs(physicalBody, editorLimb.childLimbs, false);

        SetupLogics(physicalCreature);

        SetToFloor(physicalCreature);


        return physicalCreature;
    }


    private static void CreateAttachChildLimb(CWLimb physicalParentLimb, 
        CWEditorLimb editorChildLimb, bool parentUnderTransform) {

        if (editorChildLimb.isDockedLimb) {

            CWLimb physicalChildLimb = new GameObject("ChildLimb").AddComponent<CWLimb>();

            physicalChildLimb.parent = physicalParentLimb;
            physicalParentLimb.childLimbs.Add(physicalChildLimb);

            physicalChildLimb.childLimbs = new List<CWLimb>();

            SetupLayer(physicalChildLimb);
            SetupTransforms(physicalChildLimb, editorChildLimb, physicalParentLimb, parentUnderTransform);
            SetupMeshes(physicalChildLimb, editorChildLimb);
            SetupOptimizedColliders(physicalChildLimb, editorChildLimb);
            SetupRigidybody(physicalChildLimb, 1.5f);
            SetupGroundContact(physicalChildLimb, false, false);

            SetupConfigurableJoints(physicalChildLimb, editorChildLimb);

            if (editorChildLimb.childLimbs.Count > 0) {
                CreateAttachChildLimbs(physicalChildLimb, editorChildLimb.childLimbs, true);
            }
        }
    }

    private static void CreateAttachChildLimbs(CWLimb physicalParentLimb, List<CWEditorLimb> _editorChildLimbs, bool parentUnderTransform) {

        foreach (CWEditorLimb currentLimb in _editorChildLimbs) {
            CreateAttachChildLimb(physicalParentLimb, currentLimb, parentUnderTransform);
        }
    }


    private static void SetToFloor(GameObject root) {

        float currentY = float.MaxValue;
        float lowestY = GetLowestPosOfChildLimbs(root.GetComponentsInChildren<CWLimb>(), currentY);

        root.transform.Translate(Vector3.down * lowestY);
    }


    private static float GetLowestPosOfChildLimbs(CWLimb[] childLimbs, float currentY) {

        foreach (CWLimb currentLimb in childLimbs) {

            float currentLimbY = currentLimb.endCapCenter.transform.position.y;

            if (currentLimbY < currentY) {
                currentY = currentLimbY;
            }

            float lowestYOfChildren = GetLowestPosOfChildLimbs(currentLimb.childLimbs.ToArray(), currentY);

            currentY = lowestYOfChildren < currentY ? lowestYOfChildren : currentY;
        }
        return currentY;
    }


    private static void SetupLogics(GameObject creature) {

        JointDriveController driveController = creature.gameObject.AddComponent<JointDriveController>();
        driveController.maxJointSpring = 100000;
        driveController.jointDampen = 250;
        driveController.maxJointForceLimit = 800;


        CWCreatureController creatureController = creature.AddComponent<CWCreatureController>();
        creatureController.body = creature.transform.GetChild(0);

        CWCreatureBrain creatureBrain = creature.AddComponent<CWCreatureBrain>();
        creatureBrain.creatureController = creatureController;
    }

    private static void SetupConfigurableJoints(CWLimb physicalChildLimb, CWEditorLimb editorLimb) {

        ConfigurableJoint configJoint = physicalChildLimb.gameObject.AddComponent<ConfigurableJoint>();

        configJoint.connectedBody = physicalChildLimb.parent.rb;

        configJoint.anchor = new Vector3(0, 0, physicalChildLimb.transform.InverseTransformPoint(editorLimb.pivot.position).z);


        configJoint.xMotion = configJoint.yMotion = configJoint.zMotion = ConfigurableJointMotion.Locked;

        configJoint.angularXMotion = editorLimb.rotationData.rotationalFreedom == CWRotationInterfaceRotationalFreedom.XY || editorLimb.rotationData.rotationalFreedom == CWRotationInterfaceRotationalFreedom.X ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
        configJoint.angularYMotion = editorLimb.rotationData.rotationalFreedom == CWRotationInterfaceRotationalFreedom.XY || editorLimb.rotationData.rotationalFreedom == CWRotationInterfaceRotationalFreedom.Y ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
        configJoint.angularZMotion = ConfigurableJointMotion.Locked;

        configJoint.rotationDriveMode = RotationDriveMode.Slerp;
        configJoint.projectionMode = JointProjectionMode.PositionAndRotation;


        CWRotationInterfaceData limitsData = editorLimb.rotationData;
        configJoint.lowAngularXLimit = new SoftJointLimit() { limit = limitsData.minMaxRotationX[0] };
        configJoint.highAngularXLimit = new SoftJointLimit() { limit = limitsData.minMaxRotationX[1] };
        configJoint.angularYLimit = new SoftJointLimit() { limit = Mathf.Abs(limitsData.minMaxRotationY[1]) };

    }


    private static void SetupGroundContact(CWLimb physicalBody, bool penelizeGroundContact, bool agentDoneOnGroundContact) {

        GroundContact groundContact = physicalBody.gameObject.AddComponent<GroundContact>();

        groundContact.groundContactPenalty = -1000;
        groundContact.agentDoneOnGroundContact = agentDoneOnGroundContact;
        groundContact.penalizeGroundContact = penelizeGroundContact;
    }

    private static void SetupRigidybody(CWLimb limb, float mass) {

        Rigidbody rb = limb.gameObject.AddComponent<Rigidbody>();

        // rb.isKinematic = true;
        rb.mass = mass;
        rb.drag = 0.05f;
        rb.angularDrag = 0.05f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        limb.rb = rb;
    }

    private static void SetupMeshes(CWLimb physicalChildLimb, CWEditorLimb editorLimb) {

        GameObject meshes = GameObject.Instantiate(editorLimb.meshesGroup, editorLimb.transform.position, editorLimb.transform.rotation, physicalChildLimb.transform);

        meshes.name = "Meshes";
    }

    private static void SetupOptimizedColliders(CWLimb physicalChildLimb, CWEditorLimb editorLimb) {

        CapsuleCollider collider = physicalChildLimb.gameObject.AddComponent<CapsuleCollider>();
        collider.direction = 2;
        collider.radius = collider_Radius;
        collider.height = editorLimb.GetLength();

        collider.material = CWEditorController.instance.limbPhysicMaterial;
    }

    private static void SetupTransforms(CWLimb physicalChildLimb, CWEditorLimb editorLimb, CWLimb physicalParentLimb, bool parentUnderTransform) {

        Vector3 centerPos = editorLimb.GetCenterPos();

        if (parentUnderTransform) {
            physicalChildLimb.transform.parent = physicalParentLimb.transform;
        }
        else {
            physicalChildLimb.transform.parent = physicalCreature.transform;
        }


        physicalChildLimb.transform.SetPositionAndRotation(centerPos, editorLimb.transform.rotation);

        physicalChildLimb.endCapCenter = editorLimb.endCapCenter;
    }

    private static void SetupLayer(CWLimb physicalChildLimb) {
        physicalChildLimb.gameObject.layer = LayerMask.NameToLayer("Creature");
    }

}
