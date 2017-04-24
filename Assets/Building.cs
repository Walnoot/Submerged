using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {
	public static int WORLD_SIZE = 5;

	public static Vector2 xDir = new Vector2 (1f, 0.574358974f) * 0.5f;
	public static Vector2 yDir = new Vector2 (-1f, 0.574358974f) * 0.5f;
	public static Vector2 zDir = new Vector2 (0f, 0.512820513f);

	public int Price, PriceAddition;

	public int MoneyPerTick;
	public float MoneyTickTime;

	private float moneyTimer;

	public GameObject Particle;

	public GameObject TopPart;

	public bool IsRoad, IsBottomLadder, IsTopLadder;

	public int XLength, YLength;

	public BuildingUse Use;

	public int MaxOccupants;

	private List<Person> occupants = new List<Person>();

	public string FullName;

	private float particleTimer;

	//[HideInInspector]
	public int x, y, z;

	public void Place (int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;

		Vector2 pos = x * xDir + y * yDir + z * zDir;
		transform.position = pos;

		//GetComponent<SpriteRenderer> ().sortingOrder = SortOrder();

		if (TopPart != null) {
			Instantiate (TopPart).GetComponent<Building> ().Place (x, y, z + 1);
		} else {
			Building[] buildings = GameObject.FindObjectsOfType<Building> ();
			List<Building> list = new List<Building> (buildings);

			list.Sort (delegate(Building b1, Building b2) {
				if (b1 == b2) {
					return 0;
				}

				int zcmp = -b1.z.CompareTo(b2.z);

				if(zcmp != 0){
					return zcmp;
				}

				//overlap x dir

				int minx1 = b1.x;
				int maxx1 = b1.x + b1.XLength - 1;
				int minx2 = b2.x;
				int maxx2 = b2.x + b2.XLength - 1;

				if (maxx1 >= minx2 && minx1 <= maxx2) {
					return b1.y.CompareTo (b2.y);
				}

				int miny1 = b1.y;
				int mayy1 = b1.y + b1.YLength - 1;
				int miny2 = b2.y;
				int mayy2 = b2.y + b2.YLength - 1;

				if (mayy1 >= miny2 && miny1 <= mayy2) {
					return b1.x.CompareTo (b2.x);
				}

				return (b1.x + b1.y).CompareTo (b2.x + b2.y);
			});

			int i = 0;
			foreach (Building b in list) {
				b.GetComponent<SpriteRenderer> ().sortingOrder = i--;
			}
		}
	}

	void Update () {
		if (Use == BuildingUse.Work) {
			moneyTimer += Time.deltaTime;
			if (moneyTimer > MoneyTickTime) {
				moneyTimer -= MoneyTickTime;

				GameState state = GameObject.FindObjectOfType<GameState> ();

				state.money += Mathf.RoundToInt(occupants.Count * MoneyPerTick * state.GetProductivity());
			}
		}

		int vz = GameObject.FindObjectOfType<ViewLevel> ().CurrentLevel;

		particleTimer += Time.deltaTime;
		if (particleTimer > .5f) {
			particleTimer -= .5f;

			if (GetComponent<SpriteRenderer> ().enabled && Particle != null && z == vz) {
				for (int i = 0; i < occupants.Count; i++) {
					Vector3 pos = transform.position;
					pos.y += 1f;
					pos.x += Random.Range (-1f, 1f);

					Instantiate (Particle, pos, Quaternion.identity);
				}
			}
		}
	}

	public int GetPrice () {
		int count = 0;
		foreach (Building b in GameObject.FindObjectsOfType<Building>()) {
			if (b.FullName.Equals (FullName)) {
				count++;
			}
		}

		return Price + PriceAddition * count;
	}

	public bool Full () {
		return occupants.Count == MaxOccupants;
	}

	public bool Enter (Person p) {
		if (occupants.Count == MaxOccupants) {
			return false;
		} else {
			occupants.Add (p);
			return true;
		}
	}

	public void Exit (Person p) {
		occupants.Remove (p);
	}

	public bool Contains (Person p) {
		return occupants.Contains (p);
	}

	public int ZLength {
		get {
			if (TopPart == null) {
				return 1;
			} else {
				return 1 + TopPart.GetComponent<Building> ().ZLength;
			}
		}
	}

	public bool CanBuild (int x, int y, int z) {
		if (x < 0 || x + XLength > WORLD_SIZE) {
			return false;
		}

		if (y < 0 || y + YLength > WORLD_SIZE) {
			return false;
		}

		if (IsRoad && GameObject.FindObjectOfType<Building> () == null) {
			return true;
		}

		if (findBuilding (x, y, z, XLength, YLength, ZLength) != null) {
			return false;
		}

		for (int i = 0; i < YLength; i++) {
			Building b1 = findBuilding (x - 1, y + i, z, 1, 1, 1);
			Building b2 = findBuilding (x + XLength, y + i, z, 1, 1, 1);

			if ((b1 != null && b1.IsRoad) || (b2 != null && b2.IsRoad)) {
				return true;
			}
		}

		for (int i = 0; i < XLength; i++) {
			Building b1 = findBuilding (x + i, y - 1, z, 1, 1, 1);
			Building b2 = findBuilding (x + i, y + YLength, z, 1, 1, 1);

			if ((b1 != null && b1.IsRoad) || (b2 != null && b2.IsRoad)) {
				return true;
			}
		}

		return false;
	}

	public int SortOrder () {
		return SortOrder (x, y, z, XLength, YLength);
	}

	public static Building findBuilding (int x, int y, int z, int xl, int yl, int zl) {
		foreach (Building b in GameObject.FindObjectsOfType<Building>()) {
			if (x < b.x + b.XLength && x + xl > b.x &&
			    y < b.y + b.YLength && y + yl > b.y &&
			    z == b.z) {
				return b;
			}
		}

		return null;
	}

	public static int SortOrder (int x, int y, int z, int xl, int yl) {
		return -(x + xl + y + yl) + z * 1000;
	}

	public enum BuildingUse { None, Sleep, Work, Happiness };
}
