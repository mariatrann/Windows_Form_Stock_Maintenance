/*
 * Program ID: assignment4
 * 
 * Purpose: To manage and maintain stock for a beauty shop
 * 
 * Revision History
 *      created by Maria Tran on April 2, 2021
 *      updated by Maria Tran on April 9, 2021
*/
using MTClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace assignment4
{
    public partial class MTStockMaintenance : Form
    {
        public MTStockMaintenance()
        {
            InitializeComponent();
        }

        Boolean isAdd = false;

        //Clears input fields
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtStockID.Text = "0";
            txtName.Text = "";
            txtDescription.Text = "";
            txtPrice.Text = "";
            txtMinutes.Text = "";
            chkIsProcedure.Checked = false;
            txtStockID.Enabled = true;
            isAdd = true;
            lblMessages.Text = "Input fields have been cleared\n";
        }

        //Saves the record into the text file
        private void btnSave_Click(object sender, EventArgs e)
        {
            lblMessages.Text = "";
            double price = 0;
            Int32 minutes = 0;

            if (txtStockID.Text == "0")
            {
                isAdd = true;
            }
            else
            {
                isAdd = false;
            }

            MTStock stock = new MTStock(Convert.ToInt32(txtStockID.Text));

            stock.Name = txtName.Text;
            stock.Description = txtDescription.Text;

            if (double.TryParse(txtPrice.Text, out price))
            {
                stock.Price = price;
            }
            else
            {
                lblMessages.Text += "Price is not numeric\n";
            }

            if (Int32.TryParse(txtMinutes.Text, out minutes))
            {
                stock.Minutes = minutes;
            }
            else
            {
                lblMessages.Text += "Minutes is not numeric\n";
            }

            stock.IsProcedure = chkIsProcedure.Checked;

            try
            {
                if (isAdd == true && txtStockID.Text == "0")
                {
                    stock.MTAdd(stock.StockID);
                }
                else if (isAdd == false)
                {
                    stock.MTUpdate(stock.StockID);
                }

                LoadListBox();
                lstStockRecords.SelectedValue = stock.StockID;

                isAdd = false;
            }
            catch (Exception ex)
            {
                lblMessages.Text += "Problems with saving/updating record: \n" + ex.Message + "\n";
            }

            if (lblMessages.Text != "")
            {
                return;
            }
        }

        //Deletes a selected record and re-selects the record after it
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtStockID.Enabled == false)
                {
                    MTStock.MTDelete(txtStockID.Text);
                    string name = txtName.Text;

                    LoadListBox();
                    MTStock stock = MTStock.MTGetByDescription(name);
                    if (lstStockRecords.SelectedIndex != 0)
                    {
                        lstStockRecords.SelectedValue = stock.StockID;
                    }
                }
                else
                {
                    lblMessages.Text = "Please re-select a stock item to delete\n";
                }
            }
            catch (Exception ex)
            {
                lblMessages.Text = "Problem trying to delete stock: \n" + ex.Message + "\n";
            }
        }

        //Reverts input areas to their values before it was cleared or modified
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(lstStockRecords.SelectedIndex == 0)
            {
                lstStockRecords_SelectedIndexChanged(sender, e);
            }
            else
            {
                lblMessages.Text = "Error with cancelling input areas\n";
            }
        }

        //Closes the form
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Reload listbox with stock
        private void LoadListBox()
        {
            List<MTStock> stocks = MTStock.MTGetStocks();
            stocks = stocks.OrderBy(a => a.Name).ToList();

            lstStockRecords.DisplayMember = "Name";
            lstStockRecords.ValueMember = "StockID";
            lstStockRecords.DataSource = stocks;
        }

        //A record selected in the list will reload the input areas
        private void lstStockRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtStockID.Enabled = false;
            try
            {
                MTStock record = MTStock.MTGetByStockID(Convert.ToInt32(lstStockRecords.SelectedValue));

                txtStockID.Text = record.StockID.ToString();
                txtName.Text = record.Name;
                txtDescription.Text = record.Description;
                txtPrice.Text = record.Price.ToString();
                txtMinutes.Text = record.Minutes.ToString();
                chkIsProcedure.Checked = record.IsProcedure;
            }
            catch (Exception ex)
            {
                lblMessages.Text = "Problems with loading list: \n" + ex.Message + "\n";
            }

        }

        //Loads listbox when form loads
        private void MTStockMaintenance_Load(object sender, EventArgs e)
        {
            LoadListBox();
            if (txtStockID.Text == "")
            {
                txtStockID.Text = "0";
            }
        }
    }
}
