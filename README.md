# Remote Proxy Toolbox (RPTB)

Remote Proxy Toolbox (RPTB) is a command-line application designed to make managing Caddy servers easier. Additionally, it provides the functionality to check your reverse proxy status from outside your network using an API hosted on a Vultr VPS. This API can be self-hosted on your own VPS or any other location of your choice. You will need to recompile the application with the IP address of your own API in the `CheckDomain.cs` file, specifically within the `private const string BaseUrl` variable.

## Overview

RPTB provides several modules responsible for different aspects of Caddy server management:

- `Config`: Manages Caddy server configurations, including adding, updating, deleting, and listing entries.
- `ProcUtilities`: Handles Caddy server process management tasks such as starting, stopping, and restarting.
- `Remote`: Provides utilities for checking reverse proxy status from outside your network.
- `Utilities`: Offers general-purpose utility functions to support Caddy server management tasks.

## Features

- **Menu-Driven Interface:** Utilizes a console-based menu system for intuitive interaction.
- **Process Monitoring:** Allows toggling of process monitoring to track the status of Caddy server processes.
- **Dynamic Logo:** Displays a dynamically centered logo for visual appeal and branding.
- **Domain Checking:** Enables users to asynchronously check the status of domains used in Caddy server configurations.
- **Caddy Update:** Supports downloading the latest updates for Caddy server tools and configurations.

## Usage

### Prerequisites

- .NET Core SDK installed
- Compatible Operating System (e.g., Windows)

### Getting Started

1. Clone the repository to your local machine.
2. Open the project in your preferred IDE or text editor.
3. Build the solution using the .NET Core SDK.
4. Run the `RPTB` executable to launch the application.

### Menu Options

1. Add Caddy Entry
2. Update Caddy Entry
3. Delete Caddy Entry
4. List All Caddy Entries
5. Validate Caddy Configuration
6. Start Caddy Process
7. Stop Caddy Process
8. Restart Caddy Process
9. Update Caddy Tools and Configurations
10. Check Domain Status
11. Toggle Process Monitoring
12. Exit

## Contributions

Contributions to Remote Proxy Toolbox are welcome! If you encounter any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).
