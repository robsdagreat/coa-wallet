# COA Wallet - Backend

COA Wallet is a personal finance management API built with .NET, PostgreSQL, and JWT authentication. This project allows users to manage their financial transactions, categories, and user authentication with a secure API.

## Features
- **Deployed Link**: https://coa-wallet.onrender.com/index.html

## Features
- **User Authentication**: JWT-based authentication for secure user management.
- **Categories**: Categorize your transactions with a flexible category structure, including parent-child relationships.
- **Transactions**: Add, edit, and delete transactions while keeping them linked to user accounts and categories.
- **Real-time Notifications**: Push notifications using SignalR for real-time updates.
- **Swagger UI**: API documentation provided through Swagger UI for easy testing.

## Tech Stack
- **.NET 7.0/8.0**: The application is built with .NET for creating robust APIs.
- **PostgreSQL**: The database used for storing user and transaction data.
- **Entity Framework Core**: ORM for interacting with the PostgreSQL database.
- **JWT Authentication**: JSON Web Tokens for user authentication and authorization.
- **SignalR**: Real-time communication for notifications.
- **Swagger**: API documentation and interactive interface for testing endpoints.

## Getting Started

Follow these steps to run the backend locally or in a production environment.

### Prerequisites

- [.NET 7.0 or higher](https://dotnet.microsoft.com/download/dotnet)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Docker (optional)](https://www.docker.com/get-started)
- A [Render](https://render.com) account (if deploying to Render)

### Setup Locally

1. **Clone the repository**:
   ```bash
   git clone https://github.com/robsdagreat/coa-wallet.git
   cd coa-wallet-backend
