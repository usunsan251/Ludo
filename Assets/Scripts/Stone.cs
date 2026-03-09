using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Stone : MonoBehaviour
{
    public int stoneID;
    [Header("ROUTES")]
    public Route commonRoute;
    public Route finalRoute;

    public List<Node> fullRoute = new List<Node>();
    [Header("NODES")]
    public Node startNode;

    public Node baseNode;
    public Node goalNode;
    public Node currentNode;

    int routePosition;
    int startNodeIndex;

    int steps;//rolled dice amount
    int doneSteps;

    [Header("BOOLS")]
    public bool isOut;
    bool isMoving;

    bool hasTurn;//is for huma input

    [Header("SELECTOR")]
    public GameObject selector;


    //ARC MOVEMENT
    float amplitude = 0.5f;
    float cTime = 0f;

    AudioSource audioSource;

    private void Start()
    {
        startNodeIndex = commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();

        audioSource = GetComponent<AudioSource>();

        SetSelector(false);
    }

    void CreateFullRoute()
    {
        for (int i = 0; i < commonRoute.childNodeList.Count; i++)
        {
            int tempPos = startNodeIndex + i;
            tempPos %= commonRoute.childNodeList.Count;

            fullRoute.Add(commonRoute.childNodeList[tempPos].GetComponent<Node>());

        }

        for (int i = 0; i < finalRoute.childNodeList.Count; i++)
        {
            fullRoute.Add(finalRoute.childNodeList[i].GetComponent<Node>());

        }
    }



    IEnumerator Move(int diceNumber)
    {
        if(isMoving)
        {
            yield break;
        }

        isMoving = true;

        while(steps > 0)
        {
            routePosition++;

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startPos = fullRoute[routePosition - 1].gameObject.transform.position;
            audioSource.PlayOneShot(audioSource.clip);
            //while(MoveToNextNode(nextPos, 8f)) {yield return null; }
            while (MoveInArcToNextNode(startPos, nextPos, 4f))
            {
                
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;
            
        }

        goalNode = fullRoute[routePosition];
        //CHECK POSSIBLE KICK
        if (goalNode.isTaken)
        {
            //KICK THE OTHER STONE
            goalNode.stone.ReturnToBase();
        }

        currentNode.stone = null;
        currentNode.isTaken = false;

        goalNode.stone = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        //REPORT GAMEMANAGER
        //WINCONDITION CHECK
        if (WinCondition())
        {
            GameManager.instance.ReportWinning();
        }

        //SWICH THE PLAYER
        if(diceNumber < 6)
        {
            GameManager.instance.state = GameManager.States.SWITCH_PLAYER;
        }

        else
        {
            GameManager.instance.state = GameManager.States.ROLL_DICE;
        }
        GameManager.instance.state = GameManager.States.SWITCH_PLAYER;

        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goalPos, float speed)
    {
        return goalPos != (transform.position = Vector3.MoveTowards(transform.position, goalPos, speed * Time.deltaTime));
    }

    bool MoveInArcToNextNode(Vector3 startPos, Vector3 goalPos, float speed)
    {
        cTime += speed * Time.deltaTime;
        Vector3 myPosition = Vector3.Lerp(startPos, goalPos, cTime);

        myPosition.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);

        return goalPos != (transform.position = Vector3.Lerp(transform.position, myPosition, cTime));
    }

    public bool ReturnIsOut()
    {
        return isOut;
    }

    public void LeaveBase()
    {
        steps = 1;
        isOut = true;
        routePosition = 0;
        audioSource.PlayOneShot(audioSource.clip);
        //START COROUTINE
        StartCoroutine(MoveOut());
    }

    IEnumerator MoveOut()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        while (steps > 0)
        {
            //routePosition++;

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startPos = transform.gameObject.transform.position;
            //while (MoveToNextNode(nextPos, 8f)) { yield return null; }
            while(MoveInArcToNextNode(startPos, nextPos, 4f))
            {
                yield return null;
            }



            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;
        }
       //UPDATE NODE
        goalNode = fullRoute[routePosition];
        //CHECK FOR KICKING OTHER NODE
        if (goalNode.isTaken)
        {
            //RETURN TO START BASE NODE
            goalNode.stone.ReturnToBase();
        }

        goalNode.stone = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        //REPORT BACK TO GAMEMANAGER
        GameManager.instance.state = GameManager.States.ROLL_DICE;
        isMoving = false;
    }

    public bool CheckPossibleMove(int diceNumber)
    {
        int temPos =  routePosition + diceNumber;
        if(temPos >= fullRoute.Count)
        {
            return false;
        }
        return !fullRoute[temPos].isTaken;
    }

    
    public bool CheckPossibleKick(int stoneID, int diceNumber)
    {
        int temPos = routePosition + diceNumber;
        if (temPos >= fullRoute.Count)
        {
            return false;
        }
        if (fullRoute[temPos].isTaken)
        {
            if (stoneID == fullRoute[temPos].stone.stoneID)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public void StartTheMove(int diceNumber)
    {
        steps = diceNumber;
        StartCoroutine(Move(diceNumber));
    }

    public void ReturnToBase()
    {
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        GameManager.instance.ReportTurnPossible(false);
        routePosition = 0;
        currentNode = null;
        goalNode = null;
        isOut = false;
        doneSteps = 0;

        Vector3 baseNodePos = baseNode.gameObject.transform.position;
        while(MoveToNextNode(baseNodePos, 100f))
        {
            yield return null;
        }
        GameManager.instance.ReportTurnPossible(true);
    }

    bool WinCondition()
    {
        for (int i = 0; i < finalRoute.childNodeList.Count; i++)
        {
            if (!finalRoute.childNodeList[i].GetComponent<Node>().isTaken)
            {
                return false;
            }
        }
        return true;
    }

    //-------------------------------------------------------HUMAN INPUT----------------------------------------------------

    public void SetSelector(bool on)
    {
        selector.SetActive(on);
        //THIS IS FOR HAVING THE CLICK ABILITY
        hasTurn = on;
    }

    private void OnMouseDown()
    {
        if (hasTurn)
        {
            if(!isOut)
            {
                LeaveBase();
            }
            else
            {
                StartTheMove(GameManager.instance.rolledHumanDice);
            }
            GameManager.instance.DeactivateAllSelectors();
        }
        
    }
}


