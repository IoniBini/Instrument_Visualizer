using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceDetection : MonoBehaviour
{
	//https://github.com/patrickhimes/microphone-demo/tree/4f80a624b34316d002cb5d7ddaa576164021f5d0

	public string microphone;
	private AudioSource audioSource;
	public List<string> options = new List<string>();
	public int audioSampleRate = 44100;

	void Start()
    {
		DetectAvailableInputs();

		UpdateMicrophone();
	}

	[ContextMenu("Detect Available Inputs")]
	public void DetectAvailableInputs()
    {
		options.Clear();

		//get components you'll need
		audioSource = GetComponent<AudioSource>();

		// get all available microphones
		foreach (string device in Microphone.devices)
		{
			if (microphone == null)
			{
				//set default mic to first mic found.
				microphone = device;
			}
			options.Add(device);

			Debug.Log(device);
		}
	}

	void UpdateMicrophone()
	{
		audioSource.Stop();
		//Start recording to audioclip from the mic
		audioSource.clip = Microphone.Start(microphone, true, 10, audioSampleRate);
		audioSource.loop = true;
		// Mute the sound with an Audio Mixer group becuase we don't want the player to hear it
		//Debug.Log(Microphone.IsRecording(microphone).ToString());

		if (Microphone.IsRecording(microphone))
		{ //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
			while (!(Microphone.GetPosition(microphone) > 0))
			{
			} // Wait until the recording has started. 

			Debug.Log("recording started with " + microphone);

			// Start playing the audio source
			audioSource.Play();
		}
		else
		{
			//microphone doesn't work for some reason

			Debug.Log(microphone + " doesn't work!");
		}
	}
}
