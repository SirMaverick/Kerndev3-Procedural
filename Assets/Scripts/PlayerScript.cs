using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {

    public bool powerUp;
    int score;
    int hitsTaken;
    int totalObjectives = -1;
    public Text scoreText;
    public Text hitText;


	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0, 20, 0);
        StartCoroutine(WaitForBuilt());
    }
	
	// Update is called once per frame
	void Update () {
        if (score == totalObjectives) {
            EndScreen();
        }
        if (hitsTaken == 5) {
            DeathScreen();
        }
		
	}

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Objective") {
            AddScore();
            Destroy(other.gameObject);
        }
    }

    private void AddScore() {
        score++;
        scoreText.text = "Objectives Found : " + score + " / " + totalObjectives;
    }

    private void EndScreen() {
        SceneManager.LoadScene(2);
    }

    private void DeathScreen() {
        SceneManager.LoadScene(3);
    }

    public void AddHit() {
        hitsTaken++;
        hitText.text = "Hits Taken : " + hitsTaken;
    }

    IEnumerator WaitForBuilt() {
        yield return new WaitForSeconds(0.3f);
        totalObjectives = GameObject.FindGameObjectsWithTag("Objective").Length;
        scoreText.text = "Objectives Found : " + score + " / " + totalObjectives;       
    }
}
