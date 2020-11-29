# RealismController

## About The Controller

THIS IS NOT A TUTORIAL PROJECT. THIS IS VERY ADVANCED AND YOU MUST ALREADY HAVE AN UNDERSTANDING OF THE UNITY ENGINE

The Unity Realism controller was created to give full control of the player's movement, allowing players to move through environments seamlessly and realistically. The scripts currently allow for smooth speed transitioning, animation state management, movement state management, IK integration and more. Full feature list below. The character controller and camera controller scripts both work independently allowing for integration into your current projects. However, the scripts will work together if they have the chance to. Contributions are encouraged. Happy programming!

## Features

RealismController 0.5:

* Custom editor UI
* Different types of movement styles.
* Light Prop Hunt Support
* Light First-Person Support
* Controller Support (This is currently supports the old input system, a version with Unity's new input system will be released soon)
* Independent movement speeds for each state (walking, jogging, running)
* Animation State Manager (List of states below)
* Adjustable turn/rotation smoothing on the Y-Axis
* Smooth Speed Transitioning (Your speed when transition between walking, running, and jogging states is smoothed out)
* Disable Jogging (Turn off jogging completely)
* Disable Running (Turn off running completely)
* Toggle-Mode Sprinting (Press sprint button once to toggle, otherwise the sprint button will need to be held down)
* Player Gravity
* Disable Jumping (Turn off jumping completely)
* Different types of jumps/jump states (Standing Jump, Walk Jump, Jogging Jump, Running Jump)
* Head IK Movement (IF active and set correctly. The character's head will look which way you do)

RealismCamera 0.9:

* Custom editor UI
* Gamepad/Controller Support also only on the old input system at the moment.
* Adjustable follow distance
* Smooth position lerping
* Smooth rotation damping
* Mouse & Joystick Sensitivity
* Invert X/Y rotation

## Setting up the Camera and Controller

## Inputs

Before setting up anything you need to decide your project's input system. Currently the Realism Controller only works with the old Unity input system. The easiest way to get this working is to add the included InputManager.asset file to the ProjectSettings folder located in the root of your Unity project. THIS WILL REPLACE ALL OF YOUR CURRENT INPUTS

If you don't want to replace your current input manager, then you can create new axes for the inputs listed below, or change the name of the inputs in the scripts to match the ones in your projects.

## Default Inputs Used

Inputs being used in the scripts (Case-Sensative) :

For Joystick Inputs:

* JoystickHorizontal
* JoystickVertical
* JoystickLookX
* JoystickLookY
* JoystickSprint

Required Mouse Axes (Included in all Unity Projects by default) :

* MouseX
* MouseY

Keyboard Inputs (These do not need to be added as they are only keyboard inputs, however these can be changed in the RealismController.cs) :

* KeyCode.LeftControl
* KeyCode.LeftShift

Other:

* Jump
* LockRotation (Used for prop hunt controller)

## RealismCamera.cs

Add RealismCamera.cs to your camera that will be following the player. Assign a target, and this script will work right away. Adjust the values in the RealismCamera inspector to fit your project's needs and for better results.

## RealismController.cs

Simply add character to your scene, then attach the script to the character or the GameObject containing the character's body. Adding the script will automatically add a Character Controller if one is not already there. 

You will need to add an Animator then assign the Animation Controller and Avatar. Make sure Root Motion is turned on. Use the included Animation Controller (SampleController.controller). When you open the controller you'll notice that the states are there but they do not have animations. You can download or create animations, and assign them in their appropriate states. I mentioned above that this is not a tutorial project so I will not show how to create the Animation Controller. However the animation states that are currently being used will always be listed in this readme.md.

Size the Character Controller correctly. Then in the RealsimController inspector, change the controller type, and assign the CameraTransform. Change the rest of values to match your project's needs. For the smoothest look and results, please make sure the walk, jog, and run speeds match their animations.

<img src="https://i.imgur.com/FfGVX8g.png" />

If you have set up the inputs correctly as explained above, everything will work fine. If not, then please review the instructions again.

## Supported Animation States (Case-Sensative):

Below are the supported Animation states. You will need these to create an Animation Controller for your character. Create these as boolean parameters in your Animation Controller:

RealismController 0.5

* Grounded
* Walking
* Jogging
* Running
* Jumping

## Animations

Animations are not included with the scripts, but they can be acquired by going to mixamo.com and uploading or downloading a character.

## In the Works

RealismController 0.7 (Coming soon)

* Crouching
* Foot IK (Adjusted the feet along stairs, slopes, obstacles, and terrain)
* More reliable ground detection
* Improvements on current jumping system
* New animation states
* Controller will measure distance to the ground to decide which landing animation to choose

## Realism Health Manager 0.1 (Coming Soon)

The Realism Health Manager is the next upcoming module for the Characer Controller. This will be a independent script that will work with the character controller to calculate health, armor, stamina, etc.

## Contact Me

Sorioxcode@gmail.com

Project Link: https://github.com/soriox/RealsimController
