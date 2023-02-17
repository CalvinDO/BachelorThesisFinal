using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Artificial neural network interface.
/// </summary>
public class ANNInterface : MonoBehaviour
{
    /// <summary>
    /// Artificial neural network.
    /// </summary>
    public ANN Ann;
    private Rect WindowRectInterface = new Rect(0, Screen.height, 390, 245);
    private Rect WindowRectVisualization;
    private bool ShowVisualization = false;
    private bool Resizing = false;

    private bool VisualizationWeightsFade = false;
    private bool VisualizationNeurons = true;
    private float VisualizationWeightsWidth = 1;
    private string NEATName = "";
    private bool ShowBias = false;

    private void Start()
    {
        if (gameObject.name == "StudentChild")
            Destroy(gameObject.GetComponent<ANNInterface>());
        else
        {
            WindowRectVisualization.x = Screen.width / 3;
            WindowRectVisualization.y = Screen.height / 3;
            WindowRectVisualization.width = Screen.width / 3;
            WindowRectVisualization.height = Screen.height / 3;
        }
    }

    private void OnGUI()
    {
        if (Ann != null)
        {
            WindowRectInterface = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRectInterface, WindowInterface, "ANN in " + transform.name);  // interface window
            if (ShowVisualization)
                WindowRectVisualization = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRectVisualization, WindowVisualization, "Visualization of ANN in " + transform.name);  // visualization window
        }
    }

    private void WindowInterface(int ID)                                                                                // interface window
    {
        if (WindowRectInterface.height == 65)                                                                           // small window
        {
            if (GUI.Button(new Rect(WindowRectInterface.width - 20, 0, 20, 20), "▄"))                                   // change window scale
            {
                WindowRectInterface.width = 390;
                WindowRectInterface.height = 245;
                WindowRectInterface.x = WindowRectInterface.x - 190;

            }
            ShowVisualization = InterfaceGUI.Button(1, 1, "Visualization OFF", "Visualization ON", ShowVisualization);  // perceptron's visualization (ON / OFF)
        }
        else                                                                                                            // big window
        {
            if (GUI.Button(new Rect(WindowRectInterface.width - 20, 0, 20, 20), "-"))                                   // change window scale
            {
                WindowRectInterface.width = 200;
                WindowRectInterface.height = 65;
                WindowRectInterface.x = WindowRectInterface.x;
                WindowRectInterface.x = WindowRectInterface.x + 190;
            }

            Ann.AFWM = InterfaceGUI.Button(1, 1, "Without Minus", "With Minus", Ann.AFWM);
            Ann.B = InterfaceGUI.Button(1, 2, "Bias OFF", "Bias ON", Ann.B);
            Ann.AFS = InterfaceGUI.HorizontalSlider(1, 3, "Scale", Ann.AFS, 0.1F, 5F);

            InterfaceGUI.Info(1, 4, "Inputs", Ann.Input.Length);
            InterfaceGUI.Info(1, 5, "Outputs", Ann.Output.Length);
            InterfaceGUI.Info(1, 6, "Hidden neurons", Ann.Node.Count - Ann.Input.Length - Ann.Output.Length);
            InterfaceGUI.Info(1, 7, "Weights", Ann.Connection.Count);
            ShowVisualization = InterfaceGUI.Button(2, 1, "Visualization OFF", "Visualization ON", ShowVisualization);  // perceptron's visualization (ON / OFF)

            NEATName = GUI.TextField(new Rect(200, 145, 180, 30), NEATName);
            bool Save = false;
            Save = InterfaceGUI.Button(2, 6, "Save", "Save", Save);
            bool Load = false;
            Load = InterfaceGUI.Button(2, 7, "Load", "Load", Load);
            if (Save)
                Ann.Save(NEATName);
            else if (Load)
                Ann.Load(NEATName);
        }

        if (WindowRectInterface.x < 0)                                                                                  //window restriction on the screen
            WindowRectInterface.x = 0;
        else if (WindowRectInterface.x + WindowRectInterface.width > Screen.width)
            WindowRectInterface.x = Screen.width - WindowRectInterface.width;
        if (WindowRectInterface.y < 0)
            WindowRectInterface.y = 0;
        else if (WindowRectInterface.y + WindowRectInterface.height > Screen.height)
            WindowRectInterface.y = Screen.height - WindowRectInterface.height;

        GUI.DragWindow(new Rect(0, 0, WindowRectInterface.width, 20));
    }

    private void WindowVisualization(int ID)
    {
        if (WindowRectVisualization.width != Screen.width && WindowRectVisualization.height != Screen.height)           // change window scale
        {
            if (GUI.Button(new Rect(WindowRectVisualization.width - 40, 0, 20, 20), "▄"))
            {
                WindowRectVisualization.x = 0;
                WindowRectVisualization.y = 0;
                WindowRectVisualization.width = Screen.width;
                WindowRectVisualization.height = Screen.height;
            }
        }
        else
        {
            if (GUI.Button(new Rect(WindowRectVisualization.width - 40, 0, 20, 20), "_"))
            {
                WindowRectVisualization.x = Screen.width / 3;
                WindowRectVisualization.y = Screen.height / 3;
                WindowRectVisualization.width = Screen.width / 3;
                WindowRectVisualization.height = Screen.height / 3;
            }
        }

        if (VisualizationNeurons && Ann.B)                                                                                // show bias or neuron value
        {
            if (ShowBias && GUI.Button(new Rect(WindowRectVisualization.width - 60, 0, 20, 20), "B"))
                ShowBias = false;
            if (!ShowBias && GUI.Button(new Rect(WindowRectVisualization.width - 60, 0, 20, 20), "N"))
                ShowBias = true;
        }
        else
        {
            if (ShowBias)
                ShowBias = false;
        }

        if (GUI.Button(new Rect(WindowRectVisualization.width - 20, 0, 20, 20), "x"))                                   // close window
            ShowVisualization = false;

        Visualization(WindowRectVisualization.width, WindowRectVisualization.height);

        if (WindowRectVisualization.x < 0)                                                                              //window restriction on the screen
            WindowRectVisualization.x = 0;
        else if (WindowRectVisualization.x + WindowRectVisualization.width > Screen.width)
            WindowRectVisualization.x = Screen.width - WindowRectVisualization.width;
        if (WindowRectVisualization.y < 0)
            WindowRectVisualization.y = 0;
        else if (WindowRectVisualization.y + WindowRectVisualization.height > Screen.height)
            WindowRectVisualization.y = Screen.height - WindowRectVisualization.height;

        GUI.DragWindow(new Rect(0, 0, WindowRectVisualization.width, 20));

        Vector2 Mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        Rect MouseZone = new Rect(WindowRectVisualization.width - 20, WindowRectVisualization.height - 20, 20, 20);
        Rect Resize = new Rect();
        if (Event.current.type == EventType.MouseDown && MouseZone.Contains(Mouse))
        {
            Resizing = true;
            Resize = new Rect(Mouse.x, Mouse.y, WindowRectVisualization.width, WindowRectVisualization.height);
        }
        else if (Event.current.type == EventType.MouseUp && Resizing)
        {
            Resizing = false;
        }
        else if (!Input.GetMouseButton(0))
        {
            Resizing = false;
        }
        else if (Resizing)
        {
            WindowRectVisualization.width = Mathf.Max(100, Resize.width + (Mouse.x - Resize.x));
            WindowRectVisualization.height = Mathf.Max(100, Resize.height + (Mouse.y - Resize.y));
            WindowRectVisualization.xMax = Mathf.Min(Screen.width, WindowRectVisualization.xMax);
            WindowRectVisualization.yMax = Mathf.Min(Screen.height, WindowRectVisualization.yMax);
        }
        if (WindowRectVisualization.width < 390)
            WindowRectVisualization.width = 390;
    }

    private void Visualization(float width, float height)
    {
        if (Ann != null)
        {
            Vector2 v = new Vector2(20, 30);
            width -= 40;
            height -= 80;

            if (VisualizationWeightsFade && GUI.Button(new Rect(10, 25, 85, 30), "Fade"))
                VisualizationWeightsFade = false;
            if (!VisualizationWeightsFade && GUI.Button(new Rect(10, 25, 85, 30), "Triange"))
                VisualizationWeightsFade = true;

            if (VisualizationNeurons && GUI.Button(new Rect(105, 25, 85, 30), "Value"))
                VisualizationNeurons = false;
            if (!VisualizationNeurons && GUI.Button(new Rect(105, 25, 85, 30), "Dot"))
                VisualizationNeurons = true;

            VisualizationWeightsWidth = InterfaceGUI.HorizontalSlider(2, 1, "Weights width", VisualizationWeightsWidth, 0.1F, 10);

            TextAnchor SaveTA = GUI.skin.GetStyle("Label").alignment;
            if (VisualizationNeurons)
                GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;

            if (Event.current.type == EventType.Repaint)
            {
                if (Ann.Connection.Count > 0)
                {
                    List<int> WL = new List<int>(Ann.Connection.Keys);
                    int w = 0;
                    while (w < Ann.Connection.Count)
                    {
                        int temp = WL[w];
                        Vector2 P1 = new Vector2(Ann.Node[Ann.Connection[temp].In].Position.x * width, Ann.Node[Ann.Connection[temp].In].Position.y * height + 40);
                        Vector2 P2 = new Vector2(Ann.Node[Ann.Connection[temp].Out].Position.x * width, Ann.Node[Ann.Connection[temp].Out].Position.y * height + 40);
                        if (Ann.Connection[temp].Enable)
                            DrawANNWeight.Line(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, Mathf.Abs(Formulas.ActivationFunction(Ann.Node[Ann.Connection[temp].In].Neuron * Ann.Connection[temp].Weight, Ann.AFS, Ann.AFWM)), VisualizationWeightsFade);
                        else
                            DrawANNWeight.Line(P1 + v, P2 + v, VisualizationWeightsWidth * Ann.Connection[temp].Weight, VisualizationWeightsFade);
                        w++;
                    }
                    WL.Clear();
                }
            }
            if (Ann.Node.Count > 0)
            {
                List<int> NL = new List<int>(Ann.Node.Keys);
                int n = 0;
                while (n < Ann.Node.Count)
                {
                    int temp = NL[n];
                    if (VisualizationNeurons)
                    {
                        GUI.Box(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 55, 40, 30), "");
                        if (ShowBias)
                            GUI.Label(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 55, 40, 30), Ann.Node[temp].Bias.ToString("f2"));
                        else
                            GUI.Label(new Rect(Ann.Node[temp].Position.x * width, Ann.Node[temp].Position.y * height + 55, 40, 30), Ann.Node[temp].Neuron.ToString("f2"));
                    }
                    else
                        GUI.Box(new Rect(Ann.Node[temp].Position.x * width - 6 + v.x, Ann.Node[temp].Position.y * height + 34 + v.y, 12, 12), "");
                    n++;
                }
                NL.Clear();
            }
            if (VisualizationNeurons)
                GUI.skin.GetStyle("Label").alignment = SaveTA;
        }
    }
}