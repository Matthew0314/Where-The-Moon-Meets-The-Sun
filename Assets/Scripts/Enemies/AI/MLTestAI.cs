using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System.IO;

public class MLTestAI : MonoBehaviour, IEnemyAI
{
    private string resourcePath = "MLModels/enemy_policy/MLTestModel";

    public NNModel onnxModelAsset;
    private Model runtimeModel;
    private IWorker worker;

    private FindPath findPath;
    private GenerateGrid generateGrid;
    private PlayerGridMovement playerGridMovement;
    private ExecuteAction executeAction;
    public bool DidAction { get; set; }

    void Start()
    {
        if (onnxModelAsset == null)
        {
            onnxModelAsset = Resources.Load<NNModel>(resourcePath);
            if (onnxModelAsset == null)
            {
                Debug.LogError($"Could not find NNModel at Resources/{resourcePath}");
                return;
            }
        }

        runtimeModel = ModelLoader.Load(onnxModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);

        // Get references to your existing systems
        findPath = GameObject.Find("Player").GetComponent<FindPath>();
        generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        executeAction = GameObject.Find("Player").GetComponent<ExecuteAction>();
    }

    public IEnumerator enemyAttack(GameObject enemy)
    {
        DidAction = false;
        UnitManager enemyUnit = enemy.GetComponent<UnitManager>();

        // Build the observation array
        float[] obs = new float[6]
        {
            enemyUnit.XPos, enemyUnit.ZPos,                  // Enemy position
            0, 0,                                           // Player position placeholder
            enemyUnit.GetCurrentHealth(),                  // Enemy HP
            10                                             // Player HP placeholder
        };

        // TODO: Fill obs[2] and obs[3] with actual player position
        var playerUnit = GameObject.Find("Player").GetComponent<UnitManager>();
        obs[2] = playerUnit.XPos;
        obs[3] = playerUnit.ZPos;
        obs[5] = playerUnit.GetCurrentHealth();

        // Create a Tensor and run the model
        using (Tensor input = new Tensor(1, 6, obs))
        {
            worker.Execute(input);
            Tensor output = worker.PeekOutput();

            // Choose action with highest value
            int action = 0;
            float maxVal = output[0];
            for (int i = 1; i < output.length; i++)
            {
                if (output[i] > maxVal)
                {
                    maxVal = output[i];
                    action = i;
                }
            }
            output.Dispose();

            // Now execute the action (move/attack) using your existing systems
            yield return StartCoroutine(ExecuteAction(enemyUnit, action));
        }

        DidAction = true;
        yield return null;
    }

    private IEnumerator ExecuteAction(UnitManager enemyUnit, int action)
    {
        // Example: 0=up,1=down,2=left,3=right,4=attack
        int targetX = enemyUnit.XPos;
        int targetZ = enemyUnit.ZPos;

        if (action == 0 && targetZ > 0) targetZ -= 1;
        if (action == 1 && targetZ < generateGrid.GetLength() - 1) targetZ += 1;
        if (action == 2 && targetX > 0) targetX -= 1;
        if (action == 3 && targetX < generateGrid.GetWidth() - 1) targetX += 1;
        if (action == 4)
        {
            // Check if adjacent to player
            var player = GameObject.Find("Player").GetComponent<UnitManager>();
            if (Mathf.Abs(player.XPos - enemyUnit.XPos) + Mathf.Abs(player.ZPos - enemyUnit.ZPos) == 1)
            {
                enemyUnit.GetPrimaryWeapon().InitiateQueues(enemyUnit, player, enemyUnit.XPos, enemyUnit.ZPos, player.XPos, player.ZPos);
                yield return StartCoroutine(executeAction.ExecuteAttack(enemyUnit, player));
            }
        }

        // Move the unit if action was movement
        if (action >= 0 && action <= 3 && (targetX != enemyUnit.XPos || targetZ != enemyUnit.ZPos))
        {
            List<PathTile> shortestPath = findPath.FindShortestPath(enemyUnit.XPos, enemyUnit.ZPos, targetX, targetZ);
            foreach (var tile in shortestPath)
            {
                Vector3 pos = new Vector3(generateGrid.GetGridTile(tile.x, tile.z).GetXPos(), enemyUnit.transform.position.y, generateGrid.GetGridTile(tile.x, tile.z).GetZPos());
                enemyUnit.transform.position = pos;
                yield return null;
            }
            generateGrid.MoveUnit(enemyUnit, enemyUnit.XPos, enemyUnit.ZPos, targetX, targetZ);
        }

        yield return null;
    }

    private void OnDestroy()
    {
        worker.Dispose();
    }
}
