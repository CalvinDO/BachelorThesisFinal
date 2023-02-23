
using UnityEngine;
using UnityEditor;
using System;




[CreateAssetMenu(menuName = "NewTrainingConfiguration")]
[Serializable]
public class CWTrainingConfiguration : ScriptableObject {

    [Serializable]
    public enum CWTrainingInputType {
        comDistances = 0,
        velocityDifferences = 1
    }

    [Serializable]
    public enum CWTrainingOutputType {
        anglesOnly = 0,
        anglesWithForces = 1
    }

    [Serializable]
    public enum CWTrainingDelinearizationType {
        tanh = 0,
        none = 1
    }

    [Serializable]
    public enum CWTrainingFitnessFunctionType {
        zPosOnly = 0,
        alsoPunishX = 1
    }

    [Serializable]
    public enum CWTrainingHyperparameterType {
        crossingOf = 0,
        crossingOn = 1
    }

    public CWTrainingInputType inputType;
    public CWTrainingDelinearizationType delinearizationType;
    public CWTrainingHyperparameterType hyperparameterType;
    public CWTrainingFitnessFunctionType fitnessFunctionType;
    public CWTrainingOutputType outputType;
}