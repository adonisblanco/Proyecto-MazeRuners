using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto_csharp.Ejercicios;
using Proyecto_csharp.Formularios_Maze_Runners;

namespace Proyecto_csharp.Ejercicios
{
    public partial class Menuprincipal : Form
    {
        
        public Menuprincipal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Historia();
        }
        private void Historia()
        {
            /* abrir el formulario historia desde
             este formulario*/
            panelvisor.Controls.Clear();
            Historia ctl = new Historia();
            ctl.Dock = DockStyle.Fill;
            panelvisor.Controls.Add(ctl);
            ctl.Show();
        }

        private void btnE2_Click(object sender, EventArgs e)
        {
            Juego();
        }
        private void Juego()
        {   
            /*abrir el formulario donde
             estara el juego */
            panelvisor.Controls.Clear();
            Game ctl = new Game();
            ctl.Dock = DockStyle.Fill;
            panelvisor.Controls.Add(ctl);
            ctl.Show();

        }

       

        private void panelvisor_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Menuprincipal_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            panelvisor.Controls.Clear();
            Reglas ctl = new Reglas();
            ctl.Dock = DockStyle.Fill;
            panelvisor.Controls.Add(ctl);
            ctl.Show();
        }
    }
}
