/****** Object:  StoredProcedure [wde].[XIUD_INSERTA_TIPIFICACION]    Script Date: 22/08/2018 22:10:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
CREATE PROCEDURE [wde].[XIUD_INSERTA_TIPIFICACION]

-- definicion de las variables
@tipificacion_connid        VARCHAR(50)  = NULL,
@username_agente		VARCHAR(50)  = NULL,
@tipificacion_motivo        VARCHAR(500) = NULL,
@tipificacion_Observaciones VARCHAR(250) = NULL,
@estado_tipificacion        VARCHAR(50)  = NULL,
@campana                    VARCHAR(50)  = NULL,
@IDS                        VARCHAR(MAX) = NULL,

@output                     INT          = 0 OUTPUT
AS  
     --************************************************************
     --  Sistema        :   
     -- Objetivo		: permite Guardar o actualizar tipificaciones Bajo nueva estructura de normalizacion.
     -- Creado por	    : Desarrollo
     -- Fecha creacion  : 22-08-2018
     --*************************************************************
 SET NOCOUNT ON;
 DECLARE @errno INT, @errmsg VARCHAR(255), @operacion VARCHAR(10), @consulta VARCHAR(1000), @traeEstado VARCHAR(1000), @IDENTITY INT,
 --Variables Cursor
 @MyCursor CURSOR, @itema VARCHAR(1000), @itemnumber INT;
 BEGIN TRANSACTION; -- O solo BEGIN TRAN

BEGIN TRY
    IF(@campana = 'PreviewRecord')
        BEGIN
        -----------------------------------------------------------------------------------------------------------
        ---PARA EL USO DE LAS CAMPAÑAS DESCOMENTAR ESTOS CAMPOS Y COMETAR EL SET @output-------------------------
		-----------------------------------------------------------------------------------------------------------			
         SET @output = 0
          IF EXISTS
            (
                SELECT TOP (1) 1
                FROM [wde].[INTERACCION] I
                     JOIN [wde].[CAMPANA] C ON I.[INTERACCION_ID] = C.[CAMPANA_INTERACCION_ID]
                WHERE C.GUID = @tipificacion_connid 
            )
                BEGIN
                    SET @IDENTITY =
                    (
                        SELECT TOP (1) I.[INTERACCION_ID]
                        FROM [wde].[INTERACCION] I
                             JOIN [wde].[CAMPANA] C ON I.[INTERACCION_ID] = C.[CAMPANA_INTERACCION_ID]
							 INNER JOIN [wde].[AGENTE] A
						ON I.[INTERACCION_ID] = A.[AGENTE_INTERACCION_ID]
                        WHERE C.GUID = @tipificacion_connid AND A.[USERNAME] = @username_agente 
                        order by FECHA_HORA_INGRESO desc 
                    );
		 	
                    ---UPDATE EN TABLA TIPIFICACIONES---------------------------------------------------------------------------
                    UPDATE [wde].[INTERACCION]
                      SET
                           [OBSERVACIONES] = @tipificacion_Observaciones,
                          [MOTIVO] = @tipificacion_motivo,
                          [ESTADO_TIPIFICACION] = @estado_tipificacion
                    WHERE [INTERACCION_ID] = @IDENTITY;
                    -----------------------------------------------------------------------------------------------------------
                    ---INSERT EN TABLA CLIENTE  Si no se utiliza la tabla cliente comentar este bloque-------------------------
					-----------------------------------------------------------------------------------------------------------
					                        
                    --INSERT EN TABLA TIPIFICACIONES POR FOLIO-------------------------------------------------------
                    IF(@IDS IS NOT NULL
                       AND @IDS != '')
                        BEGIN
                            ---UPDATE EN TABLA TIPIFICACIONES X FOLIO-----------------------------------------------------------------
                            DELETE TF
                            FROM [wde].[TIPIFICACION] TF
                                 INNER JOIN [wde].[INTERACCION] T ON T.[INTERACCION_ID] = TF.[TIPIFICACION_INTERACCION_ID]
                            WHERE T.[INTERACCION_ID] = @IDENTITY;
                            SET @MyCursor = CURSOR
                            FOR SELECT *
                                FROM [wde].SplitStrings(@IDS, '|', ',');
                            OPEN @MyCursor;
                            FETCH NEXT FROM @MyCursor INTO @itema;
                            WHILE @@FETCH_STATUS = 0
                                BEGIN
                                    BEGIN
                                        INSERT INTO [TIPIFICACION]
                                        ([TIPIFICACION_INTERACCION_ID],
                                         [TIPIFICACION_DICCIONARIO_ID]
                                        )
                                        VALUES
                                        (@IDENTITY,
                                         CONVERT( INTEGER, @itema)
                                        );
                                    END;
                                    FETCH NEXT FROM @MyCursor INTO @itema;
                                END;
                            CLOSE @MyCursor;
                            DEALLOCATE @MyCursor;
                        END;
                    SET @output = 1;
                END;
        END;
    ELSE
        BEGIN
            IF EXISTS
            (
                SELECT TOP (1) 1
                FROM [wde].[INTERACCION]
                WHERE [CONNID] = @tipificacion_connid 
            )
                BEGIN
                    SET @IDENTITY =
                    (
                        SELECT TOP 1 [INTERACCION_ID]
                        FROM [wde].[INTERACCION] I
						INNER JOIN [wde].[AGENTE] A
						ON I.[INTERACCION_ID] = A.[AGENTE_INTERACCION_ID]
                        WHERE [CONNID] = @tipificacion_connid AND A.[USERNAME] = @username_agente  
                        ORDER BY [FECHA_HORA_INGRESO] DESC
                    ); 
                    ---UPDATE EN TABLA TIPIFICACIONES-----------------------------------------------------
                    UPDATE [wde].[INTERACCION]
                      SET
                          [OBSERVACIONES] = @tipificacion_Observaciones,
                          [MOTIVO] = @tipificacion_motivo,
                          [ESTADO_TIPIFICACION] = @estado_tipificacion
                    WHERE [INTERACCION_ID] = @IDENTITY;
                    --------------------------------------------------------------------------------------
                    ---INSERT EN TABLA CLIENTE  Si no se utiliza la tabla cliente comentar este bloque----
                    --------------------------------------------------------------------------------------
                    --INSERT EN TABLA TIPIFICACIONES POR FOLIO--------------------------------------------
                    IF(@IDS IS NOT NULL
                       AND @IDS != '')
                        BEGIN
                            ---UPDATE EN TABLA TIPIFICACIONES X FOLIO-----------------------------------------------------------------
                            DELETE TF
                            FROM [wde].[TIPIFICACION] TF
                                 INNER JOIN [wde].[INTERACCION] T ON T.[INTERACCION_ID] = TF.[TIPIFICACION_INTERACCION_ID]
                            WHERE T.[INTERACCION_ID] = @IDENTITY;
                            SET @MyCursor = CURSOR
                            FOR SELECT *
                                FROM [wde].SplitStrings(@IDS, '|', ',');
                            OPEN @MyCursor;
                            FETCH NEXT FROM @MyCursor INTO @itema;
                            WHILE @@FETCH_STATUS = 0
                                BEGIN
                                    BEGIN
                                        INSERT INTO [TIPIFICACION]
                                        ([TIPIFICACION_INTERACCION_ID],
                                         [TIPIFICACION_DICCIONARIO_ID]
                                        )
                                        VALUES
                                        (@IDENTITY,
                                         CONVERT( INTEGER, @itema)
                                        );
                                    END;
                                    FETCH NEXT FROM @MyCursor INTO @itema;
                                END;
                            CLOSE @MyCursor;
                            DEALLOCATE @MyCursor;
                        END;
                    SET @output = 1;
                END;
        END;
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
