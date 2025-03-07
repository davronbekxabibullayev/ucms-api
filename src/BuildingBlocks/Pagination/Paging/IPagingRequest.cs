namespace Paging;
public interface IPagingRequest : ISortingRequest
{
    int First { get; set; }

    int Rows { get; set; }
}
