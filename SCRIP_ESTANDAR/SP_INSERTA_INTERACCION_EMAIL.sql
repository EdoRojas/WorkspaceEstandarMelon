/****** Object:  StoredProcedure [wde].[XXUI_INSERTA_INTERACCION_EMAIL]    Script Date: 22/08/2018 22:10:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
CREATE PROCEDURE [wde].[XXUI_INSERTA_INTERACCION_EMAIL]

-- VARIABLES DE INTERACCION
@interaccionId           VARCHAR(50)  = NULL,
@interaccionTipo         VARCHAR(50)  = NULL,
@estadoInteraccion       VARCHAR(50)  = NULL,
@estadoTipificacion      VARCHAR(50)  = NULL,

@interaccionFrom         VARCHAR(MAX) = NULL,
@interaccionTo           VARCHAR(MAX) = NULL,
@interaccionCc           VARCHAR(MAX) = NULL,
@interaccionCco          VARCHAR(MAX) = NULL,
@interaccionParentId     VARCHAR(50)  = NULL,
@output                  INT          = 0 OUTPUT  -- 0 ERROR  / 1 CORRECTO

AS  
     --************************************************************
     -- Sistema	    : WDE  
     -- Objetivo	    : permite Guardar o actualizar tipificaciones Bajo nueva estructura de normalizacion.
     -- Creado por	    : Desarrollo
     -- Fecha creacion  : 17-03-2017
     --*************************************************************
     DECLARE @IDENTITY INT= 0;
     BEGIN TRANSACTION; -- O solo BEGIN TRAN
    BEGIN TRY   
        ----------------------------
        --INSERT DE INTERACCION-----
        ----------------------------
        IF NOT EXISTS
        (
            SELECT TOP (1) 1
            FROM [wde].[INTERACCION]
            WHERE [CONNID] = @interaccionId
        )
            BEGIN
                INSERT INTO [wde].[INTERACCION]
                ([CONNID],
                 [TIPO_INTERACCION_ID],
                 [FECHA_HORA_INGRESO],
                 [ESTADO_INTERACCION],
                 [ESTADO_TIPIFICACION]
                )
                VALUES
                (@interaccionId,
                 @interaccionTipo,
                 GETDATE(), --  FECHA HORA INGRESO
                 @estadoInteraccion,
                 @estadoTipificacion
                );
                INSERT INTO [wde].[EMAIL]
                ([ID],
                 [FROM],
                 [TO],
                 [CC],
                 [CCO],
                 [INTERACCION_ID],
                 [PARENT_ID]
                )
                VALUES
                (SCOPE_IDENTITY(),
                 @interaccionFrom,
                 @interaccionTo,
                 @interaccionCc,
                 @interaccionCco,
                 @interaccionId,
                 @interaccionParentId
                );
                SET @output = 1;
            END;
        ELSE
            BEGIN
                SET @IDENTITY =
                (
                    SELECT [INTERACCION_ID]
                    FROM [wde].INTERACCION
                    WHERE [CONNID] = @interaccionId
                );
                IF(@IDENTITY > 0)
                    BEGIN
                        UPDATE [wde].[INTERACCION]
                          SET
                              [FECHA_HORA_TERMINO] = GETDATE(),
                              [ESTADO_INTERACCION] = @estadoInteraccion
                        WHERE [INTERACCION_ID] = @IDENTITY;
                        UPDATE [wde].[EMAIL]
                          SET
                              [FROM] = @interaccionFrom,
                              [TO] = @interaccionTo,
                              [CC] = @interaccionCc,
                              [CCO] = @interaccionCco,
                              [PARENT_ID] = @interaccionParentId
                        WHERE [ID] = @IDENTITY;
                        SET @output = 1;
                    END;
                ELSE
                    BEGIN
                        SET @output = 0;
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
