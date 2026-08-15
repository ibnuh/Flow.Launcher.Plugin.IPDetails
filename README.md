# Flow.Launcher.Plugin.IPDetails

A plugin for the [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher).

Retrieve current public IP details/lookup or by providing an IP address.

IP requests are cached for 24 hours to disk inside the Flow Launcher plugin directory to prevent unnecessary requests to the API and to speed up the response time.

![Demo](screenshots/demo.gif)

## Usage

| Command             | Remarks                                          |
| ------------------- | ------------------------------------------------ |
| `ip`                | Get the Public IP address of the current machine |
| `ip <ipv4-address>` | Get the Public IP address of the specified IPv4  |

## Development

For development, you can use the following command to build the plugin and copy it to the Flow Launcher plugin
directory. This script will allow you to test the plugin by doing these steps:

1. Build the plugin
2. Stop the Flow Launcher process
3. Copy the plugin to the plugin directory
4. Start the Flow Launcher process

```powershell
.\release.ps1
```

## Integrations

1. [api.ipquery.io](https://ipquery.io)

## Icons

IP Icons created by [Design Cirlce - Flaticon](https://www.flaticon.com/free-icons/ip)
