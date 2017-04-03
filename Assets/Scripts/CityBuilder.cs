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

    private GameObject plane;
    private GameObject parentBuilding;
    private GameObject parentRoad;
    private GameObject parentWater;
    private GameObject parentDelete;    
    private GameObject parentEnemy;
    private GameObject thisPlayer;
    
    private List<List<GridObject>> collumns = new List<List<GridObject>>();
    private List<GridObject> currentList, previousList, nextList;

    private int sizeX;
    private int sizeZ;
    private int enemyCounter;
    private int buildingSize;
    private int checkCounter;
    private float currentX;
    private float currentZ;
    private bool ready;

    // Use this for initialization
    private void Awake() {
        
        plane           =       GameObject.Find("Plane");
        parentBuilding  =       GameObject.Find("Buildings");
        parentRoad      =       GameObject.Find("Roads");
        parentWater     =       GameObject.Find("Water");
        parentDelete    =       GameObject.Find("Deleted");
        parentEnemy     =       GameObject.Find("Enemies");
        thisPlayer      =       GameObject.Find("Player");
        sizeX           =       (int)plane.transform.localScale.x * 10;
        sizeZ           =       (int)plane.transform.localScale.z * 10;
    }

    void Start () { //Het maken van de lists en het toevoegen van de GridObjects
        ready = true;
        for (int l = 0; l < sizeX; l++) {
            collumns.Add(new List<GridObject>());
        }
        foreach (List<GridObject> item in collumns) {
            for ( int h = 0; h < sizeZ; h++) {
                item.Add(new GridObject());

            }
        }   // Dit had in een functie gekunt die 
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
            currentX = c;
            currentList = collumns[c];

            for (int r = 0; r < sizeZ; r++) {
                GridObject grid = currentList[r];
                currentZ = r;

                int axis = Random.Range(0, 100);
                if ( axis >= 50) {
                    grid.hor = true;
                } else {
                    grid.ver = true;
                }
                //Check of het object nog geen waarde heeft
                //Random getal en huis of weg bouwen
                if ( grid.value == 0) {  
                    int number = Random.Range(0, 100);
                    if ( number >= 60) {
                        if (grid.hor == true) {
                            BuildStreet(c, r, 1, 1);
                        } else if (grid.ver == true) {
                            BuildStreet(c, r, 1, 0);
                        }
                        grid.value = 1;

                    } else {
                        grid.value = 2;
                        if (grid.hor == true) {
                            BuildRoad(c, r, 1, 0);
                        } else if ( grid.ver == true) {
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
            if (currentList[r + x].value == 0) {
                if (number >= x * x) { 
                    value++;
                    BuildStreet(c, r, value, axis);
                    currentList[r + x].value = 1;
                } else {
                    currentList[r + x].value = 2;
                }
            } else {

            }
        } else if (axis == 1) { //build Horizontal
            List<GridObject> next = collumns[c + x];
            if (next[r].value == 0) {
                if (number >= x * x) {
                    value++;
                    BuildStreet(c, r, value, axis);
                    next[r].value = 1;
                } else {
                    next[r].value = 2;
                }

            } else {

            }
        }
    }

    void CheckGrid() {
        // Lijsten toegangkelijk maken
        for (int c = 0; c < sizeX; c++) {
            currentList = collumns[c];
            if (c != 0) {
                previousList = collumns[c - 1];
            }
            if (c != sizeX - 1) {
                nextList = collumns[c + 1];
            }


            for (int r = 0; r < sizeZ; r++) {
                buildingSize = 0;
                if (currentList[r].value == 2) {
                    if (enemyCounter <= 5) { // spawn enemies op een willekeurige weg met een enorm kleine kans
                        
                        float enemyNumber = Random.Range(0, 1.0f);

                        if (enemyNumber <= 0.0003) {
                            SpawnEnemy(c, r);
                            enemyCounter++;
                        }
                    }
                    
                    if (r == 0 && c == 0) { // begin van het grid
                        currentList[r].checkValue += 2;
                    }
                    // Ergens aan de randen
                    if (r == 0 || c == 0 || r == sizeZ - 1 || c == sizeX - 1) {
                        currentList[r].checkValue++; 
                    }// Onder het object
                    if (r > 0 && currentList[r - 1].value == 1) {
                        buildingSize++;
                        // Boven het object
                    } if (r < sizeZ - 1 && currentList[r + 1].value == 1) {
                        buildingSize++;
                        // Links van het object
                    } if (c > 0 && previousList[r].value == 1) {
                        buildingSize++;
                        //Rechts van het object
                    } if (c < sizeZ && nextList[r].value == 1) {
                        buildingSize++;
                        //Verander een huis in een weg
                    } if (buildingSize == 4) {
                        currentList[r].value = 2;
                        //Geef alles eromheen een waarde verhoging
                    } else if (buildingSize >= 3 && c > 0 && c < sizeX - 1 && r > 0 && r < sizeZ - 1) {
                        currentList[r - 1].checkValue++;
                        currentList[r + 1].checkValue++;
                        previousList[r].checkValue++;
                        nextList[r].checkValue++;
                    }
                }
            }
        }
        if (checkCounter < 5) {
            checkCounter++;
            CheckGrid();
        }
        Replace();
        
            
       
        
    }

    void Replace() {
        for (int c = 0; c < sizeX; c++) {
            currentList = collumns[c];
            for (int r = 0; r < sizeZ; r++) {
                if (currentList[r].checkValue >= 4) {
                    currentList[r].value = 2;
                }
            }
        }
        if ( checkCounter < 5) {

        } else {
            Build();
        }
            
    }

    void Build() {
        for (int c = 0; c < sizeX; c++) {
            currentList = collumns[c];
            for (int r = 0; r < sizeZ; r++) {
                if (currentList[r].value == 1) {
                    Vector3 buildingPos = building.transform.localScale;
                    Instantiate(building, new Vector3(buildingPos.x / 2 + c, buildingPos.y / 2, buildingPos.z / 2 + r), Quaternion.identity, parentBuilding.transform);
                } else if ( currentList[r].value == 2) {
                    Vector3 roadPos = road.transform.localScale;
                    Instantiate(road, new Vector3(roadPos.x / 2 + c, roadPos.y / 2, roadPos.z / 2 + r), Quaternion.identity, parentRoad.transform);
                } else if ( currentList[r].value == 3) {
                    Vector3 waterPos = water.transform.localScale;
                    Instantiate(water, new Vector3(waterPos.x / 2 + c, waterPos.y / 2, waterPos.z / 2 + r), Quaternion.identity, parentWater.transform);
                } else if ( currentList[r].value == 4) {
                    Vector3 deletePos = delete.transform.localScale;
                    Instantiate(delete, new Vector3(deletePos.x / 2 + c, deletePos.y / 2, deletePos.z / 2 + r), Quaternion.identity, parentDelete.transform);
                }
            }
        }
    }

    void BuildRoad (int c, int r, int value, int axis) {
        int x = value;
        int number = Random.Range(0, 100);
        if (axis == 0) { //build Vertical     
            if (currentList[r + x].value == 0) {
                if (number >= x * x) {
                    value++;
                    currentList[r + x].value = 1;
                    BuildRoad(c, r, value, axis);
                    
                } else {

                }
            } else {

            }
        } else if (axis == 1) { //build Horizontal
            List<GridObject> next = collumns[c + x];
            if (next[r].value == 0) {
                if (number >= x * x) {
                    value++;
                    next[r].value = 2;
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
            List<GridObject> current = collumns[posX];
            if (current[posZ].value == 0) {
                current[posZ].value = 3;
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
        GameObject _enemy = Instantiate(enemy, new Vector3(c + enemy.transform.localScale.x / 2, enemy.transform.localScale.y * 1.5f, enemy.transform.localScale.z / 2 + r), Quaternion.identity, parentEnemy.transform);
    }
}



        /*
        ready = true;
        for (int l = 0; l < sizeX; l++) {
            collumns.Add(new List<int>());
        }
        foreach (List<int> item in collumns) {
            for(int h = 1; h <= sizeZ; h++) {
                item.Add(0);
            }
        }
        */

/*for (int i = 0; i < sizeX; i++) {
            currentX = i;
            currentList = collumns[i];

            if (i > 0) {
                previousList = collumns[i - 1];
            }

            for (int f = 0; f < sizeZ; f++) {
                build = false;
                currentZ = f;
                if (f > 0) {
                    if (i > 0) {
                        if (previousList[f] == 0 && currentList[f - 1] == 0 ) {
                            build = true;
                        }
                    } else if (i == 0) {
                        if (currentList[f - 1] == 0) {
                            build = true;
                        }
                    }

                    
                } else if (f == 0) {
                    if (i > 0) { 
                        if (previousList[f] == 0) {
                            build = true;
                        }
                    } else if ( i == 0 ) {
                        build = true;
                    }
                }
                if ( build ) {
                    Instantiate(building, new Vector3(currentX + building.transform.localScale.x / 2, building.transform.localScale.y / 2, currentZ + building.transform.localScale.z / 2), Quaternion.identity, parentBuilding.transform);
                    currentList[f] = 1;
                    ;
                   
                }
                
            }
           
        }   */
