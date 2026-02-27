# Collision & Spawn Zones Contract v1.0 (FINAL)

## Player Collision (Physical Bodies)
- Players are physical bodies, not points.
- Two players must not occupy the same physical space.
- Player-to-player collision is enabled and must be stable.
- Collisions must not cause excessive jitter or desync.

## Spawn Zones (No Single Points)
- Spawning and teleporting do NOT use fixed points.
- Spawn uses zones/areas and picks a free spot.

### Spawn Zone Rules
- Zone defines volume/shape, min separation, max attempts.
- Choose random candidate inside zone.
- Validate:
  - no overlap with other players
  - no overlap with world obstacles
  - valid ground constraints (if required)
- Retry up to max attempts.
- If all fail: expand search / fallback zone / delay spawn.
