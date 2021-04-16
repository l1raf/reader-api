using System.Collections.Generic;

namespace ReaderBackend.Models.ArticleElements
{
    public class TableElement : IArticleElement
    {
        public ElementType Type => ElementType.Table;

        public IEnumerable<IEnumerable<string>> Table { get; set; }

        public TableElement(IEnumerable<IEnumerable<string>> table)
        {
            Table = table;
        }
    }
}
