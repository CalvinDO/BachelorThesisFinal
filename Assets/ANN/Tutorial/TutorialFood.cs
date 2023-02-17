using UnityEngine;

public class TutorialFood : MonoBehaviour
{
    private bool Moving = false;

    void Start()
    {
        MoveFood();         //Move food
    }

    void Update()
    {
        if (Moving)
            Moving = false;
    }

    void OnCollisionEnter(Collision col)
    {
        TutorialCowControl TСC = col.gameObject.GetComponent<TutorialCowControl>();
        if (TСC != null && !Moving)
        {
            //The cow must eat at a certain angle
            if (Mathf.Abs(TСC.AngleToFood) > 5)
                TСC.Death = true;
            else
            {
                TСC.Satiety += 15;
                MoveFood();
            }
        }
    }

    //Move food
    void MoveFood()
    {
        //Random position
        transform.position = new Vector3(Random.Range(-14F, 14F), 0.5F, Random.Range(-14F, 14F));
        Moving = true;
    }
}
