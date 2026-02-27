# Networking & Prediction Contract v1.0 (FINAL+)

Goal:
- Animations and movement feel must be instant on the client (no visible latency).
- Authority remains on the server (commands validated and applied server-side).
- Design must scale to large fights by combining prediction + interest management.

## High-Level Model
- Client predicts local presentation (animation + immediate movement feel).
- Server is authoritative for final gameplay state.
- Client reconciles to server snapshots when needed.

## Predicted on Client (Instant Presentation)
Allowed immediately based on local input:
- Locomotion animations: idle/walk/run
- Jump start animation
- Fall / in-air animation
- Landing animation
- Wall transitions (Idle/Crawl/Run), head-look
- Grapple swing transitions (start/swing/release)
- Weapon teleport visibility (hand/back/belt) for local presentation
- Interaction prompt (UI only)
- Dodge animation presentation (server confirms final)

Client may start these without waiting for server, then correct if needed.

## Server-Authoritative Outcomes
Server validates and finalizes:
- Actual movement state (grounded, wall attach, grapple attach, velocity)
- Action execution: attacks, dodge finalization, skill casts, interrupts, cooldowns
- Damage, statuses, buffs/debuffs, knockback/knockdown
- Resource consumption (chakra/stamina) as final truth
- Interaction execution (Use) and its effects

## Commands vs Prediction
- Client sends command intents:
  - Move, Jump, Sprint, Attack (Tap/Hold), Block, Parry, Dodge, Cast, Use
- Client does NOT decide combat outcomes (hit, cast success, status application).
- Client may present animations immediately, but must accept corrections.

## Reconciliation Rules
If server differs:
- Correct position/velocity smoothly
- Correct locomotion state
- Action results follow server:
  - cast interrupted → cancel locally and apply short cooldown UI
  - dodge denied due to stamina → rollback presentation

## Animation Driving Rules
- Animator parameters derive from predicted state
- Server confirmation overrides mismatches
- No gameplay logic executed by animation events alone

## Weapon Teleport Replication
- Weapon visibility updates instantly on client for presentation.
- Server authoritative action state confirms final.
- On correction, client snaps weapon to correct slot immediately.

## Interaction
- Target prompt is client-side.
- Pressing F sends Use command to server.
- Server executes effect; client updates based on server confirmation.

## Interest Management
See Docs/PERFORMANCE_AND_INTEREST.md.
- Server replicates only relevant entities (AOI).
- AOI is distance/cell based for MVP.
- LOS/occlusion is optional later.
