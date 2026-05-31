CREATE TABLE [categories] (
    [category_id] int NOT NULL IDENTITY,
    [parent_id] int NULL,
    [name] nvarchar(255) NOT NULL,
    [description] nvarchar(max) NULL,
    CONSTRAINT [PK_categories] PRIMARY KEY ([category_id]),
    CONSTRAINT [FK_categories_categories_parent_id] FOREIGN KEY ([parent_id]) REFERENCES [categories] ([category_id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [roles] (
    [role_id] int NOT NULL IDENTITY,
    [role_name] nvarchar(50) NOT NULL,
    [description] nvarchar(255) NULL,
    CONSTRAINT [PK_roles] PRIMARY KEY ([role_id])
);
GO


CREATE TABLE [products] (
    [product_id] int NOT NULL IDENTITY,
    [category_id] int NULL,
    [name] nvarchar(255) NOT NULL,
    [slug] nvarchar(191) NOT NULL,
    [base_description] nvarchar(max) NULL,
    [tier_variations_json] nvarchar(max) NULL,
    [is_deleted] bit NOT NULL,
    [created_at] datetime2 NOT NULL,
    CONSTRAINT [PK_products] PRIMARY KEY ([product_id]),
    CONSTRAINT [FK_products_categories_category_id] FOREIGN KEY ([category_id]) REFERENCES [categories] ([category_id])
);
GO


CREATE TABLE [users] (
    [user_id] int NOT NULL IDENTITY,
    [role_id] int NOT NULL,
    [email] nvarchar(191) NOT NULL,
    [password_hash] nvarchar(255) NOT NULL,
    [full_name] nvarchar(255) NULL,
    [phone] nvarchar(20) NULL,
    [is_active] bit NOT NULL,
    [is_deleted] bit NOT NULL,
    [created_at] datetime2 NOT NULL,
    [updated_at] datetime2 NOT NULL,
    [reset_password_token] nvarchar(255) NULL,
    [reset_password_expiry] datetime2 NULL,
    CONSTRAINT [PK_users] PRIMARY KEY ([user_id]),
    CONSTRAINT [FK_users_roles_role_id] FOREIGN KEY ([role_id]) REFERENCES [roles] ([role_id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [product_variants] (
    [variant_id] int NOT NULL IDENTITY,
    [product_id] int NOT NULL,
    [sku] nvarchar(100) NOT NULL,
    [variant_name] nvarchar(200) NOT NULL,
    [tier_values_json] nvarchar(max) NULL,
    [price] decimal(18,2) NOT NULL,
    [stock_qty] int NOT NULL,
    [image_url] nvarchar(512) NULL,
    CONSTRAINT [PK_product_variants] PRIMARY KEY ([variant_id]),
    CONSTRAINT [FK_product_variants_products_product_id] FOREIGN KEY ([product_id]) REFERENCES [products] ([product_id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [carts] (
    [cart_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [created_at] datetime2 NOT NULL,
    [updated_at] datetime2 NOT NULL,
    CONSTRAINT [PK_carts] PRIMARY KEY ([cart_id]),
    CONSTRAINT [FK_carts_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [orders] (
    [order_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [order_status] nvarchar(50) NOT NULL,
    [total_price] decimal(18,2) NOT NULL,
    [shipping_address] nvarchar(max) NOT NULL,
    [note] nvarchar(max) NULL,
    [created_at] datetime2 NOT NULL,
    [payment_method] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_orders] PRIMARY KEY ([order_id]),
    CONSTRAINT [FK_orders_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]) ON DELETE NO ACTION
);
GO


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
GO


CREATE TABLE [wishlists] (
    [wishlist_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [product_id] int NOT NULL,
    [created_at] datetime2 NOT NULL,
    CONSTRAINT [PK_wishlists] PRIMARY KEY ([wishlist_id]),
    CONSTRAINT [FK_wishlists_products_product_id] FOREIGN KEY ([product_id]) REFERENCES [products] ([product_id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_wishlists_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([user_id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [cart_items] (
    [cart_item_id] int NOT NULL IDENTITY,
    [cart_id] int NOT NULL,
    [variant_id] int NOT NULL,
    [quantity] int NOT NULL,
    [created_at] datetime2 NOT NULL,
    CONSTRAINT [PK_cart_items] PRIMARY KEY ([cart_item_id]),
    CONSTRAINT [FK_cart_items_carts_cart_id] FOREIGN KEY ([cart_id]) REFERENCES [carts] ([cart_id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_cart_items_product_variants_variant_id] FOREIGN KEY ([variant_id]) REFERENCES [product_variants] ([variant_id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [invoices] (
    [invoice_id] int NOT NULL IDENTITY,
    [order_id] int NOT NULL,
    [invoice_number] nvarchar(100) NOT NULL,
    [billing_name] nvarchar(255) NULL,
    [billing_email] nvarchar(191) NULL,
    [tax_code] nvarchar(50) NULL,
    [subtotal] decimal(18,2) NOT NULL,
    [tax_amount] decimal(18,2) NOT NULL,
    [total_amount] decimal(18,2) NOT NULL,
    [payment_status] nvarchar(50) NOT NULL,
    [payment_method] nvarchar(50) NULL,
    [paid_at] datetime2 NULL,
    [created_at] datetime2 NOT NULL,
    CONSTRAINT [PK_invoices] PRIMARY KEY ([invoice_id]),
    CONSTRAINT [FK_invoices_orders_order_id] FOREIGN KEY ([order_id]) REFERENCES [orders] ([order_id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [order_items] (
    [order_item_id] int NOT NULL IDENTITY,
    [order_id] int NOT NULL,
    [variant_id] int NOT NULL,
    [quantity] int NOT NULL,
    [price_at_purchase] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_order_items] PRIMARY KEY ([order_item_id]),
    CONSTRAINT [FK_order_items_orders_order_id] FOREIGN KEY ([order_id]) REFERENCES [orders] ([order_id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_order_items_product_variants_variant_id] FOREIGN KEY ([variant_id]) REFERENCES [product_variants] ([variant_id]) ON DELETE NO ACTION
);
GO


CREATE INDEX [IX_cart_items_cart_id] ON [cart_items] ([cart_id]);
GO


CREATE INDEX [IX_cart_items_variant_id] ON [cart_items] ([variant_id]);
GO


CREATE INDEX [IX_carts_user_id] ON [carts] ([user_id]);
GO


CREATE INDEX [IX_categories_parent_id] ON [categories] ([parent_id]);
GO


CREATE UNIQUE INDEX [IX_invoices_invoice_number] ON [invoices] ([invoice_number]);
GO


CREATE UNIQUE INDEX [IX_invoices_order_id] ON [invoices] ([order_id]);
GO


CREATE INDEX [IX_order_items_order_id] ON [order_items] ([order_id]);
GO


CREATE INDEX [IX_order_items_variant_id] ON [order_items] ([variant_id]);
GO


CREATE INDEX [IX_orders_user_id] ON [orders] ([user_id]);
GO


CREATE INDEX [IX_product_variants_product_id] ON [product_variants] ([product_id]);
GO


CREATE UNIQUE INDEX [IX_product_variants_sku] ON [product_variants] ([sku]);
GO


CREATE INDEX [IX_products_category_id] ON [products] ([category_id]);
GO


CREATE UNIQUE INDEX [IX_products_slug] ON [products] ([slug]);
GO


CREATE INDEX [IX_user_addresses_user_id] ON [user_addresses] ([user_id]);
GO


CREATE INDEX [IX_users_role_id] ON [users] ([role_id]);
GO


CREATE INDEX [IX_wishlists_product_id] ON [wishlists] ([product_id]);
GO


CREATE INDEX [IX_wishlists_user_id] ON [wishlists] ([user_id]);
GO


