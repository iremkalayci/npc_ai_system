AI Powered TPS Game

This repository contains the main game project developed and maintained using Unity 6000.2.7f2.
It includes a complete Enemy AI system, a custom sci-fi environment, and multiple gameplay systems built from scratch.

The project currently serves as the core build in our collaborative third-person shooter (TPS) development.
All AI logic, NPC behavior, and scene design were implemented here. This version is the foundation used by all contributors.

Features

Finite State Machine with core AI states: Idle, Patrol, Chase, Attack

NPCs detect, follow, and attack the player dynamically

UFO system that moves between waypoints and attacks from above

Pumpkin heal mechanic added for the Halloween update (restores player health when collected)

Full NavMesh pathfinding integration

Clean, reusable C# scripts designed with object-oriented principles

Custom sci-fi corridor and environment setup

Works with Mixamo animations and humanoid rigs

Optimized for smooth gameplay and realistic transitions

Technology Stack
Component	Description
Engine	Unity 6000.2.7f2
Language	C#
Tools	NavMesh, Animator, Mixamo, Visual Studio, Git, Git LFS
Platform	PC (expandable for mobile)
Scene Overview

The main level takes place in a sci-fi facility environment.
It includes custom lighting, moving spaceships, animated elements, and several AI-controlled NPCs.
Players can explore the area, interact with healing pumpkins, and experience different enemy behaviors such as patrolling, chasing, and attacking.

The scene is built as the primary gameplay level and can serve as the base for future stages or missions.

How to Clone and Run

This project uses Git LFS (Large File Storage) because it contains large FBX, PNG, and WAV assets.

Install Git LFS

git lfs install


Clone the repository

git clone https://github.com/iremkalayci/npc_ai_system.git


Enter the folder

cd npc_ai_system


Pull the large files

git lfs pull


Open the project in Unity Hub, select “Add Project”, and choose this folder.
Use Unity 6000.2.7f2 as the version.
Wait for Unity to import all assets and build the Library folder automatically.
After import is complete, press Play to run the scene.

If any models or textures appear missing, run:

git lfs pull


to download the missing assets again.

Collaboration

This repository acts as the main collaborative project for our team:

@iremkalayci

@taha907

@jafarliturkay

Future Development

Advanced AI perception (hearing and vision systems)

Main menu and in-game UI (settings, sound, level select)

Level progression system with multiple stages

Environment interaction and damage effects

Polished sound design and post-processing improvements

Project Summary

This project combines technical AI development with creative level design to build a playable third-person shooter experience.
All assets, movement systems, and AI logic were created and tested within a fully integrated Unity environment.
The current version marks the first complete phase of development, with future updates planned to expand gameplay mechanics and visual fidelity.
