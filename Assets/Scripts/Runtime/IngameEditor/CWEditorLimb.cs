using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CWEditorLimbData {

    public Vector3 position;
    public Quaternion rotation;

    public CWEditorDockingBallData[] dockingBalls;
    private int dockingDirectionIndex;


    public int ballDocketAtIndex;
    public bool isDockedLimb;

    //public CWEditorLimbData parentLimb;
    public List<CWEditorLimbData> childLimbs;

    [HideInInspector]
    public CWRotationInterfaceData rotationData;
}


public class CWEditorLimb : MonoBehaviour {

    //NOT SERIALIZED

    internal const float segment_length = 0.0752f * 2 * 2;

    public GameObject meshesGroup;

    public Transform pivot;


    public Transform scalingMiddle;
    public Transform endCap;
    public Transform endCapCenter;

    public GameObject raycastCatcherRotator;

    public CWEditorDockingBall dockingBallPrefab;

    public CWEditorRotationInterface rotationInterface;

    private bool areDockingPointsDirty = true;

    //SERIALIZED

    public CWEditorDockingBall[] dockingBalls;
    public Transform ballDockedAt;
    public bool isDockedLimb;

    [HideInInspector]
    public CWEditorLimb parentLimb;
    public List<CWEditorLimb> childLimbs;

    public CWRotationInterfaceData rotationData;



    void Start() {

        if (!this.isDockedLimb && this.areDockingPointsDirty) {
            this.RemoveInnerFrontLookingDockingPoint();
            this.RemoveInnerBackLookingDockingPoint();
        }

        this.rotationData = this.rotationInterface.GetData();
    }

    // Update is called once per frame
    void Update() {

        try {
            if (CWEditorController.instance.currentBuildingLimb == this) {
                this.raycastCatcherRotator.SetActive(true);
            }
            else {
                this.raycastCatcherRotator.SetActive(false);
            }
        }
        catch (System.Exception e) {
            Debug.LogWarning(e.Message);
        }

    }

    public void UpdateRotationData() {
        this.rotationData = this.rotationInterface.GetData();
    }


    public CWEditorLimbData GetData() {

        //CWEditorLimbData limbData = ScriptableObject.CreateInstance<CWEditorLimbData>();
        CWEditorLimbData limbData = new CWEditorLimbData();


        limbData.position = this.transform.position;
        limbData.rotation = this.transform.rotation;

        limbData.dockingBalls = this.GetDockingData();


        limbData.isDockedLimb = this.isDockedLimb;


        if (this.isDockedLimb) {
            limbData.ballDocketAtIndex = Array.IndexOf(this.parentLimb.dockingBalls, ballDockedAt.GetComponent<CWEditorDockingBall>());
        }


        limbData.childLimbs = this.GetChildData(limbData);

        limbData.rotationData = this.rotationData;

        return limbData;
    }

    public void InsertDataDeserialize(CWEditorLimbData limbData) {

        this.transform.position = limbData.position;
        this.transform.rotation = limbData.rotation;

        this.areDockingPointsDirty = false;
        this.InsertDockingDataDeserialize(limbData.dockingBalls);


        this.isDockedLimb = limbData.isDockedLimb;

        if (this.isDockedLimb) {

            this.ballDockedAt = this.parentLimb.dockingBalls[limbData.ballDocketAtIndex].transform;
            this.ScalePositionCaps(this.dockingBalls.Length);
        }

        this.InsertChildsDataDeserialize(limbData.childLimbs);

        this.rotationData = limbData.rotationData;

        this.rotationInterface.SetDataDeserialize(this.rotationData);
    }


    private CWEditorDockingBallData[] GetDockingData() {

        CWEditorDockingBallData[] output = new CWEditorDockingBallData[this.dockingBalls.Length];

        for (int i = 0; i < this.dockingBalls.Length; i++) {
            output[i] = this.dockingBalls[i].GetData();
        }

        return output;
    }

    private void InsertDockingDataDeserialize(CWEditorDockingBallData[] dockingBallDatas) {

        for (int i = 0; i < this.dockingBalls.Length; i++) {
            GameObject.Destroy(this.dockingBalls[i].gameObject);
        }

        this.dockingBalls = new CWEditorDockingBall[dockingBallDatas.Length];

        for (int i = 0; i < dockingBallDatas.Length; i++) {
            this.dockingBalls[i] = GameObject.Instantiate(this.dockingBallPrefab, this.transform);
            this.dockingBalls[i].InsertDataDeserialize(dockingBallDatas[i]);
        }
    }

    private List<CWEditorLimbData> GetChildData(CWEditorLimbData parentUnderlimbData) {

        List<CWEditorLimbData> childLimbDatas = new List<CWEditorLimbData>();

        this.childLimbs.ForEach(limb => {

            CWEditorLimbData limbData = limb.GetData();

            childLimbDatas.Add(limbData);

        });

        return childLimbDatas;
    }

    private void InsertChildsDataDeserialize(List<CWEditorLimbData> childLimbs) {

        childLimbs.ForEach(limb => {

            CWEditorLimb deserializedChildLimb = GameObject.Instantiate(CWEditorController.instance.limb, this.transform);

            deserializedChildLimb.parentLimb = this;

            deserializedChildLimb.InsertDataDeserialize(limb);

            this.childLimbs.Add(deserializedChildLimb);
        });
    }


    public void SetLength(float dotWithForward) {

        int amountSegments = (int)dotWithForward + 1;

        this.ScalePositionCaps(amountSegments);

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            this.FinishBuilding(amountSegments);
        }
    }

    private void ScalePositionCaps(int amountSegments) {

        this.scalingMiddle.transform.localScale = new Vector3(1, 1, amountSegments);

        this.endCap.localPosition = Vector3.forward * -CWEditorLimb.segment_length / 2 + (amountSegments - 1) * CWEditorLimb.segment_length * Vector3.forward;
    }

    public void InstantiateDockingBalls(int amountSegments) {

        foreach (CWEditorDockingBall sphere in this.dockingBalls) {
            GameObject.Destroy(sphere.gameObject);
        }

        this.dockingBalls = new CWEditorDockingBall[amountSegments];

        Vector3 currentDockingSpherePos;

        for (int sphereIndex = 1; sphereIndex <= amountSegments; sphereIndex++) {

            currentDockingSpherePos = this.transform.position + this.transform.TransformDirection(Vector3.forward) * CWEditorLimb.segment_length * sphereIndex;
            currentDockingSpherePos -= this.transform.TransformDirection(Vector3.forward) * CWEditorLimb.segment_length / 2f;

            this.dockingBalls[sphereIndex - 1] = Instantiate(this.dockingBallPrefab, currentDockingSpherePos, Quaternion.identity, this.transform);

            int currentBuildingDockingPointIndex = int.Parse(CWEditorController.instance.currentBuildingDockingPoint.tag);
           // int currentBuildingDockingPointIndex =  Array.IndexOf(CWEditorController.instance.currentBuildingDockingBall.availableDockingPoints, CWEditorController.instance.currentBuildingDockingPoint);

            //Fix front DockingPoint at initial limb when docking
            /*
            if (CWEditorController.instance.currentBuildingDockingPoint == CWEditorController.instance.currentBuildingDockingBall.frontLookingDockingPoint && !CWEditorController.instance.currentBuildingDockingBall.limb.isDockedLimb) {
                currentBuildingDockingPointIndex = 1;
            }
            */
                
            if (sphereIndex != amountSegments) {
                this.dockingBalls[sphereIndex - 1].RemoveFrontLookingDockingPoint(currentBuildingDockingPointIndex);
            }

            this.dockingBalls[sphereIndex - 1].RemoveBackLookingDockingPoint(currentBuildingDockingPointIndex);
        }
    }

    public Vector3 GetCenterPos() {

        Vector3 centerPos = this.ballDockedAt.position + (this.dockingBalls[^1].transform.position - this.ballDockedAt.position) / 2;
        return centerPos;
    }

    public float GetLength() {
        return (this.dockingBalls.Length + (this.isDockedLimb ? 1 : 0)) * CWEditorLimb.segment_length;
    }

    private void FinishBuilding(int amountSegments) {

        this.InstantiateDockingBalls(amountSegments);

        CWEditorController.instance.RemoveCurrentBuildingDockingPoint();

        //this.RemoveInnerBackLookingDockingPoint();
        //this.RemoveOuterBackLookingDockingPoint();
        //this.RemoveInnerFrontLookingDockingPoint();

        CWEditorController.instance.SetStateNavigating();
    }

    private void RemoveInnerBackLookingDockingPoint() {
        this.dockingBalls[^1].RemoveDefaultBackLookingDockingPoint();
    }

    private void RemoveInnerFrontLookingDockingPoint() {
        this.dockingBalls[0].RemoveDefaultFrontLookingDockingPoint();
    }

    public void SetParentLimb(CWEditorLimb _parent) {

        this.name = "DockedLimb";
        this.isDockedLimb = true;
        this.parentLimb = _parent;
        _parent.childLimbs.Add(this);
    }

    public void SetBallDockedAt(Transform ballDockedAt) {
        this.ballDockedAt = ballDockedAt;
    }
}
