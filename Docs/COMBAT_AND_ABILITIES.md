# Combat & Abilities (FINAL+)

## Damage Types
- Physical → attacks only
- Magical → skills only

## Attacks & Combos
- Shared combo logic for all weapons
- Weapon-specific animations
- Attack resolution by locomotion:
  - Ground → GroundAttack
  - Air/Wall/Grapple → AirAttack
- Landing returns to locomotion immediately

## Attack Input Types
- TAP steps: parryable (Variant B)
- HOLD steps: not parryable
No throw mechanic.

## Block
- 30% damage reduction
- stamina consumed proportional to incoming damage

## Perfect Parry (Variant B)
- Only vs TAP attacks
- No stamina cost
- Cancels enemy combo
- Applies micro-stun 0.1–0.2s
- Cue during parry window (bells / weapon-tip glow)

## Dodge
- Dodge consumes stamina
- Dodge grants i-frames
- Dodge avoids projectiles and directional threats
- Cone/AoE must be avoided by leaving the area

## Casting
- Upper-body only
- May allow movement per skill config
- Weapon teleports to back/belt during cast

### Casting Types
Seals-based:
- 3 seals available
- sequence length 3/4/5
- no identical consecutive seals
- random valid sequence
- executes at final cast point

Direct cast:
- cast time in seconds
- dedicated animation
- executes at completion or cast point

## Interruptions
- Casting interrupted by enemy attack or player's own attack:
  - does not execute
  - short cooldown 0.5s
- Starting a cast cancels current attack/combo (combo resets)
- Successful cast → full cooldown
