using UnityEngine;
using System.Collections;

public class Piece : ScriptableObject 
{
	// All pieces are contained in a 4x4 array
	public const int PIECESIZE = 4;
	
	// Upper left cell of 4x4 block
	Vector2 piecePosition;
	public int[][] blocks;
	
	// I, L, J, S, Z, T, O are valid shapes
	char shape;
	// Rotation values can be from 0-3
	int pieceRotation;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Piece"/> class.
	/// </summary>
	/// <param name='type'>
	/// Shape of block.
	/// </param>
	public void Initialize(char type, Vector2 spawnPoint)
	{
		piecePosition = spawnPoint;
		
		// Initialize blocks array to 0
		blocks = new int[PIECESIZE][];
		for(int i=0; i<blocks.Length; i++)
		{
			blocks[i] = new int[PIECESIZE];
			for(int j=0; j<blocks[i].Length; j++)
			{
				blocks[i][j] = 0;
			}
		}
		
		blocks = GetPieceData(type, 0);
		shape = type;
	}
	
	public void TransformPiece(Vector2 move)
	{
		piecePosition += move;
	}
	
	public void RotatePiece(string direction)
	{
		if(direction.Equals("Left"))
			pieceRotation -= 1;
		else if (direction.Equals("Right"))
			pieceRotation += 1;
		else
			Debug.LogError("Invalid direction inputted in RotatePiece.");
		
		// Clamp rotation value
		if(pieceRotation > 3)
			pieceRotation -= 4;
		if(pieceRotation < 0)
			pieceRotation += 4;
		
		blocks = GetPieceData(shape, pieceRotation);
	}
	
	// Getters & setters
	public Vector2 GetPosition()
	{
		return piecePosition;
	}
	public char GetShape()
	{
		return shape;
	}
	public int GetRotation()
	{
		return pieceRotation;
	}
	
	/// <summary>
	/// Returns a color based on the type of block.
	/// I is light blue. O is yellow. T is purple.
	/// J is dark blue. L is orange.
	/// S is lime green. Z is red.
	/// </summary>
	public static Color GetPieceColor(char type)
	{
		Color color;
		switch(type)
		{
		case 'I':
			color = new Color(.3F,1,1); // cyan
			break;
		case 'J':
			color = new Color(.2F,.2F,1); // slightly lighter blue
			break;
		case 'L':
			color = new Color(1,.66F,0); // orange
			break;
		case 'O':
			color = new Color(1,1,.3F); // yellow
			break;
		case 'S':
			color = new Color(.3F,1,.3F); // lighter green;
			break;
		case 'Z':
			color = new Color(1F,.3F,.3F); // lighter red;
			break;
		case 'T':
			color = new Color(.75F,.3F,1); // purple
			break;
		default:
			Debug.LogError("Invalid shape entered in GetPieceColor.");
			color = Color.black;
			break;				
		}
		
		//Debug.Log ("Color of block is " + color);
		return color;
	}
	
	/// <summary>
	/// Retrieves block shape and orientation data from predefined tables.
	/// Public access for creating projections for collision detection.
	/// </summary>
	/// <returns>
	/// 2D array storing block data.
	/// </returns>
	/// <param name='type'>
	/// Shape of block by letter.
	/// </param>
	/// <param name='orientation'>
	/// Orientation of block, values 0-3.
	/// </param>
	public static int[][] GetPieceData(char type, int orientation)
	{
		int[][] pieceData = new int[PIECESIZE][];
		// Set blocks local values based on block shape
		// Set a number for coloring in GameBoard
		switch (type)
		{
			case 'I':
				if(orientation == 0){
					pieceData[0] = new int[]{0,1,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{0,1,0,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 1){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{1,1,1,1};
					pieceData[2] = new int[]{0,0,0,0};
					pieceData[3] = new int[]{0,0,0,0};
				}else if(orientation == 2){
					pieceData[0] = new int[]{0,0,1,0};
					pieceData[1] = new int[]{0,0,1,0};
					pieceData[2] = new int[]{0,0,1,0};
					pieceData[3] = new int[]{0,0,1,0};
				}else if(orientation == 3){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,0,0};
					pieceData[2] = new int[]{1,1,1,1};
					pieceData[3] = new int[]{0,0,0,0};}
				break;
			case 'L':
				if(orientation == 0){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{0,1,0,0};
					pieceData[3] = new int[]{0,1,1,0};
				}else if(orientation == 1){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,0,0};
					pieceData[2] = new int[]{1,1,1,0};
					pieceData[3] = new int[]{1,0,0,0};
				}else if(orientation == 2){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{1,1,0,0};
					pieceData[2] = new int[]{0,1,0,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 3){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,1,0};
					pieceData[2] = new int[]{1,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};}
				break;
			case 'J':
				if(orientation == 0){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,1,0};
					pieceData[2] = new int[]{0,1,0,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 1){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,0,0};
					pieceData[2] = new int[]{1,1,1,0};
					pieceData[3] = new int[]{0,0,1,0};
				}else if(orientation == 2){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{0,1,0,0};
					pieceData[3] = new int[]{1,1,0,0};
				}else if(orientation == 3){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{1,0,0,0};
					pieceData[2] = new int[]{1,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};}
				break;
			case 'S':
				if(orientation == 0){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,0,1,0};
				}else if(orientation == 1){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,0,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{1,1,0,0};
				}else if(orientation == 2){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{1,0,0,0};
					pieceData[2] = new int[]{1,1,0,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 3){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,1,0};
					pieceData[2] = new int[]{1,1,0,0};
					pieceData[3] = new int[]{0,0,0,0};}
				break;
			case 'Z':
				if(orientation == 0){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,1,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 1){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,0,0};
					pieceData[2] = new int[]{1,1,0,0};
					pieceData[3] = new int[]{0,1,1,0};
				}else if(orientation == 2){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{1,1,0,0};
					pieceData[3] = new int[]{1,0,0,0};
				}else if(orientation == 3){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{1,1,0,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};}
				break;
			case 'T':
				if(orientation == 0){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 1){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,0,0,0};
					pieceData[2] = new int[]{1,1,1,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 2){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{1,1,0,0};
					pieceData[3] = new int[]{0,1,0,0};
				}else if(orientation == 3){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,0,0};
					pieceData[2] = new int[]{1,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};}
				break;
			case 'O':
				if(orientation == 0){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,1,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};
				}else if(orientation == 1){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,1,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};
				}else if(orientation == 2){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,1,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};
				}else if(orientation == 3){
					pieceData[0] = new int[]{0,0,0,0};
					pieceData[1] = new int[]{0,1,1,0};
					pieceData[2] = new int[]{0,1,1,0};
					pieceData[3] = new int[]{0,0,0,0};}
				break;
			default:
				Debug.LogError("Invalid block type inputted.");
				break;
		}
		
		if(orientation < -1 || orientation > 3)
			Debug.LogError("Invalid orientation given.");
			
		return pieceData;
	}
}
