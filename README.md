# Terraria Clone (Chunk-Based Sandbox 2D)

A custom implementation of a 2D sandbox game heavily inspired by the mechanics of *Terraria*. Built from the ground up in Unity and .NET, this project focuses on high performance, strict modularity, and clean architectural patterns (SOLID principles).

The primary technical goal is to support a large, dynamic tile-based world with minimal CPU overhead and optimized RAM management.

## 🚀 Key Technical Features

*   **Dynamic Chunk Management (`ChunkManager`):** The world is divided into grid-based chunks that load and unload dynamically in real-time based on the position of the watched object (the player)[cite: 10]. This prevents the GPU from being overloaded with the entire map render at once.
*   **Efficient Object Pooling (`ChunkPoolManager`):** Instead of costly `Instantiate` and `Destroy` operations during exploration, the engine fully recycles inactive chunk objects[cite: 11]. Released sectors are pushed into a pool queue and immediately prepared for reuse, completely eliminating Garbage Collection spikes[cite: 11].
*   **Optimized Tilemap Rendering:** Drawing tiles inside each chunk is optimized using `SetTilesBlock`. The entire array of grid blocks is sent to the Unity native engine in a single batch call.
*   **Lightweight Save System (`SaveManager`):** World data is serialized directly into a raw binary format using C#'s `BinaryWriter` and `BinaryReader`[cite: 12]. This results in microscopic `.map` file sizes and near-instant world loading times[cite: 12].
*   **Smart Camera Confiner:** A Cinemachine 2D Virtual Camera dynamically adjusts its physical boundaries using an automatically generated Polygon Collider 2D[cite: 13]. This locks the camera view perfectly to the exact edges of the generated world array[cite: 13].

## 🛠️ Codebase Architecture

The project maintains highly decoupled layers of responsibility:
*   `WorldManager` – Central hub managing the global world state and dimensions[cite: 13].
*   `ChunkManager` – Responsible for grid coordinate mathematics and render distance filtering[cite: 10].
*   `ChunkPoolManager` – A dedicated object pool manager handling chunk lifecycle and recycling[cite: 11].
*   `SaveManager` – Independent I/O layer handling direct filesystem operations[cite: 12].
*   `WorldGenerator` – Independent generation layer utilizing Perlin Noise algorithms to construct the terrain grid.
*   `Chunk` / `TileData` / `WorldData` – Pure data models and rendering components representing blocks and map structure.

## 🗂️ Repository Structure

The repository is structured to separate root-level configuration files and external documentation from the core Unity engine assets:
*   `/Terraria clone project/` – The complete, clean Unity project directory (Source code, Assets, Packages, and Meta files).
*   The root folder contains version control configurations (`.gitignore`), project documentation, and core project assets.

---
*This project is actively developed for educational, architectural, and optimization purposes in advanced 2D Unity systems.*
