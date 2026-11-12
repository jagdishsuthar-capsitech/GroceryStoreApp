namespace GroceryApp.Models;

public class PagedApiResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}