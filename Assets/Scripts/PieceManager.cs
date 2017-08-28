using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour 
{
	const float FASTDROPDELAY = .05F;
	const float HORIZONTALREPEATDELAY = .085F;
	
	Vector3 NEXTPIECELOCATION = new Vector3(14F, 16F, .5F);
	
	public Transform unitBlockActive;
	public Transform unitBlockNext;
	
	GameManager gameManager;
	GameBoard gameBoard;
	System.Random random;
	GameObject[] displayBlocks;
	Piece activePiece;
	
	float fastDropTime;	
	float horizontalMoveTime;
	bool hasPieceLanded;
	bool[] shuffleBag;
	char nextPieceShape;
	
	/// <summary>
	/// Run on startup.
	/// </summary>
	void Start () 
	{
		gameManager = GetComponent<GameManager>();
		gameBoard = GetComponent<GameBoard>();
		random = new System.Random();
		
		// Timing of held drop repeat is based on FASTDROPDELAY
		// Initial press will have a significantly reduced delay
		fastDropTime = FASTDROPDELAY - FASTDROPDELAY/10;
		horizontalMoveTime = HORIZONTALREPEATDELAY - HORIZONTALREPEATDELAY/10;
		hasPieceLanded = true;
		shuffleBag = new bool[7];
		nextPieceShape = GetNextPiece();
	}
	
	/// <summary>
	/// Update runs once per frame.
	/// </summary>
	void Update () 
	{
		if(hasPieceLanded && !gameManager.isGameOver)
		{
			SpawnPiece();
			nextPieceShape = GetNextPiece();
			CreateNextDisplayBlocks();
		}
	
		if(!gameManager.isGameOver)
		{
			HandleInputs();
			CheckGravity();
		}
	}
	
	/// <summary>
	/// Creates a new piece and assigns it as the active piece.
	/// Randomly chooses from a pool of 7 blocks.
	/// </summary>
	void SpawnPiece()
	{
		activePiece = ScriptableObject.CreateInstance<Piece>();
		//Debug.Log("Chosen piece is " + nextPiece);
		
		activePiece.Initialize(nextPieceShape, new Vector2(3, GameBoard.BOARDHEIGHT-3));
		hasPieceLanded = false;
		
		CreateDisplayBlocks();
	}
	
	/// <summary>
	/// Randomly chooses from 7 characters representing pieces to return.
	/// It doesn't feel very random at the moment, for some reason.
	/// </summary>
	/// <returns>
	/// Char representing the next piece.
	/// </returns>
	char GetNextPiece()
	{
		if(IsShuffleBagEmpty())
			RefillBag();
		
		bool hasFoundPiece = false;
		int newIndex = 0;
		while(!hasFoundPiece)
		{
			newIndex = random.Next(7);
			//Debug.Log("Trying piece " + newIndex);
			// If item at index of shufflebag has not been used yet, end loop
			if(!shuffleBag[newIndex])
				hasFoundPiece = true;
		}
		
		// Use the piece in shufflebag at the new index
		shuffleBag[newIndex] = true;
		
		switch(newIndex)
		{
		case 0:
			return 'I';
		case 1:
			return 'L';
		case 2:
			return 'J';
		case 3:
			return 'O';
		case 4:
			return 'S';
		case 5:
			return 'Z';
		case 6:
			return 'T';
		default:
			Debug.Log("Invalid random number detected in GetNextPiece().");
			return 'I';
		}
	}
	
	/// <summary>
	/// Determines whether this instance is shuffle bag empty.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this shuffle bag is empty; otherwise, <c>false</c>.
	/// </returns>
	bool IsShuffleBagEmpty()
	{
		foreach(bool isUsed in shuffleBag)
			if(!isUsed)
				return false;
		
		//Debug.Log("Bag is empty!");
		return true;
	}

	/// <summary>
	/// Refills the shuffle bag (setting bools to false.)
	/// </summary>
	void RefillBag ()
	{
		for(int i=0; i<shuffleBag.Length; i++)
			shuffleBag[i] = false;
	}
	
	/// <summary>
	/// Generates four cubes in the shape of the next block.
	/// </summary>
	void CreateNextDisplayBlocks()
	{		
		int[][] nextPieceData = Piece.GetPieceData(nextPieceShape, 0);
		for(int i=0; i<nextPieceData.Length; i++)
			for(int j=0; j<nextPieceData[i].Length; j++)
				if(nextPieceData[i][j] != 0)
				{
					Transform cube = (Transform)Instantiate(unitBlockNext, new Vector3(i, j, 0) + NEXTPIECELOCATION, Quaternion.identity);				
					cube.renderer.material.color = Piece.GetPieceColor(nextPieceShape);
				}
	}
	
	/// <summary>
	/// Generates four cubes in the shape of the active block.
	/// These are moved around using MoveDisplayBlock() on player input.
	/// </summary>
	void CreateDisplayBlocks()
	{
		float currentX = activePiece.GetPosition().x;
		float currentY = activePiece.GetPosition().y;
		Vector3 adjust = new Vector3(.5F, .5F, .5F);
		
		//Debug.Log("Color block here.");
		for(int i=0; i<activePiece.blocks.Length; i++)
			for(int j=0; j<activePiece.blocks[i].Length; j++)
				if(activePiece.blocks[i][j] != 0)
				{
					Transform cube = (Transform)Instantiate(unitBlockActive, new Vector3(currentX+i, currentY+j, 0) + adjust, Quaternion.identity);				
					cube.renderer.material.color = Piece.GetPieceColor(activePiece.GetShape());
				}
	}
	
	/// <summary>
	/// Handles player input.
	/// Not configured to Vertical/Horizontal axis.
	/// </summary>
	void HandleInputs()
	{
		Vector2 direction = Vector2.zero;
		
		// Timing of held drop repeat is based on FASTDROPDELAY
		// Initial press will have a significantly reduced delay
		if(Input.GetKey(KeyCode.DownArrow))
		{
			fastDropTime += Time.deltaTime;
			if(IsValidMove(Vector2.up * -1) && fastDropTime > FASTDROPDELAY)
			{
				direction = Vector2.up * -1;
				fastDropTime = 0;
				gameManager.ResetDropTimer();
			}			
		}
		else
			fastDropTime = FASTDROPDELAY - FASTDROPDELAY/10;
		
		// Same process for right and left inputs, except not tied to gravity
		if(Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
		{
			horizontalMoveTime += Time.deltaTime;
			if(IsValidMove(Vector2.right) && horizontalMoveTime > HORIZONTALREPEATDELAY)
			{
				direction = Vector2.right;
				horizontalMoveTime = 0;				
			}		
		}
		// If other direction is not pressed, then reset timing to "immediately after next press"
		else if(!Input.GetKey(KeyCode.LeftArrow))
			horizontalMoveTime = HORIZONTALREPEATDELAY - HORIZONTALREPEATDELAY/10;
		
		if(Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
		{
			horizontalMoveTime += Time.deltaTime;
			if(IsValidMove(Vector2.right * -1) && horizontalMoveTime > HORIZONTALREPEATDELAY)
			{
				direction = Vector2.right * -1;
				horizontalMoveTime = 0;						
			}
		}
		else if(!Input.GetKey(KeyCode.RightArrow))
			horizontalMoveTime = HORIZONTALREPEATDELAY - HORIZONTALREPEATDELAY/10;
		
		// Rotation consists of placing a projection in the game and confirming valid move
		// Re-creates display blocks if rotation is successful, and moves block in game space
		// TODO: Add wall kicks
		if(Input.GetKeyDown(KeyCode.Z))
		{
			if(IsValidRotation("Left"))
			{
				activePiece.RotatePiece("Left");
				ClearDisplayBlocks();
				CreateDisplayBlocks();
				// Debug.Log("Piece has been rotated left.");
			}
			else // Attempt wall kick
			{
				Vector2 validTranslation = FindValidRotation("Left");
				// If valid translation found, perform wall kick
				if(!validTranslation.Equals(Vector2.zero))
				{
					direction = validTranslation;
					activePiece.RotatePiece("Left");
					ClearDisplayBlocks();
					CreateDisplayBlocks();
					//Debug.Log("Performed wall kick");
				}
			}
		}
		
		if(Input.GetKeyDown(KeyCode.X))
		{
			if(IsValidRotation("Right"))
			{
				activePiece.RotatePiece("Right");
				ClearDisplayBlocks();
				CreateDisplayBlocks();
				// Debug.Log("Piece has been rotated right.");
			}
			else // Attempt wall kick
			{
				Vector2 validTranslation = FindValidRotation("Right");
				// If valid translation found, perform wall kick
				if(!validTranslation.Equals(Vector2.zero))
				{
					direction = validTranslation;
					activePiece.RotatePiece("Right");
					ClearDisplayBlocks();
					CreateDisplayBlocks();
					//Debug.Log("Performed wall kick");
				}
			}
		}
		
		activePiece.TransformPiece(direction);
		MoveDisplayBlock(direction);
	}
	
	/// <summary>
	/// Determines whether this instance is valid move in the specified direction.
	/// Checks each 
	/// </summary>
	/// <returns>
	/// <c>true</c> if this is a valid move; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='direction'>
	/// Block's direction to check.
	/// </param>
	bool IsValidMove(Vector2 direction)
	{
		float currentX = activePiece.GetPosition().x;
		float currentY = activePiece.GetPosition().y;
		int[][] current = activePiece.blocks;
		
		for(int i=0; i<current.Length; i++)
		{
			for(int j=0; j<current[i].Length; j++)
			{
				if(current[i][j] == 1)
				{
					// check position for valid space
					if(IsSpaceOccupied((int)(currentX + i + direction.x), (int)(currentY + j)))
						return false;
					if(IsSpaceOccupied((int)(currentX + i), (int)(currentY + j + direction.y)))
						return false;
				}
			}
		}
		return true;
	}
	
	/// <summary>
	/// Determines whether this rotation is possible without collision.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this is a valid rotation, otherwise <c>false</c>.
	/// </returns>
	/// <param name='rotation'>
	/// If set to <c>true</c> rotation.
	/// </param>
	bool IsValidRotation(string rotation)
	{
		float currentX = activePiece.GetPosition().x;
		float currentY = activePiece.GetPosition().y;
		
		// get projection by finding current rotation and adding or subtracting applied rotation
		int projectedRotation = activePiece.GetRotation();
		if(rotation.Equals("Left"))
			projectedRotation--;
		if(rotation.Equals("Right"))
			projectedRotation++;
		// rotation clamping
		if(projectedRotation > 3)
			projectedRotation -= 4;
		if(projectedRotation < 0)
			projectedRotation += 4;
		
		int[][] projection = Piece.GetPieceData(activePiece.GetShape(), projectedRotation);
		
		for(int i=0; i<projection.Length; i++)
		{
			for(int j=0; j<projection[i].Length; j++)
			{
				if(projection[i][j] == 1)
				{
					// check projection block for valid space
					if(IsSpaceOccupied((int)currentX + i, (int)currentY + j))
						return false;
				}
			}
		}
		return true;
	}
	
	/// <summary>
	/// Looks for a valid rotation translated up, down, left, and right.
	/// </summary>
	/// <returns>
	/// The valid rotation, if any. If not, returns zero vector.
	/// </returns>
	/// <param name='rotation'>
	/// Rotation.
	/// </param>
	Vector2 FindValidRotation(string rotation)
	{
		float currentX = activePiece.GetPosition().x;
		float currentY = activePiece.GetPosition().y;
		Vector2[] translations = {	new Vector2(1,0), new Vector2(-1,0),
								 	new Vector2(0,1), new Vector2(0,-1),
									new Vector2(2,0), new Vector2(-2,0),
									new Vector2(0,2), new Vector2(0,-2)};
		// get projection by finding current rotation and adding or subtracting applied rotation
		int projectedRotation = activePiece.GetRotation();
		
		if(rotation.Equals("Left"))
			projectedRotation--;
		if(rotation.Equals("Right"))
			projectedRotation++;
		// rotation clamping
		if(projectedRotation > 3)
			projectedRotation -= 4;
		if(projectedRotation < 0)
			projectedRotation += 4;
		
		int[][] projection = Piece.GetPieceData(activePiece.GetShape(), projectedRotation);
		
		for(int h=0; h<translations.Length; h++)
		{
			bool validProjection = true;
			for(int i=0; i<projection.Length; i++)
			{
				for(int j=0; j<projection[i].Length; j++)
				{
					if(projection[i][j] == 1)
					{
						// check projection block for valid space
						if(IsSpaceOccupied((int)(currentX + i + translations[h].x),(int)(currentY + j + translations[h].y)))
							validProjection = false;
					}
				}
			}
			// if it is a valid projection after checks, break loop and use current translation
			if(validProjection)
				return translations[h];
		}
		// if no projections are valid, return zero vector
		return Vector2.zero;
	}
	
	/// <summary>
	/// Determines whether specified space is occupied.
	/// Used by IsValidMove() and IsValidRotation().
	/// </summary>
	bool IsSpaceOccupied(int coordX, int coordY)
	{
		// Check left, right, bottom walls first
		if(coordX < 0 || coordX > GameBoard.BOARDWIDTH-1)
			return true;
		if(coordY < 0)
			return true;
		
		// Check other blocks in game board
		if(gameBoard.IsSpaceOccupied(coordX, coordY))
			return true;
		
		return false;
	}
	
	/// <summary>
	/// Clears the display blocks from screen.
	/// </summary>
	void ClearDisplayBlocks()
	{
		displayBlocks = GameObject.FindGameObjectsWithTag("Active Block");
		foreach(GameObject o in displayBlocks)
		{
			Destroy(o);
		}
	}
	
	/// <summary>
	/// Clears the next display blocks from screen.
	/// </summary>
	void ClearNextDisplayBlocks()
	{
		displayBlocks = GameObject.FindGameObjectsWithTag("Next Block");
		foreach(GameObject o in displayBlocks)
		{
			Destroy(o);
		}
	}
	
	/// <summary>
	/// Moves the display block in the specified direction.
	/// </summary>
	/// <param name='direction'>
	/// Direction to move block in.
	/// </param>
	void MoveDisplayBlock(Vector2 direction)
	{
		displayBlocks = GameObject.FindGameObjectsWithTag("Active Block");
		foreach(GameObject o in displayBlocks)
		{
			o.transform.Translate(direction);
		}
	}
	
	/// <summary>
	/// Handles gravity, checking if it is time to drop by one space
	/// GameManager keeps track of required time and time since last drop.
	/// If drop cannot happen, lock into place and add block to GameBoard.
	/// </summary>
	void CheckGravity()
	{
		Vector2 downOneUnit = Vector2.up * -1;
		
		if(gameManager.IsTimeToDrop())
		{
			if(IsValidMove(downOneUnit))
			{
				activePiece.TransformPiece(downOneUnit);
				MoveDisplayBlock(downOneUnit);
			}
			else
			{
				gameBoard.AddBlockToBoard(activePiece);
				ClearDisplayBlocks();
				ClearNextDisplayBlocks();
				hasPieceLanded = true;
			}
		}
	}
}
