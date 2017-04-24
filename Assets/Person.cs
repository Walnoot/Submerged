using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {
	[HideInInspector]
	public int x, y, z;
	private int dx, dy, dz;

	private float timer;

	private Building.BuildingUse goal = Building.BuildingUse.Work;

	private Building currentBuilding;

	public SpriteRenderer GoalSprite;

	public Sprite Work, Sleep, Happy;

	private GameState state;

	public void Place(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;

		state = GameObject.FindObjectOfType<GameState> ();
	}
	
	// Update is called once per frame
	void Update () {
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();

		if (currentBuilding != null) {
			if (currentBuilding.Contains (this)) {
				sprite.enabled = false;
				GoalSprite.enabled = false;
			} else {
				SpriteRenderer bSprite = currentBuilding.GetComponent<SpriteRenderer> ();

				sprite.enabled = bSprite.enabled;
				sprite.color = bSprite.color;
				sprite.sortingOrder = bSprite.sortingOrder + 1;

				GoalSprite.enabled = bSprite.enabled;
				GoalSprite.color = bSprite.color;
				GoalSprite.sortingOrder = bSprite.sortingOrder + 1;

				if (goal == Building.BuildingUse.Work) {
					GoalSprite.sprite = Work;
				} else if (goal == Building.BuildingUse.Sleep) {
					GoalSprite.sprite = Sleep;
				} else if (goal == Building.BuildingUse.Happiness) {
					GoalSprite.sprite = Happy;
				} else {
					GoalSprite.enabled = false;
				}
			}
		}

		timer -= Time.deltaTime;
		if (timer < 0f) {
			timer += 1f;

			if (currentBuilding != null) {
				currentBuilding.Exit (this);
			}

			Building road = Building.findBuilding (x, y, z, 1, 1, 1);
			bool bottomLadder = road != null && road.IsBottomLadder;
			bool topLadder = road != null && road.IsTopLadder;

			bool evac = state.GetWaterTimer () < 30f && z == state.GetWaterLevel();
			bool evacBelow = state.GetWaterTimer () < 30f && z == state.GetWaterLevel() + 1;

			List<Building> paths = new List<Building> ();
			paths.Add (Building.findBuilding (x + 1, y, z, 1, 1, 1));
			paths.Add (Building.findBuilding (x - 1, y, z, 1, 1, 1));
			paths.Add (Building.findBuilding (x, y + 1, z, 1, 1, 1));
			paths.Add (Building.findBuilding (x, y - 1, z, 1, 1, 1));
			if (bottomLadder) {
				paths.Add (Building.findBuilding (x, y, z + 1, 1, 1, 1));
			}
			if (topLadder && !evacBelow) {
				paths.Add (Building.findBuilding (x, y, z - 1, 1, 1, 1));
			}

			paths = paths.FindAll (delegate(Building b) {
				return b != null;
			});

			List<Building> goals = paths.FindAll (delegate(Building b) {
				return !b.Full() && b.Use == goal;
			});

			if (goals.Count > 0 && !evac) {
				goals [0].Enter (this);
				currentBuilding = goals [0];

				//sprite.enabled = false;
				timer = 7f;

				goal = nextGoal (goal);
			} else {
				paths = paths.FindAll (delegate(Building b) {
					return b.IsRoad;
				});

				if (paths.Count >= 2) {
					paths = paths.FindAll (delegate(Building b) {
						return !(b.x == x - dx && b.y == y - dy && b.z == z - dz);
					});

					sprite.enabled = false;
				}

				if (paths.Count > 0) {
					Building next = null;

					if (evac) {
						int maxz = z;

						foreach (Building b in paths) {
							if (b.z > maxz) {
								maxz = b.z;
								next = b;

								Debug.Log ("found better path " + maxz);
							}
						}
					}

					if (next == null) {
						next = paths[Random.Range (0, paths.Count)];
					}

					currentBuilding = next;

					bool xdir = x == next.x;

					dx = next.x - x;
					dy = next.y - y;
					dz = next.z - z;

					x = next.x;
					y = next.y;
					z = next.z;

					transform.position = next.transform.position;

					sprite.flipX = xdir;
				}
			}
		}
	}

	private Building.BuildingUse nextGoal(Building.BuildingUse prevGoal) {
		if (prevGoal == Building.BuildingUse.Sleep) {
			return Building.BuildingUse.Work;
		} else if (prevGoal == Building.BuildingUse.Work) {
			if (Random.Range (0f, 1f) > .7f) {
				return Building.BuildingUse.Happiness;
			} else {
				return Building.BuildingUse.Sleep;
			}
		} else {
			return Building.BuildingUse.Sleep;
		}
	}
}
