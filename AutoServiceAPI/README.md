# Auto Service Management API

A comprehensive REST API built with ASP.NET Core for managing auto service operations, including user authentication, client management, car records, service tracking, and billing.

## Features

- **JWT Authentication** with role-based access control
- **Organisation Management** with owner and employee roles
- **Client Management** with full CRUD operations
- **Car Management** with detailed vehicle information
- **Service Management** for offered services
- **Bill Generation** with automatic calculations and discounts
- **Income Reporting** for business analytics
- **Advanced Search** capabilities across all entities

## Technology Stack

- ASP.NET Core 8.0
- Entity Framework Core with SQL Server
- JWT Bearer Authentication
- BCrypt for password hashing
- Swagger/OpenAPI documentation

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB is used by default)

### Installation

1. Clone the repository
2. Navigate to the project directory:
   ```bash
   cd AutoServiceAPI
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Update the connection string in `appsettings.json` if needed

5. Run the application:
   ```bash
   dotnet run
   ```

6. The API will be available at:
   - HTTPS: `https://localhost:7063`
   - HTTP: `http://localhost:5063`
   - Swagger UI: `https://localhost:7063/swagger`

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/v1/auth/register` | Register first organization owner | No |
| POST | `/api/v1/auth/login` | User login | No |

### Organization & Employees

| Method | Endpoint | Description | Auth Required | Role Required |
|--------|----------|-------------|---------------|---------------|
| GET | `/api/v1/organisation` | Get organization details | Yes | Owner/Employee |
| PUT | `/api/v1/organisation` | Update organization | Yes | Owner |
| POST | `/api/v1/employees` | Create employee | Yes | Owner |
| GET | `/api/v1/employees` | List employees | Yes | Owner |
| DELETE | `/api/v1/employees/{id}` | Delete employee | Yes | Owner |

### Clients

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/v1/clients` | Create client | Yes |
| GET | `/api/v1/clients` | List/search clients | Yes |
| GET | `/api/v1/clients/{id}` | Get client details | Yes |
| PUT | `/api/v1/clients/{id}` | Update client | Yes |
| DELETE | `/api/v1/clients/{id}` | Delete client | Yes |

### Cars

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/v1/cars` | Create car | Yes |
| GET | `/api/v1/cars` | List/search cars | Yes |
| GET | `/api/v1/cars/{id}` | Get car details | Yes |
| PUT | `/api/v1/cars/{id}` | Update car | Yes |
| DELETE | `/api/v1/cars/{id}` | Delete car | Yes |

### Services

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/v1/services` | Create service | Yes |
| GET | `/api/v1/services` | List services | Yes |
| PUT | `/api/v1/services/{id}` | Update service | Yes |
| DELETE | `/api/v1/services/{id}` | Delete service | Yes |

### Bills

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/v1/bills` | Create bill | Yes |
| GET | `/api/v1/bills` | List/search bills | Yes |
| GET | `/api/v1/bills/{id}` | Get bill details | Yes |
| PUT | `/api/v1/bills/{id}` | Update bill | Yes |
| DELETE | `/api/v1/bills/{id}` | Delete bill | Yes |

### Reports

| Method | Endpoint | Description | Auth Required | Role Required |
|--------|----------|-------------|---------------|---------------|
| GET | `/api/v1/reports/income` | Get income report | Yes | Owner |

## Search Parameters

### Clients
- `name`: Search by first name, last name, or full name
- `phoneNumber`: Search by phone number

### Cars
- `vin`: Search by VIN
- `licensePlate`: Search by license plate

### Bills
- `clientName`: Search by client name
- `licensePlate`: Search by car license plate
- `carType`: Search by car make/model
- `insuranceNumber`: Search by insurance number
- `dateStart`: Filter by start date
- `dateEnd`: Filter by end date

## User Flow: Creating a Bill

1. **Check for Car**: Search for the car by VIN
   ```http
   GET /api/v1/cars?vin=WVWZZZAUZMP123456
   ```

2. **Create Client** (if needed): If client doesn't exist
   ```http
   POST /api/v1/clients
   {
     "firstName": "John",
     "lastName": "Doe",
     "phoneNumber": "+1234567890"
   }
   ```

3. **Create Car** (if needed): If car doesn't exist
   ```http
   POST /api/v1/cars
   {
     "make": "Toyota",
     "model": "Camry",
     "year": 2020,
     "vin": "WVWZZZAUZMP123456",
     "clientId": "client-uuid"
   }
   ```

4. **Create Bill**: Generate the bill
   ```http
   POST /api/v1/bills
   {
     "clientId": "client-uuid",
     "carId": "car-uuid",
     "servicesPerformed": [
       {
         "serviceId": "service-uuid",
         "name": "Oil Change",
         "price": 50.00
       }
     ],
     "discount": {
       "type": "percentage",
       "value": 10
     }
   }
   ```

## Data Models

### Organisation
```json
{
  "id": "org_uuid",
  "name": "Super Auto Service",
  "location": "123 Main St, Anytown",
  "subscriptionStartDate": "2025-01-01T00:00:00Z",
  "subscriptionDueDate": "2026-01-01T00:00:00Z",
  "createdAt": "2025-01-01T00:00:00Z"
}
```

### User (Employee/Owner)
```json
{
  "id": "user_uuid",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "role": "Organisation Owner",
  "organisationId": "org_uuid"
}
```

### Client
```json
{
  "id": "client_uuid",
  "firstName": "Jane",
  "lastName": "Smith",
  "phoneNumber": "+15551234567"
}
```

### Car
```json
{
  "id": "car_uuid",
  "make": "Volkswagen",
  "model": "Golf",
  "year": 2021,
  "color": "Blue",
  "vin": "WVWZZZAUZMP123456",
  "licensePlate": "AB 123 CD",
  "insuranceNumber": "INS-987654",
  "mileage": 50000,
  "engineType": "2.0L TDI",
  "transmission": "Automatic",
  "numberOfDoors": 5,
  "clientId": "client_uuid"
}
```

### Service
```json
{
  "id": "service_uuid",
  "name": "Oil Change"
}
```

### Bill
```json
{
  "id": "bill_uuid",
  "billNumber": "2025-0001",
  "clientId": "client_uuid",
  "carId": "car_uuid",
  "date": "2025-07-30T17:00:00Z",
  "servicesPerformed": [
    {
      "serviceId": "service_uuid_1",
      "name": "Oil Change",
      "price": 100.00
    }
  ],
  "subtotal": 100.00,
  "discount": {
    "type": "percentage",
    "value": 10
  },
  "totalAmount": 90.00,
  "notes": "Customer requested synthetic oil.",
  "createdBy": "user_uuid"
}
```

## Authentication

The API uses JWT Bearer tokens for authentication. Include the token in the Authorization header:

```
Authorization: Bearer your_jwt_token_here
```

### Roles
- **Organisation Owner**: Full access to all endpoints
- **Employee**: Access to clients, cars, services, and bills (cannot manage employees)

## Error Handling

The API returns appropriate HTTP status codes:
- `200 OK`: Successful GET/PUT requests
- `201 Created`: Successful POST requests
- `204 No Content`: Successful DELETE requests
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Configuration

Key configuration settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AutoServiceDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHereThatShouldBeAtLeast32CharactersLong",
    "Issuer": "AutoServiceAPI",
    "Audience": "AutoServiceAPI",
    "ExpiryInMinutes": 60
  }
}
```

## Testing

Use the included `test-api.http` file to test the API endpoints with a REST client extension in VS Code or similar tools.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.