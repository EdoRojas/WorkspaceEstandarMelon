/****** Object:  Table [wde].[AGENTE]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[AGENTE](
	[AGENTE_INTERACCION_ID] [int] NULL,
	[FIRST_NAME] [varchar](100) NULL,
	[LAST_NAME] [varchar](100) NULL,
	[USERNAME] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[ATTACHED]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[ATTACHED](
	[ATTACHED_INTERACCION_ID] [int] NOT NULL,
	[1-CANAL] [varchar](50) NULL,
	[1-OPCION] [varchar](50) NULL,
	[1-RUT] [varchar](50) NULL,
 CONSTRAINT [PK_ATTACHED] PRIMARY KEY CLUSTERED 
(
	[ATTACHED_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[CAMPANA]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[CAMPANA](
	[CAMPANA_INTERACCION_ID] [int] NOT NULL,
	[GUID] [varchar](50) NULL,
	[CAMPAING_NAME] [varchar](200) NULL,
	[FIELD] [xml] NULL,
 CONSTRAINT [PK_CAMPANA] PRIMARY KEY CLUSTERED 
(
	[CAMPANA_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wde].[CHAT]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[CHAT](
	[CHAT_INTERACCION_ID] [int] NOT NULL,
	[APELLIDO] [varchar](50) NULL,
	[NOMBRE] [varchar](50) NULL,
	[CORREO] [varchar](200) NULL,
	[ASUNTO] [varchar](50) NULL,
 CONSTRAINT [PK_CHAT] PRIMARY KEY CLUSTERED 
(
	[CHAT_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[CLIENTE]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[CLIENTE](
	[CLIENTE_INTERACCION_ID] [int] NOT NULL,
	[RUT] [varchar](100) NULL,
 CONSTRAINT [PK_CLIENTE] PRIMARY KEY CLUSTERED 
(
	[CLIENTE_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[DICCIONARIO]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[DICCIONARIO](
	[DICCIONARIO_ID] [int] IDENTITY(1,1) NOT NULL,
	[DESCRIPCION] [varchar](80) NULL,
	[VISTA] [varchar](30) NULL,
	[DEPENDIENTE] [int] NOT NULL,
	[ACTIVO] [bit] NOT NULL,
	[JERARQUIA] [varchar](50) NULL,
	[NIVEL] [int] NOT NULL,
	[TIPO_DATO] [int] NULL,
 CONSTRAINT [PK_DICCIONARIO_DATO] PRIMARY KEY CLUSTERED 
(
	[DICCIONARIO_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[INTERACCION]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[INTERACCION](
	[INTERACCION_ID] [int] IDENTITY(1,1) NOT NULL,
	[CONNID] [varchar](50) NULL,
	[ANI] [varchar](50) NULL,
	[DNIS] [varchar](50) NULL,
	[TIPO_INTERACCION_ID] [int] NULL,
	[FECHA_HORA_INGRESO] [datetime] NULL,
	[FECHA_HORA_TERMINO] [datetime] NULL,
	[MOTIVO] [varchar](500) NULL,
	[OBSERVACIONES] [varchar](250) NULL,
	[ESTADO_INTERACCION] [varchar](50) NULL,
	[ESTADO_TIPIFICACION] [varchar](50) NULL,
	[AREA] [varchar](50) NOT NULL,
 CONSTRAINT [PK_INTERACCION] PRIMARY KEY CLUSTERED 
(
	[INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[RRSS]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[RRSS](
	[RRSS_INTERACCION_ID] [int] NOT NULL,
	[INTERACTION_RRSS_ID] [varchar](50) NULL,
	[NICKNAME] [varchar](2000) NULL,
	[USER_ID] [varchar](2000) NULL,
	[TIPO_RRSS] [int] NULL,
 CONSTRAINT [PK_FACEBOOK] PRIMARY KEY CLUSTERED 
(
	[RRSS_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[TIPIFICACION]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[TIPIFICACION](
	[TIPIFICACION_INTERACCION_ID] [int] NOT NULL,
	[TIPIFICACION_DICCIONARIO_ID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [wde].[TIPO_INTERACCION]    Script Date: 22/08/2018 22:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wde].[TIPO_INTERACCION](
	[TIPO_INTERACCION_ID] [int] IDENTITY(1,1) NOT NULL,
	[TIPO] [varchar](100) NULL,
 CONSTRAINT [PK_TIPO_INTERACCION] PRIMARY KEY CLUSTERED 
(
	[TIPO_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [wde].[TIPO_INTERACCION] ON 
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (1, N'INBOUND')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (2, N'OUTBOUND')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (3, N'CHAT')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (4, N'RRSS')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (5, N'EMAIL')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (6, N'PUSH-PREVIEW')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (7, N'PREVIEW')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (8, N'PREDICTIVO')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (9, N'SCHEDULEDCALL')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (10, N'CALLBACK')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (11, N'EMAIL_INBOUND')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (12, N'EMAIL_OUTBOUND')
GO
INSERT [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID], [TIPO]) VALUES (13, N'OUTBOUND_CAMPAIGN')
GO
SET IDENTITY_INSERT [wde].[TIPO_INTERACCION] OFF
GO
/****** Object:  Index [IDX_ID_AGENTE]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [IDX_ID_AGENTE] ON [wde].[AGENTE]
(
	[AGENTE_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [INDX_GUID]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_GUID] ON [wde].[CAMPANA]
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [INDX_DEPENDIENTE]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_DEPENDIENTE] ON [wde].[DICCIONARIO]
(
	[DEPENDIENTE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [INDX_TIPO_ACTIVO]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_TIPO_ACTIVO] ON [wde].[DICCIONARIO]
(
	[VISTA] ASC,
	[ACTIVO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [INDX_ANI_DNIS]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_ANI_DNIS] ON [wde].[INTERACCION]
(
	[ANI] ASC,
	[DNIS] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [INDX_CONNID]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_CONNID] ON [wde].[INTERACCION]
(
	[CONNID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [INDX_FECHA_HORA_INGRESO]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_FECHA_HORA_INGRESO] ON [wde].[INTERACCION]
(
	[FECHA_HORA_INGRESO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [INDX_CODIGO]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_CODIGO] ON [wde].[TIPIFICACION]
(
	[TIPIFICACION_DICCIONARIO_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [INDX_ID]    Script Date: 22/08/2018 22:21:16 ******/
CREATE NONCLUSTERED INDEX [INDX_ID] ON [wde].[TIPIFICACION]
(
	[TIPIFICACION_INTERACCION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [wde].[AGENTE]  WITH CHECK ADD  CONSTRAINT [FK_AGENTE_INTERACCION] FOREIGN KEY([AGENTE_INTERACCION_ID])
REFERENCES [wde].[INTERACCION] ([INTERACCION_ID])
ON DELETE CASCADE
GO
ALTER TABLE [wde].[AGENTE] CHECK CONSTRAINT [FK_AGENTE_INTERACCION]
GO
ALTER TABLE [wde].[ATTACHED]  WITH CHECK ADD  CONSTRAINT [FK_ATTACHED_INTERACCION] FOREIGN KEY([ATTACHED_INTERACCION_ID])
REFERENCES [wde].[INTERACCION] ([INTERACCION_ID])
ON DELETE CASCADE
GO
ALTER TABLE [wde].[ATTACHED] CHECK CONSTRAINT [FK_ATTACHED_INTERACCION]
GO
ALTER TABLE [wde].[CAMPANA]  WITH CHECK ADD  CONSTRAINT [FK_CAMPANA_INTERACCION] FOREIGN KEY([CAMPANA_INTERACCION_ID])
REFERENCES [wde].[INTERACCION] ([INTERACCION_ID])
ON DELETE CASCADE
GO
ALTER TABLE [wde].[CAMPANA] CHECK CONSTRAINT [FK_CAMPANA_INTERACCION]
GO
ALTER TABLE [wde].[CHAT]  WITH CHECK ADD  CONSTRAINT [FK_CHAT_INTERACCION] FOREIGN KEY([CHAT_INTERACCION_ID])
REFERENCES [wde].[INTERACCION] ([INTERACCION_ID])
ON DELETE CASCADE
GO
ALTER TABLE [wde].[CHAT] CHECK CONSTRAINT [FK_CHAT_INTERACCION]
GO
ALTER TABLE [wde].[CLIENTE]  WITH CHECK ADD  CONSTRAINT [FK_CLIENTE_INTERACCION] FOREIGN KEY([CLIENTE_INTERACCION_ID])
REFERENCES [wde].[INTERACCION] ([INTERACCION_ID])
ON DELETE CASCADE
GO
ALTER TABLE [wde].[CLIENTE] CHECK CONSTRAINT [FK_CLIENTE_INTERACCION]
GO
ALTER TABLE [wde].[INTERACCION]  WITH CHECK ADD  CONSTRAINT [FK_INTERACCION_TIPO_INTERACCION] FOREIGN KEY([TIPO_INTERACCION_ID])
REFERENCES [wde].[TIPO_INTERACCION] ([TIPO_INTERACCION_ID])
GO
ALTER TABLE [wde].[INTERACCION] CHECK CONSTRAINT [FK_INTERACCION_TIPO_INTERACCION]
GO
ALTER TABLE [wde].[RRSS]  WITH CHECK ADD  CONSTRAINT [FK_FACEBOOK_INTERACCION] FOREIGN KEY([RRSS_INTERACCION_ID])
REFERENCES [wde].[INTERACCION] ([INTERACCION_ID])
GO
ALTER TABLE [wde].[RRSS] CHECK CONSTRAINT [FK_FACEBOOK_INTERACCION]
GO
ALTER TABLE [wde].[TIPIFICACION]  WITH CHECK ADD  CONSTRAINT [FK_TIPIFICACION_DICCIONARIO_DATO] FOREIGN KEY([TIPIFICACION_DICCIONARIO_ID])
REFERENCES [wde].[DICCIONARIO] ([DICCIONARIO_ID])
GO
ALTER TABLE [wde].[TIPIFICACION] CHECK CONSTRAINT [FK_TIPIFICACION_DICCIONARIO_DATO]
GO
ALTER TABLE [wde].[TIPIFICACION]  WITH CHECK ADD  CONSTRAINT [FK_TIPIFICACION_INTERACCION] FOREIGN KEY([TIPIFICACION_INTERACCION_ID])
REFERENCES [wde].[INTERACCION] ([INTERACCION_ID])
ON DELETE CASCADE
GO
ALTER TABLE [wde].[TIPIFICACION] CHECK CONSTRAINT [FK_TIPIFICACION_INTERACCION]
GO
EXEC sys.sp_addextendedproperty @name=N'[1-RUT]', @value=N'VALOR DESDE LA ESTRATEGIA' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'ATTACHED'
GO
EXEC sys.sp_addextendedproperty @name=N'ATTACHED_INTERACCION_ID', @value=N'RELACION CON LA TABLA INTERACCION (INTERACCION_ID)' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'ATTACHED'
GO
EXEC sys.sp_addextendedproperty @name=N'CAMPANA_INTERACCION_ID', @value=N'RELACION CON LA TABLA INTERACCION (INTERACCION_ID)' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'CAMPANA'
GO
EXEC sys.sp_addextendedproperty @name=N'FIELD', @value=N'XML CON TODOS LOS CAMPOS DE LA CALLING LIST' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'CAMPANA'
GO
EXEC sys.sp_addextendedproperty @name=N'GUID', @value=N'IDENTIFICADOR DE LA INTERACCION DEL REGISTRO DE CAMPAÑA' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'CAMPANA'
GO
EXEC sys.sp_addextendedproperty @name=N'CHAT_INTERACCION_ID', @value=N'RELACION CON LA TABLA INTERACCION (INTERACCION_ID)' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'CHAT'
GO
EXEC sys.sp_addextendedproperty @name=N'CLIENTE_INTERACCION_ID', @value=N'RELACION CON LA TABLA INTERACCION (INTERACCION_ID)' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'CLIENTE'
GO
EXEC sys.sp_addextendedproperty @name=N'TIPO_DATO', @value=N'1=CHECKBOX / 2=COMBOBOX / 3=TEXTBOX / 4 = RADIOBUTTON' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'DICCIONARIO'
GO
EXEC sys.sp_addextendedproperty @name=N'VISTA', @value=N'ES EL ARBOL DE TIPIFICACION A MOSTRAR' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'DICCIONARIO'
GO
EXEC sys.sp_addextendedproperty @name=N'INTERACTION_RRSS_ID', @value=N'IDENTIFICADOR DE LA INTERACCION DE RRSS' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'RRSS'
GO
EXEC sys.sp_addextendedproperty @name=N'NICKNAME', @value=N'USUARIO DE FACEBOOK CLIENTE' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'RRSS'
GO
EXEC sys.sp_addextendedproperty @name=N'RRSS_INTERACCION_ID', @value=N'RELACION CON LA TABLA INTERACCION (INTERACCION_ID)' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'RRSS'
GO
EXEC sys.sp_addextendedproperty @name=N'TIPO_RRSS', @value=N'1= FACEBOOK / 2=TWITTER' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'RRSS'
GO
EXEC sys.sp_addextendedproperty @name=N'TIPIFICACION_DICCIONARIO_ID', @value=N'RELACION CON LA TABLA DICCIONARIO (DICCIONARIO_ID)' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'TIPIFICACION'
GO
EXEC sys.sp_addextendedproperty @name=N'TIPIFICACION_INTERACCION_ID', @value=N'RELACION CON LA TABLA INTERACCION (INTERACCION_ID)' , @level0type=N'SCHEMA',@level0name=N'wde', @level1type=N'TABLE',@level1name=N'TIPIFICACION'
GO
