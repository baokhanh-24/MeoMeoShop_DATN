namespace MeoMeo.Contract.DTOs.ProductReview
{
    public class ToggleReviewVisibilityDTO
    {
        public Guid ReviewId { get; set; }
        public bool? IsHidden { get; set; } // null = toggle, true/false = set specific value
        public string? Reason { get; set; } // Optional reason for hiding/showing
    }
}
