namespace LibraryHub.Web.ViewModels
{
    public class PagerViewModel
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string? Search { get; set; }
    }
}