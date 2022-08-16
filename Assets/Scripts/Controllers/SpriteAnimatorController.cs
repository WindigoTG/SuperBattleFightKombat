using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimatorController : IDisposable, IUpdateableRegular

{
    #region Inner Classes

    private class Animation
    {
        public AnimationTrack Track;
        public List<Sprite> Sprites;
        public bool Loop = false;
        public float Speed = 10;
        public float Counter = 0;
        public bool IsActive;

        public void Update()
        {
            if (!IsActive) return;

            Counter += Time.deltaTime * Speed;
            if (Loop)
            {
                while (Counter > Sprites.Count)
                {
                    Counter -= Sprites.Count;
                }
            }
            else if (Counter > Sprites.Count)
            {
                Counter = Sprites.Count - 1;
                IsActive = false;
            }
        }
    }

    #endregion


    #region Fields

    private SpriteAnimationsConfig _config;
    private Dictionary<SpriteRenderer, Animation> _activeAnimations = new Dictionary<SpriteRenderer, Animation>();

    #endregion


    #region Class Life Cycle

    public SpriteAnimatorController(SpriteAnimationsConfig config)
    {
        _config = config;
    }

    #endregion


    #region Methods

    public void StartAnimation(SpriteRenderer spriteRenderer, AnimationTrack track, bool loop, float speed)
    {
        if (_activeAnimations.TryGetValue(spriteRenderer, out var animation))
        {
            animation.Loop = loop;
            animation.Speed = speed;
            animation.IsActive = true;
            if (animation.Track != track)
            {

                if (!(track == AnimationTrack.AttackRun && animation.Track == AnimationTrack.Run) &&
                    !(track == AnimationTrack.Run && animation.Track == AnimationTrack.AttackRun))
                    animation.Counter = 0;

                animation.Track = track;
                animation.Sprites = _config.Sequences.Find(sequence => sequence.Track == track).Sprites;
            }
        }
        else
        {
            _activeAnimations.Add(spriteRenderer, new Animation()
            {
                Track = track,
                Sprites = _config.Sequences.Find(sequence => sequence.Track == track).Sprites,
                IsActive = true,
                Loop = loop,
                Speed = speed
            });
        }
    }

    public void StopAnimation(SpriteRenderer sprite)
    {
        if (_activeAnimations.ContainsKey(sprite))
        {
            _activeAnimations.Remove(sprite);
        }
    }

    public bool IsAnimationFinished(SpriteRenderer spriteRenderer)
    {
        if (_activeAnimations.TryGetValue(spriteRenderer, out var animation))
        {
            if (animation.IsActive)
                return false;
        }
        return true;
    }

    public bool HasAnimation(AnimationTrack track)
    {
        var sequence = _config.Sequences.Find(sequence => sequence.Track == track);
        return sequence != null;
    }

    #endregion


    #region IUpdateable

    public void UpdateRegular()
    {
        foreach (var animation in _activeAnimations)
        {
            animation.Value.Update();
            animation.Key.sprite = animation.Value.Sprites[(int)animation.Value.Counter];
        }
    }

    #endregion


    #region IDisposable

    public void Dispose()
    {
        _activeAnimations.Clear();
    }

    #endregion
}