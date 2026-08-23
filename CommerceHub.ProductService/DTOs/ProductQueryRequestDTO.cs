namespace CommerceHub.ProductService.DTOs
{
    public class ProductQueryRequestDTO
    {
        public string? Category { get; set; } = string.Empty;

        public decimal? MinPrice { get; set; } = 0;

        public decimal? MaxPrice { get; set; } = decimal.MaxValue;

        public string? SearchTerm { get; set; } = string.Empty;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
