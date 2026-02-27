# Development Workflow Contract

This document is binding.

## Source of Truth
- Git repository is the single source of truth
- Chat does not override code or documentation

## AI Rules
- AI implements ONLY mechanics described in Docs
- AI writes complete systems, not fragments
- AI must not invent mechanics or shortcuts
- If ambiguity exists — AI must ask before coding

## Architecture Rules
- Script/file structure is locked until release
- Internal logic may evolve, structure may not
- No gameplay logic in input handlers
- No physics logic in animation scripts
- No UI logic affecting gameplay

## Workflow
1. Docs are updated and committed
2. AI writes full code systems (consistent with Docs)
3. Developer connects animations/assets and sets Unity project settings
4. Bugs are fixed without redesigning contract rules
