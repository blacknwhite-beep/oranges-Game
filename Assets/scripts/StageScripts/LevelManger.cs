using TMPro;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{



    public GameObject player;
    public PlayerMovment1 playerMovementScript;
    public playerJump playerJumpScript;
    [SerializeField] public int playerLevel = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerJumpScript.canDoubleJump = false;
        Debug.Log("movespeed set to 7");
        playerMovementScript.speed = 7f;

    }

    // Update is called once per frame
    void Update()
    {

    }


    [ContextMenu("Level Up Player")]
    public void levelUp()
    {
        if (playerLevel == 0)
        {
            Debug.Log("Player is lv 1");
            playerLevel++;
            playerMovementScript.speed = 10f;

        }
        else if (playerLevel == 1)
        {
            Debug.Log("Player is lv 2");
            playerLevel++;
            playerMovementScript.speed = 12f;
            playerJumpScript.canDoubleJump = true;
        }
        else if (playerLevel == 2)
        {
            Debug.Log("Player is lv 3");
            playerLevel++;
            playerMovementScript.speed = 15f;
        }
       
        

        
    }
}
