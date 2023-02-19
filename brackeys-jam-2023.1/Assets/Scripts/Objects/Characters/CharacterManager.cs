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
    [SerializeField] private TMP_Text starCountText;
    private int bodyCount = 0; // bodyCount == bodies.ToArray().Length
    [SerializeField] private StartPortal spawnPortal;
    [SerializeField] private GameObject playerPrefab; 

    [SerializeField] private GameObject stars;
    private int totalStars;
    public int collectedStars = 0;

    [SerializeField] private LevelTransition levelTransition;
    private string nextLevelSceneName = "";

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

        totalStars = stars.transform.childCount;

        string[] sceneNamesList = LevelSelectionController.Instance.levelSceneNameList;
        string currentSceneName = SceneManager.GetActiveScene().name;
        int sceneIndex = System.Array.IndexOf(sceneNamesList, currentSceneName);
        Debug.Log(sceneIndex);
        if(sceneIndex == -1)
        {
            Debug.LogWarning("Scene " + currentSceneName + " cannot be found in sceneNamesList under LevelSelectionController!");
            nextLevelSceneName = "LevelSelectionScene";
        } 
        else if(sceneIndex == sceneNamesList.Length - 1)
        {
            Debug.LogWarning("Scene " + currentSceneName + " is the last level! Next level is the level selection");
            nextLevelSceneName = "LevelSelectionScene";
        }
        else
        {
            nextLevelSceneName = sceneNamesList[sceneIndex + 1];
            Debug.Log(nextLevelSceneName);
        }

        UpdateBodyCountText();
        UpdateStarCountText();

        // Init Collectables related
        audioManager.ChangeVolume("UpMusic", 1f);
        audioManager.ChangeVolume("DownMusic", 0f);
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
        // Play animation if swtich active
        if(!me._active)
        {
            me.Highlight();
        }

        // Set active
        me._active = true;
        soul._active = false;

        // Play switch sound
        audioManager.PlaySound("Down_to_Up");
        audioManager.StopSound("Up_to_Down");

        // Change music volume

        audioManager.ChangeVolume("UpMusic", 1f);
        audioManager.ChangeVolume("DownMusic", 0f);
    }

    public void SwitchSoul()
    {
        // Play animation if swtich active
        if(!soul._active)
        {
            soul.Highlight();
        }

        // Set active
        me._active = false;
        soul._active = true;

        // Play switch sound
        audioManager.PlaySound("Up_to_Down");
        audioManager.StopSound("Down_to_Up");

        // Change music volume

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
        // Death particle effect
        instance.me.GenerateExplodeParticles();
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
        SpawnMe(instance.spawnPortal.transform.position);

        instance.SwitchSoul();
    }

    public static void SoulDies()
    {
        // Death particle effect
        instance.soul.GenerateExplodeParticles();
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
        body.GenerateExplodeParticles();
        if(!instance.bodyList.Remove(body))
        {
            Debug.LogError("Error: body " + body.name + " not found in bodyList!");
        }
        // Death particle effect

        instance.bodyCount -= 1;
        instance.UpdateBodyCountText();
        Destroy(body.gameObject);
    }

    public void UpdateBodyCountText()
    {
        bodyCountText.text = string.Format("{0}/{1}", bodyCount, maxBody);
    }

    public void UpdateStarCountText()
    {
        starCountText.text = string.Format("{0}/{1}", collectedStars, totalStars);
    }

    public void CollectStar()
    {
        collectedStars += 1;
        UpdateStarCountText();
        if(collectedStars == totalStars)
        {
            // Do something when all gets collected
        }
        if(collectedStars > totalStars)
        {
            Debug.LogError("Not all stars in Scene " + SceneManager.GetActiveScene().name + " are in stars GameObject!!!");
        }
    }

    public void OpenSpawnPortal()
    {
        spawnPortal.OpenPortal();
    }

    public void LevelComplete()
    {
        levelTransition.StartLoading();
    }

    public void LoadNextLevel()
    {
        string currentLevel = GameManager.Instance.currentLevel;
        Debug.Log(currentLevel);
        LevelSelectionController.Instance.gameObject.SetActive(true);
        BonusStarHandler[] bonusStarHandlers = FindObjectsOfType<BonusStarHandler>();

        foreach (BonusStarHandler bonusStarHandler in bonusStarHandlers)
        {
            if (bonusStarHandler.gameObject.CompareTag("Level") && bonusStarHandler.gameObject.name == currentLevel)
            {
                Debug.Log("Setting Stars");
                bonusStarHandler.numStarsCollected = GameManager.Instance.currentStars;
            }
        }

        LevelSelectionController.Instance.gameObject.SetActive(false);
        if(nextLevelSceneName == "")
        {
            Debug.LogWarning("Next Level Scene Name is empty. Cannot load next level!");
        }
        else
        {
            Time.timeScale = 1f;
            audioManager.ChangeVolume("UpMusic", 1f);
            audioManager.ChangeVolume("DownMusic", 0f);

            if(nextLevelSceneName == "LevelSelectionScene")
            {
                LevelSelectionController.Instance.gameObject.SetActive(true);
            }
            SceneManager.LoadScene(nextLevelSceneName);
        }

        GameManager.Instance.currentStars = 0;
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
