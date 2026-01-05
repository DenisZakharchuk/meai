# Debug & Launch Configuration Guide

## Overview

The MeAI project includes comprehensive VS Code debugging and launch configurations for development and testing.

## 🚀 Launch Configurations

Three launch profiles are available in `.vscode/launch.json`:

### 1. Debug (Default)
```
.NET Core Launch (console)
```
- Launches in **Debug** mode
- Uses integrated terminal
- Automatically builds before launching
- Best for development and debugging with breakpoints

**To launch**: Press `F5` or use Debug → Start Debugging

### 2. Release
```
.NET Core Launch (Release)
```
- Launches in **Release** mode
- Optimized build for performance testing
- Use when you need to test production behavior

**To launch**: Press `Ctrl+Shift+D`, select "Release"

### 3. Attach to Process
```
.NET Core Attach
```
- Attach debugger to a running process
- Useful for debugging already-running applications

## 📋 Build Tasks

Available tasks in `.vscode/tasks.json`:

| Task | Command | Purpose |
|------|---------|---------|
| **build** (default) | `dotnet build` | Debug build |
| **build-release** | `dotnet build --configuration Release` | Release build |
| **clean** | `dotnet clean` | Remove build artifacts |
| **publish** | `dotnet publish` | Publish application |
| **run** | `dotnet run` | Run application directly |
| **watch** | `dotnet watch` | Watch mode (auto-rebuild) |
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
