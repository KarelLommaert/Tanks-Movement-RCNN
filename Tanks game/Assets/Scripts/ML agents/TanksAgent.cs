using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class TanksAgent : Agent
{
    //public bool Alive { get { return _healthController.CurrentHealth > 0; } }

    private RayPerception _rayPerceiver = null;
    private Transform _spawnPoint = null;
    private TankMovement _tankMovement = null;
    private TankHealth _tankHealth = null;
    private TankShooting _tankShooting = null;

    private Transform _trainingAreaTransform = null;

    private TanksAgent[] _enemyTanks = null;

    public override void InitializeAgent()
    {
        _rayPerceiver = GetComponent<RayPerception>();
        _tankMovement = GetComponent<TankMovement>();
        _tankHealth = GetComponent<TankHealth>();
        _tankShooting = GetComponent<TankShooting>();

        _tankHealth.DiedEvent += AgentDied;
    }

    public void SetEnemies(TanksAgent[] enemyAgents)
    {
        _enemyTanks = enemyAgents;
    }

    public override void CollectObservations()
    {
        float rayDistance = 60f;
        float angle = 0.0f;
        int rayAmount = 36;
        //float[] rayAngles = new float[rayAmount];
        float[] rayAngles = new float[] { 90 };
        //for (int i = 0; i < rayAmount; i++)
        //{
        //    rayAngles[i] = angle;
        //    angle += 360.0f / rayAmount;
        //}
        string[] detectableObjects = new string[] { "Tank", "Obstacle"/*, "Shell"*/ };

        ////List<float> rayPerceivings = _rayPerceiver.Perceive(rayDistance, rayAngles, detectableObjects, 1.2f, 0, gameObject);
        ////string log = string.Empty;
        ////for (int i = 0; i < rayPerceivings.Count; i++)
        ////{
        ////    log += rayPerceivings[i].ToString() + ", ";
        ////}
        ////Debug.Log(log);
        AddVectorObs(_rayPerceiver.Perceive(rayDistance, rayAngles, detectableObjects, 1.2f, 0, gameObject));

        //// Info about enemies
        //for (int i = 0; i < _enemyTanks.Length; i++)
        //{
        //    Vector3 dirToEnemy = (_enemyTanks[i].transform.position - transform.position);
        //    float distToEnemy = dirToEnemy.magnitude;
        //    dirToEnemy.Normalize();
        //    dirToEnemy.y = 0.0f;
        //    float angleToEnemy = Vector3.SignedAngle(transform.forward, dirToEnemy, Vector3.up);
        //    AddVectorObs((angleToEnemy / 360.0f) + 0.5f);
        //    AddVectorObs(distToEnemy / 100.0f);
        //    AddVectorObs(_enemyTanks[i]._tankHealth.CurrentHealth);

        //    //Debug.Log((angleToEnemy / 180.0f) + " - " + distToEnemy / 100.0f);
        //}

        // Info about this tank
        //AddVectorObs(_tankHealth.CurrentHealth);
        //AddVectorObs(transform.rotation);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //ContinuousAction(vectorAction);
        DiscreteAction(vectorAction);
        

        //bool tooFarFromEnemy = true;
        //float distToClosestEnemy = Mathf.Infinity;
        //for (int i = 0; i < _enemyTanks.Length; i++)
        //{
        //    float dist = Vector3.Distance(_enemyTanks[i].transform.position, transform.position);
        //    if (dist < distToClosestEnemy)
        //    {
        //        distToClosestEnemy = dist;
        //        //tooFarFromEnemy = false;
        //        //break;
        //    }
        //}
        //AddReward(-0.001f / distToClosestEnemy);
        //if (!tooFarFromEnemy)
        //    AddReward(0.0005f);
        //else
        //    AddReward(-0.0005f);
    }

    private void ContinuousAction(float[] vectorAction)
    {
        //_tankMovement.SetInput((vectorAction[0] * 2.0f) - 1.0f, (vectorAction[1] * 2.0f) - 1.0f);
        _tankMovement.SetInput((vectorAction[0]), vectorAction[1]);
        
        _tankShooting.SetFiringInput(vectorAction[2], Mathf.FloorToInt(vectorAction[3]) == 1);
    }

    private void DiscreteAction(float[] vectorAction)
    {
        int forwardAction = Mathf.FloorToInt(vectorAction[0]);
        float forward = 0.0f;
        switch (forwardAction)
        {
            case 1:
                forward = 0.0f;
                break;
            case 2:
                forward = -1.0f;
                break;
            case 3:
                forward = 1.0f;
                break;
            default:
                break;
        }
        int turnAction = Mathf.FloorToInt(vectorAction[1]);
        float turn = 0.0f;
        switch (turnAction)
        {
            case 1:
                turn = 0.0f;
                break;
            case 2:
                turn = -1.0f;
                break;
            case 3:
                turn = 1.0f;
                break;
            default:
                break;
        }
        int shootAction = Mathf.FloorToInt(vectorAction[2]);
        float shoot = 0.0f;
        switch (shootAction)
        {
            case 1:
                shoot = 0.0f;
                break;
            case 2:
                shoot = 1.0f;
                break;
            default:
                break;
        }
        int reloadAction = Mathf.FloorToInt(vectorAction[3]);
        bool reload = false;
        switch (reloadAction)
        {
            case 1:
                reload = false;
                break;
            case 2:
                reload = true;
                break;
            default:
                break;
        }

        //Debug.Log(forward + ", " + turn + ", " + shoot);

        _tankMovement.SetInput(forward, turn);
        _tankShooting.SetFiringInput(shoot, reload);
    }

    private void AgentDied()
    {
        //AddReward(-1.0f);
        //SetReward(-1.0f);
    }

    public override void AgentOnDone()
    {
    }

    public override void AgentReset()
    {
    }
}