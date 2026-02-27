# Performance & Interest Management Contract v1.0 (FINAL+)

Goal:
- Server supports large fights with up to ~100 players in one combat area.
- Global server capacity may be 1–2000 players, distributed across location instances.
- Control CPU + bandwidth via AOI + replication tiers + tick budget + event-driven gameplay.

---

## World Partitioning (Later Routing)
- World split into Location Instances.
- Instance target: up to 200 concurrent players (configurable).

---

## Interest Management (AOI)
Server replicates entities ONLY if relevant.

### Primary Relevancy (MVP)
- Distance-based AOI with configurable radius per category:
  - Players/NPC: larger
  - Projectiles: medium
  - Interactables: small
  - FX: minimal or event-only

### Spatial Partition (Required)
- Grid/cell partition:
  - clients subscribe to nearby cells
  - entities register into cells
  - AOI queries use neighborhood cells, not full scans

### Optional (Later)
- LOS/occlusion culling optional (not MVP).

---

## Replication Tiers (Network LOD)
- Near (combat): higher update rate + key state
- Mid: reduced update rate (position only + minimal state)
- Far: not replicated

Configurable thresholds and rates.

---

## Update Rate Controls (Tick Budget)
- Movement sim: every tick
- AOI subscription rebuild: every 0.2–0.5s
- DOT ticks: every 0.25–0.5s
- Resource regen: every 0.2–1.0s
- Heavy checks must be batched and rate-limited

---

## Event-Driven Gameplay (Preferred)
Prefer events over continuous sync:
- StatusEffect Applied/Removed events
- Cast Started/Interrupted/Completed events
- Parry success events
- Damage events when needed

Do not replicate Animator internals.

---

## Minimal Network State
Replicate only essential:
- transform (tiered)
- velocity (tiered)
- locomotion state (compact)
- action state (compact)
- weapon slot state (compact enum/bitfield)
- resources: reduced rate or events

---

## Projectiles & VFX
- Server decides outcomes (hits/damage/status).
- Clients render visuals locally.
- Network: spawn/impact events only when needed.
- Use pooling.

---

## Collision Scaling Note
- Player collision enabled (solid bodies).
- Avoid expensive per-frame overlap resolution across many players.
- Spawn/teleport guarantee non-overlapping placement.
- Prefer soft separation if needed.

---

## Combat Scaling Notes
- Avoid continuous per-frame scans against many targets.
- Melee: overlap/raycast in limited windows
- AoE: single overlap at application
- Rate limit expensive physics queries
