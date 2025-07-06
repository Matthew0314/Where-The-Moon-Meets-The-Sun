using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ExecuteAction : MonoBehaviour
{
    
    [SerializeField] PlayerGridMovement playerGridMovement;
    [SerializeField] CombatMenuManager combatMenuManager;
    [SerializeField] FindPath findPath;
    [SerializeField] GenerateGrid generateGrid;
    [SerializeField] MapManager _currentMap;
    [SerializeField] TurnManager turnManager;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] CinemachineBrain brain;
    private bool skipCutscene = false;
    private bool[,] attackGrid;
    private GameObject playerCurs;
    [SerializeField]  CinemachineVirtualCamera mainCam;
    [SerializeField] CinemachineVirtualCamera combatCam;
    Gamepad gamepad;
    void Start() {
        playerCurs = GameObject.Find("Player");
        _currentMap = GameObject.Find("GridManager").GetComponent<MapManager>();
    }

    void Update()
    {
        gamepad = Gamepad.current;
    }
    
    

    IEnumerator CheckForSkip()
    {
        while (!skipCutscene)
        {
            if (playerInput.actions["SkipCutscene"].WasPressedThisFrame()) // Input Manager should have "Skip" defined
            {
                skipCutscene = true;
            }
            yield return null;
        }
    }

    public void unitWait()
    {
        // Makes sure that the action menu and any path is deactivated
        combatMenuManager.DeactivateActionMenu();
        findPath.DestroyRange();

        // Calls the MoveUnit function
        generateGrid.MoveUnit(generateGrid.GetGridTile(playerGridMovement.getX(), playerGridMovement.getZ()).UnitOnTile, playerGridMovement.GetOrgX(), playerGridMovement.GetOrgZ(), playerGridMovement.GetCurX(), playerGridMovement.GetCurZ());

        // Sets up temporary variables
        UnitManager temp = generateGrid.GetGridTile(playerGridMovement.getX(), playerGridMovement.getZ()).UnitOnTile;
        temp.XPos = playerGridMovement.GetCurX();
        temp.ZPos = playerGridMovement.GetCurZ();

        playerGridMovement.charSelected = false;
        playerGridMovement.inMenu = false;

        // playerGridMovement.GetPlayerCollide().removePlayer();

        // If the enemy range is active it will reprint it incase an enemy unit moves or dies
        if (playerGridMovement.enemyRangeActive)
        {
            findPath.DestroyEnemyRange();
            findPath.EnemyRange();
        }

    }

    // Called after every action to reset
    public void ResetAfterAction(UnitManager playerUn) {
        findPath.DestroyRange();
        findPath.DestroyArea();
        if (playerUn.getCurrentHealth() > 0) {
            generateGrid.MoveUnit(playerUn, playerGridMovement.GetOrgX(), playerGridMovement.GetOrgZ(), playerGridMovement.GetCurX(), playerGridMovement.GetCurZ());
        }
        
        playerGridMovement.isAttacking = false;
        playerGridMovement.charSelected = false;
        playerGridMovement.inMenu = false; 
        // playerGridMovement.GetPlayerCollide().removePlayer();
    }

    public IEnumerator CycleAttackList(List<GridTile> UnitsInRange, Weapon selectedWeapon) {
        // bool playerGridMovement.IsAttacking = false;
        int currentIndex = 0;
        // UnitManager AttackingUnit = playerGridMovement.GetPlayerCollide().GetPlayer();
        UnitManager AttackingUnit = generateGrid.GetGridTile(playerGridMovement.getX(), playerGridMovement.getZ()).UnitOnTile;
        UnitManager DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
        int attackerX = playerGridMovement.GetCurX();
        int attackerZ = playerGridMovement.GetCurZ();
        int defenderX = UnitsInRange[currentIndex].GetGridX();
        int defenderZ = UnitsInRange[currentIndex].GetGridZ();
        bool weaponChange = false;

        int weaponIndex = 0;
        Weapon orgPrimWeapon = AttackingUnit.GetPrimaryWeapon();
        List<Weapon> playerWeapons = AttackingUnit.GetWeapons();
        List<GridTile> newEnemies = new List<GridTile>();
        AttackingUnit.SetPrimaryWeapon(selectedWeapon);
    
        

        combatMenuManager.DeactivateHoverMenu();
        CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

        Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
        float fspeed = 40f; // Speed of movement
        playerGridMovement.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

        // Move the enemy towards the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            float step = fspeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            yield return null;
        }

        transform.position = targetPosition; 

        while(true) {

            Vector3 currentPosition = playerGridMovement.moveCursor.transform.position;
            playerGridMovement.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            
            if (playerInput.actions["RightSelect"].WasPressedThisFrame() && playerWeapons.Count > 1) {
                newEnemies = new List<GridTile>();
                weaponIndex++;
                
                if (weaponIndex >= playerWeapons.Count)
                {
                    weaponIndex = 0;
                    
                }

                weaponChange = true;

            }

            if (playerInput.actions["LeftSelect"].WasPressedThisFrame() && playerWeapons.Count > 1) {
                newEnemies = new List<GridTile>();
                weaponIndex--;
                
                if (weaponIndex < 0)
                {
                    weaponIndex = playerWeapons.Count - 1;
                    
                }

                weaponChange = true;

            }

            if (weaponChange) {


                bool [,] tempAttack = findPath.CalculateAttack(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ(), playerWeapons[weaponIndex].Range, playerWeapons[weaponIndex].Range1, playerWeapons[weaponIndex].Range2, playerWeapons[weaponIndex].Range3);
                for (int i = 0; i < generateGrid.GetWidth(); i++) {
                    for (int j = 0; j < generateGrid.GetLength(); j++) {
                        if (tempAttack[i,j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Enemy")) {
                            
                            newEnemies.Add(generateGrid.GetGridTile(i,j));
                            
                        }
                    }
                }

                if (newEnemies.Count == 0) {
                    weaponChange = false;
                    continue;
                } else {
                    findPath.DestroyRange();
                    AttackingUnit.SetPrimaryWeapon(playerWeapons[weaponIndex]);
                    UnitsInRange = newEnemies;
                    attackGrid = tempAttack;
                    findPath.HighlightAttack(attackGrid);
                    currentIndex = 0;
                    defenderX = UnitsInRange[currentIndex].GetGridX();
                    defenderZ = UnitsInRange[currentIndex].GetGridZ();
                    CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);
                    weaponChange = false;
                    
                    
                    
                }

                
            }
            if (playerInput.actions["Select"].WasPressedThisFrame() ) {
                DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
                
                playerGridMovement.IsAttacking = true;
                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                // Debug.Log("Hello");
                //Go to another IEnumerator to show attacking stats
                break;
            }

            if (playerInput.actions["Back"].WasPressedThisFrame()) {
                playerGridMovement.IsAttacking = false;
                combatMenuManager.DeactivateExpectedMenu();
                playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetXPos(), generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetYPos(), generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetZPos());
                AttackingUnit.SetPrimaryWeapon(orgPrimWeapon);
                StartCoroutine(combatMenuManager.WeaponList(generateGrid.GetGridTile(playerGridMovement.getX(), playerGridMovement.getZ()).UnitOnTile));

                
                
                playerGridMovement.inMenu = true;
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

                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

                
                targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
                
                playerGridMovement.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

                // // Move the enemy towards the target position
                while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
                {
                    // Calculate the step based on speed and deltaTime
                    float step = fspeed * Time.deltaTime;

                    // Move the enemy towards the target position gradually
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                    yield return null;
                }

                transform.position = targetPosition; 
                // currentPosition = playerGridMovement.moveCursor.transform.position;
                // playerGridMovement.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

                


                yield return new WaitForSeconds(0.25f);
            }

           
            
           
            // Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            // playerGridMovement.moveCursor.transform.position = Vector3.MoveTowards(playerGridMovement.moveCursor.transform.position, targetPosition, 30.0f * Time.deltaTime);

            // Move the cursor towards the target position using interpolation
            // moveGrid.playerGridMovement.moveCursor.position = Vector3.Lerp(moveGrid.playerGridMovement.moveCursor.position, targetPosition, 20.0f * Time.deltaTime);
            
            // currentPosition = moveGrid.moveCursor.transform.position;
            // moveGrid.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
            

            yield return null;
        }

        // Debug.Log("Broke Free");



        if (playerGridMovement.IsAttacking) {
            //Start Attacking based on primary weapons
            yield return StartCoroutine(ExecuteAttack(AttackingUnit, DefendingEnemy));

            // Debug.Log(DefendingEnemy.primaryWeapon.WeaponName);
            // Debug.Log(AttackingUnit.primaryWeapon.WeaponName);
            combatMenuManager.DeactivateExpectedMenu();
            // AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            // AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            // Debug.Log(AttackingUnit.stats.UnitName);
            playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(attackerX, attackerZ).GetXPos(), generateGrid.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, generateGrid.GetGridTile(attackerX, attackerZ).GetZPos());
            UnitManager temp = generateGrid.GetGridTile(playerGridMovement.getX(), playerGridMovement.getZ()).UnitOnTile;
            temp.XPos = playerGridMovement.GetCurX();
            temp.ZPos = playerGridMovement.GetCurZ();
            turnManager.RemovePlayer(AttackingUnit.stats);
            ResetAfterAction(AttackingUnit);
            yield return StartCoroutine(_currentMap.CheckClearCondition());
            yield return StartCoroutine(_currentMap.CheckDefeatCondition());
            turnManager.CheckPhase();
            
            

        }
        // Debug.Log(generateGrid.GetGridTile(5, 4).UnitOnTile.stats.UnitName);

        playerGridMovement.IsAttacking = false;

        yield return null;
    }


    void CalculateExpectedAttack(UnitManager player, UnitManager enemy, int attackerX, int attackerZ, int defenderX, int defenderZ) {
        // bool extraHealth = false;
        player.GetPrimaryWeapon().InitiateQueues(player, enemy, attackerX, attackerZ, defenderX, defenderZ);
        Queue<UnitManager> AttackingQueue = player.GetPrimaryWeapon().AttackingQueue;
        Queue<UnitManager> DefendingQueue = player.GetPrimaryWeapon().DefendingQueue;

        int coun = AttackingQueue.Count;
        
        int AttackerExpectHealth = player.getCurrentHealth();
      
        int DefendExpectHealth = enemy.getCurrentHealth();

        int PDamage = 0;
        int EDamage = 0;

        int numPHits = 0;
        int numEHits = 0;

        for (int i = 0; i < coun; i++) {
            UnitManager atk = AttackingQueue.Dequeue();
            UnitManager def = DefendingQueue.Dequeue();

            int damage = 0;
            if (atk.GetPrimaryWeapon().UseMagic) {
                
                damage = atk.GetAttack() - def.GetResistance();
                Debug.LogError("USING MAGIC AHHHHHHHH  " + damage);

            } else {
                damage = atk.GetAttack() - def.GetDefense();

            }

            float multiplier = 1;

            if (def.stats.Mounted) {
                multiplier += atk.GetPrimaryWeapon().MultMounted - 1; 
            }
            if (def.stats.AirBorn) {
                multiplier += atk.GetPrimaryWeapon().MultAirBorn - 1; 
            }
            if (def.stats.Armored) {
                multiplier += atk.GetPrimaryWeapon().MultArmored - 1; 
            }
            if (def.stats.Whisper) {
                multiplier += atk.GetPrimaryWeapon().MultWhisper - 1; 
            }


            damage = (int)(damage * multiplier);

            if (damage < 0) {damage = 0;}

            if (atk.stats.UnitType == "Player") {
                DefendExpectHealth -= damage;
                PDamage = damage;
                numPHits++;
            } else {
                AttackerExpectHealth -= damage;
                EDamage = damage;
                numEHits++;
            }

        
            
        }

        combatMenuManager.SetUpExpectedMenu(player, enemy, AttackerExpectHealth, DefendExpectHealth, PDamage, EDamage, numPHits, numEHits);

        
    }



    public IEnumerator ExecuteAttack(UnitManager attackingUnit, UnitManager defendingUnit) {
        // Used to check if unit lost an extra healthbar and needs to revived
        bool extraHealthBar = false;
        UnitManager extraHltUnit = null;

        // Deactivates the hover menu
        combatMenuManager.DeactivateHoverMenu();

        // Gets attacking and defending object and rotation so it can make them face each other
        GameObject atkObj = attackingUnit.gameObject;
        GameObject defObj = defendingUnit.gameObject;
        Quaternion originalAtkRotation = atkObj.transform.rotation;
        Quaternion originalDefRotation = defObj.transform.rotation;
        atkObj.transform.LookAt(defObj.transform);
        defObj.transform.LookAt(atkObj.transform);

        // Deactivates the Expected Menu
        combatMenuManager.DeactivateExpectedMenu();

        // Get the unit circle of the attacker and defender so it can deactivate them
        GameObject atkCircle = attackingUnit.unitCircle;
        GameObject defCircle = defendingUnit.unitCircle;
        atkCircle.SetActive(false);
        defCircle.SetActive(false);

        // Destroys the Range and area
        findPath.DestroyRange();
        findPath.DestroyArea();  
        
        // Disables the cursor
        playerCurs.gameObject.GetComponent<MeshRenderer>().enabled = false;

        // Initlializes the queues
        if (attackingUnit.UnitType == "Player") {
            attackingUnit.GetPrimaryWeapon().InitiateQueues(attackingUnit, defendingUnit, playerGridMovement.GetCurX(), playerGridMovement.GetCurZ(), defendingUnit.XPos, defendingUnit.ZPos);
        } else {
            attackingUnit.GetPrimaryWeapon().InitiateQueues(attackingUnit, defendingUnit, attackingUnit.XPos, attackingUnit.ZPos, defendingUnit.XPos, defendingUnit.ZPos);
        }

        // Gets the queues
        Queue<UnitManager> AttackingQueue = attackingUnit.GetPrimaryWeapon().AttackingQueue;
        Queue<UnitManager> DefendingQueue = attackingUnit.GetPrimaryWeapon().DefendingQueue;

        // Gets the amount of times the left and right until will attack
        int leftCou = 0;
        int rightCou = 0;

        foreach (UnitManager unit in AttackingQueue) {
            if (unit == attackingUnit) { leftCou++; }
            else { rightCou++; }
        }

        // Gets Weapons for the Battle Menu
        Weapon leftWeap, rightWeap;
        leftWeap = attackingUnit.GetPrimaryWeapon(); 
        rightWeap = defendingUnit.GetPrimaryWeapon(); 

        // Used later for experience
        UnitManager playerUnit = null;
        EnemyUnit enemyUnit = null;

        if (attackingUnit.stats.UnitType == "Player") {
            playerUnit = attackingUnit;
            enemyUnit = (EnemyUnit)defendingUnit;
        }

        if (defendingUnit.stats.UnitType == "Player") {
            playerUnit = defendingUnit;
            enemyUnit = (EnemyUnit)attackingUnit;
        }
        
        // Calculates Expected Attack for both defender and attacker that will be used in the battle menu
        int defAttack = 0;
        int atkAttack = 0;
        if (attackingUnit.GetPrimaryWeapon().UseMagic) {
            atkAttack = attackingUnit.GetAttack() - defendingUnit.GetResistance();
        } else {
            atkAttack = attackingUnit.GetAttack() - defendingUnit.GetDefense();
        }

        
        if (defendingUnit.GetPrimaryWeapon() != null)
        {
            if (defendingUnit.GetPrimaryWeapon().UseMagic) {
                defAttack = defendingUnit.GetAttack() - attackingUnit.GetResistance();
            } else {
                defAttack = defendingUnit.GetAttack() - attackingUnit.GetDefense();
            }
        }
            
        if (atkAttack < 0) atkAttack = 0;
        if(defAttack < 0) defAttack = 0;

        // Opens battle menu and switches to combat cam with a 3f transition
        yield return StartCoroutine(combatMenuManager.BattleMenu(attackingUnit, defendingUnit, attackingUnit.getCurrentHealth(), defendingUnit.getCurrentHealth(), atkAttack, defAttack, leftCou, rightCou, leftWeap, rightWeap));
        // SwitchToCombatCamera(attackingUnit.gameObject.transform, defendingUnit.gameObject.transform);
        StartCoroutine(CheckForSkip());
        yield return StartCoroutine(SwitchToCombatCamera(attackingUnit.transform, defendingUnit.transform));

        // if (!skipCutscene) yield return new WaitForSeconds(3f);

        // Get the Count of the queue at its original state
        int coun = AttackingQueue.Count;

        for (int i = 0; i < coun; i++) {
            // Dequeue both the attacker and defender
            UnitManager atk = AttackingQueue.Dequeue();
            UnitManager def = DefendingQueue.Dequeue();

            // Calculates the damage
            int damage = atk.GetPrimaryWeapon().UnitAttack(atk, def, false);
            if(damage < 0) { damage = 0; }

            // Damages the defender
            def.TakeDamage(damage);

            // Decrements the weapons uses
            atk.GetPrimaryWeapon().DecrementUses();

            // If uses is 0 or less the item has been broken
            if(atk.GetPrimaryWeapon().Uses <= 0) {
                // Removes the unit that lost their weapon from the attacking queue and the other from the defending queu
                Queue<UnitManager> newAttackingQueue = new Queue<UnitManager>();
                while (AttackingQueue.Count > 0) {
                    UnitManager unit = AttackingQueue.Dequeue();
                    if (unit != atk) {
                        newAttackingQueue.Enqueue(unit);
                    }
                }
                AttackingQueue.Clear();
                AttackingQueue = newAttackingQueue;

                Queue<UnitManager> newDefendingQueue = new Queue<UnitManager>();
                while (DefendingQueue.Count > 0) {
                    UnitManager unit = DefendingQueue.Dequeue();
                    if (unit != def) {
                        newDefendingQueue.Enqueue(unit);
                    }
                }

                DefendingQueue.Clear();
                DefendingQueue = newDefendingQueue;

                // Makes the primary weapon the next one in the list, if none then set it to null
                // atk.stats.weapons.Remove(atk.primaryWeapon);
                // if (atk.stats.weapons.Count >= 1) {
                //     atk.primaryWeapon = atk.stats.weapons[0];
                // } else {
                //     atk.primaryWeapon = null;
                // }
            }


            // Sets up combat battle menu with updated health
            if (!skipCutscene) yield return StartCoroutine(combatMenuManager.BattleMenu(attackingUnit, defendingUnit, attackingUnit.getCurrentHealth(), defendingUnit.getCurrentHealth(), atkAttack, defAttack, leftCou, rightCou, leftWeap, rightWeap));
            if (!skipCutscene) yield return new WaitForSeconds(1f);

            


                
            
            // If defender dies
            if (def.getCurrentHealth() <= 0) {
                // checked if more than 1 health bar, else remove the dead unit 
                if (def.stats.HealthBars > 1) {
                    extraHealthBar = true;
                    extraHltUnit = def;
                } else {
                    _currentMap.RemoveDeadUnit(def, def.XPos, def.ZPos);
                }
                break;
            }

            // If the queue is finished then break
            if(AttackingQueue.Count == 0) {
                break;
            }
            
            if (!skipCutscene) yield return new WaitForSeconds(1f);
        }
        if (skipCutscene)
        {
            yield return StartCoroutine(combatMenuManager.BattleMenu(attackingUnit, defendingUnit, attackingUnit.getCurrentHealth(), defendingUnit.getCurrentHealth(), atkAttack, defAttack, leftCou, rightCou, leftWeap, rightWeap));
            yield return new WaitForSeconds(1f);
        }

        // Deactivate Expected Battle Menu
            combatMenuManager.DeactivateExpectedMenu();

        Quaternion targetRotation;

        
        

        if (attackingUnit.UnitType == "Player" && playerUnit.getCurrentHealth() > 0) {
            // Transforms the camera to face the player, but have them slightly to the right
            Vector3 forward = atkObj.transform.forward; 
            Vector3 right = atkObj.transform.right;     
            Vector3 offsetPosition = atkObj.transform.position - forward * -10f + right * 2f;

            // Offset y value
            offsetPosition.y = 4f;
            combatCam.transform.position = offsetPosition;

            // Calculate the target rotation by adding 180 degrees to the object's Y rotation
            Vector3 currentRotation = atkObj.transform.eulerAngles;
            targetRotation = Quaternion.Euler(0f, currentRotation.y + 180f, 0f);

            // Apply the calculated rotation to the camera
            combatCam.transform.rotation = targetRotation;

            // Set LookAt target to maintain focus on the object
            combatCam.LookAt = atkObj.transform;
        }

        
        // If the playerunit is still alive calculate and send over the amount of EXP
        if (playerUnit != null && playerUnit.getCurrentHealth() > 0) {
            int expObtained = 0;

            // Calculate EXP with a base of 30 with +5 for every level the player is below, or -5 for every level the player is above
            expObtained = 30 + ((enemyUnit.stats.Level - playerUnit.stats.Level) * 5);

            EnemyStats eneStats = (EnemyStats)enemyUnit.stats;

            // If the Enemy is a boss, multiply the EXP by 1.5
            if (eneStats.IsBoss) { expObtained = (int)((double)expObtained * 1.5); }

            // If the Enemy is still alive, divide by 2
            if (enemyUnit.getCurrentHealth() > 0) { expObtained /= 2; }

            // If the player is not on Eclipse difficulty, then multiply it by 2
            if (_currentMap.GetDifficulty() != "Eclipse") { expObtained *= 2; }

            // If the player recieved 0 or less than EXP, give them 1
            if (expObtained <= 0) { expObtained = 1; }

            // Send over the EXP
            yield return StartCoroutine(playerUnit.ExperienceGain(expObtained)); 
        }

        
        yield return new WaitForSeconds(1f);

        // Set the circle to active again, and the player to its original rotationb
        if (atkObj != null) {
            atkCircle.SetActive(true);
            atkObj.transform.rotation = originalAtkRotation;
        }

        // If enemy still alive, set circle to active and roation to original
        if (defObj != null) {
            defCircle.SetActive(true);
            defObj.transform.rotation = originalDefRotation;
        }

        // Activate the cursor again
        playerCurs.gameObject.GetComponent<MeshRenderer>().enabled = true;
        
        StopCoroutine(CheckForSkip());
        skipCutscene = false;
        // Switch to main camera again
        SwitchToMainCamera();

        
        yield return new WaitForSeconds(3f);

       
        // Apply the calculated rotation to the camera 
        targetRotation = Quaternion.Euler(22f, 0f, 0f);
        combatCam.transform.rotation = targetRotation;

        // If the enemy had an extra helath bar, reset their health
        if (extraHealthBar) {
            yield return StartCoroutine(extraHltUnit.ExtraHealthBar());
        }


        yield return null;
    }



    // public void SwitchToCombatCamera(Transform attacker, Transform defender)
    // {
    //     // Disable Cinemachine Brain temporarily to prevent override
    //     CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
    //     brain.enabled = false;

    //     // Temporarily remove LookAt target to prevent Cinemachine from overriding rotation
    //     Transform previousLookAt = combatCam.LookAt;
    //     combatCam.LookAt = null;

    //     // Ensure that the attacker is on the left side and the defender is on the right side
    //     Transform leftCharacter = attacker.position.x < defender.position.x ? attacker : defender;
    //     Transform rightCharacter = leftCharacter == attacker ? defender : attacker;

    //     // Get the midpoint between the two characters
    //     Transform midpoint = GetMidpoint(leftCharacter, rightCharacter);
    //     // combatCam.LookAt = midpoint;
    //     // combatCam.Follow = midpoint;

    //     // Calculate the direction from left character to right character
    //     Vector3 directionToFace = rightCharacter.position - leftCharacter.position;

    //     // Calculate the angle between the two characters (in 2D plane, using X and Z axis)
    //     float angle = Mathf.Atan2(directionToFace.z, directionToFace.x) * Mathf.Rad2Deg;

    //     // Normalize the angle to the range [0, 360] degrees


    //     Debug.Log("Angle " + angle);

    //     if (angle == 0f || Mathf.Abs(angle) == 180f || Mathf.Abs(angle) == 270f || Mathf.Abs(angle) == 90) {
    //         if (angle == 0f) {
    //             if (rightCharacter == attacker) {
    //                 angle = 180;
    //             }
    //         }
    //         angle = (angle + 360f) % 360f;
    //     } else if (angle > 0) {
    //         if (leftCharacter == attacker) {
    //             angle = (angle - 90f + 360f) % 360f;
    //         } else {
    //             angle = (angle + 90f + 360f) % 360f;
    //         }

    //     } else {
    //     if (rightCharacter == attacker) {
    //             angle = (angle - 90f + 360f) % 360f;
    //         } else {
    //             angle = (angle + 90f + 360f) % 360f;
    //         }
    //     }



    //     float characterDistance = Vector3.Distance(leftCharacter.position, rightCharacter.position);

    //     // Scale the offset distance based on character distance
    //     // For example, offset is proportional to distance (adjust multiplier as needed)
    //     float baseOffsetDistance = 7f; // Default offset distance
    //     float offsetDistance = baseOffsetDistance + (characterDistance * 0.5f);
    //     float angleInRadians = angle * Mathf.Deg2Rad;
    //     // float xOffset = 10f;

    //     // Calculate the X and Z offsets based on the angle
    //     float xOffset = Mathf.Sin(angleInRadians) * offsetDistance;
    //     float zOffset = Mathf.Cos(angleInRadians) * offsetDistance;

    //     // float totalDistance = Mathf.Sqrt(xOffset * xOffset + zOffset * zOffset); // Calculate diagonal distance
    //     // xOffset = (xOffset / totalDistance) * offsetDistance; // Scale the xOffset
    //     // zOffset = (zOffset / totalDistance) * offsetDistance; // Scale the zOffset


    //     // Debug.Log("Angle " + angle + " xOffset " + xOffset + " zOffset ");

    //     // Set the camera's position 10 units away from the midpoint, adjusting X and Z based on angle
    //     Vector3 cameraPosition = new Vector3(midpoint.position.x - xOffset, midpoint.position.y, midpoint.position.z - zOffset); // 5f is the y-offset

    //     Debug.Log("Angle " + angle + " xOffset " + xOffset +  " zOffset " + zOffset + " midpoint " + midpoint.transform.position + " Camera poisiton " + cameraPosition);

    //     // Move the camera to the new position
    //     combatCam.transform.position = cameraPosition;

    //     // Optionally, use the angle to set the rotation (only affecting Y-axis)
    //     Vector3 newRotation = combatCam.transform.eulerAngles;
    //     newRotation.y = angle;
    //     combatCam.transform.eulerAngles = newRotation;

    //     // Adjust the camera's field of view based on the distance between characters
    //     float distance = Vector3.Distance(leftCharacter.position, rightCharacter.position);
    //     combatCam.m_Lens.FieldOfView = Mathf.Clamp(distance * 2, 40f, 60f);

    //     combatCam.Priority = 1000; // Activate combat camera (higher priority)

    //     // // Quaternion targetRotation = Quaternion.Euler(22f, 0f, 0f);

    //     // // Apply the calculated rotation to the camera
    //     // combatCam.transform.rotation = targetRotation;

    //     // Re-enable CinemachineBrain
    //     brain.enabled = true;
    // }

    public IEnumerator SwitchToCombatCamera(Transform attacker, Transform defender)
    {
        // Disable Cinemachine Brain temporarily to prevent override
        // CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        brain.enabled = false;

        // Temporarily remove LookAt target to prevent Cinemachine from overriding rotation
        Transform previousLookAt = combatCam.LookAt;
        combatCam.LookAt = null;

        // Ensure that the attacker is on the left side and the defender is on the right side
        Transform leftCharacter = attacker.position.x < defender.position.x ? attacker : defender;
        Transform rightCharacter = leftCharacter == attacker ? defender : attacker;

        // Get the midpoint between the two characters
        Transform midpoint = GetMidpoint(leftCharacter, rightCharacter);
        // combatCam.LookAt = midpoint;
        // combatCam.Follow = midpoint;

        // Calculate the direction from left character to right character
        Vector3 directionToFace = rightCharacter.position - leftCharacter.position;

        // Calculate the angle between the two characters (in 2D plane, using X and Z axis)
        float angle = Mathf.Atan2(directionToFace.z, directionToFace.x) * Mathf.Rad2Deg;




        Debug.Log("Angle " + angle);

        if (angle == 0f || Mathf.Abs(angle) == 180f || Mathf.Abs(angle) == 270f || Mathf.Abs(angle) == 90)
        {
            if (angle == 0f)
            {
                if (rightCharacter == attacker)
                {
                    angle = 180;
                }
            }
            angle = (angle + 360f) % 360f;
        }
        else if (angle > 0)
        {
            if (leftCharacter == attacker)
            {
                angle = (angle - 90f + 360f) % 360f;
            }
            else
            {
                angle = (angle + 90f + 360f) % 360f;
            }

        }
        else
        {
            if (rightCharacter == attacker)
            {
                angle = (angle - 90f + 360f) % 360f;
            }
            else
            {
                angle = (angle + 90f + 360f) % 360f;
            }
        }



        float characterDistance = Vector3.Distance(leftCharacter.position, rightCharacter.position);

        // Scale the offset distance based on character distance
        // For example, offset is proportional to distance (adjust multiplier as needed)
        float baseOffsetDistance = 7f; // Default offset distance
        float offsetDistance = baseOffsetDistance + (characterDistance * 0.5f);
        float angleInRadians = angle * Mathf.Deg2Rad;
        // float xOffset = 10f;

        // Calculate the X and Z offsets based on the angle
        float xOffset = Mathf.Sin(angleInRadians) * offsetDistance;
        float zOffset = Mathf.Cos(angleInRadians) * offsetDistance;

        // float totalDistance = Mathf.Sqrt(xOffset * xOffset + zOffset * zOffset); // Calculate diagonal distance
        // xOffset = (xOffset / totalDistance) * offsetDistance; // Scale the xOffset
        // zOffset = (zOffset / totalDistance) * offsetDistance; // Scale the zOffset


        // Debug.Log("Angle " + angle + " xOffset " + xOffset + " zOffset ");

        // Set the camera's position 10 units away from the midpoint, adjusting X and Z based on angle
        Vector3 cameraPosition = new Vector3(midpoint.position.x - xOffset, midpoint.position.y, midpoint.position.z - zOffset); // 5f is the y-offset

        Debug.Log("Angle " + angle + " xOffset " + xOffset + " zOffset " + zOffset + " midpoint " + midpoint.transform.position + " Camera poisiton " + cameraPosition);

        // Move the camera to the new position
        combatCam.transform.position = cameraPosition;

        // Optionally, use the angle to set the rotation (only affecting Y-axis)
        Vector3 newRotation = combatCam.transform.eulerAngles;
        newRotation.y = angle;
        combatCam.transform.eulerAngles = newRotation;

        // Adjust the camera's field of view based on the distance between characters
        float distance = Vector3.Distance(leftCharacter.position, rightCharacter.position);
        combatCam.m_Lens.FieldOfView = Mathf.Clamp(distance * 2, 40f, 60f);

        combatCam.Priority = 1000; // Activate combat camera (higher priority)

        float timer = 0f;
        float maxBlendDuration = 3f;
        var originalBlend = brain.m_DefaultBlend;
        brain.enabled = true;


        while (timer < maxBlendDuration)
        {
            if (skipCutscene)
            {
                // Skip pressed — cut the blend immediately
                brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
                brain.enabled = false;

                // Force a "refresh" to apply the cut immediately
                combatCam.Priority = -1001;
                yield return null; // Wait a frame
                combatCam.Priority = 1000;
                brain.enabled = true;


                // brain.ForceCameraPosition(combatCam.transform.position, combatCam.transform.rotation);

                // yield return new WaitForSeconds(3f);

                // Restore the original blend
                brain.m_DefaultBlend = originalBlend;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // // Quaternion targetRotation = Quaternion.Euler(22f, 0f, 0f);

        // // Apply the calculated rotation to the camera
        // combatCam.transform.rotation = targetRotation;

        // Re-enable CinemachineBrain
        yield return null;
    }

    

    private Transform GetMidpoint(Transform left, Transform right)
    {
        Vector3 midpoint = (left.position + right.position) / 2;
        midpoint.y += 5f;
        GameObject midpointObject = new GameObject("Midpoint");
        midpointObject.transform.position = midpoint;
        return midpointObject.transform;
    }

    public void SwitchToMainCamera()
    {
        combatCam.Priority = 0; 
    }





    public IEnumerator CycleAssist(List<GridTile> UnitsInRange, Faith faithInUse) {

        int currentIndex = 0;
        UnitManager AttackingUnit = generateGrid.GetGridTile(playerGridMovement.getX(), playerGridMovement.getZ()).UnitOnTile;
        UnitManager DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
        int attackerX = playerGridMovement.GetCurX();
        int attackerZ = playerGridMovement.GetCurZ();
        // int defenderX = UnitsInRange[currentIndex].GetGridX();
        // int defenderZ = UnitsInRange[currentIndex].GetGridZ();
        playerGridMovement.inMenu = true;
        
    
        

        combatMenuManager.DeactivateHoverMenu();
        // CalculateExpectedAttack(AttackingUnit, UnitsInRange[currentIndex].UnitOnTile, attackerX, attackerZ, defenderX, defenderZ);

        Vector3 targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
        float fspeed = 40f; // Speed of movement
        playerGridMovement.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

        // Move the enemy towards the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            float step = fspeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            yield return null;
        }

        transform.position = targetPosition; 

        while(true) {

            Vector3 currentPosition = playerGridMovement.moveCursor.transform.position;
            playerGridMovement.moveCursor.transform.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos()+0.02f, UnitsInRange[currentIndex].GetZPos());

            if ((Input.GetKeyDown(KeyCode.Space) || (gamepad != null &&  gamepad.buttonSouth.wasPressedThisFrame))) {
                DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;     
                playerGridMovement.IsAttacking = true;
                yield return StartCoroutine(faithInUse.Use(AttackingUnit, DefendingEnemy));
                Debug.LogWarning("Howdy");
                break;
            }

            if ((Input.GetKeyDown(KeyCode.B) || (gamepad != null && gamepad.buttonEast.wasPressedThisFrame))) {
                playerGridMovement.IsAttacking = false;
                combatMenuManager.DeactivateExpectedMenu();
                playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetXPos(), generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetYPos(), generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetZPos());
                // AttackingUnit.primaryWeapon = orgPrimWeapon;
                combatMenuManager.PlayerAssist();

                
                
                playerGridMovement.inMenu = true;
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

                // defenderX = UnitsInRange[currentIndex].GetGridX();
                // defenderZ = UnitsInRange[currentIndex].GetGridZ();

                
                targetPosition = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());
                
                playerGridMovement.moveCursor.position = new Vector3(UnitsInRange[currentIndex].GetXPos(), UnitsInRange[currentIndex].GetYPos(), UnitsInRange[currentIndex].GetZPos());

                // // Move the enemy towards the target position
                while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
                {
                    // Calculate the step based on speed and deltaTime
                    float step = fspeed * Time.deltaTime;

                    // Move the enemy towards the target position gradually
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                    yield return null;
                }

                transform.position = targetPosition; 

                


                yield return new WaitForSeconds(0.25f);
            }
            yield return null;
        }
        if (playerGridMovement.IsAttacking) {
            //Start Attacking based on primary weapons
            // yield return StartCoroutine(ExecuteAttack(AttackingUnit, DefendingEnemy));

            
            // combatMenuManager.DeactivateExpectedMenu();
            // Debug.LogWarning("In Thing");
            playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(attackerX, attackerZ).GetXPos(), generateGrid.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, generateGrid.GetGridTile(attackerX, attackerZ).GetZPos());
            UnitManager temp = generateGrid.GetGridTile(playerGridMovement.getX(), playerGridMovement.getZ()).UnitOnTile;
            temp.XPos = playerGridMovement.GetCurX();
            temp.ZPos = playerGridMovement.GetCurZ();
            turnManager.RemovePlayer(AttackingUnit.stats);
            // unitWait();
            ResetAfterAction(AttackingUnit);
            
            turnManager.CheckPhase();
            yield return StartCoroutine(_currentMap.CheckClearCondition());
            yield return StartCoroutine(_currentMap.CheckDefeatCondition());
            
            

        }
        // Debug.Log(generateGrid.GetGridTile(5, 4).UnitOnTile.stats.UnitName);
        playerGridMovement.inMenu = false;
        playerGridMovement.IsAttacking = false;
        yield return null;
    }

}
