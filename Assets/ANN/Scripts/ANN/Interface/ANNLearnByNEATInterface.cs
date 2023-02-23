using UnityEngine;

/// <summary>
/// Interface of NeuroEvolution of Augmenting Topologies method.
/// </summary>
public class ANNLearnByNEATInterface : MonoBehaviour
{
    /// <summary>
    /// Artificial neural network.
    /// </summary>
    public ANN Ann;

    /// <summary>
    /// NEAT learning method.
    /// </summary>
    public ANNLearnByNEAT NL = new ANNLearnByNEAT();

    /// <summary>
    /// Is ANN learning?
    /// </summary>
    public bool Learn = false;

    private Rect WindowRect = new Rect(Screen.width, Screen.height, 580, 305);
    private bool Settings = true;

    public static ANNLearnByNEATInterface instance;

    void Awake() {

        ANNLearnByNEATInterface.instance = this;

        this.NL.Init();
    }

    private void Start()
    {
        if (gameObject.name == "StudentChild")
            Destroy(gameObject.GetComponent<ANNLearnByNEATInterface>());
        else
        {
            if (Ann == null)
                Debug.LogWarning("NEAT learn interface does not have ANN.");
            else
                NL.Ann = Ann;
        }

    }

    private void Update()
    {
        if (Learn && Ann != null)
            NL.Learn();
    }

    private void OnGUI()
    {
        if (Ann != null)
        {
            WindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRect, WindowInterface, "NEAT in " + transform.name);  // interface window
        }
    }

    private void WindowInterface(int ID)                                                    // interface window
    {
        if (WindowRect.height == 65)                                                        // small window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "▄"))                // change window scale
            {
                if (Settings)
                {
                    WindowRect.width = 580;
                    WindowRect.height = 305;
                    WindowRect.x = WindowRect.x - 380;
                }
                else
                {
                    WindowRect.width = 200;
                    WindowRect.height = 215;
                }
            }
            if (NL.MutationAddNeuron == 0 && NL.MutationAddWeight == 0 && NL.MutationChangeOneWeight == 0 && NL.MutationChangeWeights == 0)
                GUI.enabled = false;
            bool Activate = false;
            Learn = InterfaceGUI.Button(1, 1, "Learn OFF", "Learn ON", Learn, ref Activate);

            if (Activate && !Learn)
                NL.StopLearn();

            GUI.enabled = true;
        }
        else                                                                                // big window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "-"))                // change window scale
            {
                WindowRect.width = 200;
                WindowRect.height = 65;
                if (Settings)
                    WindowRect.x = WindowRect.x + 380;
            }

            if (GUI.Button(new Rect(WindowRect.width - 40, 0, 20, 20), "S"))                // Settings on/off
            {
                if (Settings)
                {
                    WindowRect.width = 200;
                    WindowRect.height = 215;
                    WindowRect.x = WindowRect.x + 380;
                    Settings = false;
                }
                else
                {
                    WindowRect.width = 580;
                    WindowRect.height = 305;
                    WindowRect.x = WindowRect.x - 380;
                    Settings = true;
                }
            }
            if (Settings)
            {
                if (Learn)
                    GUI.enabled = false;
                NL.AmountOfChildren = InterfaceGUI.IntArrows(2, 1, "Children amount", NL.AmountOfChildren, 1);
                if (NL.AmountOfChildren > 1)
                {
                    if (NL.AmountOfChildren != NL.ChildrenInWave)
                        NL.ChildrenByWave = InterfaceGUI.Button(3, 1, "Maximum in one time", "By waves", NL.ChildrenByWave);
                    NL.ChildrenInWave = InterfaceGUI.IntArrows(3, 2, "Children in wave", NL.ChildrenInWave, 1, NL.AmountOfChildren);
                }

                NL.Cross = InterfaceGUI.Button(2, 2, "Crossing OFF", "Crossing ON", NL.Cross);
                NL.PerceptronStart = InterfaceGUI.Button(2, 3, "No weights", "Perceptron", NL.PerceptronStart);

                NL.MutationSum = NL.MutationAddNeuron + NL.MutationAddWeight + NL.MutationChangeOneWeight + NL.MutationChangeWeights + NL.MutationChangeOneBias + NL.MutationChangeBias;

                if (NL.MutationSum == 0)
                    NL.MutationSum = 1;

                NL.MutationAddWeight = InterfaceGUI.HorizontalSlider(2, 4, "Ratio add weight", NL.MutationAddWeight, NL.MutationAddWeight / NL.MutationSum * 100F, 0F, 1F);
                NL.MutationChangeOneWeight = InterfaceGUI.HorizontalSlider(2, 5, "Ratio change 1 weight", NL.MutationChangeOneWeight, NL.MutationChangeOneWeight / NL.MutationSum * 100F, 0F, 1F);
                NL.MutationChangeWeights = InterfaceGUI.HorizontalSlider(2, 6, "Ratio change weights", NL.MutationChangeWeights, NL.MutationChangeWeights / NL.MutationSum * 100F, 0F, 1F);
                NL.MutationAddNeuron = InterfaceGUI.HorizontalSlider(2, 7, "Ratio add neuron", NL.MutationAddNeuron, NL.MutationAddNeuron / NL.MutationSum * 100F, 0F, 1F);

                if (Ann.B)
                {
                    NL.MutationChangeOneBias = InterfaceGUI.HorizontalSlider(2, 8, "Ratio change 1 bias", NL.MutationChangeOneBias, NL.MutationChangeOneBias / NL.MutationSum * 100F, 0F, 1F);
                    NL.MutationChangeBias = InterfaceGUI.HorizontalSlider(2, 9, "Ratio change bias", NL.MutationChangeBias, NL.MutationChangeBias / NL.MutationSum * 100F, 0F, 1F);
                }

                NL.IgnoreCollision = InterfaceGUI.Button(3, 3, "Collision ON", "Collision OFF", NL.IgnoreCollision);

                if (NL.MutationAddWeight != 0)
                    NL.AddingWeightsCount = InterfaceGUI.HorizontalSlider(3, 4, "Maximum count of weights", NL.AddingWeightsCount, 1, 4);
                if (NL.MutationChangeOneWeight != 0 || NL.MutationChangeWeights != 0 || NL.MutationChangeOneBias != 0 || NL.MutationChangeBias != 0)
                    NL.ChangeWeightSign = InterfaceGUI.HorizontalSlider(3, 5, "Chance change sign", NL.ChangeWeightSign, NL.ChangeWeightSign * 100, 0F, 1F);
                if (NL.PerceptronStart || NL.MutationAddWeight != 0 || NL.MutationChangeOneWeight != 0 || NL.MutationChangeWeights != 0 || NL.MutationChangeOneBias != 0 || NL.MutationChangeBias != 0)
                    NL.ChildrenDifference = InterfaceGUI.HorizontalSlider(3, 6, "Children difference", NL.ChildrenDifference, 0.01F, 10F);

                NL.ChanceCoefficient = InterfaceGUI.HorizontalSlider(1, 5, "Chance coefficient", NL.ChanceCoefficient, 0F, 0.5F);

                GUI.enabled = true;

                if (NL.ChanceCoefficient != 0)
                    InterfaceGUI.Info(1, 6, "Chance", NL.Chance);
                NL.Autosave = InterfaceGUI.Button(3, 7, "Autosave OFF", "Autosave ON", NL.Autosave);
                if (NL.Autosave)
                {
                    NL.AutosaveStep = InterfaceGUI.IntArrows(3, 8, "Autosave step", NL.AutosaveStep, 1);
                    NL.AutosaveName = GUI.TextField(new Rect(390, 265, 180, 30), NL.AutosaveName);
                }
            }
            else
            {
                if (NL.ChanceCoefficient != 0)
                    InterfaceGUI.Info(1, 5, "Chance", NL.Chance);
            }
            InterfaceGUI.Info(1, 1, "Best generation", NL.BestGeneration);
            InterfaceGUI.Info(1, 2, "Generation", NL.Generation);
            InterfaceGUI.Info(1, 3, "Children", NL.ChildrenInGeneration);
            InterfaceGUI.Info(1, 4, "Best longevity", NL.BestLongevity);


            if (NL.MutationAddNeuron == 0 && NL.MutationAddWeight == 0 && NL.MutationChangeOneWeight == 0 && NL.MutationChangeWeights == 0 && NL.MutationChangeOneBias == 0 && NL.MutationChangeBias == 0)
                GUI.enabled = false;

            bool Activate = false;
            if (Settings)
                Learn = InterfaceGUI.Button(1, 9, "Learn OFF", "Learn ON", Learn, ref Activate);
            else
                Learn = InterfaceGUI.Button(1, 6, "Learn OFF", "Learn ON", Learn, ref Activate);

            if (Activate && !Learn)
                NL.StopLearn();

            GUI.enabled = true;
        }

        if (WindowRect.x < 0)                                                               //window restriction on the screen
            WindowRect.x = 0;
        else if (WindowRect.x + WindowRect.width > Screen.width)
            WindowRect.x = Screen.width - WindowRect.width;
        if (WindowRect.y < 0)
            WindowRect.y = 0;
        else if (WindowRect.y + WindowRect.height > Screen.height)
            WindowRect.y = Screen.height - WindowRect.height;

        GUI.DragWindow(new Rect(0, 0, WindowRect.width, 20));
    }
}
