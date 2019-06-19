using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;
    public Slider _ammoSlider = null;
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 1.75f;
    public int _maxAmmo = 7;
    public float _reloadTime = 1.5f;

    private float _unlimitedAmmoTimer = 0.0f;
    private bool _unlimitedAmmo = false;
    private bool _reloading = false;
    private float _currentReloadTimer = 0.0f;
    private int _currentAmmo = 0;
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;
    private bool _charging = false;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
        _currentAmmo = _maxAmmo;
        _ammoSlider.value = _ammoSlider.maxValue;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        //      // Track the current state of the fire button and make decisions based on the current launch force.
        //m_AimSlider.value = m_MinLaunchForce;

        //if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
        //	// At max charge, not yet fired.
        //	m_CurrentLaunchForce = m_MaxLaunchForce;
        //	Fire();

        //} else if (Input.GetButtonDown(m_FireButton)) {
        //	// Have we pressed the Fire button for the first time?
        //	m_Fired = false;
        //	m_CurrentLaunchForce = m_MinLaunchForce;

        //	m_ShootingAudio.clip = m_ChargingClip;
        //	m_ShootingAudio.Play();

        //} else if (Input.GetButton(m_FireButton) && !m_Fired) {
        //	// Holding the fire button, not yet fired.
        //	m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

        //	m_AimSlider.value = m_CurrentLaunchForce;

        //} else if (Input.GetButtonUp(m_FireButton) && !m_Fired) {
        //	// We releasted the fire button, having not fired yet.
        //	Fire();

        //}
        if (_unlimitedAmmo)
        {
            _unlimitedAmmoTimer -= Time.deltaTime;
            if (_unlimitedAmmoTimer <= 0.0f)
            {
                _unlimitedAmmo = false;
            }
        }

        if (_reloading)
        {
            _currentReloadTimer += Time.deltaTime;
            _ammoSlider.value = (_currentReloadTimer / _reloadTime) * _ammoSlider.maxValue;

            if (_currentReloadTimer >= _reloadTime)
            {
                _reloading = false;
                _currentReloadTimer = 0.0f;
                _currentAmmo = _maxAmmo;
            }
        }
    }

    public void SetUnlimitedAmmo(float duration)
    {
        _unlimitedAmmo = true;
        _unlimitedAmmoTimer = duration;
        _currentAmmo = _maxAmmo;
        _ammoSlider.value = _currentAmmo;
    }

    public void SetFiringInput(float input, bool reload)
    {
        if (!_unlimitedAmmo)
        {
            if (reload)
                Reload();

            if (_currentAmmo <= 0 || _reloading)
                return;
        }

        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // At max charge, not yet fired.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (input > 0.5f)
        {
            if (_charging)
            {
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            else
            {
                StartCharging();
            }
        }
        else if(_charging && !m_Fired)
        {
            Fire();
        }
    }

    private void Reload()
    {
        _reloading = true;
    }

    private void StartCharging()
    {
        _charging = true;
        m_Fired = false;
        m_CurrentLaunchForce = m_MinLaunchForce - 4.0f;

        m_ShootingAudio.clip = m_ChargingClip;
        m_ShootingAudio.Play();
    }

    public void Fire()
    {
        // Instantiate and launch the shell.
		m_Fired = true;
        _charging = false;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.GetComponent<ShellExplosion>().Initialize(gameObject);

		shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play();

		m_CurrentLaunchForce = m_MinLaunchForce;

        if (!_unlimitedAmmo)
        {
            --_currentAmmo;
            _ammoSlider.value = _currentAmmo;
        }

        if (_currentAmmo <= 0)
        {
            Reload();
        }
    }

    private float nextFireTime;
    public void Fire(float launchForce, float fireRate)
    {
        if (!_unlimitedAmmo)
        {
            if (_currentAmmo <= 0 || _reloading)
                return;
        }
        if (Time.time <= nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;
        m_Fired = true;

        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;

        if (!_unlimitedAmmo)
        {
            --_currentAmmo;
            _ammoSlider.value = _currentAmmo;
        }

        if (_currentAmmo <= 0)
        {
            Reload();
        }
    }
}