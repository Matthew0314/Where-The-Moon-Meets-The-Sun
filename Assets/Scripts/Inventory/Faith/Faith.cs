using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Faith
{
    public string Name { get; protected set; }
    public bool Range1 { get; protected set; }
    public bool Range2 { get; protected set; }
    public bool Range3 { get; protected set; }
    public int Range { get; protected set; }
    public int MaxUses { get; protected set; }
    public int Uses { get; protected set; }

    protected Faith() {
        Range1 = false;
        Range2 = false;
        Range3 = false;
        Range = 0;
        Name = "none";
    }

    protected Faith(string nam, int uses, int r, bool r1, bool r2, bool r3) {
        Name = nam;
        Uses = uses;
        MaxUses = uses;
        Range = r;
        Range1 = r1;
        Range2 = r2;
        Range3 = r3;
    }

    
    public abstract bool CanUse(UnitManager user);
    public abstract IEnumerator Use(UnitManager user, UnitManager reciever);

    public abstract List<GridTile> GetUnitsInRange(UnitManager unit);

    
}

public class Heal : Faith {
    public Heal() : base("Heal", 1, 1, true, false, false) {}

    public override bool CanUse(UnitManager user) {
        if (Uses <= 0) { return false; }
        FindPath findPath = GameObject.Find("Player").GetComponent<FindPath>();
        PlayerGridMovement playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        GenerateGrid generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();

        if (findPath == null || playerGridMovement == null) { return false; }

        bool[,] attackGrid = findPath.CalculateAttack(playerGridMovement.getX(), playerGridMovement.getZ(), Range, Range1, Range2, Range3);

        int width = attackGrid.GetLength(0);  // The number of rows
        int length = attackGrid.GetLength(1); // The number of columns

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                if (attackGrid[i, j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Player") && generateGrid.GetGridTile(i,j).UnitOnTile.stats.Name != user.stats.Name) {
                    if (generateGrid.GetGridTile(i, j).UnitOnTile.getCurrentHealth() < generateGrid.GetGridTile(i, j).UnitOnTile.getMaxHealth()) {
                        return true;
                    }
                    
                }
            }
        }

        return false;


    }

    public override List<GridTile> GetUnitsInRange(UnitManager unit) {
        FindPath findPath = GameObject.Find("Player").GetComponent<FindPath>();
        PlayerGridMovement playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        GenerateGrid generateGrid = GameObject.Find("GridManager").GetComponent<GenerateGrid>();

        List<GridTile> listofTiles = new List<GridTile>();
        bool[,] attackGrid = findPath.CalculateAttack(playerGridMovement.getX(), playerGridMovement.getZ(), Range, Range1, Range2, Range3);

        int width = attackGrid.GetLength(0);  // The number of rows
        int length = attackGrid.GetLength(1); // The number of columns

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                if (attackGrid[i, j] && generateGrid.GetGridTile(i,j).UnitOnTile != null && generateGrid.GetGridTile(i,j).UnitOnTile.UnitType.Equals("Player") && generateGrid.GetGridTile(i,j).UnitOnTile.stats.Name != unit.stats.Name) {
                    if (generateGrid.GetGridTile(i, j).UnitOnTile.getCurrentHealth() < generateGrid.GetGridTile(i, j).UnitOnTile.getMaxHealth()) {
                        listofTiles.Add(generateGrid.GetGridTile(i, j));
                    }
                    
                }
            }
        }

        return listofTiles;
    }

    public override IEnumerator Use(UnitManager user, UnitManager reciever) {
        Uses--;
        reciever.HealUnit(10);
        // yield return StartCoroutine(reciever.Heal(10));
        yield return null;
    }


}
