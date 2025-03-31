using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;


public class TitleScreen : MonoBehaviour
{
    // Stores the buttons that can be chosen on each menu state
    [SerializeField] List<Button> startButtons;
    [SerializeField] List<Button> difficultyButtons;

    // Stores the different menus to make active and deactivate later on
    [SerializeField] GameObject diffMenu;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject controlMenu;

    // Stores the different difficulty strings that can be chosen
    string[] difficulties = { "Normal", "Hard", "Eclipse" };
    public static string difficulty = " ";

    // Bools to dictate which menu it is on
    bool inStartMenu = true;
    bool inDiffMenu = false;
    bool inControlMenu = false;

    // Used to prevent multiple actions from taking place
    bool axisInUse = false;
    bool oneAction = false;
    int currentIndex = 0;
    private PlayerInput playerInput; 

    void Start() {
        // Stores the player input component
        playerInput = GameObject.Find("GameManager").GetComponent<PlayerInput>();

        // Sets resolution, idk if this actually works
        Screen.SetResolution(1920, 1080, true);

        // Initilizes the first button that the player is on
        startButtons[0].Select();
    }

    void Update()
    {
        // If player is in the menu that shows the games controlls
        if (inControlMenu) {
            if (playerInput.actions["Select"].WasPressedThisFrame() || playerInput.actions["Back"].WasPressedThisFrame()) {
                inStartMenu = true;
                inControlMenu = false;
                DeactivateControlMenu();
                ActivateStartMenu();
                currentIndex = 1;
                startButtons[currentIndex].Select();
                
            }

            return;
        }
        float vertical = Input.GetAxis("Vertical");

        if (!axisInUse)
        {
            if (vertical > 0.2f) // Move up
            {
                if(inStartMenu) {
                    startButtons[currentIndex].OnDeselect(null);
                    currentIndex--;
                    if (currentIndex < 0) currentIndex = startButtons.Count - 1;
                    startButtons[currentIndex].Select();
                    axisInUse = true;
                } else if(diffMenu) {
                    difficultyButtons[currentIndex].OnDeselect(null);
                    currentIndex--;
                    if (currentIndex < 0) currentIndex = difficultyButtons.Count - 1;
                    difficultyButtons[currentIndex].Select();
                    axisInUse = true;
                }
            }
            else if (vertical < -0.2f) // Move down
            {
                if(inStartMenu) {
                    startButtons[currentIndex].OnDeselect(null);
                    currentIndex++;
                    if (currentIndex >= startButtons.Count) { currentIndex = 0; }
                    startButtons[currentIndex].Select();
                    axisInUse = true;
                } else if(inDiffMenu) {
                    difficultyButtons[currentIndex].OnDeselect(null);
                    currentIndex++;
                    if (currentIndex >= difficultyButtons.Count) { currentIndex = 0; }
                    difficultyButtons[currentIndex].Select();
                    axisInUse = true;
                }
            }
        }

        if (Mathf.Abs(vertical) < 0.2f)
        {
            axisInUse = false;
        }

        if (oneAction && playerInput.actions["Select"].WasPressedThisFrame()) {
            if(inStartMenu) {
                if (currentIndex == 0) {
                    ActivateDiffMenu();
                    DeactivateStartMenu();
                    inStartMenu = false;
                    inDiffMenu = true;
                    currentIndex = 0;
                    difficultyButtons[0].Select();
                } else {
                    ActivateControlMenu();
                    DeactivateStartMenu();
                    inControlMenu = true;
                    inStartMenu = false;
                }
            } else if (inDiffMenu) {
                difficulty = difficulties[currentIndex];
                Debug.Log(difficulty + " Selected");
                PlayGame();
            }
        }

        oneAction = true;
    }

    // Loads prologue screen
    public void PlayGame() => SceneManager.LoadScene("Prologue");

    // Methods for activating and deactivating menus
    void ActivateDiffMenu() => diffMenu.SetActive(true);

    void DeactivateDiffMenu() => diffMenu.SetActive(false);

    void DeactivateStartMenu() => startMenu.SetActive(false);

    void ActivateStartMenu() => startMenu.SetActive(true);

    void ActivateControlMenu() => controlMenu.SetActive(true);

    void DeactivateControlMenu() => controlMenu.SetActive(false);

    // Returns the difficulty
    public static string GetDifficulty() => difficulty;
}
