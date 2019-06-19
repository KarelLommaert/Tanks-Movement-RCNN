using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanksArea : MonoBehaviour
{
    public TanksAgent[] TanksInArea { get { return _tanksInArea.ToArray(); } }

    private List<TanksAgent> _tanksInArea = new List<TanksAgent>();

    private void Awake()
    {
        TanksAgent[] agents = GetComponentsInChildren<TanksAgent>();
        for (int i = 0; i < agents.Length; i++)
        {
            _tanksInArea.Add(agents[i]);
        }
    }
}