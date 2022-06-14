using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volume = AudioMasterVolume;
public static class AudioEngine
{
    public static void PlayAudio(AudioClip Clip, GameObject Parent, Volume.BUS Bus = Volume.BUS.FX, bool Looping = false)
    {
        // True if found
        AudioPlayer[] Players = Parent.GetComponents<AudioPlayer>();
        AudioPlayer Player = null;

        if(Players.Length > 0)
        {
            for(int i = 0; i < Players.Length; i++)
            {
                if(Players[i].Bus == Bus)
                {
                    Player = Players[i];
                    break;
                }
            }
        }

        // If component with correct bus does not exist, create it
        if (Player == null)
        {
            Player = Parent.AddComponent<AudioPlayer>();
            Player.Bus = Bus;
        }
           
            
        _Players.Add(Player);
       
        Player.StartCoroutine(Player.PlayCoroutine(Clip, Bus, Looping));
    }

    public static void AudioSettingsChanged(Volume.BUS Bus) => OnAudioBusChanged?.Invoke(Bus);

    /// <summary>
    /// Deletes all cached AudioPlayers or only
    /// </summary>
    /// <param name="Parent"></param>
    private static void _ClearAudioPlayers(GameObject Parent = null)
    {
        for(int i = _Players.Count - 1; i >= 0 ; i--)
        {
            if(Parent == null || (Parent != null && _Players[i].gameObject == Parent))
                GameObject.Destroy(_Players[i]);
        }

        _Players.Clear();
    }

    public static void ClearAllAudioPlayers() => _ClearAudioPlayers();
    public static void ClearAudioPlayers(this GameObject Parent) => _ClearAudioPlayers(Parent);

    public delegate void AudioBusChanged(Volume.BUS Bus);
    public static event AudioBusChanged OnAudioBusChanged;

    private static List<AudioPlayer> _Players = new List<AudioPlayer>();
}
