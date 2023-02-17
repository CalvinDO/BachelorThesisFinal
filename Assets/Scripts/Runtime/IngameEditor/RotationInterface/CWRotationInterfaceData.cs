using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CWRotationInterfaceRotationalFreedom {
    X = 1,
    Y = 2,
    XY = 3,
    Nothing = 4
}

[Serializable]
public class CWRotationInterfaceData {

    public CWRotationInterfaceRotationalFreedom rotationalFreedom;

    public Vector2 minMaxRotationX;
    public Vector2 minMaxRotationY;

    const float blockedThreshold = 5f;

    public CWRotationInterfaceData(Vector2 _minMaxRotationX, Vector2 _minMaxRotationY) {

        this.minMaxRotationX = _minMaxRotationX;
        this.minMaxRotationY = _minMaxRotationY;


        if (this.IsYBlocked()) {

            if (this.IsXBlocked()) {

                this.rotationalFreedom = CWRotationInterfaceRotationalFreedom.Nothing;
                return;
            }

            this.rotationalFreedom = CWRotationInterfaceRotationalFreedom.X;
            return;
        }

        if (this.IsXBlocked()) {

            this.rotationalFreedom = CWRotationInterfaceRotationalFreedom.Y;
            return;
        }

        this.rotationalFreedom = CWRotationInterfaceRotationalFreedom.XY;
    }

    public bool IsYBlocked() {
        return (this.minMaxRotationY[1] - this.minMaxRotationY[0]) < blockedThreshold;
    }

    public bool IsXBlocked() {
        return (this.minMaxRotationX[1] - this.minMaxRotationX[0]) < blockedThreshold;
    }

    public void Print() {
        Debug.Log("minX= " + this.minMaxRotationX[0] + "   maxX= " + this.minMaxRotationX[1]);
        Debug.Log("minY= " + this.minMaxRotationY[0] + "   maxY= " + this.minMaxRotationY[1]);
    }
}
