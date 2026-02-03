# Gummy Roundup - Unity Project Documentation

## Project Overview

**Genre:** Puzzle/Collection Game  
**Unity Version:** [Your version]  
**Target Platform:** [PC/Mobile/Console]  
**Status:** Prototype with core hookshot mechanics implemented

### Core Concept
A collection game where the player uses a hookshot to round up creatures called "Gummies" and herd them into a pen. Gummies vary in size and weight, requiring different hookshot levels to move and carry them effectively.

---

## Game Loop

1. Player explores the level with twin-stick movement controls
2. Player targets Gummies using the hookshot
3. Player either:
   - Pulls small Gummies toward themselves and carries them
   - Leads larger Gummies by staying attached and guiding them
4. Player deposits Gummies into a pen
5. Points are awarded per Gummy collected

---

## Core Mechanics

### Hookshot System
The hookshot is the primary interaction tool with multiple functions:

**States:**
- `Normal` - Default player state, can initiate hookshot
- `HookshotLaunched` - Hook is traveling (currently not used in main flow)
- `HookshotFlyingPlayer` - Player is being pulled toward grapple point (traversal)
- `HookshotPull` - Gummy is being pulled toward player
- `HookshotAttached` - Hook attached to Gummy, can pull or lead
- `HookshotCarry` - Player is carrying a Gummy

**Functions:**
1. **Traversal** - Grapple to environment objects (Layer 10: "Grappleable")
   - Pulls player toward the grapple point
   - Used for movement and navigation
   
2. **Gummy Collection** - Attach to Gummies (Layer 11: "Pullable")
   - Pull smaller Gummies toward player
   - Carry Gummies to the pen
   - Lead larger Gummies by staying attached
   
3. **Item Collection** - Pull collectibles (Layer 12)
   - Automatically pulls items toward player

**Controls:**
- Right Mouse Button: Fire hookshot / Pull Gummy
- Left Shift: Activate pull when attached
- Space: Jump / Cancel hookshot
- WASD: Movement
- Mouse: Aim (can also use gamepad right stick)

### Gummy System
- **Weight Classes:** 3 classes - Light (1), Medium (2), Heavy (3)
- **Gold Economy:** Light = 1 gold, Medium = 2 gold, Heavy = 3 gold
- **Visual Distinction:** Size scales with weight (1x, 1.5x, 2x)
- **Hookshot Level Requirements:** 
  - Light Gummies: Require Level 1 hookshot
  - Medium Gummies: Require Level 2 hookshot
  - Heavy Gummies: Require Level 3 hookshot
- **Movement Penalties When Carrying:**
  - Light: No slowdown (0%)
  - Medium: 30% slower
  - Heavy: 50% slower
- **Pull Speed:** Heavier Gummies pull slower (40 → 30 → 20 units/sec)
- **Level-Gated Interaction:** Hookshot always attaches to Gummies, but:
  - If Gummy `level >= hookshot level`: hook attaches but player cannot pull, carry, or drag — must cancel
  - If Gummy `weight > hookshot level`: same restriction — hook attaches but no pull/carry/drag
  - If both checks pass: full pull, carry, and throw functionality
- **Movement Freeze:** When hooked by a stronger hookshot (level > Gummy level), the Gummy's SimpleEnemyMovement is disabled so it stops running. Re-enabled on cancel.
- **Behavior:** Gummies use NavMesh pathfinding to move around
- **Stun System:** Gummies can be stunned temporarily (disables NavMesh)

### Scoring
- Points awarded for each Gummy collected
- Tracked via HUD system

### Economy & Progression
- **Gold System:** Collecting Gummies awards gold based on weight class
- **Hookshot Upgrades:**
  - Level 1 (Starting): Can pull Light Gummies only
  - Level 2 (5 gold): Can pull Light & Medium Gummies
  - Level 3 (10 gold): Can pull all Gummies
- **Total gold needed:** 15 gold to reach max level
- **Economy managed by GameEconomy singleton**

---

## Technical Implementation

### Key Scripts

**CM_Hookshot.cs** - Main hookshot logic
- Handles all hookshot states and transitions
- Raycasting for targeting (max range configurable)
- Player and Gummy movement during hookshot
- Carry system for smaller Gummies
- **Level/Weight Gating via `IsGummyTooStrong()`:**
  - Hookshot always attaches to Layer 11 objects
  - Checks `GummyLevel.level >= hookshotLevel` or `GummyLevel.weight > hookshotLevel`
  - If too strong: blocks pull state transition, blocks auto-pull, skips SpringJoint connection and NavMeshAgent disable (prevents dragging)
  - If within level: disables `SimpleEnemyMovement` on attach so Gummy stops fleeing, re-enables on cancel
- **SpringJoint bounciness** set from `HookshotData.bounciness`
- Uses `HookshotData` ScriptableObject for stats

**Gummy.cs** - Individual Gummy instance
- References GummyData ScriptableObject
- Initializes visual properties (size, color)
- Exposes weight class, gold value, level requirements
- Handles collection and gold awarding
- Sets Rigidbody mass based on weight

**GummyData.cs** - ScriptableObject for Gummy types
- Defines weight class (1-3)
- Gold value (1-3)
- Required hookshot level (1-3)
- Carry speed penalty (0-1)
- Pull speed
- Visual properties (size, color)
- Allows creating multiple Gummy variants

**GameEconomy.cs** - Singleton economy manager
- Tracks player's current gold
- Manages hookshot level progression
- Handles upgrade purchases (cost checking)
- Unity Events for UI updates
- Persistent across scenes (optional)

**TwinStickMovement.cs** - Player movement and rotation
- Supports keyboard/mouse and gamepad input
- Auto-detects input device
- Character Controller based movement
- Mouse aiming projects to ground plane
- **Speed can be modified by hookshot carry penalties**

**GummyLevel.cs** - Component for Gummy level/weight gating
- `level` (int) - The Gummy's level; must be below the hookshot's level to be pulled/carried
- `weight` (int) - The Gummy's weight; if higher than the hookshot's level, pull/carry/drag is blocked
- Attach to any Gummy prefab to enable level/weight gating
- Gummies without this component can always be pulled normally

**SimpleEnemyMovement.cs** - Gummy AI
- NavMesh based pathfinding
- Currently chases player (to be changed for herding behavior)
- Integrates with stun system
- **Disabled on hookshot attach** when Gummy level is below hookshot level (prevents running away while hooked)
- **Re-enabled on hookshot cancel** so Gummy resumes AI

**HookshotData.cs** - ScriptableObject for hookshot stats
- `strength` - Pull force
- `level` - Hookshot level (int), determines which Gummies can be pulled/carried
- `maxRange` - Maximum hookshot distance
- `speedMin` / `speedMax` - Player movement speed during grapple
- `bounciness` - SpringJoint spring value, controls how bouncy the hookshot line is when attached
- **Multiple hookshot types can be created** as separate data assets
- Allows easy balancing and level progression

**GenericCollisions.cs** - Pen/Goal detection for Gummies
- Handles collision with goal pen (triggers scoring)
- Handles collision with hazards (deactivates Gummy)
- **Calls Gummy.OnCollected() to award gold**
- Uses Unity Events for flexible integration
- Should be attached to each Gummy prefab

**EnemyStun.cs** - Temporary disable Gummy movement
- Disables NavMeshAgent for duration
- Unity Events for feedback

**EnemyHealth.cs** - Gummy health tracking
- Health bar integration (MoreMountains)
- Damage system
- Unity Events on hit

**LaserSight.cs** - Visual targeting aid
- Line renderer from player to target point

**HUD.cs** - UI display
- Score counter
- **Gold display**
- **Hookshot level display**
- **Upgrade cost display**
- **Optional upgrade button**
- Subscribes to GameEconomy events for real-time updates

### Input System
- Uses Unity's new Input System
- `PlayerControls.inputactions` asset defines bindings
- Supports both keyboard/mouse and gamepad
- Auto-switching between control schemes

### Layers Used
- Layer 10: "Grappleable" (environment objects for traversal)
- Layer 11: "Pullable" (Gummies that can be collected)
- Layer 12: "Collectibles" (items that auto-pull)

### Project Structure
```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── CM_Hookshot.cs (main hookshot controller)
│   │   └── TwinStickMovement.cs (player movement)
│   ├── Gummy/
│   │   ├── Gummy.cs (individual Gummy instance)
│   │   ├── SimpleEnemyMovement.cs (Gummy AI)
│   │   ├── EnemyStun.cs
│   │   ├── EnemyHealth.cs
│   │   └── GenericCollisions.cs (pen/goal detection)
│   ├── Economy/
│   │   └── GameEconomy.cs (gold & upgrades singleton)
│   ├── UI/
│   │   ├── HUD.cs
│   │   └── LaserSight.cs
│   └── [Other utility scripts]
├── Prefabs/
│   ├── PlayerHookshot.prefab
│   ├── LightGummy.prefab
│   ├── MediumGummy.prefab
│   └── HeavyGummy.prefab
├── Input/
│   └── PlayerControls.inputactions (Unity Input System asset)
└── Data/
    ├── HookshotData/
    │   └── [HookshotData ScriptableObjects]
    └── GummyData/
        ├── LightGummyData.asset
        ├── MediumGummyData.asset
        └── HeavyGummyData.asset
```

### Third-Party Assets
- **MoreMountains Feedbacks** - Used for game feel and feedback systems
- **MoreMountains Tools** - Health bar system

---

## Current Features ✓

- [x] Twin-stick player movement (keyboard/mouse + gamepad)
- [x] Hookshot targeting and firing
- [x] Grapple to environment for traversal
- [x] Pull Gummies toward player
- [x] Carry smaller Gummies
- [x] Throw carried Gummies
- [x] Basic Gummy AI (NavMesh pathfinding)
- [x] Gummy stun system
- [x] Score tracking
- [x] Visual targeting (laser sight)
- [x] ScriptableObject based hookshot stats (HookshotData)
- [x] Multiple hookshot types via data objects
- [x] Pen/Goal trigger system (GenericCollisions)
- [x] Input system with gamepad support (needs refinement)
- [x] **3 Gummy weight classes with visual size differences**
- [x] **Gold economy (1/2/3 gold per weight class)**
- [x] **Hookshot progression system (3 levels)**
- [x] **Level-gated Gummy pulling (attach always, pull/carry blocked if too strong)**
- [x] **Weight-gated Gummy pulling (weight > hookshot level blocks pull/carry/drag)**
- [x] **Gummy movement freeze on hookshot attach (SimpleEnemyMovement disabled)**
- [x] **Hookshot bounciness via HookshotData (drives SpringJoint spring)**
- [x] **Movement speed penalties when carrying heavy Gummies**
- [x] **Upgrade system (spend gold to improve hookshot)**

---

## Planned Features & Improvements

### High Priority
1. **FIX INPUT SYSTEM** - Controller issues (see InputSystemFixGuide.md)
   
2. **Improved Gummy Behavior**
   - Remove chase behavior
   - Idle/wander AI when not hooked
   - Leading behavior for Gummies that can't be carried
   - Better pathfinding around obstacles

3. **Level Design**
   - Environment with grapple points
   - Strategic pen placement
   - Gummy spawn locations
   - Obstacles and hazards

4. **Visual/Audio Polish**
   - Feedback when trying to pull Gummy above level
   - Collection effects per weight class
   - Upgrade purchase celebration
   - Weight-appropriate sound effects

### Medium Priority
1. **Economy Balancing**
   - Playtest and adjust gold values
   - Tune upgrade costs
   - Balance speed penalties
   
2. **Additional Gummy Variants**
   - Special/rare Gummies with bonus gold
   - Color variations within weight classes
   - Animated idle states

3. **Tutorial/Onboarding**
   - Teach hookshot controls
   - Explain weight/level system
   - Show upgrade system

4. **UI Improvements**
   - Better upgrade shop interface
   - Visual hookshot level indicator
   - "Too heavy!" message when insufficient level
   - Gold collection popup (+1, +2, +3)

### Lower Priority
1. **Meta Progression**
   - Multiple levels with cumulative gold
   - Permanent hookshot upgrades between levels
   - Level select with star ratings
   
2. **Challenge Modes**
   - Timer challenges
   - Limited hookshot uses
   - "No damage" runs
   
3. **Extended Economy**
   - Shop with power-ups (speed boost, magnet, etc.)
   - Combo system for bonus gold
   - Gold multipliers for fast collection

4. **Gummy Animations & Polish**
   - Idle animations
   - Pull/struggle animations
   - Stunned visual effects
   - Celebration when collected

---

## Known Issues & Technical Debt

### Current Issues
1. **INPUT SYSTEM - Controller buttons not triggering properly**
   - CM_Hookshot creates InputActions manually, bypassing PlayerControls asset
   - DualShockGamepad-specific bindings won't work with Xbox/other controllers
   - Invalid `<Mouse>/press` binding (should be `<Mouse>/leftButton`)
   - Manual control scheme detection may conflict with Unity's system
   - See `InputSystemFixGuide.md` for detailed fix instructions

2. Some commented-out code in CM_Hookshot.cs (momentum system, auto-targeting)
3. Enemy naming - referred to as "enemies" but should be "Gummies" throughout
4. FrictionController.cs and Hookshot_spring.cs appear unused
5. PlayerAttackManager.cs appears incomplete/unused
6. Grenade.cs exists but doesn't fit current game concept
7. Pen collision detection could be more robust (currently uses basic OnCollisionEnter)

### Refactoring Needed
1. Clean up unused scripts and commented code
2. Rename "enemy" references to "Gummy" throughout codebase
3. Consolidate hookshot implementations (CM_Hookshot vs Hookshot_spring)
4. Implement weight resistance in pull logic
5. State machine could be more robust (consider Finite State Machine pattern)

---

## Design Considerations

### Gummy Weight Classes (Example)
- **Light:** Hookshot Level 1 - Can pull and carry
- **Medium:** Hookshot Level 2 - Can pull but cannot carry, must lead
- **Heavy:** Hookshot Level 3 - Cannot pull, can only attach and lead

### Hookshot Upgrade Path
- Level 1: Basic hookshot, traversal only
- Level 2: Can pull Light Gummies
- Level 3: Can pull Medium Gummies
- Level 4: Can attach to Heavy Gummies

### Gameplay Flow
1. Early levels: Only Light Gummies, teach basic mechanics
2. Mid levels: Introduce Medium Gummies, require strategic pulling
3. Late levels: Mix of all types, require planning and efficient routing

---

## Next Development Steps

### Immediate Tasks
1. **FIX INPUT SYSTEM** - Top priority (see InputSystemFixGuide.md)
   - Update PlayerControls.inputactions to use generic Gamepad bindings
   - Replace CM_Hookshot to use PlayerControls asset properly
   - Fix mouse button bindings
   - Test with multiple controller types

2. **Test Economy System** (see WeightEconomySetupGuide.md)
   - Create 3 Gummy prefabs with different GummyData
   - Test gold collection and hookshot upgrades
   - Verify level-gating works correctly
   - Balance speed penalties

3. **Improve Gummy AI**
   - Remove player-chasing behavior
   - Add idle/wander states
   - Better NavMesh avoidance

4. **Refactor Terminology**
   - Change "enemy" references to "Gummy" throughout codebase
   - Update tags and layer names to match

### This Sprint
- [ ] Build first complete level with pen and Gummies
- [ ] Test full gameplay loop (find → hook → deposit → score)
- [ ] Implement basic weight resistance
- [ ] Add visual feedback for hookshot level vs Gummy requirements

---

## Questions for Discussion

1. Should there be a stamina/cooldown system for the hookshot?
2. ~~What happens if player tries to hook a Gummy above their level?~~ **Resolved:** Hook attaches but pull/carry/drag is blocked. Player must cancel.
3. Should heavier Gummies slow player movement when leading them?
4. Are there environmental hazards that could free/scatter Gummies?
5. Multiple pens or single pen per level?

---

## Development Notes

### Code Conventions
- C# standard naming (PascalCase for public, camelCase for private)
- SerializeField for inspector-visible private fields
- Use Unity Events for hookshot feedback/game feel

### Testing Priorities
- Hookshot range and feel
- Weight-based pull speed differences
- Gummy leading behavior at distance
- Pen collision detection reliability

---

## Changelog

### [Current] - Prototype v0.2
- Added `level` (int) and `bounciness` (float) fields to `HookshotData` ScriptableObject
- Added `GummyLevel` component with `level` and `weight` fields for per-Gummy gating
- Hookshot now always attaches to Gummies, but blocks pull/carry/drag if Gummy level >= hookshot level or Gummy weight > hookshot level
- When a Gummy is hooked and its level is below the hookshot's, its `SimpleEnemyMovement` is disabled (stops fleeing); re-enabled on cancel
- SpringJoint spring value is now driven by `HookshotData.bounciness`
- Fixed `EnemyNavMesh.Awake()` NullReferenceException caused by `GameManager.Instance.player` being null during object pooling — moved lookup to `OnEnable` with null guards

### Prototype v0.1
- Initial hookshot implementation
- Basic player movement (twin-stick)
- Gummy AI with NavMesh
- Pull and carry mechanics
- Score tracking foundation
