using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCurrentState : MonoBehaviour
{
    private PlayerStateMachine _playerStateMachine;
    private GameObject Player;
    private Text _text;
    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<Text>();
         Player = GameObject.FindWithTag("Player");

        if (Player != null)
        {
            _playerStateMachine = Player.GetComponent<PlayerStateMachine>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = _playerStateMachine.DebugCurrentState;
    }
}
