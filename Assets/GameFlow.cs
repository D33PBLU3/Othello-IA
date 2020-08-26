using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public Transform squareObj;
    public static Transform turnObj;

    private static int turn = Constants.B_TURN;
    private static int totalWTokens;
    private static int totalBTokens;
    private static int totalTurns = 1;
    private static Dictionary<Transform, List<Transform>> posibleSolutions = new Dictionary<Transform, List<Transform>>();
    private static Transform[,] physicalBoard = new Transform[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
    private static int[,] board = new int[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
    private static int[,] posibleDirections = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 }, { -1, -1 } };
    // Start is called before the first frame update
    void Start()
    {
        totalBTokens = 0;
        totalWTokens = 0;
        initSquares();
        initTokens();
        List<int[]> moves = getPosibleMoves(board);
        int cmoves = moves.Count;
        if(cmoves == 0)
        {
            return;
        }
        else
        {
            return;
        }
    }

   
    private void initSquares()
    {
        int boardCoordY = 0;
        int boardCoordX = 0;
        for (float y = -4; boardCoordY < Constants.BOARD_SIZE; y += 1f, boardCoordY++)
        {
            for (float x = 4; boardCoordX < Constants.BOARD_SIZE; x -= 1f, boardCoordX++)
            {
                Transform squareObjClone = Instantiate(squareObj, new Vector2(x, y), squareObj.rotation);
                squareObjClone.GetComponent<Square>().x = boardCoordX;
                squareObjClone.GetComponent<Square>().y = boardCoordY;
                physicalBoard[boardCoordX, boardCoordY] = squareObjClone;
                board[boardCoordX, boardCoordY] = Constants.NO_TOKEN;

            }
            boardCoordX = 0;
        }
    }

    private void initTokens()
    {
        board[Constants.BOARD_SIZE / 2, Constants.BOARD_SIZE / 2] = Constants.W_TOKEN;
        board[(Constants.BOARD_SIZE / 2) - 1, Constants.BOARD_SIZE / 2] = Constants.B_TOKEN;
        board[Constants.BOARD_SIZE / 2, (Constants.BOARD_SIZE / 2) - 1] = Constants.B_TOKEN;
        board[(Constants.BOARD_SIZE / 2) - 1, (Constants.BOARD_SIZE / 2) - 1] = Constants.W_TOKEN;
        refreshScreen();
        totalWTokens = 2;
        totalBTokens = 2;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static bool changeTurn()
    {

     
        if(totalTurns == (Constants.BOARD_SIZE * Constants.BOARD_SIZE) - 4)
        {
            refreshScreen();
            return false;
        }
            
        if (turn == Constants.W_TURN)
        {
            turn = Constants.B_TURN;
        }
        else
        {
            turn = Constants.W_TURN;
        }
        totalTurns++;
        List<int[]> moves = getPosibleMoves(board);
        int cmoves = moves.Count;
        if (cmoves == 0)
        {
            totalTurns--;
            changeTurn();
        }
        refreshScreen();
        if (turn == Constants.W_TURN)
        {
            int[] move = IAPlayer.minimaxDecision(board, turn, moves);
            if(makeMove(GameFlow.board, move[0], move[1], turn))
            {
                changeTurn();
            }
        }
        return true;
    }

    private static void setInPosibleSolutions(Transform squareObj, List<Transform> validSquares)
    {
        List<Transform> squaresToFlip = null;
        if (!posibleSolutions.ContainsKey(squareObj))
        {
            squareObj.GetComponent<Square>().setPosibleSolution();
            posibleSolutions.Add(squareObj, validSquares);
        }
        else
        {
            if (posibleSolutions.TryGetValue(squareObj, out squaresToFlip))
            {
                squaresToFlip.AddRange(validSquares);
            }
        }
    }

    public static bool isValid(int x, int y, int tokenType)
    {

        Transform squareObj = physicalBoard[x, y];
        bool changed = false;
        Transform currentSquareObj2 = physicalBoard[5, 7];
        int c = currentSquareObj2.GetComponent<Square>().TokenType;
        List<Transform> validSquares = new List<Transform>();
        Debug.Log(c);
        if (squareObj.GetComponent<Square>().TokenType != Constants.NO_TOKEN)
            return false;
        
        for (int i = 0; i < posibleDirections.Length / 2; i++)
        {
            int xOff = posibleDirections[i, 0];
            int yOff = posibleDirections[i, 1];
            validSquares = new List<Transform>();
            int check = 0;
            for (int checkX = x + xOff, checkY = y + yOff; checkX >= 0 && checkX < Constants.BOARD_SIZE
                && checkY >= 0 && checkY < Constants.BOARD_SIZE; checkY += yOff, checkX += xOff)
            {
                Transform currentSquareObj = physicalBoard[checkX, checkY];
                if (currentSquareObj.GetComponent<Square>().TokenType == Constants.NO_TOKEN ||
                 (currentSquareObj.GetComponent<Square>().TokenType == tokenType && check == 0))
                {
                    break;
                }
                else if (currentSquareObj.GetComponent<Square>().TokenType == tokenType)
                {
                    setInPosibleSolutions(squareObj, validSquares);
                    changed = true;
                    break;
                }
                validSquares.Add(currentSquareObj);
                check++;

            }
        }
        return changed;
    }
   
    public static bool isValid(int x, int y, int[,] board, int currentTurn) {

        for (int i = 0; i < posibleDirections.Length/2; i++)
        {
            int xOff = posibleDirections[i, 0];
            int yOff = posibleDirections[i, 1];
            if (board[x, y] != Constants.NO_TOKEN)
                return false;
            for (int checkX = x + xOff, checkY = y + yOff; checkX >= 0 && checkX < Constants.BOARD_SIZE
                && checkY >= 0 && checkY < Constants.BOARD_SIZE; checkY += yOff, checkX += xOff)
            {
                if (board[checkX, checkY] == Constants.NO_TOKEN ||
                 board[checkX, checkY] == currentTurn && (checkX == x + xOff && checkY == y + yOff))
                {
                    break;
                }
                else if (board[checkX, checkY] == currentTurn)
                {

                    return true;
                }
            }
        }
        return false;
    }

    public static void finishGame(Transform finishObj)
    {
        Instantiate(finishObj, new Vector2(5, 4), finishObj.rotation);
    }

    public static bool makeMove(int[,] board, int x, int y, int currentTurn)
    {
        bool isValid = false;
        for (int i = 0; i < posibleDirections.Length/2; i++)
        {
            int xOff = posibleDirections[i, 0];
            int yOff = posibleDirections[i, 1];
            List<int[]> tokensToFlip = new List<int[]>();
            tokensToFlip.Add(new int[2] { x, y });
            for (int checkX = x + xOff, checkY = y + yOff; checkX >= 0 && checkX < Constants.BOARD_SIZE
                && checkY >= 0 && checkY < Constants.BOARD_SIZE; checkY += yOff, checkX += xOff)
            {
                if (board[checkX, checkY] == Constants.NO_TOKEN ||
                 board[checkX, checkY] == currentTurn && (checkX == x + xOff && checkY == y + yOff))
                {
                    break;
                }
                else if (board[checkX, checkY] == currentTurn)
                {
                    flip(board, tokensToFlip, currentTurn);
                    isValid = true;
                    break;
                }
                tokensToFlip.Add(new int[2] { checkX, checkY });
            }
        }
        return isValid;
    }

    public static bool flip(int[,] board, List<int[]> tokensToFlip, int currentTurn)
    {
        foreach (int[] token in tokensToFlip)
        {
            board[token[0], token[1]] = currentTurn;
        }
        
        return true;
    }
    

    public static int score(int[,] board, int player)
    {
        int score = 0;
        for (int i = 0; i < Constants.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Constants.BOARD_SIZE; j++)
            {
                if(board[i,j] == player)
                {
                    score++;
                }
            }
        }

        return score;
    }

    public static List<int[]> getPosibleMoves(int[,] board)
    {
        List<int[]> solutions = new List<int[]>();
       
        for (int i = 0; i < Constants.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Constants.BOARD_SIZE; j++)
            {
                if (isValid(i, j, board, turn))
                {
                    solutions.Add(new int[] { i, j });
                  
                }
            }
        }
        return solutions;
    }

    public static void refreshScreen()
    {
        for (int i = 0; i < Constants.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Constants.BOARD_SIZE; j++)
            {
                if (board[i,j] != Constants.NO_TOKEN )
                {
                    Transform squareObj = physicalBoard[i, j];
                    squareObj.GetComponent<Square>().instantiateToken(board[i, j]);
                }
            }
        }
    }


    public static bool checkPosibleSolutions()
    {
        bool haveSolutions = false;
        for (int i = 0; i < Constants.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Constants.BOARD_SIZE; j++)
            {
                if (isValid(i, j, turn))
                {
                    haveSolutions = true;
                }
            }
        }
        return haveSolutions;
    }

    public static bool TryFlipTokens(Transform squareObj)
    {
        List<Transform> squaresToFlip = null;
        if (posibleSolutions.TryGetValue(squareObj, out squaresToFlip))
        {
            foreach (Transform squareToFlip in squaresToFlip)
            {
                squareToFlip.GetComponent<Square>().flipToken(Turn);
            }

            if (Turn == Constants.W_TOKEN)
            {
                totalWTokens += squaresToFlip.Count;
            }
            else
            {
                totalBTokens += squaresToFlip.Count;
            }
            removePosibleSolutions();
            return true;
        }
        return false;
       
    }

    public static bool isGameOver(int[,] board)
    {
        for (int i = 0; i < Constants.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Constants.BOARD_SIZE; j++)
            {
                if (board[i,j] == Constants.NO_TOKEN)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static void removePosibleSolutions()
    {
        foreach (KeyValuePair<Transform, List<Transform>> entry in posibleSolutions)
        {
            entry.Key.GetComponent<Square>().removePosibleSolution();
        }
        posibleSolutions.Clear();
    }
    public static int Turn { get => turn; set => turn = value; }
    public static int TotalWTokens { get => totalWTokens; set => totalWTokens = value; }
    public static int TotalBTokens { get => totalBTokens; set => totalBTokens = value; }
    public static int[,] Board { get => board; set => board = value; }
}
