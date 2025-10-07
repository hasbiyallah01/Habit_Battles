# Habit Battles

**Habit Battles** ⚔️, a gamified habit tracker where two friends compete by completing daily habits. Each habit completed is a “strike” against a shared monster. Missing a day allows the monster to heal or strike back. First person to slack loses. Players earn coins and badges they can use to unlock avatars, themes, and new monsters.

Players earn coins and badges that can be used to unlock avatars, themes, and new monsters, making habit-building engaging, consistent, and rewarding.

---

## Features

### Core Functionality

* **User Authentication** with JWT-based security
* **Habit and Battle Management** — create, join, and track habit battles
* **Strike System** — register daily habit completions (“strikes”)
* **Health and Streak Tracking** for each user
* **Coins and Rewards** for consistent habit completion

### Game Logic

* **Real-time updates** using SignalR
* **Automatic health deduction** and strike cooldown
* **Battle outcome determination** based on performance

### Admin Tools

* Manage users and battles
* Reset or configure battle rules (optional)

---

## Tech Stack

* **Language:** C#
* **Framework:** .NET 8 Web API
* **Database:** PostgreSQL
* **Real-time Communication:** SignalR / WebSockets
* **Authentication:** JWT-based authentication
* **Hosting (Recommended):** Render / Azure App Service

---

## Getting Started

### Prerequisites

* .NET 8 SDK
* PostgreSQL access

### Environment Variables (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "*****************************"
  },
  "JwtSettings": {
    "Issuer": "*****************************",
    "Audience": "*****************************",
    "SecretKey": "*****************************"
  }
}
```

### Installation

1. **Clone the repository**

   ```bash
   git clone https:/hasbiyallah01/github.com//habit-battles-backend.git
   ```

2. **Navigate to the project folder**

   ```bash
   cd habit-battles-backend
   ```

3. **Restore dependencies**

   ```bash
   dotnet restore
   ```

4. **Run database migrations** (if applicable)

   ```bash
   dotnet ef database update
   ```

5. **Start the application**

   ```bash
   dotnet run
   ```

---
## Instructions

1. **Signup / Login**
2. Go to the **Dashboard**
3. Click **Create Habit**
4. Select or type your habit, choose a duration, and enter your opponent’s email
5. Use this opponent email for testing:

   ```
   user@example.com
   ```
6. Once created, you’ll be redirected to the battle page — this is where you **strike** after completing your habit for the day.

   * You can **strike only once per day**.
   * Each successful strike gives you **streaks and coins**.
   * Your opponent **loses health** when you strike.
---

## Frontend Integration

The backend serves as the API layer for the **Habit Battles Frontend**:
👉 [Habit Battles Frontend Repository](https://github.com/your-username/habit-battles-frontend)
