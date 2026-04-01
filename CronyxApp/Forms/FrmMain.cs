using System.Net.NetworkInformation;
using CronyxApp.Services.Constants;
using CronyxLib;
using CronyxLib.Services;

namespace CronyxApp
{
    public partial class FrmMain : Form
    {
        private ImageList _imageList = new ImageList();
        public FrmMain()
        {
            InitializeComponent();
        }

        private async void FrmMain_Shown(object sender, EventArgs e)
        {
            await InitializeApp();
        }

        private async Task InitializeApp()
        {
            try
            {
                string? token = TokenService.LoadToken();
                if (string.IsNullOrEmpty(token))
                {
                    OpenLogin();
                    return;
                }

                var manager = new CronyxServiceManager(token);

                var auth = await manager.ValidateToken();

                if (auth != AuthStatus.Success)
                {
                    OpenLogin();
                    return;
                }

                //SERVICE:
                //BackgroundWorker.Stop();
                //StartSystem(manager);

                await LoadDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                OpenLogin();
            }
        }

        private void StartSystem(CronyxServiceManager manager)
        {
            manager.EnsureServiceRunning();
            BackgroundWorker.Start(manager);
        }

        private void OpenLogin()
        {
            this.Hide();

            using (var frm = new FrmLogin())
            {
                frm.ShowDialog();
            }

            this.Show();
            _ = InitializeApp();
        }

        private async Task LoadDevices()
        {
            var devices = await new CronyxLib.APICall().GetDevices();

            if (devices == null) return;

            devices = devices
                .OrderBy(x => x.DeviceName)
                .ToList();

            _imageList.Images.Clear();
            LstDevices.Items.Clear();

            int index = 0;

            foreach (var d in devices)
            {
                //Image img = await GetDeviceImage(d.DevicePhotoID);

                //_imageList.Images.Add(img ?? Properties.Resources.default_device);

                var item = new ListViewItem
                {
                    Text = d.DeviceName,
                    ImageIndex = index
                };

                LstDevices.Items.Add(item);
                index++;
            }
        }
        private void SetupListView()
        {
            _imageList.ImageSize = new Size(64, 64); // adjust if needed
            LstDevices.LargeImageList = _imageList;
            LstDevices.View = View.LargeIcon;
        }
    }
}
