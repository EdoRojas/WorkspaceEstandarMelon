/****** Object:  StoredProcedure [wde].[XXUI_INSERTA_CAMPANA]    Script Date: 22/08/2018 22:11:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
CREATE PROCEDURE [wde].[XXUI_INSERTA_CAMPANA]
-- DEJAR EL NOMBRE DE CAMPAÑA CORRESPONDIENTE
-- VARIABLES DE INTERACCION

@FIRST_NAME					  VARCHAR(100)	= NULL,
@LAST_NAME					  VARCHAR(100)	= NULL,
@USERNAME					  VARCHAR(100)	= NULL,
@tipificacion_connid          VARCHAR(50)	= NULL,
@tipificacion_ani             VARCHAR(30)	= NULL,
@tipificacion_denis           VARCHAR(30)	= NULL,
@tipificacion_tipoInteraccion INT			,
@tipificacion_motivo          VARCHAR(500)  = NULL,
@tipificacion_Observaciones   VARCHAR(250)  = NULL,
@estado_interaccion           VARCHAR(50)	= NULL,
@estado_tipificacion          VARCHAR(50)	= NULL,

--REGISTROS DE CAMPAÑAS
@GUID						  VARCHAR(50)	= NULL,

@CAMPAING_NAME				  VARCHAR(200),
@FIELD						  XML = NULL,

@EVENTO						  VARCHAR(20)	= NULL,
--SALIDA
@output						  INT			= 0 OUTPUT  -- 0 ERROR  / 1 CORRECTO
AS  
     --************************************************************
     --  Sistema        : WDE  
     -- Objetivo		: permite Guardar o actualizar tipificaciones Bajo nueva estructura de normalizacion.
     -- Creado por	    : Desarrollo
     -- Fecha creacion  : 10-02-2017
     --*************************************************************
DECLARE @IDENTITY INT = 0;
BEGIN TRANSACTION; -- O solo BEGIN TRAN
    BEGIN TRY   
        ----------------------------
        --INSERT DE INTERACCION-----
        ----------------------------
		IF (@EVENTO = 'PreviewRecord')
		BEGIN
			INSERT INTO [wde].[INTERACCION]
					   ([CONNID]
					   ,[ANI]
					   ,[DNIS]
					   ,[TIPO_INTERACCION_ID]
					   ,[FECHA_HORA_INGRESO]
					   ,[MOTIVO]
					   ,[OBSERVACIONES]
					   ,[ESTADO_INTERACCION]
					   ,[ESTADO_TIPIFICACION])
                VALUES
						(@tipificacion_connid,
						 @tipificacion_ani,
						 @tipificacion_denis,
						 @tipificacion_tipoInteraccion,
						 GETDATE(), --  FECHA HORA INGRESO
						 @tipificacion_motivo,
						 @tipificacion_Observaciones,
						 @estado_interaccion,  
						 @estado_tipificacion);	

			SET @IDENTITY = 	SCOPE_IDENTITY();		 
			
			INSERT INTO [wde].[AGENTE]
			   (
			   [AGENTE_INTERACCION_ID]
			   ,[FIRST_NAME]
			   ,[LAST_NAME]
			   ,[USERNAME]
			   )
			 VALUES
				   (@IDENTITY
				   ,@FIRST_NAME
				   ,@LAST_NAME
				   ,@USERNAME)
		   	 
			INSERT INTO [wde].[CAMPANA]
							   ([CAMPANA_INTERACCION_ID]
							   ,[GUID]
							   ,[CAMPAING_NAME]
							   ,[FIELD])
					VALUES
								(@IDENTITY,
								@GUID,
								@CAMPAING_NAME,
								@FIELD)	
			SET @output = 1;

		END	
	COMMIT TRANSACTION; -- O solo COMMIT
        RETURN @output;
	END TRY
	
BEGIN CATCH
    SET @output = 0;
    SELECT ERROR_NUMBER() AS ErrorNumber,
           ERROR_SEVERITY() AS ErrorSeverity,
           ERROR_STATE() AS ErrorState,
           ERROR_PROCEDURE() AS ErrorProcedure,
           ERROR_LINE() AS ErrorLine,
           ERROR_MESSAGE() AS ErrorMessage;
ROLLBACK TRANSACTION; -- O solo ROLLBACK
    RETURN @output;
END CATCH;

