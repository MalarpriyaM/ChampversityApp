# Champversity Legacy Application

This is the legacy application for Champversity, an organization providing student masters consultation services and admission services.

## Description

The Champversity application provides the following functionality:

1. Students can download an Excel template for their application
2. Students fill out the Excel template and upload it back to the system
3. The system processes the uploaded Excel files and creates university-specific files
4. A batch process converts these files into XML and sends them to the respective universities
5. Universities respond with interview slot options for selected candidates
6. Students can select their preferred interview slots

## Application Structure

- **Champversity.Web**: Contains the web frontend, controllers, and views
- **Champversity.DataAccess**: Contains the data access layer, models, and services

## Technical Details

- ASP.NET Core 10.0 MVC application
- Entity Framework Core for data access
- SQLite fallback for local non-Windows development
- SQL Server support when a SQL Server connection string is configured
- NPOI library for Excel processing
- Background services for batch processing

## Setup Instructions

### macOS or Linux

1. Ensure the .NET 10 SDK is installed.
2. From the repository root, run:

	```bash
	chmod +x setup.sh
	./setup.sh
	```

3. Start the web application:

	```bash
	dotnet run --project Champversity.Web
	```

4. Open `http://localhost:5272`.

The application uses a local SQLite database at `Champversity.Web/App_Data/Champversity.db` when the configured connection string points to LocalDB on a non-Windows machine.

### Windows

1. Ensure the .NET 10 SDK is installed.
2. Run `setup.bat` from the repository root.
3. Start the web application:

	```powershell
	dotnet run --project Champversity.Web
	```

If SQL Server LocalDB is available, the application will continue using the connection string from `Champversity.Web/appsettings.json`.

## Default Admin Login

- Email: `admin@champversity.com`
- Password: `Admin@123`

The admin user and roles are created automatically on first startup.

## How to Use

1. Visit the home page.
2. Click on "Start Application" to begin the process.
3. Download the Excel template.
4. Fill out the template with your information.
5. Upload the completed template.
6. Use your application reference number to check status.
7. Select interview slots when they become available.

## Online Repository

This project is hosted on GitHub: https://github.com/MalarpriyaM/ChampversityApp

## Contact

For support, please contact support@champversity.com