using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWEditorRotationInterface : MonoBehaviour
{
    public CWEditorRotationInterfaceSingleAxis xRotationInterface;
    public CWEditorRotationInterfaceSingleAxis yRotationInterface;

    public CWRotationInterfaceData GetData() {
        return new CWRotationInterfaceData(this.xRotationInterface.GetAngles(), this.yRotationInterface.GetAngles());
    }

    public void SetDataDeserialize(CWRotationInterfaceData rotationData) {

        this.xRotationInterface.SetAnglesDeserialize(rotationData.minMaxRotationX);
        this.yRotationInterface.SetAnglesDeserialize(rotationData.minMaxRotationY);
    }
}
