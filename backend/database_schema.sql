IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [user_addresses] (
    [user_address_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [full_name] nvarchar(255) NOT NULL,
    [phone] nvarchar(20) NOT NULL,
    [address_line] nvarchar(500) NOT NULL,
    [is_default] bit NOT NULL,
    [created_at] datetime2 NOT NULL,
    CONSTRAINT [PK_user_addresses] PRIMARY KEY ([user_address_id]),
    CONSTRAINT [FK_user_addresses_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_user_addresses_user_id] ON [user_addresses] ([user_id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260525150701_AddUserAddresses', N'9.0.0');

ALTER TABLE [product_variants] ADD [variant_name] nvarchar(100) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260526154146_AddVariantName', N'9.0.0');

ALTER TABLE [products] ADD [tier_variations_json] nvarchar(max) NULL;

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[product_variants]') AND [c].[name] = N'variant_name');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [product_variants] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [product_variants] ALTER COLUMN [variant_name] nvarchar(200) NOT NULL;

ALTER TABLE [product_variants] ADD [tier_values_json] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260526161016_AddTierVariations', N'9.0.0');

ALTER TABLE [users] ADD [is_deleted] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [products] ADD [is_deleted] bit NOT NULL DEFAULT CAST(0 AS bit);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530183950_AddSoftDelete', N'9.0.0');

DROP INDEX [IX_categories_slug] ON [categories];

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[categories]') AND [c].[name] = N'slug');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [categories] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [categories] DROP COLUMN [slug];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530204011_RemoveCategorySlug', N'9.0.0');

ALTER TABLE [orders] ADD [payment_method] nvarchar(50) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260530214506_AddPaymentMethod', N'9.0.0');

COMMIT;
GO

