using UnityEngine;

public class DemoCar : MonoBehaviour
{
    public float Turn = 0;
    public float Accelerate = 0;
    public float SpeedScale = 5;

    public Vector3 SpawnPosition;
    public Vector3 SpawnAngles;

    public bool Crash = false;

    public float Life = 0;
    public float LifeTime = 0;

    void Start()
    {
        SpawnPosition = new Vector3(-20, 0, -10);
        SpawnAngles = Vector3.zero;
    }

    void Update()
    {
        if (Accelerate > 1)
            Accelerate = 1;
        else if (Accelerate < -1)
            Accelerate = -1;

        if (Turn > 1)
            Turn = 1;
        else if (Turn < -1)
            Turn = -1;

        float AS = SpeedScale * Accelerate;
        transform.Rotate(0, AS * Turn, 0);

        float Move = 0.1F * AS;
        transform.Translate(0, 0, Move);

        LifeTime += SpeedScale;
        Life += Move;

        if (Crash)
        {
            transform.position = SpawnPosition;
            transform.eulerAngles = SpawnAngles;
            LifeTime = 0;
            Crash = false;
        }
        if (LifeTime > 18000 && !Crash)
            Crash = true;
    }

    void OnTriggerEnter()
    {
        Crash = true;
    }

    void OnTriggerExit()
    {
        Crash = false;
    }
}
