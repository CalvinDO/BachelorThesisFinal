using UnityEngine;

public class DemoCarNEATSelfLearnInterface : MonoBehaviour
{
    public GameObject StudentGO;
    public DemoCarNEAT Student;
    public string Crash;
    public string Life;

    private ANNLearnByNEATInterface NLBEI;

    private float CarsSpeed = 10;
    
    private bool ShowMenu = true;

    void Start()
    {
        NLBEI = StudentGO.gameObject.AddComponent<ANNLearnByNEATInterface>();
        NLBEI.Ann = Student.Ann;

        NLBEI.NL.AmountOfChildren = 100;
        NLBEI.NL.ChildrenInWave = 100;
        NLBEI.NL.ChildrenByWave = false;
        NLBEI.NL.StudentData(StudentGO, Student, "Ann", Student.Car, Crash, Life);
    }

    void Update()
    {
        if (Student != null)
        {
            Student.Car.SpeedScale = CarsSpeed;
            if (Student.ReloadedANN)
            {
                NLBEI.NL.Reset();
                NLBEI.Learn = false;
                Student.ReloadedANN = false;
            }
        }
    }

    void OnGUI()
    {
        ShowMenu = InterfaceGUI.Button(1, 1, "Show menu", "Hide menu", ShowMenu);
        if (ShowMenu)
        {
            CarsSpeed = InterfaceGUI.HorizontalSlider(1, 2, "Cars speed", CarsSpeed, 0.1F, 20F);
            Student.ByAxisControl = InterfaceGUI.Button(1, 3, "Get button", "Get axis", Student.ByAxisControl, ref Student.ReloadANN);
        }
    }
}

