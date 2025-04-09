# Welding-Simulator
Graduation project: A Unity-powered virtual welding simulator for training purposes.

## Rendering Pipeline
This project uses the **Universal Render Pipeline (URP)** for rendering.

## Packages Dependencies
- **Unity Version**: 2022.3.60f1
- **XR Plugin Management**: 4.5.1
- **XR Interaction Toolkit**: 2.6.4

## Project Settings
- **Unity Platform**: Switch to **Android** in **Build Settings**.
- **Texture Compression**: ASTC
- **XR Plugin Management**: Use **Oculus XR Plugin**.
- **Player Settings**: 
  - **Scripting Backend**: IL2CPP
  - **Target Architecture**: ARM64
- **XR Settings**: Enable **Virtual Reality Supported** and add **Oculus** SDK.
- **Output**: Build APK for deployment on Meta Quest 3.
- **API Level**
    - **Minimum API Level**: 29 (Android 10)
    - **Target API Level**: 32 (Android 12).