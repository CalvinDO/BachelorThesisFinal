using UnityEngine;

public class DemoCarNEAT : MonoBehaviour
{
    public DemoCar Car;

    public ANN Ann = new ANN();                     // create ANN

    public bool ByAxisControl = true;

    public bool ReloadANN = false;
    public bool ReloadedANN = false;

    public bool Crash = false;

    void Start()
    {
        Ann.Create(false, 9, 2);
        ANNInterface NI = gameObject.AddComponent<ANNInterface>();
        NI.Ann = Ann;
    }

    void Update()
    {
        if (Car == null)
            Car = gameObject.GetComponent<DemoCar>();
        else
        {
            if (ReloadANN)
            {
                if (ByAxisControl)
                    Ann.Create(Ann.AFS, Ann.B, Ann.AFWM, Ann.Input.Length, 2);
                else
                    Ann.Create(Ann.AFS, Ann.B, Ann.AFWM, Ann.Input.Length, 4);
                ReloadANN = false;
            }

            int i = 0;
            RaycastHit Hit;
            float HMR = 180F / (Ann.Input.Length - 1);
            while (i < Ann.Input.Length)
            {
                if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 0.1F, 0)), transform.TransformDirection(Quaternion.AngleAxis(HMR * i - 90F, Vector3.up) * Vector3.forward), out Hit, 100F, 1 << 0))
                    Ann.Input[i] = Hit.distance / 100F;
                else
                    Ann.Input[i] = 1F;

                Debug.DrawLine(transform.position + transform.TransformDirection(new Vector3(0, 0.1F, 0)), Hit.point, Color.red);
                i++;
            }
            Ann.Solution();
            if (ByAxisControl)
            {
                if (Ann.AFWM)
                {
                    Car.Turn = Ann.Output[0];
                    Car.Accelerate = Ann.Output[1];
                }
                else
                {
                    Car.Turn = Ann.Output[0] * 2F - 1F;
                    Car.Accelerate = Ann.Output[1] * 2F - 1F;
                }
            }
            else
            {
                if (Ann.AFWM)
                {
                    Car.Turn = (Ann.Output[1] + Ann.Output[0]) / 2F;
                    Car.Accelerate = (Ann.Output[3] + Ann.Output[2]) / 2F;
                }
                else
                {
                    Car.Turn = Ann.Output[1] - Ann.Output[0];
                    Car.Accelerate = Ann.Output[3] - Ann.Output[2];
                }
            }
        }
        Crash = Car.Crash;
    }
}