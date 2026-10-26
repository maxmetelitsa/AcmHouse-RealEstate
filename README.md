# AcmHouse — Real Estate Agency Desktop App

A WPF desktop application that automates the daily work of a real estate agency: renting out residential and commercial property, buying and selling property, generating contracts, tracking employee shifts and reporting revenue.

C# · .NET Framework 4.8 · WPF · MVVM · ADO.NET · SQLite · xUnit

> 🇷🇺 Русское описание — [ниже](#русская-версия).

## Features

**Employee**
- Browse residential and commercial offers with photos and search them by address or name.
- Rent out a property: pick an existing client or enter a new one, choose dates, see the price instantly, and get the rental contract generated as a Word (.docx) file.
- Buy and sell property: register sellers, create sale listings and generate sale contracts.
- Send a property offer to a client by email (HTML letter with the photo attached).
- Personal profile: open and close work shifts, see own contracts and earned profit.
- Calendar of events with pop-up reminders.

**Administrator**
- Manage residential and commercial listings (add, edit, delete, upload photos).
- Register employees and review each employee's shifts, contracts and profit.
- Track all rental contracts and open the generated documents.
- Revenue report for any date range: company total and per-employee profit.

## Architecture

The sign-in, catalog and rental windows follow **MVVM**: the window only binds to a view model and handles navigation; the logic lives in plain C# classes that are covered by unit tests.

```
View (XAML)  ──binds to──▶  ViewModel  ──uses──▶  Domain rules          (Domain/)
                                │                  RentalCalculator, ClientValidator
                                └──uses──▶  Repositories / services  (Data/, Services/)
                                                   SQL, sign-in, .docx contracts
```

| Layer | Folder | What is there |
| --- | --- | --- |
| View models | `ViewModels/` | `SignInViewModel`, `CatalogViewModel`, `RentAccommodationViewModel` |
| MVVM helpers | `Mvvm/` | `ViewModelBase` (INotifyPropertyChanged), `RelayCommand` |
| Business rules | `Domain/` | Rental period and price, agent profit, client data validation |
| Data access | `Data/` | Repositories with parameterized SQL; a rental is saved in one transaction |
| Services | `Services/` | Sign-in, contract generation, message boxes behind an interface |
| Security | `Security/` | PBKDF2 (HMAC-SHA256) password hashing with a random salt |

Other windows (admin panel, trade, profile, calendar) still use code-behind and are the next candidates for the same refactoring.

## Getting started

Requirements: Windows, Visual Studio 2022 with the **.NET desktop development** workload.

1. Clone the repository and open `AcmHouse.sln`.
2. Restore NuGet packages (right-click the solution → **Restore NuGet Packages**).
3. Press **F5**. The app opens on the sign-in window.

The SQLite database ships with the repository (`AcmHouse/Database/RentalHouse.db`), so no database server is needed.

### Demo accounts

All people in the database are fictional.

| Role | Email | Password |
| --- | --- | --- |
| Administrator | `admin@acmhouse.by` | `Admin1234` |
| Employee | `ivan.petrov@acmhouse.by` | `Demo1234` |
| Employee | `anna.kovaleva@acmhouse.by` | `Demo1234` |

Only password hashes are stored. To reset the demo data, run `AcmHouse/Database/seed-demo-data.sql` against the database (for example in DB Browser for SQLite).

An employee has to **open a shift** in the profile before signing contracts.

### Sending offers by email (optional)

SMTP credentials are not stored in the code. To enable "send offer to client", set these environment variables before starting the app:

```powershell
setx ACMHOUSE_SMTP_USER "your-address@mail.ru"
setx ACMHOUSE_SMTP_PASSWORD "your-app-password"
# optional, defaults to smtp.mail.ru
setx ACMHOUSE_SMTP_HOST "smtp.mail.ru"
```

## Tests

`AcmHouse.Tests` is an xUnit project with 58 tests for the business rules, password hashing and the three view models (repositories and dialogs are replaced with fakes). Run them from **Test → Run All Tests** (Test Explorer) in Visual Studio.

## Project structure

```
AcmHouse/
├── ViewModels/, Mvvm/       MVVM layer
├── Domain/                  Business rules
├── Data/                    Repositories (SQL)
├── Services/                Sign-in, contracts, paths, messages
├── Security/                Password hashing
├── Authorization/           Sign-in and employee registration windows
├── AccommodationOffers/     Offer catalog and offer details (incl. email to client)
├── Rent/                    Renting out property
├── Trade/                   Buying and selling property
├── Profile/                 Employee profile, shifts, calendar
├── Admin/                   Admin windows: listings, employees, contracts, events, reports
├── CompanyInfo/, Contracts/ .docx generation (Open XML SDK)
├── Database/                SQLite database and demo-data script
└── Images/                  Property photos
AcmHouse.Tests/              xUnit tests
```

## Known limitations

- The admin, trade, profile and calendar windows still keep their logic in code-behind.
- Repositories are tested indirectly (through fakes); there are no integration tests against SQLite yet.

---

## Русская версия

**AcmHouse** — десктопное приложение на WPF для автоматизации работы агентства недвижимости.

**Возможности сотрудника:** каталог жилой и коммерческой недвижимости с фото и поиском; оформление аренды с мгновенным расчётом стоимости и автоматической генерацией договора в Word (.docx); покупка и продажа недвижимости; отправка предложения клиенту на email; личный кабинет со сменами, договорами и прибылью; календарь событий с напоминаниями.

**Возможности администратора:** управление объявлениями и фотографиями; регистрация сотрудников и контроль их смен, договоров и прибыли; учёт договоров аренды; отчёт по выручке за период.

**Архитектура:** окна входа, каталога и оформления аренды построены по MVVM: логика вынесена в модели представлений, бизнес-правила и репозитории и покрыта 58 тестами на xUnit. Пароли хранятся только в виде хешей PBKDF2.

**Технологии:** C#, .NET Framework 4.8, WPF, MVVM, ADO.NET, SQLite, LINQ, Open XML SDK, xUnit.

**Запуск:** откройте `AcmHouse.sln` в Visual Studio 2022, восстановите NuGet-пакеты и нажмите F5.
Вход администратора: `admin@acmhouse.by` / `Admin1234`, сотрудника: `ivan.petrov@acmhouse.by` / `Demo1234`.
Для отправки писем задайте переменные окружения `ACMHOUSE_SMTP_USER` и `ACMHOUSE_SMTP_PASSWORD`.
