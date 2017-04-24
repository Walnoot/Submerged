using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDrag : MonoBehaviour {

	public GameObject BuildingPrefab;
	public GameObject SelectPrefab;

	private List<GameObject> tiles = new List<GameObject> ();

	private ViewLevel viewLevel;
	private GameState state;

	private int oldLevel;

	// Use this for initialization
	void Start () {
		viewLevel = GameObject.FindObjectOfType<ViewLevel> ();
		state = GameObject.FindObjectOfType<GameState> ();

		showTiles ();
	}

	private void showTiles () {
		oldLevel = viewLevel.CurrentLevel;

		foreach (GameObject t in tiles) {
			Destroy (t);
		}

		for (int x = 0; x < Building.WORLD_SIZE; x++) {
			for (int y = 0; y < Building.WORLD_SIZE; y++) {
				if (Building.findBuilding (x, y, viewLevel.CurrentLevel, 1, 1, 1) == null) {
					Vector2 pos = Building.xDir * x + Building.yDir * y + viewLevel.CurrentLevel * Building.zDir;

					GameObject tile = Instantiate (SelectPrefab, pos, Quaternion.identity);
					tile.GetComponent<SpriteRenderer> ().sortingOrder = Building.SortOrder(x, y, viewLevel.CurrentLevel + 1, 1, 1);
					tiles.Add (tile);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (1)) {
			remove ();
			return;
		}

		Building b = BuildingPrefab.GetComponent<Building> ();

		Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		pos -= Building.xDir * b.XLength * 0.5f;
		pos -= Building.yDir * b.YLength * 0.5f;

		int gz = viewLevel.CurrentLevel;
		float gx = 0.5f * (pos.x / Building.xDir.x + (pos.y - gz * Building.zDir.y) / Building.xDir.y);
		float gy = 0.5f * ((pos.y - gz * Building.zDir.y) / Building.xDir.y - pos.x / Building.xDir.x);

		int rx = Mathf.RoundToInt (gx);
		int ry = Mathf.RoundToInt (gy);

		GetComponent<SpriteRenderer> ().sortingOrder = Building.SortOrder(rx, ry, gz, b.XLength, b.YLength);
		GetComponent<SpriteRenderer> ().color = Color.gray;

		if (rx >= 0 && rx < Building.WORLD_SIZE && ry >= 0 && ry < Building.WORLD_SIZE) {
			bool canBuild = b.CanBuild(rx, ry, viewLevel.CurrentLevel);

			if (canBuild) {
				pos = rx * Building.xDir + ry * Building.yDir + gz * Building.zDir;
				//pos.y += 0.05f;

				GetComponent<SpriteRenderer> ().sortingOrder = b.SortOrder ();
				GetComponent<SpriteRenderer> ().color = Color.white;
			}

			if (Input.GetMouseButtonDown (0) && canBuild) {
				if (state.Buy (b.GetPrice())) {
					GameObject building = Instantiate (BuildingPrefab);
					building.GetComponent<Building>().Place (rx, ry, gz);

					Building prefabBuilding = BuildingPrefab.GetComponent<Building> ();

					remove ();
				}
			}
		}

		transform.position = pos;

		if (viewLevel.CurrentLevel != oldLevel) {
			showTiles ();
		}
	}

	private void remove() {
		Destroy (gameObject);

		foreach (GameObject t in tiles) {
			Destroy (t);
		}
	}
}
