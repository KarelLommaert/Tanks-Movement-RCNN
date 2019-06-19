using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float MaxHealth { get { return m_StartingHealth; } }
    public float CurrentHealth { get { return m_CurrentHealth; } }

    [SerializeField] private float m_StartingHealth = 100f;          
    [SerializeField] private Slider m_Slider;                        
    [SerializeField] private Image m_FillImage;                      
    [SerializeField] private Color m_FullHealthColor = Color.green;  
    [SerializeField] private Color m_ZeroHealthColor = Color.red;    
    [SerializeField] private GameObject m_ExplosionPrefab;

    private bool _invulnerable = false;
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;

    public delegate void OnDied();
    public event OnDied DiedEvent;

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        //_invulnerable = false;
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    public void SetInvulnerable(bool invulnerable)
    {
        _invulnerable = invulnerable;
    }

    public void TakeDamage(float amount)
    {
        if (_invulnerable)
            return;

        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;

		SetHealthUI();

		if (m_CurrentHealth <= 0f && !m_Dead)
        {
			OnDeath();
		}
    }

    public void Heal(float amount)
    {
        m_CurrentHealth += amount;

        if (m_CurrentHealth > m_StartingHealth)
        {
            m_CurrentHealth = m_StartingHealth;
        }

        SetHealthUI();
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
		m_Slider.value = m_CurrentHealth;

		m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
		m_Dead = true;

		m_ExplosionParticles.transform.position = transform.position;
		m_ExplosionParticles.gameObject.SetActive(true);
		m_ExplosionParticles.Play();
		m_ExplosionAudio.Play();

        gameObject.SetActive(false);

        // Died event
        DiedEvent?.Invoke();
    }
}