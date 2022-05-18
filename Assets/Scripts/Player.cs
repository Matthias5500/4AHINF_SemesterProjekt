
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player
    {
        public string Name = "";
        public string Email = "";
        public string Password = "";
        /*public int Health = 100;
        public float JumpVelocity = 2f;
        public float PlayerSpeed = .02f;*/
        public int ChestsOpened = 0;
        public bool[] ItemsCollected = new bool[3];

        public string getName()
        {
            string path = Application.persistentDataPath + "/CurrentPlayer.txt";
            StreamReader reader = new StreamReader(path);
            Name = reader.ReadToEnd();
            reader.Close();

            return Name;
        }


    }
}