# CodeOverlay

**CodeOverlay** is a lightweight Windows desktop utility that allows users to compile and run C++ code snippets directly from the clipboard using global hotkeys.

The project is designed as a productivity tool for fast code testing, algorithm practice, and future AI-assisted code execution workflows.

---

## Overview

CodeOverlay started as a simple clipboard-based C++ compiler and evolved into a small desktop application with a user interface, background mode, overlay result window, and automatic code template generation.

The application can take incomplete C++ snippets from the clipboard, wrap them with predefined includes and a `main()` function if needed, compile the generated source file, run it, and display the result in a floating overlay window.

---

## Current Status

**Version:** `v0.5.0-alpha`  
**Status:** Active development  
**Platform:** Windows  
**Primary language:** C# / WPF  
**Target runtime:** .NET 9 Windows

---

## Features

### Implemented

- WPF desktop application
- Multi-screen UI navigation
- Home screen
- Settings screen
- GitHub screen
- Start / Exit / Save / Back controls
- Background mode after pressing Start
- Global hotkey handling
- Clipboard-based C++ code execution
- Automatic C++ include injection
- Automatic `main()` wrapper generation
- C++ compilation using Clang++
- Visual Studio Build Tools detection
- Overlay window for execution results
- Execution timeout protection
- Basic high-tech animated right panel
- Service-based project structure
- Centralized application shutdown logic
- GitHub repository integration
- Settings UI foundation

---

## Hotkeys

After pressing **Start**, the application hides the main window and begins listening for global hotkeys.

| Hotkey | Action |
|---|---|
| `Ctrl + Shift + Space` | Compile clipboard text with predefined C++ includes and generated `main()` |
| `Ctrl + Shift + Alt + O` | Stop overlay mode and reopen the main window |

---

## How It Works

The workflow is simple:

```text
Copy a small C++ snippet
    ↓
Press Start in CodeOverlay
    ↓
The app enters background mode
    ↓
Press a global hotkey
    ↓
CodeOverlay reads clipboard text
    ↓
Applies the selected C++ template mode
    ↓
Generates a temporary C++ source file
    ↓
Compiles the generated file with Clang++
    ↓
Runs the executable with timeout protection
    ↓
Displays stdout, stderr, or compiler errors in the overlay window
