using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerListPanel : BasePanel
{
    [SerializeField] private RectTransform _playerWidgetParent;
    [SerializeField] private RoomPlayerWidget _playerWidgetPrefab;

    private Dictionary<string, RoomPlayerWidget> _playersById = new Dictionary<string, RoomPlayerWidget>();

    public override void SetEnabled(bool isEnabled)
    {
        base.SetEnabled(isEnabled);

        if (!isEnabled)
            ClearPlayerlist();
    }

    public void AddPlayer(string playerId, string playerName, bool isMaster)
    {
        if (_playersById.ContainsKey(playerId))
            return;

        var newPlayer = GameObject.Instantiate(_playerWidgetPrefab, _playerWidgetParent);
        newPlayer.FillInfo(playerName, isMaster);
        _playersById.Add(playerId, newPlayer);
    }

    public void RemovePlayer(string playerId)
    {
        if (!_playersById.ContainsKey(playerId))
            return;

        GameObject.Destroy(_playersById[playerId].gameObject);
        _playersById.Remove(playerId);
    }

    public void UpdateMasterStatus(string masterId)
    {
        foreach (var player in _playersById)
            player.Value.UpdateMasterStatus(player.Key.Equals(masterId));
    }

    public void ClearPlayerlist()
    {
        foreach (var player in _playersById)
            GameObject.Destroy(player.Value.gameObject);

        _playersById.Clear();
    }
}
