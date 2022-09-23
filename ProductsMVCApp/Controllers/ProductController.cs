using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProductsMVCApp.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace ProductsMVCApp.Controllers
{
    public class ProductController : Controller
    {
        private string connString = @"Data Source =. ;Initial Catalog = ProductsDB; Integrated Security = true";
        private string path = AppDomain.CurrentDomain.BaseDirectory;
        private int jsonId;
        private string fileName = "jsonProducts.json";
        List<ProductModel> products = new List<ProductModel>();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();
                    string query = "Select * FROM Products";
                    SqlDataAdapter sda = new SqlDataAdapter(query, con);
                    sda.Fill(dt);
                }
                if (System.IO.File.Exists(Path.Combine(path, fileName)))
                {
                    string filePath = Path.Combine(path, fileName);
                    string jsonTxt = System.IO.File.ReadAllText(filePath);
                    products = JsonConvert.DeserializeObject<List<ProductModel>>(jsonTxt);
                }
                else
                {
                    string filePath = Path.Combine(path, fileName);
                    System.IO.File.Create(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in: " + ex.ToString());
            }           
            return await Task.Run(() => View(dt));
        }

        public ActionResult IndexJSON()
        {
            List<ProductModel> products = new List<ProductModel>();
            try
            {
                if (System.IO.File.Exists(Path.Combine(path, fileName)))
                {
                    string filepath = Path.Combine(path, fileName);
                    string jsonTxt = System.IO.File.ReadAllText(filepath);
                    products = JsonConvert.DeserializeObject<List<ProductModel>>(jsonTxt);
                }
                if (products != null)
                {
                    return View(products);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in: " + ex.ToString());
                return View();
            }
        }

        [HttpGet] 
        // GET: Product/Create
        public ActionResult Create()
        {
            return View(new ProductModel());
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(ProductModel productModel)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "Insert into Products Values (@ProductName, @ProductDescription, @ProductCategory, @ProductManufacturer, @ProductSupplier, @ProductPrice)";
                    string queryId = "Select MAX(ProductID) FROM Products";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductName", productModel.ProductName);
                    cmd.Parameters.AddWithValue("@ProductDescription", productModel.ProductDescription);
                    cmd.Parameters.AddWithValue("@ProductCategory", productModel.ProductCategory);
                    cmd.Parameters.AddWithValue("@ProductManufacturer", productModel.ProductManufacturer);
                    cmd.Parameters.AddWithValue("@ProductSupplier", productModel.ProductSupplier);
                    cmd.Parameters.AddWithValue("@ProductPrice", productModel.ProductPrice);
                    cmd.ExecuteNonQuery();
                    SqlCommand cmdID = new SqlCommand(queryId, conn);
                    jsonId = (Int32)cmdID.ExecuteScalar();
                }
                string filePath = Path.Combine(path, fileName);
                if (jsonId == 0)
                {
                    jsonId = products.Count + 1;
                }
                productModel.ProductID = jsonId;
                string jsonTxt = System.IO.File.ReadAllText(filePath); 
                products = JsonConvert.DeserializeObject<List<ProductModel>>(jsonTxt);
                if (products == null)
                {
                    products = new List<ProductModel>();
                }
                products.Add(productModel);
                string json = JsonConvert.SerializeObject(products, Formatting.Indented);
                System.IO.File.WriteAllText(filePath, json);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in: " + ex.ToString());
                return View();
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            ProductModel productModel = new ProductModel();
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "Select * from Products where ProductID = @ProductID";
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    sda.SelectCommand.Parameters.AddWithValue("@ProductID", id);
                    sda.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    productModel.ProductID = Convert.ToInt32(dt.Rows[0][0].ToString());
                    productModel.ProductName = dt.Rows[0][1].ToString();
                    productModel.ProductDescription = dt.Rows[0][2].ToString();
                    productModel.ProductCategory = dt.Rows[0][3].ToString();
                    productModel.ProductManufacturer = dt.Rows[0][4].ToString();
                    productModel.ProductSupplier = dt.Rows[0][5].ToString();
                    productModel.ProductPrice = Convert.ToDecimal(dt.Rows[0][6].ToString());
                    return View(productModel);
                }
                else return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in: " + ex.ToString());
                return View();
            }
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(ProductModel productModel)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "Update Products Set ProductName = @ProductName, ProductDescription = @ProductDescription, ProductCategory = @ProductCategory, ProductManufacturer = @ProductManufacturer, ProductSupplier = @ProductSupplier, ProductPrice = @ProductPrice where ProductID = @ProductID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productModel.ProductID);
                    cmd.Parameters.AddWithValue("@ProductName", productModel.ProductName);
                    cmd.Parameters.AddWithValue("@ProductDescription", productModel.ProductDescription);
                    cmd.Parameters.AddWithValue("@ProductCategory", productModel.ProductCategory);
                    cmd.Parameters.AddWithValue("@ProductManufacturer", productModel.ProductManufacturer);
                    cmd.Parameters.AddWithValue("@ProductSupplier", productModel.ProductSupplier);
                    cmd.Parameters.AddWithValue("@ProductPrice", productModel.ProductPrice);
                    cmd.ExecuteNonQuery();
                }
                string filePath = Path.Combine(path, fileName);
                string jsonTxt = System.IO.File.ReadAllText(filePath);
                products = JsonConvert.DeserializeObject<List<ProductModel>>(jsonTxt);
                var productsFromJson = jsonTxt;
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i].ProductID == productModel.ProductID)
                    {
                        products[i] = productModel;
                        var jsonForSerialize = JsonConvert.SerializeObject(products, Formatting.Indented);
                        System.IO.File.WriteAllText(filePath, jsonForSerialize);
                        break;
                    }
                }             
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in: " + ex.ToString());
                return View();
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            ProductModel productModel = new ProductModel();
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "Select * from Products where ProductID = @ProductID";
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    sda.SelectCommand.Parameters.AddWithValue("@ProductID", id);
                    sda.Fill(dt);
                }
                if (dt.Rows.Count == 1)
                {
                    productModel.ProductID = Convert.ToInt32(dt.Rows[0][0].ToString());
                    productModel.ProductName = dt.Rows[0][1].ToString();
                    productModel.ProductDescription = dt.Rows[0][2].ToString();
                    productModel.ProductCategory = dt.Rows[0][3].ToString();
                    productModel.ProductManufacturer = dt.Rows[0][4].ToString();
                    productModel.ProductSupplier = dt.Rows[0][5].ToString();
                    productModel.ProductPrice = Convert.ToDecimal(dt.Rows[0][6].ToString());
                    return View(productModel);
                }
                else return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in: " + ex.ToString());
                return View();
            }
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "Delete from Products where ProductID = @ProductID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", id);
                    cmd.ExecuteNonQuery();
                }
                string filePath = Path.Combine(path, fileName);
                string jsonTxt = System.IO.File.ReadAllText(filePath);
                products = JsonConvert.DeserializeObject<List<ProductModel>>(jsonTxt);
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i].ProductID == id)
                    {
                        products.Remove(products[i]);
                        var jsonForSerialize = JsonConvert.SerializeObject(products, Formatting.Indented);
                        System.IO.File.WriteAllText(filePath, jsonForSerialize);
                        break;
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in: " + ex.ToString());
                return View();
            }            
        }
    }
}
