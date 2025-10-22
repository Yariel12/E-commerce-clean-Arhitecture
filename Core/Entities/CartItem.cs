using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public Cart Cart { get; set; }

        public Product Product { get; set; }
    }
}
