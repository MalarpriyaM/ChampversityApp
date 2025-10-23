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

## Setup Instructions

1. Run the `setup.bat` file to set up the application
2. Open the solution in Visual Studio
3. Build and run the application

## Application Structure

- **Champversity.Web**: Contains the web frontend, controllers, and views
- **Champversity.DataAccess**: Contains the data access layer, models, and services

## Implementation Details

### Legacy System Features

- Excel template download and upload functionality
- Batch processing for validation and university file creation
- XML generation and processing
- Interview slot management

### Technical Details

- ASP.NET Core 9.0 application with Razor Pages
- Entity Framework Core for data access
- NPOI library for Excel processing
- Background services for batch processing
- XML processing for university communication