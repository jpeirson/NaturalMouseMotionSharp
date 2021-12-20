# NaturalMouseMotionSharp

## Build Status

[![.NET](https://github.com/jpeirson/NaturalMouseMotionSharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jpeirson/NaturalMouseMotionSharp/actions/workflows/dotnet.yml)
[![NuGet version (SoftCircuits.Silk)](https://img.shields.io/nuget/v/NaturalMouseMotionSharp.svg?style=flat-square)](https://www.nuget.org/packages/NaturalMouseMotionSharp/)

## Overview

This is a C# port of [JoonasVali/NaturalMouseMotion](https://github.com/JoonasVali/NaturalMouseMotion) Java library. 
Where possible, this port is a one-to-one mapping of the Java library, but conforms to C# naming style, and uses
C# features where appropriate, e.g., `async` methods.

This library depends only on the `Microsoft.Extensions.Logging.Abstractions` package.

----

The rest of this README is a C# port of JoonasVali's README for the original Java library...

----

## NaturalMouseMotion ##

This library provides a way to move cursor to specified coordinates on screen reliably,
while being randomly arced to look like real hand moved it there by using a mouse.
The default settings should look good enough for most cases, but if user wishes, 
they can heavily customize the settings and implementations responsible for the trajectory 
of the cursor for specific use cases.

Some of the features NaturalMouseMotion contains:

  * **Deviation**: Deviation leads the mouse away from direct trajectory, creating and arc instead of straight line
  * **Noise**: Noise creates errors in the movement, this can simulate hand shakiness, someone using a non accurate mouse or bad surface under the mouse.
  * **Speed** and **flow**: Speed and flow are defining the progressing of the mouse at given time, for example it's possible that movement starts slow and then gains speed, or is just variating.
  * **Overshoots**: Overshoots happen if user is not 100% accurate with the mouse and hits an area next to the target instead, requiring to adjust the cursor to reach the actual target.
  * **Coordinate translation**: Coordinate translation allows to specify offset and dimensions to restrict a movement in a different area than the screen or in a virtual screen inside the real screen.

## Demonstration video (2.0.0, Java version): ## 
https://www.youtube.com/watch?v=CuG9LvQ0fdQ

## Project Configuration: ##

Include the following in your project to consume this package. Substitute the `Version` attribute as necessary.

```xml
<ItemGroup>
  <PackageReference Include="NaturalMouseMotionSharp" Version="0.2" />
</ItemGroup>
```

## Running: ##

NaturalMouseMotion is compiled against `netstandard2.0`, and runs on .NET Framework 4.6.1+ and .NET Core 2.0+.

You start by creating a new `NaturalMouseMotionSharp.Api.MouseMotionFactory`
or using the default instance, by calling `MouseMotionFactory.Default;`

To build a `MouseMotion` instance you use the factory instance:
`MouseMotion motion = mouseMotionFactory.Build(int xDest, int yDest);`

And the reusable `MouseMotion` can be run by using the
`motion.Move();` blocking method, or `motion.MoveAsync()` async method, which then moves the cursor.
This instance can be saved to call later or repeatedly.

Shorthand method for moving the mouse can be also found from the factory,
so a quick access can be found by calling `MouseMotionFactory.Default.Move(int xDest, int yDest);`

## Precooked templates ##

NaturalMouseMotion includes [FactoryTemplates](https://github.com/jpeirson/NaturalMouseMotionSharp/blob/master/NaturalMouseMotionSharp/Util/FactoryTemplates.cs) class which contains a variety of behaviors for your simulations.

  * **GrannyMotionFactory** is a slow and clumsy mouse movement with a lot of noise.
  * **FastGamerMotionFactory** is a fast and persistent movement with low noise, simulating behavior of someone who knows where the click should land, like a professional gamer.
  * **AverageComputerUserMotionFactory** is a medium speed movement with average settings, slower than fast gamer, but much faster than granny motion.
  * **DemoRobotMotionFactory** is featureless constant speed robotic movement with no mistakes.
  
## Translating coordinates from real screen to virtual screen ##

Sometimes you might need to simulate the mouse movement somewhere else than in the real screen or keep the mouse in a smaller area in the screen. 
In this case it is possible to perform coordinate translation by limiting the screen to certain area only. The following code is restricting the 
movement to **500*500** box at coordinates **(50,50)** on screen.
```csharp
    MouseMotionFactory factory = new MouseMotionFactory();
    var screenSize = new System.Drawing.Size(500, 500);
    var offset = new System.Drawing.Point(50, 50);
    factory.Nature = new ScreenAdjustedNature(new DefaultRobot(), screenSize, offset);
    // Rest of the factory settings here
```

For example by calling `factory.Move(10, 10)`, the mouse is going to be moved to **(60, 60)** on real screen as the offset is **(50, 50)**.
Attempting to move the mouse outside the **500x500** box with larger values is going to make it hit the imaginary wall at real screen coordinates **(550, 550)**.

NB: By setting the nature in the factory all previously set nature settings are lost because the instance is reassigned, which means setting nature should be done 
as a first thing in the factory, before any other settings are touched (as the settings are stored in the nature).

## How do I get a mouse that works on multiple screens?

You use the previously mentioned `ScreenAdjustedNature` and configure it to extend the main screen area to the side which the other screen resides.

Extending main screen to left
```csharp
    // How to extend screen to left. This means your main monitor is in the right and additional screen is left
    // of the main screen.
    var screenOnTheLeft = new System.Drawing.Size(2000, 1080);
    var mainScreen = new System.Drawing.Size(2000, 1080);

    ScreenAdjustedNature nature = new ScreenAdjustedNature(
        // Automation object. Can substitute with your own IRobot for emulating movements with
        // something other than a mouse or something other than a screen.
        new DefaultRobot(),
        // The new dimension of the screen is a sum of both screen dimensions:
        new System.Drawing.Size(screenOnTheLeft.width + mainScreen.width, bothScreenHeight),
        // Adjust the (0, 0) point to be at the left upper corner of the left screen:
        new System.Drawing.Point(-screenOnTheLeft.width, 0)
    );

    // Creates a factory with the screen adjusted nature.
    MouseMotionFactory factory = FactoryTemplates.CreateAverageComputerUserMotionFactory(nature);

    // Move mouse to the left upper corner of the left screen
    factory.Move(0, 0);
    // Move mouse to the left upper corner of the right screen
    factory.Move(screenOnTheLeft.width + 1, 0);
```

Extending main screen to right
```csharp
    // How to extend screen to right. This means your main monitor is in the left and additional screen is right
    // of the main screen.
    var screenOnTheRight = new System.Drawing.Size(2000, 1080);
    var mainScreen = new System.Drawing.Size(2000, 1080);

    ScreenAdjustedNature nature = new ScreenAdjustedNature(
        new DefaultRobot(),
        // The new dimension of the screen is a sum of both screen dimensions:
        new System.Drawing.Size(screenOnTheRight.width + mainScreen.width, bothScreenHeight),
        // Adjust the (0, 0) point to be at the left upper corner of the main screen:
        new System.Drawing.Point(0, 0)
    );

    // Creates a factory with the screen adjusted nature.
    MouseMotionFactory factory = FactoryTemplates.CreateAverageComputerUserMotionFactory(nature);

    // Move mouse to the left upper corner of the main (left) screen
    factory.Move(0, 0);
    // Move mouse to the left upper corner of the right screen
    factory.Move(mainScreen.width + 1, 0);
```

## What if I want to move something else than a mouse on something else than a screen?

In this case you need to provide your own `IRobot` to the factory. 

You need to implement the `MoveMouse(int x, int y)` so it will call the necessary device with new coordinates.

You need to implement the `GetMouseLocation()` and `GetScreenSize()` methods to get the cursor coordinates from the specified device.

And that's basically it, rest is handled by NaturalMouseMotion internals.

### I want to run NaturalMouseMotion in a headless environment

See previous section. You need to provide your own `IRobot` object.
If you attempt to use default implementations in a headless environment, then an exception will be thrown.

## Troubleshooting

### The mouse gets stuck trying to move to destination or won't end up on correct pixel.

There are several possible causes to that behavior:

* You can use `SystemDiagnosis.ValidateMouseMovement();` to test if the mouse positioning works as intended.
* There is some other program moving the mouse in a separate process or other thread moving the mouse in the same process working against the goal of the library. Solution is to make sure this doesn't happen.
* You accidentally move the mouse manually during the process. (This shouldn't cause a lasting effect, just temporary hiccup in the trajectory)
* You have altered MouseMotionFactory configuration in a way that causes it to get stuck.
* There might be some security setting in your computer preventing the mouse from moving.
* There's an actual bug somewhere in the NaturalMouseMotionSharp library that needs fixing on the library side.
