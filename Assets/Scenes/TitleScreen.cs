using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;


public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Button> startButtons;
    [SerializeField] List<Button> difficultyButtons;
    [SerializeField] GameObject diffMenu;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject controlMenu;
    string[] difficulties = { "Normal", "Hard", "Eclipse" };
    bool inStartMenu = true;
    bool inDiffMenu = false;
    bool axisInUse = false;
    bool oneAction = false;
    bool inControlMenu = false;
    public static string difficulty = " ";
    int currentIndex = 0;


    Gamepad gamepad;
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        startButtons[0].Select();
    }

    // // Update is called once per frame
    void Update()
    {
        gamepad = Gamepad.current;

        if (inControlMenu) {
            if ((Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)) || (Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame))) {
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
                    if (currentIndex < 0) { currentIndex = startButtons.Count - 1; }
                    startButtons[currentIndex].Select();
                    axisInUse = true;
                } else if(diffMenu) {
                    difficultyButtons[currentIndex].OnDeselect(null);
                    currentIndex--;
                    if (currentIndex < 0) { currentIndex = difficultyButtons.Count - 1; }
                    difficultyButtons[currentIndex].Select();
                    axisInUse = true;
                }
                // buttons[currentIndex].OnDeselect(null);
                // // currentIndex = (currentIndex - 1 + buttons.Count) % buttons.Count;
                // currentIndex--;
                // if (currentIndex < 0) { currentIndex = buttons.Count - 1; }
                // buttons[currentIndex].Select();
                // axisInUse = true;
                // // yield return new WaitForSeconds(0.25f);
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
                // buttons[currentIndex].OnDeselect(null);
                // // currentIndex = (currentIndex + 1) % buttons.Count;
                // currentIndex++ ;
                // if (currentIndex >= buttons.Count) { currentIndex = 0; }
                // buttons[currentIndex].Select();
                // axisInUse = true;
                // // yield return new WaitForSeconds(0.25f);
            }
        }

        if (Mathf.Abs(vertical) < 0.2f)
        {
            axisInUse = false;
        }

        if (oneAction && (Input.GetKeyDown(KeyCode.Space) || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame))) // "Submit" button
        {
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

    public void PlayGame() {
        SceneManager.LoadScene("Prologue");
    }

    void ActivateDiffMenu() {
        diffMenu.SetActive(true);
    }

    void DeactivateDiffMenu() {
        diffMenu.SetActive(false);
    }

    void DeactivateStartMenu() {
        startMenu.SetActive(false);
    }

    void ActivateStartMenu() {
        startMenu.SetActive(true);
    }

    void ActivateControlMenu() {
        controlMenu.SetActive(true);
    }

    void DeactivateControlMenu() {
        controlMenu.SetActive(false);
    }

    public static string GetDifficulty() {
        return difficulty;
    }
}
