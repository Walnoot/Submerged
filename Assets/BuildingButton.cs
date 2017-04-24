using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour {

	public GameObject Building;
	public GameObject Drag;

	public Text DescriptionLabel;

	[TextArea]
	public string Description;

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer> ().color = Color.gray;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseEnter () {
		GetComponent<SpriteRenderer> ().color = Color.white;

		Building b = Building.GetComponent<Building> ();

		DescriptionLabel.text = b.FullName + " - Cost: " + b.GetPrice() + "\n" + Description;
	}

	void OnMouseExit () {
		GetComponent<SpriteRenderer> ().color = Color.gray;

		DescriptionLabel.text = "";
	}

	void OnMouseDown () {
		if (GameObject.FindObjectOfType<BuildingDrag> () == null) {
			GameObject drag = Instantiate (Drag);

			drag.GetComponent<BuildingDrag> ().BuildingPrefab = Building;
			drag.GetComponent<SpriteRenderer> ().sprite = Building.GetComponent<SpriteRenderer> ().sprite;
		}
	}
}
