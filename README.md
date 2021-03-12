# IcoPngCombine
A small tool that combines an icon set represented as a set of PNGs into a set of .ICO files.

# Building
As this is not intended to be a packaged solution, you'll need to build this tool yourself.

To do this

1. Clone this repo with submodules.
2. Fix a typo in a `using` directive at the top of the file `IcoPngCombine\IconLib\System\Drawing\IconLib\LibraryFormats\PEFormat.cs`, it should say `IconLib` instead of `IconLIb`. This is a typo in the submodule, so I'm not able to incorporate a fix here.
3. Build the `IcoPngCombine\IcoPngCombine.csproj` normally with .NET Core SDK or Visual Studio.

# Usage
This tool understands PNG files organized in the `size\category\icon_name.png` file structure. And example of this structure can be found in [KDE's Oxygen Icons](https://github.com/KDE/oxygen-icons) repo.

Supply the path to the directory containing size folders as a first argument to the tool and it will create a `results` folder inside the supplied folder and put all icons, organized by categories, into it.

The tool was only tested on Windows and its operation on other OSes is not guaranteed since it uses some parts of Windows Forms library. Although, nothing graphical, so it may work.