using System;
using System.Windows.Forms;
using SVNManagerLib;

namespace TestHarness
{
    public partial class Harness : Form
    {
        private bool _txtServerValid = false;
        private bool _txtCommandsValid = false;
        private bool _txtRepoDirValid = false;

        public Harness()
        {
            InitializeComponent();
        }

        private void btnRoot_Click(object sender, EventArgs e)
        {
            fbdLoad.Description = "Directory where the Subversion server was installed";
            DialogResult result = fbdLoad.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtServer.Text = fbdLoad.SelectedPath;
            }
        }

        private void btnCommands_Click(object sender, EventArgs e)
        {
            fbdLoad.Description = "Directory where the Subversion server commands reside";
            DialogResult result = fbdLoad.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtCommands.Text = fbdLoad.SelectedPath;
            }
        }

        private void btnRepos_Click(object sender, EventArgs e)
        {
            fbdLoad.Description = "Directory where the Subversion repositories reside";
            DialogResult result = fbdLoad.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtRepoDir.Text = fbdLoad.SelectedPath;
            }
        }

        private void txtServer_Leave(object sender, EventArgs e)
        {
            if (txtServer.Text.Length > 0)
            {
                _txtServerValid = true;
                FormIsValid();
            }
            else
            {
                _txtServerValid = false;
                FormIsValid();
            }
        }

        private void txtCommands_Leave(object sender, EventArgs e)
        {
            if (txtCommands.Text.Length > 0)
            {
                _txtCommandsValid = true;
                FormIsValid();
            }
            else
            {
                _txtCommandsValid = false;
                FormIsValid();
            }
        }

        private void txtRepoDir_Leave(object sender, EventArgs e)
        {
            if (txtRepoDir.Text.Length > 0)
            {
                _txtRepoDirValid = true;
                FormIsValid();
            }
            else
            {
                _txtRepoDirValid = false;
                FormIsValid();
            }
        }

        private void FormIsValid()
        {
            if (_txtServerValid && _txtRepoDirValid && _txtCommandsValid)
            {
                btnLoad.Enabled = true;
            }
            else
            {
                btnLoad.Enabled = false;
            }
        }

        private void txtRepoDir_TextChanged(object sender, EventArgs e)
        {
            if (txtRepoDir.Text.Length > 0)
            {
                _txtRepoDirValid = true;
                FormIsValid();
            }
            else
            {
                _txtRepoDirValid = false;
                FormIsValid();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            SVNController controller = new SVNController(txtServer.Text, txtCommands.Text,txtRepoDir.Text);

            foreach(SVNRepository repo in controller.RepositoryCollection)
            {
                cboRepos.Items.Add(repo.Name);
            }

            cboRepos.Enabled = true;
            cboRepos.SelectedIndex = 0;
        }
    }
}