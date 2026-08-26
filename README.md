# WPF CRUD Application

A simple desktop CRUD application developed in **C#/.NET with WPF**, primarily created to practice and demonstrate the **MVVM (Model-View-ViewModel) pattern**, data binding, and asynchronous operations.

## Features

- Create, read, update, and delete records
- WPF data binding between Views and ViewModels
- `ObservableCollection` for collection updates
- `INotifyPropertyChanged` for property change notifications
- Commands for View-ViewModel interaction
- Asynchronous CRUD operations using `async` / `await`
- Separation of UI, application logic, and data through MVVM

## Architecture

The application follows the **MVVM pattern** to separate responsibilities between:

- **Model** – Represents the application data
- **View** – Defines the WPF user interface
- **ViewModel** – Exposes data and commands to the View and coordinates the application logic

This separation reduces coupling between the UI and application logic and makes the code easier to maintain and extend.

## Async Operations

CRUD operations are implemented asynchronously using C# `async` / `await`.

This approach avoids blocking the UI thread while data operations are being executed, keeping the WPF interface responsive.

## Technologies

- C#
- .NET
- WPF
- MVVM
- XAML
- async / await

## Running the Application

A self-contained Windows build is included in the repository and can be executed without installing the .NET runtime.

Executable:

`MVVMApplication/MVVMApplication/builds/CrudWPFAppMVVMApplication.exe`

Alternatively, the project can be opened and run from Visual Studio.
