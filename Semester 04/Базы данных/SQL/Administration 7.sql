-- Удаление базы данных
ALTER DATABASE vlads_computer SET OFFLINE WITH ROLLBACK IMMEDIATE;
ALTER DATABASE vlads_computer SET SINGLE_USER
USE master
GO
DROP DATABASE vlads_computer

-- Восстановление базы данных
USE master
GO
RESTORE DATABASE vlads_computer
FROM DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\Backup\vlads_omputer_backup.bak' WITH FILE = 1, NOUNLOAD, STATS = 5