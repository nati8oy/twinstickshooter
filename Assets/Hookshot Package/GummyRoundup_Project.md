# Gummy Roundup - Unity Project Documentation

## Project Overview

**Genre:** Puzzle/Collection Game  
**Unity Version:** [Your version]  
**Target Platform:** [PC/Mobile/Console]  
**Status:** Prototype with core hookshot mechanics implemented

### Core Concept
A collection game where the player uses a hookshot to round up creatures called "Gummies" and herd them into a pen. Gummies have a weight class (Light, Medium, Heavy), and the hookshot must have sufficient strength to pull and carry them.

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

**Controls (Standard Mode):**
| Action | Keyboard/Mouse | Gamepad |
|---|---|---|
| Fire hookshot / Throw Gummy | Right Mouse Button | R1 (Right Shoulder) |
| Activate pull when attached | Left Shift | South Button (X/A) |
| Cancel hookshot | Space | L1 (Left Shoulder) |
| Movement | WASD | Left Stick |
| Aim | Mouse | Right Stick |

**Controls (Auto-Target Mode):**
| Action | Keyboard/Mouse | Gamepad |
|---|---|---|
| Fire / Pull / Throw (contextual) | Right Mouse / Left Shift | South Button (X/A) |
| Cancel hookshot | Space | East Button (Circle/B) |
| Cycle target | Tab | West Button (Square/X) |
| Movement | WASD | Left Stick |
| Aim | Automatic (faces closest gummy or movement direction) | Automatic |

Auto-target is toggled via `DebugManager.autoTargetEnabled` in the inspector. When carrying a gummy, the player faces movement direction instead of tracking a target.

### Gummy System
- **Weight Classes:** 3 classes via `GummyLevel.Weight` enum — Light (1), Medium (2), Heavy (3)
- **Gold Economy:** Light = 1 gold, Medium = 2 gold, Heavy = 3 gold
- **Visual Distinction:** Size scales with weight (1x, 1.5x, 2x)
- **Strength-Gated Interaction:** Hookshot always attaches to Gummies, but:
  - If Gummy weight > hookshot strength: hook attaches but player cannot pull, carry, or drag — must cancel
  - If hookshot strength >= Gummy weight: full pull, carry, and throw functionality
- **Carry Speed Penalties (explicit per-tier values, tunable in inspector):**
  - Light: `lightCarrySpeed = 1.0` (no slowdown)
  - Medium: `mediumCarrySpeed = 0.7` (30% slower)
  - Heavy: `heavyCarrySpeed = 0.45` (55% slower)
- **Movement Freeze:** When hooked by a strong enough hookshot, the Gummy's `GummyBehaviour` (or `SimpleEnemyMovement`) is disabled so it stops running. Re-enabled on cancel.
- **Behaviour System (`GummyBehaviour.cs`):** Each Gummy has two configurable behaviour layers:
  - **Idle Movement** (when player is not nearby):
    - `WanderShort` — roams within a small area near spawn (3 units)
    - `WanderMedium` — roams a moderate area (7 units)
    - `WanderFar` — roams a large area (15 units)
  - **Player Reaction** (triggered when player enters detection radius):
    - `Follow` — moves toward the player, stops at a set distance
    - `ScatterFar` — flees far away from the player (20 units)
    - `ScatterNearby` — scoots a short distance away (5 units)
    - `Ignore` — keeps wandering, doesn't react to player
  - When the player leaves the detection radius, the Gummy returns to idle with a fresh wander destination
  - All distances, speeds, pause duration, and detection radius are tunable in the inspector
- **Stun System:** Gummies can be stunned temporarily (disables NavMesh)

### Scoring
- Points awarded for each Gummy collected
- Tracked via HUD system

### Economy & Progression
- **Gold System:** Collecting Gummies awards gold based on weight class
- **Hookshot Strength Upgrades:**
  - Light (Starting): Can pull Light Gummies only
  - Medium: Can pull Light & Medium Gummies
  - Heavy: Can pull all Gummies
- **Economy managed by GameEconomy singleton**

---

## Technical Implementation

### Key Scripts

**CM_Hookshot.cs** - Main hookshot logic
- Handles all hookshot states and transitions
- Raycasting for targeting (max range configurable) via `FindBestTarget()` (direct raycast + fuzzy fallback)
- **Targeting decal integration** — drives `TargetIndicator` to show a ground decal beneath the current target in Normal state
- Player and Gummy movement during hookshot
- Carry system for smaller Gummies
- **Weight/Strength Gating via `IsGummyTooStrong()`:**
  - Hookshot always attaches to Layer 11 objects
  - Checks `GummyLevel.WeightValue > hookshotStrength`
  - If too strong: blocks pull state transition, blocks auto-pull, skips SpringJoint connection and NavMeshAgent disable (prevents dragging)
  - If strong enough: disables `GummyBehaviour` (or `SimpleEnemyMovement` fallback) on attach so Gummy stops fleeing, re-enables on cancel
- **Carry speed penalties** — explicit per-tier multipliers (`lightCarrySpeed`, `mediumCarrySpeed`, `heavyCarrySpeed`) applied via `TwinStickMovement`
- **SpringJoint bounciness** set from `HookshotData.bounciness`
- Uses `HookshotData` ScriptableObject for stats
- **Auto-target mode inputs:** South button contextual (fire/pull/throw based on state), East button cancel, West button cycle target — only active when `DebugManager.autoTargetEnabled` is true
- **`IsCancelPressed()`** helper — checks East (auto mode) or L1/Space (standard mode) for all cancel points
- **Syncs auto-target detection radius** to `hookshotData.maxRange` on enable

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
- Proper gravity with `isGrounded` check (resets vertical velocity when grounded to prevent accumulation)
- Combines horizontal movement, FrictionController velocity, and gravity into a single `controller.Move` call (prevents dual-Move conflicts)
- Mouse aiming projects to ground plane
- **Reads `CM_Hookshot.dragSpeedMultiplier`** to apply carry speed penalties per weight tier
- **Auto-target rotation override:** When `DebugManager.autoTargetEnabled` is true:
  - **Hooked state priority:** When attached/pulling/flying toward a gummy, player faces the hooked target (not auto-target's closest)
  - Tracks closest gummy via `AutoTarget` component (smooth rotation using `gamepadRotateSmoothing`)
  - Faces movement direction (left stick) when no target is in range or when carrying a gummy
  - Completely bypasses standard right-stick/mouse aiming

**GummyLevel.cs** - Component for Gummy weight gating
- `weight` - `Weight` enum (Light, Medium, Heavy); if heavier than hookshot strength, pull/carry/drag is blocked
- `WeightValue` property returns the int value (1, 2, or 3) for comparisons and speed calculations
- Attach to any Gummy prefab to enable weight gating
- Gummies without this component can always be pulled normally

**GummyBehaviour.cs** - Gummy AI (replaces SimpleEnemyMovement)
- NavMesh based pathfinding with configurable idle and player-reaction behaviours
- **Two enum dropdowns** in inspector, independently configurable per Gummy:
  - `IdleMovement`: WanderShort, WanderMedium, WanderFar
  - `PlayerReaction`: Follow, ScatterFar, ScatterNearby, Ignore
- Stores spawn position as `homePosition` — idle wander radiates from this point
- Player detection via configurable `playerDetectionRadius` (default 8f)
- Idle wander includes configurable pause at each destination (`idlePauseDuration`)
- Scatter is one-shot (runs once, then waits) — prevents jittery re-scattering
- `OnEnable()` resets state and generates fresh destination (hookshot re-enable compatible)
- `OnDisable()` calls `ResetPath()` (hookshot disable compatible)
- `SetHomePosition(Vector3)` public method for object pooling support
- Configurable speeds per mode: `wanderSpeed`, `followSpeed`, `scatterSpeed`

**SimpleEnemyMovement.cs** - Legacy Gummy AI (kept for backward compatibility)
- Simple NavMesh player-chasing behaviour
- Still supported as fallback by CM_Hookshot if GummyBehaviour is not present

**HookshotData.cs** - ScriptableObject for hookshot stats
- `strength` - `GummyLevel.Weight` enum (Light/Medium/Heavy); determines which Gummies can be pulled/carried
- `maxRange` - Maximum hookshot distance
- `speedMin` / `speedMax` - Player movement speed during grapple
- `bounciness` - SpringJoint spring value, controls how bouncy the hookshot line is when attached
- `throwForce` - Force applied when throwing a carried Gummy (default 2000)
- **Multiple hookshot types can be created** as separate data assets
- Allows easy balancing and progression

**GenericCollisions.cs** - Pen/Goal detection and hazard handling
- Handles collision with goal pen (triggers scoring)
- Handles collision with hazards (deactivates object, fires `onHitHazard` event)
- **Hazard immunity while grappling** — if `CM_Hookshot.isGrappling` is true, hazard collisions are skipped (player-only; Gummies are unaffected)
- **Calls Gummy.OnCollected() to award gold**
- Uses Unity Events for flexible integration
- Attached to Gummy prefabs and the player

**EnemyStun.cs** - Temporary disable Gummy movement
- Disables NavMeshAgent for duration
- Unity Events for feedback

**EnemyHealth.cs** - Gummy health tracking
- Health bar integration (MoreMountains)
- Damage system
- Unity Events on hit

**LaserSight.cs** - Legacy visual targeting aid (disabled — replaced by TargetIndicator)
- Line renderer from player to target point

**TargetIndicator.cs** - Ground-projected targeting decal
- Shows a decal on the ground beneath the hookshot's current target
- Only visible when in Normal state and a valid target is in range
- Works with both direct raycast and fuzzy targeting
- Auto-target mode: decal follows AutoTarget's closest target
- Configurable decal size, ground offset, ground layer, and material

**HUD.cs** - UI display
- Score counter
- **Gold display**
- **Hookshot strength display**
- **Upgrade cost display**
- **Optional upgrade button**
- Subscribes to GameEconomy events for real-time updates

**AutoTarget.cs** - Auto-targeting system for simplified controls
- `Physics.OverlapSphere` with configurable `detectionRadius` and `targetLayerMask` (set to Layer 11 for Gummies)
- Maintains a distance-sorted list of targets in radius, updated every 0.2s via coroutine
- Defaults to closest target; `CycleTarget()` advances to next target in list
- `HasTarget` property — true when a valid active target is in range
- `attackDirection` — normalized direction to current target
- `detectionRadius` synced to `hookshotData.maxRange` by CM_Hookshot on enable
- `FindClosestGrapplePoint()` for grapple point targeting (separate from auto-target)
- **Setup:** Set `targetLayerMask` to Layer 11 ("Pullable") in the inspector

**DebugManager.cs** - Debug toggles (Singleton)
- `godMode` — disables player damage
- `infiniteAmmo` — unlimited ammo
- `autoTargetEnabled` — toggles auto-target control scheme on/off

**FrictionController.cs** - Acceleration and deceleration
- Calculates acceleration-based velocity and exposes it via `FrictionVelocity` property
- Velocity accumulates at `accelRate` while input is held, clamped to `targetSpeed`
- Damping reduces velocity when input is released
- Does **not** call `characterController.Move()` directly — `TwinStickMovement` reads `FrictionVelocity` and combines it into a single Move call to avoid dual-Move conflicts with gravity/grappling

### Input System
- Uses Unity's new Input System
- `PlayerControls.inputactions` asset defines bindings for movement and aim
- `CM_Hookshot` creates its own `InputAction` objects with dual bindings (keyboard/mouse + gamepad)
- All gamepad bindings use generic `<Gamepad>` (works with DualShock, Xbox, Switch Pro, etc.)
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
│   │   ├── GummyBehaviour.cs (configurable idle/reaction AI)
│   │   ├── SimpleEnemyMovement.cs (legacy Gummy AI)
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
- [x] Basic Gummy AI (NavMesh pathfinding) — now replaced by GummyBehaviour system
- [x] Gummy stun system
- [x] Score tracking
- [x] Visual targeting (ground decal via TargetIndicator — replaced laser sight line renderer)
- [x] ScriptableObject based hookshot stats (HookshotData)
- [x] Multiple hookshot types via data objects
- [x] Pen/Goal trigger system (GenericCollisions)
- [x] Input system with full gamepad support (generic `<Gamepad>` bindings for all controllers)
- [x] **3 Gummy weight classes with visual size differences**
- [x] **Gold economy (1/2/3 gold per weight class)**
- [x] **Hookshot strength system (Light/Medium/Heavy enum)**
- [x] **Strength-gated Gummy pulling (attach always, pull/carry blocked if gummy too heavy)**
- [x] **Gummy movement freeze on hookshot attach (GummyBehaviour/SimpleEnemyMovement disabled)**
- [x] **Configurable Gummy behaviour system (GummyBehaviour.cs) with idle movement and player reaction enums**
- [x] **Hookshot bounciness via HookshotData (drives SpringJoint spring)**
- [x] **Carry speed penalties per weight tier (Light=1.0, Medium=0.7, Heavy=0.45, tunable in inspector)**
- [x] **Upgrade system (spend gold to improve hookshot)**
- [x] **Hazard immunity while grappling (player ignores hazard collisions during grapple)**
- [x] **Proper gravity system (ground check prevents velocity accumulation)**
- [x] **Consolidated movement** — FrictionController velocity merged into TwinStickMovement's single `controller.Move()` call
- [x] **Auto-target control scheme** — simplified controls for less experienced players:
  - Toggleable via `DebugManager.autoTargetEnabled`
  - Auto-rotates player to face closest gummy in detection radius (Layer 11)
  - Faces movement direction when no target or when carrying a gummy
  - Contextual South button (fire/pull/throw), East cancel, West cycle target
  - Detection radius synced to hookshot max range

---

## Planned Features & Improvements

### High Priority
1. ~~**FIX INPUT SYSTEM**~~ (done — v0.5)

2. **Fuzzy Hookshot Targeting**
   - Replace single raycast with a cone/arc-based detection so the hookshot is more forgiving
   - If no direct raycast hit, check within a configurable arc angle for valid targets (Grappleable/Pullable/Collectible)
   - Prefer the closest valid target within the arc
   - Arc angle should be tunable in the inspector (e.g., `hookshotArcAngle`)
   - Applies to CM_Hookshot.HandleHookshotStart() targeting logic

3. ~~**Auto-Target Control Scheme**~~ (done — v0.6)

4. ~~**Improved Gummy Behavior**~~ (done — GummyBehaviour.cs)
   - ~~Remove chase behavior~~ (replaced with configurable idle/reaction system)
   - ~~Idle/wander AI when not hooked~~ (WanderShort/Medium/Far)
   - Leading behavior for Gummies that can't be carried
   - Better pathfinding around obstacles

3. **Level Design**
   - Environment with grapple points
   - Strategic pen placement
   - Gummy spawn locations
   - Obstacles and hazards

4. **Throw Landing Marker**
   - When throwing a carried Gummy, display a ground marker (decal/projector) showing the predicted landing spot
   - Gives players visual feedback on where the Gummy will land before and during the throw
   - Helps with aiming Gummies into the pen

5. **Visual/Audio Polish**
   - Feedback when trying to pull Gummy above hookshot strength
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
   - Explain weight/strength system
   - Show upgrade system

4. **UI Improvements**
   - Better upgrade shop interface
   - Visual hookshot strength indicator
   - "Too heavy!" message when insufficient strength
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
1. ~~**INPUT SYSTEM - Controller buttons not triggering properly**~~ (fixed — v0.5)
2. Some commented-out code in CM_Hookshot.cs (momentum system, auto-targeting)
3. Enemy naming - referred to as "enemies" but should be "Gummies" throughout
4. Hookshot_spring.cs appears unused
5. PlayerAttackManager.cs appears incomplete/unused
6. Grenade.cs exists but doesn't fit current game concept
7. Pen collision detection could be more robust (currently uses basic OnCollisionEnter)

### Refactoring Needed
1. Clean up unused scripts and commented code
2. Rename "enemy" references to "Gummy" throughout codebase
3. Consolidate hookshot implementations (CM_Hookshot vs Hookshot_spring)
4. ~~Implement weight resistance in pull logic~~ (done)
5. State machine could be more robust (consider Finite State Machine pattern)

---

## Design Considerations

### Level Design Pillars
1. **Have a clear goal** — every level should communicate what the player needs to do without ambiguity
2. **Don't waste space** — every area and object in a level should serve a purpose; no filler
3. **Keep it fresh** — incrementally introduce mechanics so the player is always learning something new

### Gummy Weight Classes
- **Light:** Hookshot Strength Light or above — Can pull and carry (no speed penalty)
- **Medium:** Hookshot Strength Medium or above — Can pull and carry (30% slower)
- **Heavy:** Hookshot Strength Heavy — Can pull and carry (55% slower)

### Hookshot Strength Tiers
- Light: Can pull Light Gummies only
- Medium: Can pull Light & Medium Gummies
- Heavy: Can pull all Gummies

### Gameplay Flow
1. Early levels: Only Light Gummies, teach basic mechanics
2. Mid levels: Introduce Medium Gummies, require strategic pulling
3. Late levels: Mix of all types, require planning and efficient routing

---

## Next Development Steps

### Immediate Tasks
1. ~~**FIX INPUT SYSTEM**~~ (done — v0.5)

2. **Test Economy System** (see WeightEconomySetupGuide.md)
   - Create 3 Gummy prefabs with different GummyData
   - Test gold collection and hookshot upgrades
   - Verify strength-gating works correctly
   - Balance speed penalties

3. ~~**Improve Gummy AI**~~ (done — GummyBehaviour.cs)
   - ~~Remove player-chasing behavior~~ (replaced with enum-based reactions)
   - ~~Add idle/wander states~~ (WanderShort/Medium/Far with pause)
   - Better NavMesh avoidance

4. **Refactor Terminology**
   - Change "enemy" references to "Gummy" throughout codebase
   - Update tags and layer names to match

### This Sprint
- [ ] Build first complete level with pen and Gummies
- [ ] Test full gameplay loop (find → hook → deposit → score)
- [x] ~~Implement basic weight resistance~~ (done — carry speed penalties per weight tier)
- [ ] Add visual feedback for hookshot strength vs Gummy weight

---

## Questions for Discussion

1. Should there be a stamina/cooldown system for the hookshot?
2. ~~What happens if player tries to hook a Gummy above their strength?~~ **Resolved:** Hook attaches but pull/carry/drag is blocked. Player must cancel.
3. ~~Should heavier Gummies slow player movement?~~ **Resolved:** Yes, when carrying. Explicit per-tier speed multipliers (Light=1.0, Medium=0.7, Heavy=0.45).
4. Are there environmental hazards that could free/scatter Gummies?
5. Multiple pens or single pen per level?

---

## Development Notes

### Code Conventions
- C# standard naming (PascalCase for public, camelCase for private)
- SerializeField for inspector-visible private fields
- Use Unity Events for hookshot feedback/game feel

### Testing Priorities
- **Auto-target:** toggle on/off, verify rotation switches between target tracking and movement direction
- **Auto-target:** walk near gummies, confirm player auto-faces closest one
- **Auto-target:** carry a gummy, confirm player faces movement direction instead of target
- **Auto-target:** cycle target with West/Tab when multiple gummies in range
- **Auto-target:** South button contextual behavior (fire in Normal, pull in Attached, throw in Carry)
- **Auto-target:** East button cancels hookshot in all hookshot states
- **Auto-target:** detection radius matches hookshot max range
- **Auto-target:** when hooked onto a gummy, player should face the hooked target (not cycle to closest)
- **Auto-target:** standard controls (R1/L1) still work when auto-target is off
- **Throw force:** verify different HookshotData assets can have different throw forces
- Hookshot range and feel
- Weight-based pull speed differences
- Gummy leading behavior at distance
- Pen collision detection reliability
- GummyBehaviour: test each IdleMovement + PlayerReaction combination
- GummyBehaviour: verify hookshot attach/pull/carry/throw/cancel all resume behaviour correctly
- GummyBehaviour: confirm no SetDestination errors when agent is off NavMesh

---

## Changelog

### [Current] - Prototype v0.9
- **Removed jump** — jump functionality removed from `TwinStickMovement`; game is now ground-based with grapple traversal
- **Consolidated movement calls** — `FrictionController` no longer calls `characterController.Move()` directly; exposes velocity via `FrictionVelocity` property, which `TwinStickMovement` reads and combines with player input and gravity into a single `controller.Move()` call (fixes jump/gravity conflicts caused by dual Move calls)

### Prototype v0.8
- **Targeting decal** — replaced LaserSight line renderer with `TargetIndicator.cs`, a ground-projected decal that appears beneath the current hookshot target
  - Only visible in Normal state when a valid target is in range
  - Works with direct raycast and fuzzy targeting (shows what the hookshot will actually hit)
  - Auto-target mode: decal follows AutoTarget's closest target
  - Configurable size, ground offset, ground layer, and material via inspector
- **`FindBestTarget()` method** — consolidated targeting logic (direct raycast + fuzzy fallback) into a reusable method used by both the targeting preview and the fire action

### Prototype v0.7
- **Throw force now data-driven** — `HookshotData.throwForce` field added so throw force can be tuned per hookshot type via ScriptableObject
- **Hooked-target rotation priority** — when auto-target is active and the player is in an attached/pull/flying state, the player faces the hooked gummy instead of switching to the auto-target's closest target
- **Fixed player deactivating on goal collision** — `GenericCollisions` now skips `SetActive(false)` for the player when hitting the goal; only Gummies are deactivated on deposit
- **Fixed MissingReferenceException in HandleHookshotStart** — added null check for `shotPoint` to prevent error when input fires after player is destroyed

### Prototype v0.6
- **Auto-target control scheme** — simplified controls for less experienced players:
  - Toggle via `DebugManager.autoTargetEnabled` in inspector
  - `AutoTarget.cs` rewritten: uses `Physics.OverlapSphere` with `targetLayerMask` (Layer 11) and configurable `detectionRadius`
  - Targets sorted by distance, defaults to closest; `CycleTarget()` cycles to next
  - `TwinStickMovement.HandleRotation()` overridden when auto-target is on:
    - Smooth-rotates toward tracked gummy when target is in range
    - Faces movement direction (left stick) when no target or when carrying a gummy
  - **Auto-target gamepad controls:** South = contextual fire/pull/throw (state-dependent), East = cancel, West = cycle target
  - Standard mode controls (R1/L1/South) unchanged and still work
  - Keyboard controls (right mouse/space/shift/tab) work in both modes
  - `IsCancelPressed()` helper checks East (auto) or L1/Space (standard) for all cancel points
  - Detection radius synced to `hookshotData.maxRange` by CM_Hookshot on enable
  - Fixed `FindClosestGrapplePoint()` bug: was using `closestTarget` instead of `closestGrapplePoint`

### Prototype v0.5
- **Input system fixed** — CM_Hookshot now uses dual bindings (keyboard/mouse + gamepad) for all actions:
  - Hookshot fire/throw: Right Mouse + R1
  - Pull: Left Shift + South Button (X/A)
  - Jump/Cancel: Space + L1
- **PlayerControls.inputactions** updated: all `<DualShockGamepad>` bindings replaced with generic `<Gamepad>` (supports Xbox, Switch Pro, etc.)
- Fixed invalid `<Mouse>/press` binding to `<Mouse>/leftButton` in PlayerControls.inputactions
- Added gamepad binding (L1) for Jump action in PlayerControls.inputactions
- **Gravity consolidated into TwinStickMovement** — uses `controller.isGrounded` check to reset vertical velocity, preventing infinite accumulation
- **Removed gravity from FrictionController** — gravity is now handled solely by TwinStickMovement
- **Fixed FrictionController damping bug** — stray semicolon on `if` statement caused damping to run every frame (even during input), fighting acceleration
- **Fixed FrictionController directional bias** — `speedDif` was calculated against world X axis only, causing resistance when moving right; replaced with direct input direction
- **Hazard immunity while grappling** — `GenericCollisions` skips hazard collisions when `CM_Hookshot.isGrappling` is true (player only, Gummies unaffected)

### Prototype v0.4
- Added `GummyBehaviour.cs` — configurable Gummy AI with two enum dropdowns:
  - `IdleMovement`: WanderShort (3u), WanderMedium (7u), WanderFar (15u)
  - `PlayerReaction`: Follow, ScatterFar (20u), ScatterNearby (5u), Ignore
- Gummies return to idle when player leaves detection radius, generating a fresh wander destination
- All distances, speeds, pause duration, and detection radius tunable in inspector
- OnEnable/OnDisable handle hookshot integration (reset state on re-enable, clear path on disable)
- Updated `CM_Hookshot.cs` to try `GummyBehaviour` first, fall back to `SimpleEnemyMovement` — backward compatible
- `SimpleEnemyMovement` kept as legacy fallback for non-gummy enemies

### Prototype v0.3
- Removed `level` system from `GummyLevel`, `HookshotData`, and `CM_Hookshot` — replaced by weight/strength enum
- `HookshotData.strength` is now a `GummyLevel.Weight` enum (Light/Medium/Heavy) — determines which Gummies can be pulled/carried
- `GummyLevel.weight` is now a `Weight` enum (Light/Medium/Heavy) instead of a free int
- `IsGummyTooStrong()` simplified to: `gummyWeight > hookshotStrength`
- Carry speed penalties are now explicit per-tier values (`lightCarrySpeed=1.0`, `mediumCarrySpeed=0.7`, `heavyCarrySpeed=0.45`), tunable in inspector on CM_Hookshot
- `TwinStickMovement` reads `CM_Hookshot.dragSpeedMultiplier` to apply carry speed penalty
- Speed penalty only applies when carrying, not when attached/dragging
- Fixed `EnemyMovement.cs` — added `navMeshAgent.isOnNavMesh` guards to `ChasePlayer()`, `ChaseEndPoint()`, `MoveToDestination()`, and `HasReachedDestination()` to prevent errors when agent is off NavMesh

### Prototype v0.2
- Added `bounciness` (float) field to `HookshotData` ScriptableObject
- Added `GummyLevel` component for per-Gummy weight gating
- Hookshot now always attaches to Gummies, but blocks pull/carry/drag if Gummy is too heavy
- When hooked, Gummy's `SimpleEnemyMovement` is disabled (stops fleeing); re-enabled on cancel
- SpringJoint spring value is now driven by `HookshotData.bounciness`
- Fixed `EnemyNavMesh.Awake()` NullReferenceException caused by `GameManager.Instance.player` being null during object pooling — moved lookup to `OnEnable` with null guards

### Prototype v0.1
- Initial hookshot implementation
- Basic player movement (twin-stick)
- Gummy AI with NavMesh
- Pull and carry mechanics
- Score tracking foundation
