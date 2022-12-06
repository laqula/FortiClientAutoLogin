using FortiClientAutoLogin.Passwords;

namespace FortiClientAutoLogin
{
    public partial class FrmVpnPass : Form
    {
        public FrmVpnPass()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            PasswordManager.Save(PasswordType.VPN, tbVpnPass.Text);
            this.Close();
        }
    }
}