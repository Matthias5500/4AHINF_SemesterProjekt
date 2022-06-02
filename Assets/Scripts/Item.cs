using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using Random = UnityEngine.Random;
using DefaultNamespace;


public class Item : MonoBehaviour
    {
        [SerializeField] private GameObject item;
        [SerializeField] Sprite CoffeCup;
        [SerializeField] Sprite jumpItemSprite;
        [SerializeField] Sprite healthItemSprite;
        [SerializeField] Sprite MissileLauncher;
        private int ItemCount;
        private SpriteRenderer _spriteRenderer;
        private Player player = new Player();
        private int num;

        public Item(SpriteRenderer spriteRenderer)
        {
            _spriteRenderer = spriteRenderer;
        }
        
        //Choose random item
        public Sprite GETRandomSprite() {
            ItemCount = 5;  //must be 1 higher than the number of items
            Sprite randItemSprite = null;
            int rand = Random.Range(1, ItemCount);
            if (rand == 1) // 1
                randItemSprite = CoffeCup;
            else if(rand == 2) // 2
                randItemSprite = jumpItemSprite;
            else if(rand == 3) // 3
                randItemSprite = healthItemSprite;
            else if (rand == 4) // 4
                randItemSprite = MissileLauncher;
            
            return randItemSprite;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            player = new Player();
            Sprite itemSprite = item.GetComponent<SpriteRenderer>().sprite; //get the sprite of the item
            if (other.CompareTag("Player")) { //if the item is picked up by the player
                var playerController = other.GetComponent(nameof(PlayerController)) as PlayerController;
                if (playerController) { //if playerController is not null
                    //if you delete the item here it throws an error therefore its moved away so the player can't pick it up multiple times 
                    item.transform.position = new Vector3(0, 1000, 0);
                    //Player gets more speed
                    if (itemSprite == CoffeCup) { // if the item is a coffee cup
                        playerController.IncreasePlayerSpeed(1.2f); //increase the players speed by 20%
                        num = 0; //set num to 0 because the coffee cups id is 0
                        //update ItemsCollected[] so the CoffeeCup is displayed as true
                        StartCoroutine(getRequest("http://localhost:5000/record/" + player.getName(), num));
                    }
                    //Player gets more jump speed
                    else if (itemSprite == jumpItemSprite) {
                        playerController.IncreasePlayerjumpSpeed(1.2f);
                        num = 1;
                        StartCoroutine(getRequest("http://localhost:5000/record/" + player.getName(), num));
                    }
                    //Player gets more health
                    else if (itemSprite == healthItemSprite) {
                        playerController.ChangePlayerHealth(-50);
                        num = 2;
                        StartCoroutine(getRequest("http://localhost:5000/record/" + player.getName(), num));
                    }
                    //Player gets more Missiles
                    else if (itemSprite == MissileLauncher) {
                        playerController.IncreaseMissileCount(2);
                        num = 3;
                        StartCoroutine(getRequest("http://localhost:5000/record/" + player.getName(), num));
                    }
                }
            }
        }

        IEnumerator getRequest(string uri, int num)
        {
            var uwr = new UnityWebRequest(uri, "GET");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
                player = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
                
                player.ItemsCollected[num] = true;

                string json = JsonConvert.SerializeObject(player);
                StartCoroutine(putRequest("http://localhost:5000/update/"+player.Name, json));
            }
            Destroy(item);
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
    }
