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

- ImGUI is Designed primary for Development/Debugging UI
- Persistent UI State between Draw calls
- That's resulting in higher frame rates
- Simpler creation of custom controls
- No need for implementing the low levle rendering interface (vertex data, ...)

Differences to AvalonUI
-----------------------

- AvalonUI is designed primary for GUI-First applications, not for 3D games.
- AvalonUI is overloaded with features you do not need in 3D games.
- AvalonUI has Less control in rendering pipeline

Development Status
------------------

AxGui is currently in Development. There's no stable release yet.