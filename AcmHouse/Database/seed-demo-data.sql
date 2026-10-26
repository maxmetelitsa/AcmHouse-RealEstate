-- Demo data for AcmHouse: fictional people only.
-- Employees sign in with password 'Demo1234', the administrator (admin@acmhouse.by) with 'Admin1234'.
-- Passwords are stored as PBKDF2-SHA256 hashes (see Security/PasswordHasher.cs).
BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS Administrators (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Surname TEXT NOT NULL, Email TEXT NOT NULL UNIQUE, PasswordHash TEXT NOT NULL);
DELETE FROM Administrators;
DELETE FROM Users;
DELETE FROM Landlords;
DELETE FROM Sellers;
DELETE FROM [Order];
DELETE FROM UserOrders;
DELETE FROM OrderDetails;
DELETE FROM OrdersProfit;
DELETE FROM OpenedShift;
DELETE FROM ClosedShift;
DELETE FROM CalendarEvents;
DELETE FROM sqlite_sequence WHERE name IN ('Administrators','Users','Landlords','Sellers','OrderDetails','OrdersProfit','OpenedShift','ClosedShift','CalendarEvents');
INSERT INTO Administrators (Name, Surname, Email, PasswordHash) VALUES ('Admin', 'Manager', 'admin@acmhouse.by', 'PBKDF2$100000$35Bgm5/rZHSbZeyv60Vqkw==$HgofhjyHr5V5YZEPpp8KCCk9jSPMbDO6JC8qDl314es=');
INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, Shift) VALUES ('Иван', 'Петров', 'ivan.petrov@acmhouse.by', 'PBKDF2$100000$4g4wzrh642a/hOhUktOw0Q==$KYNXFNQXq/pPuFFlLQkGYTRokjlPPb13fO0GppEVfDo=', 'MP1234567', '3010190A001PB1', 0);
INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, Shift) VALUES ('Анна', 'Ковалёва', 'anna.kovaleva@acmhouse.by', 'PBKDF2$100000$pspidofV19WXBiW/odPA3w==$lWcSRwD/O2SPBiCvO2RycmG7D9uu41UeWQqUBPdTWes=', 'MP2345678', '4150592B002PB2', 0);
INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, Shift) VALUES ('Сергей', 'Новик', 'sergey.novik@acmhouse.by', 'PBKDF2$100000$aCzD9hV7lQRmsQkPUStDbA==$sub8DOAoe7KzNttvcY2VcWJRrXZnzrM/ULXFWILNQTM=', 'MP3456789', '3220788C003PB3', 0);
INSERT INTO Landlords (Name, Surname, Email, Phone, PassportNumber, IdentificationNumber) VALUES ('О.П.', 'Сидоренко', 'sidorenko@example.com', '+375 29 000-00-01', 'MP4567890', '4010185D004PB4');
INSERT INTO Landlords (Name, Surname, Email, Phone, PassportNumber, IdentificationNumber) VALUES ('Д.А.', 'Ермолаев', 'ermolaev@example.com', '+375 29 000-00-02', 'MP5678901', '3120399E005PB5');
INSERT INTO Landlords (Name, Surname, Email, Phone, PassportNumber, IdentificationNumber) VALUES ('Е.В.', 'Лукашевич', 'lukashevich@example.com', '+375 33 000-00-03', 'MP6789012', '4250693F006PB6');
INSERT INTO Sellers (ID, Initials, Surname, Email, Phone, PassportNumber, IdentificationNumber) VALUES (7, 'М.С.', 'Гончаренко', 'goncharenko@example.com', '+375 44 000-00-04', 'MP7890123', '3150184G007PB7');
COMMIT;
