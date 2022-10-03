using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceDetection : MonoBehaviour
{
	//https://github.com/patrickhimes/microphone-demo/tree/4f80a624b34316d002cb5d7ddaa576164021f5d0

	//most of this code is just the same as the link above, but I just adjusted one or two things to make it easier to use in editor

	[Tooltip("The audio device which this obj will be using")]
	public string microphone;
	private AudioSource audioSource;
	[Tooltip("The list of available audio devices")]
	public List<string> options = new List<string>();
	private int audioSampleRate = 44100;

	void Start()
    {
		DetectAvailableInputs();

		UpdateMicrophone();
	}

	[ContextMenu("Detect Available Inputs")]
	public void DetectAvailableInputs()
    {
		options.Clear();

		//get the source from the object
		audioSource = GetComponent<AudioSource>();

		// get all available microphones connected in the computer
		foreach (string device in Microphone.devices)
		{
			//stops the process in case no mic is plugged
			if (microphone == null)
			{
				//set default mic to first mic found.
				microphone = device;
			}
			//adds all devices into a list
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
