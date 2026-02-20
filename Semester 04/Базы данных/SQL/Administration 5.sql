-- Создание резервной копии
EXEC sp_addumpdevice 'disk','master_backup',
'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\Backup\vlads_computer_backup.bak'
GO
Use vlads_computer
DBCC CHECKDB ('vlads_computer') WITH NO_INFOMSGS
BACKUP DATABASE vlads_computer to master_backup WITH INIT