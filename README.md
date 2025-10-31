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
How to Use This Project

If you want to clone or use this project on your own system, please follow these steps carefully.
This project uses Git LFS (Large File Storage) because it contains large .fbx and .png assets.

git lfs install
-

git clone https://github.com/iremkalayci/<your-repo-name>.git
-
cd <your-repo-name>
-
git lfs pull
-
Open Unity Hub
Click Add Project → select the project folder
Open the project with the same Unity version used for development (Unity 6000.2.7f2)
Wait until Unity imports all assets and generates the Library folder automatically.
After the import is complete, you can press Play and test the NPC AI system directly in the scene.
!
If models or textures appear missing, run this command again:
git lfs pull
This usually happens when Git LFS was not initialized before cloning.
Developed by İrem Kalaycı

