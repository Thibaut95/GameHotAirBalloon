using System.Collections;
using System.Collections.Generic;

public interface ITable
{
    void SortBy(string columnName);
    void SetScores(List<object> list);
}