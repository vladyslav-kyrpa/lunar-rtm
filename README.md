# Lunar Real Time Messenger v0.1
----------
Lunar is a real-time messenger web application. With Lunar, you can create and customise your profile, start new chats and invite other users to join. You can also modify the chat's public information and picture, remove members or give them roles.

## Componenets
----------
### 1. Backend
The Backend is built with ASP.NET Core (.NET 8), providing an API that:
- Handles data
- Moderates user interactions with that data
- Handles real-time communications between users
- Buffers messages and ensures delivery

Technologies used:
- ASP.NET Core
- Entity Framework (EF)
- ASP.NET Identity for authentication and authorisation (Cookie-based)
- SignalR for real-time communication

Architecture:
- Layer 1: API that handles HTTP requests
- Layer 2: Business Logic and data model
- Layer 3: Database communication with (EF)

### 2. Frontend
The client application is built with ReactTS as a single-page application (SPA). It handles user UI interactions and makes HTTP requests to the backend.
Technologies used:
- ReactTS + Vite
- Tailwind
- Nginx proxy (Fome `Same-Origin` header and static files caching)
- SignalR for real-time communication

### 3. Database
All data is stored in a PostgreSQL (v17) database.

## Deployment
----------
All components are containerised and managed with Docker Compose. The provided `docker-compose.yaml` script automatically builds and runs all the required components.
To deploy locally, you need a couple of steps:
1. Configure the `.env` file
2. Install `docker` and `docker-compose` on the host machine
3. Run `docker compose up --build` in the terminal

Then the script will download all the necessary dependencies, create containers and start the app.

For distributed or production deployment, additional configuration is required.

## Future plans
-----------
Planned features:
- Google account authentication
- End-to-end encryption for private dialogues
- File sharing (images and other attachments)
