# Git Workflow & Branching Strategy (Assignment Section B)

This document is the team playbook for the source-control deliverables, plus the
screenshot checklist mapped to the marking rubric.

## Branching strategy

```
main  ------------------●-------●--------→   (stable; protected; merges via PR)
                          \       \
feature/donations-*       ●--●----/            (feature branches)
feature/volunteer-*              ●--●--→
```

- **`main`** is the stable branch. Nothing is committed to it directly.
- **Every feature gets a branch** named `feature/<what-it-does>` (kebab-case).
- Features are merged back via a **pull request with a review** (at least one
  teammate clicks through and approves before merging).
- When two feature branches touch the same file, the **second merge resolves the
  conflict deliberately** (see below) and documents it.

## Per-member workflow (group of up to 5)

Each member must commit to at least one feature/service. Recommended split:

| Member | Feature branch | Scope |
|---|---|---|
| A | `feature/functions-tax-certificate` | `Function/` project |
| B | `feature/donation-checkout` | donation flow + helpers usage |
| C | `feature/volunteer-portal` | volunteer sign-up + employee approvals |
| D | `feature/ci-pipeline` | `azure-pipelines.yml` + template |
| E | `feature/nuget-helpers` | `GiftOfTheGivers.Helpers` + tests |

For each feature:

```bash
git switch main
git switch -c feature/<name>       # 1. branch
# ...edit files...
git add -A
git commit -m "feat(<name>): what changed and why"   # 2. meaningful commit message
git push -u azure feature/<name>   # 3. push the branch
# 4. open a Pull Request in Azure DevOps (Repos -> Pull requests -> New)
# 5. teammate reviews & approves, then Complete the PR (merge)
```

## Demonstrating a conflict + resolution (for the top marking band)

1. Member A changes line X of a shared file on `feature/a`, merges to `main`.
2. Member B changes the **same lines** on `feature/b`; when merging, Git reports a
   conflict.
3. In Visual Studio: *Git → Resolve Conflicts* opens the merge editor — keep the
   correct combination of both changes, commit the merge with a message such as
   `merge: resolved conflict in Controllers/DonationController.cs (kept both the
   quick-amount UI and the anonymous toggle)`.
4. Screenshot the merge editor and the resulting history graph.

## Screenshot checklist (mapped to the rubric)

| Rubric row | What to capture |
|---|---|
| Repo setup in VS + Azure Repos | Repos file list showing all 4 projects; commit history (Team Explorer / Azure DevOps **History** view) |
| Branching & merging | Branch dropdown showing 2+ feature branches; commits distinct per member (author names differ); merge commits in the history graph; conflict-resolution editor if demoed |
| PRs reviewed / comments | Azure DevOps PR page with reviewer approval and a comment thread (also: this repo's GitHub PR #1 shows the same workflow) |

## Where things live

- GitHub (this repo): `https://github.com/MJjimmy/gifts` — branch
  `arena/01a0d077-gifts`, PR #1 (open, with review comment).
- Azure Repos: create the project in Azure DevOps, add it as a second remote and
  push the same history:

```bash
git remote add azure https://<org>@dev.azure.com/<org>/<project>/_git/<repo>
git push azure arena/01a0d077-gifts:main
```
