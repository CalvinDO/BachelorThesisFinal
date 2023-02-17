using UnityEngine;

/// <summary>
/// Fulfilling cyclic and alike tasks.
/// </summary>
public static class Formulas
{
    /// <summary>
    /// Random value from -r to r.
    /// </summary>
    /// <param name="r">Float value.</param>
    /// <returns>Return float value.</returns>
    public static float Randomizer(float r)
    {
        r = Random.Range(-r, r);
        return r;
    }

    /// <summary>
    /// Transform string to float value.
    /// </summary>
    /// <param name="S">Float value in string.</param>
    /// <returns>Return float value.</returns>
    public static float StringToFloat(string S)
    {
        float I;
        if (float.TryParse(S, out I) == false)
        {
            I = 0;
        }
        return I;
    }

    /// <summary>
    /// Transform string to int value.
    /// </summary>
    /// <param name="S">Int value in string.</param>
    /// <returns>Return int value.</returns>
    public static int StringToInt(string S)
    {
        int I;
        if (int.TryParse(S, out I) == false)
        {
            I = 0;
        }
        return I;
    }

    /// <summary>
    /// Transform string to bool value.
    /// </summary>
    /// <param name="S">Bool value in string.</param>
    /// <returns>Return bool value.</returns>
    public static bool StringToBool(string S)
    {
        bool I;
        if (bool.TryParse(S, out I) == false)
        {
            I = false;
        }
        return I;
    }

    /// <summary>
    /// Float[][][] array. Transfer values from array.
    /// </summary>
    /// <param name="From">From what float[][][] array.</param>
    /// <returns>Return float[][][] array.</returns>
    public static float[][][] FromArray(float[][][] From)
    {
        float[][][] To = new float[From.Length][][];
        int l = 0;
        while (l < From.Length)
        {
            To[l] = new float[From[l].Length][];
            int k = 0;
            while (k < From[l].Length)
            {
                To[l][k] = new float[From[l][k].Length];
                int j = 0;
                while (j < From[l][k].Length)
                {
                    To[l][k][j] = From[l][k][j];
                    j++;
                }
                k++;
            }
            l++;
        }
        return To;
    }

    /// <summary>
    /// Float[] array. Transfer values from array.
    /// </summary>
    /// <param name="From">From what float[] array.</param>
    /// <returns>Return float[] array.</returns>
    public static float[] FromArray(float[] From)
    {
        float[] To = new float[From.Length];
        int i = 0;
        while (i < From.Length)
        {
            To[i] = From[i];
            i++;
        }
        return To;
    }

    /// <summary>
    /// Int[] array. Transfer values from array.
    /// </summary>
    /// <param name="From">From what int[] array.</param>
    /// <returns>Return int[] array.</returns>
    public static int[] FromArray(int[] From)
    {
        int[] To = FromArray(From, 0);
        return To;
    }

    /// <summary>
    /// Int[] array + corretion. Transfer values from array.
    /// </summary>
    /// <param name="From">From what int[] array.</param>
    /// <param name="Corection">Int value. Add to each value in array.</param>
    /// <returns>Return int[] array.</returns>
    public static int[] FromArray(int[] From, int Corection)
    {
        int[] To = new int[From.Length];
        int i = 0;
        while (i < From.Length)
        {
            To[i] = From[i] + Corection;
            i++;
        }
        return To;
    }

    /// <summary>
    /// Transfer values from one array to another.
    /// </summary>
    /// <param name="From">From array.</param>
    /// <param name="To">To array.</param>
    public static void FromArray(float[] From, float[] To)
    {
        int i = 0;
        while (i < From.Length)
        {
            To[i] = From[i];
            i++;
        }
    }

    /// <summary>
    /// Activation function of the neuron.
    /// </summary>
    /// <param name="Sum">The sum of all values of neurons multiplied with weight.</param>
    /// <param name="AFS">Activation function scale. 1 - default scale.</param>
    /// <param name="AFWM">Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to output neurons. If "false" - from 0 to 1.</param>
    /// <returns>Returns float value.</returns>
    public static float ActivationFunction(float Sum, float AFS, bool AFWM)
    {
        int Minus = 0;
        if (AFWM)
            Minus = 1;
        return Sum = (1F + Minus) / (1F + Mathf.Exp(-(AFS * Sum - (1 - Minus) / 2F))) - Minus;        //Sigmoid function
    }
}
