AxGui
=====

AxGui is a simple lightweight GUI. Supported backends:

- OpenGL
- Vulkan
- DirectX
- Bitmap
- Every surface type supported by Skia

How does rendering works?

Skia is a very efficient hardware accelated 2D Drawing library developed by google. AxGUI simply uses this library to draw the controls.

Benefits
--------

- Designed for embedding into 2D/3D Rendering applications (2D/3D Engine, Game engines, ...)
- Very few Object Allocations per draw call
- Very fast draws
- UI Calculations and Rendering could be done in seperate Threads
- Define UI via Code or XML
- Integrated SVG-Renderer
- All events (Mouse, Keyboard, Resize, ...) are virtualized.

Differences to ImGUI
--------------------

- Designed primary for Development/Debugging UI
- Persistent UI State between Draw calls
- That's resulting in higher frame rates
- Simpler creation of custom controls
- No need for implementing the low levle rendering interface (vertex data, ...)

Differences to AvalonUI
-----------------------

- Designed primary for GUI-First applications, not for 3D games.
- Overloaded with features you do not need in 3D games.
- Less control rendering pipeline