-- ===================================
-- Create Schema User template
-- ===================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'wde')
EXEC sys.sp_executesql N'CREATE SCHEMA [wde] AUTHORIZATION [dbo]'

GO




