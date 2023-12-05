using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum FieldType
{
    BUSH, BOMB, SEED, TREE
}


public class FieldHandler: MonoBehaviour
{

    //private List<GameObject> startBushes;
    public GameField[][] fields;

    public static int FIELD_SIZE;
    public static Vector3 FIELD_OFFSET;

    public GameObject PrefabBushLight;
    public GameObject PrefabBushDark;
    public GameObject PrefabSeed;
    public GameObject PrefabGreenTree;
    public GameObject PrefabYellowTree;
    public GameObject PrefabRedTree;

    public GameObject gameEnvironmentObj;

    private int bombCount = 20;
    private int treeCount = 3;
    private int seedCount = 35;

    public Color[] bushDarkColors;
    public Color[] bushLightColors;


    private static FieldHandler instance;

    public static FieldHandler GetInstance()
    {
        return instance;
    }

    public void Init()
    {
        instance = this;
        FIELD_SIZE = 20; //change field size here (for internal use)
        FIELD_OFFSET = gameEnvironmentObj.transform.position;
    }

    public void SetUpNewField()
    {
        SetUpFields();

        InstantiateFieldObjects();
        //DestroyBushesAtStart();

        CalculateBlockedViewByTrees();
        ResetChicken();
    }

    private void ResetChicken()
    {
        GridMovement.chickenResetting = true;
        GameObject kangaroo = GameManager.GetInstance().chickenObj;
        kangaroo.transform.position = new Vector3(5.5f, 0.5f, 5.5f) + FIELD_OFFSET;
        kangaroo.transform.rotation = Quaternion.Euler(Vector3.zero);
        Rigidbody kangarooRigidbody = kangaroo.GetComponent<Rigidbody>();
        kangarooRigidbody.velocity = Vector3.zero;
        kangarooRigidbody.angularVelocity = Vector3.zero;
        kangaroo.GetComponent<GridMovement>().chickenCam.transform.rotation = Quaternion.Euler(90, 0, 0);
        GridMovement.chickenResetting = false;
    }

    public void DestroyAllObjectsWithTag(string tag)
    {
        GameObject[] fieldObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject fieldObject in fieldObjects)
            Destroy(fieldObject);
    }

    private void SetUpFields()
    {
        InitGameFields();
        AssignFieldTypes();
    }

    private void AssignFieldTypes()
    {
        Dictionary<FieldType, int> fieldTypes = new Dictionary<FieldType, int>
        {
            { FieldType.BOMB, bombCount },
            { FieldType.TREE, treeCount },
            { FieldType.SEED, seedCount }
        };

        int failCounter = 0;
        foreach (KeyValuePair<FieldType, int> fieldType in fieldTypes)
        {
            int successfulAssignments = 0;
            while (successfulAssignments < fieldType.Value)
            {
                Vector2 fieldPosition = new Vector2(Random.Range(0, FIELD_SIZE), Random.Range(0, FIELD_SIZE));
                GameField field = fields[(int)fieldPosition.x][(int)fieldPosition.y];

                if (IsFieldTypeAllowedAtField(fieldType, fieldPosition, field))
                {
                    field.type = fieldType.Key;
                    successfulAssignments++;
                }

                else
                {
                    failCounter++;
                    if (failCounter > 10_000) throw new Exception("Endless loop in field creation!!");
                }
            }
        }
    }

    private void InitGameFields()
    {
        fields = new GameField[FIELD_SIZE][];
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i] = new GameField[fields.Length];

            for (int a = 0; a < fields[i].Length; a++)
            {
                fields[i][a] = new GameField(i + 5.5f, a + 5.5f);
            }
        }
    }

    private bool IsFieldTypeAllowedAtField(KeyValuePair<FieldType, int> fieldType, Vector2 fieldPosition, GameField field)
    {
        //only place bomb/tree if it does not block the last possible way to the destination 
        bool pathToDestinationStillExists = (fieldType.Key != FieldType.BOMB && fieldType.Key != FieldType.TREE)
            || DestinationWouldStillBeReachable(fieldPosition);
        bool noSeedInDestination = fieldType.Key != FieldType.SEED || fieldPosition != new Vector2(FIELD_SIZE - 1, FIELD_SIZE - 1);
        bool noNeighboredTrees = fieldType.Key != FieldType.TREE || GetAllNeighbourFieldsOfType(fieldPosition, FieldType.TREE).Count == 0;
        bool noTreesOnTheBorders = fieldType.Key != FieldType.TREE || PositionIsNotOnFieldBorder(fieldPosition);
        bool noTreesNearDestination = fieldType.Key != FieldType.TREE || fieldPosition.x < FIELD_SIZE - 3 || fieldPosition.y < FIELD_SIZE - 3;
        bool onlyBushesInFirstNineFields = fieldType.Key == FieldType.BUSH || fieldPosition.x >= 3 || fieldPosition.y >= 3;

        return field.type == FieldType.BUSH
            && pathToDestinationStillExists
            && noSeedInDestination
            && noNeighboredTrees
            && noTreesOnTheBorders
            && noTreesNearDestination
            && onlyBushesInFirstNineFields;
    }

    private bool PositionIsNotOnFieldBorder(Vector2 nextRandomVec)
    {
        return nextRandomVec.x % (FIELD_SIZE - 1) != 0 && nextRandomVec.y % (FIELD_SIZE - 1) != 0;
    }

    private bool DestinationWouldStillBeReachable(Vector2 newBlockedPosition)
    {
        Vector2 start = new Vector2(0, 0);
        Vector2 dest = new Vector2(FIELD_SIZE - 1, FIELD_SIZE - 1);

        if (newBlockedPosition == start || newBlockedPosition == dest)
            return false;

        bool[,] visited = new bool[FIELD_SIZE, FIELD_SIZE];
        Queue queue = new Queue();

        queue.Enqueue(start);
        visited[0, 0] = true;

        while (queue.Count != 0)
        {
            Vector2 node = (Vector2)queue.Dequeue();
            if (node == dest)
                return true;

            foreach (Vector2 child in GetNeighbors(node, newBlockedPosition))
            {
                if (!visited[(int)child.x, (int)child.y])
                {
                    queue.Enqueue(child);
                    visited[(int)child.x, (int)child.y] = true;
                }
            }
        }

        return false;
    }

    List<Vector2> GetNeighbors(Vector2 node, Vector2 newBombPosition)
    {
        List<Vector2> returnList = new List<Vector2>();

        //up
        if (node.y < FIELD_SIZE - 1 && FieldWouldStillBeVisitable(new Vector2(node.x, node.y + 1), newBombPosition))
            returnList.Add(new Vector2(node.x, node.y + 1));

        //down
        if (node.y > 0 && FieldWouldStillBeVisitable(new Vector2(node.x, node.y - 1), newBombPosition))
            returnList.Add(new Vector2(node.x, node.y - 1));

        //right
        if (node.x < FIELD_SIZE - 1 && FieldWouldStillBeVisitable(new Vector2(node.x + 1, node.y), newBombPosition))
            returnList.Add(new Vector2(node.x + 1, node.y));

        //left
        if (node.x > 0 && FieldWouldStillBeVisitable(new Vector2(node.x - 1, node.y), newBombPosition))
            returnList.Add(new Vector2(node.x - 1, node.y));
        return returnList;
    }

    public List<Vector2> GetAllNeighbourFieldsOfType(Vector2 from, FieldType typeToCheck)
    {
        List<Vector2> returnList = new List<Vector2>();

        //row above
        if (from.y < FIELD_SIZE - 1)
        {
            if (from.x > 0 && fields[(int)from.x - 1][(int)from.y + 1].type == typeToCheck)
                returnList.Add(new Vector2(from.x - 1, from.y + 1));

            if (fields[(int)from.x][(int)from.y + 1].type == typeToCheck)
                returnList.Add(new Vector2(from.x, from.y + 1));

            if (from.x < FIELD_SIZE - 1 && fields[(int)from.x + 1][(int)from.y + 1].type == typeToCheck)
                returnList.Add(new Vector2(from.x + 1, from.y + 1));
        }

        //left
        if (from.x > 0 && fields[(int)from.x - 1][(int)from.y].type == typeToCheck)
            returnList.Add(new Vector2(from.x - 1, from.y));

        //right
        if (from.x < FIELD_SIZE - 1 && fields[(int)from.x + 1][(int)from.y].type == typeToCheck)
            returnList.Add(new Vector2(from.x + 1, from.y));

        //row below
        if (from.y > 0)
        {
            if (from.x > 0 && fields[(int)from.x - 1][(int)from.y - 1].type == typeToCheck)
                returnList.Add(new Vector2(from.x - 1, from.y - 1));

            if (fields[(int)from.x][(int)from.y - 1].type == typeToCheck)
                returnList.Add(new Vector2(from.x, from.y - 1));

            if (from.x < FIELD_SIZE - 1 && fields[(int)from.x + 1][(int)from.y - 1].type == typeToCheck)
                returnList.Add(new Vector2(from.x + 1, from.y - 1));
        }

        return returnList;
    }

    public bool FieldIsNotBlockedOrDangerous(Vector2 rasterPos)
    {
        return fields[(int)rasterPos.x][(int)rasterPos.y].type != FieldType.BOMB
            && fields[(int)rasterPos.x][(int)rasterPos.y].type != FieldType.TREE;
    }

    public bool FieldIsVisitable(Vector2 rasterPos)
    {
        return rasterPos.x >= 0 && rasterPos.x < FIELD_SIZE && rasterPos.y >= 0 && rasterPos.y < FIELD_SIZE &&
            fields[(int)rasterPos.x][(int)rasterPos.y].type != FieldType.TREE;
    }

    private bool FieldWouldStillBeVisitable(Vector2 position, Vector2 newBombPosition)
    {
        return position != newBombPosition && FieldIsNotBlockedOrDangerous(position);
    }



    void InstantiateFieldObjects()
    {

        foreach (GameField[] row in fields)
        {
            foreach (GameField field in row)
            {
                InstantiateFieldObject(field);
            }
        }
    }

    private void InstantiateFieldObject(GameField field)
    {

        int randomRotationDegree = Random.Range(0, 359);

        switch (field.type)
        {
            case FieldType.BUSH:
            case FieldType.BOMB:
                InstantiateBush(field, randomRotationDegree);
                break;
            case FieldType.TREE:
                InstantiateTree(field);
                break;
            case FieldType.SEED:
                InstantiateSeeds(field, randomRotationDegree);
                break;

        }
    }


    private GameObject InstantiateBush(GameField field, int rotationDegree)
    {
        GameObject bush;
        float randomScaleMultiplier = Random.Range(0f, 0.5f) + 0.85f;
        Vector3 bushScale = new Vector3(25, 25, 30 * randomScaleMultiplier);
        Quaternion rotation = Quaternion.Euler(new Vector3(-90, 0, rotationDegree));

        Vector3 bushPos = new Vector3(field.posX, 0.31f, field.posZ) + FIELD_OFFSET;

        if (GetRandomBool())
        {
            bush = Instantiate(PrefabBushDark, bushPos, rotation);
            bush.transform.localScale = bushScale;
        }
        else
        {
            bush = Instantiate(PrefabBushLight, bushPos, rotation);
            bush.transform.localScale = bushScale;
        }
        bush.GetComponent<BushScript>().SetField(field);

        //--------------------------------------------------------
        //TODO destroy startbushes on game start (with sound)
        //--------------------------------------------------------


        //check whether it is a bush in the starting area
        //Vector2 rasterPos = ChickenController.GetRasterPosition(new Vector3(field.posX, 0, field.posZ));
        //print(rasterPos);
        //if (rasterPos.x < 3 && rasterPos.y < 3)
        //{
        //    print("in");
        //    startBushes.Add(bush);
        //}

        return bush;
    }


    private void InstantiateTree(GameField field)
    {
        GameObject treeobject = GetTreeObject();
        float treeYOffset = 0.72f * treeobject.transform.localScale.z / 110f;
        Quaternion treeRotation = Quaternion.Euler(new Vector3(-90, 0, 0)); // TODO: use randomRotationDegree
        Vector3 treePos = new Vector3(field.posX, treeYOffset, field.posZ) + FIELD_OFFSET;
        Instantiate(treeobject, treePos, treeRotation);
    }


    private void InstantiateBomb(GameField field, int randomRotationDegree)
    {
        GameObject bush = InstantiateBush(field, randomRotationDegree);
        bush.GetComponent<BushScript>().hasBomb = true;
    }


    private void InstantiateSeeds(GameField field, int randomRotationDegree)
    {
        Quaternion seedRotation = Quaternion.Euler(new Vector3(0, randomRotationDegree, 0));
        Vector3 seedPos = new Vector3(field.posX, 0, field.posZ) + FIELD_OFFSET;
        Instantiate(PrefabSeed, seedPos, seedRotation);
    }

    private GameObject GetTreeObject()
    {
        int treeTypeRandom = Random.Range(0, 3);
        switch (treeTypeRandom)
        {
            case 0: return PrefabYellowTree;
            case 1: return PrefabRedTree;
            default: return PrefabGreenTree;
        }
    }


    private void CalculateBlockedViewByTrees()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            for (int j = 0; j < fields[0].Length; j++)
            {
                if (fields[i][j].type == FieldType.TREE)
                    MarkNeighboursOfTreeAsNotViewable(i, j);
            }

        }
    }

    private void MarkNeighboursOfTreeAsNotViewable(int i, int j)
    {
        for (int k = -2; k <= 2; k++)
        {
            for (int l = -2; l <= 2; l++)
            {
                if (i + k < fields.Length && i + k >= 0 && j + l < fields[0].Length && j + l >= 0)
                    fields[i + k][j + l].notViewableByCamera = true;
            }
        }
    }

    public bool GetRandomBool()
    {
        return Random.Range(0, 2) > 0;
    }


    public bool FieldIsNotViewableFromBirdView(Vector2 field)
    {
        return fields[(int)field.x][(int)field.y].notViewableByCamera;
    }

    public FieldType GetFieldTypeOfField(Vector2 field)
    {
        return fields[(int)field.x][(int)field.y].type;
    }

    public FieldType SetFieldTypeOfField(Vector2 field, FieldType type)
    {
        return fields[(int)field.x][(int)field.y].type = type;
    }

    public void SetBombCount(int bc)
    {
        bombCount = bc;
    }

    public void SetTreeCount(int tc)
    {
        treeCount = tc;
    }

    public void SetSeedCount(int sc)
    {
        seedCount = sc;
    }
}
