namespace eSSLAttendanceDesktopClient.Domain
{
    public class Pagination
    {
        public int PageSize { get; set; } = 10;
        public int Take { get; set; } = 10;
        public int Skip { get; set; } = 0;
        public int CurrentPage { get; set; } = 1;
        public int TotalPage { get; set; } = 0;
        public int TotalRecord { get; set; } = 0;
    }
}
