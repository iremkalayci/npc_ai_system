his repository includes the enemy AI system I made from scratch in Unity 6000.2.7f2.
It’s a simple but solid setup where enemies can patrol, notice the player, chase, and attack using a state-based logic.
-

This system is now used inside the main project https://github.com/taha907/TemuDeltaForce
My own player controller isn’t part of that game, but all the NPC behavior and AI code here were created by me.

Features

-Finite State Machine with basic states: Idle, Patrol, Chase, Attack
-Player detection based on distance and view angle
-Movement handled with NavMeshAgent
-Animation-driven attack events
-Clean, reusable C# scripts
-Works easily in other Unity projects

Tech
-Engine: Unity 6000.2.7f2
-Language: C#
-Tools: NavMesh, Animator, Mixamo, Visual Studio, Git
