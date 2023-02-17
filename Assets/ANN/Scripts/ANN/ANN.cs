using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Artificial Neural Network.
/// Non-MonoBehaviour script.
/// </summary>
public class ANN
{
    /// <summary>
    /// Activation function scale. 1 - default scale.
    /// </summary>
    public float AFS = 1;

    /// <summary>
    /// Bias. If "true", then each connection (neuron) gets its own offset, except input neurons.
    /// </summary>
    public bool B = false;

    /// <summary>
    /// Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to output neurons. If "false" - from 0 to 1.
    /// </summary> 
    public bool AFWM = true;

    /// <summary>
    /// Input layer of ANN. The size of the array indicates the number of input values.
    /// </summary>
    public float[] Input;

    /// <summary>
    /// Output layer of ANN. The size of the array indicates the number of input values.
    /// </summary>
    public float[] Output;

    /// <summary>
    /// Dictionary of the nodes (neurons) of ANN.
    /// </summary>
    public Dictionary<int, ANNNode> Node = new Dictionary<int, ANNNode>();

    /// <summary>
    /// Dictionary of the connections (weights) between nodes (neurons).
    /// </summary>
    public Dictionary<int, ANNConnection> Connection = new Dictionary<int, ANNConnection>();

    /// <summary>
    /// Create ANN using parameters. AFS = 1. AFWM = true.
    /// </summary>
    /// <param name="NumberOfInputs">The number of input neurons.</param>
    /// <param name="NumbersOfOutputs">The number of output neurons.</param>
    public void Create(int NumberOfInputs, int NumbersOfOutputs)
    {
        Create(1, true, NumberOfInputs, NumbersOfOutputs);
    }

    /// <summary>
    /// Create ANN using parameters. AFS = 1.
    /// </summary>
    /// <param name="AFWM">Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to input and output neurons. If "false" - from 0 to 1.</param>
    /// <param name="NumberOfInputs">The number of input neurons.</param>
    /// <param name="NumbersOfOutputs">The number of output neurons.</param>
    public void Create(bool AFWM, int NumberOfInputs, int NumbersOfOutputs)
    {
        Create(1, AFWM, NumberOfInputs, NumbersOfOutputs);
    }

    /// <summary>
    /// Create ANN using parameters. AFWM = true.
    /// </summary>
    /// <param name="AFS">Activation function scale. 1 - default scale.</param>
    /// <param name="NumberOfInputs">The number of input neurons.</param>
    /// <param name="NumbersOfOutputs">The number of output neurons.</param>
    public void Create(float AFS, int NumberOfInputs, int NumbersOfOutputs)
    {
        Create(AFS, true, NumberOfInputs, NumbersOfOutputs);
    }

    /// <summary>
    /// Create ANN using parameters.
    /// </summary>
    /// <param name="AFS">Activation function scale. 1 - default scale.</param>
    /// <param name="AFWM">Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to input and output neurons. If "false" - from 0 to 1.</param>
    /// <param name="NumberOfInputs">The number of input neurons.</param>
    /// <param name="NumbersOfOutputs">The number of output neurons.</param>
    public void Create(float AFS, bool AFWM, int NumberOfInputs, int NumbersOfOutputs)
    {
        Create(AFS, false, true, NumberOfInputs, NumbersOfOutputs);
    }

    /// <summary>
    /// Create ANN using parameters.
    /// </summary>
    /// <param name="AFS">Activation function scale. 1 - default scale.</param>
    /// <param name="B">Bias. If "true", then each connection (neuron) gets its own offset, except input neurons.</param>
    /// <param name="AFWM">Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to input and output neurons. If "false" - from 0 to 1.</param>
    /// <param name="NumberOfInputs">The number of input neurons.</param>
    /// <param name="NumbersOfOutputs">The number of output neurons.</param>
    public void Create(float AFS, bool B, bool AFWM, int NumberOfInputs, int NumbersOfOutputs)
    {
        Node.Clear();
        Connection.Clear();
        this.AFS = AFS;
        this.B = B;
        this.AFWM = AFWM;
        Input = new float[NumberOfInputs];
        Output = new float[NumbersOfOutputs];

        int n = 0;
        float s = 1F / NumberOfInputs;
        while (n < NumberOfInputs)
        {
            Node.Add(n, new ANNNode(new Vector2(0, s / 2F + s * n), -1));
            n++;
        }
        s = 1F / NumbersOfOutputs;
        while (n < NumberOfInputs + NumbersOfOutputs)
        {
            Node.Add(n, new ANNNode(new Vector2(1, s / 2F + s * (n - NumberOfInputs)), 0));
            n++;
        }
    }

    /// <summary>
    /// ANN solution. :)
    /// </summary>
    public void Solution()
    {
        //Reset Fullness
        List<int> NL = new List<int>(Node.Keys);
        int n = 0;
        while (n < Node.Count)
        {
            int temp = NL[n];
            if (temp < Input.Length)
                Node[temp].Fullness = -1;
            else
                Node[temp].Fullness = 0;
            Node[temp].Neuron = 0;
            n++;
        }

        //Inputs
        n = 0;
        while (n < Input.Length)
        {
            Node[n].Neuron = Input[n];
            n++;
        }

        //Neurons adding Fullness
        List<int> WL = new List<int>(Connection.Keys);
        if (Connection.Count > 0)
        {
            int w = 0;
            while (w < Connection.Count)
            {
                int temp = WL[w];
                if (Connection[temp].Enable)
                    Node[Connection[temp].Out].Fullness++;
                w++;
            }
        }
        WL.Clear();

        //Fullness reconfigure
        n = 0;
        while (n < Node.Count)
        {
            if (Node[NL[n]].Fullness == 0)
            {
                Node[NL[n]].Fullness = -1;
            }
            n++;
        }

        //Solution
        bool Finish = false;
        while (!Finish)
        {
            Finish = true;
            n = 0;
            while (n < NL.Count)
            {
                if (Node[NL[n]].Fullness >= 0)
                {
                    int w = 0;
                    int temp = NL[n];
                    while (w < Node[temp].ConnectionIn.Count)
                    {
                        if (Node[Connection[(int)Node[temp].ConnectionIn[w]].In].Fullness == -1)
                        {
                            if (Connection[(int)Node[temp].ConnectionIn[w]].Enable)
                            {
                                //Sumator & Fullness take away
                                Node[temp].Neuron += Connection[(int)Node[temp].ConnectionIn[w]].Weight * Node[Connection[(int)Node[temp].ConnectionIn[w]].In].Neuron;
                                Node[temp].Fullness--;
                            }
                        }
                        w++;
                    }
                }
                n++;
            }

            n = 0;
            while (n < NL.Count)
            {
                int temp = NL[n];
                if (Node[temp].Fullness == -1)
                {
                    Node[temp].Fullness = -2;
                    NL.RemoveAt(n);
                    n--;
                }
                else if (Node[temp].Fullness == 0)
                {
                    Node[temp].Fullness = -1;
                    if (B)
                        Node[temp].Neuron += Node[temp].Bias;
                    Node[temp].Neuron = Formulas.ActivationFunction(Node[temp].Neuron, AFS, AFWM);
                    if (NL.Count > 0)
                    {
                        int l = 0;
                        while (l < Output.Length)
                        {
                            if (temp == l + Input.Length)
                            {
                                NL.RemoveAt(n);
                                n--;
                                l = Output.Length;
                            }
                            l++;
                        }
                    }
                }
                n++;
            }
            if (NL.Count > 0)
                Finish = false;
        }
        NL.Clear();

        //Outputs
        n = Input.Length;
        while (n < Input.Length + Output.Length)
        {
            Output[n - Input.Length] = Node[n].Neuron;
            n++;
        }
    }

    /// <summary>
    /// Saving ANN parameters to a file.
    /// </summary>
    /// <param name="ANNFile">Name of file.</param>
    public void Save(string ANNFile)
    {
        if (ANNFile != "")
        {
            if (!Directory.Exists(Application.dataPath + "/ANN/ANN"))
            {
                Directory.CreateDirectory(Application.dataPath + "/ANN/ANN");
            }
            if (File.Exists(Application.dataPath + "/ANN/ANN/" + ANNFile + ".ann"))
                File.Delete(Application.dataPath + "/ANN/ANN/" + ANNFile + ".ann");

            if (!File.Exists(Application.dataPath + "/ANN/ANN/" + ANNFile + ".ann"))
            {
                StreamWriter SC = File.CreateText(Application.dataPath + "/ANN/ANN/" + ANNFile + ".ann");
                SC.WriteLine("ANN V1.0");
                SC.WriteLine(AFS);
                SC.WriteLine(B);
                SC.WriteLine(AFWM);
                SC.WriteLine(Input.Length);
                SC.WriteLine(Output.Length);
                SC.WriteLine(Node.Count);
                SC.WriteLine(Connection.Count);

                int i = 0;
                while (i < Node.Count)
                {
                    SC.WriteLine(Node[i].ToString());
                    i++;
                }

                i = 0;
                while (i < Connection.Count)
                {
                    SC.WriteLine(Connection[i].ToString());
                    i++;
                }

                SC.Close();
            }
            Debug.Log("ANN saved. File name is: " + ANNFile);
        }
        else
            Debug.LogWarning("ANN not saved. File name not entered.");
    }

    /// <summary>
    /// Load ANN from file.
    /// </summary>
    /// <param name="ANNFile">Name of file.</param>
    public void Load(string ANNFile)
    {
        if (ANNFile != "")
        {
            if (Directory.Exists(Application.dataPath + "/ANN/ANN"))
            {
                if (File.Exists(Application.dataPath + "/ANN/ANN/" + ANNFile + ".ann"))
                {
                    StreamReader SR = File.OpenText(Application.dataPath + "/ANN/ANN/" + ANNFile + ".ann");
                    string Version = SR.ReadLine();
                    if (Version == "ANN V1.0")
                    {
                        Node.Clear();
                        Connection.Clear();
                        AFS = Formulas.StringToInt(SR.ReadLine());
                        B = Formulas.StringToBool(SR.ReadLine());
                        AFWM = Formulas.StringToBool(SR.ReadLine());
                        int NumberOfInputs = Formulas.StringToInt(SR.ReadLine());
                        Input = new float[NumberOfInputs];
                        int NumbersOfOutputs = Formulas.StringToInt(SR.ReadLine());
                        Output = new float[NumbersOfOutputs];

                        int Nodes = Formulas.StringToInt(SR.ReadLine());
                        int Connections = Formulas.StringToInt(SR.ReadLine());

                        int n = 0;
                        while (n < NumberOfInputs)
                        {
                            Node.Add(n, new ANNNode(SR.ReadLine()));
                            n++;
                        }
                        while (n < NumberOfInputs + NumbersOfOutputs)
                        {
                            Node.Add(n, new ANNNode(SR.ReadLine()));
                            n++;
                        }
                        while (n < Nodes)
                        {
                            Node.Add(n, new ANNNode(SR.ReadLine()));
                            n++;
                        }
                        n = 0;
                        while (n < Connections)
                        {
                            Connection.Add(n, new ANNConnection(SR.ReadLine()));
                            n++;
                        }

                        SR.Close();
                        NumberingCorrection();
                        Debug.Log("ANN loaded.");
                    }
                    else if (Version == "PerceptronStatic V1.0")
                    {
                        Node.Clear();
                        Connection.Clear();
                        AFS = Formulas.StringToInt(SR.ReadLine());
                        B = Formulas.StringToBool(SR.ReadLine());
                        AFWM = Formulas.StringToBool(SR.ReadLine());
                        int NumberOfInputs = Formulas.StringToInt(SR.ReadLine());
                        Input = new float[NumberOfInputs];
                        int Nodes = NumberOfInputs;
                        int[] NodesL = new int[Formulas.StringToInt(SR.ReadLine()) + 2];
                        NodesL[0] = NumberOfInputs;
                        int n = 1;
                        int Connections = 0;
                        while (n < NodesL.Length - 1)
                        {
                            NodesL[n] = Formulas.StringToInt(SR.ReadLine());
                            if (n == 0)
                                Connections += NumberOfInputs * NodesL[n];
                            else
                                Connections += NodesL[n - 1] * NodesL[n];
                            Nodes += NodesL[n];
                            n++;
                        }

                        int NumbersOfOutputs = Formulas.StringToInt(SR.ReadLine());
                        Output = new float[NumbersOfOutputs];
                        Nodes += NumbersOfOutputs;
                        NodesL[n] = NumbersOfOutputs;
                        Connections += NodesL[n - 1] * NumbersOfOutputs;
                        n = 0;
                        int NodeOut = 0;
                        int NodeIn = 0;
                        int ConNum = 0;
                        while (n < NodesL.Length)
                        {
                            int r = 0;
                            while (r < NodesL[n])
                            {
                                if (NodeOut == Nodes)
                                    NodeOut = NumberOfInputs;
                                else if (NodeOut == NumberOfInputs)
                                    NodeOut += NumbersOfOutputs;

                                if (NodeIn == Nodes)
                                    NodeIn = NumberOfInputs;
                                else if (NodeIn == NumberOfInputs)
                                    NodeIn += NumbersOfOutputs;

                                float s = 1F / NodesL[n];
                                Node.Add(NodeOut, new ANNNode(new Vector2(n * (1F / (NodesL.Length - 1)), s / 2F + s * r)));
                                if (n > 0)
                                {
                                    int l = 0;
                                    while (l < NodesL[n - 1])
                                    {
                                        Connection.Add(ConNum, new ANNConnection(NodeIn, NodeOut, Formulas.StringToFloat(SR.ReadLine()), true));
                                        Node[NodeOut].ConnectionIn.Add(ConNum);
                                        ConNum++;
                                        NodeIn++;
                                        l++;
                                    }
                                    if (B)
                                    {
                                        Node[NodeOut].Bias = Formulas.StringToFloat(SR.ReadLine());
                                    }
                                    NodeIn -= NodesL[n - 1];
                                }
                                NodeOut++;
                                r++;
                            }
                            if (n > 0)
                                NodeIn += NodesL[n - 1];
                            n++;
                        }

                        SR.Close();
                        NumberingCorrection();
                        Debug.Log("ANN loaded from Perceptron's file.");
                    }
                    else
                    {
                        SR.Close();
                        Debug.LogWarning("ANN not loaded. Unsuitable version of ANN.");
                    }
                }
                else
                    Debug.LogWarning("ANN not loaded. There is no such file name.");
            }
            else
                Debug.LogWarning("ANN not loaded. Folder for the ANN does not exist.");
        }
        else
            Debug.LogWarning("ANN not loaded. File name not entered.");
    }

    /// <summary>
    /// Arrange the numbering of nodes in the connections.
    /// </summary>
    public void FixConnections()
    {
        List<int> NL = new List<int>(Node.Keys);
        int n = 0;
        while (n < NL.Count)
        {
            int temp = NL[n];
            Node[temp].ConnectionIn.Clear();
            if (temp < Input.Length + Output.Length)
                Node[temp].Fullness = -1;
            else
                Node[temp].Fullness = 0;
            n++;
        }

        NL = new List<int>(Connection.Keys);
        n = 0;
        while (n < NL.Count)
        {
            Node[Connection[NL[n]].Out].ConnectionIn.Add(NL[n]);
            n++;
        }
        NL.Clear();
    }

    /// <summary>
    /// Sort node numbering in order.
    /// </summary>
    public void NumberingCorrection()
    {
        Dictionary<int, ANNConnection> TempConnection = new Dictionary<int, ANNConnection>();
        Dictionary<int, ANNNode> TempNode = new Dictionary<int, ANNNode>();

        int n = 0;
        float s = 1F / Input.Length;
        while (n < Input.Length)
        {
            TempNode.Add(n, new ANNNode(new Vector2(0, s / 2F + s * n), -1));
            n++;
        }
        s = 1F / Output.Length;
        while (n < Input.Length + Output.Length)
        {
            TempNode.Add(n, new ANNNode(Node[n].Bias, new Vector2(1, s / 2F + s * (n - Input.Length)), 0));
            n++;
        }

        List<int> WL = new List<int>(Connection.Keys);
        List<int> NL = new List<int>(Node.Keys);

        WL.Sort();
        NL.Sort();

        int w = 0;
        while (w < WL.Count)
        {
            ANNConnection NC = new ANNConnection(Connection[WL[w]].In, Connection[WL[w]].Out, Connection[WL[w]].Weight, Connection[WL[w]].Enable);
            int tempNI = NL.IndexOf(NC.In);
            int tempNO = NL.IndexOf(NC.Out);
            TempConnection.Add(w, new ANNConnection(tempNI, tempNO, NC.Weight, NC.Enable));
            if (!TempNode.ContainsKey(tempNI))
            {
                ANNNode NN = Node[NC.In];
                TempNode.Add(tempNI, new ANNNode(NN.Bias, NN.Position, NN.Fullness));
            }
            if (!TempNode.ContainsKey(tempNO))
            {
                ANNNode NN = Node[NC.Out];
                TempNode.Add(tempNO, new ANNNode(NN.Bias, NN.Position, NN.Fullness));
            }
            w++;
        }

        NL.Clear();
        WL.Clear();

        Connection.Clear();
        n = 0;
        while (n < TempConnection.Count)
        {
            Connection.Add(n, TempConnection[n]);
            n++;
        }
        TempConnection.Clear();

        Node.Clear();
        n = 0;
        while (n < TempNode.Count)
        {
            Node.Add(n, new ANNNode(TempNode[n].Bias, TempNode[n].Position));
            n++;
        }
        TempNode.Clear();
        FixConnections();
    }
}