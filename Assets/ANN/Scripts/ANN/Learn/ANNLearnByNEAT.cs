using System.Collections.Generic;
using UnityEngine;

public class ANNLearnByNEAT
{
    /// <summary>
    /// Amount of "children" in each generation.
    /// </summary>
    public int AmountOfChildren = 10;

    /// <summary>
    /// Amount of "children" in each generation's wave.
    /// </summary>
    public int ChildrenInWave = 1;
    private int ChildrenInWaveMustBe = 1;

    /// <summary>
    /// If "true" - by waves. If "false" - use maximum in one time.
    /// </summary>
    public bool ChildrenByWave = true;
    private int ChildrenOneByOneLeft = 0;

    /// <summary>
    /// The best generation.
    /// </summary>
    public int BestGeneration = 0;

    /// <summary>
    /// Total number of generations.
    /// </summary>
    public int Generation;

    /// <summary>
    /// The number of "children" in this generation.
    /// </summary>
    public int ChildrenInGeneration;

    /// <summary>
    /// Permission to "cross" the two best "children".
    /// </summary>
    public bool Cross = false;

    /// <summary>
    /// If "true", then in the first generation of children all input neurons will be conneced with the output neurons.
    /// </summary>
    public bool PerceptronStart = false;

    /// <summary>
    /// Chance to choose the current worst generation. It changes with each new generation.
    /// </summary>
    public float Chance = 100;
    private float ChanceInBestGeneration = 100;

    /// <summary>
    /// The coefficient of influence on the chance of choosing the current worst generation. Effects on condition that it is not zero and the present generation was worse than the previous one.
    /// </summary>
    public float ChanceCoefficient = 0F;

    /// <summary>
    /// Auto-save on/off.
    /// </summary>
    public bool Autosave = false;

    /// <summary>
    /// Auto-save step.
    /// </summary>
    public int AutosaveStep = 10;

    /// <summary>
    /// Name of perceptron for auto-save.
    /// </summary>
    public string AutosaveName = "";

    /// <summary>
    /// Ignore children collision on/off.
    /// </summary>
    public bool IgnoreCollision = true;

    /// <summary>
    /// The best "longevity" at the moment.
    /// </summary>
    public float BestLongevity = -Mathf.Infinity;

    /// <summary>
    /// The difference of weights between generations.
    /// </summary>
    public float ChildrenDifference = 2F;

    /// <summary>
    /// Learning ANN.
    /// </summary>
    public ANN Ann;

    private Dictionary<int, ANNNode> PossibleNeuron = new Dictionary<int, ANNNode>();
    private Dictionary<int, ANNConnection> PossibleWeight = new Dictionary<int, ANNConnection>();
    private Dictionary<int, ANNConnection> PossibleWeightForPossibleNeuron = new Dictionary<int, ANNConnection>();

    //StudentData
    private GameObject Student;
    private Object HereIsANN;
    private string ANNName;
    private Object StudentControls;
    private string StudentCrash;
    private string StudentLife;
    private bool HaveStudentData = false;
    private bool HaveStudent = false;

    private GameObject[] Child;
    private ANN[] ChildN;
    private bool[] ChildCrash;
    private float[] Longevity;
    private float BestLongevityInGeneration = -Mathf.Infinity;

    private ANN BestANN;
    private ANN AlmostBestANN;

    public static ANNLearnByNEAT instance;

    public void Init() {
        ANNLearnByNEAT.instance = this;
    }

    /// <summary>
    /// Chance to use mutations "add neuron".
    /// </summary>
    public float MutationAddNeuron = 0;

    /// <summary>
    /// Chance to use mutations "add weight".
    /// </summary>
    public float MutationAddWeight = 0;

    /// <summary>
    /// Chance to use mutations "change jne weight".
    /// </summary>
    public float MutationChangeOneWeight = 0;

    /// <summary>
    /// Chance to use mutations "change all weights".
    /// </summary>
    public float MutationChangeWeights = 0;

    /// <summary>
    /// Chance to use mutations "change bias of one neuron".
    /// </summary>
    public float MutationChangeOneBias = 0;

    /// <summary>
    /// Chance to use mutations "change bias of all neurons".
    /// </summary>
    public float MutationChangeBias = 0;

    /// <summary>
    /// Sum of mutation chances. 
    /// </summary>
    public float MutationSum = 0;

    /// <summary>
    /// How many weights need to add.
    /// </summary>
    public int AddingWeightsCount = 1;

    /// <summary>
    /// Chance to change sign of some weights.
    /// </summary>
    public float ChangeWeightSign = 0; 

    /// <summary>
    /// Data collection for training.
    /// </summary>
    /// <param name="Student">The main game object (GameObject) is to learn.</param>
    /// <param name="HereIsANN">Script containing ANN.</param>
    /// <param name="ANNName">The name of the variable (ANN) which is called the ANN in the script that contains the ANN (HereIsANN).</param>
    /// <param name="StudentControls">Script for control the main game object.</param>
    /// <param name="StudentCrash">The name of the variable (bool) which is called the cause of the "crash" of the game object in the control script (StudentControls).</param>
    /// <param name="StudentLife">The name of the variable (float) is called the "longevity" of the game object in the control script (StudentControls).</param>
    public void StudentData(GameObject Student, Object HereIsANN, string ANNName, Object StudentControls, string StudentCrash, string StudentLife)
    {
        if (Student != null && HereIsANN != null && StudentControls != null)
        {
            this.Student = Student;
            this.HereIsANN = HereIsANN;
            this.ANNName = ANNName;
            this.StudentControls = StudentControls;
            this.StudentCrash = StudentCrash;
            this.StudentLife = StudentLife;
            HaveStudentData = true;
        }
    }

    /// <summary>
    /// The training of the ANN.
    /// </summary>
    public void Learn()
    {
        if (ChanceCoefficient < 0)
            ChanceCoefficient = 0;
        else if (ChanceCoefficient > 0.5F)
            ChanceCoefficient = 0.5F;
        if (Generation == 0)
            Generation = 1;

        if (HaveStudentData)
        {
            if (!HaveStudent)
            {

                System.Type studentControlType = StudentControls.GetType();
                System.Reflection.FieldInfo fieldInfo = studentControlType.GetField(StudentCrash);
                object obj = Student.GetComponent(StudentControls.GetType());

                fieldInfo.SetValue(obj, true);
                //StudentControls.GetType().GetField(StudentCrash).SetValue(Student.GetComponent(StudentControls.GetType()), true);
                StudentControls.GetType().GetField(StudentLife).SetValue(Student.GetComponent(StudentControls.GetType()), 0);
                HaveStudent = true;
            }
            else
            {
                if (ChildrenInGeneration == 0)
                {
                    Ann.NumberingCorrection();
                    PossibleСonfiguration();
                    ChildrenInGeneration = AmountOfChildren;
                    CreateChildren();
                }
                else if (ChildrenInWaveMustBe == 0 && ChildrenByWave)
                {
                    CreateChildren();
                }
                else
                {
                    int c = 0;
                    while (c < Child.Length)
                    {
                        if (!ChildrenByWave && Child[c] == null && ChildrenOneByOneLeft > 0)
                        {
                            CreateChild(c);
                        }
                        else if (Child[c] != null)
                        {
                            Object TempObject = Child[c].GetComponent(HereIsANN.GetType());
                            //Mutaions
                            if (ChildN[c] == null)
                            {
                                ChildN[c] = (ANN)HereIsANN.GetType().GetField(ANNName).GetValue(TempObject);
                                ANNCopy(ChildN[c], Ann);
                                Mutation(ChildN[c]);
                            }

                            TempObject = Child[c].GetComponent(StudentControls.GetType());

                            if (!ChildCrash[c])
                                ChildCrash[c] = (bool)StudentControls.GetType().GetField(StudentCrash).GetValue(TempObject);

                            Longevity[c] = (float)StudentControls.GetType().GetField(StudentLife).GetValue(TempObject);

                            if (ChildCrash[c])
                            {
                                if (BestLongevityInGeneration < Longevity[c])
                                {
                                    BestLongevityInGeneration = Longevity[c];
                                    if (BestANN != null && Cross)
                                    {
                                        AlmostBestANN = new ANN();
                                        ANNCopy(AlmostBestANN, BestANN);
                                    }
                                    else
                                        BestANN = new ANN();

                                    ANNCopy(BestANN, ChildN[c]);

                                    if (BestLongevity < BestLongevityInGeneration)
                                    {
                                        BestGeneration = Generation;
                                        BestLongevity = BestLongevityInGeneration;
                                    }
                                }
                                if (Child[c] != null)
                                {
                                    Object.Destroy(Child[c]);
                                    ChildrenInWaveMustBe--;
                                    if (ChildrenInWaveMustBe == 0 || !ChildrenByWave)
                                    {
                                        StudentControls.GetType().GetField(StudentCrash).SetValue(Student.GetComponent(StudentControls.GetType()), true);
                                        StudentControls.GetType().GetField(StudentLife).SetValue(Student.GetComponent(StudentControls.GetType()), 0);
                                    }
                                    ChildrenInGeneration--;
                                }
                            }
                        }
                        c++;
                    }

                    if (ChildrenInGeneration == 0)
                    {
                        Child = new GameObject[0];
                        if (BestLongevity <= BestLongevityInGeneration)
                        {
                            if (BestGeneration == Generation)
                            {
                                BestLongevity = BestLongevityInGeneration;
                                ChanceInBestGeneration = Chance;

                                if (Cross && BestANN != null && AlmostBestANN != null)
                                {
                                    Crossing(BestANN, AlmostBestANN);
                                    BestANN = null;
                                    AlmostBestANN = null;
                                }
                                else
                                {
                                    if (BestANN != null)
                                        ANNCopy(Ann, BestANN);
                                }
                            }
                        }
                        else
                        {
                            if (ChanceCoefficient != 0 && BestLongevity != BestLongevityInGeneration)
                            {
                                float P = 100 * Mathf.Exp((BestLongevityInGeneration - BestLongevity) / Chance);
                                if (P > Random.Range(0F, 100F))
                                {
                                    BestGeneration = Generation;
                                    BestLongevity = BestLongevityInGeneration;
                                    ChanceInBestGeneration = Chance;

                                    if (Cross && BestANN != null && AlmostBestANN != null)
                                    {
                                        Crossing(BestANN, AlmostBestANN);
                                        BestANN = null;
                                        AlmostBestANN = null;
                                    }
                                    else
                                    {
                                        ANNCopy(Ann, BestANN);
                                    }
                                }
                            }
                        }
                        if (ChanceCoefficient != 0)
                            Chance = Chance * (1 - ChanceCoefficient);
                        if (Autosave && Generation % AutosaveStep == 0)
                        {
                            if (BestGeneration == Generation)
                            {
                                Ann.NumberingCorrection();
                                Ann.Save(AutosaveName + "_G-" + Generation + "_L-" + BestLongevityInGeneration + "_BG-" + BestGeneration + "_BL-" + BestLongevity);
                            }
                            else
                            {
                                ANN temp = new ANN();
                                ANNCopy(temp, BestANN);
                                temp.NumberingCorrection();
                                temp.Save(AutosaveName + "_G-" + Generation + "_L-" + BestLongevityInGeneration + "_BG-" + BestGeneration + "_BL-" + BestLongevity);
                            }
                        }
                        Generation++;

                        CWTrainingManagerDataCollector.instance.tempWavesMaxDistances.Add(CWTrainingUIStats.instance.maxDistance);

                        BestLongevityInGeneration = -Mathf.Infinity;
                        ChildrenOneByOneLeft = AmountOfChildren;
                    }
                }
            }
        }
    }

    private void CreateChildren()
    {
        ChildrenOneByOneLeft = AmountOfChildren;
        ChildrenInWaveMustBe = ChildrenInWave;
        if (ChildrenInWaveMustBe > ChildrenInGeneration)
            ChildrenInWaveMustBe = ChildrenInGeneration;

        ClearGeneration();
        Child = new GameObject[ChildrenInWaveMustBe];
        ChildN = new ANN[ChildrenInWaveMustBe];
        ChildCrash = new bool[ChildrenInWaveMustBe];


        Longevity = new float[ChildrenInWaveMustBe];

        int i = 0;
        while (i < ChildrenInWaveMustBe)
        {
            CreateChild(i);
            i++;
        }

        MutationSum = MutationAddNeuron + MutationAddWeight + MutationChangeOneWeight + MutationChangeWeights + MutationChangeOneBias + MutationChangeBias;
    }

    private void CreateChild(int c)
    {
        ChildrenOneByOneLeft--;
        ChildN[c] = null;
        ChildCrash[c] = false;
        Longevity[c] = 0;

        Child[c] = Object.Instantiate(Student);
        Child[c].name = "StudentChild";

        foreach (ANNInterface NI in Child[c].GetComponentsInChildren<ANNInterface>())
            Object.Destroy(NI);

        foreach (ANNLearnByNEATInterface NL in Child[c].GetComponentsInChildren<ANNLearnByNEATInterface>())
            Object.Destroy(NL);

        if (IgnoreCollision)
        {
            foreach (Collider col in Child[c].GetComponentsInChildren<Collider>())
            {
                int i = 0;
                while (i < Child.Length)
                {
                    if (Child[i] != null && i != c)
                    {
                        foreach (Collider colp in Child[i].GetComponentsInChildren<Collider>())
                            Physics.IgnoreCollision(col, colp, true);
                    }
                    i++;
                }
                foreach (Collider colp in Student.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(col, colp, true);
            }

            foreach (Collider2D col in Child[c].GetComponentsInChildren<Collider2D>())
            {
                int i = 0;
                while (i < Child.Length)
                {
                    if (Child[i] != null && i != c)
                    {
                        foreach (Collider2D colp in Child[i].GetComponentsInChildren<Collider2D>())
                            Physics2D.IgnoreCollision(col, colp, true);
                    }
                    i++;
                }
                foreach (Collider2D colp in Student.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(col, colp, true);
            }
        }
    }

    /// <summary>
    /// Immediate stop learning with the transfer of information of the best ANN from the best generation to the ANN what is studying.
    /// </summary>
    public void StopLearn()
    {
        if (ChildCrash != null)
        {
            int i = 0;
            while (i < ChildCrash.Length)
            {
                ChildCrash[i] = true;
                i++;
            }
            Learn();
        }
        if (BestANN != null && BestGeneration == Generation)
        {
            if (Cross && AlmostBestANN != null)
            {
                Crossing(BestANN, AlmostBestANN);
                BestANN = null;
                AlmostBestANN = null;
            }
            else
                ANNCopy(Ann, BestANN);
            Ann.NumberingCorrection();
        }

        if (Autosave)
            Ann.Save(AutosaveName + "_G-" + Generation + "_L-" + BestLongevityInGeneration + "_BG-" + BestGeneration + "_BL-" + BestLongevity);

        ClearGeneration();
        Generation = BestGeneration + 1;
        Chance = ChanceInBestGeneration;
        ChildrenInWaveMustBe = 0;
        ChildrenOneByOneLeft = 0;
        ChildrenInGeneration = 0;
        HaveStudent = false;
        StudentControls.GetType().GetField(StudentCrash).SetValue(Student.GetComponent(StudentControls.GetType()), false);
    }

    private void ClearGeneration()
    {
        if (Child != null)
        {
            int i = 0;
            while (i < Child.Length)
            {
                if (Child[i] != null)
                {
                    Object.Destroy(Child[i]);
                }
                i++;
            }
            Child = new GameObject[0];
        }
    }

    /// <summary>
    /// Reset learning info.
    /// </summary>
    public void Reset()
    {
        ClearGeneration();
        Generation = 0;
        BestLongevity = -Mathf.Infinity;
        BestLongevityInGeneration = -Mathf.Infinity;
        Chance = 100;
        HaveStudent = false;
    }

    private void Crossing(ANN N1, ANN N2)
    {
        //To first add second
        int w = 0;
        List<int> WL = new List<int>(N2.Connection.Keys);
        while (w < WL.Count)
        {
            if (!N1.Connection.ContainsKey(WL[w]))
            {
                N1.Connection.Add(WL[w], new ANNConnection(N2.Connection[WL[w]].In, N2.Connection[WL[w]].Out, N2.Connection[WL[w]].Weight, N2.Connection[WL[w]].Enable));
            }
            else
            {
                if ((!N1.Connection[WL[w]].Enable && N2.Connection[WL[w]].Enable) || (N1.Connection[WL[w]].Enable && !N2.Connection[WL[w]].Enable))
                {
                    N1.Connection[WL[w]].Enable = false;
                }
                else if (!N1.Connection[WL[w]].Enable && !N2.Connection[WL[w]].Enable)
                {
                    if (Random.Range(0F, 1F) < 0.5F)
                        N1.Connection[WL[w]].Enable = true;
                }
            }
            w++;
        }
        WL.Clear();
        int n = 0;
        List<int> NL = new List<int>(N2.Node.Keys);
        while (n < NL.Count)
        {
            if (!N1.Node.ContainsKey(NL[n]))
            {
                N1.Node.Add(NL[n], new ANNNode(N2.Node[NL[n]].Bias, N2.Node[NL[n]].Position));
            }
            n++;
        }
        NL.Clear();

        ANNCopy(Ann, N1);
    }

    private void Mutation(ANN CN)
    {
        if (CN.Connection.Count == 0 && PerceptronStart)
        {
            int i = 0;
            int w = 0;
            while (i < CN.Input.Length)
            {
                int o = 0;
                while (o < CN.Output.Length)
                {
                    CN.Connection.Add(w, new ANNConnection(i, CN.Input.Length + o));
                    CN.Connection[w].Weight = Random.Range(-1F, 1F) * ChildrenDifference;
                    CN.Node[CN.Input.Length + o].ConnectionIn.Add(w);
                    o++;
                    w++;
                }
                i++;
            }
        }
        else
        {
            float r = Random.Range(0F, MutationSum);
            if (r < MutationAddNeuron)
                AddNeuron(CN);
            else if (r < MutationAddNeuron + MutationAddWeight)
                AddWeight(CN);
            else if (r < MutationAddNeuron + MutationAddWeight + MutationChangeOneWeight)
                ChangeOneWeight(CN);
            else if (r < MutationAddNeuron + MutationAddWeight + MutationChangeOneWeight + MutationChangeWeights)
                ChangeWeights(CN);
            else if (r < MutationAddNeuron + MutationAddWeight + MutationChangeOneWeight + MutationChangeWeights + MutationChangeOneBias)
                ChangeOneBias(CN);
            else
                ChangeBias(CN);
        }
    }

    private void PossibleСonfiguration()
    {
        PossibleNeuron.Clear();
        PossibleWeight.Clear();
        PossibleWeightForPossibleNeuron.Clear();
        Dictionary<int, ANNConnection> UsedWeight = new Dictionary<int, ANNConnection>();
        //Used weights
        int w = 0;
        while (w < Ann.Connection.Count)
        {
            ANNConnection NW = Ann.Connection[w];
            UsedWeight.Add(w, new ANNConnection(NW.In, NW.Out, 0));
            w++;
        }
        //Possible weights from inputs
        int i = 0;
        w = Ann.Connection.Count;
        while (i < Ann.Input.Length)
        {
            int n = Ann.Input.Length;
            while (n < Ann.Node.Count)
            {
                ANNConnection NW = new ANNConnection(i, n, 0);
                int c = 0;
                bool add = true;
                while (c < UsedWeight.Count)
                {
                    if (UsedWeight[c].In == NW.In && UsedWeight[c].Out == NW.Out)
                        add = false;
                    c++;
                }
                if (add)
                {
                    PossibleWeight.Add(w, new ANNConnection(i, n, 0));
                    w++;
                }
                n++;
            }
            i++;
        }
        //Possible weights to outputs
        i = Ann.Input.Length + Ann.Output.Length;
        while (i < Ann.Node.Count)
        {
            int n = Ann.Input.Length;
            while (n < Ann.Input.Length + Ann.Output.Length)
            {
                ANNConnection NW = new ANNConnection(i, n, 0);
                int c = 0;
                bool add = true;
                while (c < UsedWeight.Count)
                {
                    if (UsedWeight[c].In == NW.In && UsedWeight[c].Out == NW.Out)
                        add = false;
                    c++;
                }
                if (add)
                {
                    PossibleWeight.Add(w, new ANNConnection(i, n, 0));
                    w++;
                }
                n++;
            }
            i++;
        }
        UsedWeight.Clear();
        //Possible neurons on used weights
        w = 0;
        int nw = 0;
        while (w < Ann.Connection.Count)
        {
            if (Ann.Connection[w].Enable)
            {
                PossibleNeuron.Add(w + Ann.Node.Count, new ANNNode(new Vector2(Random.Range(0.1F, 0.9F), Random.Range(0F, 1F))));// (Ann.Node[Ann.Connection[w].In].Position + Ann.Node[Ann.Connection[w].Out].Position + new Vector2(0.1F, 0.1F)) / 2F));
                PossibleWeightForPossibleNeuron.Add(PossibleWeight.Count + Ann.Connection.Count + nw, new ANNConnection(Ann.Connection[w].In, w + Ann.Node.Count));
                nw++;
                PossibleWeightForPossibleNeuron.Add(PossibleWeight.Count + Ann.Connection.Count + nw, new ANNConnection(w + Ann.Node.Count, Ann.Connection[w].Out));
                nw++;
            }
            w++;
        }
    }

    private void AddNeuron(ANN CN)
    {
        if (PossibleNeuron.Count > 0)
        {
            List<int> WL = new List<int>(PossibleNeuron.Keys);
            int tempN = WL[Random.Range(0, WL.Count)];
            WL.Clear();
            CN.Node.Add(tempN, new ANNNode(PossibleNeuron[tempN].Position));

            WL = new List<int>(PossibleWeightForPossibleNeuron.Keys);
            int w = 0;
            int In = 0;
            int Out = 0;
            while (w < WL.Count)
            {
                int tempW = WL[w];
                if (PossibleWeightForPossibleNeuron[tempW].In == tempN)
                {
                    Out = PossibleWeightForPossibleNeuron[tempW].Out;
                    CN.Connection.Add(tempW, new ANNConnection(PossibleWeightForPossibleNeuron[tempW].In, Out));
                    CN.Connection[tempW].Weight = Random.Range(-1F, 1F) * ChildrenDifference;
                }
                else if (PossibleWeightForPossibleNeuron[tempW].Out == tempN)
                {
                    In = PossibleWeightForPossibleNeuron[tempW].In;
                    CN.Connection.Add(tempW, new ANNConnection(In, PossibleWeightForPossibleNeuron[tempW].Out));
                    CN.Connection[tempW].Weight = Random.Range(-1F, 1F) * ChildrenDifference;
                }
                w++;
            }
            w = 0;
            WL = new List<int>(CN.Connection.Keys);
            while (w < CN.Connection.Count)
            {
                if (CN.Connection[WL[w]].In == In && CN.Connection[WL[w]].Out == Out)
                {
                    CN.Connection[WL[w]].Enable = false;
                }
                w++;
            }
            WL.Clear();
            CN.FixConnections();
        }
        else
        {
            if (CN.Connection.Count > 0)
            {
                float r = Random.Range(0F, MutationSum - MutationAddNeuron);
                if (r < MutationAddWeight)
                    AddWeight(CN);
                else if (r < MutationAddWeight + MutationChangeOneWeight)
                    ChangeOneWeight(CN);
                else if (r < MutationAddWeight + MutationChangeOneWeight + MutationChangeWeights)
                    ChangeWeights(CN);
                else if (r < MutationAddWeight + MutationChangeOneWeight + MutationChangeWeights + MutationChangeOneBias)
                    ChangeOneBias(CN);
                else
                    ChangeBias(CN);
            }
            else
            {
                AddWeight(CN);
            }
        }
    }

    private void AddWeight(ANN CN)
    {
        if (PossibleWeight.Count > 0)
        {
            List<int> WL = new List<int>(PossibleWeight.Keys);
            int t = 0;
            while (t < AddingWeightsCount)
            {
                int temp = WL[Random.Range(0, PossibleWeight.Count)];
                if (!CN.Connection.ContainsKey(temp))
                {
                    CN.Connection.Add(temp, new ANNConnection(PossibleWeight[temp].In, PossibleWeight[temp].Out));
                    CN.Connection[temp].Weight = Random.Range(-1F, 1F) * ChildrenDifference;
                }
                CN.FixConnections();
                t++;
            }
            WL.Clear();
        }
        else
        {
            float r = Random.Range(0F, MutationSum - MutationAddWeight);
            if (r < MutationAddNeuron)
                AddNeuron(CN);
            else if (r < MutationAddNeuron + MutationChangeOneWeight)
                ChangeOneWeight(CN);
            else if (r < MutationAddNeuron + MutationChangeOneWeight + MutationChangeWeights)
                ChangeWeights(CN);
            else if (r < MutationAddNeuron + MutationChangeOneWeight + MutationChangeWeights + MutationChangeOneBias)
                ChangeOneBias(CN);
            else
                ChangeBias(CN);
        }
    }

    private void ChangeOneWeight(ANN CN)
    {
        if (CN.Connection.Count > 0)
        {
            List<int> WL = new List<int>(CN.Connection.Keys);
            int w = Random.Range(0, WL.Count);
            while (!CN.Connection[WL[w]].Enable)
                w = Random.Range(0, WL.Count);
            if (ChangeWeightSign != 0 && Random.value < ChangeWeightSign)
                CN.Connection[WL[w]].Weight = -CN.Connection[WL[w]].Weight;
            else
                CN.Connection[WL[w]].Weight += Random.Range(-1F, 1F) * ChildrenDifference;
            WL.Clear();
        }
        else
        {
            if (CN.Connection.Count > 0)
            {
                float r = Random.Range(0F, MutationSum - MutationChangeOneWeight);
                if (r < MutationAddNeuron)
                    AddNeuron(CN);
                else if (r < MutationAddNeuron + MutationAddWeight)
                    AddWeight(CN);
                else if (r < MutationAddNeuron + MutationAddWeight + MutationChangeWeights)
                    ChangeWeights(CN);
                else if (r < MutationAddNeuron + MutationAddWeight + MutationChangeWeights + MutationChangeOneBias)
                    ChangeOneBias(CN);
                else
                    ChangeBias(CN);
            }
            else
            {
                AddWeight(CN);
            }
        }
    }

    private void ChangeWeights(ANN CN)
    {
        if (CN.Connection.Count > 0)
        {
            List<int> WL = new List<int>(CN.Connection.Keys);
            int w = 0;
            while (w < WL.Count)
            {
                if (CN.Connection[WL[w]].Enable)
                {
                    if (ChangeWeightSign != 0 && Random.value < ChangeWeightSign)
                        CN.Connection[WL[w]].Weight = -CN.Connection[WL[w]].Weight;
                    else
                        CN.Connection[WL[w]].Weight += Random.Range(-1F, 1F) * ChildrenDifference;
                }
                w++;
            }
            WL.Clear();
        }
        else
        {
            float r = Random.Range(0F, MutationSum - MutationChangeWeights);
            if (r < MutationAddNeuron)
                AddNeuron(CN);
            else if (r < MutationAddNeuron + MutationAddWeight)
                AddWeight(CN);
            else if (r < MutationAddNeuron + MutationAddWeight + MutationChangeOneWeight)
                ChangeOneWeight(CN);
            else if (r < MutationAddNeuron + MutationAddWeight + MutationChangeOneWeight + MutationChangeOneBias)
                ChangeOneBias(CN);
            else
                ChangeBias(CN);
        }
    }

    private void ChangeOneBias(ANN CN)
    {
        if (CN.Node.Count > CN.Input.Length)
        {
            List<int> NL = new List<int>(CN.Node.Keys);
            int n = Random.Range(CN.Input.Length, NL.Count);
            if (ChangeWeightSign != 0 && Random.value < ChangeWeightSign)
                CN.Node[NL[n]].Bias = -CN.Node[NL[n]].Bias;
            else
                CN.Node[NL[n]].Bias += Random.Range(-1F, 1F) * ChildrenDifference;
            NL.Clear();
        }
    }

    private void ChangeBias(ANN CN)
    {
        if (CN.Node.Count > CN.Input.Length)
        {
            List<int> NL = new List<int>(CN.Node.Keys);
            int n = 0;
            while (n < NL.Count)
            {
                if (ChangeWeightSign != 0 && Random.value < ChangeWeightSign)
                    CN.Node[NL[n]].Bias = -CN.Node[NL[n]].Bias;
                else
                    CN.Node[NL[n]].Bias += Random.Range(-1F, 1F) * ChildrenDifference;
                n++;
            }
            NL.Clear();
        }
    }

    private void ANNCopy(ANN To, ANN From)
    {
        To.AFS = From.AFS;
        To.AFWM = From.AFWM;
        To.B = From.B;
        To.Input = new float[From.Input.Length];
        To.Output = new float[From.Output.Length];
        To.Connection.Clear();
        int i = 0;
        List<int> WL = new List<int>(From.Connection.Keys);
        while (i < WL.Count)
        {
            To.Connection.Add(WL[i], new ANNConnection(From.Connection[WL[i]].In, From.Connection[WL[i]].Out, From.Connection[WL[i]].Weight, From.Connection[WL[i]].Enable));
            i++;
        }
        i = 0;
        To.Node.Clear();
        WL.Clear();
        WL = new List<int>(From.Node.Keys);
        while (i < WL.Count)
        {
            To.Node.Add(WL[i], new ANNNode(From.Node[WL[i]].Bias, From.Node[WL[i]].Position));
            i++;
        }
        WL.Clear();
        To.FixConnections();
    }
}
