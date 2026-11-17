# lab3-student-management-system-27133

StudentTrack: Student Management System

A Windows Forms desktop application (C#, .NET) for managing student records and marks, built with the MVC architecture and a MySQL database named studenttracking.

Lab 3 personal project. Author: MUSANA Jean Bosco, Student ID: 27133

Features
Register and login: teachers create an account and log in before using the system.
Add student: enter name, age, email, department, semester and 9 course marks, with input validation.
Edit and delete: click a row to load it, then Update or Delete (with Yes/No confirmation).
Search: the grid filters live by student name or ID.
GPA and summary: each student's GPA is calculated as the average of the 9 marks. The form shows the total students, the class average GPA and the top student.
Audit log: every register, login, add, update and delete is recorded and can be viewed in the app.
Courses
Web Technology
PL/SQL
Software Engineering
Object Oriented Programming
Operating System
Java Programming
English
Applied Mathematics
Multivariable Calculus
Architecture (MVC)
Folder	Role	Contents
model/	Model	Student, Lookup, AuditEntry
data/	Model (data access)	DatabaseHelper and the repositories that run the SQL
view/	View	LoginForm, RegisterForm, MainForm, AuditForm
controller/	Controller	AuthController, StudentController, AuditController, Session (validation and logic)

Views never talk to the database directly. They call controllers, and controllers call repositories.

Database

Database name: studenttracking (MySQL / MariaDB). It has 7 tables and 1 view:

Table	Purpose
Users	Teacher accounts
Departments	Student departments
Semesters	Academic semesters
Students	Student details (FK to Departments)
Courses	The 9 courses
Enrollments	One mark per student per course (FK to Students, Courses, Semesters)
AuditLog	Record of user actions (FK to Users)
vw_StudentGPA (view)	GPA calculated as the average mark, never stored

All SQL commands are parameterized, which prevents SQL injection.

Requirements
Windows 10 or newer
Visual Studio 2022 or later with the .NET desktop development workload
XAMPP (MySQL + phpMyAdmin)
NuGet package MySqlConnector (restored automatically)
How to run
Start MySQL in the XAMPP Control Panel.
Open http://localhost/phpmyadmin, click the SQL tab, paste the contents of Database/studenttracking.sql and click Go. This creates the database, tables and seed data.
Open the solution in Visual Studio.
Check the connection string in data/DatabaseHelper.cs. The default is user root with an empty password (the XAMPP default).
Press F5.
Click Create an account, register, log in and start adding students.
Notes
Passwords are stored as plain text, as the project specification allows. A real product should hash passwords.
The application is single-user and runs on one computer.
