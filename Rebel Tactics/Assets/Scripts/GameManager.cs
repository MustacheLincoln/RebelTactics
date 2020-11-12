using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text _turnText;

    public int turn = 0;
    public PlayerController[] players;

    void Awake()
    {
        players = FindObjectsOfType<PlayerController>();
    }

    private void Start()
    {
        players[turn].StartTurn();
        _turnText.text = turn.ToString();
    }

    public void NextTurn()
    {
        turn++;
        if (turn > players.Length - 1)
            turn = 0;
        players[turn].StartTurn();
        _turnText.text = turn.ToString();
    }
}
