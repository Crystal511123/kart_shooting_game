using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Tooltip("The start/finish line.")]
    public GameObject StartFinishLine;
    [Tooltip("The list of checkpoints.")]
    public GameObject[] CheckPointList;

    [Tooltip("Coin Object")]
    public GameObject Coin;
    [Tooltip("Coin Spawn Locations")]
    public GameObject[] CoinSpawns;

    private int _gameState = -1;
    private int _checkPointNum;
    private Collider _startLineCollider;

    private int _score = 0;

    private float _startTime;
    private bool _isPlaying = false;
    public Text TimerText;

    void Start()
    {
        _checkPointNum = CheckPointList.Count();
        for (int i = 0; i < _checkPointNum; i++)
        {
            CheckPointList[i].SetActive(false);
        }
        _startLineCollider = StartFinishLine.GetComponent<Collider>();

        foreach (GameObject CoinSpawn in CoinSpawns)
            Instantiate(Coin, CoinSpawn.transform.position, CoinSpawn.transform.rotation);
    }
    
    private void Update()
    {
        if (_isPlaying)
        {
            UpdateTimer();
        }
    }
    public void LoadCheckPoint()
    {
        if (_gameState == -1)
        {
            _startTime = Time.time;
            _isPlaying = true;
            _startLineCollider.enabled = false;
        }
        else if (_gameState == _checkPointNum)
        {
            Debug.Log("Finish!");
            _isPlaying = false;
            _startLineCollider.enabled = false;
            return;
        }
        else
        {
            Debug.Log(_gameState);
            CheckPointList[_gameState].SetActive(false);
        }
        _gameState++;
        if (_gameState == _checkPointNum)
        {
            _startLineCollider.enabled = true;
        }
        else
    {
            CheckPointList[_gameState].SetActive(true);
        }
    }

    public void Score()
    {
        _score++;
    }
    private void UpdateTimer()
    {
        float currentTime = Time.time - _startTime - _score;
        currentTime = Mathf.Max(0, currentTime);
        float minutes = (int)(currentTime / 60);
        float seconds = currentTime % 60;
        TimerText.text = string.Format("{0:0}:{1:00.00}", minutes, seconds);
    }
}