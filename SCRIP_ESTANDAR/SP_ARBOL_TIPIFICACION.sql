/****** Object:  StoredProcedure [wde].[SXXX_TRAE_ARBOL_DE_TIPIFICACION]    Script Date: 22/08/2018 22:13:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Desarrollo
-- Create date: 22-08-2018
-- Description:	Trea el diccionario de datos para cargalo en el login
-- =============================================
CREATE PROCEDURE [wde].[SXXX_TRAE_ARBOL_DE_TIPIFICACION] 

AS
SELECT [DICCIONARIO_ID]
      ,[DESCRIPCION]
      ,[VISTA]
      ,[DEPENDIENTE]
      ,[ACTIVO]
      ,[JERARQUIA]
      ,[NIVEL]
      ,[TIPO_DATO]
  FROM [wde].[DICCIONARIO]
  WHERE [ACTIVO] = 1
