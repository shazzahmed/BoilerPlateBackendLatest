DBCC CHECKIDENT ('Classes', RESEED, 15);
DBCC CHECKIDENT ('ClassSections', RESEED, 29);

USE master;
ALTER DATABASE SMSANGULAR SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE SMSANGULAR;


BEGIN
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = N'';

    SELECT @sql = @sql + '
    ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' +
                    QUOTENAME(OBJECT_NAME(parent_object_id)) +
                    ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
    FROM sys.foreign_keys;

    PRINT @sql; -- Optional: to preview what will be dropped

    EXEC sp_executesql @sql;
END
DROP TABLE IF EXISTS [ASPNETUsers], [Department], [Students], [Staff], [Tenants];
