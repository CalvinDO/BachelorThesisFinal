using System.Collections;
using UnityEngine;

/// <summary>
/// Artificial Neural Network node.
/// Non-MonoBehaviour script.
/// </summary>
public class ANNNode
{
    /// <summary>
    /// The value of neuron.
    /// </summary>
    public float Neuron = 0;

    /// <summary>
    /// The value of neuron's bias.
    /// </summary>
    public float Bias = 0;

    /// <summary>
    /// List of input connections numbers.
    /// </summary>
    public ArrayList ConnectionIn;

    /// <summary>
    /// The fullness of the neuron connections when solving.
    /// </summary>
    public int Fullness;

    /// <summary>
    /// Position at visualization.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Artificial Neural Network node.
    /// </summary>
    /// <param name="Position">Position at visualization.</param>
    public ANNNode(Vector2 Position)
    {
        Neuron = 0;
        this.Position = Position;
        Fullness = 0;
        ConnectionIn = new ArrayList();
        Bias = 0;
    }

    /// <summary>
    /// Artificial Neural Network node.
    /// </summary>
    /// <param name="Bias">Neuron's bias.</param>
    /// <param name="Position">Position at visualization.</param>
    public ANNNode(float Bias, Vector2 Position)
    {
        Neuron = 0;
        this.Position = Position;
        Fullness = 0;
        ConnectionIn = new ArrayList();
        this.Bias = Bias;
    }

    /// <summary>
    /// Artificial Neural Network node.
    /// </summary>
    /// <param name="Position">Position at visualization.</param>
    /// <param name="Fullness">The fullness of the neuron connections when solving.</param>
    public ANNNode(Vector2 Position, int Fullness)
    {
        Neuron = 0;
        this.Position = Position;
        this.Fullness = Fullness;
        ConnectionIn = new ArrayList();
        Bias = 0;
    }

    /// <summary>
    /// Artificial Neural Network node.
    /// </summary>
    /// <param name="Bias">Neuron's bias.</param>
    /// <param name="Position">Position at visualization.</param>
    /// <param name="Fullness">The fullness of the neuron connections when solving.</param>
    public ANNNode(float Bias, Vector2 Position, int Fullness)
    {
        Neuron = 0;
        this.Position = Position;
        this.Fullness = Fullness;
        ConnectionIn = new ArrayList();
        this.Bias = Bias;
    }

    /// <summary>
    /// Node's info to string.
    /// </summary>
    public override string ToString()
    {
        string Weights = "";
        int w = 0;
        while (w < ConnectionIn.Count)
        {
            Weights += ";";
            Weights += ConnectionIn[w];
            w++;
        }
        return (Bias + ";" + Position.x + ";" + Position.y + Weights);
    }

    /// <summary>
    /// Load node's info from string.
    /// </summary>
    /// <param name="Info">Node's string info.</param>
    public ANNNode(string Info)
    {
        if (ConnectionIn != null)
            ConnectionIn.Clear();
        else
            ConnectionIn = new ArrayList();
        string I = "";
        int c = 0;
        int stage = 0;
        while (c < Info.Length)
        {
            if (Info[c] != ';' && c != Info.Length - 1)
                I += Info[c];
            else
            {
                if (c == Info.Length - 1)
                    I += Info[c];
                if (stage == 0)
                    Bias = Formulas.StringToFloat(I);
                else if (stage == 1)
                    Position.x = Formulas.StringToFloat(I);
                else if (stage == 2)
                    Position.y = Formulas.StringToFloat(I);
                else
                    ConnectionIn.Add(Formulas.StringToInt(I));
                I = "";
                stage++;
            }
            c++;
        }
    }
}