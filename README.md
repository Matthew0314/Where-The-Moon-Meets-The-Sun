# Leave The World Behind

> A solo-developed tactical RPG built in Unity featuring modular gameplay systems, objective-driven combat, and extensible enemy AI.

---

## Overview

**Leave The World Behind** is a tactical role-playing game (TRPG) developed independently in **Unity** and inspired by games such as **Fire Emblem**, **Final Fantasy Tactics**, and **Triangle Strategy**.

Players command a party of unique characters across grid-based battlefields, completing objectives such as defeating enemy commanders, escorting allies, surviving enemy assaults, and escaping dangerous situations. Success depends on careful positioning, resource management, and long-term tactical planning rather than simply defeating every enemy.

This project was created to explore scalable game architecture while building a complete tactical combat system. Every major gameplay feature, from movement and combat to enemy AI, was designed with modularity and extensibility in mind, allowing new mechanics, abilities, maps, and enemy behaviors to be added with minimal changes to the existing codebase.

---

## Features

- Grid-based tactical combat
- Objective-driven missions
- A* pathfinding for intelligent movement
- Modular enemy AI behaviors
- Character classes, stats, and progression
- Dynamic combat calculations
- Combat forecast interface
- Battle cutscenes
- Interactive tutorial system

---

## Gameplay

Leave The World Behind emphasizes tactical decision-making over brute force. Every battle presents different objectives that encourage players to adapt their strategy to the situation.

### Command Points
A shared resource used to move units, attack enemies, and perform special actions each turn. Efficient Command Point management is critical to success.

### Limits
Powerful character-specific abilities that become available after filling a combat gauge during battle.

### Arts
High-cost offensive and defensive abilities that allow players to turn the tide of combat through careful resource management.

### Objective-Based Missions

Maps feature a variety of objectives including:

- Defeat Boss
- Rout Enemy Forces
- Escort Allies
- Escape Maps
- Survival Missions

These mechanics encourage players to think several turns ahead and adapt their strategy based on the mission rather than relying on a single playstyle.

---

## Enemy AI

Enemy behavior is powered by a modular AI framework designed to create varied and engaging encounters.

### AI Features

- A* pathfinding for intelligent battlefield navigation
- Modular AI behaviors that can be assigned to any unit
- Dynamic movement evaluation before selecting actions
- Context-aware target selection
- Easily extensible behavior system

### Implemented Behaviors

- Aggressive
- Passive
- Tracker
- Attack-in-Range

Rather than relying on hardcoded enemy logic, behaviors are implemented as interchangeable modules. This architecture makes encounters easier to design while keeping the AI system maintainable and scalable as new enemy types are introduced.

---

## Technical Highlights

The project focuses heavily on creating reusable gameplay systems instead of one-off implementations.

### Gameplay Systems

- Grid-based movement system
- Turn management
- Character and class framework
- Combat calculations
- Status effects
- Battle objectives
- Tutorial framework
- Cutscene system

### Architecture

Core gameplay systems are separated into independent modules including:

- Combat
- Movement
- AI
- Map Management
- Character Data
- User Interface

This modular architecture allows new mechanics, maps, enemy behaviors, and gameplay features to be introduced without requiring significant modifications to existing systems.

---

## Technology Stack

- **Engine:** Unity
- **Language:** C#
- **UI:** Unity UI
- **Version Control:** Git & GitHub

---

## Project Goals

This project was created to improve my understanding of:

- Object-Oriented Programming
- Gameplay Architecture
- Artificial Intelligence
- Tactical Combat Design
- Scalable Software Design
- User Interface Development
<!--
---

## Screenshots

### Tactical Combat

![Combat](images/combat.png)

### Combat Forecast

![Forecast](images/forecast.png)

### Enemy AI

![AI](images/ai.png)

### Battle Cutscene

![Cutscene](images/cutscene.png)

---
-->
## Future Work

Planned improvements include:

- Additional enemy AI behaviors
- New character classes
- Expanded ability system
- Terrain effects and environmental hazards
- Additional mission types
- Improved combat balancing
- Expanded story content
- More cinematic battle cutscenes

---

# Author

**Matthew Holzer**

- GitHub: https://github.com/Matthew0314
- LinkedIn: https://linkedin.com/in/matthewholzer314
- Portfolio: https://matthewholzer.com/
