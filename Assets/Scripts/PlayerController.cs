using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private GameObject PlayerObject;
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider;
    private Vector2 moveInput;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite ChestOpen;
    [SerializeField] private Sprite ChestClosed;
    [SerializeField] private GameObject Chest;
    [SerializeField] private GameObject itemGameObject;
    private Item item;
    private Player player = new Player();

    public void IncreasePlayerSpeed(float increment)
    {
        player.PlayerSpeed *= increment;
    }
    public void IncreasePlayerjumpSpeed(float increment)
    {
        player.JumpVelocity *= increment;
    }

    public void DecreasePlayerHealth(int decrement)
    {
        player.Health -= decrement;
    }
    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        item = itemGameObject.GetComponent<Item>();
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
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
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
        
        if (player.Health <= 0)
        {
            Respawn();
        }
        /* To add values to the moveInput property you write "new Vector3 (x, y, z)" and fill those
        values with the inputs that you would like to use. In my case I used for the X axis
        Input.GetAxis("Horizontal") to get the input values from the default
        keys A, D, Left Arrow and Right Arrow and for the Z axis Input.GetAxis("Vertical") to
        get the default input values from the keys W, S, Up Arrow and Down Arrow.
        */
        moveInput = new Vector2(Input.GetAxis("Horizontal"), 0);
        //this is executed in Update because in FixedUpdate GetKeyDown runs twice
         if(Input.GetKeyDown(KeyCode.E))
            OpenChest();
    }
    private void FixedUpdate()
    {
        
        playerMovement();
    }

    private void playerMovement()
    {
        //Jump if feet touch solid object
        if (IsGrounded() && Input.GetAxisRaw("Jump") > 0){
            rigidbody.velocity = Vector2.up * player.JumpVelocity;
        }

        rigidbody.position += moveInput * player.PlayerSpeed;
        
        //Swap sprite direction if your moving in different dirrections
        if (moveInput.x > 0)
            transform.localScale = Vector3.one;
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        
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
            string path = "C:/Users/mattl/Desktop/Schule/POS/UnityGame/4AHINF_SemesterProjekt/Assets/Scripts/CurrentPlayer.txt";
            StreamReader reader = new StreamReader(path);
            player.Name = reader.ReadToEnd();
            player.ChestsOpened += 1;
            string json = JsonConvert.SerializeObject(player);
            StartCoroutine(putRequest("http://localhost:5000/update/"+player.Name, json));
            
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
    }

    public void Respawn()
    {
        player.Health = 100;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Menu");
    }


    IEnumerator getRequest(string uri, string json)
    {
        var uwr = new UnityWebRequest(uri, "GET");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler) new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            var temp = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
            if (temp != null)
            {
                player = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
            }
        }
    }
}
