# Instrument_Visualizer

Ionatan Biniamin Maghidman

Git Repo: https://github.com/IoniBini/Instrument_Visualizer

------------------------------------------------

Description:
A tool that allows you to connect instruments into unity via audio interface, and have them translated into visuals in real time

------------------------------------------------

Equipment needed to connect an instrument to this application:
- An audio interface
- Make sure that the audio interface's driver is updated
- Any instrument with an input jack or USB entrance, such as a guitar, keyboard or even a microphone
- A TRS 1/4” Cable to XLR cable (for instrument to interface connection)
- A computer with this projected downloaded (lol)

------------------------------------------------

How to use:

PLEASE NOTE THAT THE ONLY REASON I AM NOT SUBMITTING A BUILT VERSION AT FIRST is because this application works best when you build it yourself...
I will soon send an updated assignment file which contains a build that should work for general purposes, but as I already have explained to Lachlan in class, it is best if you follow the steps below and
then just make a build for yourself...

I have created a sample scene called "SampleScene" which is already totally set up and ready for use. If you want to use this set up, here are the steps to using the application:

1. Make sure the number of children inside the "InstrumentParent" is equivalent to the EXACT number of instruments you wish to capture
2. Go inside each "Instrument_Visualizer" child obj. Right click the "Voice Detection" script, and select the "Detect Available Inputs" option
3. The list of devices will be populated by the names of each the devices connected to this computer
4. Simply copy the device name you wish to use and paste it in the field called "Microphone"
5. It is all set up, and you are now able to change the settings of the "InstrumentVisualizer5" to fine tune the capturing and visuals of your instrument (all the variables have tool tips that explain what each thing does)
6. Press play and watch the magic!

Once this is all set up correctly, you may build the current scene under File>BuildSettings, then "AddOpenScenes", and you can build to whatever location you wish. This build will now work for your
currently set up chosen device.

In case you do NOT want to use the sample scene, and want to set it all up yourself, here are the steps to creating a scene that will work:

1. Connect your updated interface to the computer
2. Connect your instrument to the interface (do not forget to make the gain of both the interface and that of your instrument high enough to be captured)
3. Create a scene which must contain the prefab called "InstrumentParent"
4. Drag the prefab "Instrument_Visualizer" into the "InstrumentParent" as a child of it
5. Go in your scene view and click Window>Rendering>Lighting, under the Environment tab, you can set the material of the skybox. You should set it to the material called "sky" inside the Materials folder
6. You are done! Now go back to the first part of the "How to use" here and continue from there

------------------------------------------------

Summary of the procedural system's intent:

To highlight the dynamics of a live preformance, from the individual instrument level to the whole orchestra together.
There are 3 "levels of capture and display":
1. Micro Level: the most prominent frequencies (notes and chords) that a particular instrument is playing, displaying them as groups of frequency bands with their own modifiers.
2. Mid Level: the individual instrument's summed output which comes as a result of the average of its bands. This visualizes the particular instrument's moments of most and least participation in a song.
3. Macro Level: the orchestras's overall output intensity, which manifests as a byproduct of unison of multiple instruments simultaniously

------------------------------------------------

A list of all third-party assets, code, sounds, etc. used in the creation of the project:

Instrument detection: https://github.com/patrickhimes/microphone-demo/tree/4f80a624b34316d002cb5d7ddaa576164021f5d0
I learned some principles of audio collection from this video series, but I copied little to no code at all: https://www.youtube.com/watch?v=Ri1uNPNlaVs&t=652s
I also had to learn shader code from scratch, some of which I learned by studying (not copying) these:
https://forum.unity.com/threads/custom-shader-on-skybox.583525/
https://learn.unity.com/tutorial/writing-your-first-shader-in-unity#
https://www.ronja-tutorials.com/post/047-invlerp_remap/

------------------------------------------------

Extra comments:

This repo has a sample scene as well as a video attached which displays what this tool is capable of achieving.
To get the most out of this tool, I HEAVILY recommend that you do not simply use the sample scene within the editor, because it is not catered for the particular details of the instrument YOU might choose to connect.
Instead, I recommend that you download the project, connect your instrument in, and play around with the parameters so that it suits your instrument the best, and ONLY THEN make a build yourself, and use THAT as your visualizer.
