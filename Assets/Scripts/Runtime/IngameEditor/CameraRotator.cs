using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraRotator : MonoBehaviour {
    private Camera m_camera;
    public Transform Target;
    public float Distance = 5.0f;
    public float XSpeed = 5.0f;
    public float YSpeed = 5.0f;

    public float YMinLimit = -360f;
    public float YMaxLimit = 360f;

    public float DistanceMin = .5f;
    public float DistanceMax = 5000f;

    private float m_x = 0.0f;
    private float m_y = 0.0f;

    private float mouseStartX;
    private float mouseStartY;


    private void Awake() {
        m_camera = GetComponent<Camera>();
    }

    private void Start() {
        //SyncAngles();
        this.m_camera.transform.SetPositionAndRotation(Vector3.back * 10, Quaternion.identity);

        //this.mouseStartX = Input.GetAxis("Mouse X");
        //this.mouseStartY = Input.GetAxis("Mouse X");

    }

    public void SyncAngles() {
        Vector3 angles = transform.eulerAngles;
        m_x = angles.y;
        m_y = angles.x;
    }

    private void LateUpdate() {
        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");

        deltaX = deltaX * XSpeed;
        deltaY = deltaY * YSpeed;

        if (Input.GetKey(KeyCode.Mouse2) && !CWEditorController.instance.freezeCam && CWEditorController.instance.navigationState == CWNavigationState.RotatingZooming) {

            m_x += deltaX;
            m_y -= deltaY;
        }

        m_y = ClampAngle(m_y, YMinLimit, YMaxLimit);

        if (!CWEditorController.instance.freezeCam) {
            Zoom();
        }
    }

    public void Zoom() {

        Quaternion rotation = Quaternion.Euler(m_y, m_x, 0);
        transform.rotation = rotation;

        float mwheel = Input.GetAxis("Mouse ScrollWheel");


        if (m_camera != null) {

            if (m_camera.orthographic) {
                m_camera.orthographicSize -= mwheel * m_camera.orthographicSize;
                if (m_camera.orthographicSize < 0.01f) {
                    m_camera.orthographicSize = 0.01f;
                }
            }


            Distance = Mathf.Clamp(Distance - mwheel * Distance, DistanceMin, DistanceMax);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);
            Vector3 position = rotation * negDistance + Target.position;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F) {
            angle += 360F;
        }
        if (angle > 360F) {
            angle -= 360F;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
