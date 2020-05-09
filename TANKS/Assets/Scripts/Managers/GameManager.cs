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
    private int TimerText;
    private TankMovement movementScript;
    private int flagCarryNumber;
    
    
    

    
    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();
        StartCoroutine(GameLoop());
        

      
        
    }

    public void FlagTaken()
    {
        for (int i = 0; i < m_Tanks.Length;)
        {
            if (m_Tanks[i].m_Instance == movementScript.m_FlagTank)
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

        timeLeft -= Time.deltaTime;
        TimerText = Mathf.RoundToInt(timeLeft);
        if (timeLeft < 0f)
        {
            StartCoroutine(RoundEnding());
        }
        timerText.text = TimerText.ToString();
      

       

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


    private IEnumerator GameLoop()
    {
        Debug.Log("Called GameLoop");
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
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
        Debug.Log(m_MessageText.text);
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        m_MessageText.text = string.Empty;
        EnableTankControl();
        timeLeft = 60;
        timerText.gameObject.SetActive(true);

      
        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    public IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = null;

        if (movementScript.m_RoundWonByMe != null)
        {
            m_RoundWinner = movementScript.m_RoundWonByMe;
        }

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }

        m_GameWinner = GetGameWinner();

        string message = EndMessage();

        m_MessageText.text = message;
        //timerText.text = "";
        timerText.gameObject.SetActive(false);
       

        yield return m_EndWait;
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
                return m_Tanks[i];
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

    



 
    
}