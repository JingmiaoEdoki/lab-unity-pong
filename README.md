# LiftIA Lab Unity Pong

A Unity arcade Pong game designed as a hands-on lab for AI-assisted development training. This project serves as a practical environment for developers to learn how to use AI tools like Claude Code to explore codebases, implement game features, and iterate on game feel.

> **Based on:** PaddleGameSO (Unity Game Systems Cookbook)
> **Architecture:** Event-driven with ScriptableObjects

## Prerequisites

- **Unity 6** — version exacte : `6000.4.0f1` ([Unity Hub > Install Editor > Archive](https://unity.com/releases/editor/archive))
- **Claude Code** — installé et fonctionnel ([documentation](https://docs.anthropic.com/en/docs/claude-code))
- **MCP for Unity** — connecte Claude Code à l'éditeur Unity
  ```bash
  pip install mcpforunityserver
  ```

## Getting Started

### 1. Fork & clone

```bash
gh repo fork liftia/lab-unity-pong --clone
cd lab-unity-pong
```

### 2. Ouvrir dans Unity

1. Unity Hub > **Add** > **Add project from disk** > sélectionner le dossier
2. Vérifier que la version `6000.4.0f1` est utilisée
3. Ouvrir le projet (première ouverture : quelques minutes d'import)

### 3. Vérifier que le jeu tourne

1. Ouvrir `Assets/Scenes/Bootloader_Scene.unity`
2. **Play**
3. Joueur 1 : **W** / **S** — Joueur 2 : **flèche haut** / **flèche bas**

### 4. Configurer Claude Code + MCP

Créer un fichier `.mcp.json` à la racine du projet :
```json
{
  "mcpServers": {
    "unityMCP": {
      "command": "uvx",
      "args": ["--from", "mcpforunityserver", "mcp-for-unity"]
    }
  }
}
```

Dans Unity : **Window** > **MCP for Unity** pour vérifier la connexion.

## Features

This Pong game includes:

- **Event-Driven Architecture:** ScriptableObject-based publish/subscribe system
- **2-Player Local:** Keyboard controls for both players
- **Scoring System:** Event-based scoring with configurable win conditions
- **Physics-Based Movement:** Rigidbody2D paddles and ball
- **Configurable Gameplay:** ScriptableObject data for all game parameters
- **Level Layouts:** JSON import/export for level configurations

## About This Lab

This project is part of the **LiftIA** training program for developers learning to leverage AI coding assistants effectively. The Pong game provides a focused codebase where you can practice:

- Exploring and understanding an unfamiliar architecture with AI
- Implementing AI opponents and tuning game feel
- Creating visual effects (shaders, particles, post-processing)
- Building extensible gameplay systems (power-ups)
- Integrating new game modes without breaking existing code
- Generating procedural audio

## Project Structure

```
lab-unity-pong/
├── Assets/
│   ├── PaddleBall/             # Game-specific code
│   │   ├── Scripts/            # Ball, Paddle, Bouncer, ScoreGoal
│   │   │   ├── Managers/       # GameManager, GameSetup, ScoreManager
│   │   │   ├── ScriptableObjects/  # GameDataSO, LevelLayoutSO
│   │   │   └── UI/             # ScoreView, WinScreen
│   │   ├── Input/              # InputSystem actions
│   │   ├── Prefabs/            # Ball, Paddle, Goal, Wall
│   │   └── Data/               # SO assets (events, config, players)
│   ├── Core/                   # Reusable framework
│   │   ├── EventChannels/      # Generic pub/sub event system
│   │   ├── UI/                 # UIManager, View base classes
│   │   ├── Utilities/          # NullRefChecker, SaveLoad
│   │   ├── Objectives/         # Objective tracking system
│   │   └── Audio/              # Audio delegation
│   └── Scenes/
│       ├── Bootloader_Scene.unity
│       └── SampleScene.unity
├── Packages/
└── ProjectSettings/
```

## License

This project is based on Unity's Game Systems Cookbook samples and is provided for educational purposes.
