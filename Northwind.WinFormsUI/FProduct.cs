using Northwind.Business.Abstract;
using Northwind.Business.Concrete;
using Northwind.Business.DependencyResolvers.Ninject;
using Northwind.DataAccess.Concrete.EntityFramework;
using Northwind.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Northwind.WinFormsUI
{
    public partial class FProduct : Form
    {
        IProductService _productService;
        ICategoryService _categoryService;
        bool isUpdated;
        bool isLoaded;
        public FProduct()
        {
            InitializeComponent();
            _productService = InstanceFactory.GetInstance<IProductService>();
            _categoryService = InstanceFactory.GetInstance<ICategoryService>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            isLoaded = true;
            ListProduct();
            ListCategory();

        }

        private void ListProduct()
        {
            dgwProduct.DataSource = _productService.GetAll();
        }

        private void ListCategory()
        {
            cbxCategory.DataSource = _categoryService.GetAll();
            cbxCategory.ValueMember = "CategoryId";
            cbxCategory.DisplayMember = "CategoryName";


            cbxCategoryId.DataSource = _categoryService.GetAll();
            cbxCategoryId.ValueMember = "CategoryId";
            cbxCategoryId.DisplayMember = "CategoryName";
        }

        private void cbxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (isLoaded)
                {
                    ListProduct();
                }
                else
                {
                    dgwProduct.DataSource = _productService.GetProductsByCategory(Convert.ToInt32(cbxCategory.SelectedValue));

                }
                isLoaded = false;

            }
            catch
            {
            }
        }


        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrEmpty(txtSearchProduct.Text.Trim()))
                {
                    dgwProduct.DataSource = _productService.GetProductsByName(txtSearchProduct.Text);

                }
                else
                {
                    ListProduct();
                }
            }
            catch
            {
            }

        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            AddProduct();
        }

        private void AddProduct()
        {
            try
            {
                if (isUpdated)
                {
                    _productService.Update(new Product
                    {

                        ProductId = Convert.ToInt32(dgwProduct.CurrentRow.Cells[0].Value),
                        CategoryId = Convert.ToInt32(cbxCategoryId.SelectedValue),
                        ProductName = txtProductName.Text,
                        QuantityPerUnit = txtQuantityPerUnit.Text,
                        UnitPrice = Convert.ToDecimal(txtUnitPrice.Text),
                        UnitsInStock = Convert.ToInt16(txtUnitInStock.Text)
                    });
                    MessageBox.Show("Product Updated. ");
                    ListProduct();
                }
                else
                {
                    _productService.Add(new Product
                    {
                        CategoryId = Convert.ToInt32(cbxCategoryId.SelectedValue),
                        ProductName = txtProductName.Text,
                        QuantityPerUnit = txtQuantityPerUnit.Text,
                        UnitPrice = Convert.ToDecimal(txtUnitPrice.Text),
                        UnitsInStock = Convert.ToInt16(txtUnitInStock.Text)
                    });
                    MessageBox.Show("Product Added. ");
                    ListProduct();
                }
                isUpdated = false;
                ClearText();
            }
            catch (Exception)
            {

                throw;
            }
        
        }

        private void dgwProduct_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var productId = Convert.ToInt32(dgwProduct.CurrentRow.Cells[0].Value);
            var product = _productService.GetProductById(productId);
            txtProductName.Text = product.ProductName.ToString();
            txtQuantityPerUnit.Text = product.QuantityPerUnit.ToString();
            txtUnitInStock.Text = product.UnitsInStock.ToString();
            txtUnitPrice.Text = product.UnitPrice.ToString();
            cbxCategoryId.SelectedValue = product.CategoryId;
            isUpdated = true;
        }
        private void ClearText()
        {
            txtProductName.Text = null;
            txtQuantityPerUnit.Text = null;
            txtUnitInStock.Text = null;
            txtUnitPrice.Text = null;

            ListCategory();
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgwProduct.CurrentRow != null)
            {
                var productName = dgwProduct.CurrentRow.Cells[2].Value.ToString();
                var productId = Convert.ToInt32(dgwProduct.CurrentRow.Cells[0].Value);
                var dialogResult = MessageBox.Show("Do you want to delete " + productName + "?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    _productService.Delete(new Product
                    {
                        ProductId = productId
                    });
                    MessageBox.Show("Product Deleted");

                }
            }


        }
    }
}
