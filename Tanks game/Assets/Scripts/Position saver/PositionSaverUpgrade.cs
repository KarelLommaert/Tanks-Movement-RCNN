using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSaverUpgrade : MonoBehaviour
{
    // Singleton implementation
    public static PositionSaverUpgrade Instance { get { return _instance; } }
    private static PositionSaverUpgrade _instance = null;

    public static float SavesPerSecond = 5.0f;

    //private List<Vector2[]> _storedPositions = new List<Vector2[]>();
    private List<ObjectPixelRepresentation[]> _storedPositions = new List<ObjectPixelRepresentation[]>();
    private List<ObjectToTrack> _objectsToTrack = new List<ObjectToTrack>();
    private float _accumulatedTime = 0.0f;
    private int _currentStepAmount = 0;

    //[SerializeField] int _autoSaveStepAmount = 100;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void Update()
    {
        if (GameManager.Playing)
        {
            _accumulatedTime += Time.deltaTime;

            if (_accumulatedTime > (1.0f / SavesPerSecond))
            {
                _accumulatedTime -= (1.0f / SavesPerSecond);
                ++_currentStepAmount;

                if (_currentStepAmount == 50)
                    StoreSequence(50, SequenceType.GameOpening);

                //if (_currentStepAmount > _autoSaveStepAmount)
                //{
                //    StoreSequence(100, SequenceType.Player);
                //    _autoSaveStepAmount += _autoSaveStepAmount;
                //}

                // Save the positions of all objects we're tracking
                ObjectPixelRepresentation[] pixelRepresentations = new ObjectPixelRepresentation[_objectsToTrack.Count];
                for (int i = 0; i < _objectsToTrack.Count; ++i)
                {
                    pixelRepresentations[i] = new ObjectPixelRepresentation(_objectsToTrack[i].ObjColor, new Vector2(_objectsToTrack[i].ObjTransform.position.x, _objectsToTrack[i].ObjTransform.position.z), _objectsToTrack[i].ObjTransform.rotation.eulerAngles);
                }
                _storedPositions.Add(pixelRepresentations);
            }
        }
    }

    public void EndRound(GameMode gameMode)
    {
        if (_currentStepAmount > 0)
        {
            StoreSequence(_currentStepAmount, SequenceType.Player);

            if (gameMode == GameMode.Timer)
                StoreSequence(_currentStepAmount, SequenceType.Timer);
            else if (gameMode == GameMode.LowHP)
                StoreSequence(_currentStepAmount, SequenceType.LowHP);
            else
                StoreSequence(_currentStepAmount, SequenceType.NormalMode);

            //StoreSequence(50, SequenceType.Kill);
        }
        _currentStepAmount = 0;
        //_storedPositions.Clear();
    }

    [HideInInspector] public List<Sequence> StoredSequences = new List<Sequence>();
    public void StoreSequence(int size, SequenceType type)
    {
        StoredSequences.Add(new Sequence(GetPositions(size), type));
        Debug.Log("Sequence of type " + type.ToString() + " was stored");
    }

    public void EmptyStoredPositions()
    {
        _storedPositions.Clear();
    }

    public void StartTracking(ObjectToTrack obj)
    {
        if (!_objectsToTrack.Contains(obj))
            _objectsToTrack.Add(obj);
    }

    public void StopTracking(ObjectToTrack obj)
    {
        if (_objectsToTrack.Contains(obj))
            _objectsToTrack.Remove(obj);
    }

    public ObjectPixelRepresentation[][] GetPositions(int steps)
    {
        if (steps > _storedPositions.Count)
            steps = _storedPositions.Count;

        ObjectPixelRepresentation[][] positions = new ObjectPixelRepresentation[steps][];
        int index = _storedPositions.Count - steps;
        for (int i = 0; i < steps; i++)
        {
            positions[i] = _storedPositions[index + i];
        }
        return positions;
    }
}

public struct ObjectToTrack
{
    public ObjectToTrack(Transform t, Color c)
    {
        ObjTransform = t;
        ObjColor = c;
    }

    public readonly Transform ObjTransform;
    public Color ObjColor;
}