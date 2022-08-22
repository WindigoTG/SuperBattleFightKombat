using System;
using UnityEngine;

public class PlayerHealth
{
    int _maxHealth = 3;
    int _currentHealth;

    float _invincibilityPeriod = 1f;
    float _lastHitTime;

    public event Action Damage;
    public event Action<string> Death;

    PlayerView _playerView;

    public PlayerHealth()
    {
        Reset();
    }

    public void TakeDamage(int damage, string attackerID)
    {
        if (_currentHealth <= 0)
            return;

        if (Time.time - _lastHitTime > _invincibilityPeriod)
        {
            _lastHitTime = Time.time;

            _currentHealth -= damage;

            if (_currentHealth > 0)
                Damage?.Invoke();
            else
            {
                Death?.Invoke(attackerID);
            }

            _playerView?.SetPlayerHealth(_currentHealth);
        }

    }

    public void Reset()
    {
        _currentHealth = _maxHealth;
        _lastHitTime = Time.time;
        _playerView?.SetPlayerHealth(_currentHealth);
    }

    public void SetPlayerView(PlayerView view)
    {
        _playerView = view;
        _playerView.SetPlayerHealth(_currentHealth);
    }
}

