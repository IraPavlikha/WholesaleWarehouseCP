using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WholesaleWarehouseCP
{
    public partial class Form4 : Form
    {
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> suppliersCollection;
        private IMongoCollection<BsonDocument> warehousesCollection;

        public Form4()
        {
            InitializeComponent();
            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("warehouse_management");
            suppliersCollection = database.GetCollection<BsonDocument>("suppliers");
            warehousesCollection = database.GetCollection<BsonDocument>("warehouse");
            LoadSuppliersData();
            LoadWarehousesData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Form2();
            form2.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                var supplierId = selectedRow.Cells["_id"].Value.ToString();

                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(supplierId));
                var supplier = suppliersCollection.Find(filter).FirstOrDefault();

                if (supplier != null)
                {
                    string info = $"Назва: {supplier["name"]}\n" +
                                 $"Контактна особа: {supplier["contact_person"]}\n" +
                                 $"Телефон: {supplier["phone"]}\n" +
                                 $"Email: {supplier["email"]}\n" +
                                 $"Адреса: {supplier["address"]}";

                    MessageBox.Show(info, "Інформація про постачальника", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
  
            if (e.RowIndex >= 0)
            {
                var selectedRow = dataGridView2.Rows[e.RowIndex];
                var warehouseId = selectedRow.Cells["_id"].Value.ToString();

                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(warehouseId));
                var warehouse = warehousesCollection.Find(filter).FirstOrDefault();

                if (warehouse != null)
                {
                    string info = $"Розташування: {warehouse["location"]}\n" +
                                 $"Остання інвентаризація: {warehouse["last_inventory_date"]}\n" +
                                 $"Примітки: {warehouse["notes"]}";

                    MessageBox.Show(info, "Інформація про склад", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void LoadSuppliersData()
        {
            try
            {
                var suppliers = suppliersCollection.Find(new BsonDocument()).ToList();
                var supplierList = new List<object>();

                foreach (var supplier in suppliers)
                {
                    supplierList.Add(new
                    {
                        Назва = supplier["name"].ToString(),
                        Телефон = supplier["phone"].ToString(),
                        Email = supplier["email"].ToString()
                    });
                }

                dataGridView1.DataSource = supplierList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні постачальників: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWarehousesData()
        {
            try
            {
                var warehouses = warehousesCollection.Find(new BsonDocument()).ToList();
                var warehouseList = new List<object>();

                foreach (var warehouse in warehouses)
                {
                    warehouseList.Add(new
                    {
                        Розташування = warehouse["location"].ToString(),
                        Інвентаризація = warehouse["last_inventory_date"].ToLocalTime().ToString("dd.MM.yyyy")
                    });
                }

                dataGridView2.DataSource = warehouseList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні складів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label2_Click(object sender, EventArgs e) { }
    }
}