# Git Workflow Guide

## Repository Status

```
Repository: MeAI (OpenAI Integration Project)
Branch: master
Commits: 1 (Initial commit)
Status: Clean (all changes committed)
```

## Basic Git Commands

### View Status
```bash
git status              # Show current status
git log --oneline       # View commit history
git log --graph --all   # Visual commit history
```

### Make Changes
```bash
git add <file>          # Stage specific file
git add .               # Stage all changes
git commit -m "message" # Create commit
git push                # Upload to remote (when configured)
```

### Branch Operations
```bash
git branch              # List branches
git branch <name>       # Create new branch
git checkout <branch>   # Switch to branch
git checkout -b <branch> # Create and switch to branch
git merge <branch>      # Merge branch into current
```

## Recommended Workflow

### 1. Create Feature Branch
```bash
git checkout -b feature/new-feature
# Make changes
git add .
git commit -m "Add new feature"
```

### 2. Keep Master Clean
```bash
git checkout master
git merge feature/new-feature
git branch -d feature/new-feature
```

### 3. Common Branches
- `master` - Production-ready code
- `develop` - Development branch
- `feature/*` - New features
- `bugfix/*` - Bug fixes

## Commit Message Convention

Use clear, descriptive messages:

```
feat: Add chat streaming support
fix: Correct embedding dimension
docs: Update README with examples
refactor: Simplify service initialization
test: Add unit tests for ChatService
chore: Update dependencies
```

## Quick Reference

| Command | Purpose |
|---------|---------|
| `git init` | Initialize repository |
| `git add .` | Stage all changes |
| `git commit -m "msg"` | Create commit |
| `git status` | View status |
| `git log` | View history |
| `git push` | Upload commits |
| `git pull` | Download commits |
| `git branch` | List branches |
| `git checkout -b name` | Create branch |
| `git merge branch` | Merge branch |

## Useful Aliases

Add to `.gitconfig` for shortcuts:

```bash
git config --global alias.st status
git config --global alias.co checkout
git config --global alias.br branch
git config --global alias.ci commit
git config --global alias.log 'log --oneline --graph'
```

## Remote Repository (When Ready)

### Connect to GitHub/GitLab
```bash
git remote add origin <repository-url>
git branch -M main      # Rename master to main
git push -u origin main # Push and set tracking
```

## Current Project Status

- ✅ Git repository initialized
- ✅ .gitignore configured
- ✅ Initial commit created
- ✅ Ready for feature development
- ⏳ Remote repository (when needed)

## Next Steps

1. Create feature branches for new features
2. Keep commits atomic and descriptive
3. Review changes before committing
4. Push to remote when configured
5. Use pull requests for collaboration

Happy coding! 🚀
