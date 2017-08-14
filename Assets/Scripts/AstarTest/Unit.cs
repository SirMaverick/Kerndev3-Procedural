using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    public Transform target;
    public float speed = 20;
    public float turnDst = 5;
    public float turnSpeed = 3;
    public float stoppingDst = 10;

    State state;
    State previousState;
    Path path;

    private enum State {
        Move,
        Rest,
        Attack
    }

    private void Start() {
        target = GameObject.Find("Player").transform;
        state = State.Move;
        StartCoroutine(UpdatePath());
    }

    void Update() {
        print(state);

        if (((target.transform.position) - (transform.position)).magnitude < 2 && state ==State.Move) {
            StopCoroutine(FollowPath());
            state = State.Attack;
        }
        switch (state) {
            case State.Move:
                if ( previousState != state) {
                    StopCoroutine(FollowPath());
                    StartCoroutine(UpdatePath());
                    previousState = State.Move;
                    break;
                }
                break;


            case State.Attack:
                if (previousState != state) {
                    target.GetComponent<PlayerScript>().AddHit();
                    previousState = State.Attack;
                    state = State.Rest;
                }
                break;

            case State.Rest:
                if(previousState != state) {
                    StopCoroutine(FollowPath());
                    StartCoroutine(Rest());
                    previousState = State.Rest;
                }
                break;
        }
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccesful) {
        if(pathSuccesful) {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath() {
        if(Time.timeSinceLevelLoad < 0.3f) {
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while(true) {
            yield return new WaitForSeconds(minPathUpdateTime); 
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
                
            }
            
        }
    }

    IEnumerator FollowPath() {
        bool followingPath = true;
        int pathIndex = 0;
        float speedPercent = 1;
        transform.LookAt(path.lookPoints[0]);

        while(followingPath) {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while(path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)) {
                if (pathIndex == path.finishLineIndex) {
                    followingPath = false;
                    break;
                } else {
                    pathIndex++;
                }
            }
            if (followingPath) {
                if(pathIndex >= path.slowDownIndex && stoppingDst > 0) {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if(speedPercent < 0.01f) {
                        followingPath = false;
                    }
                }
               
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self); 
            }
             
            yield return null;
        }
    }


    IEnumerator Rest() {
        yield return new WaitForSeconds(3.0f);
        state = State.Move;
    }

    public void OnDrawGizmos() {
        if (path != null) {
            path.DrawWithGizmos();
            }
        }
    }
