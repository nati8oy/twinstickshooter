# Gummy Weight & Hookshot Economy System - Setup Guide

## Overview
This system implements:
- 3 Gummy weight classes (Light, Medium, Heavy)
- Gold economy (1, 2, 3 gold per weight class)
- Hookshot progression system (3 levels)
- Movement speed penalties when carrying heavier Gummies
- Level-gated pulling (can't pull Gummies above your hookshot level)

---

## Setup Steps

### Step 1: Create GameEconomy Manager

1. Create an empty GameObject in your scene
2. Name it "GameEconomy"
3. Add the `GameEconomy.cs` script
4. Configure in Inspector:
   - Current Gold: 0
   - Current Hookshot Level: 1
   - Max Hookshot Level: 3
   - Level 2 Cost: 5 gold
   - Level 3 Cost: 10 gold
5. Wire up Unity Events if you want visual/audio feedback:
   - onGoldChanged
   - onHookshotLevelChanged
   - onUpgradePurchased
   - onInsufficientFunds

### Step 2: Create GummyData ScriptableObjects

Create 3 GummyData assets (one for each weight class):

#### Light Gummy Data
1. Right-click in Project → Create → Gummy Data
2. Name it "LightGummyData"
3. Configure:
   - Gummy Name: "Light Gummy"
   - Weight Class: 1
   - Gold Value: 1
   - Required Hookshot Level: 1
   - Carry Speed Penalty: 0 (no slowdown)
   - Pull Speed: 40
   - Size Multiplier: 1.0
   - Gummy Color: Light color (e.g., pastel yellow)

#### Medium Gummy Data
1. Create → Gummy Data
2. Name it "MediumGummyData"
3. Configure:
   - Gummy Name: "Medium Gummy"
   - Weight Class: 2
   - Gold Value: 2
   - Required Hookshot Level: 2
   - Carry Speed Penalty: 0.3 (30% slower)
   - Pull Speed: 30
   - Size Multiplier: 1.5
   - Gummy Color: Medium color (e.g., orange)

#### Heavy Gummy Data
1. Create → Gummy Data
2. Name it "HeavyGummyData"
3. Configure:
   - Gummy Name: "Heavy Gummy"
   - Weight Class: 3
   - Gold Value: 3
   - Required Hookshot Level: 3
   - Carry Speed Penalty: 0.5 (50% slower)
   - Pull Speed: 20
   - Size Multiplier: 2.0
   - Gummy Color: Dark color (e.g., red/purple)

### Step 3: Update Gummy Prefabs

For each Gummy prefab (create 3 separate prefabs for each weight class):

1. **Add/Update Components:**
   - Add `Gummy.cs` component
   - Keep `Rigidbody` (required)
   - Keep collider
   - Keep `SimpleEnemyMovement.cs`
   - Replace `GenericCollisions.cs` with `GenericCollisions_WithEconomy.cs`

2. **Configure Gummy Component:**
   - Assign the appropriate GummyData asset:
     - Light Gummy Prefab → LightGummyData
     - Medium Gummy Prefab → MediumGummyData
     - Heavy Gummy Prefab → HeavyGummyData
   - Assign Visual Model (the child mesh that gets scaled/colored)
   - Assign Gummy Renderer (for color tinting)

3. **Verify Layer:**
   - Ensure Gummy is on Layer 11 ("Pullable")

4. **Setup GenericCollisions Events:**
   - Wire up `onHitGoal` event to play effects, particles, sounds, etc.
   - The gold awarding is automatic via the Gummy component

### Step 4: Update Player Hookshot

1. Open your PlayerHookshot prefab
2. Replace `CM_Hookshot.cs` with `CM_Hookshot_WithWeightSystem.cs`
3. Configure the new event:
   - `onInsufficientLevel` - plays when trying to pull Gummy above level
     - Could play error sound
     - Could show UI message "Hookshot level too low!"

4. The script will automatically:
   - Check hookshot level vs Gummy requirements
   - Apply speed penalties when carrying
   - Use Gummy-specific pull speeds

### Step 5: Update HUD

1. Open your HUD Canvas
2. Add TextMeshPro elements for:
   - Gold count
   - Hookshot level
   - Upgrade cost (optional)
   - Upgrade button (optional)

3. Replace `HUD.cs` with `HUD_WithEconomy.cs`

4. Assign UI references in Inspector:
   - Score Text (existing)
   - Gold Text (new)
   - Hookshot Level Text (new)
   - Upgrade Cost Text (new, optional)
   - Upgrade Button (new, optional)
   - Upgrade Button Text (new, optional)

### Step 6: Setup Goal Pen

Your pen is already set up, but verify:
1. Pen has a Collider with "Is Trigger" checked
2. Pen is tagged "goal"
3. When Gummy enters pen:
   - GenericCollisions detects trigger
   - Calls Gummy.OnCollected()
   - Gold is added via GameEconomy
   - Gummy is deactivated

---

## Testing Checklist

### Basic Economy
- [ ] Collecting Light Gummy awards 1 gold
- [ ] Collecting Medium Gummy awards 2 gold
- [ ] Collecting Heavy Gummy awards 3 gold
- [ ] Gold displays correctly in HUD
- [ ] Hookshot level displays correctly

### Hookshot Progression
- [ ] Starting at level 1, can pull Light Gummies
- [ ] Cannot pull Medium Gummies at level 1
- [ ] Can upgrade to level 2 for 5 gold
- [ ] At level 2, can pull Medium Gummies
- [ ] Cannot pull Heavy Gummies at level 2
- [ ] Can upgrade to level 3 for 10 gold
- [ ] At level 3, can pull all Gummies

### Level Gating Behavior
- [ ] Hooking Gummy above level: hookshot attaches but doesn't pull
- [ ] onInsufficientLevel event fires (play sound/show message)
- [ ] Pressing pull button does nothing if level too low
- [ ] Jumping cancels the hookshot properly

### Movement Speed Penalties
- [ ] Carrying Light Gummy: no speed change
- [ ] Carrying Medium Gummy: 30% slower
- [ ] Carrying Heavy Gummy: 50% slower
- [ ] Speed restores when throwing Gummy
- [ ] Speed restores when canceling hookshot

### Visual Feedback
- [ ] Light Gummies are smallest (1x scale)
- [ ] Medium Gummies are medium (1.5x scale)
- [ ] Heavy Gummies are largest (2x scale)
- [ ] Colors are distinct for each type
- [ ] Pull speed varies by weight (Heavy pulls slowest)

### Upgrade System
- [ ] Upgrade button disabled when can't afford
- [ ] Upgrade button enabled when can afford
- [ ] Clicking upgrade button spends gold and increases level
- [ ] At max level, upgrade button shows "MAX LEVEL"

---

## Quick Debug Tips

### Console Commands (via GameEconomy context menu)
Right-click GameEconomy component in Inspector:
- "Add 10 Gold" - instantly add gold for testing
- "Upgrade Hookshot" - instant upgrade
- "Reset Economy" - reset to level 1, 0 gold

### Testing Different Levels
To quickly test different hookshot levels:
1. Select GameEconomy in hierarchy
2. Change "Current Hookshot Level" in Inspector
3. Changes take effect immediately

### Testing Gold Collection
1. Create a simple level with all 3 Gummy types
2. Collect each type and verify gold amounts
3. Check HUD updates correctly

---

## Common Issues & Solutions

### "Gummy doesn't award gold"
- Verify Gummy component has GummyData assigned
- Check GameEconomy exists in scene
- Enable debug logs in GenericCollisions to see if collection triggers

### "Can't pull any Gummies"
- Check that GameEconomy.CurrentHookshotLevel >= Gummy.RequiredHookshotLevel
- Verify GummyData has correct requiredHookshotLevel set
- Check that CanPullGummy() is returning true in debugger

### "Speed penalty not working"
- Verify TwinStickMovement reference is set in CM_Hookshot
- Check that basePlayerSpeed is being stored correctly
- Add Debug.Log in ApplyCarrySpeedPenalty to see values

### "Visual size not changing"
- Assign Visual Model reference in Gummy component
- Or leave it null to scale the whole GameObject
- Check that GummyData.sizeMultiplier is different for each type

### "Upgrade button always disabled"
- Verify GameEconomy instance exists
- Check that HUD is subscribed to economy events (see Start method)
- Ensure CanAffordUpgrade() logic is correct

---

## Balancing Tips

### Gold Values
Current: Light=1, Medium=2, Heavy=3
- Adjust based on playtesting
- Could make Heavy worth more (e.g., 5 gold) to incentivize harder work

### Upgrade Costs
Current: Level 2 = 5 gold, Level 3 = 10 gold
- Level 2 requires collecting 5 Light Gummies (or 3 Medium, etc.)
- Level 3 requires total of 15 gold from start
- Adjust if progression feels too slow/fast

### Speed Penalties
Current: Light=0%, Medium=30%, Heavy=50%
- If Heavy feels too punishing, reduce to 40%
- Could add risk/reward: Heavy Gummies are slower but worth more

### Pull Speeds
Current: Light=40, Medium=30, Heavy=20
- Affects how fast Gummies come to player
- Slower = more skill required to not lose them to hazards
- Faster = easier but less strategic

---

## Extension Ideas

### Future Features
1. **Combo System:** Bonus gold for collecting multiple Gummies quickly
2. **Hookshot Durability:** Limited uses before needing repair/upgrade
3. **Gummy Variants:** Special Gummies with unique properties
4. **Shop System:** Buy power-ups with gold (not just hookshot upgrades)
5. **Time Bonus:** Extra gold for fast completion
6. **Multiplier:** Chain collections without touching ground = more gold

### Additional Gummy Types
- **Tiny Gummy:** 0.5 gold, super easy to carry
- **Boss Gummy:** 10 gold, requires special technique
- **Golden Gummy:** Rare spawn, worth 5x normal gold
- **Slippery Gummy:** Escapes if not collected quickly

---

## File Reference

**New Scripts:**
- `GummyData.cs` - ScriptableObject defining Gummy properties
- `Gummy.cs` - Component on each Gummy instance
- `GameEconomy.cs` - Singleton managing gold and upgrades
- `CM_Hookshot_WithWeightSystem.cs` - Updated hookshot with weight checks
- `GenericCollisions_WithEconomy.cs` - Updated collision handler
- `HUD_WithEconomy.cs` - Updated HUD with gold/level display

**Updated Files:**
- Replace old CM_Hookshot with new version
- Replace old GenericCollisions with new version
- Replace old HUD with new version (or add gold/level display manually)
