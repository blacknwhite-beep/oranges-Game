using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    public static Action<GameObject> OnPlayerSpawned;

    private void Awake() 
    {
       player = Instantiate(playerPrefab, transform.position, Quaternion.identity);


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnPlayerSpawned?.Invoke(player);
    }

}
