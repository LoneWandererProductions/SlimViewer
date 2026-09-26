# SlimViewer

**SlimViewer** is a Windows-based image viewer, editor and image-management tool written in C# and WPF.

The project started as a lightweight image viewer and has evolved into a modular image-processing and editing framework. Besides the desktop application itself, the repository contains reusable libraries for image processing, comparison and analysis, textures, file handling, plugins and UI components.

The application is designed to handle both individual images and larger image collections while keeping the underlying imaging functionality separated into reusable components.

## Features

### Image Viewer & Management

- Open and browse individual images
- Open complete folders and optionally include subfolders
- Thumbnail-based image navigation
- File search
- File deletion, moving and renaming
- Refresh image collections
- Support for CBZ/comic archives
- GIF playback
- Optional zoom locking
- Image metadata and analysis

### Image Editing

SlimViewer provides a collection of direct image-editing operations:

- Undo / Redo
- Drawing
- Erasing
- Color selection
- Mirroring
- 90° rotation in both directions
- Scaling
- Brightening and darkening
- Pixelation
- Texture generation
- Image filters
- Export to clipboard
- Export image data as text

The editor also contains configurable drawing and texture tools.

### Experimental Layered Document System

SlimViewer contains an **experimental layered document system** that is currently being developed as a foundation for future editing functionality.

The system currently provides a document model with:

- Raster layers
- Shape layers
- Layer visibility
- Layer opacity
- Layer ordering
- Layer selection
- Layer operations
- Command-based history
- Undo / Redo
- Document serialization

Shape layers can represent primitives such as:

- Lines
- Rectangles
- Ellipses
- Polygons
- Polylines

Documents can be serialized as `.slimdoc` files.

**Important:** The layered document system currently has **no user-facing editor frontend** in SlimViewer. It is an experimental backend/core component and is not yet integrated into the normal image-editing workflow.

The existing SlimViewer editing functionality is still primarily based on the established bitmap-oriented editing pipeline.

The purpose of the new document system is to provide a foundation for future non-destructive and layered editing without coupling the document model directly to the WPF frontend.

## Image Comparison & Analysis

SlimViewer contains a dedicated image comparison subsystem.

### Duplicate Detection

Images can be searched for:

- Exact duplicates
- Perceptually similar images

Multiple folders can be included in a search, with optional subfolder processing.

### Detailed Comparison

Two images can be compared side by side with:

- Image information
- Difference-image generation
- Color information
- Detailed comparison data
- Export of comparison results

## Batch Processing

SlimViewer includes several tools for processing larger image collections.

### Batch Resizer

The batch resizer supports:

- Percentage-based scaling
- Explicit pixel dimensions
- Optional image filters
- Output format conversion
- Separate input and output folders

### Batch Rename

The batch rename tool supports:

- Text replacement
- Text removal
- Appending text
- Removing text from filename beginnings/endings
- Removing a configurable number of characters
- Number reordering
- Live preview before changes are written to disk

### Format Conversion

Images can be converted between supported formats through the format conversion tool.

## GIF Editing

SlimViewer contains a GIF frame editor with support for:

- Frame inspection
- Saving individual frames
- Applying a delay to all frames
- GIF playback

## Filters

The current filter system contains a range of image-processing filters, including:

- Grayscale
- Invert
- Sepia
- Black & White
- Polaroid
- Contour
- Brightness
- Contrast
- Hue Shift
- Color Balance
- Vintage
- Sharpen
- Gaussian Blur
- Emboss
- Box Blur
- Laplacian
- Edge Enhance
- Motion Blur
- Unsharp Mask
- Difference of Gaussians
- Crosshatch
- Floyd-Steinberg Dithering
- Anisotropic Kuwahara
- Supersampling Antialiasing
- Post-Processing Antialiasing
- Pencil Sketch Effect

Filters are implemented in the imaging library rather than being tied directly to the WPF application.

## Texture Generation

The texture subsystem provides procedural texture generation.

Current texture types include:

- Noise
- Clouds
- Marble
- Wood
- Wave
- Crosshatch
- Concrete
- Canvas
- Cellular
- Color Mapped
- Magical Ether
- Cobblestone
- Dragon Scales
- Lava Pool

Texture generation is separated into its own `Imaging.Texture` project and can therefore be reused independently of the main viewer.

## Image Format Support

The application works with common image formats including:

- PNG
- JPG / JPEG
- GIF
- BMP
- TIFF / TIF
- WebP
- CBZ

Additional formats can be provided through the plugin system.

Experimental/custom formats are also available through the CIF subsystem.

## Plugin Architecture

Image format support is extensible through a dedicated plugin interface.

The plugin system separates image decoder/encoder implementations from the core imaging library.

Plugins can implement interfaces such as:

- `IImageDecoderPlugin`
- `IImageEncoderPlugin`

The repository currently contains example/plugin implementations for:

- WebP
- PPM (Netpbm P6)

The plugin architecture is designed so additional formats can be added without modifying the SlimViewer application itself.

## Architecture

SlimViewer is split into several projects rather than keeping the entire application in a single executable.

### Application

**`SlimViewer`**

The WPF application and application entry point.

### Views & Application Logic

**`SlimViews`**

Contains views, image-processing commands, tooling windows, document controller and UI-facing application logic.

### Imaging

**`Imaging`**

Core image-processing functionality, including image rendering, pixel operations, filters, GIF support, image streams, masks, image conversion and plugin loading.

### Layered Image Documents

**`Imaging.Objects`**

A WPF-independent experimental document and editing model containing:

- Documents
- Layers
- Raster layers
- Shape layers
- Shapes
- Commands
- History
- Serialization
- Raster codecs
- Rendering infrastructure

This project is intended to keep the core document model independent from the WPF UI.

### Image Comparison

**`Imaging.Compare`**

Contains image comparison, duplicate detection, similarity detection and image analysis functionality.

### Texture System

**`Imaging.Texture`**

Procedural texture generation and raw texture manipulation.

### Image Plugins

**`Imaging.Plugins.Interface`**

Interfaces required by image decoder/encoder plugins.

**`Imaging.Plugins`**

Plugin implementations and examples.

### File Handling

**`FileHandler`**

Reusable file-management functionality including:

- Copy
- Move
- Delete
- Safe delete
- Rename
- Search
- Sorting
- File observation
- Extension handling
- File metadata

### UI Components

Several projects contain reusable WPF components:

- `Common.Controls`
- `Common.Dialogs`
- `Common.Images`
- `SlimControls`
- `ViewModel`
- `Common.Converter`

## Testing

The repository contains automated tests for the experimental layered document system, including tests for:

- Document creation
- Layer operations
- Shapes
- Undo/Redo history
- Raster editing
- Serialization
- Flattening/rendering

The test project is:

`Imaging.Objects.Tests`

## Technology

SlimViewer is built with:

- **C#**
- **.NET 9**
- **WPF**
- **System.Drawing** for parts of the current imaging pipeline
- Native/unsafe memory operations where appropriate

The main application targets Windows:

```text
net9.0-windows
```

The layered document core is separated from the WPF application and targets:

```text
net9.0
```

## Project Status

SlimViewer is an actively developed project.

The current application provides a functional image viewing, management and bitmap-editing workflow with additional tools for image comparison, batch processing, GIF editing, texture generation and image format plugins.

Alongside the existing application, the repository contains several experimental components that are being developed independently.

The **layered document system** is one of these experiments. It currently provides the underlying document, layer, shape, command-history and serialization infrastructure, but **does not yet have a user-facing frontend** and should therefore be considered experimental.

The intention is to use this system as a foundation for future layered and non-destructive editing functionality.

Other parts of the repository may likewise be experimental or under active development.

## Repository Structure

```text
SlimViewer/
├── SlimViewer/                 # WPF application
├── SlimViews/                  # Views and application logic
├── SlimControls/               # Editing controls
├── ViewModel/                  # MVVM helpers
│
├── Imaging/                    # Core imaging functionality
├── Imaging.Objects/            # Experimental layered document model
├── Imaging.Compare/            # Comparison and duplicate detection
├── Imaging.Texture/             # Procedural textures
├── Imaging.Plugins/             # Image format plugins
├── Imaging.Plugins.Interface/   # Plugin interfaces
├── Imaging.Cifs/               # CIF/custom image format
│
├── FileHandler/                # File management
├── Common.Images/              # Image-related WPF controls
├── Common.Controls/            # General WPF controls
├── Common.Dialogs/             # Dialog infrastructure
├── Common.Converter/           # WPF converters
│
├── Extended.Extensions/        # Extended collection/helpers
├── Extended.Objects/           # Data structures and memory utilities
├── Extended.Unmanaged/         # Unmanaged collections
├── Core.MemoryLog/             # Logging
├── DataFormatter/              # Data/file formatting
└── Mathematics.Constants/      # Mathematical helpers/constants
```

## Building

Clone the repository and open the solution in Visual Studio with the .NET 9 SDK installed.

Build the solution and run the `SlimViewer` project.

Plugin assemblies are loaded from the application's plugin directory.

## License

See the repository license files for the current licensing terms.
