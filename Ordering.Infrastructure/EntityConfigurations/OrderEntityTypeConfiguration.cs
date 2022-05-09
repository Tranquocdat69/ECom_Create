namespace FPTS.FIT.BDRD.Services.Ordering.Infrastructure.EntityConfigurations;

class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> orderConfiguration)
    {
        orderConfiguration.ToTable("orders", OrderDbContext.DEFAULT_SCHEMA);

        orderConfiguration.HasKey(o => o.Id);

        orderConfiguration.Ignore(b => b.DomainEvents);

        orderConfiguration.Property(o => o.Id);

        orderConfiguration
            .OwnsOne(o => o.Address, a =>
            {
                a.Property<int>("OrderId");
                a.WithOwner();
            });

        orderConfiguration.Property(o => o.OrderDate)
            .HasColumnName("Order_Date");
        orderConfiguration.Property(o => o.CustomerId)
            .HasColumnName("User_Id");

        var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));
    }
}
