namespace WebApp.Models
{
    // okunan response.json objelerinin analiz sonucunda aynı satırda yer alanlar
    // her objenin y eksenindeki merkezine bakılarak aynı satirda olup olmadigi hesaplanir
    public class Row
    {
        public decimal CenterY { get; set; }

        public List<RowElement> Elements { get; set;} = new List<RowElement>();
    }

    // her satirda json objeleri satir elementi olarak belilenir
    // her objenin x eksenindeki yerine bakilarak satirda sirasi belirlenir
    public class RowElement
    { 
        public string Content { get; set; }

        public decimal CenterX { get; set; }
    }
}
