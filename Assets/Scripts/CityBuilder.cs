using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {

    public GameObject enemy;
    public GameObject building;
    public GameObject road;
    public GameObject water;
    public GameObject delete;
    public GameObject empty;
    public GameObject objective;

    private GameObject plane;
    private GameObject parentBuilding;
    private GameObject parentRoad;
    private GameObject parentWater;
    private GameObject parentDelete;    
    private GameObject parentEnemy;
    private GameObject parentObjective;
    private GameObject thisPlayer;

    public  Node[,] nodeList;
    private Node currentNode;
    private Grid grid;
    public static int givenSize;

    public int sizeX;
    public int sizeZ;
    private int enemyCounter;
    private int objectiveCounter;
    private int buildingSize;
    private int checkCounter;
    private float currentX;
    private float currentZ;
    private bool ready;
    public bool built = false;

    // Use this for initialization
    private void Awake() {
            
        grid            =       GetComponent<Grid>();
        plane           =       GameObject.Find("Plane");
        parentBuilding  =       GameObject.Find("Buildings");
        parentRoad      =       GameObject.Find("Roads");
        parentWater     =       GameObject.Find("Water");
        parentDelete    =       GameObject.Find("Deleted");
        parentEnemy     =       GameObject.Find("Enemies");
        parentObjective =       GameObject.Find("Objectives");
        thisPlayer      =       GameObject.Find("Player");
        plane.transform.localScale = new Vector3(givenSize, 1, givenSize);
        sizeX           =       (int)plane.transform.localScale.x * 10;
        sizeZ           =       (int)plane.transform.localScale.z * 10;
    }

    void Start () { //Het maken van de lists en het toevoegen van de GridObjects
        ready = true;
        nodeList = grid.grid;
        
        
        
        // Dit had in een functie gekunt die 
        // afgesteld wordt op de grootte van de plane
        BuildWater();
        BuildWater();
        BuildWater();



    }
	
	void Update () { 
		if (ready) { // Zorg dat je maar
                    // 1 keer de stad creëert
            
            Create();
            ready = false;
        }
	}


    void Create() {
        //Makkelijk toegangkelijke lijsten maken
        for ( int c = 0; c < sizeX; c++) {
            for (int r = 0; r < sizeZ; r++) {
                currentNode = nodeList[c, r];

                int axis = Random.Range(0, 100);
                if ( axis >= 50) {
                    currentNode.hor = true;
                } else {
                    currentNode.ver = true;
                }
                //Check of het object nog geen waarde heeft
                //Random getal en huis of weg bouwen
                if ( currentNode.buildNumber == 0) {  
                    int number = Random.Range(0, 100);
                    if ( number >= 80) {
                        if (currentNode.hor == true) {
                            BuildStreet(c, r, 1, 1);
                        } else if (currentNode.ver == true) {
                            BuildStreet(c, r, 1, 0);
                        }
                        currentNode.buildNumber = 1;

                    } else {
                        currentNode.buildNumber = 2;
                        if (currentNode.hor == true) {
                            BuildRoad(c, r, 1, 0);
                        } else if ( currentNode.ver == true) {
                            BuildRoad(c, r, 1, 1);
                        }
                    }
                }
            }
        }
        
        CheckGrid();   
    }

    void BuildStreet(int c, int r, int value, int axis) {
        //Straat bouwen nadat er 1 huis is geplaatst
        int x = value;
        int number = Random.Range(0, 100);
        if (axis == 0) { //build Vertical     
            if (r + x < sizeZ - 1&& nodeList[c, r + x].buildNumber == 0) {
                if (number >= x * x) { 
                    value++;
                    BuildStreet(c, r, value, axis);
                    nodeList[c, r + x].buildNumber = 1;
                } else {
                    nodeList[c, r + x].buildNumber = 2;
                }
            } else {

            }
        } else if (axis == 1) { //build Horizontal
            if (c + x < sizeX - 1 && nodeList[c + x, r].buildNumber == 0) {
                if (number >= x * x) {
                    value++;
                    BuildStreet(c, r, value, axis);
                    nodeList[c + x , r].buildNumber = 1;
                } else {
                    nodeList[c + x , r].buildNumber = 2;
                }

            } else {

            }
        }
    }

    void CheckGrid() {
        // Lijsten toegankelijk maken
        for (int c = 0; c < sizeX; c++) {
            for (int r = 0; r < sizeZ; r++) {
                buildingSize = 0;
                if (nodeList[c, r].buildNumber == 2) {
                    if (enemyCounter <= 5) { // spawn enemies op een willekeurige weg met een enorm kleine kans
                        
                        float enemyNumber = Random.Range(0, 1.0f);

                        if (enemyNumber <= 0.0003) {
                            SpawnEnemy(c, r);
                            enemyCounter++;
                        }
                    }

                    if (objectiveCounter <= 4) {
                        float objectiveNumber = Random.Range(0, 1.0f);

                        if (objectiveNumber <= 0.0003) {
                            SpawnObjective(c, r);
                            objectiveCounter++;
                        }
                    }
                    
                    if (r == 0 && c == 0) { // begin van het grid
                        nodeList[c ,r].checkValue += 2;
                    }
                    // Ergens aan de randen
                    if (r == 0 || c == 0 || r == sizeZ - 1 || c == sizeX - 1) {
                        nodeList[c, r].checkValue++; 
                    }// Onder het object
                    if (r > 0 && nodeList[c, r - 1].buildNumber == 1) {
                        buildingSize++;
                        // Boven het object
                    } if (r < sizeZ - 1 && nodeList[c, r + 1].buildNumber == 1) {
                        buildingSize++;
                        // Links van het object
                    } if (c > 0 && nodeList[c - 1, r].buildNumber == 1) {
                        buildingSize++;
                        //Rechts van het object
                    } if (c < sizeZ -1 && nodeList[c + 1, r].buildNumber== 1) {
                        buildingSize++;
                        //Verander een huis in een weg
                    } if (buildingSize == 4) {
                        nodeList[c, r].buildNumber = 2;
                        //Geef alles eromheen een waarde verhoging
                    } else if (buildingSize >= 3 && c > 0 && c < sizeX - 1 && r > 0 && r < sizeZ - 1) {
                        nodeList[c, r - 1].checkValue++;
                        nodeList[c, r + 1].checkValue++;
                        nodeList[c - 1, r].checkValue++;
                        nodeList[c + 1, r].checkValue++;
                    }
                }
            }
        }
        if (checkCounter < 3) {
            checkCounter++;
            CheckGrid();
        }
        Replace();
        
            
       
        
    }

    void Replace() {
        for (int c = 0; c < sizeX; c++) {
            for (int r = 0; r < sizeZ; r++) {
                if (nodeList[c, r].checkValue >= 4) {
                    nodeList[c, r].buildNumber = 2;
                }
            }
        }
        if ( checkCounter < 3) {

        } else {
            Build();
        }
            
    }

    void Build() {
        if (built == false) {
            for (int c = 0; c < sizeX; c++) {
                for (int r = 0; r < sizeZ; r++) {
                    if (nodeList[c, r].buildNumber == 1) {
                        GameObject go = Instantiate(building, nodeList[c, r].worldPosition, Quaternion.identity, parentBuilding.transform);
                        go.transform.localScale = new Vector3(go.transform.localScale.x, Random.Range(1.0f, 8.0f), go.transform.localScale.z);
                        go.transform.position = new Vector3(go.transform.position.x, go.transform.localScale.y / 2, go.transform.position.z);
                    } else if (nodeList[c, r].buildNumber == 2) {
                        Instantiate(road, nodeList[c, r].worldPosition + new Vector3(0, road.transform.localScale.y / 2, 0), Quaternion.identity, parentRoad.transform);
                    } else if (nodeList[c, r].buildNumber == 3) {
                        Instantiate(water, nodeList[c, r].worldPosition + new Vector3(0, water.transform.localScale.y / 2, 0), Quaternion.identity, parentWater.transform);
                    } else if (nodeList[c, r].buildNumber == 4) {
                        Instantiate(delete, nodeList[c,r].worldPosition + new Vector3(0, delete.transform.localScale.y / 2, 0), Quaternion.identity, parentDelete.transform);
                    }
                }
            }
            built = true;
            grid.CheckTerrain();
        }
        
    }

    void BuildRoad (int c, int r, int value, int axis) {
        int x = value;
        int number = Random.Range(0, 100);
        if (axis == 0) { //build Vertical     
            if ((r + x)< sizeZ -1 && nodeList[c, r + x].buildNumber == 0) {
                if (number >= x * x) {
                    value++;
                    nodeList[c, r + x].buildNumber = 1;
                    BuildRoad(c, r, value, axis);
                    
                } else {

                }
            } else {

            }
        } else if (axis == 1) { //build Horizontal
            if (c + x < sizeX - 1 && nodeList[c + 1, r].buildNumber == 0) {
                if (number >= x * x) {
                    value++;
                    nodeList[c +1, r].buildNumber = 2;
                    BuildRoad(c, r, value, axis);
                    
                } else {
                    
                }

            } else {

            }
        }
    }


    void BuildWater () {
        int posX = 0;
        int posZ = Random.Range(0, sizeZ);
        
        while (posX != sizeX - 1) {
            if (nodeList[posX, posZ].buildNumber == 0) {
                nodeList[posX, posZ].buildNumber = 3;
            }
            int number = Random.Range(0, 100);
            if ( number >= 80 && posZ != sizeZ - 1) {
                posZ += 1;
            } else if ( number >= 40 && posX != sizeX - 1) {
                posX += 1;
            } else if (number >= 20 && posZ != 0) {
                posZ -= 1;
            } else if (number >= 0 && posX != 0) {
                posX -= 1;
            } 
        }
    }

    void SpawnEnemy(int c, int r) {
        GameObject _enemy = Instantiate(enemy, nodeList[c, r].worldPosition + new Vector3(0, 2f, 0), Quaternion.identity, parentEnemy.transform);
    }

    void SpawnObjective(int c, int r) {
        GameObject _objective = Instantiate(objective, nodeList[c, r].worldPosition + new Vector3(0, 1.0f, 0), Quaternion.identity, parentObjective.transform);
    }
}