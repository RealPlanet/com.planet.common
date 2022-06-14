using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volume = AudioMasterVolume;

namespace Planet.Audio.Engine{
public class AudioPlayer : MonoBehaviour
{
    private AudioSource _Source;
    public AudioMasterVolume.BUS Bus;
		private void Awake()
		{
			_Source = gameObject.AddComponent<AudioSource>();
			AudioEngine.OnAudioBusChanged += AudioEngine_OnAudioBusChanged;
		}

		private void AudioEngine_OnAudioBusChanged(Volume.BUS Bus)
		{
			if (Bus != this.Bus)
			{
				return;
			}

			_Source.volume = Volume.GetBUSVolume(Bus);
		}

		private void OnDestroy()
		{
			AudioEngine.OnAudioBusChanged -= AudioEngine_OnAudioBusChanged;
			Destroy(_Source);
		}

		public IEnumerator PlayCoroutine(AudioClip Clip, Volume.BUS Bus, bool Looping)
		{
			_Source.clip = Clip;
			_Source.volume = Volume.GetBUSVolume(Bus);
			_Source.loop = Looping;
			yield return null;

			_Source.Play();
		}
	}
}
