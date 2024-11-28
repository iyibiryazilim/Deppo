using System;

namespace Deppo.Core.SortModels;

public class SortModel
{
    public const string ItemCodeAsc = "ORDER BY ITEMS.CODE ASC";
    public const string ItemCodeDesc = "ORDER BY ITEMS.CODE DESC";
    public const string ItemNameAsc = "ORDER BY ITEMS.NAME ASC";
    public const string ItemNameDesc = "ORDER BY ITEMS.NAME DESC";

    public const string CurrentCodeAsc = "ORDER BY CLCARD.CODE ASC";
    public const string CurrentCodeDesc = "ORDER BY CLCARD.CODE DESC";
    public const string CurrentNameAsc = "ORDER BY CLCARD.DEFINITION_ ASC";
    public const string CurrentNameDesc = "ORDER BY CLCARD.DEFINITION_ DESC";
    public const string PriorityAsc = "ORDER BY SUPPASGN.PRIORITY ASC";
    public const string PriorityDesc = "ORDER BY SUPPASGN.PRIORITY DESC";
}
