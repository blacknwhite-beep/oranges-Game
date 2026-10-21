using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    private DroneMovement1 drone;

    void Awake()
    {
        drone = GetComponentInParent<DroneMovement1>();
    }


    

   
    void Start() { 
   

        


    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
