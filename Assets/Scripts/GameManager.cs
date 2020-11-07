using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static void ChangerScene(string nomScene){
        switch (nomScene){
            case "Start":
            SceneManager.LoadScene("Start");
            break;
            case "Level 1":
            SceneManager.LoadScene("Level 1");
            break;
            case "Level 2":
            SceneManager.LoadScene("Level 2");
            break;
            case "End":
            SceneManager.LoadScene("End");
            break;
        }
    }
}
