# Gostranner
![Gostranner menu](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/GostrannerImage.jpg)  

This is a personal project inspired by Ghostrunner.  
I made it in Unity during a summer.  

I focused on the locomotion system, attack/defense and parry of the enemy projectiles.  
I also made a mesh cut algorithm because I liked the idea of cutting things in little pieces; although it's not very efficient, I really enjoyed developing it.  

* [Locomotion](#Locomotion)
* [Combat](#Combat)
* [Mesh cut](#Mesh-Cut)

<a name="Locomotion"></a>
## Locomotion
The locomotion system is based on a FSM that handles all the different movement states.  
The state controller can be found [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/Player/FSM/StateControllers/PlayerMovementStateController.cs) while all the movement states can be found [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/tree/main/Assets/Scripts/Player/FSM/States/Movement%20States).  

Every state controller in this project contains two variables to know what is the previous state (`previousState`) and what will be the next state (`nextTargetState`). In this way, when one state requests a state transition to another state, it can execute custom logic based on the previous/next state.  


<a name="Combat"></a>
## Combat


<a name="Mesh-Cut"></a>
## Mesh cut
