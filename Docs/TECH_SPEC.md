# Technical Specification (FINAL+)

## Architectural Principles

### 1) Facade Pattern
- ArenaPlayer reads input and issues commands/events only
- No gameplay logic in input handlers
- No input reading outside Facade/Input layer

### 2) Modular Motors / Systems
- Movement, Combat, Casting, Status, Inventory, Interaction are separate modules
- Modules do not read input directly
- Modules operate on shared context/state and communicate via events

### 3) Player Context
PlayerContext holds:
- component references (rigidbody/animator/camera/network)
- runtime state (locomotion/action)
- access to systems (stats, status effects, abilities, inventory, interaction)

Modules must not search components at runtime.

### 4) Animation System
- Base locomotion layer for movement states
- Upper-body layer for attacks/casting (arms only)
- Head-look overlay for wall states (camera rotates head only)
- No physics in animation scripts
- Landing event forces locomotion resolution immediately

### 5) Action Model
- LocomotionState and ActionState are separate
- Actions can interrupt each other per FEATURES contract
- Status effects apply tags/modifiers; they do not implement game logic

### 6) Interaction Architecture
- Target-based only:
  - raycast from camera center
  - only targeted Interactable can be used
- Interactable defines:
  - prompt text
  - use effect
- UI only displays prompt; no gameplay logic

### 7) Dodge Architecture
- Dodge is an action that consumes stamina and provides i-frames.
- Dodge effectiveness depends on skill type:
  - projectiles can be dodged
  - cone/AoE require exiting area

### 8) Networking & Prediction
See Docs/NETWORKING_SPEC.md.
- Client predicts presentation (animations + feel).
- Server is authoritative for outcomes and final state.

### 9) Interest Management & Performance
See Docs/PERFORMANCE_AND_INTEREST.md.
- AOI distance/cell based for MVP
- replication tiers + event-driven gameplay updates
