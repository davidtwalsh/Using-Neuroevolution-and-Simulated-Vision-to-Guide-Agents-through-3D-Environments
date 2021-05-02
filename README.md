# Using-Neuroevolution-and-Simulated-Vision-to-Guide-Agents-through-3D-Environments

## About the Program

  This project demonstrates the power of neuroevolution and fitness score by training agents (each led by a neural network) over many generations to solve visual obstacles and puzzles. Two simulations are included in the project: one where agents use raycasting as vision to navigate a procedurally generated catwalk to reach a goal, and one where agents each use a camera as vision to play the children’s game “red-light-green-light”. In both simulations, after many generations, each group of agents were able to complete their visual tasks. 

## How to use

   The ray casting simulation can be started by opening up the scene “RandomPathSim”, and the camera simulation can be started by opening up the scene “RedLightScene”. In both cases, the generation number can be advanced by pressing the “next gen” button located in the top left of the game window. The group of current neural networks can be saved using the “Save Neural Nets” button in the bottom left of the game window, and then can be loaded with the “Load Neural Nets” button in the bottom right of the window. In order to use the neural networks that I have trained over many generations, you must load the neural networks before saving over them. 

## The program in motion

Over many generations these agents below learn to follow the path to reach their goal without falling off by using raycasting as a vision source

![Catwalk (2)](https://user-images.githubusercontent.com/46041406/116799162-ae543c00-aac4-11eb-8c28-83fc4e19d4d0.gif)


Over many generations the agents below learn how to play the game red-light-green-light by constantly feeding the color data obtained from their attached cameras into their neural networks

![redlight](https://user-images.githubusercontent.com/46041406/116799268-ea3bd100-aac5-11eb-9aec-dc89d9137bb1.gif)



