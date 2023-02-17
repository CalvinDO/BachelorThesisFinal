/// <summary>
/// Connection between neurons.
/// Non-MonoBehaviour script.
/// </summary>
public class ANNConnection
{
    /// <summary>
    /// Input neuron number.
    /// </summary>
    public int In;

    /// <summary>
    /// Output neuron number.
    /// </summary>
    public int Out;

    /// <summary>
    /// Connection weight.
    /// </summary>
    public float Weight;

    /// <summary>
    /// Connection activity.
    /// </summary>
    public bool Enable = true;

    /// <summary>
    /// Connection between neurons.
    /// </summary>
    /// <param name="In">Input neuron number.</param>
    /// <param name="Out">Output neuron number.</param>
    /// <param name="Weight">Connection weight.</param>
    /// <param name="Enable">Connection activity.</param>
    public ANNConnection(int In, int Out, float Weight, bool Enable)
    {
        this.In = In;
        this.Out = Out;
        this.Weight = Weight;
        this.Enable = Enable;
    }

    /// <summary>
    /// Connection between neurons. Connection activity (Enable) = true.
    /// </summary>
    /// <param name="In">Input neuron number.</param>
    /// <param name="Out">Output neuron number.</param>
    /// <param name="Weight">Connection weight.</param>
    public ANNConnection(int In, int Out, float Weight)
    {
        this.In = In;
        this.Out = Out;
        this.Weight = Weight;
        Enable = true;
    }

    /// <summary>
    /// Connection between neurons. Connection weight (Weight) = 0. Connection activity (Enable) = true.
    /// </summary>
    /// <param name="In">Input neuron number.</param>
    /// <param name="Out">Output neuron number.</param>
    public ANNConnection(int In, int Out)
    {
        this.In = In;
        this.Out = Out;
        Weight = 0;
        Enable = true;
    }

    /// <summary>
    /// Connection's info to string.
    /// </summary>
    public override string ToString()
    {
        return (In + ";" + Out + ";" + Weight + ";" + Enable);
    }

    /// <summary>
    /// Load connection's info from string.
    /// </summary>
    /// <param name="Info">Connection's string info.</param>
    public ANNConnection(string Info)
    {
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
                    In = Formulas.StringToInt(I);
                else if (stage == 1)
                    Out = Formulas.StringToInt(I);
                else if (stage == 2)
                    Weight = Formulas.StringToFloat(I);
                else
                    Enable = Formulas.StringToBool(I);
                I = "";
                stage++;
            }
            c++;
        }
    }
}