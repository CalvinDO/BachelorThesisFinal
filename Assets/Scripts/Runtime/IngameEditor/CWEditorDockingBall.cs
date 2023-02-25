using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class CWEditorDockingBallData {

    public Vector3 position;
    public Quaternion rotation;

    public List<int> removedIndizes;
}

public class CWEditorDockingBall : MonoBehaviour {

    public Transform[] availableDockingPoints;
    public CWEditorLimb limb;

    [SerializeField]
    public Transform backLookingDockingPoint;
    [SerializeField]
    public Transform frontLookingDockingPoint;

    private List<int> removedIndizes;

    public Transform[] oppositeSiteDockingPoints;

    void Awake() {

        for (int index = 0; index < this.availableDockingPoints.Length; index++) {
            this.availableDockingPoints[index].tag = "" + index;
        }

        this.removedIndizes = new List<int>();

        this.limb = this.GetComponentInParent<CWEditorLimb>();
    }

    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    public Transform GetClosestDockingPoint(Vector3 position) {

        float closestDistance = float.MaxValue;
        Transform closestDockingPoint = this.availableDockingPoints[0];


        int closestPointIndex = -1;

        foreach (Transform currentDockingPoint in this.availableDockingPoints) {

            float currentDistance = Vector3.Distance(currentDockingPoint.position, position);

            if (currentDistance < closestDistance) {

                closestDistance = currentDistance;
                closestDockingPoint = currentDockingPoint;
            }

            closestPointIndex++;
        }

        return closestDockingPoint;
    }

    public void RemoveAvailableDockingPoint(Transform closestDockingPoint) {

        List<Transform> tempList = new List<Transform>(this.availableDockingPoints);
        int index = Array.IndexOf(this.availableDockingPoints, closestDockingPoint);

        tempList.RemoveAt(index);

        this.removedIndizes.Add(index);

        this.availableDockingPoints = tempList.ToArray();
    }

    public void RemoveAvailableDockingPointPerIndex(int index) {

        List<Transform> tempList = new List<Transform>(this.availableDockingPoints);

        tempList.RemoveAt(index);

        this.removedIndizes.Add(index);

        this.availableDockingPoints = tempList.ToArray();
    }

    public void RemoveBackLookingDockingPoint(int currentBuildingDockingPointIndex) {
        this.RemoveAvailableDockingPoint(this.oppositeSiteDockingPoints[currentBuildingDockingPointIndex]);
    }

    public void RemoveFrontLookingDockingPoint(int currentBuildingDockingPointIndex) {
        this.RemoveAvailableDockingPointPerIndex(currentBuildingDockingPointIndex);
    }

    public void RemoveDefaultBackLookingDockingPoint() {
        this.RemoveAvailableDockingPoint(this.backLookingDockingPoint);
    }

    public void RemoveDefaultFrontLookingDockingPoint() {
        this.RemoveAvailableDockingPoint(this.frontLookingDockingPoint);
    }

    public CWEditorDockingBallData GetData() {

        //CWEditorDockingBallData ballData = ScriptableObject.CreateInstance<CWEditorDockingBallData>();

        CWEditorDockingBallData ballData = new CWEditorDockingBallData();

        ballData.position = this.transform.position;
        ballData.rotation = this.transform.rotation;

        ballData.removedIndizes = this.removedIndizes;

        return ballData;
    }

    public void InsertDataDeserialize(CWEditorDockingBallData ballData) {

        this.transform.SetPositionAndRotation(ballData.position, ballData.rotation);

        ballData.removedIndizes.ForEach(index => this.RemoveAvailableDockingPointPerIndex(index));

    }
}
