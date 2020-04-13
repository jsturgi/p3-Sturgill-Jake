using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;
    public Slider m_ReloadCooldown;
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;
    public float m_ReloadTime = 1.5f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;
    private bool m_Reloading = false;
    private float m_ReloadTimeCopy = 1.5f;
    private bool m_FlagRunner = false;
    


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
        m_ReloadCooldown.value = 1.5f;
        CTFBroker.FlagTaken += CTFBroker_FlagTaken;
    }

    private void CTFBroker_FlagTaken()
    {
        FlagCarrier();
    }

    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

        m_FlagRunner = false;
    }
    

    private void Update()
    {
        //Debug.Log(m_ReloadTimeCopy);
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired && !m_Reloading && !m_FlagRunner)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton) && !m_Reloading && !m_FlagRunner)
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired && !m_Reloading && !m_FlagRunner)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Reloading && !m_FlagRunner)
        {
            Fire();
        }
        if (m_Reloading)
        {
            SetReloadCooldownUI();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
       

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        m_CurrentLaunchForce = m_MinLaunchForce;

        m_Reloading = true;
        StartCoroutine(Reloading(m_ReloadTime));
        //StartCoroutine(Cooldown(m_ReloadTimeCopy));

    }

    private IEnumerator Reloading(float m_ReloadTime)
    {
        yield return new WaitForSeconds(m_ReloadTime);
        m_Reloading = false;
        m_ReloadTimeCopy = m_ReloadTime;
    }
    /*
        private IEnumerator Cooldown(float m_ReloadTimeCopy)
        {
            while (m_ReloadTimeCopy > 0)
            {
                m_ReloadTimeCopy -= Time.deltaTime;
                m_ReloadCooldown.value = Mathf.Lerp(0f, 1.5f, m_ReloadTimeCopy / m_ReloadTime);
                yield return null;
            }
        }
    */

    private void SetReloadCooldownUI()
    {
        m_ReloadTimeCopy -= Time.deltaTime;
        m_ReloadCooldown.value = m_ReloadTimeCopy;
    }

    private void FlagCarrier()
    {
        m_FlagRunner = true;
    }
}