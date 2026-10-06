using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform Target { get; private set; }
    public bool PlayerDetected => Target != null;

    private int insideCount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Zone entered by: " + other.name + " | tag: " + other.tag);

        if (!other.CompareTag("Player")) return;

        insideCount++;
        Target = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Zone exited by: " + other.name);

        if (!other.CompareTag("Player")) return;

        insideCount--;
        if (insideCount <= 0)
        {
            insideCount = 0;
            Target = null;
        }
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
