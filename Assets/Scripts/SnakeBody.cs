using UnityEngine;
using System.Collections;

public class SnakeBody : MonoBehaviour {

	public GameObject leader;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Move () {
		gameObject.transform.position = leader.transform.position;
	}
}
