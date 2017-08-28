using UnityEngine;
using System.Collections;

public class GameBoard : MonoBehaviour 
{
	// A Tetris board is 10x22 units, two of which are not displayed
	public const int BOARDWIDTH = 10;
	public const int BOARDHEIGHT = 22;
		
	public Transform unitBlockStage;
	
	GameManager gameManager;
	
	int[][] board;
	
	/// <summary>
	/// Run on startup.
	/// </summary>
	void Start () 
	{
		gameManager = GetComponent<GameManager>();
		
		board = new int[BOARDWIDTH][];
		for(int i=0; i<BOARDWIDTH; i++)
			board[i] = new int[BOARDHEIGHT];
	}
		
	/// <summary>
	/// Adds the block to the GameBoard.
	/// Calls AddDisplayBlock() afterwards to display it in game world.
	/// </summary>
	public void AddBlockToBoard(Piece piece)
	{
		Vector3 adjust = new Vector3(.5F, .5F, .5F);
		Vector2 startPosition = piece.GetPosition();
		int coordX;
		int coordY;
		
		// Copy over blocks from Piece object to GameBoard
		for(int i=0; i<piece.blocks.Length; i++)
			for(int j=0; j<piece.blocks[i].Length; j++)
				if(piece.blocks[i][j] == 1)
				{
					coordX = (int)(startPosition.x + i);
					coordY = (int)(startPosition.y + j);
					board[coordX][coordY] = 1;
					// Debug.Log(" Added block to location " + coordX + ", " + coordY);
				
					// Create a display block at the specified location.
					Transform cube = (Transform)Instantiate(unitBlockStage, new Vector3(coordX, coordY, 0) + adjust, Quaternion.identity);
					cube.renderer.material.color = Piece.GetPieceColor(piece.GetShape());
				}
		
		HandleLines();
		CheckIfGameOver();
	}
	
	/// <summary>
	/// Check if any lines were created for deletion, and if so, delete them.\
	/// Handle incrementing score here as well.
	/// </summary>
	void HandleLines()
	{
		int linesDeleted = 0;
		// Check each line, top-down
		for(int i=BOARDHEIGHT-2; i>-1; i--)
		{
			if(IsLineFull(i))
			{
				DeleteLine(i);
				linesDeleted++;
			}
		}
		gameManager.AddLines(linesDeleted);
	}
	
	/// <summary>
	/// Determines whether the specified line number is full or not.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this line is filled; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='lineNumber'>
	/// Line number (1 is the bottom line, 20 the top.)
	/// </param>
	bool IsLineFull(int lineNumber)
	{
		for(int i=0; i<BOARDWIDTH; i++)
			if(board[i][lineNumber] == 0)
				return false;
		
		//Debug.Log ("A line has been found!");
		return true;
	}
	
	/// <summary>
	/// Deletes the line specified by lineNumber and shifting down all rows.
	/// Redraw the game board afterwards.
	/// </summary>
	/// <param name='lineNumber'>
	/// Line number to delete.
	/// </param>
	void DeleteLine(int lineNumber)
	{
		Debug.Log ("Deleting line number " + lineNumber);
		// Copy the game board arrays onto themselves,
		// shifting over data by 1 and using deleted line as a starting index
		for(int i=0; i<board.Length; i++)
		{
			Debug.Log ("Details: i="+i+", lineNumber="+lineNumber+", board[lineNumber].Length="+board[i].Length);
			System.Array.Copy (board[i], lineNumber+1, board[i], lineNumber, board[i].Length - lineNumber - 1);
		}
			
		GameObject[] stageBlocks = GameObject.FindGameObjectsWithTag("Stage Blocks");
		
		// Destroy stage blocks based on height, following that move blocks above the height line down by one
		// Certainly not the most elegant system
		foreach(GameObject o in stageBlocks)
			if(o.transform.position.y > lineNumber)
				if(o.transform.position.y < lineNumber + 1)
					Destroy(o);
		
		stageBlocks = GameObject.FindGameObjectsWithTag("Stage Blocks");
		foreach(GameObject o in stageBlocks)
			if(o.transform.position.y > lineNumber)
				o.transform.Translate(Vector3.down);
	}
	
	/// <summary>
	/// Checks if the game is over.
	/// The condition for ending the game is to have any block above row 20 filled.
	/// </summary>
	void CheckIfGameOver()
	{
		for(int i=0; i<BOARDWIDTH; i++)
			for(int j=20; j<BOARDHEIGHT; j++)
				if(board[i][j] == 1)
					gameManager.isGameOver = true;
	}
	
	/// <summary>
	/// Determines whether space at given coordinates is occupied or not.
	/// </summary>
	public bool IsSpaceOccupied(int coordX, int coordY)
	{
		if(coordX > -1 && coordY > -1)
			return board[coordX][coordY] == 1;
		else // Out of bounds
			return true;
	}
	
	/// <summary>
	/// Resets the game.
	/// </summary>
	public void ResetBoard()
	{
		board = new int[BOARDWIDTH][];
		for(int i=0; i<BOARDWIDTH; i++)
			board[i] = new int[BOARDHEIGHT];
		
		GameObject[] stageBlocks = GameObject.FindGameObjectsWithTag("Stage Blocks");
		foreach(GameObject o in stageBlocks)
			Destroy(o);
	}
}
