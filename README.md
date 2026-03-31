### Habit Hero:  The AI-Powered Gamified Habit Tracker

_Habit Hero is a mobile app designed for engaging and dynamic habit tracking_

**HabitHero API**

Backend API for HabitHero, a gamified habit-tracking application that helps users build consistency through daily habits, quests, and rewards.

This API powers core application features including user authentication, habit tracking, quest management, and AI-generated habit suggestions.


### 🚀 Overview

HabitHero is designed to make habit-building engaging and rewarding. Users can track daily habits, earn points, join quests with others, and receive AI-generated suggestions to improve their routines.

This repository contains the ASP.NET Core Web API responsible for handling all backend logic, database interactions, and integrations.


### 🛠️ Tech Stack

- **Backend:** ASP.NET Core Web API  
- **Database:** SQL Server  
- **ORM:** Entity Framework Core  
- **Language:** C#  
- **Architecture:** RESTful API  
- **Other:** LINQ, JSON-based API responses  

### 🔑 Features

**👤 User Management**
- User registration and authentication
- Stores user profile data and total points

**✅ Habit Tracking**
- Create and manage habits
- Generate AI habit ideas
- Track daily habit occurrences
- Mark habits as completed
- Automatically update user points
- Generate adaptive, goal-based habits using AI that evolve over time as users build consistency and progress (future)

**🎯 Quests System**
- Create and join group quests
- Invite other users to participate
- Accept/reject quest invitations
- Quest invitation notificatications
- Track shared progress across users

**🤖 AI Habit Suggestions**
- Generates personalized habit ideas using OpenAI
- Users can select and add suggested habits directly into their routine

**📊 Points System**
- Users earn points for completing habits
- Real-time updates returned from API endpoints

---

### 🧠 Architecture Notes
- Designed using a layered architecture separating controllers, business logic, and data access
- Entity Framework Core is used for database interaction and query abstraction
- RESTful principles are followed for clean and predictable API design
- Endpoints are optimized to return updated data (e.g., habits + points) in a single response to reduce frontend calls

---

### ⚙️ Getting Started

**Prerequisites**
- .NET SDK
- SQL Server
- Visual Studio or VS Code

**Setup**
   1. Clone repository 
   2. Download SQL database (file dbHabitHero.sql)
   3. Configure database connection in appsettings.json
   4. Run
---




# 🌱 _Built with the idea that small, consistent actions can turn into something big_
