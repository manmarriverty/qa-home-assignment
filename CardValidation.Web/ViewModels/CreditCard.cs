namespace CardValidation.ViewModels
{
    public class CreditCard
    {
        public string Owner { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;  // MM/YY or MM/YYYY
        public string Cvc { get; set; } = string.Empty;        // 3 or 4 digits
    }
}
