using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BattleStartMenu : MonoBehaviour
{
    private Button[,] buttons;
    private string[,] actions;
    private int rows = 2;
    private int columns = 2;
    [SerializeField] Button unitButton;
    [SerializeField] Button mapButton;
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject battleStartMenu;

    private int currentRow = 0;
    private int currentCol = 0;
    public float sensitivity = 0.5f;
    public Vector2 moveInput;
    public PlayerInput playerInput;
    private bool inStartMenu = false;
    private bool inMapMenu = false;
    private PlayerGridMovement playerGridMovement;

    void Awake()
    {
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();

        if (unitButton == null || mapButton == null || startButton == null || exitButton == null)
        {
            Debug.LogError("One or more buttons are not assigned in the inspector.");
            return;
        }

        actions = new string[2, 2]
        {
            { "Units", "Map" },
            { "Start" , "Exit" }
        };

        buttons = new Button[2, 2]
        {
            { unitButton, mapButton },
            { startButton, exitButton }
        };
    }

    void Update()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
    }

    public IEnumerator StartMenu()
    {
        battleStartMenu.SetActive(true);
        inStartMenu = true;

        bool axisInUse = false;
        bool oneAction = false;

        int currentRow = 0;
        int currentCol = 0;

        int rowCount = buttons.GetLength(0); // 2
        int colCount = buttons.GetLength(1); // 2

        // Select initial button
        buttons[currentRow, currentCol].Select();

        while (true)
        {
            float vertical = moveInput.y;
            float horizontal = moveInput.x;

            if (!axisInUse)
            {
                // Move Up
                if (vertical > sensitivity)
                {
                    currentRow = (currentRow - 1 + rowCount) % rowCount;
                    axisInUse = true;
                }
                // Move Down
                else if (vertical < -sensitivity)
                {
                    currentRow = (currentRow + 1) % rowCount;
                    axisInUse = true;
                }
                // Move Left
                else if (horizontal < -sensitivity)
                {
                    currentCol = (currentCol - 1 + colCount) % colCount;
                    axisInUse = true;
                }
                // Move Right
                else if (horizontal > sensitivity)
                {
                    currentCol = (currentCol + 1) % colCount;
                    axisInUse = true;
                }

                Debug.Log(currentRow + " " + currentCol);

                // Select new button
                buttons[currentRow, currentCol].Select();
            }

            // Reset axis flag when joystick is released
            if (Mathf.Abs(vertical) < sensitivity && Mathf.Abs(horizontal) < sensitivity)
                axisInUse = false;

            // Handle Selection
            if (oneAction && playerInput.actions["Select"].WasPressedThisFrame())
            {
                string selectedAction = actions[currentRow, currentCol];

                // if (selectedAction == "Start") { StartGame(); break; }
                // else if (selectedAction == "Exit") { ExitGame(); break; }
                // else if (selectedAction == "Units") { OpenUnits(); break; }
                // else if (selectedAction == "Map") { OpenMap(); break; }
                // else continue;

                if (selectedAction == "Start")
                {
                    break;
                }
                else if (selectedAction == "Map")
                {
                    yield return StartCoroutine(InMapMenu());
                }
            }

            // Handle Back
            // if (oneAction && playerInput.actions["Back"].WasPressedThisFrame())
            // {
            //     CloseMenu();
            //     break;
            // }

            oneAction = true;

            yield return null;
        }

        battleStartMenu.SetActive(false);
        inStartMenu = false;

        yield return null;
    }

    private IEnumerator InMapMenu()
    {
        battleStartMenu.SetActive(false);
        inMapMenu = true;
        while (true)
        {
            if (playerInput.actions["Back"].WasPressedThisFrame() && playerGridMovement.BackToStartMenu())
            {
                battleStartMenu.SetActive(true);
                inMapMenu = false;
                yield break;
            }

            yield return null;
        }

        yield return null;
    }


    public bool GetInStartMenu() => inStartMenu;
    public bool GetInMapMenu() => inMapMenu;
}
