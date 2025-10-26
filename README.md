# MathMonsters-AR

## Requirements

- GitHub Desktop (for an easy clone workflow)
- Unity Hub (to manage Unity installs and projects)
- Unity Editor (2022.3.6.2f2 LTS)
- Vuforia Engine (installed into the project via Asset Store / Package Manager)

## Quick setup

### 1) Clone the repo using GitHub Desktop
1. Open GitHub Desktop.
2. File → Clone repository → URL (or choose from your GitHub repositories).
3. Enter the clone location (choose a folder, e.g., a workspace directory).
4. Click Clone.
5. After cloning, open the local folder in File Explorer to confirm files are present.

### 2) Add the project to Unity Hub
1. Open Unity Hub.
2. In the Projects tab click "Add".
3. Browse to the folder where you cloned the repo and select the project root (the folder containing Assets and ProjectSettings).
4. Click "Open" to add the project to Unity Hub.
5. Launch the project using the Unity Editor version that matches the project (install via Unity Hub if needed).

### 3) Install Vuforia Engine
There are two common ways to add Vuforia to this Unity project:

- From the Unity Asset Store (Editor)
  1. In the Unity Editor open Window → Asset Store (or use the browser and sign in at assetstore.unity.com).
  2. Search for "Vuforia Engine".
  3. Download and then Import the Vuforia Engine package into the open project.
  4. Follow any setup prompts and accept required package imports.

- From Package Manager (recommended if a scoped package is available)
  1. In the Unity Editor open Window → Package Manager.
  2. If Vuforia appears in the list, select it and click Install.
  3. If not listed, use "Add package from git URL..." or "Add package from disk..." and provide the package source if you have a specific package folder or ID (e.g., a local/composed package).
  4. After installation, follow Vuforia setup instructions (e.g., enable Vuforia in Player Settings if required).

Notes:
- After installing Vuforia you may need to configure license key