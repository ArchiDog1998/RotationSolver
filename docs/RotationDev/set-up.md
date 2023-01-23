# Set Up

## Software

First you need to install the [Visual Studio](https://visualstudio.microsoft.com/). Please Check the `.NET desktop development`  under Workloads, `Class Designer` under Individual components.

![.NET desktop development](assets/image-20230122152037534.png)

![Class Designer](assets/image-20230122152134510.png)

## Repository

We use the fork workflow for rotation development. First you need to fork the [Rotation Solver](https://github.com/ArchiDog1998/RotationSolver) to your own repository. Then use Visual Studio to open it. Choose the directory to hold this solution.

![Fork and download](assets/image-20230122152448737.png)

## Configuration

Select the folder your `Dalamud` is in to the `DalamudLibPath`.

![Dalamud Path](assets/image-20230122153105601.png)

Build your own `dll`.

![Build yourself](assets/image-20230122153420605.png)

Go to the Game `FFXIV` and change the settings of dalamud. The path is under your directory set before.

NOTICE: before doing this, please disable or delete the version in release.

![Add plugin's Path](assets/image-20230122153730563.png)

Then, under the Dev Tools, you'll see the plugins you built!

![The plugin you built](assets/image-20230122154109122.png)