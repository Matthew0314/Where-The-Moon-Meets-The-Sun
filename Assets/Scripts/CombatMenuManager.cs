using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatMenuManager : MonoBehaviour
{
    private IMaps _currentMap;
    private TurnManager manageTurn;    
    private PlayerGridMovement moveGrid;
    private GenerateGrid generateGrid;
    // private UnitManager DefendingEnemy;
    // private UnitManager AttackingUnit;
    // public Transform moveCursor;
    // private int attackerX;
    // private int attackerZ;
    // private int defenderX;
    // private int defenderZ;

    //Action Menu
    private GameObject attackButton;
    private GameObject itemButton;
    private GameObject waitButton;


    //hover menu
    private GameObject enemyBar;
    private GameObject playerBar;
    private GameObject hoverMenu;

    private TextMeshProUGUI levelText;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI weaponText;
    private TextMeshProUGUI unitNameText;


    //Expected Menu
    private Image EHealthLost;
    private Image EHealth;
    private Image PHealthLost;
    private Image PHealth;
    private GameObject Menu;

    private TextMeshProUGUI PlayerName;
    private TextMeshProUGUI PlayerWeapon;
    private TextMeshProUGUI PlayerDamage;
    private TextMeshProUGUI PlayerHit;
    private TextMeshProUGUI PlayerCrit;
    private TextMeshProUGUI PlayerCurrHealth;

    private TextMeshProUGUI EnemyName;
    private TextMeshProUGUI EnemyWeapon;
    private TextMeshProUGUI EnemyDamage;
    private TextMeshProUGUI EnemyHit;
    private TextMeshProUGUI EnemyCrit;
    private TextMeshProUGUI EnemyCurrHealth;

    [SerializeField] GameObject HPIndicator;
    GameObject HPplayer;
    GameObject HPenemy;

    // Start is called before the first frame update
    void Start()
    {
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        moveGrid = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        manageTurn = GameObject.Find("GridManager").GetComponent<TurnManager>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();

        //Action Menu
        attackButton = GameObject.Find("Canvas/AttackButton");
        itemButton = GameObject.Find("Canvas/WaitButton");
        waitButton = GameObject.Find("Canvas/ItemButton");

        DeactivateActionMenu();

        //Hover Menu
        enemyBar = GameObject.Find("Canvas/HoverUnitMenu/EnemyBar");
        playerBar = GameObject.Find("Canvas/HoverUnitMenu/PlayerBar");
        hoverMenu = GameObject.Find("Canvas/HoverUnitMenu");

        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        weaponText = GameObject.Find("WeaponText").GetComponent<TextMeshProUGUI>();
        unitNameText = GameObject.Find("UnitNameText").GetComponent<TextMeshProUGUI>();

        DeactivateHoverMenu();

        //Expected Menu
        EHealthLost = GameObject.Find("EHealthLostBar").GetComponent<Image>();
        EHealth = GameObject.Find("EnemyHealthBar").GetComponent<Image>();
        PHealthLost = GameObject.Find("PHealthLostBar").GetComponent<Image>();
        PHealth = GameObject.Find("PlayerHealthBar").GetComponent<Image>();
        Menu = GameObject.Find("Canvas/ExpectedBattleMenu");

        //Canvas/ExpectBattleMenu/Text/Player/
        PlayerName = GameObject.Find("PlayerUnitName").GetComponent<TextMeshProUGUI>();
        PlayerWeapon = GameObject.Find("PlayerWeapon").GetComponent<TextMeshProUGUI>();
        PlayerDamage = GameObject.Find("PlayerDamage").GetComponent<TextMeshProUGUI>();
        PlayerHit = GameObject.Find("PlayerHit").GetComponent<TextMeshProUGUI>();
        PlayerCrit = GameObject.Find("PlayerCrit").GetComponent<TextMeshProUGUI>();
        PlayerCurrHealth = GameObject.Find("PlayerCurrentHealth").GetComponent<TextMeshProUGUI>();

        //Canvas/ExpectBattleMenu/Text/Enemy/
        EnemyName = GameObject.Find("EnemyUnitName").GetComponent<TextMeshProUGUI>();
        EnemyWeapon = GameObject.Find("EnemyWeapon").GetComponent<TextMeshProUGUI>();
        EnemyDamage = GameObject.Find("EnemyDamage").GetComponent<TextMeshProUGUI>();
        EnemyHit = GameObject.Find("EnemyHit").GetComponent<TextMeshProUGUI>();
        EnemyCrit = GameObject.Find("EnemyCrit").GetComponent<TextMeshProUGUI>();
        EnemyCurrHealth = GameObject.Find("EnemyCurrentHealth").GetComponent<TextMeshProUGUI>();

        DeactivateExpectedMenu();
    }

    public void ActivateActionMenu() {
        attackButton.SetActive(true);
        waitButton.SetActive(true);
        itemButton.SetActive(true);
    }

    public void DeactivateActionMenu() {
        attackButton.SetActive(false);
        waitButton.SetActive(false);
        itemButton.SetActive(false);
    }


    public void PlayerWait() {
        
        manageTurn.RemovePlayer(moveGrid.playerCollide.GetPlayer().stats);
        moveGrid.unitWait();
        manageTurn.CheckPhase();
        _currentMap.CheckClearCondition();
        
        
    }

    public void PlayerAttack() {

        Debug.Log("Attack");
        moveGrid.isAttacking = true;
        List<GridTile> UnitsInRange = new List<GridTile>();
        
        
        for (int i = 0; i < generateGrid.GetWidth(); i++) {
            for (int j = 0; j < generateGrid.GetLength(); j++) {
                if (moveGrid.attackGrid[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                    
                    UnitsInRange.Add(generateGrid.GetGridTile(i,j));
                    
                }
            }
        }

        if (UnitsInRange.Count == 0) {
            moveGrid.isAttacking = false;
            return;
        }

        DeactivateActionMenu();

        StartCoroutine(moveGrid.CycleAttackList(UnitsInRange));

    }


//---------------------------------------Hover Menu---------------------------------------------------//

    public void DeactivateHoverMenu() {
        enemyBar.SetActive(false);
        playerBar.SetActive(false);
        hoverMenu.SetActive(false);
    }

    public void ActivateHoverMenu(UnitManager unit) {

        levelText.text = $"{unit.stats.Level}";
        healthText.text = $"{unit.currentHealth} / {unit.stats.Health}";

        if (unit.primaryWeapon != null) {
            weaponText.text = unit.primaryWeapon.WeaponName;
        } else {
            weaponText.text = "";
        }

        unitNameText.text = unit.stats.Name;

        hoverMenu.SetActive(true);

        if (unit.stats.UnitType == "Player") {
            playerBar.SetActive(true);
        } else if (unit.stats.UnitType == "Enemy") {
            enemyBar.SetActive(true);
        } else {
            //Should never be called
            playerBar.SetActive(true);
        }     
    }

//-------------------------------------Expected Menu--------------------------------------------------//

public void SetUpExpectedMenu(UnitManager player, UnitManager enemy, int expectedPlayerHP, int expectedEnemyHP, int PDamage, int EDamage, int numPHits, int numEHits) {
        int playerHit = player.primaryWeapon.HitRate + (player.stats.Luck * 4) - enemy.stats.Evasion;
        int enemyHit = enemy.primaryWeapon.HitRate + (enemy.stats.Luck * 4) - player.stats.Evasion;
        int playerCrit = player.primaryWeapon.CritRate + (int)(player.stats.Luck / 2);
        int enemyCrit = enemy.primaryWeapon.CritRate + (int)(enemy.stats.Luck / 2);

        Debug.Log(expectedEnemyHP);

        if (playerHit > 100) {
            playerHit = 100;
        } else if (playerHit < 0) {
            playerHit = 0;
        }

        if (playerCrit > 100) {
            playerCrit = 100;
        } else if (playerCrit < 0) {
            playerCrit = 0;
        }
        if (enemyHit > 100) {
            enemyHit = 100;
        } else if (enemyHit < 0) {
            enemyHit = 0;
        }
        if (enemyCrit > 100) {
            enemyCrit = 100;
        } else if (enemyCrit < 0) {
            enemyCrit = 0;
        }

        PlayerName.text = player.stats.Name;
        PlayerWeapon.text = player.primaryWeapon.WeaponName;
        
        if (numPHits > 1) {
            PlayerDamage.text = $"{PDamage} x {numPHits}";
            PlayerHit.text = $"{playerHit}%";
            PlayerCrit.text = $"{playerCrit}%";
        } else if (numPHits == 0) {
            PlayerDamage.text = "-";
            PlayerHit.text = "-";
            PlayerCrit.text = "-";
        } else {
            PlayerDamage.text = $"{PDamage}";
            PlayerHit.text = $"{playerHit}%";
            PlayerCrit.text = $"{playerCrit}%";
        }

        PlayerCurrHealth.text = $"{player.currentHealth}";

        if (expectedPlayerHP < 0) { expectedPlayerHP = 0; }
        if (expectedEnemyHP < 0) { expectedEnemyHP = 0; }

        float PLostFill = ((float)expectedPlayerHP / (float)player.stats.Health);
        float PCurrFill = ((float)player.currentHealth / (float)player.stats.Health);

        PHealth.fillAmount = PLostFill;
        PHealthLost.fillAmount = PCurrFill;

        Destroy(HPplayer);

        float pXPos = (290.0f * (1.0f - PLostFill)) - 217.0f + 960f;
        HPplayer = Instantiate(HPIndicator, new Vector3(pXPos, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject plaChild = HPplayer.transform.GetChild(0).gameObject;
        TextMeshProUGUI plaText = plaChild.GetComponent<TextMeshProUGUI>();
        plaText.text = $"{expectedPlayerHP}";



        EnemyName.text = enemy.stats.Name;
        EnemyWeapon.text = enemy.primaryWeapon.WeaponName;
        
        if (numEHits > 1) {
            EnemyDamage.text = $"{EDamage} x {numEHits}";
            EnemyHit.text = $"{enemyHit}%";
            EnemyCrit.text = $"{enemyCrit}%";
        } else if (numEHits == 0) {
            EnemyDamage.text = "-";
            EnemyHit.text = "-";
            EnemyCrit.text = "-";
        } else {
            EnemyDamage.text = $"{EDamage}";
            EnemyHit.text = $"{enemyHit}%";
            EnemyCrit.text = $"{enemyCrit}%";
        }

        EnemyCurrHealth.text = $"{enemy.currentHealth}";

        float ELostFill = ((float)expectedEnemyHP / (float)enemy.stats.Health);
        float ECurrFill = ((float)enemy.currentHealth / (float)enemy.stats.Health);

        Destroy(HPenemy);

        float eXPos = (291.0f * (float)ELostFill) + 140.0f + 960f;
        HPenemy = Instantiate(HPIndicator, new Vector3(eXPos, 540f, 0f), Quaternion.identity, Menu.transform);

        GameObject eneChild = HPenemy.transform.GetChild(0).gameObject;
        TextMeshProUGUI eneText = eneChild.GetComponent<TextMeshProUGUI>();
        eneText.text = $"{expectedEnemyHP}";

        EHealth.fillAmount = ELostFill;
        EHealthLost.fillAmount = ECurrFill;

        Menu.SetActive(true);
    }

    public void DeactivateExpectedMenu() {
        Menu.SetActive(false);
    }


}















 /*public IEnumerator CycleAttackList() {
        bool isAttacking = false;
        int currentIndex = 0;
        AttackingUnit = moveGrid.playerCollide.GetPlayer();
        attackerX = moveGrid.GetCurX();
        attackerZ = moveGrid.GetCurZ();

        while(true) {

            Vector3 currentPosition = moveGrid.moveCursor.transform.position;
            moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            if (Input.GetKeyDown(KeyCode.Space)) {
                DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
                
                isAttacking = true;
                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                Debug.Log("Hello");
                //Go to another IEnumerator to show attacking stats
                break;
            }

            if (Input.GetKeyDown(KeyCode.B)) {
                Debug.Log("Hello");
                break;
            }

            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= .15) {
                float rawHorizontalInput = Input.GetAxis("Horizontal");

                // Determine the sign of the input
                float horizontalSign = Mathf.Sign(rawHorizontalInput);

                // Round down to -1 if negative, round up to 1 if positive
                int horizontalInput = (int)Mathf.Ceil(horizontalSign);

                // Move through the list based on the horizontal input
                if (horizontalInput > 0)
                {
                    // Move up in the list
                    currentIndex++;
                
                    if (currentIndex >= UnitsInRange.Count)
                    {
                        currentIndex = 0; // Wrap around to the start
                        
                    }
                    
                }
                else if (horizontalInput < 0)
                {
                    // Move down in the list
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = UnitsInRange.Count - 1; // Wrap around to the end
                    }
                    
                
                }

                Debug.Log("Index Changed");

                currentPosition = moveGrid.moveCursor.transform.position;
                moveGrid.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());


                yield return new WaitForSeconds(0.5f);
            }
            
           
            // Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            // moveCursor.transform.position = Vector3.MoveTowards(moveCursor.transform.position, targetPosition, 30.0f * Time.deltaTime);

            // Move the cursor towards the target position using interpolation
            // moveGrid.moveCursor.position = Vector3.Lerp(moveGrid.moveCursor.position, targetPosition, 20.0f * Time.deltaTime);
            
            // currentPosition = moveGrid.moveCursor.transform.position;
            // moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            

            yield return null;
        }

        Debug.Log("Broke Free");

        if (isAttacking) {
            //Start Attacking based on primary weapons
            Debug.Log(DefendingEnemy.primaryWeapon.WeaponName);
            Debug.Log(AttackingUnit.primaryWeapon.WeaponName);
            AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, attackerX, attackerZ, defenderX, defenderZ);
            Debug.Log(AttackingUnit.stats.UnitName);
            moveGrid.moveCursor.position = new Vector3(generateGrid.GetGridTile(attackerX, attackerZ).GetXPos(), generateGrid.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, generateGrid.GetGridTile(attackerX, attackerZ).GetZPos());
            manageTurn.RemovePlayer(AttackingUnit.stats);
            moveGrid.ResetAfterAction(AttackingUnit);
            manageTurn.CheckPhase();
            _currentMap.CheckClearCondition();
        }

        moveGrid.isAttacking = false;

        yield return null;
    }*/


    // public void unitAttack(Queue<UnitManager> attacking, Queue<UnitManager> defending) {
    //     int queueSize = attacking.Count;
    //     for (int i = 0; i < queueSize; i++) {
    //         UnitManager atk = attacking.Dequeue();
    //         UnitManager def = defending.Dequeue();

    //         int damage = atk.stats.Attack + atk.primaryWeapon.Attack - def.stats.Defense;

    //         float multiplier = 1;

    //         if (def.stats.Mounted) {
    //             multiplier += atk.primaryWeapon.MultMounted - 1; 
    //         }
    //         if (def.stats.AirBorn) {
    //             multiplier += atk.primaryWeapon.MultAirBorn - 1; 
    //         }
    //         if (def.stats.Armored) {
    //             multiplier += atk.primaryWeapon.MultArmored - 1; 
    //         }
    //         if (def.stats.Whisper) {
    //             multiplier += atk.primaryWeapon.MultWhisper - 1; 
    //         }

    //         Debug.Log("defender current health " + def.currentHealth + " " + def.stats.Health);

    //         damage = (int)(damage * multiplier);

    //         Debug.Log(atk.stats.UnitName + " Did" + damage + " damage to " + def.stats.UnitName);

    //         def.currentHealth -= damage;

    //         Debug.Log("defender current health " + def.currentHealth + " " + def.stats.Health);

    //         if (def.currentHealth <= 0) {
    //             Debug.Log(def.stats.UnitName + "Has died");
    //             break;
    //         }
    //     }

    // }