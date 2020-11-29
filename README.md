# RealismController

## About The Controller

THIS IS NOT A TUTORIAL PROJECT. THIS IS VERY ADVANCED AND YOU MUST ALREADY HAVE AN UNDERSTANDING OF THE UNITY ENGINE

The Unity Realsim controller was created to give full control of the player's movement, allowing players to move through environments seamlessly and realistically. The scripts currently allow for smooth speed transitioning, animation state management, movement state management, IK integration and more. Full feature list below. The character controller and camera controller scripts both work independently allowing for a no hassle integration into your current projects. However, the scripts will work together if they have the chance to. Contributions are encouraged. Happy programming!

## Features

RealismController 0.5:

* Different types of movement styles.
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

* Gamepad/Controller Support also only on the old input system at the moment.
* Adjustable follow distance
* Smooth position lerping
* Smooth rotation damping
* Mouse & Joystick Sensitivity
* Invert X/Y rotation

## Setting up the Realism Controller

Simply add character to your scene, then attach the script to the character or the GameObject containing the character's body. Adding the script will automatically add a Character Controller if one is not already there. You will need to add an Animator along with the controller and avatar if you are using animations. Size the Character Controller correctly, change the controller type, and assign the CameraTransform in the RealsimController inspector (the camera that will follow the player).

<img src="https://i.imgur.com/FfGVX8g.png" />

Change the values to match your project desires and you are ready to go. This will not work right away without the included 

## Supported Animation States (Case-Sensative):

Below are the supported Animation states. You will need these to create an Animation Controller for your character. 

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
