using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text _turnText;

    public int turn = 1;

    public void NextTurn()
    {
        turn++;
        var players = FindObjectsOfType<PlayerController>().Length;
        if (turn > players)
            turn = 1;

        _turnText.text = turn.ToString();
    }
}
