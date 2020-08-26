using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPlayer : MonoBehaviour
{
    private static readonly Random _random = new Random();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int[] randonPlay(List<int[]> posibleMoves)
    {
        System.Random ran = new System.Random();

        int pos = ran.Next(0, posibleMoves.Count - 1);

        return posibleMoves[pos];
    }

    private static int heuristic(int[,] board, int player)
    {
        int opponent = (player == Constants.W_TOKEN) ? Constants.B_TOKEN : Constants.W_TOKEN;

        int ourScore = GameFlow.score(board, player);
        int opponentScore = GameFlow.score(board, opponent);

        return (ourScore - opponentScore);
    }

    public static int[] minimaxDecision(int[,] board, int turn, List<int[]> moves)
    {
        int opponent = (turn == Constants.W_TOKEN) ? Constants.B_TOKEN : Constants.W_TOKEN;
        int bestMovalHeuristic = -99999;
        int[] bestMoval = null;
        int maxSearchLevel = 5;

        for (int i = 0; i < moves.Count; i++)
        {
            int[] move = moves[i];
            if (move[0] == Constants.BOARD_SIZE - 1 || move[0] == 0 || move[1] == 0 || move[1] == Constants.BOARD_SIZE - 1)
            {
                return move;
            }
            int[,] tmpBoard = (int[,])board.Clone();
            
            GameFlow.makeMove(tmpBoard, move[0], move[1], turn);
            int val = minimaxValue(tmpBoard, turn, opponent, 1, maxSearchLevel);
            if (val > bestMovalHeuristic)
            {
                bestMovalHeuristic = val;
                bestMoval = (int[])move.Clone();

               
            }

            if(val == bestMovalHeuristic)
            {
                System.Random ran = new System.Random();
                int desition = ran.Next(0, 1);
                if (desition == 0)
                {
                    bestMovalHeuristic = val;
                    bestMoval = (int[])move.Clone();
                }
            }
        }
        return bestMoval;
    }

    private static int minimaxValue(int[,] board, int originalTurn, int currentTurn, int currentSearchLevel, int maxSearchLevel)
    {
        int opponent = (currentTurn == Constants.W_TOKEN) ? Constants.B_TOKEN : Constants.W_TOKEN;
        if (currentSearchLevel == maxSearchLevel || GameFlow.isGameOver(board))
        {
            return heuristic(board, originalTurn);
        }
        List<int[]> moves = GameFlow.getPosibleMoves(board);
        if (moves.Count <= 0)
        {
            return minimaxValue(board, originalTurn, opponent, currentSearchLevel + 1, maxSearchLevel);
        }
        else
        {
            int bestMovalHeuristic = (originalTurn == currentTurn) ? -99999 : 99999;
            for (int i = 0; i < moves.Count; i++)
            {
                int[,] tmpBoard = (int[,])board.Clone();
                int[] move = moves[i];
                GameFlow.makeMove(tmpBoard, move[0], move[1], currentTurn);
                int val = minimaxValue(tmpBoard, originalTurn, opponent, currentSearchLevel + 1, maxSearchLevel);
                if(originalTurn == currentTurn)
                {
                    if (val > bestMovalHeuristic)
                        bestMovalHeuristic = val;

                }
                else
                {
                    if (val < bestMovalHeuristic)
                        bestMovalHeuristic = val;
                }
            }

            return bestMovalHeuristic;
            
        }
    }
}
