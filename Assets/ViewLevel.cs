using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewLevel : MonoBehaviour {
	public int CurrentLevel = 0;

	private float yOffset;

	private GameState state;

	// Use this for initialization
	void Start () {
		yOffset = transform.position.y;

		state = GameObject.FindObjectOfType<GameState> ();
	}
	
	// Update is called once per frame
	void Update () {
		int newLevel = CurrentLevel;

		if (Input.mouseScrollDelta.y > 0f) {
			newLevel += 1;
		} else if (Input.mouseScrollDelta.y < 0f) {
			newLevel -= 1;
		}

		if (newLevel < state.GetWaterLevel()) {
			newLevel = state.GetWaterLevel();
		}

		int maxLevel = 0;
		foreach (Building b in GameObject.FindObjectsOfType<Building>()) {
			if (b.z > maxLevel) {
				maxLevel = b.z;
			}
		}

		if (newLevel > maxLevel) {
			newLevel = maxLevel;
		}

		if (newLevel != CurrentLevel) {
			CurrentLevel = newLevel;

			Vector3 pos = transform.position;
			pos.y = CurrentLevel * Building.zDir.y + yOffset;
			transform.position = pos;

			foreach (Building b in GameObject.FindObjectsOfType<Building>()) {
				SpriteRenderer sprite = b.GetComponent<SpriteRenderer> ();
				if (b.z <= CurrentLevel) {
					sprite.enabled = true;

					float grayScale = Mathf.Max(1f - (CurrentLevel - b.z) * 0.1f, 0f);
					if (b.z != CurrentLevel) {
						grayScale -= .4f;
					}

					sprite.color = new Color(grayScale, grayScale, grayScale);
				} else {
					sprite.enabled = false;
				}
			}
		}
	}
}
