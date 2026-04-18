# 💰 SpendSense – Personal Expense Tracker

SpendSense is a secure and user-friendly personal expense tracker web application built using ASP.NET Core MVC. It helps users efficiently manage their income and expenses, gain insights through interactive charts and generate detailed financial reports.

---

## 🚀 Features

### 🔐 Authentication & Security
- Secure user registration and login using ASP.NET Core Identity
- Claims-based authentication & authorization
- Profile photo upload support
- Complete user data isolation (each user accesses only their own data)

---

### 📊 Expense Management
- Add, edit, and delete income & expense transactions
- Custom categories with emoji icons
- Organized transaction tracking with date filters
- Indian Rupee (₹) currency formatting

---

### 📈 Data Visualization
- Interactive Doughnut Charts for category-wise distribution
- Spline Charts for expense trends over time
- Real-time financial insights dashboard

---

### 📄 Reporting & Export
- Export transactions as styled PDF reports
- Filter reports by custom date ranges
- Clean and readable financial summaries

---

## 🛠️ Tech Stack

- Backend: ASP.NET Core MVC, C#  
- Frontend: Razor Views, HTML, CSS, JavaScript  
- Database: SQL Server  
- ORM: Entity Framework Core (Code First + Migrations)  
- Authentication: ASP.NET Core Identity  
- Charts: Syncfusion  
- PDF Generation: iText7  

---

## 🧠 Key Concepts Implemented

- Claims-based authentication & role-based authorization  
- Secure user data handling and isolation  
- MVC architecture (Model-View-Controller)  
- Entity Framework Core migrations  
- Third-party library integration (Syncfusion, iText7)  
- Clean and maintainable code structure  

---

## 📁 Project Structure


SpendSense/
│
├── Controllers/ # Handles request logic
├── Models/ # Data models & entities
├── Views/ # Razor UI views
├── wwwroot/ # Static files (CSS, JS, images)
├── Data/ # Database context & migrations
├── Services/ # Business logic
├── appsettings.json # Configuration
└── Program.cs # Entry point


---

## ⚙️ Setup & Installation

### 🔹 Prerequisites
- .NET SDK (6.0 or above)
- SQL Server
- Visual Studio / VS Code

---

## 🔐 Configuration

- Update database connection string in `appsettings.json`
- Ensure SQL Server is running

---

## 📊 Use Cases

- Personal finance management  
- Budget tracking  
- Expense analysis  
- Financial reporting  

---

## 🚀 Future Improvements

- Mobile responsiveness enhancement  
- Bank API integration  
- Multi-currency support  
- Dark mode UI  
- Advanced analytics  

---

## 📜 License

This project is licensed under the MIT License.

---