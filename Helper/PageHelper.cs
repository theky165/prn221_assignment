namespace WebRazor.Helper
{
    public class PageHelper
    {
        public int pageCurrent { get; set; }
        public int totalPages { get; set; }

        public Func<int?, string> generateUrl { get; set; }
    }
}
