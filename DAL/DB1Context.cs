using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using finsyncapi.DAL.Entities;

namespace finsyncapi.DAL;

public partial class DB1Context : DbContext
{
    public DB1Context(DbContextOptions<DB1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountProfile> AccountProfiles { get; set; }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<ExpenseSplit> ExpenseSplits { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupProfile> GroupProfiles { get; set; }

    public virtual DbSet<Ledger> Ledgers { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<SnowflakeConfig> SnowflakeConfigs { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionPayment> TransactionPayments { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "otp_purpose", new[] { "EMAIL_VERIFICATION", "LOGIN", "PASSWORD_RESET" })
            .HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("accounts_pkey");

            entity.ToTable("accounts", "app");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccountTypeId).HasColumnName("account_type_id");
            entity.Property(e => e.Balance)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0.0000")
                .HasColumnName("balance");
            entity.Property(e => e.BalanceAsOf)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("balance_as_of");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'INR'::bpchar")
                .IsFixedLength()
                .HasColumnName("currency_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AccountType).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("accounts_account_type_id_fkey");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Accounts)
                .HasPrincipalKey(p => p.CurrencyCode)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("accounts_currency_code_fkey");
        });

        modelBuilder.Entity<AccountProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("profile_accounts_pkey");

            entity.ToTable("account_profiles", "app");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountProfiles)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("profile_accounts_account_id_fkey");
        });

        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_types_pkey");

            entity.ToTable("account_types", "master");

            entity.HasIndex(e => e.Name, "account_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs", "logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.Changes)
                .HasColumnType("jsonb")
                .HasColumnName("changes");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.RecordId)
                .HasMaxLength(50)
                .HasColumnName("record_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories", "app");

            entity.HasIndex(e => new { e.ProfileId, e.CategoryName, e.TransactionTypeId }, "categories_profile_id_category_name_transaction_type_id_key").IsUnique();

            entity.HasIndex(e => new { e.CategoryName, e.TransactionTypeId }, "uq_default_categories")
                .IsUnique()
                .HasFilter("(is_default = true)");

            entity.HasIndex(e => new { e.ProfileId, e.GroupId, e.CategoryName, e.TransactionTypeId }, "uq_user_categories")
                .IsUnique()
                .HasFilter("(is_default = false)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("master.generate_snowflake_id()")
                .HasColumnName("id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .HasColumnName("is_default");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Group).WithMany(p => p.Categories)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("categories_group_id_fkey");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("categories_parent_category_id_fkey");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Categories)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("categories_transaction_type_id_fkey");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("currencies_pkey");

            entity.ToTable("currencies", "master");

            entity.HasIndex(e => e.CurrencyCode, "currencies_currency_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("currency_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Symbol)
                .HasMaxLength(10)
                .HasColumnName("symbol");
        });

        modelBuilder.Entity<ExpenseSplit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expense_split_pk");

            entity.ToTable("expense_split", "txn");

            entity.HasIndex(e => new { e.TransactionId, e.ProfileId }, "uq_expense_split_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.OwedAmount)
                .HasPrecision(12, 2)
                .HasColumnName("owed_amount");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.Percentage)
                .HasPrecision(5, 2)
                .HasColumnName("percentage");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.Share)
                .HasPrecision(10, 2)
                .HasColumnName("share");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Profile).WithMany(p => p.ExpenseSplits)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_split_profile");

            entity.HasOne(d => d.Transaction).WithMany(p => p.ExpenseSplits)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("fk_split_transaction");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("groups_pkey");

            entity.ToTable("groups", "app");

            entity.HasIndex(e => new { e.GroupName, e.CreatedBy }, "groups_group_name_created_by_idx")
                .IsUnique()
                .HasFilter("(is_active = true)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("master.generate_snowflake_id()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasColumnName("group_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<GroupProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("group_profiles_pkey");

            entity.ToTable("group_profiles", "app");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("master.generate_snowflake_id()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupRole)
                .HasDefaultValue((short)2)
                .HasComment("{1 : ADMIN, 2 : MEMBER}")
                .HasColumnName("group_role");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<Ledger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("auth_ledger_pkey");

            entity.ToTable("ledger", "txn");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("otps_pkey");

            entity.ToTable("otps", "auth");

            entity.HasIndex(e => new { e.UserId, e.Email, e.PhoneNumber, e.Purpose }, "idx_otps_user_contact");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attempts)
                .HasDefaultValue(0)
                .HasColumnName("attempts");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("is_used");
            entity.Property(e => e.OtpHash).HasColumnName("otp_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Purpose).HasColumnName("purpose");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("profiles_new_pkey");

            entity.ToTable("profiles", "auth");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .HasColumnName("name");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens", "auth");

            entity.HasIndex(e => e.Token, "refresh_tokens_token_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsRevoked)
                .HasDefaultValue(false)
                .HasColumnName("is_revoked");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("is_used");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<SnowflakeConfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("snowflake_config_pkey");

            entity.ToTable("snowflake_config", "master");

            entity.Property(e => e.Id)
                .HasDefaultValue(1)
                .HasColumnName("id");
            entity.Property(e => e.NodeId).HasColumnName("node_id");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("entries_pkey");

            entity.ToTable("transactions", "txn");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.SplitType).HasColumnName("split_type");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("transaction_date");
            entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Version)
                .HasDefaultValue(1)
                .HasColumnName("version");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("entries_account_id_fkey");

            entity.HasOne(d => d.Group).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("entries_group_id_fkey");

            entity.HasOne(d => d.Profile).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaction_profile");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("entries_transaction_type_id_fkey");
        });

        modelBuilder.Entity<TransactionPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transaction_details_pk");

            entity.ToTable("transaction_payments", "txn");

            entity.HasIndex(e => new { e.TransactionId, e.AccountId }, "uq_transaction_details_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DebitCredit).HasColumnName("debit_credit");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transaction_types_pkey");

            entity.ToTable("transaction_types", "master");

            entity.HasIndex(e => e.TransactionTypeName, "transaction_types_transaction_type_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AllowGroup)
                .HasDefaultValue(false)
                .HasColumnName("allow_group");
            entity.Property(e => e.DebitCredit).HasColumnName("debit_credit");
            entity.Property(e => e.TransactionTypeName)
                .HasMaxLength(50)
                .HasColumnName("transaction_type_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "auth");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "users_phone_number_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.GoogleId)
                .HasMaxLength(100)
                .HasColumnName("google_id");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.Param1)
                .HasColumnType("character varying")
                .HasColumnName("param1");
            entity.Property(e => e.Param2)
                .HasColumnType("character varying")
                .HasColumnName("param2");
            entity.Property(e => e.Param3)
                .HasColumnType("character varying")
                .HasColumnName("param3");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserRole).HasColumnName("user_role");

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRole)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("users_role_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_roles_pkey");

            entity.ToTable("user_roles", "master");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
