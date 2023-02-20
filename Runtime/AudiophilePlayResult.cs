using System;
using UnityEngine;

namespace PixelDust.Audiophile
{
    public class AudiophilePlayResult
    {
        private AudiophilePlayer audiophilePlayer;
        public AudiophilePlayer AudiophilePlayer => audiophilePlayer;

        public bool? Loop
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.loop : false;
            }
            set
            {
                this.audiophilePlayer?.SetOverrideLoop(value);
            }
        }

        public float? Volume
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.audioSource.volume : 0;
            }
            set
            {
                this.audiophilePlayer?.SetOverrideVolume(value);
            }
        }

        public float? Pitch
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.audioSource.pitch : 0;
            }
            set
            {
                this.audiophilePlayer?.SetOverridePitch(value);
            }
        }

        public bool Persist
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.Persist : false;
            }
            set
            {
                this.audiophilePlayer?.SetPersist(value);
            }
        }

        public float SoundLength
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.audioSource.clip.length / Math.Abs(audiophilePlayer.audioSource.pitch): 0;
            }
        }

        public Transform FollowTransform
        {
            get
            {
                return audiophilePlayer ? audiophilePlayer.FollowTransform : null;
            }
            set
            {
                this.audiophilePlayer?.SetFollowTransform(value);
            }
        }

        public event Action<AudiophilePlayResult> onLoop;

        public AudiophilePlayResult(AudiophilePlayer audiophilePlayer)
        {
            this.audiophilePlayer = audiophilePlayer;
            this.audiophilePlayer.onStopped += OnStopped;
            this.audiophilePlayer.onLooped += () => onLoop?.Invoke(this);
        }

        private void OnStopped()
        {
            audiophilePlayer = null;
        }

        public void Stop()
        {
            if (this.audiophilePlayer)
            {
                this.audiophilePlayer.Stop();
            }
            else
            {
                Debug.Log("Audiophile - Audiophile player is null");
            }
        }
    }
}
