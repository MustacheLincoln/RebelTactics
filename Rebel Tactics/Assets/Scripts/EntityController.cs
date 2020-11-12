using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class EntityController : MonoBehaviour
{
    CursorGrid _cursorGrid;
    GameManager _gameManager;

    public NavMeshAgent agent;
    public NavMeshObstacle obstacle;

    public bool moving;
    public bool movable;
    public int maxMoveDistance = 6;
    public int number;

    private void Start()
    {
        _cursorGrid = FindObjectOfType<CursorGrid>();
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.GetEntities();
        for (int i = 0; i <= _gameManager.entities.Length - 1; i++)
            if (_gameManager.entities[i] == this)
                number = i;
        agent.enabled = false;
        obstacle.enabled = true;
        if (number == _gameManager.turn)
            StartTurn();
    }

    public void StartTurn()
    {
        agent.enabled = false;
        obstacle.enabled = false;
        movable = true;
        moving = false;
        StartCoroutine(_cursorGrid.CalculateRange(this));
    }

    public void EndTurn()
    {
        agent.enabled = false;
        obstacle.enabled = true;
        _gameManager.NextTurn();
        movable = false;
        moving = false;
    }

    public void Move(Vector3 destination)
    {
        obstacle.enabled = false;
        agent.enabled = true;
        agent.SetDestination(destination);
        movable = false;
        moving = true;
    }

    private void Update()
    {
        if (moving == true)
            if (!agent.pathPending)
                if (agent.remainingDistance <= agent.stoppingDistance)
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        EndTurn();

        if (_gameManager.turn == number)
        {
            if (movable == true && moving == false)
                _cursorGrid.CaptureCursor(this);
        }           
    }
}
