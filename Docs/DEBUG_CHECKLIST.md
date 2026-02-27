# Debug Checklist (FINAL+)

## Movement
- Locomotion states switch correctly
- Landing resets actions and returns to locomotion
- Grapple never freezes player
- Wall run only up/sideways; down input switches to crawl
- Camera rotates head only on walls (body locked)

## Combat
- Attacks Physical only; skills Magical only
- Weapon teleport works (hand ↔ back/belt)
- Casting interrupts correctly (short CD 0.5s)
- Cast can be canceled by own attack (skill → short CD)
- Attack can be canceled by starting a cast (combo resets)
- Perfect parry:
  - works only vs TAP
  - cancels enemy combo
  - applies micro-stun 0.1–0.2s
  - cue appears during parry window
- Dodge:
  - consumes stamina
  - i-frames behave correctly
  - avoids projectiles correctly
  - cone/AoE require exiting area to avoid damage

## Interaction
- Prompt appears only when target valid
- Pressing F triggers correct target effect

## Performance / AOI
- Client does not receive entities outside AOI
- Replication tiers reduce update rates mid-range
- Event-driven status updates work without spamming state
- Spawn zones never overlap player bodies

- Ground jump works only when grounded
- Wall jump works from crawl and run
- Only one air jump allowed before landing
- Air jump resets only on landing
- Grapple release preserves momentum and allows air jump
