using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.EntityFrameworkCore.ValueComparers;
using Volo.Abp.EntityFrameworkCore.ValueConverters;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using VoloAbp.OTel.Authors;
using VoloAbp.OTel.Books;

namespace VoloAbp.OTel.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class OTelDbContext :
    AbpDbContext<OTelDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    // 1. 定义一个静态的、宽松的 Options
    private static readonly JsonSerializerOptions RelaxedJsonOptions = new JsonSerializerOptions
    {
        // 关键：允许不安全的字符（包括中文）不被转义
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // 保持 ABP 默认的驼峰命名（可选，为了保持一致性）
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // 如果你想省库空间，设为 true 跳过 null；否则保持默认
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new ObjectToInferredTypesConverter()
        }
    };

    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DbSet<Book> Books { get; set; }

    public DbSet<Author> Authors { get; set; }

    public OTelDbContext(DbContextOptions<OTelDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(OTelConsts.DbTablePrefix + "YourEntities", OTelConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        builder.Entity<Book>(b =>
        {
            b.ToTable(OTelConsts.DbTablePrefix + "Books", OTelConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);

            // ADD THE MAPPING FOR THE RELATION
            b.HasOne<Author>().WithMany().HasForeignKey(x => x.AuthorId).IsRequired();
        });

        builder.Entity<Author>(b =>
        {
            b.ToTable(OTelConsts.DbTablePrefix + "Authors",
                OTelConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(AuthorConsts.MaxNameLength);

            b.HasIndex(x => x.Name);
        });


        //https://github.com/abpframework/abp/blob/9.3.6/framework/src/Volo.Abp.EntityFrameworkCore/Volo/Abp/EntityFrameworkCore/ValueConverters/ExtraPropertiesValueConverter.cs
        //https://github.com/abpframework/abp/blob/9.3.6/framework/src/Volo.Abp.EntityFrameworkCore/Volo/Abp/EntityFrameworkCore/ValueComparers/ExtraPropertyDictionaryValueComparer.cs
        //替换重写ExtraPropertiesValueConverter -> MyExtraPropertiesValueConverter ; ExtraPropertyDictionaryValueComparer -> MyExtraPropertyDictionaryValueComparer;
        //foreach (var entityType in builder.Model.GetEntityTypes())
        //{
        //    // 检查是否实现了 IHasExtraProperties 接口
        //    if (!typeof(IHasExtraProperties).IsAssignableFrom(entityType.ClrType))
        //    {
        //        continue;
        //    }

        //    var b = builder.Entity(entityType.ClrType);

        //    // 获取 ExtraProperties 属性
        //    var property = b.Metadata.FindProperty(nameof(IHasExtraProperties.ExtraProperties));

        //    // 双重检查属性是否存在且类型正确
        //    if (property != null && property.ClrType == typeof(ExtraPropertyDictionary))
        //    {
        //        var type = typeof(MyExtraPropertiesValueConverter<>).MakeGenericType(b.Metadata.ClrType);
        //        var extraPropertiesValueConverter = Activator.CreateInstance(type)!.As<ValueConverter<ExtraPropertyDictionary, string>>();
        //        property.SetValueConverter(extraPropertiesValueConverter);
        //        property.SetValueComparer(new MyExtraPropertyDictionaryValueComparer());
        //    }

        //    // 你的代码中有 TryConfigureObjectExtensions()，通常它放在循环内部是安全的，
        //    // 但要确保你的 HasConversion 是最后执行的，或者明确覆盖。
        //    b.TryConfigureObjectExtensions(); 
        //}


        // 2. 遍历所有实体，找到 ExtraProperties 字段并替换转换器
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            // 查找名为 ExtraProperties 的属性
            var property = entityType.FindProperty(nameof(IHasExtraProperties.ExtraProperties));

            // 确保属性存在，且类型确实是 ExtraPropertyDictionary
            if (property != null && property.ClrType == typeof(ExtraPropertyDictionary))
            {
                // 设置自定义的 ValueConverter
                property.SetValueConverter(new ValueConverter<ExtraPropertyDictionary, string>(
                    // 序列化逻辑：使用我们定义的 RelaxedJsonOptions
                    d => JsonSerializer.Serialize(d, RelaxedJsonOptions),
                    // 反序列化逻辑
                    s => JsonSerializer.Deserialize<ExtraPropertyDictionary>(s, RelaxedJsonOptions) ?? new ExtraPropertyDictionary()
                ));
                //var valueConverter = property.GetValueConverter();
                //var valueComparer = property.GetValueComparer();
            }
        }
    }
}
