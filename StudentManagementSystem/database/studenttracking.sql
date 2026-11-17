
CREATE DATABASE IF NOT EXISTS studenttracking;
USE studenttracking;

-- Drop in reverse order so the script can be re-run safely
DROP VIEW  IF EXISTS vw_StudentGPA;
DROP TABLE IF EXISTS AuditLog;
DROP TABLE IF EXISTS Enrollments;
DROP TABLE IF EXISTS Courses;
DROP TABLE IF EXISTS Students;
DROP TABLE IF EXISTS Semesters;
DROP TABLE IF EXISTS Departments;
DROP TABLE IF EXISTS Users;

-- 1. Users (teachers who log in)
CREATE TABLE Users (
    UserId   INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(50) NOT NULL
) ENGINE=InnoDB;


CREATE TABLE Departments (
    DepartmentId   INT AUTO_INCREMENT PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB;


CREATE TABLE Semesters (
    SemesterId   INT AUTO_INCREMENT PRIMARY KEY,
    SemesterName VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB;


CREATE TABLE Students (
    StudentId    INT AUTO_INCREMENT PRIMARY KEY,
    Name         VARCHAR(100) NOT NULL,
    Age          INT NOT NULL,
    Email        VARCHAR(100) NOT NULL,
    DepartmentId INT NOT NULL,
    CONSTRAINT CK_Students_Age CHECK (Age BETWEEN 5 AND 100),
    CONSTRAINT FK_Students_Departments
        FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
) ENGINE=InnoDB;


CREATE TABLE Courses (
    CourseId   INT AUTO_INCREMENT PRIMARY KEY,
    CourseCode VARCHAR(20)  NOT NULL UNIQUE,
    CourseName VARCHAR(100) NOT NULL UNIQUE,
    Credits    INT NOT NULL DEFAULT 3
) ENGINE=InnoDB;


CREATE TABLE Enrollments (
    EnrollmentId INT AUTO_INCREMENT PRIMARY KEY,
    StudentId    INT NOT NULL,
    CourseId     INT NOT NULL,
    SemesterId   INT NOT NULL,
    Mark         DOUBLE NOT NULL,
    CONSTRAINT CK_Enr_Mark CHECK (Mark BETWEEN 0 AND 100),
    CONSTRAINT FK_Enr_Students  FOREIGN KEY (StudentId)  REFERENCES Students(StudentId) ON DELETE CASCADE,
    CONSTRAINT FK_Enr_Courses   FOREIGN KEY (CourseId)   REFERENCES Courses(CourseId),
    CONSTRAINT FK_Enr_Semesters FOREIGN KEY (SemesterId) REFERENCES Semesters(SemesterId),
    CONSTRAINT UQ_Enr UNIQUE (StudentId, CourseId)
) ENGINE=InnoDB;


CREATE TABLE AuditLog (
    LogId   INT AUTO_INCREMENT PRIMARY KEY,
    UserId  INT NULL,
    Action  VARCHAR(50)  NOT NULL,
    Details VARCHAR(200) NULL,
    LogTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Audit_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL
) ENGINE=InnoDB;

-- GPA view: calculated, never stored (SRS FR5.1 / FR5.2)
CREATE VIEW vw_StudentGPA AS
SELECT s.StudentId,
       s.Name,
       ROUND(AVG(e.Mark), 2) AS GPA
FROM Students s
LEFT JOIN Enrollments e ON e.StudentId = s.StudentId
GROUP BY s.StudentId, s.Name;


INSERT INTO Departments (DepartmentName) VALUES
 ('Information Technology'), ('Computer Science'), ('Software Engineering');

INSERT INTO Semesters (SemesterName) VALUES
 ('2026-2027 Semester 1'), ('2026-2027 Semester 2');

INSERT INTO Courses (CourseCode, CourseName, Credits) VALUES
 ('WT101',  'Web Technology',               3),
 ('PLS102', 'PL/SQL',                       3),
 ('SE103',  'Software Engineering',         3),
 ('OOP104', 'Object Oriented Programming',  3),
 ('OS105',  'Operating System',             3),
 ('JAV106', 'Java Programming',             3),
 ('ENG107', 'English',                      2),
 ('AM108',  'Applied Mathematics',          3),
 ('MVC109', 'Multivariable Calculus',       3);