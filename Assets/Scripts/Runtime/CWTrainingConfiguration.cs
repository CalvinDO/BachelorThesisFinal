
using UnityEngine;
using UnityEditor;
using System;




[CreateAssetMenu(menuName = "NewTrainingConfiguration")]
[Serializable]
public class CWTrainingConfiguration : ScriptableObject {

    [Serializable]
    public enum CWTrainingInputType {
        firstType, secondType
    }
    [Serializable]
    public enum CWTrainingOutputType {
        firstType, secondType
    }
    [Serializable]
    public enum CWTrainingDelinearizationType {
        firstType, secondType
    }
    [Serializable]
    public enum CWTrainingFitnessFunctionType {
        firstType, secondType
    }
    [Serializable]
    public enum CWTrainingHyperparameterType {
        firstType, secondType
    }

    public CWTrainingInputType inputType;
    public CWTrainingOutputType outputType;
    public CWTrainingDelinearizationType delinearizationType;
    public CWTrainingFitnessFunctionType fitnessFunctionType;
    public CWTrainingHyperparameterType hyperparameterType;
}