-- ================================================
-- Template generated from Template Explorer using:
-- Create Inline Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ========================================================================
-- Sistema			: WDE  
-- Description		: Creacion de la funcion que separa las tipificaciones
-- Author		    : Desarrollo
-- Create date		: 10-02-2017
-- ========================================================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [wde].[SplitStrings]
(
	-- Add the parameters for the function here
	@List       NVARCHAR(MAX),
	@Separator1 Varchar(100),
	@Separator2 Varchar(100)
)
RETURNS  TABLE 
AS
	-- Add the SELECT statement with parameter references here
   RETURN 
   (  
      SELECT Item = y.i.value('(./text())[1]', 'nvarchar(4000)')
      FROM 
      ( 
        SELECT x = CONVERT(XML, '<i>' 
          + REPLACE(REPLACE(@List, ISNULL(@Separator1,''), '</i><i>') , ISNULL(@Separator2,''), '</i><i>') 
          + '</i>').query('.')
      ) AS a CROSS APPLY x.nodes('i') AS y(i)
   );
GO

GO
