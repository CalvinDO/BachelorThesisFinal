using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGizmos {


    public class CWEditorRotationInterfaceHandleController : MonoBehaviour {

        public CWEditorRotationInterfaceHandleController otherHandle;

        void Start() {

        }

        // Update is called once per frame
        void Update() {

            if (this.CompareTag("HandleIsTransformed")) {
                otherHandle.transform.localRotation = Quaternion.Euler(this.transform.localRotation.eulerAngles.x, this.transform.localRotation.eulerAngles.y, -this.transform.localRotation.eulerAngles.z);
            }
        }
    }
}
