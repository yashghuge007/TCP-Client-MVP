# TCP Client MVP

A simple TCP client application built with C# to learn and understand the interaction of client with Server.  
This project demonstrates fundamental TCP networking and separation of concerns for maintainable and testable code.

## Features

- Connect to a TCP server with a specified IP address and port.
- Send and receive messages over a TCP connection.
- Clean, user-friendly interface.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or later (recommended) (Although I used VS Code :))

### Building the Project

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yashghuge007/TCP-Client-MVP.git
   cd TCP-Client-MVP
   ```
2. **Launcch the Server:**
   - Navigate to Server folder.
   - Launch Server by running node main.js in the PWD.

3. **Open the solution:**
   - Open `ABXClient.sln` in Visual Studio.

4. **Build and Run:**
   - Build the solution (`Ctrl+Shift+B`).
   - Set `ABXClient` as the startup project.
   - Run the application (`F5` or `Ctrl+F5`).

### Usage

1. Enter the server's IP address, port and the OutputPath location in appsettings.json.
2. Launch the application.
3. Received messages will be displayed in the message area.

## Project Structure

- `ABXClient.sln` — Visual Studio solution file.
- `ABXClient/` — Main client application (source code).

## Contributing

Contributions, issues and feature requests are welcome!  
Feel free to open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Author

- [yashghuge007](https://github.com/yashghuge007)
