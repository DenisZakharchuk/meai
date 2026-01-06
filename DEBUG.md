# Debug & Launch Configuration Guide

## Overview

The MeAI project includes comprehensive VS Code debugging and launch configurations with **automatic Docker and PostgreSQL container startup**.

When you press **F5**, the following happens automatically:
1. ✅ Docker daemon starts (if not running)
2. ✅ PostgreSQL and pgAdmin containers start (if not running)
3. ✅ Application is built
4. ✅ Debugger is attached

This ensures the persistence layer is always ready before the application starts.

## 🚀 Launch Configurations

Three launch profiles are available in `.vscode/launch.json`:

### 1. Debug (Default - Recommended)
```
.NET Core Launch (console)
```
- Launches in **Debug** mode
- Automatically starts Docker daemon if needed
- Automatically starts PostgreSQL/pgAdmin containers if needed
- Uses integrated terminal
- Best for development with persistence layer

**To launch**: Press `F5` or use Debug → Start Debugging

**What happens:**
```
F5 → ensure-docker-compose → ensure-docker-daemon → build → Launch debugger
```

### 2. Release
```
.NET Core Launch (Release)
```
- Launches in **Release** mode (optimized build)
- Same Docker/container startup as Debug
- For testing production behavior

**To launch**: Press `Ctrl+Shift+D`, select "Release"

### 3. Attach to Process
```
.NET Core Attach
```
- Attach debugger to a running process
- Useful for debugging already-running applications

## 📋 Automatic Setup Tasks

New automatic startup tasks have been added:

### ensure-docker-daemon
**Purpose:** Check if Docker is running and start it if needed

**What it does:**
- Runs `docker ps` to check if daemon is responding
- If Docker is not running, starts it with `sudo service docker start`
- Falls back to `sudo /usr/bin/dockerd` if service command fails
- Waits 3 seconds for daemon initialization
- Output is silent to avoid terminal clutter

**Requires:**
- Docker installed
- Optional: Passwordless sudo for docker commands
  ```bash
  sudo visudo
  # Add line: username ALL=(ALL) NOPASSWD: /usr/sbin/service, /usr/bin/dockerd
  ```

### ensure-docker-compose
**Purpose:** Check if PostgreSQL container is running and start docker-compose if needed

**What it does:**
- Checks if `meai-postgres` container is running
- If not running, executes `docker-compose up -d`
- Waits 5 seconds for containers to fully initialize
- Output is silent (check output panel if needed)
- Depends on: `ensure-docker-daemon` (runs it first)

**Containers started:**
- PostgreSQL on localhost:5432
- pgAdmin on http://localhost:5050

## 📋 Build Tasks

Available tasks in `.vscode/tasks.json`:

| Task | Command | Purpose | Dependencies |
|------|---------|---------|--------------|
| **ensure-docker-daemon** | Docker check/start | Ensure Docker is running | None |
| **ensure-docker-compose** | docker-compose up -d | Ensure containers running | ensure-docker-daemon |
| **build** (default) | `dotnet build` | Debug build | ensure-docker-compose |
| **build-release** | `dotnet build --configuration Release` | Release build | ensure-docker-compose |
| **clean** | `dotnet clean` | Remove build artifacts | None |
| **publish** | `dotnet publish` | Publish application | None |
| **run** | `dotnet run` | Run application directly | None |
| **watch** | `dotnet watch` | Watch mode (auto-rebuild) | None |
| **test** | `dotnet test` | Run tests | None |
| **test** | `dotnet test` | Run tests |

### Running Tasks

**Keyboard Shortcuts**:
- Press `Ctrl+Shift+B` to run default build task
- Press `Ctrl+Shift+P` → "Tasks: Run Task" to select specific task

**Command Palette**:
1. Press `Ctrl+Shift+P`
2. Type "Tasks: Run Task"
3. Select desired task

## 🔧 Debugging Features

### Setting Breakpoints
1. Click on the line number in the editor
2. A red circle appears at the breakpoint
3. Press `F5` to start debugging
4. Execution pauses at breakpoints

### Debug Controls
- **Continue** (`F5`): Resume execution
- **Step Over** (`F10`): Execute next line
- **Step Into** (`F11`): Enter function calls
- **Step Out** (`Shift+F11`): Exit current function
- **Restart** (`Ctrl+Shift+F5`): Restart debugging session
- **Stop** (`Shift+F5`): Stop debugging

### Debug Views
- **Variables**: Local and global variables
- **Watch**: Monitor specific expressions
- **Call Stack**: Function call hierarchy
- **Breakpoints**: Manage all breakpoints
- **Debug Console**: Execute expressions, view output

## ⚙️ Editor Settings

Configured in `.vscode/settings.json`:

```json
{
    "omnisharp.enableRoslynAnalyzers": true,
    "omnisharp.enableEditorConfigSupport": true,
    "[csharp]": {
        "editor.defaultFormatter": "ms-dotnettools.csharp",
        "editor.formatOnSave": true
    }
}
```

### Features Enabled
- ✅ Roslyn analyzers for code analysis
- ✅ EditorConfig support
- ✅ Automatic code formatting on save
- ✅ Auto-import organization
- ✅ Code actions on save

## 🔍 Debugging Examples

### Example 1: Debug a Chat Example
1. Open `Services/ChatExample.cs`
2. Click line 30 to set a breakpoint (before async operation)
3. Press `F5` to start debugging
4. Run the application and select option 1 (Chat Example)
5. Execution pauses at your breakpoint
6. Inspect variables in the Variables panel

### Example 2: Watch Expression
1. During debugging, press `Ctrl+Shift+P`
2. Select "Debug: Add to Watch"
3. Enter variable name (e.g., `messages`)
4. Variable is tracked in Watch panel

### Example 3: Debug Console
1. While debugging, press `Ctrl+Shift+Y`
2. Type C# expressions to evaluate
3. Example: `messages.Count`

## 💡 Best Practices

1. **Use Breakpoints Strategically**
   - Set breakpoints at critical decision points
   - Don't leave excessive breakpoints active

2. **Configure User Secrets**
   ```bash
   dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
   ```
   - Prevents hardcoded API keys
   - Secure for development

3. **Run in Watch Mode**
   ```bash
   Ctrl+Shift+P → "Tasks: Run Task" → "watch"
   ```
   - Automatically rebuilds on file changes
   - Great for rapid development iteration

4. **Use Conditional Breakpoints**
   - Right-click breakpoint → Edit Breakpoint
   - Set conditions: `messages.Count > 2`
   - Pauses only when condition is true

5. **Profile Performance**
   - Use Release build for performance testing
   - Compare Debug vs Release performance

## 🐛 Troubleshooting

### Breakpoints Not Hitting
1. Ensure Debug build is active
2. Check if code is optimized away
3. Rebuild solution (`Ctrl+Shift+B`)
4. Restart VS Code if needed

### Cannot Find DLL
1. Run `dotnet build` from terminal
2. Check path in `launch.json` matches your project
3. Verify `.NET 8.0` is installed: `dotnet --version`

### API Key Errors During Debug
1. Ensure user secrets are set
2. Check `appsettings.json` has placeholder
3. Verify `OpenAI:ApiKey` configuration

## 📚 Resources

- [VS Code Debugging](https://code.visualstudio.com/docs/editor/debugging)
- [C# Debugging in VS Code](https://github.com/OmniSharp/omnisharp-vscode)
- [.NET CLI Commands](https://docs.microsoft.com/en-us/dotnet/core/tools/)

## Quick Reference

| Action | Shortcut |
|--------|----------|
| Start Debug | `F5` |
| Stop Debug | `Shift+F5` |
| Restart | `Ctrl+Shift+F5` |
| Step Over | `F10` |
| Step Into | `F11` |
| Step Out | `Shift+F11` |
| Continue | `F5` |
| Toggle Breakpoint | `Ctrl+K Ctrl+B` |
| Toggle Line Comment | `Ctrl+/` |
| Run Task | `Ctrl+Shift+B` |

## 🐳 Docker + Persistence Layer Debugging

### View Container Status

```bash
# Check running containers
docker-compose ps

# View PostgreSQL logs
docker-compose logs -f postgres

# View pgAdmin logs
docker-compose logs -f pgadmin

# Connect to PostgreSQL
psql -h localhost -U meai_user -d meai_db

# Access pgAdmin web UI
# Open browser: http://localhost:5050
# Login: admin@meai.local / admin_password
```

### Troubleshoot Container Issues

**Containers won't start:**
```bash
# Check for port conflicts
sudo lsof -i :5432  # PostgreSQL port
sudo lsof -i :5050  # pgAdmin port

# View detailed error logs
docker-compose logs postgres pgadmin
```

**Docker daemon not responding:**
```bash
# Manual Docker start
sudo service docker start

# Or as root:
sudo /usr/bin/dockerd
```

**Tasks running but appear to hang:**
- Open Output panel: View → Output → Tasks
- Look for any error messages
- Run manual check: `docker ps`

## 🔄 Typical Debug Workflow

### First Launch (First-Time Setup)
```
1. Press F5
2. Tasks run automatically:
   - �� Docker daemon check (may prompt for password)
   - 🐘 PostgreSQL & pgAdmin startup (waits 5 seconds)
   - Build application
3. Debugger launches with breakpoints ready
4. Application starts in integrated terminal
5. Persistence layer is ready to use
```

### Subsequent Launches (Containers Already Running)
```
1. Press F5
2. Tasks run quickly:
   - ✓ Docker already running (skipped)
   - ✓ PostgreSQL already running (skipped)
   - Build application
3. Debugger launches immediately
4. Application starts
```

### Stopping Debug Session
```
1. Press Shift+F5 (or click Stop button)
2. Debugger stops
3. Containers remain running (you can restart immediately)
4. To stop containers: docker-compose down
```

## 🔧 Configuration Files Reference

### .vscode/launch.json
Controls debug configurations and startup behavior:
- `preLaunchTask`: Task to run before debugging (now: ensure-docker-compose)
- `program`: Path to compiled binary
- `console`: Output console type (integrated terminal)

### .vscode/tasks.json
Defines automated tasks:
- `ensure-docker-daemon`: Docker startup check
- `ensure-docker-compose`: Container startup check
- `build`: Compilation task (depends on containers being ready)
- Dependencies ensure correct execution order

### docker-compose.yml
Container configuration:
- PostgreSQL service on port 5432
- pgAdmin service on port 5050
- Health checks for reliability
- Persistent volumes for data

## 📊 Output Panel Guide

When running tasks, open the Output panel (View → Output) to see:

**Task Output:**
```
> Executing task: ensure-docker-daemon
🐳 Starting Docker daemon...
✓ Docker started

> Executing task: ensure-docker-compose
🐘 Starting PostgreSQL and pgAdmin...
✓ Containers started

> Executing task: build
Microsoft (R) Build Engine version 17.8.43+f0cbb1397
... compilation ...
Build succeeded
```

**Debug Output:**
Shows breakpoint hits, variable changes, and debug console output.

**Problems:**
Compilation errors and warnings are displayed here.

## ✅ Verification Checklist

When setting up debugging for the first time:

- [ ] Docker is installed: `docker --version`
- [ ] Docker daemon can start: `docker ps`
- [ ] docker-compose is installed: `docker-compose --version`
- [ ] VS Code C# extension is installed
- [ ] .vscode folder contains: launch.json, tasks.json, settings.json
- [ ] appsettings.json has connection string
- [ ] OpenAI API key is set in user-secrets
- [ ] F5 launches without errors
- [ ] PostgreSQL container shows in `docker ps`
- [ ] Can connect to pgAdmin at http://localhost:5050

## 🆘 Common Issues

### Issue: "Docker command not found"
```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
```

### Issue: Permission denied (Docker)
```bash
# Add user to docker group
sudo usermod -aG docker $USER
newgrp docker
```

### Issue: Port already in use
```bash
# Kill process using port 5432
sudo lsof -ti:5432 | xargs kill -9

# Or stop conflicting container
docker-compose down
```

### Issue: Breakpoints not working
- Ensure you're in Debug configuration (not Release)
- Rebuild solution: Ctrl+Shift+B
- Restart debugger: Shift+F5 then F5

### Issue: Variable inspection doesn't work
- Check you're paused at breakpoint
- Hover over variable in editor
- Or use Debug Console (Ctrl+Shift+Y)

## 🔗 Related Documentation

- [PERSISTENCE.md](PERSISTENCE.md) - Database persistence guide
- [README.md](README.md) - Project overview
- [VS Code Debugging](https://code.visualstudio.com/docs/editor/debugging)
- [.NET Debugging](https://learn.microsoft.com/en-us/dotnet/fundamentals/)
