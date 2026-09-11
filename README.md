# V Lens — Remote Virtual Machine Manager

A WPF desktop application for managing Windows virtual machines hosted on Hyper-V servers. V Lens lets you view machine details, monitor logged-in users, sign off remote sessions, and control VM power state — all from a single dashboard.

## Features

- **Machine Dashboard** — View status, OS info, CPU, RAM, disk space, host name, and more for all configured VMs
- **Logged-In Users** — See who is signed in to each remote machine with session details (state, idle time, logon time)
- **Sign Off Users** — Remotely sign off a user session from any machine
- **VM Power Control** — Shut down, restart, or turn on virtual machines via right-click context menu
- **Owner Filtering** — Filter machines by owner; auto-selects the current logged-in user's machines on load
- **Export to CSV** — Export the machines grid to a CSV file
- **Registry Host Lookup** — Reads the Hyper-V physical host name directly from VM guest registry

## Requirements

- Windows with .NET Framework 4.8
- Domain-joined machine (for user display name lookup)
- Network access to the remote VMs (WMI, SMB)
- Appropriate permissions on target machines (admin or remote management rights)

## Getting Started

### 1. Configure the Machine List

Create a JSON file (e.g. `RemoteMachines.json`) in the application directory with the following format:

```json
[
  {
    "Owner": "LASTNAME Firstname",
    "MachineNames": [
      {
        "NodeName": "VM-NODE-NAME",
        "HostName": "HYPERV-HOST.domain.com",
        "Purpose": "Description of this VM"
      },
      {
        "NodeName": "ANOTHER-VM",
        "HostName": "HYPERV-HOST.domain.com",
        "Purpose": "Another VM description"
      }
    ]
  },
  {
    "Owner": "ANOTHER Owner Name",
    "MachineNames": [
      {
        "NodeName": "SOME-VM",
        "HostName": "ANOTHER-HOST.domain.com",
        "Purpose": "Purpose of this VM"
      }
    ]
  }
]
```

| Field | Description |
|---|---|
| `Owner` | Display name of the person who owns the VMs |
| `NodeName` | The VM's computer name (hostname on the network) |
| `HostName` | The Hyper-V host server where the VM is hosted |
| `Purpose` | Free-text description of the VM's purpose |

### 2. Launch the Application

Run `SignedInUsers.exe`. The application window will open centered on screen.

### 3. Load Machines

1. Select a JSON file from the **Path** dropdown (auto-populated from `*.json` files in the app directory), or click **Browse** to choose one
2. Click **Load** to load the machine list
3. Optionally filter by **Owner** using the dropdown — it auto-selects your name if found

### 4. Scan Machines

- **Go** — Scans the selected machine in the left panel
- **Go All** — Scans all loaded machines sequentially

The scan collects the following information from each machine via WMI:

| Data | Source |
|---|---|
| Power status | ICMP Ping |
| Logged-in users | WMI `Win32_Process` (explorer.exe) + `quser` |
| OS info (caption, version, architecture) | WMI `Win32_OperatingSystem` |
| Hardware (CPU, RAM) | WMI `Win32_ComputerSystem` |
| Disk info (size, free space) | WMI `Win32_LogicalDisk` |
| Hyper-V host name | Remote Registry (`Virtual Machine\Guest\Parameters`) |

Progress and elapsed time are shown in the status bar at the bottom.

## Using the Application

### Machines Tab

Displays a grid of all scanned VMs with columns for status, host, owner, processors, RAM, disk space, OS details, last boot time, and more.

**Right-click context menu:**
| Action | Description |
|---|---|
| **Shut down** | Shuts down the selected VM via WMI |
| **Restart** | Restarts the selected VM via WMI |
| **Turn on** | Starts the VM on the Hyper-V host (requires host access) |

### Users Tab

Displays all logged-in users across scanned machines with columns for machine name, user name, session state, idle time, last login, session name, and session ID. Hover over a user name to see their display name.

**Right-click context menu:**
| Action | Description |
|---|---|
| **Sign Off** | Signs off the selected user's session from the remote machine. Prompts for confirmation before executing. |

### Machine List (Left Panel)

Lists all loaded machine names. Select a machine and click **Go** to scan just that one.

**Right-click context menu:**
| Action | Description |
|---|---|
| **Remove** | Removes the selected machine from the list |
| **Remove All** | Clears all machines from the list |

### Export

Click **Export CSV** to export the machines grid to a timestamped CSV file in the `ExportData` folder within the application directory.

## Troubleshooting

| Issue | Solution |
|---|---|
| Machine shows "Powered Off" | The machine may be off, or a firewall is blocking ICMP ping |
| WMI queries fail | Ensure the Windows Firewall allows WMI traffic and the Remote Management rules are enabled on the target machine |
| "The RPC server is unavailable" | Enable WMI through the firewall on the remote machine, or set `HKLM\SYSTEM\CurrentControlSet\Control\Terminal Server\AllowRemoteRPC` to `1` |
| Registry host name is empty | The Remote Registry service must be running on the target VM |
| User display name not resolved | Requires domain connectivity to look up names via Active Directory |
| Turn On VM fails with "VM not found" | Verify the `NodeName` in the JSON matches the VM display name on the Hyper-V host, and that the current user has Hyper-V admin rights on the host |

## Project Structure

```
njanapal/
├── njanapal/                    # Shared library project
│   ├── MachineHandler.cs        # WMI queries, registry, disk, OS info
│   ├── RemoteMachine.cs         # Logged-in users, sign off, reboot, shutdown, turn on VM
│   ├── HelperCls/
│   │   ├── Machine.cs           # Machine data model
│   │   ├── User.cs              # User session data model
│   │   └── ...                  # Other helper models
│   ├── UserMachines.cs          # JSON deserialization models (Node, UserMachines)
│   └── RemoteMachines.json      # Sample machine configuration
│
├── SignedInUsers/                # WPF application project (V Lens)
│   ├── MainWindow.xaml          # Main UI layout
│   ├── MainWindowViewModel.cs   # ViewModel with all commands and logic
│   ├── ViewModelBase.cs         # INotifyPropertyChanged base class
│   ├── RelayCommand.cs          # ICommand implementation
│   └── Styles/                  # UI styles and resource dictionaries
│
└── README.md
```

## Build

Open the solution in Visual Studio and build. Targets **.NET Framework 4.8**.

```
msbuild njanapal.sln /p:Configuration=Release
```

## License

Copyright © 2026. All rights reserved.
