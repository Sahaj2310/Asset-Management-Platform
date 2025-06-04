# Asset Management Platform

A modern web-based solution for managing company assets, users, and inventory with secure authentication and real-time tracking.

## Features

- 🔐 Secure Authentication & Authorization
- 👥 User Management
- 🏢 Company Management
- 📊 Asset Tracking
- 📱 Responsive Design
- 🔄 Real-time Updates

## Tech Stack

### Backend
- .NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- AutoMapper
- Swagger/OpenAPI

### Frontend (Coming Soon)
- React/Angular
- TypeScript
- Material-UI/Tailwind CSS
- Redux/Context API

## Getting Started

### Prerequisites
- .NET Core 8.0 SDK
- SQL Server
- Node.js (for frontend)
- Git

### Installation

1. Clone the repository
```bash
git clone https://github.com/Sahaj2310/Asset-Management-Platform.git
cd Asset-Management-Platform
```

2. Configuration Setup
```bash
# Copy the template configuration file
cp appsettings.template.json backend/appsettings.json

# Edit appsettings.json with your settings:
# - Update database connection string
# - Set JWT secret key and URLs
# - Configure email settings
```

3. Backend Setup
```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

4. Frontend Setup (Coming Soon)
```bash
cd frontend
npm install
npm start
```

## API Documentation

The API documentation is available at `/swagger` when running the backend server.

### Key Endpoints

- Authentication
  - POST `/api/auth/register` - Register new user
  - POST `/api/auth/login` - User login
  - GET `/api/auth/confirm-email` - Confirm email address

- Profile
  - GET `/api/profile` - Get user profile
  - PUT `/api/profile` - Update user profile

- Company
  - GET `/api/company` - Get company details
  - POST `/api/company` - Create new company
  - PUT `/api/company` - Update company details

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

Sahaj Modi - [@Sahaj2310](https://github.com/Sahaj2310)

Project Link: [https://github.com/Sahaj2310/Asset-Management-Platform](https://github.com/Sahaj2310/Asset-Management-Platform)

## Recent Changes

### Site-Company Relationship
- Each Site is now linked to a Company via a `CompanyId` (Guid) foreign key.
- The API returns the company name with each site (see `SiteReadDto`).

### Use of Guid Keys
- All primary keys for Sites (and Companies) are now `Guid` for better uniqueness and security.

### DTOs and Repository Pattern
- All API endpoints use DTOs for input/output.
- Data access is handled via repositories for clean separation of concerns.

### Large Data Files and .gitignore
- `wwwroot/data/` and `wwwroot/uploads/` are excluded from version control via `.gitignore`.
- **Do not commit large or user-uploaded files to GitHub.**
- To set up location data, follow the instructions below.

## Data Setup for Location APIs

1. Download the following files from the [countries-states-cities-database](https://github.com/dr5hn/countries-states-cities-database/tree/master/json):
   - countries.json
   - states.json
   - cities.json
2. Place these files in the `wwwroot/data/` directory of this project. (This folder is ignored by Git.) 