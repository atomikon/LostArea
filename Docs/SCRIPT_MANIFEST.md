# Script Manifest (Approved .cs Files)
Version: v1.0
Status: LOCKED

This document defines the ONLY approved C# script files
that may exist in the project.

Adding, removing, or renaming scripts requires
explicit update of this document and a commit.

---

## Core / Bootstrap

- PlayerContext.cs  
  Central runtime context holding references, states, and access to systems.

- ArenaPlayer.cs  
  Facade input reader. Converts input into commands/events only.

- GameBootstrap.cs  
  Initializes core systems and player setup.

---

## Input

- PlayerInputHandler.cs  
  Maps Input Actions to high-level player intents.

---

## Movement

- MovementStateMachine.cs  
  Controls locomotion states and transitions.

- GroundMovementMotor.cs  
  Handles ground movement logic.

- AirMovementMotor.cs  
  Handles airborne movement and air control.

- JumpMotor.cs  
  Handles ground jump, air jump (double jump), wall jump.

- WallMovementMotor.cs  
  Handles wall idle, crawl, run, wall jump.

- GrappleMotor.cs  
  Handles swing-only grapple mechanics.

---

## Combat

- CombatStateMachine.cs  
  Controls combat action states.

- AttackController.cs  
  Handles attack combos and attack execution.

- BlockController.cs  
  Handles blocking and stamina drain.

- ParryController.cs  
  Handles parry timing, validation, and results.

- DodgeController.cs  
  Handles dodge, stamina cost, and i-frames.

---

## Casting / Abilities

- CastController.cs  
  Controls casting lifecycle and interruptions.

- SealSequenceGenerator.cs  
  Generates valid seal combinations (3/4/5, no repeats).

- AbilityExecutor.cs  
  Executes skills after successful cast.

---

## Skills & Effects

- SkillDefinition.cs  
  Data definition for skills (type, cost, cast, cooldown).

- SkillTargeting.cs  
  Resolves targeting logic (projectile, cone, AoE).

- StatusEffectController.cs  
  Applies, updates, and removes status effects.

---

## Stats & Resources

- StatsComponent.cs  
  Holds character stats and derived values.

- ResourceComponent.cs  
  Handles health, chakra, stamina, regen, and costs.

---

## Interaction

- InteractionScanner.cs  
  Raycasts from crosshair to find interactable targets.

- InteractionController.cs  
  Executes Use (F) interaction logic.

- Interactable.cs  
  Base class/interface for all interactable objects.

---

## Inventory / Equipment

- InventoryComponent.cs  
  Holds items and equipment.

- EquipmentComponent.cs  
  Applies stat modifiers from equipment.

---

## Animation

- AnimationDriver.cs  
  Drives animator parameters based on state.

- UpperBodyLayerController.cs  
  Controls upper-body-only animations.

- HeadLookController.cs  
  Controls head rotation during wall movement.

---

## Networking

- NetworkPlayerProxy.cs  
  Handles network authority, prediction, reconciliation.

- NetworkInterestObject.cs  
  Registers entity in AOI / interest system.

---

## Utilities

- CooldownTimer.cs  
  Generic cooldown timer utility.

- TimerService.cs  
  Centralized timer handling.

---

## Rules

- No script may exist outside this manifest.
- No system logic may be embedded in prefabs.
- No gameplay logic may exist in animation events.
- Any change requires manifest update + commit.