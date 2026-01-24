Inventory Management System – README

Project Overview:  
This is a simple and functional Inventory Management System developed using:
* Visual Studio 2019
* .NET Framework (Windows Forms)
* Microsoft SQL Server Express LocalDB (Version 13.0.7037)

***

Technologies Used:  
* C# (.NET Framework – WinForms)  
* SQL Server 2019 LocalDB (Express)  
* Crystal Reports

***

Database Info:  
* Database File: `dbIMS.mdf`  
* The database is located in the project folder and linked using `|DataDirectory|` for portability.

Connection String Example:
SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\dbIMS.mdf;Integrated Security=True");

***

Login Credentials:  
The application supports 3 predefined users:

| Username | Password |
|**********|**********|
| Akash    | 12345    |
| Tahir    | 1234     |
| Ahmad    | 123      |

Enter these on the login form to access the system.

***
