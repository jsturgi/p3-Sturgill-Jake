using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdle;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = .2f;
    public GameObject m_TankFlag;
    public GameObject m_FlagTank;

    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;
    private bool m_FlagCarry;
    
    

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
        m_TankFlag.SetActive(false);
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
        
    }

    private void FlagTaken()
    {
        m_TankFlag.SetActive(true);
        m_FlagCarry = true;
    }

    // Update is called once per frame
    void Update()
    {
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }

    private void EngineAudio()
    {
        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdle;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }

        else
        {
            if (m_MovementAudio.clip == m_EngineIdle)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }
    
    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if ( collision.gameObject.tag == "Flag")
        {
            m_FlagTank = this.gameObject;
            m_FlagTank.GetComponent<TankShooting>().FlagCarrier();

            FlagTaken();
            
            CTFBroker.CallFlagTaken();
        }
        
        if (collision.gameObject.name == "SpawnPoint1")
        {
            if (m_FlagCarry)
            {
                Debug.Log("Player1 Wins");
            }
        }
        if (collision.gameObject.name == "SpawnPoint2")
        {
            if (m_FlagCarry)
            {
                Debug.Log("Player2 Wins!");
            }
        }
    }
}
