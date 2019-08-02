using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Engine;


namespace Intro_Screen

{
    public partial class Intro : Form
    {
        private Player _player;

        //xml file the player data will save to
        public const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public Intro()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                btnContinue.Enabled = true;
            }
            else
            {
                btnContinue.Enabled = false;
            }
        }

        private void Intro_Load(object sender, EventArgs e)
        {
            cboStyle.Items.Add("GUI Based Gameplay");
            cboStyle.Items.Add("Text Based Gameplay");
        }

        private void CboStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
#pragma warning disable CS0252

            if (cboStyle.SelectedItem == "GUI Based Gameplay")

#pragma warning restore CS0252 
            {
                File.Delete(PLAYER_DATA_FILE_NAME);

                Close();
                Process.Start("RPG.exe");  
            }
#pragma warning disable CS0252

            if (cboStyle.SelectedItem == "Text Based Gameplay")

#pragma warning restore CS0252
            {
                 File.Delete(PLAYER_DATA_FILE_NAME);

                Close();
                Process.Start("RPG_Console.exe");
            }
        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
#pragma warning disable CS0252

            if (cboStyle.SelectedItem == "GUI Based Gameplay")

#pragma warning restore CS0252 
            {
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));

                    Close();
                    Process.Start("RPG.exe");
                }
            }
#pragma warning disable CS0252

            if (cboStyle.SelectedItem == "Text Based Gameplay")

#pragma warning restore CS0252
            {
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));

                    Close();
                    Process.Start("RPG_Console.exe");
                }
            }
        }
    }
}
