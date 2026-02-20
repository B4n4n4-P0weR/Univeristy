-- Переименование базы данных
USE computer
ALTER DATABASE computer SET SINGLE_USER
EXEC SP_RENAMEDB 'computer', 'vlads_computer'
ALTER DATABASE vlads_computer SET MULTI_USER