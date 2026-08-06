# Mini_Game_Design_Patterns

# Astro Rush
A 2D side-scrolling competitive racing game built in Unity 6.

## Game Description
You pick one of four astronaut characters and race against three AI opponents.
The player is always slower than the AI — the only way to win is by firing
swap shots that instantly exchange your position with whoever you hit.

---

## Requirements
- Unity 6 (6000.0.x)
- Universal Render Pipeline (URP)
- Input System package
- TextMeshPro package

---

## Setup Instructions

### 1. Clone or download the project
```
git clone https://github.com/YOUR_USERNAME/astro-rush.git
```
Or download the ZIP and extract it.

### 2. Open in Unity
1. Open Unity Hub
2. Click **Add project from disk**
3. Navigate to the extracted folder and select it
4. Click **Open** — Unity will import all assets automatically

### 3. Open the scene
1. In the Project window go to **Assets → Scenes**
2. Double click **LEVEL1** to open it

### 4. Press Play
Hit the Play button at the top of the Unity editor.
The main menu will appear immediately.

---

## How to Play
| Action | Key |
|---|---|
| Jump | W or Up Arrow |
| Fire swap shot | Spacebar |
| Pause / Resume | Escape |

1. Click **Play** on the main menu
2. Choose your character on the character select screen
3. Wait for the 3-2-1-GO countdown
4. Race to the finish line — use swap shots to overtake AI racers
5. First to the finish wins

---

## Project Structure
```
Assets/
  Scripts/
    Racer.cs             — Base class for all characters (OOP root)
    PlayerController.cs  — Human input (inherits Racer)
    AIController.cs      — AI logic (inherits Racer)
    GameManager.cs       — Singleton: UI panel management
    RaceManager.cs       — Singleton: race flow and countdown
    ShootingSystem.cs    — Factory: fires swap bullets, manages shot count
    SwapBullet.cs        — Projectile that swaps positions on hit
    FinishLine.cs        — Trigger that detects the winner
    PauseMenu.cs         — Singleton: pause/resume/restart
    CameraFollow.cs      — Follows the selected player character
    ShotPickup.cs        — Collectible that adds shots
  Animations/
    run_purple.anim      — Purple character run cycle
    run_pink.anim        — Pink character run cycle
    run_yellow.anim      — Yellow character run cycle
    run_green.anim       — Green character run cycle
  Scenes/
    LEVEL1              — Main game scene (menu + race in one scene)
```

---

## OOP Principles in Code
| Principle | Where |
|---|---|
| Encapsulation | Racer.cs lines 29-30 — isFrozen and freezeTimer are private |
| Abstraction | Racer.cs line 194 — TakeHit() is virtual, called without type checks |
| Inheritance | PlayerController and AIController both extend Racer |
| Polymorphism | SwapBullet calls racer.TakeHit() — correct override runs automatically |

## Design Patterns in Code
| Pattern | Where |
|---|---|
| Singleton | GameManager.cs line 6, RaceManager.cs, PauseMenu.cs |
| Factory Method | ShootingSystem.cs lines 45-63 — Fire() builds and initialises bullets |
| Singleton + State | PauseMenu.cs — exclusively controls Time.timeScale |

## Algorithms in Code
| Algorithm | Where |
|---|---|
| State-based AI decision logic | Racer.cs lines 177-183 — HandleAI() |
| Resource optimization | ShootingSystem.cs lines 14-70 — shot count management |

---

## Assets
- Character sprites: Kenney Assets (kenney.nl) — free to use
- Background and tilemap: Kenney Assets
- Font: TextMeshPro default

---

## Known Issues
- Characters need a Ground layer assigned on the Racer component
  for jumping to work correctly
- The bullet prefab must be assigned in the ShootingSystem
  component on each character
