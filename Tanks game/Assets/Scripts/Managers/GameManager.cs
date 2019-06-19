using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GameMode
{
    Tutorial,
    Normal,
    Timer,
    LowHP
}

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 3;        
    public float m_StartDelay = 1f;         
    public float m_EndDelay = 2f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;
    public GameObject m_AITankPrefab;
    public GameObject m_PlayerTankPrefab;
    public TankManager[] m_Tanks;
    public GameMode CurrentGameMode;
    public Text m_TimerText;
    public Text m_GameAmountText;
    public GameModeMenu m_NextGameMenu;
    public Button m_LeavePracticeModeButton;

    private int _gamesWonCurrentGameMode = 0;
    private TanksAgent[] _agents = null;
    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    [SerializeField] private float _maxTime = 60.0f;
    private float _currentTimer = 0.0f;
    private int _gameCount = 0;
    public static bool Playing = false;

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        _gameCount = 0;
        SpawnAllTanks();
        SetCameraTargets();
        //StartNewRound();
        ChangeGameMode(GameMode.Tutorial);

        //StartCoroutine(GameLoop());
        m_MessageText.text = string.Empty;
        m_LeavePracticeModeButton.gameObject.SetActive(false);
        m_GameAmountText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (CurrentGameMode == GameMode.Timer && Playing)
        {
            _currentTimer -= Time.deltaTime;

            m_TimerText.text = ((int)_currentTimer).ToString();

            if (_currentTimer <= 0.0f)
            {
                Debug.Log("Timer expired");
                EndRound(false, true);
            }
        }
        else if (CurrentGameMode == GameMode.Tutorial)
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (!m_Tanks[i].IsTankAlive)
                {
                    m_Tanks[i].Reset();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            EndRound(true);
        }
    }

    //private void CheckAgentsAlive()
    //{
    //    int agentsAlive = 0;
    //    for (int i = 0; i < m_Tanks.Length; i++)
    //    {
    //        if (m_Tanks[i].IsTankAlive)
    //            ++agentsAlive;
    //    }

    //    if (agentsAlive <= 1)
    //        ResetArea();
    //}

    private void CheckAgentsAlive()
    {
        bool playerAlive = false;
        int agentsAlive = 0;
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].IsTankAlive)
                ++agentsAlive;

            if (m_Tanks[i].IsTankAlive && m_Tanks[i].m_PlayerControlled)
                playerAlive = true;
        }

        if (agentsAlive <= 1 || !playerAlive)
        {
            //ResetArea();
            EndRound(playerAlive);
        }
    }

    //private IEnumerator OpenLoadingScreenAndSave()
    //{
    //    //Instantiate(SavingDataTextPrefab);
    //    //yield return null;
    //    GameObject.FindObjectOfType<PictorialRepresentationSaver>().SaveAllStoredSequences();
    //    SceneManager.LoadScene(0);
    //    yield return null;
    //}

    private void EndRound(bool playerAlive, bool lostToTimer = false)
    {
        DisableTankControl();
        m_NextGameMenu.EnablePlayButton();

        if (Playing)
        {
            if (playerAlive)
            {
                ++_gamesWonCurrentGameMode;
                m_MessageText.text = "You won!";
            }
            else if (lostToTimer)
                m_MessageText.text = "Time ran out...";
            else
                m_MessageText.text = "You died...";
        }

        Playing = false;

        ++_gameCount;
        if (_gameCount == 11)
            ChangeGameMode(GameMode.Timer);
        else if (_gameCount == 21)
            ChangeGameMode(GameMode.LowHP);
        else if (_gameCount == 31)
        {
            m_MessageText.text = string.Empty;
            m_NextGameMenu.SetGameFinish(_gamesWonCurrentGameMode);
        }

        PositionSaverUpgrade.Instance.EndRound(CurrentGameMode);
        if (!playerAlive)
            EnableTanks(false);

        //StartNewRound();

    }

    public void LeavePracticeMode()
    {
        PositionSaverUpgrade.Instance.EmptyStoredPositions();
        DisableTankControl();
        EnableTanks(false);
        EndRound(true);
        ChangeGameMode(GameMode.Normal);
    }

    private void ChangeGameMode(GameMode mode)
    {
        // Neutralize any settings from the previous game mode
        m_MessageText.text = string.Empty;
        m_TimerText.gameObject.SetActive(false);
        m_LeavePracticeModeButton.gameObject.SetActive(false);
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_PlayerControlled)
            {
                m_Tanks[i].m_Instance.GetComponent<TankHealth>().SetInvulnerable(false);
            }
        }

        // Set menu
        m_NextGameMenu.SetGameMode(mode, _gamesWonCurrentGameMode);
        _gamesWonCurrentGameMode = 0;

        // Switch any settings necessary for this game mode
        CurrentGameMode = mode;

        switch (mode)
        {
            case GameMode.Tutorial:
                for (int i = 0; i < m_Tanks.Length; i++)
                {
                    if (m_Tanks[i].m_PlayerControlled)
                    {
                        m_Tanks[i].m_Instance.GetComponent<TankHealth>().SetInvulnerable(true);
                    }
                }
                break;
            case GameMode.Normal:
                break;
            case GameMode.Timer:
                _currentTimer = _maxTime;
                m_TimerText.gameObject.SetActive(true);
                break;
            case GameMode.LowHP:
                break;
            default:
                break;
        }
    }

    public void StartNewRound()
    {
        Playing = true;
        _currentTimer = _maxTime;
        EnableTanks(true);
        EnableTankControl();
        m_NextGameMenu.EnableMenu(false);
        m_GameAmountText.text = (_gameCount).ToString() + "/30";
        m_MessageText.text = string.Empty;

        if (CurrentGameMode == GameMode.Tutorial)
            m_LeavePracticeModeButton.gameObject.SetActive(true);
        else
            m_GameAmountText.gameObject.SetActive(true);

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            TankManager tank = m_Tanks[i];
            if (tank.IsTankAlive)
                tank.SetAgentDone(0.5f);

            if (CurrentGameMode == GameMode.LowHP)
                tank.Reset(true);
            else
                tank.Reset();
        }
    }

    private void ResetArea()
    {
        ++_gameCount;
        if (_gameCount == 3)
            PositionSaverUpgrade.Instance.EmptyStoredPositions();
        else if (_gameCount == 13)
            CurrentGameMode = GameMode.Timer;
        else if (_gameCount == 23)
            CurrentGameMode = GameMode.LowHP;
        else if (_gameCount == 33)
            m_NextGameMenu.SetGameFinish(_gamesWonCurrentGameMode);
            //GameObject.FindObjectOfType<PictorialRepresentationSaver>().SaveAllStoredSequences();

        m_GameAmountText.text = _gameCount.ToString() + "/33";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            TankManager tank = m_Tanks[i];
            if (tank.IsTankAlive)
                tank.SetAgentDone(0.5f);

            if (CurrentGameMode == GameMode.LowHP)
                tank.Reset(true);
            else
                tank.Reset();
        }

        if (CurrentGameMode == GameMode.Timer)
        {
            _currentTimer = _maxTime;
            m_TimerText.gameObject.SetActive(true);
        }
        else
            m_TimerText.gameObject.SetActive(false);


        PositionSaverUpgrade.Instance.EndRound(CurrentGameMode);
    }

    private void SpawnAllTanks()
    {
        // Spawn all tanks
        _agents = new TanksAgent[m_Tanks.Length];

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_PlayerControlled)
                m_Tanks[i].m_Instance = Instantiate(m_PlayerTankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            else
                m_Tanks[i].m_Instance = Instantiate(m_AITankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;

            _agents[i] = m_Tanks[i].m_Instance.GetComponent<TanksAgent>();
        }

        // Initialize tanks
        Transform playerTank = null;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].m_Instance.GetComponent<TankHealth>().DiedEvent += CheckAgentsAlive;

            // Assign enemies
            TanksAgent[] enemies = new TanksAgent[m_Tanks.Length - 1];
            int index = 0;
            for (int j = 0; j < m_Tanks.Length; j++)
            {
                if (m_Tanks[i].m_Instance != m_Tanks[j].m_Instance)
                {
                    if (m_Tanks[i].m_PlayerControlled)
                    {
                        playerTank = m_Tanks[i].m_Instance.transform;
                    }
                    enemies[index] = _agents[j];
                    ++index;
                }
            }

            m_Tanks[i].Setup(enemies, m_Tanks[i].m_PlayerControlled);
            if (!m_Tanks[i].m_PlayerControlled)
            {
                Transform waypointParent = GameObject.Find("AI Waypoints").transform;
                List<Transform> waypoints = new List<Transform>();
                Transform[] children = waypointParent.GetComponentsInChildren<Transform>();
                for (int j = 0; j < children.Length; j++)
                {
                    if (children[j] != waypointParent)
                        waypoints.Add(children[j]);
                }
                m_Tanks[i].m_Instance.GetComponent<StateController>().SetupAI(true, waypoints, playerTank);
            }
        }

        //ResetArea();
        EnableTanks(false);
    }

    private void EnableTanks(bool enable)
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance.SetActive(enable);
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
		ResetAllTanks();
		DisableTankControl();

		m_CameraControl.SetStartPositionAndSize();

		m_RoundNumber++;
		m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
		EnableTankControl();

		m_MessageText.text = string.Empty;

		while (!OneTankLeft()) {
			yield return null;	
		}
    }

    private IEnumerator RoundEnding()
    {
		DisableTankControl();

		m_RoundWinner = GetRoundWinner();

		if (m_RoundWinner != null) {
			m_RoundWinner.m_Wins++;
		}

		m_GameWinner = GetGameWinner();

		m_MessageText.text = EndMessage();

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