# Welcome to my ProjectStandards
> *Actual name pending...*

***

# About 
This is a library with standard methods, classes, structs and other for use within all my projects, their branches and modules, mostly due to me being lazy and not wanting to rewrite a bunch of stuff, and also for interprocess interop.
I recommend employing it as a NuGET package, and if using a VS compiler, compiling with `Trim Unused Code` selected, due to that you probably won't be using all modules within this SL.

***

# Modules

## Console.cs
Has a Linux and Windows supported custom console window (through Avalonia) for the logging of anything written to the `PrintLog` method, mostly used for debug but can be used in production for error tracing.

## InternalLogging.cs
Functionality for setting up and handling a log file given a path, with autoflushing, asynchronous operations and batch flushing, among others.

## Sanctum.cs & SanctumNotation.cs
These files are filled with the implementation and notation, respectively, for four classes, explained below:
### The Magistrate Class
Contains extension and helper methods for general use, including single line Exception processing, Bitwise operations, Console Extensions, single line function processing and other.
### Maid
Contains common Lists of items (Such as ana rray of numeric types), and other general use entities
### Mechanic
Contains custom structs, mostly used for interlibrary compatibility and conversion (such as having a multilayer compatible Point class for implicitly convertion between System.Drawing.Point and System.Windows.Point)
### Manta
Contains custom crossplatform (linux & windows) window objects (such as filedialogs)
### Melon
Contains Generic Math functions for all numeric types.
### Client and Server
Obsolete classes that attempt to use MMF manipulation to send and recieve interprocess commands (evolution of pipes, currently being renovated to use stdin, stdout & stderr) 
