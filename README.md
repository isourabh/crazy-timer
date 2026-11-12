# Crazy Timer

A minimal C# Windows Forms stopwatch for Windows 10 and 11 (x64).

- **Start** begins or resumes counting upward.
- **Stop** pauses without clearing the elapsed time.
- **Reset** stops and returns to `00:00:00.00`.

The display shows hours, minutes, seconds, and hundredths. Hours do not wrap
after 24. Closing the app discards the elapsed time. Use Tab to navigate the
buttons, Space to activate the focused button, or Alt+S / Alt+T / Alt+R.

## Build and run

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
on Windows, then run from the repository root:

```powershell
dotnet run --project src\CrazyTimer
dotnet test CrazyTimer.slnx -c Release
```

## Publish a standalone executable

```powershell
dotnet publish src\CrazyTimer\CrazyTimer.csproj -p:PublishProfile=Standalone
```

Distribute this file:

```text
src\CrazyTimer\bin\Release\net10.0-windows\win-x64\publish\CrazyTimer.exe
```

The executable bundles .NET, so users do not need to install .NET separately
or keep supporting files next to it. Bundled native libraries are extracted
to a per-user temporary location when launched; that location must be writable.
The bundled runtime makes the download larger than a framework-dependent app.

## Build the MSI installer

After publishing the executable, build the installer with the .NET SDK.
The pinned [WiX SDK](https://wixtoolset.org/) is restored automatically; review
its licensing terms before commercial use.

```powershell
dotnet build installer\CrazyTimer.Installer.wixproj -c Release -t:Rebuild
```

Output: `installer\bin\Release\CrazyTimer-1.0.0-x64.msi`.

The offline x64 MSI installs for the current user under
`%LOCALAPPDATA%\Programs\Crazy Timer`, adds a Start menu shortcut, and registers
an uninstall entry in Windows Settings. It does not need administrator rights.
There is no setup wizard; Windows Installer provides installation progress when
the package is opened interactively.

Silent installation and uninstallation:

```powershell
msiexec.exe /i "installer\bin\Release\CrazyTimer-1.0.0-x64.msi" /qn /norestart
msiexec.exe /x "installer\bin\Release\CrazyTimer-1.0.0-x64.msi" /qn /norestart
```

For another release, publish and package with the same version:

```powershell
dotnet publish src\CrazyTimer\CrazyTimer.csproj -p:PublishProfile=Standalone -p:Version=1.0.1
dotnet build installer\CrazyTimer.Installer.wixproj -c Release -t:Rebuild -p:AppVersion=1.0.1
```

Use an increasing three-part MSI version. Keep the existing `UpgradeCode` and
component GUID stable: a newer package replaces the previous version and older
versions are blocked. Close the app before upgrading or uninstalling.

WiX performs Windows Installer ICE validation during the build. If system policy
blocks it (warning `WIX1105`), the package can still be built, but rerun the build
on a machine that permits validation before release; do not suppress the warning
and treat it as a validated release.

## Microsoft Store: EXE/MSI route

The default build is **unsigned**, suitable for local testing, not Store
submission. Before submitting:

1. Register in Partner Center and reserve the product name. Confirm the publisher
   metadata before your first public release.
2. Obtain trusted code signing. Microsoft requires the installer and its PE
   payloads to be digitally signed with a certificate chaining to a CA in the
   Microsoft Trusted Root Program. A self-signed development certificate is not
   sufficient. Ensure signing covers applicable PE payloads before bundling and
   sign the final app executable before building the MSI.
3. Package the signed executable, then sign and timestamp the resulting MSI.
   The installer project consumes the existing executable without rebuilding it.
   You can select an externally prepared signed executable with
   `-p:AppExecutable="C:\releases\CrazyTimer.exe"`.
4. Verify signatures and test silent install, launch, upgrade, and uninstall on a
   clean Windows machine without .NET installed.
5. Host the signed MSI at a direct, version-specific HTTPS URL, for example
   `https://your-domain.example/releases/1.0.0/CrazyTimer-1.0.0-x64.msi`.
   Never replace the file at a submitted URL; use a new version and URL for updates.
6. Submit that URL in Partner Center with silent-install parameters, supported
   OS/architecture, listing text, icons/screenshots, age rating, and applicable
   privacy declarations. Microsoft reviews the submission before publication.

Store hosting and code signing are not provided for this route. Store listing
does not add an automatic updater to the app; this minimal app has none.
Keep the submitted installer URL available for users.

See [Microsoft's installer requirements](https://learn.microsoft.com/en-us/windows/apps/publish/publish-your-app/msi/app-package-requirements)
and [distribution options](https://learn.microsoft.com/en-us/windows/apps/distribute-through-store/how-to-distribute-your-win32-app-through-microsoft-store).
