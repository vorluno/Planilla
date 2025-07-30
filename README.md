# 🧾 Planilla - Payroll Management System

A payroll management system built for enterprise needs. This project handles dynamic income and deduction rules, payroll periods, and payslip generation with role-based access. Built using clean architecture principles.

---

## ✨ Features

- 🔐 Role-based authentication (ASP.NET Core Identity)
- 📆 Dynamic payroll periods
- 💵 Custom deductions (loans, bonuses, overtime, taxes)
- 🧾 Payslip generation and preview
- 🧱 Clean Architecture: Domain, Application, Infrastructure, Web
- 🔄 Hybrid UI: Blazor Server + React via JSInterop
- ☁️ Azure-ready deployment (SQL Database, Key Vault)

---

## 🛠 Tech Stack

![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)
![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=flat-square&logo=blazor&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=flat-square&logo=react&logoColor=61DAFB)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)

---

## 🚧 Project Status

🛠️ In development — Internal testing and feature expansion ongoing.

---

## 🚀 Getting Started

```bash
# Clone the repository
git clone https://github.com/swlarot/Planilla.git

# Open the solution in Visual Studio
Planilla.sln

# Configure your database connection in appsettings.json
# Then apply EF migrations:
Update-Database
