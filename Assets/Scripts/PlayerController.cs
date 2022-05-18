using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private GameObject Sword;
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider;
    private Vector2 moveInput;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite ChestOpen;
    [SerializeField] private Sprite ChestClosed;
    [SerializeField] private GameObject Chest;
    [SerializeField] private GameObject itemGameObject;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image HealthBarKoordinates;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    private Item item;
    private Player player = new Player();
    private float playerSpeed = .02f;
    private float playerjumpForce = 2f;
    private float playerHealth = 100;
    private float maxHealth = 100;
    private int i = 0;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        item = itemGameObject.GetComponent<Item>();
    }
    private void Awake()
    {
        // To store the component Rigidbody in the property _Rigidbody
        // so that you can use it anywhere in your code.
        TryGetComponent(out rigidbody);
    }
    // Update is called once per frame
    private void Update()
    {
        PullOutWeapon();
        //Check for Internet Connection
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
        }
        
        //this is executed in Update because in FixedUpdate GetKeyDown runs twice
         if(Input.GetKeyDown(KeyCode.E))
            OpenChest();

         //Enter pause menu
         if (Input.GetKeyDown(KeyCode.Escape))
             PauseMenu();
         
    }
    private void FixedUpdate()
    {
        
        moveInput = new Vector2(Input.GetAxis("Horizontal"), 0);
        //fixes the HealthBar to the player
        var temp = PlayerObject.transform.position;
        uiCanvas.transform.position = new Vector3(temp.x, temp.y, temp.z);
        if (playerHealth <= 0)
        {
            Respawn();
        }
        playerMovement();
    }

    public void PauseMenu()
    {
        i++;
        pauseMenuCanvas.transform.position = PlayerObject.transform.position;
        pauseMenuCanvas.SetActive(true);
        HealthBarKoordinates.gameObject.SetActive(false);
        Time.timeScale = 0;
        //Exit pause menu
        if (i % 2 == 0)
        {
            Time.timeScale = 1;
            HealthBarKoordinates.gameObject.SetActive(true);
            pauseMenuCanvas.SetActive(false);
            i = 0;
        }
    }
    private void playerMovement()
    {
        //Jump if feet touch solid object
        if (IsGrounded() && Input.GetAxisRaw("Jump") > 0){
            rigidbody.velocity = Vector2.up * playerjumpForce;
        }

        rigidbody.position += moveInput * playerSpeed;
        
        //Swap sprite direction if your moving in different dirrections
        if (moveInput.x > 0)
            transform.localScale = Vector3.one;
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        
    }
    public void IncreasePlayerSpeed(float increment)
    {
        playerSpeed *= increment;
    }
    public void IncreasePlayerjumpSpeed(float increment)
    {
        playerjumpForce *= increment;
    }
    public void ChangePlayerHealth(int decrement)
    {
        if (playerHealth - decrement > maxHealth)
        {
            Debug.Log("MAX Health");
            playerHealth = maxHealth;
            healthBarImage.fillAmount = Mathf.Clamp(playerHealth, 0, 1);
        }
        else
        {
            playerHealth -= decrement;
            //convert to percentage
            float healthPercentage = playerHealth / maxHealth;
            //reduce health bar size
            healthBarImage.fillAmount = Mathf.Clamp(healthPercentage, 0, 1f);
        }
    }

    private void PullOutWeapon()
    {
        if (Input.GetMouseButtonDown(0)) {
            Sword.gameObject.SetActive(true);
        }
        if(Input.GetMouseButtonUp(0))   {
            Sword.gameObject.SetActive(false);
        }
    }
    private bool CheckForChest(){
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(boxCollider.bounds.center, .2f);
        int i = 0;
        while (i < hitColliders.Length){
            if (hitColliders[i].CompareTag("Chest") && hitColliders[i].GetComponent<SpriteRenderer>().sprite == ChestClosed)
            {
                hitColliders[i].GetComponent<SpriteRenderer>().sprite = ChestOpen;
                return true;
            }
            i++;
        }

        return false;
    }
    private Vector3 GetPosOfNearestChest() {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(boxCollider.bounds.center, .2f);
        int i = 0;
        while (i < hitColliders.Length){
            if (hitColliders[i].CompareTag("Chest") && hitColliders[i].GetComponent<SpriteRenderer>().sprite == ChestOpen)
            {
                return hitColliders[i].transform.position;
            }
            i++;
        }
        return Vector3.zero;
    }
    private void OpenChest()
    {
        if (CheckForChest())
        {
            player.Name = player.getName();
            StartCoroutine(getRequest("http://localhost:5000/record/" + player.Name));
            
            GameObject newItem = Instantiate(itemGameObject);
            newItem.GetComponent<SpriteRenderer>().sprite = item.GETRandomSprite();
            newItem.transform.position = GetPosOfNearestChest()+ new Vector3(0,.25f,0);
        }
    }
    private bool IsGrounded(){
        var bounds = boxCollider.bounds;
        var raycastHit2D = Physics2D.BoxCast(bounds.center, bounds.size, 0f,
            Vector2.down, .1f, platformsLayerMask);

        return raycastHit2D.collider != null;
    }  //so you cant jump twice
    public void Respawn()
    {
        playerHealth = 100;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Menu");
    }
    IEnumerator getRequest(string uri)
    {
        var uwr = new UnityWebRequest(uri, "GET");
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();
        
        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            //Debug.Log("Received: " + uwr.downloadHandler.text);
            player = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
            player.ChestsOpened += 1;
            string json = JsonConvert.SerializeObject(player);
            StartCoroutine(putRequest("http://localhost:5000/update/"+player.Name, json));
        }
        
    }
    IEnumerator putRequest(string url, string json)
    {
        //spent 3 hours because this doesn't work for some reason
        /*byte[] dataToPut = System.Text.Encoding.UTF8.GetBytes(json);
        UnityWebRequest uwr = UnityWebRequest.Put(url, dataToPut);
        yield return uwr.SendWebRequest();*/
        
        var uwr = new UnityWebRequest(url, "PUT");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        /*else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }*/
    }
    public void loadMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
        
    }
    
}
