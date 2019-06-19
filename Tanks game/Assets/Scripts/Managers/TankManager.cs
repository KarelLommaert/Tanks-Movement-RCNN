using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public bool IsTankAlive { get { return m_Health.CurrentHealth > 0.0f; } }

    public bool m_PlayerControlled = false;
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     

    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private TankHealth m_Health;
    private TanksAgent m_Agent;
    private GameObject m_CanvasGameObject;

    public void Setup(TanksAgent[] enemyAgents, bool isPlayer)
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_Health = m_Instance.GetComponent<TankHealth>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
        m_Agent = m_Instance.GetComponent<TanksAgent>();

        if (isPlayer)
        {
            m_Agent.SetEnemies(enemyAgents);
            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerNumber = m_PlayerNumber;
        }


        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }

    public void SetAgentDone(float addReward = 0.0f)
    {
        if (m_Agent)
        {
            if (addReward != 0.0f)
                m_Agent.AddReward(addReward);

            m_Agent.Done();
        }
    }

public void DisableControl()
    {
        if (m_Movement)
            m_Movement.enabled = false;
        if (m_Shooting)
            m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        if (m_Movement)
            m_Movement.enabled = true;
        if (m_Shooting)
            m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    public void Reset(bool lowHPMode = false)
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);

        if (lowHPMode && m_PlayerControlled)
        {
            m_Health.TakeDamage(m_Health.CurrentHealth - 1);
        }

        if (!m_PlayerControlled)
        {
            m_Instance.GetComponent<StateController>().ResetState();
        }
    }
}
