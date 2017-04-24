using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("start")) {
			SceneManager.LoadScene ("main");
		}

		if (Input.GetButtonDown ("quit")) {
			Application.Quit ();
		}
	}
}
