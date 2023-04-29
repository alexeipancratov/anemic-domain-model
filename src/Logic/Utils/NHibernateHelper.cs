using System.Data.Common;
using NHibernate.Dialect;
using NHibernate.Dialect.Schema;

namespace Logic.Utils;

public class CustomMsSqlDataBaseSchema : MsSqlDataBaseSchema
{
    public CustomMsSqlDataBaseSchema(DbConnection connection) : base(connection)
    {
    }
 
    public override ISet<string> GetReservedWords()
    {
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}
 
 
public class EnhancedMsSql2008Dialect : MsSql2008Dialect
{
    public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
    {
        return new CustomMsSqlDataBaseSchema(connection);
    }
}