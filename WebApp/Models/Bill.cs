using Newtonsoft.Json.Linq;
using System.IO;

namespace WebApp.Models
{
    /// <summary>
    /// response.js deserialize edilmesi icin kullanilan sınıf
    /// buradaki fis bill sinifi uzerinden deserialize edilir
    /// </summary>
    public class Bill
    { 
        public List<BillElement> BillElements { get; set; } = new List<BillElement>();

        public Bill()
        {

        }

        // parametre olarak verilen path deki json dosyasının deserialize edilmesi
        public Bill(string path)
        {
            using (var reader = new StreamReader(path))
            {
                var json = JToken.Parse(reader.ReadToEnd()).Children().ToList();
                foreach (var x in json)
                {
                    var billElement = new BillElement();
                    billElement.locale = x["locale"] is not null ? x["locale"].ToString() : null;
                    billElement.description = x["description"] is not null ? x["description"].ToString() : null;
                    billElement.boundingPoly = new BoundingPoly();
                    billElement.boundingPoly.vertices = new List<Vertex>();
                    billElement.boundingPoly.vertices.Add(new Vertex
                    {
                        x = Convert.ToDecimal(x["boundingPoly"]["vertices"][0]["x"]),
                        y = Convert.ToDecimal(x["boundingPoly"]["vertices"][0]["y"]),
                    });
                    billElement.boundingPoly.vertices.Add(new Vertex
                    {
                        x = Convert.ToDecimal(x["boundingPoly"]["vertices"][1]["x"]),
                        y = Convert.ToDecimal(x["boundingPoly"]["vertices"][1]["y"]),
                    });
                    billElement.boundingPoly.vertices.Add(new Vertex
                    {
                        x = Convert.ToDecimal(x["boundingPoly"]["vertices"][2]["x"]),
                        y = Convert.ToDecimal(x["boundingPoly"]["vertices"][2]["y"]),
                    });
                    billElement.boundingPoly.vertices.Add(new Vertex
                    {
                        x = Convert.ToDecimal(x["boundingPoly"]["vertices"][3]["x"]),
                        y = Convert.ToDecimal(x["boundingPoly"]["vertices"][3]["y"]),
                    });
                    BillElements.Add(billElement);
                }
            }
        }

        // bill elementlerinin satirlar seklinde listelenmesi
        public List<Row> GetRows()
        {
            // bill'in satirlari
            var rows = new List<Row>();

            // bill icindeki herhangi bir satirin yuksekligi
            var height = BillElements[1].boundingPoly.vertices[3].y - BillElements[1].boundingPoly.vertices[0].y;

            // satir yuksekliginden faydalanarak sapma payi hesaplanmasi
            var variance = height / 2;

            // bill elementleri yani herbir json objesi for dongusune sokulut
            for (var i = 1; i < BillElements.Count; i++)
            { 
                // json objesinin y ekseninde merkezi bulunur
                var yCenter = (BillElements[i].boundingPoly.vertices[3].y + BillElements[i].boundingPoly.vertices[0].y) /2 ;

                // y ekseni merkezi goz onunde bulundurularak herhangi bir satira ait olup olmayacagi
                // sapma payi da kullanılarak bulunur
                var properRow = rows.FirstOrDefault(x => yCenter >= (x.CenterY - variance) && yCenter <= (x.CenterY + variance));

                // uygun bir row bulunur ise
                if (properRow is not null)
                {
                    // mevcut json objesi uygun satira eklenir
                    properRow.Elements.Add(new RowElement
                    {
                        Content = BillElements[i].description,
                        CenterX = (BillElements[i].boundingPoly.vertices[0].x + BillElements[i].boundingPoly.vertices[1].x) / 2
                    });
                }
                else
                {
                    // yeni bir satir olusturulur ve bu json objesi eklenir
                    // yeni satirin y eksenindeki yeri bu objenin y merkezi olarak atanir
                    rows.Add(new Row
                    {
                        CenterY = yCenter,
                        Elements = new List<RowElement> { 
                            new RowElement { 
                                Content = BillElements[i].description, 
                                CenterX = (BillElements[i].boundingPoly.vertices[0].x + BillElements[i].boundingPoly.vertices[1].x) / 2
                            }                      
                        }
                    });
                }
            }

            // olusturulan satırlar y ekseni uzerinde kucukten buyuge siralanir
            rows = rows.OrderBy(x => x.CenterY).ToList();
            // satirlarin json objeleri yani row elementleri x ekseninde kucukten buyuge siralanir
            rows.ForEach(x =>
            {
                x.Elements = x.Elements.OrderBy(x => x.CenterX).ToList();
            });

            return rows;
        }
    }

    // okunan json objeleridir
    public class BillElement
    {
        public string locale { get; set; }

        public string description { get; set; }

        public BoundingPoly boundingPoly { get; set; }
    }

    public class BoundingPoly
    {
        public List<Vertex> vertices { get; set; }
    }

    public class Vertex
    {
        public decimal x { get; set; }

        public decimal y { get; set; }
    }
}
