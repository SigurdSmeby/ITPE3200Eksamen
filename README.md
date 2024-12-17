# ITPE3200Eksamen

**Why we created this project:**  
This project was developed as a final examination assignment in the ITPE3100 Web Applications course at OsloMet. Our goal was to create two full-stack web applications that demonstrate our understanding of modern web technologies, including frontend frameworks, backend APIs, and database management. The first sub-application uses .NET MVC as a backend and .cshtml Pages in the frontend, to focus on backend functionality, server-side validation. In contrast, the second sub-application, built with React, emphasizes client-side validation, dynamic content rendering, and intuitive filtering mechanisms.

---

## Sub Application 1

**Steps to run:**

1. **Install .NET SDK (if not already installed):**  
   You can download the .NET SDK from the official [.NET website](https://dotnet.microsoft.com/) or install it via terminal:  
   `winget install Microsoft.DotNet.SDK.8.`

2. **Verify SDK installation:**  
   `dotnet --version`

3. **Navigate to the Sub-Application-1 project directory:**  
   Use your terminal or preferred editor.

4. **Restore required NuGet packages:**  
   `dotnet restore`

5. **Database configuration (optional):**  
   By default, the database is deleted and recreated every time the server runs for testing purposes. To test persistent data, comment out line 56 in `Program.cs`:  
   `// context.Database.EnsureDeleted();`

6. **Run the application:**  
   `dotnet run`  
   Or use your editor’s run/debug options.

7. **Access the application:**  
   Open [http://localhost:5259](http://localhost:5259) (or the URL provided in the terminal).

**Why restore NuGet packages?**  
This ensures all external dependencies (such as libraries for database access and authentication) are properly downloaded and configured.

---

## Sub Application 2

### Server (Backend)

1. **Install .NET SDK (if not already installed):**  
   `winget install Microsoft.DotNet.SDK.8.`

2. **Verify SDK installation:**  
   `dotnet --version`

3. **Navigate to the server folder:**  
   `cd path/to/Sub-Application-2/server`

4. **Restore required NuGet packages:**  
   `dotnet restore`

5. **Database configuration (optional):**  
   By default, the database is deleted and recreated every time the server runs. To test persistence, comment out line 77 in `server/Program.cs`:  
   `// context.Database.EnsureDeleted();`

6. **Run the server:**  
   `dotnet run`  
   Or use your editor’s integrated run tools.

**Why restore NuGet packages?**  
This ensures all external dependencies are properly set up.

---

### Client (React Frontend)

1. **Install Node.js (if not already installed):**  
   [Download Node.js](https://nodejs.org/en/download/package-manager/current) and follow the installation instructions.

2. **Navigate to the client folder:**  
   `cd path/to/Sub-Application-2/client`

3. **Install Node.js dependencies:**  
   `npm i`

4. **Start the React application:**  
   `npm start`

5. **Access the application:**  
   The React app should open automatically in your default browser. If not, navigate to the provided URL in your terminal.

**Why install Node.js dependencies?**  
This ensures all necessary libraries and frameworks for the frontend (like React) are properly installed and ready to use.
