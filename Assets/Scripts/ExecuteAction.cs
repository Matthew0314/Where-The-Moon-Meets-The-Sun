using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ExecuteAction : MonoBehaviour
{
    
    private PlayerGridMovement playerGridMovement;
    private CombatMenuManager combatMenuManager;
    private FindPath findPath;
    private GenerateGrid generateGrid;
    private IMaps _currentMap;
    private TurnManager turnManager;  
    public bool[,] attackGrid;
    private GameObject playerCurs;
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera combatCam;
    Gamepad gamepad;
    void Start()
    {
        playerCurs = GameObject.Find("Player");
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        combatMenuManager = GameObject.Find("Canvas").GetComponent<CombatMenuManager>();
        findPath = GameObject.Find("Player").GetComponent<FindPath>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        _currentMap = GameObject.Find("GridManager").GetComponent<IMaps>();
        turnManager = GameObject.Find("GridManager").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        gamepad = Gamepad.current;
    }

    public void unitWait() {
        combatMenuManager.DeactivateActionMenu();
        
        findPath.DestroyRange();
        
        // gridControl.GetGridTile(curX, curZ).UnitOnTile = playerCollide.GetPlayer();
        // gridControl.GetGridTile(orgX, orgZ).UnitOnTile = null;

        generateGrid.MoveUnit(playerGridMovement.GetPlayerCollide().GetPlayer(), playerGridMovement.GetOrgX(), playerGridMovement.GetOrgZ(), playerGridMovement.GetCurX(), playerGridMovement.GetCurZ());
        // Debug.Log("Moved Player from " + orgX + " " + orgZ + " to " + curX + " " + curZ);

        UnitManager temp = playerGridMovement.GetPlayerCollide().GetPlayer();
        temp.XPos = playerGridMovement.GetCurX();
        temp.ZPos = playerGridMovement.GetCurZ();

        // List<UnitManager> tempUnits = _currentMap.GetMapUnits();

        // UnitManager tempUnit = tempUnits.Find()
        

        findPath.DestroyArea();
        playerGridMovement.charSelected = false;
        playerGridMovement.inMenu = false;       


           

        playerGridMovement.GetPlayerCollide().removePlayer();

        if (playerGridMovement.enemyRangeActive) {
            findPath.DestroyEnemyRange();
            findPath.EnemyRange();
        }
            
    }

    public void ResetAfterAction(UnitManager playerUn) {
        findPath.DestroyRange();
        findPath.DestroyArea();
        if (playerUn.getCurrentHealth() > 0) {
            generateGrid.MoveUnit(playerUn, playerGridMovement.GetOrgX(), playerGridMovement.GetOrgZ(), playerGridMovement.GetCurX(), playerGridMovement.GetCurZ());
        }
        
        playerGridMovement.isAttacking = false;
        playerGridMovement.charSelected = false;
        playerGridMovement.inMenu = false; 
        playerGridMovement.GetPlayerCollide().removePlayer();
    }

    public IEnumerator CycleAttackList(List<GridTile> UnitsInRange) {
        // bool playerGridMovement.IsAttacking = false;
        int currentIndex = 0;
        UnitManager AttackingUnit = playerGridMovement.GetPlayerCollide().GetPlayer();
        UnitManager DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
        int attackerX = playerGridMovement.GetCurX();
        int attackerZ = playerGridMovement.GetCurZ();
        int defenderX = UnitsInRange[currentIndex].GetGridX();
        int defenderZ = UnitsInRange[currentIndex].GetGridZ();
        bool weaponChange = false;

        int weaponIndex = 0;
        Weapon orgPrimWeapon = AttackingUnit.primaryWeapon;
        List<Weapon> playerWeapons = AttackingUnit.stats.weapons;
        List<GridTile> newEnemies = new List<GridTile>();
    
        

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

            
            if (Input.GetKeyDown(KeyCode.E) || gamepad.rightShoulder.wasPressedThisFrame && playerWeapons.Count > 1) {
                newEnemies = new List<GridTile>();
                weaponIndex++;
                
                if (weaponIndex >= playerWeapons.Count)
                {
                    weaponIndex = 0;
                    
                }

                weaponChange = true;

            }

            if (Input.GetKeyDown(KeyCode.Q) || gamepad.leftShoulder.wasPressedThisFrame  && playerWeapons.Count > 1) {
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
                    AttackingUnit.primaryWeapon = playerWeapons[weaponIndex];
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
            if (Input.GetKeyDown(KeyCode.Space) || gamepad.buttonSouth.wasPressedThisFrame) {
                DefendingEnemy = UnitsInRange[currentIndex].UnitOnTile;
                
                playerGridMovement.IsAttacking = true;
                defenderX = UnitsInRange[currentIndex].GetGridX();
                defenderZ = UnitsInRange[currentIndex].GetGridZ();
                Debug.Log("Hello");
                //Go to another IEnumerator to show attacking stats
                break;
            }

            if (Input.GetKeyDown(KeyCode.B) || gamepad.buttonEast.wasPressedThisFrame) {
                playerGridMovement.IsAttacking = false;
                combatMenuManager.DeactivateExpectedMenu();
                playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetXPos(), generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetYPos(), generateGrid.GetGridTile(playerGridMovement.GetCurX(), playerGridMovement.GetCurZ()).GetZPos());
                AttackingUnit.primaryWeapon = orgPrimWeapon;
                StartCoroutine(combatMenuManager.ActivateActionMenu());

                
                
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

        Debug.Log("Broke Free");



        if (playerGridMovement.IsAttacking) {
            //Start Attacking based on primary weapons
            yield return StartCoroutine(ExecuteAttack(AttackingUnit, DefendingEnemy));

            Debug.Log(DefendingEnemy.primaryWeapon.WeaponName);
            Debug.Log(AttackingUnit.primaryWeapon.WeaponName);
            combatMenuManager.DeactivateExpectedMenu();
            // AttackingUnit.primaryWeapon.InitiateQueues(AttackingUnit, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            // AttackingUnit.primaryWeapon.unitAttack(AttackingUnit.primaryWeapon.AttackingQueue, AttackingUnit.primaryWeapon.DefendingQueue, DefendingEnemy, attackerX, attackerZ, defenderX, defenderZ);
            Debug.Log(AttackingUnit.stats.UnitName);
            playerGridMovement.moveCursor.position = new Vector3(generateGrid.GetGridTile(attackerX, attackerZ).GetXPos(), generateGrid.GetGridTile(attackerX, attackerZ).GetYPos() + 0.02f, generateGrid.GetGridTile(attackerX, attackerZ).GetZPos());
            UnitManager temp = playerGridMovement.GetPlayerCollide().GetPlayer();
            temp.XPos = playerGridMovement.GetCurX();
            temp.ZPos = playerGridMovement.GetCurZ();
            turnManager.RemovePlayer(AttackingUnit.stats);
            ResetAfterAction(AttackingUnit);
            turnManager.CheckPhase();
            _currentMap.CheckClearCondition();
            _currentMap.CheckDefeatCondition();
            

        }
        // Debug.Log(generateGrid.GetGridTile(5, 4).UnitOnTile.stats.UnitName);

        playerGridMovement.IsAttacking = false;

        yield return null;
    }


    void CalculateExpectedAttack(UnitManager player, UnitManager enemy, int attackerX, int attackerZ, int defenderX, int defenderZ) {
        
        player.primaryWeapon.InitiateQueues(player, enemy, attackerX, attackerZ, defenderX, defenderZ);
        Queue<UnitManager> AttackingQueue = player.primaryWeapon.AttackingQueue;
        Queue<UnitManager> DefendingQueue = player.primaryWeapon.DefendingQueue;

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

            int damage = atk.stats.Attack + atk.primaryWeapon.Attack - def.stats.Defense;

            float multiplier = 1;

            if (def.stats.Mounted) {
                multiplier += atk.primaryWeapon.MultMounted - 1; 
            }
            if (def.stats.AirBorn) {
                multiplier += atk.primaryWeapon.MultAirBorn - 1; 
            }
            if (def.stats.Armored) {
                multiplier += atk.primaryWeapon.MultArmored - 1; 
            }
            if (def.stats.Whisper) {
                multiplier += atk.primaryWeapon.MultWhisper - 1; 
            }


            damage = (int)(damage * multiplier);

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
        combatMenuManager.DeactivateHoverMenu();
        GameObject atkObj = attackingUnit.gameObject;
        GameObject defObj = defendingUnit.gameObject;
        Quaternion originalAtkRotation = atkObj.transform.rotation;
        Quaternion originalDefRotation = defObj.transform.rotation;
        atkObj.transform.LookAt(defObj.transform);
        defObj.transform.LookAt(atkObj.transform);
        bool playerOnLeft;
        combatMenuManager.DeactivateExpectedMenu();
        if (attackingUnit.UnitType == "Player") {
            playerOnLeft = true;
        } else {
            playerOnLeft = false;
        }
        
        // CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        // while (brain.ActiveVirtualCamera != combatCam)
        // {
        //     yield return null; // Wait until next frame
        // }

        GameObject atkCircle = attackingUnit.unitCircle;
        GameObject defCircle = defendingUnit.unitCircle;
        atkCircle.SetActive(false);
        defCircle.SetActive(false);
        findPath.DestroyRange();
        findPath.DestroyArea();

        List<UnitManager> mapUnits = _currentMap.GetMapUnits();
        List<GameObject> unitObj = new List<GameObject>();

        // foreach (UnitManager unit in mapUnits) {
        //     if (unit != attackingUnit && unit != defendingUnit) {
        //         unitObj.Add(unit.gameObject);

        //         unit.gameObject.SetActive(false);
        //     }
            
        // }
        
        
        
        // moveCursor.gameObject.SetActive(false);
        playerCurs.gameObject.GetComponent<MeshRenderer>().enabled = false;
        
        attackingUnit.primaryWeapon.InitiateQueues(attackingUnit, defendingUnit, attackingUnit.XPos, attackingUnit.ZPos, defendingUnit.XPos, defendingUnit.ZPos);
        Queue<UnitManager> AttackingQueue = attackingUnit.primaryWeapon.AttackingQueue;
        Queue<UnitManager> DefendingQueue = attackingUnit.primaryWeapon.DefendingQueue;

        UnitManager playerUnit = null;
        EnemyUnit enemyUnit = null;

        

        Queue<UnitManager> tempQueue = new Queue<UnitManager>(AttackingQueue);

        int playerCount = 0;
        int enemyCount = 0;

        // Process the temporary queue
        while (tempQueue.Count > 0)
        {
            UnitManager ak = tempQueue.Dequeue();

            if (ak.UnitType == "Player")
            {
                playerCount++;
            }
            else if (ak.UnitType == "Enemy")
            {
                enemyCount++;
            }
        }

        int atkCount = 0;
        int defCount = 0;

        

        if (attackingUnit.stats.UnitType == "Player") {
            playerUnit = attackingUnit;
            enemyUnit = (EnemyUnit)defendingUnit;
            atkCount = playerCount;
            defCount = enemyCount;
        }

        if (defendingUnit.stats.UnitType == "Player") {
            playerUnit = defendingUnit;
            enemyUnit = (EnemyUnit)attackingUnit;
            atkCount = enemyCount;
            defCount = playerCount;
        }

        yield return StartCoroutine(combatMenuManager.BattleMenu(attackingUnit, defendingUnit, playerOnLeft, attackingUnit.stats.Health, defendingUnit.stats.Health, 10, 10, atkCount, defCount));
        SwitchToCombatCamera(attackingUnit.gameObject.transform, defendingUnit.gameObject.transform);
        yield return new WaitForSeconds(3f);

        int coun = AttackingQueue.Count;
        // Debug.Log(" COUNT " + coun);

        for (int i = 0; i < coun; i++) {
            UnitManager atk = AttackingQueue.Dequeue();
            UnitManager def = DefendingQueue.Dequeue();
            int damage = atk.primaryWeapon.UnitAttack(atk, def, false);
            def.TakeDamage(damage);
            // Debug.Log(atk.stats.UnitName + " hits " + def.stats.UnitName + " for " +  damage);
            
            // if (atk.UnitType == "Player") {
            //     yield return StartCoroutine(combatMenuManager.BattleMenu(attackingUnit, defendingUnit, playerOnLeft, atk.getCurrentHealth(), def.getCurrentHealth(), 10, 10, 10, 10));
            //     yield return new WaitForSeconds(1f);
            // } else {
            //     yield return StartCoroutine(combatMenuManager.BattleMenu(attackingUnit, defendingUnit, playerOnLeft, def.getCurrentHealth(), atk.getCurrentHealth(), 10, 10, 10, 10));
            //     yield return new WaitForSeconds(1f);
            // }

            yield return StartCoroutine(combatMenuManager.BattleMenu(attackingUnit, defendingUnit, playerOnLeft, attackingUnit.getCurrentHealth(), defendingUnit.getCurrentHealth(), 10, 10, atkCount, defCount));
            yield return new WaitForSeconds(1f);
                
            

            if (def.getCurrentHealth() <= 0) {
                // Debug.Log("Removing " + def.stats.UnitName);
                _currentMap.RemoveDeadUnit(def, def.XPos, def.ZPos);
                // Debug.Log("Removing " + def.stats.UnitName);
                break;
            }
            
            yield return new WaitForSeconds(1f);
        }
        // Debug.Log("End Execute Attack");

        combatMenuManager.DeactivateExpectedMenu();

        Quaternion targetRotation;

        
        

        if (attackingUnit.UnitType == "Player") {
            Vector3 forward = atkObj.transform.forward; // The forward direction of the attacker
            Vector3 right = atkObj.transform.right;     // The right direction of the attacker
            // Offset position: slightly behind and to the right of the attacker
            Vector3 offsetPosition = atkObj.transform.position - forward * -10f + right * 2f;
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

        

        if (playerUnit != null && playerUnit.getCurrentHealth() > 0) {
            int expObtained = 0;

            expObtained = 30 + ((enemyUnit.stats.Level - playerUnit.stats.Level) * 5);

            // if (enemyUnit.stats.IsBoss) {

            // }

            if (enemyUnit.getCurrentHealth() > 0) {
                expObtained /= 2;
            }

            Debug.Log("AHHHHHH GAINED " + expObtained + " EXP");

            yield return StartCoroutine(playerUnit.ExperienceGain(210));
            
        }
        yield return new WaitForSeconds(1f);
        atkCircle.SetActive(true);
        // defCircle.SetActive(true);

        if (defObj != null) {
            defCircle.SetActive(true);
            defObj.transform.rotation = originalDefRotation;
        }
        playerCurs.gameObject.GetComponent<MeshRenderer>().enabled = true;
        atkObj.transform.rotation = originalAtkRotation;
        // foreach (GameObject unit in unitObj) {
        //     unit.SetActive(true);
        // }
        
        // moveCursor.gameObject.SetActive(true);
        SwitchToMainCamera();

        
        yield return new WaitForSeconds(3f);

        targetRotation = Quaternion.Euler(22f, 0f, 0f);

        // Apply the calculated rotation to the camera
        combatCam.transform.rotation = targetRotation;


        yield return null;


    } 



    public void SwitchToCombatCamera(Transform attacker, Transform defender)
    {
        // Disable Cinemachine Brain temporarily to prevent override
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
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

        // Normalize the angle to the range [0, 360] degrees
        

        Debug.Log("Angle " + angle);

        if (angle == 0f || Mathf.Abs(angle) == 180f || Mathf.Abs(angle) == 270f || Mathf.Abs(angle) == 90) {
            if (angle == 0f) {
                if (rightCharacter == attacker) {
                    angle = 180;
                }
            }
            angle = (angle + 360f) % 360f;
        } else if (angle > 0) {
            if (leftCharacter == attacker) {
                angle = (angle - 90f + 360f) % 360f;
            } else {
                angle = (angle + 90f + 360f) % 360f;
            }
            
        } else {
        if (rightCharacter == attacker) {
                angle = (angle - 90f + 360f) % 360f;
            } else {
                angle = (angle + 90f + 360f) % 360f;
            }
        }



        
        // else if (angle > 0f && angle < 180f)
        // {
        //     angle = (angle - 90f + 360f) % 360f; // Shift by +90 for 0–180 range
        // }
        // else
        // {
        //     angle = (angle + 90f) % 360f; // Shift by -90 for 180–360 range
        // }



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

        Debug.Log("Angle " + angle + " xOffset " + xOffset +  " zOffset " + zOffset + " midpoint " + midpoint.transform.position + " Camera poisiton " + cameraPosition);

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

        // // Quaternion targetRotation = Quaternion.Euler(22f, 0f, 0f);

        // // Apply the calculated rotation to the camera
        // combatCam.transform.rotation = targetRotation;

        // Re-enable CinemachineBrain
        brain.enabled = true;
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

}
