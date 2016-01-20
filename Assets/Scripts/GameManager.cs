using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	#region Settings
	private const int bottomBorder = -9;
	private const int topBorder = 8;
	private const int leftBorder = -14;
	private const int rightBorder = 14;

	private const int initialSnakeSize = 3;
	private const float cooldownPeriod = 5;//In Seconds
	#endregion

	public GameObject snakeHeadPrefab;
	public GameObject snakeBodyPrefab;

	public GameObject periodItem;
	public GameObject commaItem;
	public GameObject semicolonItem;
	public GameObject colonItem;
	public GameObject blankItem;

	public GameObject questionText;
	public GameObject scoreText;
	public GameObject wrongText;

	private GameObject currentSnakeHead;
	private List<GameObject> currentSnakeBodies;

	private List<GameObject> spawnedItems;

	private GameObject answerObject;

	float cooldown;

	// Use this for initialization
	void Start () {
		currentSnakeBodies = new List<GameObject> ();
		spawnedItems = new List<GameObject> ();
		CreateSnake (initialSnakeSize);
		SpawnItem ();
		PickSentence ();
		SpawnItem (answerObject);
		cooldown = 0;
	}
	
	// Update is called once per frame
	void Update () {
		bool cooling = false;
		if(cooldown > 0)
		{
			cooldown -= Time.deltaTime;
		}
		if((cooldown <= 0) && (currentSnakeHead == null))
		{
			wrongText.GetComponent<Text> ().enabled = false;
			CreateSnake(initialSnakeSize);
			scoreText.GetComponent<Text> ().text = "Score: 0";
			foreach(GameObject item in spawnedItems)
			{
				Destroy (item);
			}
			spawnedItems.Clear ();
			PickSentence ();
			SpawnItem ();
			SpawnItem (answerObject);
		}
	}

	void CreateSnake (int length) {
		currentSnakeHead = Instantiate (snakeHeadPrefab, new Vector2 (0, 0), Quaternion.identity) as GameObject;
		currentSnakeHead.name = "SnakeHead";
		for(int x = 0; x < length - 1; x++) {
			Object newObj = Instantiate(snakeBodyPrefab, new Vector2((x + 1), 0), Quaternion.identity);
			newObj.name = "SnakeBody" + x;
			if(x == 0)
			{
				GameObject.Find (newObj.name).GetComponent<SnakeBody> ().leader = GameObject.Find ("SnakeHead");
			}
			else {
				GameObject.Find (newObj.name).GetComponent<SnakeBody> ().leader = GameObject.Find ("SnakeBody" + (x - 1));
			}
			currentSnakeBodies.Add ((GameObject)newObj);
		}
		currentSnakeHead.GetComponent<SnakeHead> ().cycling = true;
	}

	void CheckCollisions () {
		if(currentSnakeHead.transform.position.x > rightBorder || currentSnakeHead.transform.position.x < leftBorder ||
			currentSnakeHead.transform.position.y > topBorder || currentSnakeHead.transform.position.y < bottomBorder)
		{
			Debug.Log ("You went out of the border");
			DestroySnake ();
			wrongText.GetComponent<Text> ().text = "You Died!\nGame Over\nCorrect Answer: " + answerObject.name;
			wrongText.GetComponent<Text> ().enabled = true;
			cooldown = cooldownPeriod;
			return;
		}
		else
		{
			foreach(GameObject body in currentSnakeBodies) {
				if(body.transform.position == currentSnakeHead.transform.position)
				{
					Debug.Log("You Died");
					DestroySnake ();
					wrongText.GetComponent<Text> ().text = "You Died!\nGame Over\nCorrect Answer: " + answerObject.name;
					wrongText.GetComponent<Text> ().enabled = true;
					cooldown = cooldownPeriod;
					return;
				}
			}

			GameObject itemToRemove = null;

			foreach(GameObject item in spawnedItems) {
				if(item.transform.position == currentSnakeHead.transform.position)
				{
					if(item.name == answerObject.name)
					{
						Debug.Log ("Correct");
						scoreText.GetComponent<Text> ().text = "Score: " + (currentSnakeBodies.Count + 2 - initialSnakeSize);
						AddToSnake ();
						itemToRemove = item;
						Destroy (item);
					}
					else
					{
						Debug.Log ("Wrong");
						DestroySnake ();
						wrongText.GetComponent<Text> ().text = "Incorrect!\nGame Over\nCorrect Answer: " + answerObject.name;
						wrongText.GetComponent<Text> ().enabled = true;
						cooldown = cooldownPeriod;
					}
				}
			}

			if(itemToRemove != null)
			{
				spawnedItems.Remove(itemToRemove);
				PickSentence ();
				SpawnItem ();
				SpawnItem (answerObject);
			}
		}
	}

	void SpawnItem () {
		int ran = Random.Range (0, 5);
		GameObject itemToSpawn;
		switch(ran) {
		case 0:
			itemToSpawn = periodItem;
			break;
		case 1:
			itemToSpawn = commaItem;
			break;
		case 2:
			itemToSpawn = semicolonItem;
			break;
		case 3:
			itemToSpawn = colonItem;
			break;
		case 4:
			itemToSpawn = blankItem;
			break;
		default:
			itemToSpawn = periodItem;
			break;
		}

		SpawnItem (itemToSpawn);
	}

	void SpawnItem (GameObject itemToSpawn) {
		int xSpawn = 0;
		int ySpawn = 0;

		bool safe = false;

		while(!safe) {
			xSpawn = Random.Range (leftBorder, rightBorder + 1);
			ySpawn = Random.Range (bottomBorder, topBorder + 1);

			safe = true;
			if((currentSnakeHead.transform.position.x == xSpawn) && (currentSnakeHead.transform.position.y == ySpawn))
			{
				Debug.Log("Tried to spawn on snake: (" + currentSnakeHead.transform.position.x + "," + currentSnakeHead.transform.position.y + ") (" + xSpawn + "," + ySpawn + ")");
				safe = false;
				continue;
			}
			foreach(GameObject body in currentSnakeBodies)
			{
				if((body.transform.position.x == xSpawn) && (body.transform.position.y == ySpawn))
				{
					Debug.Log("Tried to spawn on snake: (" + currentSnakeHead.transform.position.x + "," + currentSnakeHead.transform.position.y + ") (" + xSpawn + "," + ySpawn + ")");
					safe = false;
					break;
				}
			}
			foreach(GameObject item in spawnedItems)
			{
				if((item.transform.position.x == xSpawn) && (item.transform.position.y == ySpawn))
				{
					Debug.Log ("Tried to spawn on item: (" + currentSnakeHead.transform.position.x + "," + currentSnakeHead.transform.position.y + ") (" + xSpawn + "," + ySpawn + ")");
					safe = false;
					break;
				}
			}
		}

		GameObject obj = Instantiate (itemToSpawn, new Vector2 (xSpawn, ySpawn), Quaternion.identity) as GameObject;
		obj.name = itemToSpawn.name;
		spawnedItems.Add(obj);
	}

	void PickSentence()
	{
		int ran = Random.Range (0, 20);
		string sentence;
		GameObject ans;
		/* 
		sentence = "";
		ans = gameObject;
		*/
		switch(ran)
		{
		case 0:
			sentence = "I want to wear the red shirt _ for my favorite color is red.";
			ans = commaItem;
			break;
		case 1:
			sentence = "Janie easily got an A on the test _ she studies very hard.";
			ans = semicolonItem;
			break;
		case 2:
			sentence = "As today is my birthday_ I hope I get a lot of presents; in fact, I hope I get a new car.";
			ans = commaItem;
			break;
		case 3:
			sentence = "As today is my birthday, I hope I get a lot of presents_ in fact, I hope I get a new car.";
			ans = semicolonItem;
			break;
		case 4:
			sentence = "As today is my birthday, I hope I get a lot of presents; in fact_ I hope I get a new car.";
			ans = commaItem;
			break;
		case 5:
			sentence = "The war is imminent_ the armies are prepared, and they are ready to annihilate their opponent.";
			ans = semicolonItem;
			break;
		case 6:
			sentence = "The war is imminent; the armies are prepared_ and they are ready to annihilate their opponent.";
			ans = commaItem;
			break;
		case 7:
			sentence = "The mission seemed impossible_ so we systematized everything we needed.";
			ans = commaItem;
			break;
		case 8:
			sentence = "The war is imminent; the armies are prepared, and_ they are ready to annihilate their opponent.";
			ans = blankItem;
			break;
		case 9:
			sentence = "Tony called his mom to inform her about his sister_ \"Jordan is extremely sick!";
			ans = colonItem;
			break;
		case 10:
			sentence = "Tomorrow will be Sunday_ December 2.";
			ans = commaItem;
			break;
		case 11:
			sentence = "Gold watches, for example, are going on sale_ today";
			ans = blankItem;
			break;
		case 12:
			sentence = "Yes_ I would like more water!";
			ans = commaItem;
			break;
		case 13:
			sentence = "His eyes went bad_ consequently, he had to resign his position as a proofreader.";
			ans = semicolonItem;
			break;
		case 14:
			sentence = "I invited Sara, Susan, Leon, and John to the party_ Joe, Robert, and Charles also dropped in.";
			ans = semicolonItem;
			break;
		case 15:
			sentence = "Unfortunately, we do not have enough time_ I need to visit the grocery store and the bank before going to the party today.";
			ans = semicolonItem;
			break;
		case 16:
			sentence = "Unfortunately_ we do not have enough time; I need to visit the grocery store and the bank before going to the party today.";
			ans = commaItem;
			break;
		case 17:
			sentence = "The mission seemed impossible, so_ we systematized everything we needed.";
			ans = blankItem;
			break;
		case 18:
			sentence = "The mission seemed impossible, so we systematized everything we needed_";
			ans = periodItem;
			break;
		case 19:
			sentence = "In the 19th century, intellectuals still debated whether the United States would ever produce great writers_ however, this time period is one of the most powerful eras.";
			ans = semicolonItem;
			break;
		default:
			Debug.Log ("Sentence Defaulting");
			sentence = "I want to wear the red shirt _ for my favorite color is red.";
			ans = commaItem;
			break;
		}

		questionText.GetComponent<Text> ().text = sentence;
		answerObject = ans;
	}

	void AddToSnake()
	{
		int snakeLength = currentSnakeBodies.Count + 1;
		GameObject newBody = null;
		foreach(GameObject body in currentSnakeBodies)
		{
			if(body.name == ("SnakeBody" + (snakeLength - 2))) {
				int xSpawn;
				int ySpawn;

				GameObject lead = body.GetComponent<SnakeBody> ().leader;
				Vector2 leadPos = lead.transform.position;
				Vector2 bodyPos = body.transform.position;

				if(leadPos.x == bodyPos.x){
					xSpawn = (int)bodyPos.x;
					if (leadPos.y > bodyPos.y) {
						ySpawn = (int)bodyPos.y - 1;
					} else {
						ySpawn = (int)bodyPos.y + 1;
					}
				}
				else{
					ySpawn = (int)bodyPos.y;
					if(leadPos.x > bodyPos.x) {
						xSpawn = (int)bodyPos.x - 1;
					} else {
						xSpawn = (int)bodyPos.x + 1;
					}
				}

				newBody = Instantiate (snakeBodyPrefab, new Vector2 (xSpawn, ySpawn), Quaternion.identity) as GameObject;
				newBody.name = "SnakeBody" + (snakeLength - 1);
				newBody.GetComponent<SnakeBody> ().leader = body;
			}
		}
		currentSnakeBodies.Add (newBody);
	}

	void DestroySnake()
	{
		Destroy (currentSnakeHead);
		currentSnakeHead = null;
		foreach(GameObject body in currentSnakeBodies)
		{
			Destroy (body);
		}
		currentSnakeBodies.Clear ();
	}
}
