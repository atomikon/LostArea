# Features & Structure Contract v1.0 (FINAL+)

This document locks all core gameplay rules.

---

## Core Pillars
- Skill-based shinobi movement
- Momentum and physics driven traversal
- Action combat with clean interruption rules
- Sandbox-ready architecture (NPC/quests later)
- Optimization-first: AOI/interest management and event-driven replication

---

## Locomotion States
- Ground
- Air
- Wall: Idle / Crawl / Run
- Grapple: Swing only

Landing ALWAYS overrides actions.

---

## Movement Rules
- Running allowed at 0 stamina
- Wall crawling allowed at 0 stamina
- Wall run, grapple swing, block require stamina
- Grapple (Q) is movement-only:
  - swing on arc (pendulum)
  - no pull
  - no targeting/lock-on
  - no hanging/freeze state
  - auto/conditional release preserving momentum
  - consumes 15% stamina per use

---

## Wall System
### Wall States
- WallIdle: holding position
- WallCrawl:
  - movement in all directions (up/down/sideways)
  - allowed at 0 stamina
- WallRun:
  - requires Shift
  - movement only up or sideways
  - down input switches to WallCrawl (down)
  - consumes stamina
- Space during WallRun → wall jump + upward impulse
- Wall top-out:
  - Crawl → climb animation
  - Run → automatic transition

### Camera & Body Rules (Wall)
- While wall crawling or wall running:
  - camera rotation affects head only
  - body orientation is locked to wall
  - camera must not rotate full body on walls

---

## Combat Rules
- Attacks deal Physical damage only
- Skills deal Magical damage only
- Weapons:
  - unique animations
  - shared combo logic
- Air / Wall / Grapple attacks resolve as AirAttack
- Landing immediately resets attack state and returns to locomotion (idle/walk/run)

---

## Casting System
- Casting uses upper body only
- Casting may occur while moving (if allowed by the skill)
- Casting can be interrupted by:
  - enemy attacks
  - player’s own attack
- Interrupted cast:
  - skill does NOT execute
  - goes on short cooldown (0.5s)
- Successful cast → full cooldown

---

## Skill Casting Types
### Seals-Based Skills
- Use hand seals
- Total seals: 3
- Seal sequence length: 3 / 4 / 5 (defined per skill)
- No identical seals consecutively
- Valid sequence chosen randomly
- Each seal has its own upper-body animation
- Skill executes at final cast point

### Direct Cast Skills
- No seals
- Cast time defined in seconds
- Dedicated casting animation
- Executes at cast completion or cast point

---

## Skills Categories
1. Offensive:
   - Projectile
   - Cone
   - Self AoE
   - AoE Projectile
2. Buffs:
   - Self
   - AoE

---

## Resources
- Stamina:
  - Dodge
  - Wall run
  - Grapple swing (15% per use)
  - Block
- Chakra (Mana):
  - Skills consume chakra in percentages (chakra is 100%)
- Health:
  - reduced by defenses
- Equipment and buffs can:
  - increase max values
  - increase regeneration
  - reduce costs

---

## Dodge & Skill Avoidance

### Dodge (Evade)
- Dodge is an active action
- Dodge consumes stamina
- Dodge grants short invulnerability frames
- Dodge is effective against directional threats

### Skill Avoidance Rules
- Projectile skills:
  - can be dodged
  - cannot be parried or blocked
- Cone skills:
  - cannot be dodged while inside the cone
  - player must exit the cone area
- AoE skills:
  - cannot be dodged while inside the area
  - player must exit the AoE radius

---

## Block & Parry
- Block:
  - reduces incoming damage by 30%
  - consumes stamina (% per damage)
- Perfect Parry:
  - no stamina cost
  - cancels enemy combo
  - applies micro-stun (0.1–0.2s)
  - has visual/audio cue window (bells / weapon-tip glow)

### Parry Eligibility (Variant B)
- Parry works ONLY against TAP attacks / TAP combo steps
- HOLD attacks / HOLD combos cannot be parried
- There is NO throw mechanic in the game

---

## Action Interruption
- Attack ↔ Cast are mutually interruptible
- Cast interrupted by attack → short cooldown (0.5s)
- Attack interrupted by starting a cast → combo canceled immediately

---

## Weapon Handling
- Weapons are NOT drawn/sheath animated
- Weapon teleport rules:
  - Attack → weapon teleports to hand
  - Cast → weapon teleports to back/belt (depending on weapon type)
- If no weapon equipped → no action

---

## Interaction (Use)
- Button F is always Use / Interact
- Interaction is target-based (crosshair raycast), not proximity-based
- When valid target is under crosshair:
  - show prompt ("Use" + optional action/object text)
- Pressing F triggers target-defined effect:
  - pick up item
  - drink sake / consumable
  - talk to NPC (future)
  - activate object

---

## Animation Rules
- Locomotion has absolute priority
- Upper body overlays only (casting/attacks/head-look on walls)
- No sticky animation states

- ## Jump Rules
- Single ground jump
- One air jump (double jump) allowed before landing
- Air jump resets only on landing
- Wall jump and grapple release allow air jump
