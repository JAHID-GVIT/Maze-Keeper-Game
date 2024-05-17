using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Maze : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject north;
        public GameObject east;
        public GameObject west;
        public GameObject south;
    }
    public GameObject yourObject;
    public Material groundMaterial;
    public GameObject wall;
    public float wallLength = 1.0f;
    public int xSize = 5;
    public int ySize = 5;
    private Vector3 initialPos;
    private GameObject wallHolder;
    private Cell[] cells;
    private int currentCell = 0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;


    // Start is called before the first frame update
    void Start()
    {
        CreateWalls();
        CreateGroundUnderMaze();
    }

    void CreateWalls()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";
        initialPos = new Vector3((-xSize / 2) + wallLength / 2, 0.0f, (-ySize / 2) + wallLength / 2);
        Vector3 Mypos = initialPos;
        GameObject tempWall;

        // Create walls for x axis
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
                Mypos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, Mypos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        // Create walls for y axis
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                Mypos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, Mypos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        CreateCells();
    }
    void CreateGroundUnderMaze()
    {
        // Calculate the size of the ground plane based on the wall length and maze dimensions
        float groundWidth = xSize * wallLength;
        float groundLength = ySize * wallLength;

        // Create a ground plane centered under the maze
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = new Vector3(0, -0.5f, 0); // Adjust the Y position as needed
        ground.transform.localScale = new Vector3(groundWidth, 1, groundLength);

        // Remove the collider since we don't need it
        Destroy(ground.GetComponent<Collider>());

        // Add the GroundTextureSetter script to the ground object
        GroundTextureSetter textureSetter = ground.AddComponent<GroundTextureSetter>();

        // Check if the ground material is assigned in the Maze script
        if (groundMaterial != null)
        {
            // Assign the ground material to the GroundTextureSetter script
            textureSetter.groundMaterial = groundMaterial;
        }
        else
        {
            Debug.LogWarning("Ground material is not assigned in the Maze script.");
        }
    }



    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        // Get all the children
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }
        // assigns walla to the cells
        for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++)
        {
            if (termCount == xSize)
            {
                eastWestProcess++;
                termCount = 0;
            }
            cells[cellprocess] = new Cell();
            cells[cellprocess].east = allWalls[eastWestProcess];
            cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];

            eastWestProcess++;

            termCount++;
            childProcess++;
            cells[cellprocess].west = allWalls[eastWestProcess];
            cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
        }
        CreateMaze();

    }
    void CreateMaze()
    {
        while (visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNeighbour();
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }
            Debug.Log("Finished");
            //Invoke("CreateMaze", 0.0f);
            //If you want to see how maze is created then use this and change the while loop above to if......
            if (visitedCells == totalCells)
            {
                // Spawn an object in a random corner of the maze
                int randomCorner = Random.Range(0, 4);
                Vector3 spawnPosition;

                switch (randomCorner)
                {
                    case 0:
                        spawnPosition = new Vector3(initialPos.x - wallLength / 2, 0.0f, initialPos.z - wallLength / 2);
                        break;
                    case 1:
                        spawnPosition = new Vector3(initialPos.x + xSize * wallLength - wallLength / 2, 0.0f, initialPos.z - wallLength / 2);
                        break;
                    case 2:
                        spawnPosition = new Vector3(initialPos.x - wallLength / 2, 0.0f, initialPos.z + ySize * wallLength - wallLength / 2);
                        break;
                    default:
                        spawnPosition = new Vector3(initialPos.x + xSize * wallLength - wallLength / 2, 0.0f, initialPos.z + ySize * wallLength - wallLength / 2);
                        break;
                }

                // Instantiate the object at the random corner
                Instantiate(yourObject, spawnPosition, Quaternion.identity);
            }
        }

    }
    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1:
                Destroy(cells[currentCell].north);
                break;
            case 2:
                Destroy(cells[currentCell].east);
                break;
            case 3:
                Destroy(cells[currentCell].west);
                break;
            case 4:
                Destroy(cells[currentCell].south);
                break;
        }
    }
    void GiveMeNeighbour()
    {

        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;
        // West 
        if (currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;

            }
        }
        // East
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;

            }
        }
        //North
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;

            }
        }
        //South
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;

            }
        }
        if (length != 0)
        {
            int theChosenOne = Random.Range(0, length);
            currentNeighbour = neighbours[theChosenOne];
            wallToBreak = connectingWall[theChosenOne];
        }
        else
        {
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
