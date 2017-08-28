using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public const int POINTS_SINGLE = 100;
	public const int POINTS_DOUBLE = 300;
	public const int POINTS_TRIPLE = 600;
	public const int POINTS_TETRIS = 1000;
	
	GameBoard gameBoard;
	
	int currentLevel;
	int currentScore;
	int currentLines;
	float timePerDrop;
	float timeSinceLastDrop;
	
	bool isTimeToDrop;
	public bool isGameOver;
	
	/// <summary>
	/// Run on startup.
	/// </summary>
	void Start () 
	{
		gameBoard = GetComponent<GameBoard>();
		
		currentLevel = 0;
		currentScore = 0;
		currentLines = 0; 
		
		timePerDrop = .5F - (currentLevel * .02F);
		timeSinceLastDrop = 0F;
		isTimeToDrop = false;
	}
	
	/// <summary>
	/// Update runs once per frame.
	/// </summary>
	void Update () 
	{			
		if(!isGameOver)
		{
			timeSinceLastDrop += Time.deltaTime;
			if(timeSinceLastDrop > timePerDrop)
			{
				timeSinceLastDrop -= timePerDrop;
				isTimeToDrop = true;
			}
		}
	}
	
	/// <summary>
	/// Handles GUI here.
	/// </summary>
	void OnGUI()
	{
		string label = "Level: " + currentLevel + 
			"\nScore: " + currentScore +
			"\nLines: " + currentLines;
		if(isGameOver)
			label = "Game over - play again?";
		
		// Create box with label showing game info or "Play Again"
		if (GUI.Button (new Rect (10,10,200,120), label))
			ResetGame ();
		
		// Create an exit button
		if (GUI.Button (new Rect (10, Screen.height-60,50,50), "Quit"))
			Application.Quit();
	}
	
	/// <summary>
	/// Resets the game.
	/// </summary>
	void ResetGame ()
	{
		gameBoard.ResetBoard();
		isGameOver = false;
		currentLevel = 0;
		currentScore = 0;
		currentLines = 0;
	}
	
	/// <summary>
	/// Increases game level.
	/// Caps at level 20, upon which drop speed is doubled.
	/// Formula is [500 - (level * 20)]ms, and set at 100ms at lv 20 and higher.
	/// </summary>
	void LevelUp()
	{
		currentLevel++;
		if(currentLevel < 20)
			timePerDrop = .5F - (currentLevel * .02F);
		if(currentLevel >= 20)
			timePerDrop = .1F;
	}
	
	/// <summary>
	/// Determines whether block should drop by one unit now.
	/// Stores variable in boolean so time can continue to count,
	/// and strange timing dependencies based on msSinceLastDrop being reset do not occur.
	/// Resets the timeToDrop bool after using it.
	/// </summary>
	/// <returns>
	/// <c>true</c> if it's time to drop; otherwise, <c>false</c>.
	/// </returns>
	public bool IsTimeToDrop()
	{
		if(isTimeToDrop)
		{
			isTimeToDrop = false;
			return true;
		}
		return false;			
	}
	
	/// <summary>
	/// Resets the drop timer, used when player presses Down.
	/// As a safety measure, also sets timeToDrop to false.
	/// </summary>
	public void ResetDropTimer()
	{
		timeSinceLastDrop = 0F;
		isTimeToDrop = false;
	}
	
	/// <summary>
	/// Adds the lines to line total.
	/// Also handles adding points to score total.
	/// </summary>
	/// <param name='linesCleared'>
	/// Lines cleared.
	/// </param>
	public void AddLines(int linesCleared)
	{
		currentLines += linesCleared;
		
		if(currentLines/10 > currentLevel)
			LevelUp();
		
		AddPoints(linesCleared);
	}
	
	/// <summary>
	/// Adds points based on lines cleared.
	/// </summary>
	/// <param name='linesCleared'>
	/// Lines cleared.
	/// </param>
	void AddPoints(int linesCleared)
	{
		if(linesCleared == 1)
			currentScore += POINTS_SINGLE;
		if(linesCleared == 2)
			currentScore += POINTS_DOUBLE;
		if(linesCleared == 3)
			currentScore += POINTS_TRIPLE;
		if(linesCleared == 4)
			currentScore += POINTS_TETRIS;
	}	
}
