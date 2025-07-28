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
In the specific, `previousState` is always non null except at the start when entering the first state; `nextTargetState` becomes null before entering a state, so this can be useful in the `Exit` method of a state.  

```
        public override async UniTask ChangeState(IState state)
        {
            if (currentState == (T)state || changingState)
                return;

            changingState = true;
            nextTargetState = (T)state;

            if (currentState != null)
                await currentState.Exit();

            nextTargetState = null;
            previousState = currentState;

            await state.Enter();
            currentState = (T)state;
            changingState = false;
        }
```

Each state has a scriptable object containing the settings for the logic of that specific state.  
One of the most complex movement states is the wall run one.  

### Wall Run
Player can enter in the wall run state only when falling mid air.  
The check is done in the [BaseFallingState](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/Player/FSM/States/Movement%20States/FallingState/BaseFallingState.cs)'s Tick as follow:  

```
        protected void CheckWallRun()
        {
            Vector3 wallCheckOrigin = player.transform.position + Vector3.up * wallRunStateModel.wallRayHeightOffset;
            rightHit = Physics.Raycast(wallCheckOrigin, player.transform.right, out rightWallHitInfo, wallRunStateModel.wallRayLenght, wallRunStateModel.wallCheckLayers);
            leftHit = Physics.Raycast(wallCheckOrigin, -player.transform.right, out leftWallHitInfo, wallRunStateModel.wallRayLenght, wallRunStateModel.wallCheckLayers);

            if ((rightHit && IsVerticalWall(rightWallHitInfo)) || (leftHit && IsVerticalWall(leftWallHitInfo)))
            {
                wallDirection = Vector3.Cross(player.transform.up, rightHit ? rightWallHitInfo.normal : leftWallHitInfo.normal);
                if (Vector3.Dot(player.transform.forward, wallDirection) < 0)
                    wallDirection *= -1;
            }
            else
                wallDirection = Vector3.zero;

            if (wallDirection != Vector3.zero
                && Vector3.Dot(player.inputMovementDirection, wallDirection) > wallRunStateModel.forwardDotFallThreshold
                && Vector3.Dot(player.transform.forward, wallDirection) > wallRunStateModel.forwardDotFallThreshold)
            {
                controller.ChangeState(wallRunState).Forget();
            }
        }
```
So, two raycasts (one left and one right) are shoot from the player to verify the presence of a wall.  
The result of the raycasts is passed to the `IsVerticalWall` function that simply checks if the object hit is perpendicular to the ground by calculating a dot product:  

```
protected bool IsVerticalWall(RaycastHit hitInfo) => Vector3.Dot(player.transform.up, hitInfo.normal) == 0.0f;
```

Next, the `wallDirection` is calculated; this is the vector parallel to the wall and in the same direction of the player's forward. This is used as a vector reference to determine if the player is looking and pressing the input in that direction (within a certain range); if this condition is not met, the player will not enter the wall run state or will fall if already in it.  

When entering the wall run state, gravity is disabled, the camera is tilted and the player starts moving along the wall direction with a certain speed.  
This is the logic executed in the Tick of the state:  

```
        public override void Tick()
        {
            wallCheckOrigin = player.transform.position + Vector3.up * wallRunStateModel.wallRayHeightOffset;

            if (Vector3.Dot(player.inputMovementDirection, wallMovementDirection) < wallRunStateModel.forwardDotFallThreshold
                || Vector3.Dot(player.transform.forward, wallMovementDirection) < wallRunStateModel.forwardDotFallThreshold
                || !Physics.Raycast(wallCheckOrigin, -wallNormal, out wallHit, wallRunStateModel.wallRayLenght, wallRunStateModel.wallCheckLayers))
            {
                controller.ChangeState(idleState).Forget();
            }
            else
            {
                if (jumpAction.triggered)
                    controller.ChangeState(parabolicJumpState).Forget();

                if(dashAction.triggered)
                    controller.ChangeState(dashState).Forget();

                player.characterController.Move(wallMovementDirection * wallRunStateModel.wallRunSpeed * Time.deltaTime);
            }
        }
```

Player will fall if one of the following conditions occur:  
1. no input movement
2. input movement is not in the same direction of the wall movement direction (considering the `forwardDotFallThreshold`)
3. player's forward (which is equal to the looking direction) is not in the same direction of the wall movement direction (considering the `forwardDotFallThreshold`)
4. wall ends

All this checks are done by using dot products and a raycast.  

Player can also exit the wall run state with a jump in the direction he's looking.  

```
            if (controller.nextTargetState == parabolicJumpState)
            {
                Vector3 jumpDirection = Quaternion.AngleAxis(rightSide ? -wallRunStateModel.minJumpDirectionAngle : wallRunStateModel.minJumpDirectionAngle, Vector3.up) * wallMovementDirection;
                float jumpDotThreshold = Mathf.Sin(wallRunStateModel.minJumpDirectionAngle);
                
                if (Vector3.Dot(player.transform.forward, wallMovementDirection) < jumpDotThreshold
                    && Vector3.Dot(player.transform.forward, wallNormal) > 0)
                {
                    jumpDirection = player.transform.forward;
                }

                ((PlayerParabolicJumpState)parabolicJumpState).jumpDirection = jumpDirection;
            }
```

As you can see, there is a minimum jump angle from the wall (`minJumpDirectionAngle`); this is done to avoid situations in which the player could simply climb the wall by jumping along its direction.  

See the wall run state [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/Player/FSM/States/Movement%20States/WallRunState/PlayerWallRunState.cs).

<a name="Combat"></a>
## Combat
The combat system is based on another FSM that works in parallel with the movement one and handles all the different combat states.  
The state controller can be found [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/Player/FSM/StateControllers/PlayerCombatStateController.cs) while all the combat states can be found [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/tree/main/Assets/Scripts/Player/FSM/States/Combat%20States).  

The combat system consists of three states: two attacks and a defense one.  
The attacks can be concatenated with a simple combo system. The player's sword invokes, with an animation event, the opening of the combo window; the attack state subscribe to the event when entering and if the attack input is pressed in this window, the bool `comboTrigger` is set to true. The sword animation, at the end invokes an event that calls the `EndAttack` method in the attack state, eventually passing to the next attacking state:  

```
        private void EndAttack()
        {
            if (comboTrigger)
                controller.ChangeState(nextAttackState).Forget();
            else
                controller.ChangeState(controller.initialState).Forget();
        }
```

In the defense state the player can defend himself from the enemy's bullets. When a bullet hits the player while in this state, a function is called:  

```
        private void EvaluateBulletHit(Bullet bullet)
        {
            if (defending && Vector3.Dot(player.fpCamera.transform.forward, -bullet.transform.forward) > thresholdCosine)
            {
                if (timeSinceEnter <= defenseModel.parryWindow)
                {
                    float angle = Random.Range(0f, Mathf.PI);
                    float radius = Random.Range(defenseModel.minRepositioningRadius, defenseModel.maxRepositioningRadius);
                    Vector3 randomOffset = player.fpCamera.transform.right * Mathf.Cos(angle) * radius + player.fpCamera.transform.up * Mathf.Sin(angle) * radius;
                    bullet.transform.position += randomOffset;
                    bullet.Parry();
                }
                else
                    player.stamina.ConsumeStamina(bullet.bulletModel.playerStaminaConsume);
            }
            else
                player.Death();
        }
```

As you can see, the hit is evaluated with a dot product to determine whether the player is facing the bullet within a certain range defined by the `thresholdCosine` value. If this condition is not met, it's game over, otherwise, the parry logic condition is evaluated.  
The bullet can be parried if the time elapsed since entering the state is less than or equal to a `parryWindow` defined in the scriptable object of the defense state itself. If the condition is met, the bullet is randomly repositioned for cosmetic purposes and then the `bullet.Parry()` is called, reversing the direction of the bullet and returning it to the enemy.  

See the defense state [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/Player/FSM/States/Combat%20States/Defense/PlayerDefenseState.cs) and the bullets [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/tree/main/Assets/Scripts/Projectiles).  

<a name="Mesh-Cut"></a>
## Mesh cut
When attacking, the meshes of the enemies (and also some cubes in the Test Area scene) can be cut in half.  
Every gameobject that can be cut has [this script](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/MeshCut/Sliceable.cs) that handles a couple of things:  
* the **cut cooldown**: there is a cooldown to avoid consecutive unwanted cuts; a coroutine is used to prepare the gameobject for the cut:  
```
        private async UniTask PrepareForCut()
        {
            await UniTask.WaitForSeconds(cutReadyCooldown);
            cutReady = true;
        }
```
* **maximum number of cuts**: it is possible to set a maximum number of cuts to limit the number of gameobjects generated in the scene, with the goal of limiting performance deterioration; once the max number is reached and the gameobject goes out of sight, it is destroyed:
```
        private void OnBecameInvisible()
        {
            if (cutNumber >= maxCuts)
                Destroy(gameObject);
        }
```

The core of the mesh cut feature resides in [this script](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/MeshCut/MeshCutter.cs).  
The logic processes all the triangles of the target mesh generating two gameobjects (actually only one because the original one gets modified).  
The triangle, which is composed of vertices, normals and uvs (see [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/MeshCut/MeshTriangle.cs)), is split into two parts if the cutting plane passes through it (this check is done with the `plane.GetSide` function), otherwise is simply put in the left or right gameobject.  
Splitting a triangle is where the things gets a bit tricky, here is a "simple" explanation of what happens:  
* a raycast is performed from the two vertices on one side to the third one on the other side of the plane
* the two resulting intersection points (see [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/blob/main/Assets/Scripts/MeshCut/IntersectionPoint.cs)) with the plane are calculated and their vertex, normal and uv are calculated by lerping the vertex, normal and uv of the two vertices that generated that intersection
* considering the three original vertices and the two new intersection points, new triangles are created (correcting their facing) and added to the left/right gameobjects

Once this logic is done, the only thing that remain to do is filling the hole in the two meshes that has been created by the cut.  
It is calculated the center location of the hole:  

```
            // calculate the center of the intersection points of the mesh
            // (left and right meshes have the same intersection points, so it doesn't make any difference using one or the other)
            Vector3 holeCenter = Vector3.zero;
			for (int i = 0; i < leftMesh.intersectionPoints.Count; i++)
				holeCenter += leftMesh.intersectionPoints[i].vertex;
			holeCenter /= leftMesh.intersectionPoints.Count;
```

And then the triangles are created.  

Although this system is not performant at all, I had a lot of fun developing it and understanding how the meshes works. I think delegating all the hard work to the GPU or use the Unity's job system might help improve performances.  

For more details on the mesh cut, you can find the scripts [here](https://github.com/AlessandroSimeoni/GhostrunnerLike-VerticalSlice/tree/main/Assets/Scripts/MeshCut).  
