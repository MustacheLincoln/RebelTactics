using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text _turnText;
    public int turn = 0;
    public EntityController[] entities;

    void Start()
    {
        _turnText.text = turn.ToString();
    }
    public void GetEntities()
    {
        entities = FindObjectsOfType<EntityController>();
        if (turn > entities.Length - 1)
        {
            turn = 0;
            entities[turn].StartTurn();
            _turnText.text = turn.ToString();
        }
    }

    public void NextTurn()
    {
        turn++;
        if (turn > entities.Length - 1)
            turn = 0;
        entities[turn].StartTurn();
        _turnText.text = turn.ToString();
    }
}
