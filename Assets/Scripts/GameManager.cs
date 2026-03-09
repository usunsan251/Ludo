using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    public static GameManager instance;

    [System.Serializable]
    public class Entity
    {
        public string playerName;
        public Stone[] myStones;
        public bool hasturn;
        public enum PlayerTypes
        {
            HUMAN,
            CPU,
            NO_PLAYER
        }
        public PlayerTypes playerType;
        public bool hasWon;
    }

    public List<Entity> playerList = new List<Entity>();

    //STATEMACHINE
    public enum States
    {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER
    }
    public States state;

    public int activePlayer;
    bool switchingPlayer;
    bool turnPossible = true;

    //HUMAN INPUTS
    //GAMEOBJECT FOR OUR BUTTON
    public GameObject rollButton;
    [HideInInspector]public int rolledHumanDice;

    public Dice dice;

    void Awake()
    {
        instance = this;

        for (int i = 0; i < playerList.Count; i++)
        {
            if (SaveSettings.players[i] == "HUMAN")
                playerList[i].playerType = Entity.PlayerTypes.HUMAN;
            if(SaveSettings.players[i] == "CPU")
                playerList[i].playerType = Entity.PlayerTypes.CPU;
        }
    }

    void Start()
    {
        ActivateButton(false);

        int ramdomPlayer = Random.Range(0, playerList.Count);
        activePlayer = ramdomPlayer;
        VirtualCameraSelector virtualCameraSelector;
        GameObject obj = GameObject.Find("VirtualCameraSelector");
        virtualCameraSelector = obj.GetComponent<VirtualCameraSelector>();

        Debug.Log("Before setting _currentCamera: " + virtualCameraSelector._currentCamera);
        Debug.Log("Active player: " + activePlayer);
        VirtualCameraSelector.Instance.SetCurrentCamera(activePlayer);

        Debug.Log("After setting _currentCamera: " + virtualCameraSelector._currentCamera);
        //VirtualCameraSelector.Instance.VcamChange();
        Info.Instance.ShowMessage(playerList[activePlayer].playerName + " starts first!");
        Debug.Log(activePlayer);
        Debug.Log(virtualCameraSelector._currentCamera);
    }


    void Update()
    {
        if (playerList[activePlayer].playerType == Entity.PlayerTypes.CPU)
        {
            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if (turnPossible)
                        {
                            StartCoroutine(RollDiceDelay());
                            state = States.WAITING;
                        }
                        
                      
                    }

                    break;
                case States.WAITING:
                    {
                        
                    }

                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }

                    break;

            }
        }

        if (playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN)
        {
            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if (turnPossible)
                        {
                            //DEACTIVATE HIGHLIGHITS
                            ActivateButton(true);
                            state = States.WAITING;
                        }


                    }

                    break;
                case States.WAITING:
                    {

                    }

                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            //DEACTIVATE BUTTON
                            //DEACTIVATE HIGHLIGHTS

                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }

                    break;

            }
        }
    }

    void CPUDice()
    {
        dice.RollDice();
    }

    public void RollDice(int _diceNumber)// CALL THIS FROM DICE
    {
        int diceNumber = _diceNumber;//Random.Range(1, 7);
        if (playerList[activePlayer].playerType == Entity.PlayerTypes.CPU)
        {
            
            if (diceNumber == 6)
            {
                //check the start node
                CheckStartNode(diceNumber);
            }

            if (diceNumber < 6)
            {
                //check for kick
                MoveAStone(diceNumber);
            }
           
        }

        if (playerList[activePlayer].playerType == Entity.PlayerTypes.HUMAN)
        {
            rolledHumanDice = _diceNumber;
            HumanRollDice();
        }
        Debug.Log("dice rolled number" + diceNumber);
        Info.Instance.ShowMessage(playerList[activePlayer].playerName + " has rolled " + diceNumber);
    }

    IEnumerator RollDiceDelay()
    {
        yield return new WaitForSeconds(2);
        //RollDice();
        CPUDice();

    }

    void CheckStartNode(int diceNumber)
    {
        //IS ANYONE ON THE START NODE
        bool startNodeFull = false;
        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startNodeFull = true;
                break;//WE ARE DONE HERE WE FOUND A MATCH
            }
        }
        if(startNodeFull)
        {
            //MOVE A STONE
            MoveAStone(diceNumber);
            Debug.Log("The Start Node is Full");
        }
        else//STARTNODE IS EMPTY
        {
            //IF AT LEAST ONE IS INSIDE THE BASE
            for (int i = 0;i < playerList[activePlayer].myStones.Length; i++)
            {
                if (!playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    //LEAVE THE BASE
                    playerList[activePlayer].myStones[i].LeaveBase();
                    state = States.WAITING;
                    return;
                }
               
            }
            //MOVE A STONE
            MoveAStone(diceNumber);
        }
    }

    void MoveAStone(int diceNumber)
    {
        List <Stone> movableStones = new List <Stone>();
        List<Stone> movekickStones = new List<Stone>();

        
        //KILL THE LIST
        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                //CHECK FOR POSSIBLE KICK
                if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID, diceNumber))
                {
                    movekickStones.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }

                //CHECK FOR POSSIBLE MOVE
                if (playerList[activePlayer].myStones[i].CheckPossibleMove(diceNumber))
                {
                    movableStones.Add(playerList[activePlayer].myStones[i]);
                }
            }
        }
    
        //PERFOM KICK IF POSSIBLE
        if(movekickStones.Count > 0)
        {
            int num = Random.Range(0, movekickStones.Count);
            movekickStones[num].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }

        //PERFOM MOVE IF POSSIBLE
        if (movableStones.Count > 0)
        {
            int num = Random.Range(0, movableStones.Count);
            movableStones[num].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }
        //NONE IS POSSIBLE
        //SWITHING THE PLAY
        state = States.SWITCH_PLAYER;
        Debug.Log("Should switch Player");
    }

    IEnumerator SwitchPlayer()
    {
        if (switchingPlayer)
        {
            yield break;
        }

        Debug.Log("Switching Player");
        switchingPlayer = true;

        yield return new WaitForSeconds(2);
        //SET NEXT PLAYER
        SetNextActivePlayer();

        switchingPlayer = false;
       
    }

    void SetNextActivePlayer()
    {
        activePlayer++;
        activePlayer %= playerList.Count;

        int available = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playerList[i].hasWon)
            {
                available++;
            }
        }

        if (playerList[activePlayer].hasWon && available > 1)
        {
            SetNextActivePlayer();
            return;
        }
        else if(available < 2)
        {
            //GAME OVER SCENE
            SceneManager.LoadScene("GameOver");
            state = States.WAITING;
            return;
        }

        VirtualCameraSelector.Instance.VcamChange();
        Info.Instance.ShowMessage(playerList[activePlayer].playerName + "'s turn");
        state = States.ROLL_DICE;
    }

    public void ReportTurnPossible(bool possible)
    {
        turnPossible = possible;
    }

    public void ReportWinning()
    {
        //SHOW SOME UI
        playerList[activePlayer].hasWon = true;
        //SAVE SOMEWHERE
        for (int i = 0; i < SaveSettings.winners.Length; i++)
        {
            if (SaveSettings.winners[i] == "")
            {
                SaveSettings.winners[i] = playerList[activePlayer].playerName;
                break;
            }
        }
    }

    //--------------------------------------------------------------HUMAN INPUT-------------------------------------------------------------
    void ActivateButton(bool on)
    {
        rollButton.SetActive(on);
    }

   public void DeactivateAllSelectors()
    {
        for (int i = 0;i < playerList.Count;i++)
        {
            for(int j = 0; j < playerList[i].myStones.Length; j++)
            {
                playerList[i].myStones[j].SetSelector(false);
            }
        }
    }

    public void HumanRoll()
    {
        dice.RollDice();
        ActivateButton(false);
    }

    //THIS SITS ON THE ROLL DICE BUTTON
    public void HumanRollDice()
    {
      
        //ROLL DICE
        //rolledHumanDice = Random.Range(1, 7);


        //MOVABLE STONE LIST

        List<Stone> movableStones = new List<Stone>();

        
        //KILL THE LIST
        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                
                //CHECK FOR POSSIBLE KICK
                if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID, rolledHumanDice))
                {
                    movableStones.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }

                //CHECK FOR POSSIBLE MOVE
                if (playerList[activePlayer].myStones[i].CheckPossibleMove(rolledHumanDice))
                {
                    movableStones.Add(playerList[activePlayer].myStones[i]);
                }
            }
        }
        //START NODE FULL CHECK
        //IS ANYONE ON THE START NODE
        bool startNodeFull = false;
        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startNodeFull = true;
                break;//WE ARE DONE HERE WE FOUND A MATCH
            }
        }
        //NUMBER < 6
        if(rolledHumanDice < 6)
        {
            for (int i = 0; i < playerList[activePlayer].myStones.Length; ++i)
            {
                //MAKE SURE HE IS OUT ALREADY
                if (playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    
                    if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID, rolledHumanDice))
                    {
                        movableStones.Add(playerList[activePlayer].myStones[i]);
                        continue;

                    }
                    if (playerList[activePlayer].myStones[i].CheckPossibleMove(rolledHumanDice))
                    {
                        movableStones.Add(playerList[activePlayer].myStones[i]);

                    }
                }
            }

            movableStones.AddRange(PossibleStones());
        }

        //NUMBER = 6 && !startnode
        if(rolledHumanDice == 6 && !startNodeFull)
        {
            //INSIDE BASE CHECK
            for(int i = 0; i < playerList[activePlayer].myStones.Length; ++i)
            {
                if (!playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    movableStones.Add(playerList[activePlayer].myStones[i] );
                }
            }

            //OUTSIDE CHECK
            movableStones.AddRange(PossibleStones());
        }
        //NUMBER = 6 && startnode
        else if (rolledHumanDice == 6 && startNodeFull)
        {
            movableStones.AddRange(PossibleStones());
        }

        //ACTIVATE ALL POSSIBLE SELECTOR
        if(movableStones.Count > 0)
        {
            for (int i = 0; i < movableStones.Count; i++)
            {
                movableStones[i].SetSelector(true);
            }
        }

        else
        {
            state = States.SWITCH_PLAYER;
        }
       

    }

    List <Stone> PossibleStones()
    {
        List<Stone> tempList = new List<Stone>();

        for (int i = 0; i < playerList[activePlayer].myStones.Length; ++i)
        {
            //MAKE SURE HE IS OUT ALREADY
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                
                if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID, rolledHumanDice))
                {
                    tempList.Add(playerList[activePlayer].myStones[i]);
                    continue;

                }
                if (playerList[activePlayer].myStones[i].CheckPossibleMove(rolledHumanDice))
                {
                    tempList.Add(playerList[activePlayer].myStones[i]);

                }
            }
        }

        return tempList;
    }
}
