using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonButton : MonoBehaviour {
	public Text DescriptionLabel;

	[TextArea]
	public string Description;

	private int cost = 300;

	void Start () {
		GetComponent<SpriteRenderer> ().color = Color.gray;
	}

	void OnMouseEnter () {
		GetComponent<SpriteRenderer> ().color = Color.white;

		DescriptionLabel.text = "Citizen - Cost: " + cost + "\n" + Description;
	}

	void OnMouseExit () {
		GetComponent<SpriteRenderer> ().color = Color.gray;

		DescriptionLabel.text = "";
	}

	void OnMouseDown () {
		GameState state = GameObject.FindObjectOfType<GameState> ();

		if (state.Buy (cost)) {
			cost += 100;
			OnMouseEnter ();

			state.AddPerson ();
		}
	}
}
