using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CWEditorRotationInterfaceSingleAxis : MonoBehaviour {

    public Transform minRotationHandle;
    public Transform maxRotationHandle;

    public Image minImage;
    public Image maxImage;


    // Update is called once per frame
    void Update() {

        float minAngle = this.GetAngles()[0];
        float maxAngle = this.GetAngles()[1];

        float minFillFactor = -minAngle / 360f;
        this.minImage.fillAmount = minFillFactor;

        float maxFillFactor = maxAngle/ 360f;
        this.maxImage.fillAmount = maxFillFactor;


        /*
        //solve different: block in gizmocode
        if (minAngle > -0.5) {
            this.minRotationHandle.transform.Rotate(0, 0, -minAngle);
        }

        if (maxAngle < 0.5) {
            this.maxRotationHandle.transform.Rotate(0, 0, maxAngle);
        }
        */
    }

    public Vector2 GetAngles() {

        return new Vector2(this.minRotationHandle.transform.localRotation.eulerAngles.z - 360f, this.maxRotationHandle.transform.localRotation.eulerAngles.z);
    }

    public void SetAnglesDeserialize(Vector2 minMaxRotation) {

        this.minRotationHandle.transform.localRotation = Quaternion.Euler(0, 0, minMaxRotation[0] + 360f);
        this.maxRotationHandle.transform.localRotation = Quaternion.Euler(0, 0, minMaxRotation[1]);

    }
}
