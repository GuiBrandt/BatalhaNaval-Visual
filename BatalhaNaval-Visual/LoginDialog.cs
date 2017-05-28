using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatalhaNaval_Visual
{
    /// <summary>
    /// Formulário de login
    /// </summary>
    public partial class LoginDialog : Form
    {
        /// <summary>
        /// Obtém o nome de usuário digitado
        /// </summary>
        public string User
        {
            get
            {
                return txtUser.Text.Trim();
            }
        }

        /// <summary>
        /// Construtor
        /// </summary>
        public LoginDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento de clique no botão OK
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(User))
            {
                txtUser.Focus();
                SystemSounds.Exclamation.Play();
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Evento de clique no botão Cancelar
        /// </summary>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
