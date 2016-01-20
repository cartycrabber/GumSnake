using UnityEngine;
using System.Collections;

public class SnakeHead : MonoBehaviour {

	private enum Direction
	{
		UP,
		DOWN,
		LEFT,
		RIGHT
	};

	private GameObject gameManager;

	private int snakeLength;

	public bool cycling;

	private bool addLength = false;

	private Direction movementDirection;
	private const float cycleTime = .15f; //in seconds
	private float deltaCycleTime; //Time since last cycle

	private const float snakeMovement = 1f;//the distance to move

	// Use this for initialization
	void Start () {
		movementDirection = Direction.LEFT;
		deltaCycleTime = 0;
		gameManager = GameObject.Find("Manager");
	}
	
	// Update is called once per frame
	void Update () {
		if(cycling) {
			deltaCycleTime += Time.deltaTime;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			movementDirection = Direction.UP;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			movementDirection = Direction.DOWN;
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			movementDirection = Direction.LEFT;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			movementDirection = Direction.RIGHT;
		}

		if (deltaCycleTime >= cycleTime) {
			deltaCycleTime = 0;

			snakeLength = GameObject.FindGameObjectsWithTag ("SnakeBody").Length + 1;

			if (!addLength) {
				for (int x = snakeLength - 2; x >= 0; x--) {
					GameObject.Find ("SnakeBody" + x).SendMessage ("Move");
				}
			}
			switch (movementDirection) {
			case Direction.UP:
				transform.position = new Vector2 (transform.position.x, transform.position.y + snakeMovement);
				break;
			case Direction.DOWN:
				transform.position = new Vector2 (transform.position.x, transform.position.y - snakeMovement);
				break;
			case Direction.LEFT:
				transform.position = new Vector2 (transform.position.x - snakeMovement, transform.position.y);
				break;
			case Direction.RIGHT:
				transform.position = new Vector2 (transform.position.x + snakeMovement, transform.position.y);
				break;
			}

			gameManager.SendMessage ("CheckCollisions");
		}
	}

	void AddLength ()
	{
		addLength = true;
	}
}
