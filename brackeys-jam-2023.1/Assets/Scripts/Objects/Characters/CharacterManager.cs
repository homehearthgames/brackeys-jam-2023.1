using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static RedLineTrigger;
using static Player;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    [SerializeField] public Player me;
    [SerializeField] private Player soul;
    [SerializeField] private int maxBody = 3;
    private LinkedList<Player> bodyList = new LinkedList<Player>();
    [SerializeField] private TMP_Text bodyCountText;
    private int bodyCount = 0; // bodyCount == bodies.ToArray().Length
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerPrefab; 

    [SerializeField] private GameObject stars;
    private int totalStars;
    public int collectedStars = 0;

    [SerializeField] private string nextLevelSceneName = "";

    private AudioManager audioManager;
    
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

        if(playerPrefab == null)
        {
            Debug.LogWarning("playerPrefab is Null in CharacterManager!!");
        }
        
        //subscribe to event sent by RedLineTrigger
        //Debug.Log("Hello?");
        PlayerCrossedLine+=OnPlayerCrossedLine;        
    }

    // Start is called before the first frame update
    void Start()
    {
        if(me == null && soul == null)
        {
            Debug.LogWarning("The initial Me or Soul is missing or not set in inspector");
        }
        if(maxBody < 0)
        {
            Debug.LogError("Max Body count in " + SceneManager.GetActiveScene().name + " is negative!!!");
        }

        audioManager = AudioManager.instance;

        UpdateBodyCountText();

        // Init Collectables related
        totalStars = stars.transform.childCount;
    }

    void Update()
    {
        getSwitchInput();
    }

    // Precondition: Me always exists in the level
    private void getSwitchInput()
    {
        // Switch only works when Soul exists
        if(soul != null)
        {
            if(Input.GetButtonDown("MeSwitch"))
            {
                SwitchMe();
            } 
            else if(Input.GetButtonDown("SoulSwitch"))
            {
                SwitchSoul();
            }
        }
    }

    public void SwitchMe()
    {
        // Switching to Me
        me._active = true;
        soul._active = false;

        audioManager.ChangeVolume("UpMusic", 1f);
        audioManager.ChangeVolume("DownMusic", 0f);
    }

    public void SwitchSoul()
    {
        // Switching to soul
        me._active = false;
        soul._active = true;

        audioManager.ChangeVolume("UpMusic", 0f);
        audioManager.ChangeVolume("DownMusic", 1f);
    }

    // Precondition: Me doesn't exist in the current level
    public static void SpawnMe(Vector3 spawnPosition)
    {
        // Spawn a Me Prefab at the spawn location
        if (instance.playerPrefab != null){
            GameObject newMe = Instantiate(instance.playerPrefab, spawnPosition, Quaternion.identity);
            // Trigger spawn animation if we have (if we do, make Player's initial state deactivate and make a tirgger in animation to activate)
            
            // Set a reference to Me if not already set
            instance.me = newMe.GetComponent<Player>();
            instance.me._active = false;
        }
        // LogWarning if (me != null && me != Me Prefab)
    }

    public static void SpawnSoul(Vector3 spawnPosition)
    {
        if(instance.soul != null)
        {
            SoulDies();
        }
        GameObject newSoul = Instantiate(instance.playerPrefab, spawnPosition, Quaternion.identity);
        
        // Trigger spawn animation if we have (if we do, make Player's initial state deactivate and make a tirgger in animation to activate)
            
        // Set a reference to Me if not already set
        instance.soul = newSoul.GetComponent<Player>();
        instance.soul._status = PlayerState.soul;
    }

    public static void SpawnDead(Vector3 spawnPosition)
    {
        GameObject newDead = Instantiate(instance.playerPrefab, spawnPosition, Quaternion.identity);

        Player dead =  newDead.GetComponent<Player>();
        dead._active = false;
        dead._status = PlayerState.dead;
    }

    public static void MeDies()
    {
        // change state
        instance.me.Status = PlayerState.soul;
        instance.me.FlipY();
        // if soul != null, a soul exists, that soul dies first
        if(instance.soul!=null){
            SoulDies();
        }
        // set soul = me, me = null
        instance.soul = instance.me;
        instance.me = null;
        // Call SpawnMe()
        SpawnMe(instance.spawnPoint.position);

        instance.SwitchSoul();
    }

    public static void SoulDies()
    {
        // change state
        instance.soul.Status = PlayerState.dead;
        instance.soul.gameObject.layer = LayerMask.NameToLayer("Ground");
        instance.soul.FlipY();
        // set active to false and pass the active to Me
        instance.SwitchMe();
        // add body into bodies
        instance.bodyList.AddLast(instance.soul);
        instance.bodyCount += 1;
        instance.UpdateBodyCountText();
        if(instance.bodyCount > instance.maxBody)
        {
            // Destroy the first body
            LinkedListNode<Player> first = instance.bodyList.First;
            BodyDies(first.Value);
        }
        // set soul = null
        instance.soul = null;
    }
    
    public static void BodyDies(Player body)
    {
        if(!instance.bodyList.Remove(body))
        {
            Debug.LogError("Error: body " + body.name + " not found in bodyList!");
        }
        instance.bodyCount -= 1;
        instance.UpdateBodyCountText();
        Destroy(body.gameObject);
    }

    public void UpdateBodyCountText()
    {
        bodyCountText.text = string.Format("x {0} / {1}", bodyCount, maxBody);
    }

    public void CollectStar()
    {
        collectedStars += 1;
        if(collectedStars == totalStars)
        {
            // Do something when all gets collected
        }
        if(collectedStars > totalStars)
        {
            Debug.LogError("Not all stars in Scene " + SceneManager.GetActiveScene().name + " are in stars GameObject!!!");
        }
    }

    public void LoadNextLevel()
    {
        if(nextLevelSceneName == "")
        {
            Debug.LogWarning("Next Level Scene Name is empty. Cannot load next level!");
        }
        else
        {
            audioManager.ChangeVolume("UpMusic", 1f);
            audioManager.ChangeVolume("DownMusic", 0f);
            SceneManager.LoadScene(nextLevelSceneName);
        }
    }

    private void OnDestroy() {
        PlayerCrossedLine-=OnPlayerCrossedLine;
    }

    // Consider Moving the next 2 methods in PlayerControl
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

    public static void OnPlayerCrossedLine(Player playerInstance){
        
        switch(playerInstance.Status){
            case PlayerState.me:
                //playerInstance.Status = PlayerState.soul;
                MeDies();
                
                break;
            case PlayerState.soul:
                //playerInstance.Status = PlayerState.dead;
                SoulDies();
                break;
            default:
                BodyDies(playerInstance);
            break;
        }
        // playerInstance.FlipY();
        //Debug.Log($"Player {playerInstance.name} crossed the RedLine. New State: {playerInstance.Status}");
    }

    public static Vector3 GetOppositePos(Vector3 pos){
        // Check if pos is above the line at y=-0.5
        if (pos.y >= -0.0f)
        {
            //return new Vector3(pos.x, -pos.y - 1f,0);
            return new Vector3(pos.x, -pos.y ,0);
        }
        // pos is below the line at y=-0.5
        else
        {
            return new Vector3(pos.x, -pos.y ,0);
            //return new Vector3(pos.x, -pos.y + 1f,0);
        }
    }

}
