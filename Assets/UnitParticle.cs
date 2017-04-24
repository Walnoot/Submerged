using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParticle : MonoBehaviour {
	public AnimationCurve Curve;
	public float TotalTime;

	private float timer;

	private float startY;

	// Use this for initialization
	void Start () {
		startY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		Vector3 pos = transform.position;
		pos.y = startY + Curve.Evaluate (timer / TotalTime);
		transform.position = pos;

		if (timer > TotalTime) {
			Destroy (gameObject);
		}
	}
}
