using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeGizmos;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using TMPro;

public enum CWEditorState {
    Navigating = 1,
    BuildingLimb = 2,
    ViewingRotations = 3,
    SettingRotations = 4,
}

public enum CWNavigationState {
    RotatingZooming = 1,
    Moving = 2
}


public class CWEditorController : MonoBehaviour {


    public bool lockMouse;
    public Camera mainCamera;
    public CameraRotator cameraRotator;
    public Transform camRotatorPivot;

    [Range(.1f, 1)]
    public float cameraMoveSpeed;


    public CWEditorLimb limb;
    public GameObject limbPreview;


    public CWEditorState editorState;
    public CWNavigationState navigationState;

    [HideInInspector]
    public CWEditorLimb currentBuildingLimb;

    [Range(1, 10)]
    public float limbProlongingFactor;


    public static CWEditorController instance;

    public CWEditorLimb startingLimb;

    public TransformGizmo transformGizmo;

    public bool freezeCam;

    public PhysicMaterial limbPhysicMaterial;
    private bool editorIsClosing;

    private int currentLoadIndex = 0;

    public TextMeshProUGUI slotDisplay;


    public CWEditorDockingBall currentBuildingDockingBall;
    public Transform currentBuildingDockingPoint;

    public TMP_InputField configIndexInput;

    void Awake() {
        CWEditorController.instance = this;
    }

    void Start() {

        this.editorState = CWEditorState.Navigating;

        Cursor.lockState = this.lockMouse ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void Update() {

        this.CheckMousePress();

        switch (this.editorState) {
            case CWEditorState.Navigating:

                this.freezeCam = false;

                this.CheckSwitchBuildingLimb();
                this.CheckSwitchViewingRotation();

                this.MoveCam();

                break;
            case CWEditorState.BuildingLimb:

                this.CheckABortBuildingLimb();

                try {
                    this.ManageBuildingLimb();
                }
                catch (NullReferenceException nre) {
                    Debug.Log("limbBuilding aborted");
                }

                this.MoveCam();

                break;
            case CWEditorState.ViewingRotations:

                this.freezeCam = false;

                this.CheckSwitchSettingRotation();
                this.CheckEndRotationInterface();

                this.MoveCam();

                break;
            case CWEditorState.SettingRotations:

                this.freezeCam = true;

                this.UpdateRotationInterface(this.startingLimb);
                this.CheckEndRotationInterface();


                break;
            default:
                break;
        }
    }

    private void CheckABortBuildingLimb() {

        if (Input.GetKeyUp(KeyCode.Mouse1)) {

            this.AbortBuildingLimb();
        }
    }

    private void CheckSwitchViewingRotation() {

        if (Input.GetKeyUp(KeyCode.R)) {

            this.ToggleRotationInterface(this.startingLimb);

            this.SetStateViewingRotations();
        }
    }

    private void CheckSwitchSettingRotation() {

        if (TransformGizmo.instance.isTransforming) {
            this.SetStateSettingRotations();
        }
    }

    private void CheckEndRotationInterface() {

        if (Input.GetKeyUp(KeyCode.R)) {

            this.ToggleRotationInterface(this.startingLimb);

            this.SetStateNavigating();
        }
        else if (!TransformGizmo.instance.isTransforming) {
            this.SetStateViewingRotations();
        }
    }

    public void SetStateNavigating() {
        this.editorState = CWEditorState.Navigating;
    }

    private void SetStateBuildingLimb() {

        this.editorState = CWEditorState.BuildingLimb;
        this.limbPreview.SetActive(false);
    }


    private void SetStateViewingRotations() {

        this.editorState = CWEditorState.ViewingRotations;
        this.limbPreview.SetActive(false);
    }

    private void SetStateSettingRotations() {
        this.editorState = CWEditorState.SettingRotations;
        this.limbPreview.SetActive(false);
    }

    private void UpdateRotationInterface(CWEditorLimb _limb) {

        if (_limb.isDockedLimb) {
            _limb.UpdateRotationData();
        }

        if (_limb.childLimbs.Count > 0) {
            this.UpdateRotationInterfaces(_limb.childLimbs);
        }
    }

    private void UpdateRotationInterfaces(List<CWEditorLimb> _limbs) {

        foreach (CWEditorLimb currentLimb in _limbs) {
            this.UpdateRotationInterface(currentLimb);
        }
    }



    private void ToggleRotationInterface(CWEditorLimb _limb) {

        if (_limb.isDockedLimb) {
            _limb.rotationInterface.gameObject.SetActive(!_limb.rotationInterface.gameObject.activeInHierarchy);
        }

        if (_limb.childLimbs.Count > 0) {
            this.ToggleRotationInterfaces(_limb.childLimbs);
        }
    }
    private void ToggleRotationInterfaces(List<CWEditorLimb> _limbs) {

        foreach (CWEditorLimb currentLimb in _limbs) {
            this.ToggleRotationInterface(currentLimb);
        }
    }


    private void MoveCam() {

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse2)) {
            this.navigationState = CWNavigationState.Moving;

            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            this.camRotatorPivot.position -= this.cameraMoveSpeed * deltaX * this.mainCamera.transform.right;
            this.camRotatorPivot.position -= this.cameraMoveSpeed * deltaY * this.mainCamera.transform.up;
        }
        else {
            this.navigationState = CWNavigationState.RotatingZooming;
        }
    }

    private void AbortBuildingLimb() {

        this.currentBuildingLimb.parentLimb.childLimbs.Remove(this.currentBuildingLimb);
        GameObject.Destroy(this.currentBuildingLimb.gameObject);
        this.currentBuildingLimb = null;

        this.editorState = CWEditorState.Navigating;
    }

    private void ManageBuildingLimb() {

        RaycastHit hit;
        Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);

        LayerMask mask = LayerMask.GetMask("RaycastCatcher");


        if (Physics.Raycast(ray, out hit, 99999, mask)) {

            Vector3 limbToHit = hit.point - this.currentBuildingLimb.transform.position;
            float dotWithForward = Vector3.Dot(limbToHit, this.currentBuildingLimb.transform.forward);

            if (dotWithForward < 0) {
                dotWithForward = 0;
            }

            this.currentBuildingLimb.SetLength(dotWithForward * this.limbProlongingFactor);
        }
    }

    private void CheckSwitchBuildingLimb() {

        if (this.editorIsClosing) {
            return;
        }

        RaycastHit hit;
        Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);

        LayerMask mask = LayerMask.GetMask("Creature");


        if (Physics.Raycast(ray, out hit, 50, mask)) {

            this.limbPreview.SetActive(true);
            this.HandleLimbHit(hit);
        }
        else {
            this.limbPreview.SetActive(false);
        }
    }

    private void CheckMousePress() {
        /*
        if (Input.GetKey(KeyCode.Mouse0)) {
            this.mouse0Pressed = true;
        }
        */
    }

    public void RemoveCurrentBuildingDockingPoint() {
        this.currentBuildingDockingBall.RemoveAvailableDockingPoint(this.currentBuildingDockingPoint);
    }

    void HandleLimbHit(RaycastHit hit) {

        CWEditorDockingBall hitDockingBall = hit.transform.GetComponent<CWEditorDockingBall>();

        Transform closestDockingPointTransform = hitDockingBall.GetClosestDockingPoint(hit.point);

        Vector3 dockingPosition = closestDockingPointTransform.position;
        this.limbPreview.transform.position = dockingPosition;

        Quaternion dockingRotation = closestDockingPointTransform.rotation;
        this.limbPreview.transform.rotation = dockingRotation;



        if (Input.GetKeyUp(KeyCode.Mouse0)) {

            this.DockLimb(dockingPosition, dockingRotation, hitDockingBall.limb, hitDockingBall.transform);

            this.currentBuildingDockingBall = hitDockingBall;
            this.currentBuildingDockingPoint = closestDockingPointTransform;

            SetStateBuildingLimb();
        }
    }

    private void DockLimb(Vector3 position, Quaternion rotation, CWEditorLimb parent, Transform ballDockedAt) {

        this.currentBuildingLimb = GameObject.Instantiate(this.limb, position, rotation, parent.transform);
        this.currentBuildingLimb.SetParentLimb(parent);
        this.currentBuildingLimb.SetBallDockedAt(ballDockedAt);
    }

    public void OnClickStart() {

        this.editorIsClosing = true;

        this.PresetConfigurationIndex();

        GameObject physicalCreature = CWEditorPhysicalCreatureFactory.GetPhysicalCreature(this.startingLimb);

        this.SwitchScene(physicalCreature);
    }

    private void PresetConfigurationIndex() {
        try {
            CWTrainingManagerDataCollector.presetConfigIndex = int.Parse(this.configIndexInput.text);

            if (CWTrainingManagerDataCollector.instance.currentConfigurationIndex < 0 || CWTrainingManagerDataCollector.instance.currentConfigurationIndex > 4) {
                throw new Exception("index out of range");
            }

            CWTrainingManagerDataCollector.copyConfigIndexFromPreset = true;
        }
        catch (Exception e) {
            CWTrainingManagerDataCollector.copyConfigIndexFromPreset = false;
        }
    }

    public void OnClickSave() {

        string serializedLimb = JsonUtility.ToJson(this.startingLimb.GetData());

        this.WriteToFreeSlot(serializedLimb, 0);
    }

    private void WriteToFreeSlot(string serializedLimb, int index) {


        string currentPath = Application.dataPath + "/SavedCreatures/" + index + ".json";

        if (!File.Exists(currentPath) || File.ReadAllText(currentPath) == "") {


            File.WriteAllText(currentPath, serializedLimb);


            return;
        }


        this.WriteToFreeSlot(serializedLimb, ++index);
    }


    public void OnClickLoad() {


        string currentPath = Application.dataPath + "/SavedCreatures/" + this.currentLoadIndex + ".json";

        this.UpdateSlotDisplay();


        this.currentLoadIndex++;

        string increasedPath = Application.dataPath + "/SavedCreatures/" + this.currentLoadIndex + ".json";

        if (!File.Exists(increasedPath)) {
            this.currentLoadIndex = 0;
        }


        GameObject.Destroy(this.startingLimb.gameObject);

        this.startingLimb = CWEditorController.GetDeserializedLimbFromPath(currentPath);
    }

    public static CWEditorLimb GetDeserializedLimbFromPath(string currentPath) {

        string deserializedLimbString = File.ReadAllText(currentPath);

        CWEditorLimbData newLimbData = new CWEditorLimbData();

        JsonUtility.FromJsonOverwrite(deserializedLimbString, newLimbData);

        CWEditorLimb deserializedStartingLimb = GameObject.Instantiate(CWEditorController.instance.limb);
        deserializedStartingLimb.InsertDataDeserialize(newLimbData);

        return deserializedStartingLimb;
    }

    private void UpdateSlotDisplay() {
        this.slotDisplay.text = "Current Slot: " + this.currentLoadIndex;
    }


    private void SwitchScene(GameObject physicalCreature) {

        DontDestroyOnLoad(physicalCreature);

        SceneManager.LoadScene(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
    }
}