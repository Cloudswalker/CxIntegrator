# CRM → CXone Integration Console App

## Overview

This is a simple C# console application that fetches tickets from a mock CRM system 
(simulating Zendesk/Freshdesk APIs) and sends them as JSON to a simulated CXone API endpoint.

---

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download) or later installed
- Internet connection (if using real external APIs)
- Basic knowledge of .NET CLI commands

---

## Getting Started

1. **Clone the repository:**

```bash
git clone https://github.com/Cloudswalker/CxIntegrator
cd CxIntegrator
```

2. **Restore dependencies:**

```bash
dotnet restore
```

3. **Configure API URLs:**

Create or update the `appsettings.json` file in the project root with your API endpoints:

```json
{
  "CrmApi": "https://jsonplaceholder.typicode.com/posts",
  "CxoneApi": "https://httpbin.org/post",
  "LogFilePath": "log.txt"
}
```

Note: You can replace the URLs with real endpoints or local mocks.

4. **Run the application:**

```bash
dotnet run
```

The console will log progress, including:
Fetching tickets from CRM
Sending tickets to CXone API
Success or error messages and codes

---

## Project Structure

- Models/ — Data models for CRM and CXone tickets and PriorityLevel enum
- Services/Interfaces/ - Interfaces for CRM and CXone API calls, plus Mapping
- Services/ —  Implementations for CRM and CXone API calls, plus mapping logic
- Program.cs — Application entry point, DI container setup, and workflow orchestration
- appsettings.json — Configuration file for external API URLs

## Features

- Fetch tickets from a RESTful CRM endpoint (mocked with external service)
- Map CRM ticket JSON to CXone ticket format
- Send transformed tickets via POST request to CXone endpoint (mocked with external service)
- Logging of operations and error handling
- Dependency Injection for clean architecture and testability

## Contact

- For questions or feedback, contact: kit_bohdan@hotmail.com