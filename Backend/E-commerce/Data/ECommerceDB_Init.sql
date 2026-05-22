
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ECommerceDB')
BEGIN
    ALTER DATABASE ECommerceDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ECommerceDB;
END
GO

CREATE DATABASE ECommerceDB;
GO

USE ECommerceDB;
GO

CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId]    nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32)  NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

--  TABLES 

CREATE TABLE [Roles] (
    [Id]   uniqueidentifier NOT NULL,
    [Name] nvarchar(50)     NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Categories] (
    [Id]   uniqueidentifier NOT NULL,
    [Name] nvarchar(100)    NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Brands] (
    [Id]   uniqueidentifier NOT NULL,
    [Name] nvarchar(150)    NOT NULL,
    CONSTRAINT [PK_Brands] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PaymentMethods] (
    [Id]          uniqueidentifier NOT NULL,
    [Name]        nvarchar(100)    NOT NULL,
    [Description] nvarchar(255)    NULL,
    [IsActive]    bit              NOT NULL DEFAULT 1,
    CONSTRAINT [PK_PaymentMethods] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id]          uniqueidentifier NOT NULL,
    [Name]        nvarchar(100)    NOT NULL,
    [Password]    nvarchar(100)    NOT NULL,
    [Email]       nvarchar(100)    NOT NULL,
    [FullName]    nvarchar(100)    NULL,
    [PhoneNumber] nvarchar(10)     NULL,
    [CreatedDate] datetime2        NOT NULL DEFAULT GETDATE(),
    [totalSpend]  decimal(18,2)    NOT NULL DEFAULT 0,
    [RoleId]      uniqueidentifier NOT NULL,
    [IsDeleted]   bit              NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id])
);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

CREATE TABLE [Vouchers] (
    [Id]                uniqueidentifier NOT NULL,
    [Code]              nvarchar(50)     NOT NULL,
    [DiscountType]      int              NOT NULL,  -- 0=Percentage, 1=FixedAmount
    [DiscountValue]     decimal(18,2)    NOT NULL DEFAULT 0,
    [MinOrderAmount]    decimal(18,2)    NOT NULL DEFAULT 0,
    [MaxDiscountAmount] decimal(18,2)    NOT NULL DEFAULT 0,
    [TotalQuantity]     int              NOT NULL DEFAULT 0,
    [UsedCount]         int              NOT NULL DEFAULT 0,
    [StartDate]         datetime2        NOT NULL,
    [EndDate]           datetime2        NOT NULL,
    [IsActive]          bit              NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Vouchers] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Vouchers_Code] ON [Vouchers] ([Code]);
GO

CREATE TABLE [Products] (
    [Id]            uniqueidentifier NOT NULL,
    [Name]          nvarchar(100)    NOT NULL,
    [Description]   nvarchar(200)    NULL,
    [Price]         decimal(18,2)    NOT NULL DEFAULT 0,
    [AverageRating] float            NOT NULL DEFAULT 5.0,
    [CategoryId]    uniqueidentifier NOT NULL,
    [BrandId]       uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id]),
    CONSTRAINT [FK_Products_Brands]     FOREIGN KEY ([BrandId])    REFERENCES [Brands]([Id])
);
GO

CREATE TABLE [ProductVariants] (
    [Id]        uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [Name]      nvarchar(max)    NOT NULL,
    [Price]     decimal(18,2)    NOT NULL DEFAULT 0,
    [Quantity]  int              NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ProductVariants] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariants_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ProductImages] (
    [ProductImageId] uniqueidentifier NOT NULL,
    [ProductId]      uniqueidentifier NOT NULL,
    [ImageUrl]       nvarchar(max)    NOT NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY ([ProductImageId]),
    CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Carts] (
    [Id]        uniqueidentifier NOT NULL,
    [UserId]    uniqueidentifier NOT NULL,
    [CreatedAt] datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Carts_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]);
GO

CREATE TABLE [CartItems] (
    [Id]               uniqueidentifier NOT NULL,
    [CartId]           uniqueidentifier NOT NULL,
    [ProductVariantId] uniqueidentifier NOT NULL,
    [Quantity]         int              NOT NULL DEFAULT 1,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_Carts]           FOREIGN KEY ([CartId])           REFERENCES [Carts]([Id])          ON DELETE CASCADE,
    CONSTRAINT [FK_CartItems_ProductVariants] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants]([Id])
);
GO

CREATE TABLE [Orders] (
    [Id]              uniqueidentifier NOT NULL,
    [OrderDate]       datetime2        NOT NULL DEFAULT GETUTCDATE(),
    [SubTotal]        decimal(18,2)    NOT NULL DEFAULT 0,
    [DiscountAmount]  decimal(18,2)    NOT NULL DEFAULT 0,
    [TotalAmount]     decimal(18,2)    NOT NULL DEFAULT 0,
    [Status]          int              NOT NULL DEFAULT 0,  -- 0=Pending,1=Processing,2=Shipped,3=Delivered,4=Cancelled
    [UserId]          uniqueidentifier NOT NULL,
    [ShippingAddress] nvarchar(255)    NOT NULL,
    [PaymentMethodId] uniqueidentifier NOT NULL,
    [VoucherId]       uniqueidentifier NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users]          FOREIGN KEY ([UserId])          REFERENCES [Users]([Id])          ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_PaymentMethods] FOREIGN KEY ([PaymentMethodId]) REFERENCES [PaymentMethods]([Id]),
    CONSTRAINT [FK_Orders_Vouchers]       FOREIGN KEY ([VoucherId])       REFERENCES [Vouchers]([Id])
);
GO

CREATE TABLE [OrderDetails] (
    [Id]               uniqueidentifier NOT NULL,
    [OrderId]          uniqueidentifier NOT NULL,
    [ProductVariantId] uniqueidentifier NOT NULL,
    [OrderQuantity]    int              NOT NULL DEFAULT 1,
    [UnitPrice]        decimal(18,2)    NOT NULL DEFAULT 0,
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderDetails_Orders]          FOREIGN KEY ([OrderId])          REFERENCES [Orders]([Id])          ON DELETE CASCADE,
    CONSTRAINT [FK_OrderDetails_ProductVariants] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants]([Id])
);
GO

CREATE TABLE [Reviews] (
    [Id]            uniqueidentifier NOT NULL,
    [ProductId]     uniqueidentifier NOT NULL,
    [UserId]        uniqueidentifier NOT NULL,
    [OrderDetailId] uniqueidentifier NULL,
    [Rating]        int              NOT NULL DEFAULT 5,
    [Comment]       nvarchar(500)    NULL,
    [CreatedDate]   datetime2        NOT NULL DEFAULT GETUTCDATE(),
    [Image]         nvarchar(max)    NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_Products]     FOREIGN KEY ([ProductId])     REFERENCES [Products]([Id])     ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Users]        FOREIGN KEY ([UserId])        REFERENCES [Users]([Id])        ON DELETE NO ACTION,
    CONSTRAINT [FK_Reviews_OrderDetails] FOREIGN KEY ([OrderDetailId]) REFERENCES [OrderDetails]([Id])
);
GO

CREATE TABLE [SupportRequests] (
    [Id]          uniqueidentifier NOT NULL,
    [UserId]      uniqueidentifier NOT NULL,
    [StaffId]     uniqueidentifier NULL,
    [OrderId]     uniqueidentifier NULL,
    [Subject]     nvarchar(150)    NOT NULL,
    [Message]     nvarchar(1000)   NOT NULL,
    [Status]      nvarchar(50)     NOT NULL DEFAULT 'Pending',
    [CreatedDate] datetime2        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SupportRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SupportRequests_Users_Customer] FOREIGN KEY ([UserId])  REFERENCES [Users]([Id])  ON DELETE NO ACTION,
    CONSTRAINT [FK_SupportRequests_Users_Staff]    FOREIGN KEY ([StaffId]) REFERENCES [Users]([Id])  ON DELETE NO ACTION,
    CONSTRAINT [FK_SupportRequests_Orders]         FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]) ON DELETE NO ACTION
);
GO

-- ==================== SEED DATA ====================

-- ROLES (3)
INSERT INTO [Roles] ([Id], [Name]) VALUES
    ('11111111-1111-1111-1111-111111111111', 'Admin'),
    ('22222222-2222-2222-2222-222222222222', 'Staff'),
    ('33333333-3333-3333-3333-333333333333', 'Customer');
GO

-- CATEGORIES (8)
INSERT INTO [Categories] ([Id], [Name]) VALUES
    ('01000001-0000-0000-0000-000000000000', N'Điện thoại'),
    ('01000002-0000-0000-0000-000000000000', N'Laptop'),
    ('01000003-0000-0000-0000-000000000000', N'Máy tính bảng'),
    ('01000004-0000-0000-0000-000000000000', N'Tai nghe'),
    ('01000005-0000-0000-0000-000000000000', N'Đồng hồ thông minh'),
    ('01000006-0000-0000-0000-000000000000', N'Phụ kiện'),
    ('01000007-0000-0000-0000-000000000000', N'Màn hình'),
    ('01000008-0000-0000-0000-000000000000', N'Bàn phím & Chuột');
GO

-- BRANDS (6)
INSERT INTO [Brands] ([Id], [Name]) VALUES
    ('02000001-0000-0000-0000-000000000000', 'Apple'),
    ('02000002-0000-0000-0000-000000000000', 'Samsung'),
    ('02000003-0000-0000-0000-000000000000', 'Dell'),
    ('02000004-0000-0000-0000-000000000000', 'Sony'),
    ('02000005-0000-0000-0000-000000000000', 'LG'),
    ('02000006-0000-0000-0000-000000000000', 'Logitech');
GO

-- PAYMENT METHODS (5)
INSERT INTO [PaymentMethods] ([Id], [Name], [Description], [IsActive]) VALUES
    ('04000001-0000-0000-0000-000000000000', N'Thanh toán khi nhận hàng (COD)', N'Thanh toán bằng tiền mặt khi nhận hàng',          1),
    ('04000002-0000-0000-0000-000000000000', 'VNPay',                            N'Thanh toán qua cổng VNPay (ATM, Visa, Master)',    1),
    ('04000003-0000-0000-0000-000000000000', 'MoMo',                             N'Thanh toán qua ví điện tử MoMo',                  1),
    ('04000004-0000-0000-0000-000000000000', 'ZaloPay',                          N'Thanh toán qua ví điện tử ZaloPay',               0),
    ('04000005-0000-0000-0000-000000000000', N'Chuyển khoản ngân hàng',          N'Chuyển khoản trực tiếp qua tài khoản ngân hàng',  1);
GO

-- USERS: 1 Admin + 4 Staff + 6 Customer
-- Password lưu plain-text, hash bằng BCrypt ở Phase 3
INSERT INTO [Users] ([Id], [Name], [Password], [Email], [FullName], [PhoneNumber], [CreatedDate], [totalSpend], [RoleId], [IsDeleted]) VALUES
    /*Admin pass: Admin@123 */
    ('03000001-0000-0000-0000-000000000000', 'admin',        '$2a$11$32bqO8TCC6vsw13lwlqF9.BulFvsC/SnDcQqSK7DVvKQqkXVVycVC',    'admin@shop.com',        N'Admin Shop',     '0000000000', '2022-01-01', 0,        '11111111-1111-1111-1111-111111111111',0),
    /*Staff pass: Staff@123 */
    ('03000002-0000-0000-0000-000000000000', 'dinhvangiang', '$2a$11$ezz5jur0577zKjXK.8wbEeAoKaRCD9kM3fPFHK4b7f5GzZMyREkY.',    'dinhvangiang@shop.com', N'Đinh Văn Giang', '0967890123', '2022-05-10', 0,        '22222222-2222-2222-2222-222222222222',0),
    ('03000003-0000-0000-0000-000000000000', 'ngothiha',     '$2a$11$ezz5jur0577zKjXK.8wbEeAoKaRCD9kM3fPFHK4b7f5GzZMyREkY.',    'ngothiha@shop.com',     N'Ngô Thị Hà',     '0978901234', '2022-08-15', 0,        '22222222-2222-2222-2222-222222222222',0),
    ('03000004-0000-0000-0000-000000000000', 'trinhminhich', '$2a$11$ezz5jur0577zKjXK.8wbEeAoKaRCD9kM3fPFHK4b7f5GzZMyREkY.',    'trinhminhich@shop.com', N'Trịnh Minh Ích', '0989012345', '2023-01-20', 0,        '22222222-2222-2222-2222-222222222222',0),
    ('03000005-0000-0000-0000-000000000000', 'buithikim',    '$2a$11$ezz5jur0577zKjXK.8wbEeAoKaRCD9kM3fPFHK4b7f5GzZMyREkY.',    'buithikim@shop.com',    N'Bùi Thị Kim',    '0990123456', '2023-07-01', 0,        '22222222-2222-2222-2222-222222222222',0),
    /*Customer pass: Customer@123 */
    ('03000006-0000-0000-0000-000000000000', 'nguyenvanan',  '$2a$11$36XTo6G.9ZDcEDc7WMbnmuTLhLUDF8tYgtx2co699f2M/bn12JHKq', 'nguyenvanan@gmail.com', N'Nguyễn Văn An',  '0901234567', '2023-03-15', 45600000, '33333333-3333-3333-3333-333333333333',0),
    ('03000007-0000-0000-0000-000000000000', 'tranthibich',  '$2a$11$36XTo6G.9ZDcEDc7WMbnmuTLhLUDF8tYgtx2co699f2M/bn12JHKq', 'tranthibich@gmail.com', N'Trần Thị Bích',  '0912345678', '2023-06-20', 28900000, '33333333-3333-3333-3333-333333333333',0),
    ('03000008-0000-0000-0000-000000000000', 'leminhcuong',  '$2a$11$36XTo6G.9ZDcEDc7WMbnmuTLhLUDF8tYgtx2co699f2M/bn12JHKq', 'leminhcuong@gmail.com', N'Lê Minh Cường',  '0923456789', '2023-09-10', 67800000, '33333333-3333-3333-3333-333333333333',0),
    ('03000009-0000-0000-0000-000000000000', 'phamthudung',  '$2a$11$36XTo6G.9ZDcEDc7WMbnmuTLhLUDF8tYgtx2co699f2M/bn12JHKq', 'phamthudung@gmail.com', N'Phạm Thu Dung',  '0934567890', '2024-01-05', 15200000, '33333333-3333-3333-3333-333333333333',0),
    ('03000010-0000-0000-0000-000000000000', 'hoangducem',   '$2a$11$36XTo6G.9ZDcEDc7WMbnmuTLhLUDF8tYgtx2co699f2M/bn12JHKq', 'hoangducem@gmail.com',  N'Hoàng Đức Em',   '0945678901', '2024-02-18',  6990000, '33333333-3333-3333-3333-333333333333',0),
    ('03000011-0000-0000-0000-000000000000', 'vothiphuong',  '$2a$11$36XTo6G.9ZDcEDc7WMbnmuTLhLUDF8tYgtx2co699f2M/bn12JHKq', 'vothiphuong@gmail.com', N'Võ Thị Phương',  '0956789012', '2023-11-30', 38500000, '33333333-3333-3333-3333-333333333333',0);
GO

-- VOUCHERS (5)
-- DiscountType: 0=Percentage, 1=FixedAmount
INSERT INTO [Vouchers] ([Id], [Code], [DiscountType], [DiscountValue], [MinOrderAmount], [MaxDiscountAmount], [TotalQuantity], [UsedCount], [StartDate], [EndDate], [IsActive]) VALUES
    ('0c000001-0000-0000-0000-000000000000', 'SALE10',    0,  10.00,  5000000, 1000000, 100,  42, '2024-04-01', '2024-04-30', 1),
    ('0c000002-0000-0000-0000-000000000000', 'FREESHIP',  1,  30000,   500000,   30000, 200,  87, '2024-04-01', '2024-05-31', 1),
    ('0c000003-0000-0000-0000-000000000000', 'WELCOME50', 1,  50000,   200000,   50000, 500, 120, '2024-01-01', '2024-12-31', 1),
    ('0c000004-0000-0000-0000-000000000000', 'SUMMER20',  0,  20.00, 10000000, 2000000,  50,  50, '2024-03-01', '2024-03-31', 0),
    ('0c000005-0000-0000-0000-000000000000', 'VIP15',     0,  15.00, 20000000, 3000000,  30,   5, '2024-05-01', '2024-05-31', 1);
GO

-- PRODUCTS (10)
INSERT INTO [Products] ([Id], [Name], [Description], [Price], [AverageRating], [CategoryId], [BrandId]) VALUES
    ('05000001-0000-0000-0000-000000000000', N'iPhone 15 Pro Max',
     N'iPhone 15 Pro Max với chip A17 Pro mạnh mẽ, camera 48MP chuyên nghiệp, màn hình Super Retina XDR 6.7 inch và khung titanium cao cấp.',
     29990000, 4.8, '01000001-0000-0000-0000-000000000000', '02000001-0000-0000-0000-000000000000'),

    ('05000002-0000-0000-0000-000000000000', N'Samsung Galaxy S24 Ultra',
     N'Samsung Galaxy S24 Ultra với bút S Pen tích hợp, camera 200MP, màn hình Dynamic AMOLED 2X 6.8 inch và chip Snapdragon 8 Gen 3.',
     31990000, 4.7, '01000001-0000-0000-0000-000000000000', '02000002-0000-0000-0000-000000000000'),

    ('05000003-0000-0000-0000-000000000000', N'MacBook Pro 14 inch M3 Pro',
     N'MacBook Pro 14 inch với chip M3 Pro, màn hình Liquid Retina XDR, pin lên đến 18 giờ và hiệu năng chuyên nghiệp vượt trội.',
     49990000, 4.9, '01000002-0000-0000-0000-000000000000', '02000001-0000-0000-0000-000000000000'),

    ('05000004-0000-0000-0000-000000000000', N'Dell XPS 15 9530',
     N'Dell XPS 15 với màn hình OLED 3.5K, Intel Core i7 thế hệ 13, card đồ họa RTX 4060 và thiết kế cao cấp mỏng nhẹ.',
     42990000, 4.5, '01000002-0000-0000-0000-000000000000', '02000003-0000-0000-0000-000000000000'),

    ('05000005-0000-0000-0000-000000000000', N'AirPods Pro 2',
     N'AirPods Pro thế hệ 2 với chip H2, chống ồn chủ động thế hệ mới, âm thanh Adaptive Audio và hộp sạc MagSafe.',
      6990000, 4.8, '01000004-0000-0000-0000-000000000000', '02000001-0000-0000-0000-000000000000'),

    ('05000006-0000-0000-0000-000000000000', N'Sony WH-1000XM5',
     N'Sony WH-1000XM5 với chống ồn hàng đầu thế giới, âm thanh Hi-Res Audio, 30 giờ pin và thiết kế thoải mái.',
      8490000, 4.7, '01000004-0000-0000-0000-000000000000', '02000004-0000-0000-0000-000000000000'),

    ('05000007-0000-0000-0000-000000000000', N'Apple Watch Series 9 45mm',
     N'Apple Watch Series 9 với chip S9, màn hình Always-On Retina, Double Tap gesture và theo dõi sức khỏe toàn diện.',
     11990000, 4.6, '01000005-0000-0000-0000-000000000000', '02000001-0000-0000-0000-000000000000'),

    ('05000008-0000-0000-0000-000000000000', N'iPad Pro 12.9 inch M2',
     N'iPad Pro 12.9 inch với chip M2, màn hình Liquid Retina XDR mini-LED, kết nối Thunderbolt và Apple Pencil Pro.',
     28990000, 4.8, '01000003-0000-0000-0000-000000000000', '02000001-0000-0000-0000-000000000000'),

    ('05000009-0000-0000-0000-000000000000', N'Logitech MX Master 3S',
     N'Chuột không dây Logitech MX Master 3S với cuộn MagSpeed, 8000 DPI, pin 70 ngày và kết nối đa thiết bị.',
      2490000, 4.7, '01000008-0000-0000-0000-000000000000', '02000006-0000-0000-0000-000000000000'),

    ('05000010-0000-0000-0000-000000000000', N'LG UltraWide 34WN80C',
     N'Màn hình LG UltraWide 34 inch IPS, độ phân giải WQHD 3440x1440, cổng USB-C 96W và HDR10.',
     12990000, 4.5, '01000007-0000-0000-0000-000000000000', '02000005-0000-0000-0000-000000000000');
GO

-- PRODUCT VARIANTS (25 total)
-- iPhone 15 Pro Max (4)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000001-0000-0000-0000-000000000000', '05000001-0000-0000-0000-000000000000', N'256GB - Đen Titan',    29990000, 15),
    ('06000002-0000-0000-0000-000000000000', '05000001-0000-0000-0000-000000000000', N'256GB - Trắng Titan',  29990000, 10),
    ('06000003-0000-0000-0000-000000000000', '05000001-0000-0000-0000-000000000000', N'512GB - Đen Titan',    33990000, 12),
    ('06000004-0000-0000-0000-000000000000', '05000001-0000-0000-0000-000000000000', N'1TB - Titan Tự nhiên', 39990000,  8);
GO

-- Samsung Galaxy S24 Ultra (3)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000005-0000-0000-0000-000000000000', '05000002-0000-0000-0000-000000000000', N'256GB - Đen',       31990000, 10),
    ('06000006-0000-0000-0000-000000000000', '05000002-0000-0000-0000-000000000000', N'512GB - Xám Titan', 35990000, 12),
    ('06000007-0000-0000-0000-000000000000', '05000002-0000-0000-0000-000000000000', N'1TB - Vàng Titan',  41990000,  8);
GO

-- MacBook Pro 14 M3 Pro (3)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000008-0000-0000-0000-000000000000', '05000003-0000-0000-0000-000000000000', N'M3 Pro 18GB/512GB - Đen vũ trụ', 49990000, 8),
    ('06000009-0000-0000-0000-000000000000', '05000003-0000-0000-0000-000000000000', N'M3 Pro 18GB/1TB - Bạc',          57990000, 7),
    ('06000010-0000-0000-0000-000000000000', '05000003-0000-0000-0000-000000000000', N'M3 Max 36GB/1TB - Đen vũ trụ',   74990000, 5);
GO

-- Dell XPS 15 9530 (2)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000011-0000-0000-0000-000000000000', '05000004-0000-0000-0000-000000000000', N'i7/16GB/512GB/RTX4060', 42990000, 8),
    ('06000012-0000-0000-0000-000000000000', '05000004-0000-0000-0000-000000000000', N'i9/32GB/1TB/RTX4070',   55990000, 7);
GO

-- AirPods Pro 2 (2)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000013-0000-0000-0000-000000000000', '05000005-0000-0000-0000-000000000000', N'Trắng - USB-C',     6990000, 50),
    ('06000014-0000-0000-0000-000000000000', '05000005-0000-0000-0000-000000000000', N'Trắng - Lightning', 6490000, 30);
GO

-- Sony WH-1000XM5 (2)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000015-0000-0000-0000-000000000000', '05000006-0000-0000-0000-000000000000', N'Đen', 8490000, 30),
    ('06000016-0000-0000-0000-000000000000', '05000006-0000-0000-0000-000000000000', N'Bạc', 8490000, 30);
GO

-- Apple Watch Series 9 (3)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000017-0000-0000-0000-000000000000', '05000007-0000-0000-0000-000000000000', N'45mm - Nhôm Đen / Dây Sport Đen',     11990000, 15),
    ('06000018-0000-0000-0000-000000000000', '05000007-0000-0000-0000-000000000000', N'45mm - Nhôm Bạc / Dây Sport Xanh',    11990000, 12),
    ('06000019-0000-0000-0000-000000000000', '05000007-0000-0000-0000-000000000000', N'45mm - Thép không gỉ / Dây Milanese', 17990000,  8);
GO

-- iPad Pro 12.9 M2 (3)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000020-0000-0000-0000-000000000000', '05000008-0000-0000-0000-000000000000', N'128GB WiFi - Bạc',               28990000, 10),
    ('06000021-0000-0000-0000-000000000000', '05000008-0000-0000-0000-000000000000', N'256GB WiFi+5G - Xám không gian', 35990000,  8),
    ('06000022-0000-0000-0000-000000000000', '05000008-0000-0000-0000-000000000000', N'1TB WiFi - Bạc',                 49990000,  7);
GO

-- Logitech MX Master 3S (2)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000023-0000-0000-0000-000000000000', '05000009-0000-0000-0000-000000000000', N'Đen',   2490000, 60),
    ('06000024-0000-0000-0000-000000000000', '05000009-0000-0000-0000-000000000000', N'Trắng', 2490000, 40);
GO

-- LG UltraWide 34WN80C (1)
INSERT INTO [ProductVariants] ([Id], [ProductId], [Name], [Price], [Quantity]) VALUES
    ('06000025-0000-0000-0000-000000000000', '05000010-0000-0000-0000-000000000000', N'34 inch WQHD', 12990000, 20);
GO

-- PRODUCT IMAGES (1 per product)
INSERT INTO [ProductImages] ([ProductImageId], [ProductId], [ImageUrl]) VALUES
    (NEWID(), '05000001-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=iPhone+15+Pro+Max'),
    (NEWID(), '05000002-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=S24+Ultra'),
    (NEWID(), '05000003-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=MacBook+Pro+14'),
    (NEWID(), '05000004-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=Dell+XPS+15'),
    (NEWID(), '05000005-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=AirPods+Pro+2'),
    (NEWID(), '05000006-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=Sony+WH-1000XM5'),
    (NEWID(), '05000007-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=Apple+Watch+S9'),
    (NEWID(), '05000008-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=iPad+Pro+12.9'),
    (NEWID(), '05000009-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=MX+Master+3S'),
    (NEWID(), '05000010-0000-0000-0000-000000000000', 'https://placehold.co/800x800?text=LG+UltraWide+34');
GO

-- ORDERS (5)
-- Status: 0=Pending, 1=Processing, 2=Shipped, 3=Delivered, 4=Cancelled
INSERT INTO [Orders] ([Id], [OrderDate], [SubTotal], [DiscountAmount], [TotalAmount], [Status], [UserId], [ShippingAddress], [PaymentMethodId], [VoucherId]) VALUES
    ('07000001-0000-0000-0000-000000000000', '2024-04-20', 36980000, 1000000, 35980000, 3,
     '03000006-0000-0000-0000-000000000000', N'123 Nguyễn Huệ, Q1, TP.HCM',
     '04000002-0000-0000-0000-000000000000', '0c000001-0000-0000-0000-000000000000'),  -- VNPay + SALE10

    ('07000002-0000-0000-0000-000000000000', '2024-04-22', 49990000, 0, 49990000, 2,
     '03000008-0000-0000-0000-000000000000', N'789 Trần Hưng Đạo, Q3, TP.HCM',
     '04000001-0000-0000-0000-000000000000', NULL),  -- COD, no voucher

    ('07000003-0000-0000-0000-000000000000', '2024-04-23', 10980000, 500000, 10480000, 1,
     '03000007-0000-0000-0000-000000000000', N'456 Lê Lợi, Q5, TP.HCM',
     '04000003-0000-0000-0000-000000000000', '0c000002-0000-0000-0000-000000000000'),  -- MoMo + FREESHIP

    ('07000004-0000-0000-0000-000000000000', '2024-04-24', 11990000, 0, 11990000, 0,
     '03000009-0000-0000-0000-000000000000', N'321 Điện Biên Phủ, Bình Thạnh, TP.HCM',
     '04000002-0000-0000-0000-000000000000', NULL),  -- VNPay, no voucher

    ('07000005-0000-0000-0000-000000000000', '2024-04-15', 31990000, 0, 31990000, 4,
     '03000011-0000-0000-0000-000000000000', N'987 Hai Bà Trưng, Q1, TP.HCM',
     '04000001-0000-0000-0000-000000000000', NULL);  -- COD, cancelled
GO

-- ORDER DETAILS (7 items)
INSERT INTO [OrderDetails] ([Id], [OrderId], [ProductVariantId], [OrderQuantity], [UnitPrice]) VALUES
    -- ORD-001: iPhone 256GB Đen Titan + AirPods Pro 2 USB-C
    ('08000001-0000-0000-0000-000000000000', '07000001-0000-0000-0000-000000000000', '06000001-0000-0000-0000-000000000000', 1, 29990000),
    ('08000002-0000-0000-0000-000000000000', '07000001-0000-0000-0000-000000000000', '06000013-0000-0000-0000-000000000000', 1,  6990000),
    -- ORD-002: MacBook Pro M3 Pro 18GB/512GB
    ('08000003-0000-0000-0000-000000000000', '07000002-0000-0000-0000-000000000000', '06000008-0000-0000-0000-000000000000', 1, 49990000),
    -- ORD-003: Sony WH-1000XM5 Đen + Logitech MX Master 3S Đen
    ('08000004-0000-0000-0000-000000000000', '07000003-0000-0000-0000-000000000000', '06000015-0000-0000-0000-000000000000', 1,  8490000),
    ('08000005-0000-0000-0000-000000000000', '07000003-0000-0000-0000-000000000000', '06000023-0000-0000-0000-000000000000', 1,  2490000),
    -- ORD-004: Apple Watch Series 9 45mm Nhôm Đen
    ('08000006-0000-0000-0000-000000000000', '07000004-0000-0000-0000-000000000000', '06000017-0000-0000-0000-000000000000', 1, 11990000),
    -- ORD-005: Samsung S24 Ultra 256GB Đen (cancelled)
    ('08000007-0000-0000-0000-000000000000', '07000005-0000-0000-0000-000000000000', '06000005-0000-0000-0000-000000000000', 1, 31990000);
GO

-- REVIEWS (5)
INSERT INTO [Reviews] ([Id], [ProductId], [UserId], [OrderDetailId], [Rating], [Comment], [CreatedDate]) VALUES
    -- Nguyễn Văn An → iPhone (linked to ODD-001)
    ('09000001-0000-0000-0000-000000000000',
     '05000001-0000-0000-0000-000000000000', '03000006-0000-0000-0000-000000000000',
     '08000001-0000-0000-0000-000000000000',
     5, N'Máy chất lượng tuyệt vời, camera cực đỉnh, pin trâu hơn mong đợi. Rất hài lòng!', '2024-04-22'),

    -- Lê Minh Cường → iPhone (không có ODD khớp → NULL)
    ('09000002-0000-0000-0000-000000000000',
     '05000001-0000-0000-0000-000000000000', '03000008-0000-0000-0000-000000000000',
     NULL,
     4, N'Sản phẩm tốt, đóng gói cẩn thận. Chỉ tiếc giá hơi cao nhưng xứng đáng.', '2024-04-10'),

    -- Trần Thị Bích → AirPods Pro 2 (NULL)
    ('09000003-0000-0000-0000-000000000000',
     '05000005-0000-0000-0000-000000000000', '03000007-0000-0000-0000-000000000000',
     NULL,
     5, N'AirPods Pro 2 chống ồn tốt hơn thế hệ trước rất nhiều. Xuất sắc!', '2024-03-15'),

    -- Võ Thị Phương → MacBook Pro (NULL)
    ('09000004-0000-0000-0000-000000000000',
     '05000003-0000-0000-0000-000000000000', '03000011-0000-0000-0000-000000000000',
     NULL,
     5, N'MacBook M3 Pro quá mượt, làm việc cả ngày không nóng, pin ổn định.', '2024-04-01'),

    -- Phạm Thu Dung → Sony WH-1000XM5 (NULL)
    ('09000005-0000-0000-0000-000000000000',
     '05000006-0000-0000-0000-000000000000', '03000009-0000-0000-0000-000000000000',
     NULL,
     4, N'Sony XM5 âm thanh tuyệt vời, chống ồn tốt. Thiết kế đẹp hơn XM4.', '2024-04-05');
GO

-- SUPPORT REQUESTS (3)
-- Status: Pending / InProgress / Resolved
INSERT INTO [SupportRequests] ([Id], [UserId], [StaffId], [OrderId], [Subject], [Message], [Status], [CreatedDate]) VALUES
    ('0a000001-0000-0000-0000-000000000000',
     '03000006-0000-0000-0000-000000000000',  -- Nguyễn Văn An
     '03000003-0000-0000-0000-000000000000',  -- Ngô Thị Hà
     '07000001-0000-0000-0000-000000000000',  -- ORD-001
     N'Hỏi về bảo hành sản phẩm',
     N'Cho tôi hỏi iPhone 15 Pro Max mua ở đây được bảo hành bao lâu và bảo hành ở đâu?',
     'Resolved', '2024-04-22'),

    ('0a000002-0000-0000-0000-000000000000',
     '03000008-0000-0000-0000-000000000000',  -- Lê Minh Cường
     '03000003-0000-0000-0000-000000000000',  -- Ngô Thị Hà
     '07000002-0000-0000-0000-000000000000',  -- ORD-002
     N'Đơn hàng giao lâu',
     N'Tôi đặt hàng 2 ngày rồi mà vẫn chưa nhận được. Khi nào nhận được vậy?',
     'InProgress', '2024-04-24'),

    ('0a000003-0000-0000-0000-000000000000',
     '03000007-0000-0000-0000-000000000000',  -- Trần Thị Bích
     NULL, NULL,
     N'Hỏi về mã giảm giá',
     N'Mã SALE10 của tôi bị báo hết hạn dù chưa đến ngày 30/4?',
     'Pending', '2024-04-25');
GO

-- EF MIGRATIONS HISTORY
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES
    ('20260503000000_InitialCreate', '8.0.0');
GO

-- ==================== SUMMARY ====================
PRINT '=== ECommerceDB initialized successfully ===';
PRINT 'Roles: 3  |  Categories: 8  |  Brands: 6  |  PaymentMethods: 5';
PRINT 'Users: 11 (1 Admin + 4 Staff + 6 Customers)';
PRINT 'Vouchers: 5  |  Products: 10  |  Variants: 25  |  Images: 10';
PRINT 'Orders: 5  |  OrderDetails: 7  |  Reviews: 5  |  SupportRequests: 3';
GO

-- ==================== BACKUP ====================
-- Sửa đường dẫn rồi bỏ comment để backup:
-- BACKUP DATABASE ECommerceDB TO DISK = 'D:\Backup\ECommerceDB.bak' WITH FORMAT, NAME = 'ECommerceDB Full Backup';
-- GO