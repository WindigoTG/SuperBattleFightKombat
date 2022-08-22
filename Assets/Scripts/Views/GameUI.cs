using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameUI
{
    #region Fields

    [SerializeField] private RectTransform _playerStatsParent;
    [SerializeField] private PlayerStatsWidget _playerStatsPrefab;

    private Dictionary<string, PlayerStatsWidget> _playerStatsByID = new Dictionary<string, PlayerStatsWidget>();

    #endregion


    #region Methods

    public void AddPlayer(string playerId, string playerName, int lives, int score = 0)
    {
        if (_playerStatsByID.ContainsKey(playerId))
            return;

        var widget = GameObject.Instantiate(_playerStatsPrefab, _playerStatsParent);
        widget.SetPlayerName(playerName);
        widget.SetLivesAmount(lives);
        widget.SetScoreAmount(score);
        _playerStatsByID.Add(playerId, widget);
    }

    public void RemovePlayer(string playerID)
    {
        if (!_playerStatsByID.ContainsKey(playerID))
            return;

        GameObject.Destroy(_playerStatsByID[playerID].gameObject);
        _playerStatsByID.Remove(playerID);
    }

    public void UpdateScoreForPlayer(int score, string playerID)
    {
        if (_playerStatsByID.ContainsKey(playerID))
            _playerStatsByID[playerID].SetScoreAmount(score);
    }

    public void UpdateLivesForPLayer(int lives, string playerID)
    {
        if (_playerStatsByID.ContainsKey(playerID))
            _playerStatsByID[playerID].SetLivesAmount(lives);
    }

    #endregion
}
