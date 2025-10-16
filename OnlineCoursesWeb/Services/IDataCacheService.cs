using System.Collections;

namespace OnlineCoursesWeb.Services;

public interface IDataCacheService
{
    void PrimeCache(); 
    IEnumerable GetData(string tableName);
}