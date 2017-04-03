using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMove : MonoBehaviour {

    private Text text;

    private enum State {
        Move,
        Wander,
        Attack
    }

    private GameObject player;
    private GameObject plane;
    private State state;
    private Vector3 rayDirection;
    private int sizeX;
    private int sizeZ;
    private int posX;
    private int posZ;
    private int hitsTaken;
    private float speed;
    private float step;
    private float chaseRange = 1000;


      
    // Use this for initialization
	void Start () {
        speed = 3;
        step = speed * Time.deltaTime;
        player = GameObject.Find("Player");
        plane = GameObject.Find("Plane");
        text = GameObject.Find("Text").GetComponent<Text>();
        sizeX = (int)plane.transform.localScale.x * 10;
        sizeZ = (int)plane.transform.localScale.z * 10;
	}

    // Update is called once per frame
	void Update () {
        text.text = "Hits Taken" + hitsTaken;
        rayDirection = player.transform.position - transform.position;
        RaycastHit hit;

        switch(state) {
            case State.Move:
                if (Physics.Raycast(transform.position, rayDirection, out hit, chaseRange)) {
                    Debug.DrawRay(transform.position, hit.point, Color.red);
                    if (hit.transform.tag == "Player") {
                        
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
                        if (((player.transform.position) - (transform.position)).magnitude < 2) {
                            state = State.Attack;
                        }
                    } else {
                        SetPosition();
                        state = State.Wander;
                    }
                }
                break;

            case State.Attack:
                StartCoroutine(Attack());
                break;

            case State.Wander:
                if (transform.position.x == posX && transform.position.z == posZ) {
                    SetPosition();
                }
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(posX, transform.position.y, posZ), step);
                if (Physics.Raycast(transform.position, rayDirection, out hit, chaseRange)) {
                    if (hit.transform.tag == "Player") {
                        state = State.Move;
                    }
                }
                
                break;
        }
	}

    void SetPosition() {
        posX = Random.Range(0, sizeX);
        posZ = Random.Range(0, sizeZ);
    }

    IEnumerator Attack() {
        hitsTaken++;
        yield return new WaitForSeconds(0.5f);
        
        state = State.Move;
    }
}
