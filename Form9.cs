using System;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form9 : Form
    {
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> productsCollection;
        private IMongoCollection<BsonDocument> warehousesCollection;

        public Form9()
        {
            InitializeComponent();
            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            productsCollection = database.GetCollection<BsonDocument>("products");
            warehousesCollection = database.GetCollection<BsonDocument>("warehouse");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = textBox1.Text.Trim();
                if (string.IsNullOrEmpty(searchText))
                {
                    MessageBox.Show("Введіть текст для пошуку.");
                    return;
                }
                var filter = Builders<BsonDocument>.Filter.Eq("barcode", searchText) |
                             Builders<BsonDocument>.Filter.Regex("name", new BsonRegularExpression(searchText, "i"));

                var product = await productsCollection.Find(filter).FirstOrDefaultAsync();

                if (product != null)
                {
                    var warehouse = await warehousesCollection.Find(Builders<BsonDocument>.Filter.Eq("_id", product["warehouse_id"])).FirstOrDefaultAsync();

                    if (warehouse != null)
                    {
                        string message = $"Товар: {product["name"]}\n" +
                                         $"Кількість: {product["stock_quantity"]}\n" +
                                         $"Склад: {warehouse["location"]}";

                        MessageBox.Show(message, "Товар знайдено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не знайдено склад для цього товару.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Товар не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при пошуку товару: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
