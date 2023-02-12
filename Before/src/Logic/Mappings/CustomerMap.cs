using FluentNHibernate.Mapping;
using Logic.Entities;

namespace Logic.Mappings;

public class CustomerMap : ClassMap<Customer>
{
    public CustomerMap()
    {
        Id(x => x.Id);

        Map(x => x.Name).CustomType<string>().Access.CamelCaseField(Prefix.Underscore);
        Map(x => x.Email).CustomType<string>().Access.CamelCaseField(Prefix.Underscore);
        Map(x => x.MoneySpent).CustomType<decimal>().Access.CamelCaseField(Prefix.Underscore);

        Component(c => c.Status, cp =>
        {
            cp.Map(s => s.Type, "Status").CustomType<int>();
            cp.Map(s => s.ExpirationDate, "StatusExpirationDate").
                CustomType<DateTime?>()
                .Access.CamelCaseField(Prefix.Underscore).Nullable();
        });
        
        HasMany(x => x.PurchasedMovies).Access.CamelCaseField(Prefix.Underscore);
    }
}
