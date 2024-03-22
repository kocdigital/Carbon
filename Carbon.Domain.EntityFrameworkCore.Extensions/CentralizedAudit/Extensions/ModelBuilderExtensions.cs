using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;

namespace EFCoreAuditing.Extensions
{
    public static class ModelBuilderExtensions
    {
        internal static bool IsEnabledSoftDelete = false;

        //public static void ApplyAllTypeConfigurations<TContext>(this ModelBuilder modelBuilder, string nameSpace)
        //     where TContext : DbContext
        //{
        //    var applyConfigurationMethodInfo = modelBuilder
        //        .GetType()
        //        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        //        .First(m => m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));

        //    var ret = typeof(TContext).Assembly
        //        .GetTypes()
        //        .Where(t => t.Namespace == nameSpace)
        //        .Select(t =>
        //                (t, i: t.GetInterfaces().FirstOrDefault(i =>
        //                 i.Name.Equals(typeof(IEntityTypeConfiguration<>).Name, StringComparison.Ordinal))))
        //        .Where(it => it.i != null)
        //        .Select(it => (et: it.i.GetGenericArguments()[0], cfgObj: Activator.CreateInstance(it.t)))
        //        .Select(it => applyConfigurationMethodInfo.MakeGenericMethod(it.et)
        //        .Invoke(modelBuilder, new[] { it.cfgObj }))
        //        .ToList();
        //}

        //public static ModelBuilder ModifyAllTypeConfigurations(this ModelBuilder modelBuilder)
        //{
        //    foreach (var entity in modelBuilder.Model.GetEntityTypes())
        //    {
        //        SnakeCaseifyTableName(entity);

        //        foreach (var property in entity.GetProperties())
        //        {
        //            SnakeCaseifyColumnName(property);

        //            ModifyProperty(property);
        //        }
        //    }
        //    return modelBuilder;
        //}

        //private static void ModifyProperty(Microsoft.EntityFrameworkCore.Metadata.IMutableProperty property)
        //{
        //    if (property.ClrType == typeof(string))
        //    {
        //        if (property.GetMaxLength() == null && property.Relational().ColumnType == null)
        //            property.SetMaxLength(256);
        //    }
        //}

        //private static void SnakeCaseifyColumnName(Microsoft.EntityFrameworkCore.Metadata.IMutableProperty property)
        //{
        //    property.Relational().ColumnName = CamelCaseToSnakeCase(property.Relational().ColumnName);
        //}

        //private static void SnakeCaseifyTableName(Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entity)
        //{
        //    entity.Relational().TableName = CamelCaseToSnakeCase(entity.Relational().TableName);
        //}

      
        //public static ModelBuilder UseCitext(this ModelBuilder modelBuilder, DataStoreType dataStoreType = DataStoreType.PostgreSql)
        //{

        //    modelBuilder = modelBuilder.HasPostgresExtension("citext");

        //    foreach (var entity in modelBuilder.Model.GetEntityTypes())
        //    {
        //        foreach (var property in entity.GetProperties())
        //        {
        //            if (dataStoreType == DataStoreType.PostgreSql)
        //            {
        //                if (property.ClrType == typeof(string))
        //                {
        //                    property.Npgsql().ColumnType = "public.citext";
        //                }
        //            }
        //        }
        //    }

        //    return modelBuilder;
        //}

        

        private static string CamelCaseToSnakeCase(string clrName)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < clrName.Length; i++)
            {
                var c = clrName[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                        sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
