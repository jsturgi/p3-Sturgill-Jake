using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public TextMeshProUGUI m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;
    public float timeLeft = 60.0f;
    public TextMeshProUGUI timerText;
    public GameObject m_SpawnPoint1;
    public GameObject m_SpawnPoint2;
    
    

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private TankMovement movementscript;
    private int TimerText;
    private int flagCarryNumber;
    private enum gameState { RoundStarted, RoundPlaying, RoundEnded, RoundBeginning, RoundInitiating };
    private gameState currentState;
    private int endGameLoopBlocker = 0;
    
    
    

    
    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();
        StartCoroutine(RoundStarting());
        timeLeft = 60f;
        timerText.gameObject.SetActive(false);
        endGameLoopBlocker = 0;
        



    }

    public void FlagTaken()
    {
        for (int i = 0; i < m_Tanks.Length;)
        {
            if (m_Tanks[i].m_Instance == movementscript.m_FlagTank)
            {
                flagCarryNumber = m_Tanks[i].m_PlayerNumber;
            }
            i++;
        }

        if (m_SpawnPoint1.tag == flagCarryNumber.ToString())
        {
            m_SpawnPoint1.GetComponent<Renderer>().material.color = Color.blue;
        }

        if (m_SpawnPoint2.tag == flagCarryNumber.ToString())
        {
            m_SpawnPoint2.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    private void Update()

    {       
        timerText.text = TimerText.ToString();
        CheckGameState();
        
    }

   

    private void SpawnAllTanks()
    {
       
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].flagCarrierNumber = i + 1;
            m_Tanks[i].Setup();
            
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    

    private IEnumerator RoundTimer()
    {
        
        while ( timeLeft > 0f)
        {
            yield return new WaitForSeconds(1);
            timeLeft -= 1;
            TimerText = Mathf.RoundToInt(timeLeft);
        }
        if (timeLeft <= 0f)
        {
            timerText.gameObject.SetActive(false);
            
        }
       
        
    }
    private IEnumerator RoundStarting()
    {
        timerText.gameObject.SetActive(false);
        
        
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;
        currentState = gameState.RoundBeginning;
        endGameLoopBlocker = 0;
        
        
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        StartCoroutine(RoundTimer());
        m_MessageText.text = string.Empty;
        EnableTankControl();
        
        timerText.gameObject.SetActive(true);

      
        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    public IEnumerator RoundEnding()
    {
        if ( endGameLoopBlocker < 1)
        {
            DisableTankControl();
            StopCoroutine(RoundTimer());

            m_RoundWinner = null;

            if (timeLeft < 0f)
            {
                m_MessageText.text = "DRAW!";
                timerText.gameObject.SetActive(false);
                yield return m_EndWait;
                currentState = gameState.RoundInitiating;
            }
            m_RoundWinner = GetRoundWinner();
            m_RoundWinner.m_Wins++;

            m_GameWinner = GetGameWinner();

            string message = EndMessage();

            m_MessageText.text = message;

            timerText.gameObject.SetActive(false);
            endGameLoopBlocker++;
        }

        currentState = gameState.RoundInitiating;
        

        
        
        
        
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {


            if (m_Tanks[i].m_Instance.activeSelf)
            {

                return m_Tanks[i];
            }
                
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }


    public void CheckGameState()
    {
        if (currentState == gameState.RoundStarted)
        {
            StopCoroutine(RoundStarting());
            StartCoroutine(RoundPlaying());
            currentState = gameState.RoundPlaying;
        }
        if (OneTankLeft())
        {
            StopCoroutine(RoundPlaying());
            currentState = gameState.RoundEnded;
            
            StartCoroutine(RoundEnding());
        }
        if ( timeLeft <= 0f)
        {
            if (currentState != gameState.RoundEnded)
            {
                currentState = gameState.RoundEnded;
                StopCoroutine(RoundPlaying());
                StartCoroutine(RoundEnding());
            }
        }
        if (currentState == gameState.RoundBeginning)
        {
            StopCoroutine(RoundStarting());
            StartCoroutine(RoundPlaying());
            currentState = gameState.RoundPlaying;
        }
        if (currentState == gameState.RoundInitiating)
        {
            
            StopCoroutine(RoundEnding());
            StartCoroutine(RoundStarting());
            
        }
    }
    



 
    
}