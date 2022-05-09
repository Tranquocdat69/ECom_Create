namespace FPTS.FIT.BDRD.Services.Ordering.App.DTOs
{
    public class OrderDTO
    {
        public string City { get; set; }
        public string Street { get; set; }
        public DateTime CreateDate { get; set; }
        public int ItemCount { get; set; }

        public OrderDTO(string city, string street, DateTime createDate, int itemCount)
        {
            City = city;
            Street = street;
            CreateDate = createDate;
            ItemCount = itemCount;
        }
    }
}
