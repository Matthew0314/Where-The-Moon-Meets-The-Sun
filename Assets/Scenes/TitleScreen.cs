using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update

    public static string difficulty;
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void PlayGame() {
        SceneManager.LoadScene("Prologue");
    }
}
