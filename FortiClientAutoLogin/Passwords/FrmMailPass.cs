using FortiClientAutoLogin.Passwords;

namespace FortiClientAutoLogin
{
    public partial class FrmMailPass : Form
    {
        public FrmMailPass()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
{
            PasswordManager.Save(PasswordType.EMAIL, tbMailPass.Text);
            this.Close();
        }
    }
}