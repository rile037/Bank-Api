# Transaction API

This is a simple ASP.NET Core web API for managing transactions and user data.

## Overview

The Transaction API provides endpoints for making transactions, checking balances, retrieving transaction history, creating users, and user authentication. It is designed to handle basic financial transactions and user management functionalities.

## Features

- **Transaction Management**: Allows users to make transactions between accounts.
- **User Management**: Provides functionalities for creating users, checking balances, and retrieving user data.
- **Authentication**: Implements JWT-based authentication for secure user login.

## Technologies Used

- **ASP.NET Core**: Framework for building web APIs in C#.
- **Entity Framework Core**: ORM for database interaction.
- **JWT Authentication**: Token-based authentication mechanism.
- **BCrypt**: Hashing library for password security.

## Getting Started

To run the Transaction API locally, follow these steps:

1. Clone the repository to your local machine.
2. Configure the database connection string in the `appsettings.json` file.
3. Build and run the application using Visual Studio or the .NET CLI.
4. Access the API endpoints using a tool like Postman or your preferred HTTP client.

## API Endpoints

- **/makeTransaction**: POST endpoint for making transactions between accounts.
- **/checkBalance**: GET endpoint for retrieving the balance of a user.
- **/getTransactions**: GET endpoint for retrieving transaction history.
- **/createUser**: POST endpoint for creating a new user.
- **/login**: POST endpoint for user authentication.

## Contributors

- rile037 - Developer

## License

This project is licensed under the [MIT License].
