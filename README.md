# Habit Battles

A gamified habit tracker that transforms personal growth into friendly competition. Two friends compete by completing daily habits, with each completed task counting as a “strike” against a shared monster. Missing a day allows the monster to heal or retaliate, introducing a fun and challenging sense of accountability.

Players earn coins and badges that can be used to unlock avatars, themes, and new monsters, making habit-building engaging, consistent, and rewarding.

---

## Features

### 1v1 Habit Battles

Compete with friends to stay consistent with daily habits.

### Dynamic Monsters

Each battle features a shared monster that reacts to your progress — healing or striking back depending on performance.

### Rewards System

Earn coins and badges for streaks and victories.

### Customization

Unlock new avatars, battle themes, and monster types as you progress.

### Progress Tracking

View daily, weekly, and monthly consistency reports to monitor growth and performance.

---

## Tech Stack

### Frontend

* Next.js 14 with TypeScript
* Tailwind CSS for UI styling
* SignalR or WebSockets for live battle updates

### Backend

* C# .NET 8 Web API
* PostgreSQL database
* JWT-based or OAuth authentication

---

## Getting Started

### Prerequisites

* Node.js 18+
* .NET 8 SDK
* PostgreSQL

---

### Environment Variables

#### Backend (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "*****************************"
  },
  "JwtSettings": {
    "SecretKey": "***************************",
    "Issuer": "***************************",
    "Audience": "***************************"
  }
}
```

---

## Installation

### Clone the repository

```bash
git clone https://github.com/yourusername/habit-battles.git
```

### Install frontend dependencies

```bash
cd frontend
npm install
```

### Install backend dependencies

```bash
cd backend
dotnet restore
```

### Run the applications

```bash
# Frontend
cd frontend && npm run dev

# Backend
cd backend && dotnet run
```

---

## Project Structure

```
├── frontend/          # Next.js frontend application
├── backend/           # .NET 8 Web API
└── README.md
```

---

## Status

This project is still in active development — expect updates and potential feature changes as it evolves.
