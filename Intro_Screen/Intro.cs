using Engine;
using RPG;
using RPG_Console;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Intro_Screen

{
    public partial class Intro : Form
    {
        private static Player _player;

        //Console Program
        ConsoleProgram crp = new ConsoleProgram();

        //GUI Program
        RPGForm rpgForm = new RPGForm();

        //xml file the player data will save to
        public const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public Intro()
        {
            InitializeComponent();

            _player = PlayerDataMapper.CreateFromDatabase();

            if (_player == null)
            {
                btnContinue.Enabled = false;
                /*
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    btnContinue.Enabled = true;
                }
                else
                {
                    btnContinue.Enabled = false;
                }
                */
            }
            else
            {
                btnContinue.Enabled = true;
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

                Hide();
                rpgForm.Show();
            }
#pragma warning disable CS0252
            if (cboStyle.SelectedItem == "Text Based Gameplay")
#pragma warning restore CS0252
            {
                File.Delete(PLAYER_DATA_FILE_NAME);

                Hide();
                Process.Start(crp.TextReturnPath() + "\\RPG_Console.exe");
            }
        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
#pragma warning disable CS0252
            if (cboStyle.SelectedItem == "GUI Based Gameplay")
#pragma warning restore CS0252 
            {
                _player = PlayerDataMapper.CreateFromDatabase();
                /*
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));

                    Hide();
                    rpgForm.Show();
                }
                */
            }
#pragma warning disable CS0252
            if (cboStyle.SelectedItem == "Text Based Gameplay")
#pragma warning restore CS0252
            {
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));

                    Hide();
                    Process.Start(crp.TextReturnPath() + "\\RPG_Console.exe");
                }
            }
        }
    }
}
