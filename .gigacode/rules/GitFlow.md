---
mode: all
apply: apply
---

# Git Workflow & Commit Standards

## Branching Strategy
1.  **Base Branch:** Always start new work from the latest `main`.
2.  **Sync Before Start:** Before creating a feature branch, ensure your local `main` is up to date: 
    ```bash
    git checkout main && git pull origin main
    ```
3.  **Naming Convention:** All feature branches must follow the pattern:
    `feature/MIG-{TaskNumber}`
    *   Example: `feature/MIG-52`
    *   Do not use prefixes like `fix/`, `hotfix/`, or personal names like `misha-branch`.

## Commit Message Format
Every commit message MUST include the task ID (Jira/MIG key) at the very beginning of the subject line.

**Template:**
`MIG-XXX: Short description in imperative mood`

*   **Correct Examples:**
    *   `MIG-52: Implement employee profession filtering on backend`
    *   `MIG-53: Add debounce to search input field`
    *   `MIG-54: Fix Cyrillic encoding issue in Docker migrations`
    
*   **Incorrect Examples:**
    *   `Added filter logic` (Missing ID)
    *   `I fixed the bug MIG-52` (Wrong format, wordy)
    *   `[MIG-52] Filter` (Brackets are optional but colon and space after ID are preferred for readability)