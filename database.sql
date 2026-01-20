/*
    WORKSHOP PLATFORM - TAM VE TEMİZ VERİTABANI KURULUM SCRİPTİ
    Bu script:
    1. WorkshopPlatformDb veritabanını oluşturur (yoksa).
    2. Gerekli TÜM tabloları (Users, Workshops, Seanslar, Katilimcilar, Odemeler) eksiksiz oluşturur.
    3. İlişkileri (Foreign Key) ve kısıtlamaları tanımlar.
    4. Demo veri EKLEMEZ (Temiz kurulum).
*/

-- 1. Veritabanı Oluşturma
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WorkshopPlatformDb')
BEGIN
    CREATE DATABASE WorkshopPlatformDb;
    PRINT 'Veritabanı oluşturuldu.';
END
GO

USE WorkshopPlatformDb;
GO

-- 2. Tablo Oluşturma

-- USERS TABLOSU
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [AdSoyad] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [Telefon] NVARCHAR(20) NULL,
        [ProfileImage] NVARCHAR(255) NULL, -- New Column
        [Password] NVARCHAR(100) NOT NULL,
        [Role] INT NOT NULL, -- 1: Student, 2: Instructor, 3: Admin
        [ResetToken] NVARCHAR(100) NULL,
        [ResetTokenExpiry] DATETIME2 NULL,
        [OlusturulmaTarihi] DATETIME2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
    PRINT 'Users tablosu oluşturuldu.';
END
GO

-- WORKSHOPS TABLOSU
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Workshops]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Workshops] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Baslik] NVARCHAR(200) NOT NULL,
        [Aciklama] NVARCHAR(MAX) NULL,
        [Kategori] NVARCHAR(100) NULL,
        [UcretliMi] BIT NOT NULL,
        [Fiyat] DECIMAL(18,2) NULL,
        [OlusturanKullaniciId] INT NOT NULL,
        [OlusturulmaTarihi] DATETIME2 NOT NULL DEFAULT (GETDATE()),
        [Durum] INT NOT NULL,
        [Sehir] NVARCHAR(100) NULL,
        [Ilce] NVARCHAR(100) NULL,
        [MekanAd] NVARCHAR(200) NULL,
        [Enlem] DECIMAL(10,8) NULL,
        [Boylam] DECIMAL(11,8) NULL,
        CONSTRAINT [PK_Workshops] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Workshops_Users_OlusturanKullaniciId] FOREIGN KEY ([OlusturanKullaniciId]) REFERENCES [Users] ([Id])
    );
    PRINT 'Workshops tablosu oluşturuldu.';
END
GO

-- WORKSHOP SEANSLARI TABLOSU
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WorkshopSeanslari]') AND type in (N'U'))
BEGIN
    CREATE TABLE [WorkshopSeanslari] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [WorkshopId] INT NOT NULL,
        [Tarih] DATETIME2 NOT NULL,
        [Saat] TIME NOT NULL,
        [Kontenjan] INT NOT NULL,
        [KalanKontenjan] INT NOT NULL,
        CONSTRAINT [PK_WorkshopSeanslari] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WorkshopSeanslari_Workshops_WorkshopId] FOREIGN KEY ([WorkshopId]) REFERENCES [Workshops] ([Id]) ON DELETE CASCADE
    );
    PRINT 'WorkshopSeanslari tablosu oluşturuldu.';
END
GO

-- WORKSHOP KATILIMCILARI TABLOSU
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WorkshopKatilimcilari]') AND type in (N'U'))
BEGIN
    CREATE TABLE [WorkshopKatilimcilari] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [KullaniciId] INT NOT NULL,
        [WorkshopSeansId] INT NOT NULL,
        [KatilimTarihi] DATETIME2 NOT NULL DEFAULT (GETDATE()),
        [KatilimDurumu] INT NOT NULL,
        CONSTRAINT [PK_WorkshopKatilimcilari] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WorkshopKatilimcilari_Users_KullaniciId] FOREIGN KEY ([KullaniciId]) REFERENCES [Users] ([Id]),
        CONSTRAINT [FK_WorkshopKatilimcilari_WorkshopSeanslari_WorkshopSeansId] FOREIGN KEY ([WorkshopSeansId]) REFERENCES [WorkshopSeanslari] ([Id])
    );
    PRINT 'WorkshopKatilimcilari tablosu oluşturuldu.';
END
GO

-- ODEMELER TABLOSU
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Odemeler]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Odemeler] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [KullaniciId] INT NOT NULL,
        [WorkshopId] INT NOT NULL,
        [Tutar] DECIMAL(18,2) NOT NULL,
        [OdemeDurumu] INT NOT NULL,
        [OdemeTarihi] DATETIME2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_Odemeler] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Odemeler_Users_KullaniciId] FOREIGN KEY ([KullaniciId]) REFERENCES [Users] ([Id]),
        CONSTRAINT [FK_Odemeler_Workshops_WorkshopId] FOREIGN KEY ([WorkshopId]) REFERENCES [Workshops] ([Id])
    );
    PRINT 'Odemeler tablosu oluşturuldu.';
END
GO

-- 7. Mevcut Veritabanı Güncellemeleri (Migrations)
-- Eğer tablolar zaten varsa ama yeni sütunlar eksikse ekle

-- Users: ProfileImage
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Users]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'ProfileImage')
    BEGIN
        ALTER TABLE [Users] ADD [ProfileImage] NVARCHAR(255) NULL;
        PRINT 'Users tablosuna ProfileImage sütunu eklendi.';
    END
END

-- Users: Password
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Users]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'Password')
    BEGIN
        ALTER TABLE [Users] ADD [Password] NVARCHAR(100) NOT NULL DEFAULT 'password';
        PRINT 'Users tablosuna Password sütunu eklendi.';
    END
END

-- Users: Role
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Users]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'Role')
    BEGIN
        ALTER TABLE [Users] ADD [Role] INT NOT NULL DEFAULT 1;
        PRINT 'Users tablosuna Role sütunu eklendi.';
    END
END

-- Users: ResetToken ⚓✨
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Users]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'ResetToken')
    BEGIN
        ALTER TABLE [Users] ADD [ResetToken] NVARCHAR(100) NULL;
        PRINT 'Users tablosuna ResetToken sütunu eklendi.';
    END
END

-- Users: ResetTokenExpiry ⚓✨
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Users]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'ResetTokenExpiry')
    BEGIN
        ALTER TABLE [Users] ADD [ResetTokenExpiry] DATETIME2 NULL;
        PRINT 'Users tablosuna ResetTokenExpiry sütunu eklendi.';
    END
END

-- Workshops: Location Columns ⚓🗺️
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Workshops]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Workshops]') AND name = 'Sehir')
        ALTER TABLE [Workshops] ADD [Sehir] NVARCHAR(100) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Workshops]') AND name = 'Ilce')
        ALTER TABLE [Workshops] ADD [Ilce] NVARCHAR(100) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Workshops]') AND name = 'MekanAd')
        ALTER TABLE [Workshops] ADD [MekanAd] NVARCHAR(200) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Workshops]') AND name = 'Enlem')
        ALTER TABLE [Workshops] ADD [Enlem] DECIMAL(10,8) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Workshops]') AND name = 'Boylam')
        ALTER TABLE [Workshops] ADD [Boylam] DECIMAL(11,8) NULL;
    PRINT 'Workshops tablosuna Konum sütunları eklendi.';
END
GO

PRINT '---------------------------------------------------';
PRINT 'VERİTABANI KURULUMU BAŞARIYLA TAMAMLANDI.';
PRINT 'TÜM TABLOLAR EKSİKSİZ MEVCUT.';
PRINT '---------------------------------------------------';
GO
