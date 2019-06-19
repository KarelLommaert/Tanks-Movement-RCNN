using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSaver : MonoBehaviour
{
    public Color Color { get { return _representationColor; } }
    public int Radius { get { return _representationRadius; } }

    //public static float SavesPerSecond = 5.0f;

    [SerializeField] private Color _representationColor = Color.green;
    [SerializeField] private int _representationRadius = 5;
    //private float _accumulatedTime = 0.0f;
    //private bool _saving = true;
    private ObjectToTrack _pixelRepresentation;

    //private List<Vector2> _savedPositions = new List<Vector2>();

    //private void LateUpdate()
    //{
    //    if (_saving)
    //    {
    //        _accumulatedTime += Time.deltaTime;

    //        if (_accumulatedTime > (1.0f / SavesPerSecond))
    //        {
    //            _accumulatedTime -= (1.0f / SavesPerSecond);
    //            _savedPositions.Add(new Vector2(transform.position.x, transform.position.z));

    //            if (_savedPositions.Count > 100000)
    //            {
    //                _saving = false;
    //            }
    //        }
    //    }
    //}

    private void OnEnable()
    {
        _pixelRepresentation = new ObjectToTrack(transform, _representationColor);
        PositionSaverUpgrade.Instance.StartTracking(_pixelRepresentation);
    }

    private void OnDisable()
    {
        if (PositionSaverUpgrade.Instance)
            PositionSaverUpgrade.Instance.StopTracking(_pixelRepresentation);

        //PictorialRepresentationSaver saver = FindObjectOfType<PictorialRepresentationSaver>();
        //if (saver)
        //    saver.GetComponent<SequenceSaver>().StoreSequence(new PositionSequence(_representationColor, GetSequencePositions(_savedPositions.Count)));
    }

    //public Vector2[] GetSequencePositions(int length)
    //{
    //    Vector2[] positions = new Vector2[length];

    //    if (length > _savedPositions.Count)
    //    {
    //        for (int i = 0; i < length - _savedPositions.Count; i++)
    //        {
    //            positions[i] = Vector2.zero;
    //        }
    //        for (int i = 0; i < _savedPositions.Count; i++)
    //        {
    //            positions[length - _savedPositions.Count + i] = _savedPositions[i];
    //        }
    //    }
    //    else
    //    {
    //        int startIndex = _savedPositions.Count - length;
    //        for (int i = 0; i < length; i++)
    //        {
    //            //positions[i] = _savedPositions[_savedPositions.Count - i - 1];
    //            positions[i] = _savedPositions[startIndex + i];
    //        }
    //    }

    //    return positions;
    //}
}