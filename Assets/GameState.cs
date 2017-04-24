using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
	public int money;
	public int Happiness;

	public GameObject Road, Person, Grass, House, Bar, Factory;

	public Text MoneyLabel, PopulationLabel, DemandLabel, SeaLevelLabel;

	[TextArea]
	public string DemandDescription;

	public List<GameObject> Demands;

	private Building currentDemand;
	private float productivity;
	private int nextDemandPop = 5, demandAmountRequired;

	private List<GameObject> grassTiles = new List<GameObject>();

	private int waterLevel = 0;
	private float waterTimer = 240f;

	public Text TutorialText;

	void Start () {
		Instantiate (Road).GetComponent<Building> ().Place (0, 2, 0);
		Instantiate (Road).GetComponent<Building> ().Place (1, 2, 0);
		Instantiate (Road).GetComponent<Building> ().Place (2, 2, 0);
		Instantiate (Road).GetComponent<Building> ().Place (3, 2, 0);
		Instantiate (Road).GetComponent<Building> ().Place (4, 2, 0);

		Instantiate (Factory).GetComponent<Building> ().Place (3, 3, 0);
		Instantiate (House).GetComponent<Building> ().Place (0, 3, 0);
		Instantiate (Bar).GetComponent<Building> ().Place (3, 0, 0);

		Instantiate (Person).GetComponent<Person>().Place(0, 2, 0);

		for (int x = 0; x < Building.WORLD_SIZE; x++) {
			for (int y = 0; y < Building.WORLD_SIZE; y++) {
				Vector3 pos = Building.xDir * x + Building.yDir * y;

				GameObject tile = Instantiate (Grass, pos, Quaternion.identity);

				tile.GetComponent<SpriteRenderer>().sortingOrder = - x - y;
				grassTiles.Add (tile);
			}
		}
	}

	void Update () {
		MoneyLabel.text = "" + money;
		PopulationLabel.text = "" + PopulationCount();

		if (PopulationCount () == nextDemandPop) {
			nextDemandPop += 3;
			productivity = .9f;

			currentDemand = Demands[Random.Range(0, Demands.Count)].GetComponent<Building>();

			demandAmountRequired = demandCount () + 1;
		}

		if (currentDemand != null) {
			productivity -= Time.deltaTime * 0.01f / 4f;
			if (productivity < .75f) {
				productivity = .75f;
			}

			int percentage = Mathf.RoundToInt (productivity * 100f);

			DemandLabel.text = string.Format (DemandDescription, currentDemand.FullName, percentage);

			if (demandAmountRequired == demandCount ()) {
				currentDemand = null;
				DemandLabel.text = "";
			}
		}

		int seconds = Mathf.FloorToInt (waterTimer);
		int minutes = seconds / 60;
		seconds = seconds % 60;

		string timeLeft;
		if (minutes == 0) {
			timeLeft = waterTimer.ToString("F1");
		} else {
			timeLeft = minutes + ":" + seconds.ToString("D2");
		}

		SeaLevelLabel.text = timeLeft;
		
		waterTimer -= Time.deltaTime;
		if (waterTimer < 0f) {
			waterTimer += 150f;

			IncreaseWater ();
		}

		if (Input.GetButtonDown ("quit")) {
			Application.Quit ();
		}

		if (Input.GetButtonDown ("help")) {
			TutorialText.enabled = !TutorialText.enabled;

			foreach (Image c in TutorialText.gameObject.GetComponentsInChildren<Image>()) {
				c.enabled = TutorialText.enabled;
			}
		}
	}

	private void IncreaseWater () {
		waterLevel++;

		foreach (GameObject tile in grassTiles) {
			Destroy (tile);
		}
		grassTiles.Clear ();

		foreach (Building b in GameObject.FindObjectsOfType<Building>()) {
			if (b.z < waterLevel) {
				Destroy (b.gameObject);
			}
		}

		foreach (Person p in GameObject.FindObjectsOfType<Person>()) {
			if (p.z < waterLevel) {
				Destroy (p.gameObject);
			}
		}
	}

	public int GetWaterLevel () {
		return waterLevel;
	}

	public float GetWaterTimer () {
		return waterTimer;
	}

	private int demandCount () {
		int count = 0;

		foreach (Building b in GameObject.FindObjectsOfType<Building>()) {
			if (b.FullName.Equals (currentDemand.FullName)) {
				count++;
			}
		}

		return count;
	}

	public float GetProductivity () {
		if (currentDemand == null) {
			return 1f;
		} else {
			return productivity;
		}
	}

	public void AddPerson () {
		List<Building> spawns = new List<Building> ();

		foreach (Building b in GameObject.FindObjectsOfType<Building>()) {
			if (b.IsRoad) {
				spawns.Add (b);
			}
		}

		Building spawn = spawns[Random.Range(0, spawns.Count)];

		Instantiate (Person).GetComponent<Person>().Place(spawn.x, spawn.y, spawn.z);
	}

	public bool Buy (int price) {
		if (money >= price) {
			money -= price;
			return true;
		}

		return false;
	}

	public int PopulationCount () {
		return GameObject.FindObjectsOfType<Person> ().Length;
	}
}
