using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planet.Audio.Engine
{	
	public class AudioMasterVolume
	{
		public enum BUS
		{
			MASTER,
			FX,
			MUSIC,
			UI,
			AMBIENCE,
			VOICE
		}

		public void Init()
		{
			for(int i= 0; i < Enum.GetNames(typeof(BUS)).Length; i++)
			{

			}
		}

		public static float GetBUSVolume(BUS Bus)
		{
			return PlayerPrefs.GetFloat(Bus.ToString());
		}

	}
}