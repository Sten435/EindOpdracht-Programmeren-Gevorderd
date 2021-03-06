USE [Fitness]
GO
ALTER TABLE [dbo].[TijdSloten] DROP CONSTRAINT [TijdsSlot_Reservatie_FK]
GO
ALTER TABLE [dbo].[Reservaties] DROP CONSTRAINT [Reservatie_Toestel_FK]
GO
ALTER TABLE [dbo].[Reservaties] DROP CONSTRAINT [Reservatie_Klant_FK]
GO
ALTER TABLE [dbo].[Interesses] DROP CONSTRAINT [Interesses_Klant_FK]
GO
ALTER TABLE [dbo].[Adressen] DROP CONSTRAINT [Adres_Klant_FK]
GO
ALTER TABLE [dbo].[Toestellen] DROP CONSTRAINT [DF__Toestelle__Verwi__5AEE82B9]
GO
ALTER TABLE [dbo].[Toestellen] DROP CONSTRAINT [DF__Toestelle__Inser__44FF419A]
GO
ALTER TABLE [dbo].[Reservaties] DROP CONSTRAINT [DF__tmp_ms_xx__Inser__6FE99F9F]
GO
ALTER TABLE [dbo].[Klanten] DROP CONSTRAINT [DF__Klanten__Inserte__3D5E1FD2]
GO
/****** Object:  Table [dbo].[Toestellen]    Script Date: 1/06/2022 22:11:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Toestellen]') AND type in (N'U'))
DROP TABLE [dbo].[Toestellen]
GO
/****** Object:  Table [dbo].[TijdSloten]    Script Date: 1/06/2022 22:11:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TijdSloten]') AND type in (N'U'))
DROP TABLE [dbo].[TijdSloten]
GO
/****** Object:  Table [dbo].[Reservaties]    Script Date: 1/06/2022 22:11:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reservaties]') AND type in (N'U'))
DROP TABLE [dbo].[Reservaties]
GO
/****** Object:  Table [dbo].[Klanten]    Script Date: 1/06/2022 22:11:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Klanten]') AND type in (N'U'))
DROP TABLE [dbo].[Klanten]
GO
/****** Object:  Table [dbo].[Interesses]    Script Date: 1/06/2022 22:11:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Interesses]') AND type in (N'U'))
DROP TABLE [dbo].[Interesses]
GO
/****** Object:  Table [dbo].[Config]    Script Date: 1/06/2022 22:11:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Config]') AND type in (N'U'))
DROP TABLE [dbo].[Config]
GO
/****** Object:  Table [dbo].[Adressen]    Script Date: 1/06/2022 22:11:12 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Adressen]') AND type in (N'U'))
DROP TABLE [dbo].[Adressen]
GO
/****** Object:  Table [dbo].[Adressen]    Script Date: 1/06/2022 22:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Adressen](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StraatNaam] [varchar](255) NOT NULL,
	[HuisNummer] [varchar](255) NOT NULL,
	[Plaats] [varchar](255) NOT NULL,
	[PostCode] [int] NOT NULL,
	[Klant_KlantenNummer] [int] NOT NULL,
 CONSTRAINT [Adressen_PK] PRIMARY KEY CLUSTERED 
(
	[Klant_KlantenNummer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [Adres_PK] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Config]    Script Date: 1/06/2022 22:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Config](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SlotTijdUur] [bigint] NOT NULL,
	[StandaardInherstelling] [bit] NULL,
	[LowerBoundUurReservatie] [int] NULL,
	[UpperBoundUurReservatie] [int] NULL,
	[AantalDagenInToekomstReserveren] [int] NULL,
 CONSTRAINT [Config_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Interesses]    Script Date: 1/06/2022 22:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Interesses](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Klant_KlantenNummer] [int] NOT NULL,
	[InteresseNaam] [varchar](255) NOT NULL,
 CONSTRAINT [Interesses_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Klanten]    Script Date: 1/06/2022 22:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Klanten](
	[KlantenNummer] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Voornaam] [varchar](255) NOT NULL,
	[Achternaam] [varchar](255) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[GeboorteDatum] [datetime] NOT NULL,
	[InsertedTimestamp] [datetime] NULL,
	[ABONNEMENT] [varchar](255) NOT NULL,
 CONSTRAINT [Klant_PK] PRIMARY KEY CLUSTERED 
(
	[KlantenNummer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reservaties]    Script Date: 1/06/2022 22:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservaties](
	[Klant_KlantenNummer] [int] NOT NULL,
	[ReservatieNummer] [int] IDENTITY(1,1) NOT NULL,
	[Toestel_IdentificatieCode] [int] NOT NULL,
	[InsertedTimestamp] [datetime] NULL,
 CONSTRAINT [Reservatie_PK] PRIMARY KEY CLUSTERED 
(
	[ReservatieNummer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TijdSloten]    Script Date: 1/06/2022 22:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TijdSloten](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StartTijd] [datetime] NOT NULL,
	[EindTijd] [datetime] NOT NULL,
	[Reservatie_ReservatieNummer] [int] NOT NULL,
 CONSTRAINT [TijdsSlot_PK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Toestellen]    Script Date: 1/06/2022 22:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Toestellen](
	[IdentificatieCode] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ToestelType] [varchar](255) NOT NULL,
	[InHerstelling] [bit] NULL,
	[InsertedTimestamp] [datetime] NULL,
	[Verwijderd] [bit] NULL,
 CONSTRAINT [Toestel_PK] PRIMARY KEY CLUSTERED 
(
	[IdentificatieCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Klanten] ADD  DEFAULT (getdate()) FOR [InsertedTimestamp]
GO
ALTER TABLE [dbo].[Reservaties] ADD  DEFAULT (getdate()) FOR [InsertedTimestamp]
GO
ALTER TABLE [dbo].[Toestellen] ADD  DEFAULT (getdate()) FOR [InsertedTimestamp]
GO
ALTER TABLE [dbo].[Toestellen] ADD  DEFAULT ((0)) FOR [Verwijderd]
GO
ALTER TABLE [dbo].[Adressen]  WITH CHECK ADD  CONSTRAINT [Adres_Klant_FK] FOREIGN KEY([Klant_KlantenNummer])
REFERENCES [dbo].[Klanten] ([KlantenNummer])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Adressen] CHECK CONSTRAINT [Adres_Klant_FK]
GO
ALTER TABLE [dbo].[Interesses]  WITH CHECK ADD  CONSTRAINT [Interesses_Klant_FK] FOREIGN KEY([Klant_KlantenNummer])
REFERENCES [dbo].[Klanten] ([KlantenNummer])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Interesses] CHECK CONSTRAINT [Interesses_Klant_FK]
GO
ALTER TABLE [dbo].[Reservaties]  WITH CHECK ADD  CONSTRAINT [Reservatie_Klant_FK] FOREIGN KEY([Klant_KlantenNummer])
REFERENCES [dbo].[Klanten] ([KlantenNummer])
GO
ALTER TABLE [dbo].[Reservaties] CHECK CONSTRAINT [Reservatie_Klant_FK]
GO
ALTER TABLE [dbo].[Reservaties]  WITH CHECK ADD  CONSTRAINT [Reservatie_Toestel_FK] FOREIGN KEY([Toestel_IdentificatieCode])
REFERENCES [dbo].[Toestellen] ([IdentificatieCode])
GO
ALTER TABLE [dbo].[Reservaties] CHECK CONSTRAINT [Reservatie_Toestel_FK]
GO
ALTER TABLE [dbo].[TijdSloten]  WITH CHECK ADD  CONSTRAINT [TijdsSlot_Reservatie_FK] FOREIGN KEY([Reservatie_ReservatieNummer])
REFERENCES [dbo].[Reservaties] ([ReservatieNummer])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TijdSloten] CHECK CONSTRAINT [TijdsSlot_Reservatie_FK]
GO
