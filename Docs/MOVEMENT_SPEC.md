# Movement Specification (FINAL)

## Stamina Rules
Allowed at 0 stamina:
- Ground running
- Wall crawling

Disallowed at 0 stamina:
- Wall run
- Grapple swing
- Block
- Dodge (if stamina required and insufficient)

## Grapple (Q)
- Swing only (pendulum arc)
- No pull, no targeting, no hanging
- Auto/conditional release with momentum preservation
- Costs 15% stamina per use

## Wall Camera Behavior
- While wall crawling or wall running:
  - camera rotation affects head only
  - body orientation locked to wall
  - camera must not rotate full body on walls

## Landing Rule
Landing:
- clears air action remnants
- resolves locomotion immediately (idle/walk/run)


## Jump System

### Ground Jump
- Allowed only when grounded
- Applies upward impulse
- Does not consume stamina
- Switches player to Air state

### Wall Jump
- Can be performed from WallCrawl or WallRun
- Applies impulse away from wall and upward
- Consumes stamina
- After wall jump, one Air Jump is allowed

### Air Jump (Double Jump)
- Only one air jump is allowed before landing
- Resets only on landing
- Allowed after:
  - ground jump
  - wall jump
  - grapple release
- Consumes stamina (recommended 20–25%)

### Grapple Release
- Releasing grapple preserves momentum
- Does not reset air jump
- Air jump may be used after release
