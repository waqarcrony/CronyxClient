using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.ServiceProcess;
using CronyxApp.Services.Constants;
using CronyxLib;
using CronyxLib.Services;
using Newtonsoft.Json;
using BackgroundWorker = CronyxLib.Services.BackgroundWorker;

namespace CronyxApp
{
    public partial class FrmSelectStore : Form
    {
        public LoginResponseVM SelectedStore { get; private set; }

        public FrmSelectStore(List<LoginResponseVM> stores)
        {
            InitializeComponent();

            CmbStore.DataSource = stores;
            CmbStore.DisplayMember = "StoreName";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (CmbStore.SelectedItem == null)
            {
                MessageBox.Show("Select a store");
                return;
            }

            SelectedStore = (LoginResponseVM)CmbStore.SelectedItem;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
