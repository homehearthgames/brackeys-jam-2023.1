using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RedLineTrigger;
using static PlayerController;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    
    
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        //subscribe to event sent by RedLineTrigger
        Debug.Log("Hello?");
        PlayerCrossedLine+=OnPlayerCrossedLine;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDestroy() {
        PlayerCrossedLine-=OnPlayerCrossedLine;
    }

    public static float CalculateJumpForce(float gravityStrength, float jumpHeight)
    {
        //h = v^2/2g
        //2gh = v^2
        //sqrt(2gh) = v
        return Mathf.Sqrt(2 * gravityStrength * jumpHeight);
    }

    public static float CalculateJumpTime(float gravityStrength, float jumpVelocity, float velocityRatio, float maxJumpHeight, float minJumpHeight)
    {
        // time takes to reach jumpVelocity * velocityRatio + time to take 
        return (jumpVelocity * (velocityRatio - 1)) / - gravityStrength + 
               (maxJumpHeight - minJumpHeight) / (jumpVelocity * velocityRatio);
    }

    public static void OnPlayerCrossedLine(PlayerController playerInstance){
        
        switch(playerInstance.Status){
            case PlayerState.me:
                playerInstance.Status = PlayerState.soul;
                break;
            case PlayerState.soul:
                playerInstance.Status = PlayerState.dead;
                break;
            default:

            break;
        }
        Debug.Log($"Player {playerInstance.name} crossed the RedLine. New State: {playerInstance.Status}");
    }
}
