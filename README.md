# InvoiceManager

Hello, I am programmer and freelancer Martin Zaloga and this is my simple web project for demonstration what I can do in one manday.
This simple project is done in asp.net core MVC using Entity Framework core.
This project is running on http://invoice-manager.site/.
For more informations about me and next my references please visit web site https://zaloga.cz/.

# Original request:
Create a simple ASP.NET Core MVC application in C# to manage invoices in any database system. The application should have the following features:

creating/editing an invoice,
adding/removing invoice items.
Another part of the application is to create a simple API. Access to API should be restricted by a secret key which is sent as a header value in the request. Please prepare 3 endpoints which have following functionality:

getting collection of unpaid invoices,
paying invoice (changing status to paid),
editing invoice (PATCH request).

# My proposed solution

Model for Invoice:

int Id PK (not null)

int InvoiceNumber (not null)

string Supplier (not null)

string Customer (not null)

string InvoiceSubject (not null)

string PayMethod (not null)

string BankAccountNumber (null)

DateTime CreatedDate (not null)

DateTime PaydDate (not null)

DateTime DueDate (not null)

Model for Invoice item:

int Id PK (not null)

string InvoiceItemSubject (not null)

int Price (not null)

int InvoiceId FK (not null)

Web:

CRUD and list of invoices

CRUD of invoice items

API:

getting collection of (unpaid) invoices

paying invoice (changing PaydDate from null to current date and time)

editing invoice

DB:

as DB storage is used MSSQL

as ORM is used Entity Framework core
