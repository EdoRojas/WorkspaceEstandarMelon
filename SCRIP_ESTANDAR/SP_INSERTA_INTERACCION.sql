/****** Object:  StoredProcedure [wde].[XXUI_INSERTA_INTERACCION]    Script Date: 22/08/2018 22:11:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
CREATE PROCEDURE [wde].[XXUI_INSERTA_INTERACCION]

-- VARIABLES DE INTERACCION

@FIRST_NAME					  VARCHAR(100)	= NULL,
@LAST_NAME					  VARCHAR(100)	= NULL,
@USERNAME					  VARCHAR(100)	= NULL,
@tipificacion_connid          VARCHAR(50)	= NULL,
@tipificacion_ani             VARCHAR(30)	= NULL,
@tipificacion_denis           VARCHAR(30)	= NULL,
@tipificacion_tipoInteraccion INT,
@tipificacion_motivo          VARCHAR(500)  = NULL,
@tipificacion_Observaciones   VARCHAR(250)  = NULL,
@estado_interaccion           VARCHAR(50)	= NULL,
@estado_tipificacion          VARCHAR(50)	= NULL,
@AREA						  VARCHAR(50),
----ATTACHED
@1_CANAL					  VARCHAR(50)	=NULL,
@1_OPCION					  VARCHAR(50)	=NULL,
@1_RUT						  VARCHAR(50)	=NULL,

-- CAMPAÑAS
@GUID						  varchar(50)	= NULL,

@EVENTO						 VARCHAR(20)	= NULL,
--SALIDA
@output						  INT			= 0 OUTPUT  -- 0 ERROR  / 1 CORRECTO

AS  
     --************************************************************
     --  Sistema        : WDE  
     -- Objetivo		: permite Guardar o actualizar tipificaciones Bajo nueva estructura de normalizacion.
     -- Creado por	    : Desarrollo
     -- Fecha creacion  : 22-08-2018
     --*************************************************************
DECLARE @IDENTITY INT = 0;
BEGIN TRANSACTION; -- O solo BEGIN TRAN
    BEGIN TRY   
		IF (@EVENTO = 'EventDialing' OR @EVENTO ='EventRinging')
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
					   ,[ESTADO_TIPIFICACION]
					   ,[AREA])
                VALUES
						(@tipificacion_connid,
						 @tipificacion_ani,
						 @tipificacion_denis,
						 @tipificacion_tipoInteraccion,
						 GETDATE(), --  FECHA HORA INGRESO
						 @tipificacion_motivo,
						 @tipificacion_Observaciones,
						 @estado_interaccion,  
						 @estado_tipificacion,
						 @AREA);						 
			
			SET @IDENTITY = SCOPE_IDENTITY();
			SET @output = 1;

			INSERT INTO [wde].[AGENTE]
			   ([AGENTE_INTERACCION_ID]
			   ,[FIRST_NAME]
			   ,[LAST_NAME]
			   ,[USERNAME]
			   )
			 VALUES
				   (@IDENTITY
				   ,@FIRST_NAME
				   ,@LAST_NAME
				   ,@USERNAME)

			IF(@EVENTO ='EventRinging')
			BEGIN
				INSERT INTO [wde].[ATTACHED]
					   ([ATTACHED_INTERACCION_ID]
					   ,[1-CANAL]
					   ,[1-OPCION]
					   ,[1-RUT])
				VALUES
					  (@IDENTITY
					  ,@1_CANAL
					  ,@1_OPCION
					  ,@1_RUT)
			END
		END
			
        IF (@EVENTO ='EventReleased')
        BEGIN 
			SET @IDENTITY =(SELECT [INTERACCION_ID] FROM [wde].INTERACCION WHERE [CONNID] = @tipificacion_connid);
			IF(@IDENTITY >0 )
				BEGIN
					UPDATE [wde].[INTERACCION]
					set  [FECHA_HORA_TERMINO]		= GETDATE(),
						[ESTADO_INTERACCION]	= @estado_interaccion
					where [INTERACCION_ID] =  @IDENTITY			
					SET @output = 1;
				END
			ELSE
				BEGIN
					SET @output = 0; 
				END 
		END 
	
		IF (@EVENTO	='EventDialingCampana')	
		BEGIN
			UPDATE I
				SET [CONNID]				= @tipificacion_connid,
					[FECHA_HORA_INGRESO]	= GETDATE(),
					[ANI]					= @tipificacion_ani,
					[DNIS]					= @tipificacion_denis
				FROM [wde].[INTERACCION] I 
				JOIN [wde].[CAMPANA] C
				ON I.[INTERACCION_ID] = C.[CAMPANA_INTERACCION_ID]
				WHERE C.GUID = @GUID		
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

