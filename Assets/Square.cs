using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public Transform wtokenObj;
    public Transform btokenObj;
    public Transform posiblePositionObj;
    public Transform finishObj;
    private Transform posiblePositionObjClone;

    public int x;
    public int y;
    int tokenType;

    public Square(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // Start is called before the first frame update
    void Start()
    {
        //posiblePositionObjClone =  Instantiate(posiblePositionObj, transform.position, posiblePositionObj.rotation);
        //posiblePositionObjClone.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {

        if (TokenType != Constants.NO_TOKEN)
        {
            return;
        }
        else if (!GameFlow.makeMove(GameFlow.Board , x, y, GameFlow.Turn))
        {
            return;
        }
        if (!GameFlow.changeTurn())
        {
            GameFlow.finishGame(finishObj);
        }
        Debug.Log(x);
        Debug.Log(y);
    }

    public bool flipToken(int tokenType)
    {
        if(TokenType == Constants.NO_TOKEN)
        {
            return false;
        }
        else if(TokenType == Constants.W_TOKEN)
        {
            Destroy(wtokenObj);
            TokenType = Constants.B_TOKEN;
            Instantiate(btokenObj, transform.position, btokenObj.rotation);
        }
        else
        {
            Destroy(btokenObj);
            TokenType = Constants.W_TOKEN;
            Instantiate(wtokenObj, transform.position, wtokenObj.rotation);
        }
        return true;
    }

    public bool instantiateToken(int tokenType)
    {
        if(tokenType != Constants.W_TOKEN && tokenType != Constants.B_TOKEN)
        {
            return false;
        }
        TokenType = tokenType;
        if(tokenType == Constants.W_TOKEN)
        {
            Instantiate(wtokenObj, transform.position, wtokenObj.rotation);
        }
        else
        {
            Instantiate(btokenObj, transform.position, btokenObj.rotation);
        }
        return true;
    }

    public void setPosibleSolution()
    {
      // posiblePositionObjClone.gameObject.SetActive(true);
        


    }

    public void removePosibleSolution()
    {
      //  posiblePositionObjClone.gameObject.SetActive(false);

    }

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int TokenType { get => tokenType; set => tokenType = value; }
}
