using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceSaver : MonoBehaviour
{
    //List<PositionSequence> _savedSequences = new List<PositionSequence>();

    //private void Update()
    //{
    //    for (int i = 0; i < _savedSequences.Count; i++)
    //    {
    //        _savedSequences[i].Age += Time.deltaTime;

    //        // Remove old sequences
    //        if (_savedSequences[i].Age > 1000)
    //            _savedSequences.RemoveAt(i);
    //    }
    //}

    //public void StoreSequence(PositionSequence sequence)
    //{
    //    _savedSequences.Add(sequence);
    //}

    //public PositionSequence[] GetSequences(int length)
    //{
    //    List<PositionSequence> requestedSequences = new List<PositionSequence>();

    //    for (int i = 0; i < _savedSequences.Count; i++)
    //    {
    //        // If sequence is not too old, add it
    //        if (_savedSequences[i].Age < length * (1.0f / PositionSaver.SavesPerSecond))
    //        {
    //            // Fill sequence with empty vectors for the steps it has been inactive
    //            int oldSize = _savedSequences[i].Positions.Length;
    //            int inactiveSteps = (int)(_savedSequences[i].Age / (1.0f / PositionSaver.SavesPerSecond));
    //            Vector2[] newPositions = new Vector2[length];

    //            for (int j = 0; j < newPositions.Length; j++)
    //            {
    //                // Fill these with actual positions, unless there aren't enough positions
    //                if (j < length - inactiveSteps)
    //                {
    //                    int index = oldSize - (length - inactiveSteps) + j;
    //                    if (index < 0)
    //                        newPositions[j] = Vector2.zero;
    //                    else
    //                        newPositions[j] = _savedSequences[i].Positions[index];
    //                }
    //                // Fill these with empty positions
    //                else
    //                    newPositions[j] = Vector2.zero;
    //            }

    //            _savedSequences[i].Positions = newPositions;
    //            requestedSequences.Add(_savedSequences[i]);
    //        }
    //    }

    //    return requestedSequences.ToArray();
    //}
}

public class PositionSequence
{
    public PositionSequence(Color color, Vector2[] positions)
    {
        RepresentationColor = color;
        Positions = positions;
        Age = 0.0f;
    }

    public Color RepresentationColor;
    public Vector2[] Positions;
    public float Age;
}