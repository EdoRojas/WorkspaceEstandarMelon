/****** Object:  StoredProcedure [wde].[SXXX_TRAE_HISTORIAL]    Script Date: 22/08/2018 22:09:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [wde].[SXXX_TRAE_HISTORIAL] 
-- Add the parameters for the stored procedure here
@ANI  VARCHAR(30) = NULL,
@DNIS VARCHAR(30) = NULL,
@TIPO VARCHAR(10) = NULL
AS
   SELECT TOP (10) I.[INTERACCION_ID],
                     I.[FECHA_HORA_INGRESO],
					 I.[ANI],
					 I.[DNIS],
					 A.[1-OPCION],				 
                     AG.[USERNAME],
                     I.[MOTIVO],
                     I.[OBSERVACIONES],
                     T.[TIPO]
     FROM [wde].[INTERACCION] I
	 INNER JOIN [wde].[TIPO_INTERACCION] T
	 ON I.[TIPO_INTERACCION_ID] = T.[TIPO_INTERACCION_ID]
	 INNER JOIN  [wde].[AGENTE] AG
	 ON I.[INTERACCION_ID] = AG.[AGENTE_INTERACCION_ID]
	LEFT JOIN [wde].[ATTACHED] A
	ON I.[INTERACCION_ID] = A.[ATTACHED_INTERACCION_ID]
     WHERE I.[ANI] = @ANI OR I.[DNIS] = @DNIS 
     ORDER BY I.[INTERACCION_ID] DESC;