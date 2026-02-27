# Dependencies & Versions (Pinned)

This project pins exact dependency versions to avoid unexpected behavior changes.

## Engine
- Unity: 6000.3.8f1 (LTS, pinned – do not change during production)

## Networking
- FishNet is used from project start.
- Core gameplay systems must remain network-agnostic.
- Networking is implemented via proxy layers.

## Input
- Unity Input System package — pin exact version installed in the project

## Pinning Rules (Must)
- Commit these files:
  - Packages/manifest.json
  - Packages/packages-lock.json
- Do not allow Unity Package Manager to auto-upgrade dependencies.
- Any dependency update requires:
  1) changelog entry
  2) testing against DEBUG_CHECKLIST.md
  3) explicit approval
