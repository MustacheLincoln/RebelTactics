using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class PlayerController : MonoBehaviour
{
    CursorGrid _cursorGrid;
    GameManager _gameManager;

    public NavMeshAgent agent;
    public NavMeshObstacle obstacle;

    public bool movable;
    public int maxMoveDistance = 6;
    public int number;

    private void Start()
    {
        _cursorGrid = FindObjectOfType<CursorGrid>();
        _gameManager = FindObjectOfType<GameManager>();
        movable = false;
        var players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i <= players.Length - 1; i++)
            if (players[i] == this)
                number = i + 1;
    }

    private void Update()
    {
        if (_gameManager.turn != number)
        {
            if (movable == false)
                if (!agent.pathPending)
                    if (agent.remainingDistance <= agent.stoppingDistance)
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {
                            _gameManager.NextTurn();
                            agent.enabled = false;
                            obstacle.enabled = true;
                            movable = true;
                        }
            if (movable == true)
                _cursorGrid.CaptureCursor(this);
        }
        if (_gameManager.turn == number)
        {
            obstacle.enabled = false;
            agent.enabled = true;
        }           
    }
}
