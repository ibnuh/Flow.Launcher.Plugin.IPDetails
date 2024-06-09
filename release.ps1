# Publish the .NET project with specified configurations
dotnet publish Flow.Launcher.Plugin.IPDetails -c Release -r win-x64 --no-self-contained

# Create a zip archive of the published output
$publishPath = "Flow.Launcher.Plugin.IPDetails/bin/Release/win-x64/publish"
$zipPath = "Flow.Launcher.Plugin.IPDetails/bin/IPDetails.zip"
Compress-Archive -LiteralPath $publishPath -DestinationPath $zipPath -Force

# Stop the Flow Launcher process if it is running
if (Get-Process -Name Flow.Launcher -ErrorAction SilentlyContinue) {
    Stop-Process -Name Flow.Launcher -Force
}

# Define the destination path for the plugin
$pluginPath = Join-Path $env:APPDATA "FlowLauncher/Plugins/IPDetails-1.0.0"

# Copy the publish folder to the plugin folder for testing
Copy-Item -Path "$publishPath/*" -Destination $pluginPath -Recurse -Force

# Start the Flow Launcher process
$flowLauncherPath = Join-Path $env:LOCALAPPDATA "FlowLauncher/Flow.Launcher.exe"
Start-Process -FilePath $flowLauncherPath
