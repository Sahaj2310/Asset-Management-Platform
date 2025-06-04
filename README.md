# Asset Management Platform

A comprehensive asset management solution built with .NET Core and modern web technologies.

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

2. Backend Setup
```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

3. Frontend Setup (Coming Soon)
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