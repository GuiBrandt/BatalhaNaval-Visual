using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BatalhaNaval;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading.Tasks;

namespace BatalhaNaval_Visual
{
    public partial class FormTabuleiro : Form
    {
        ClienteP2P _cliente;
        Tabuleiro _tabuleiro;
        IntPtr _cursor = IntPtr.Zero;
        Bitmap _thumb = null;
        TipoDeNavio? _dragged = null;
        int _dir = 3;
        Dictionary<Tiro, ResultadoDeTiro> _tirosDados;
        List<Tiro> _tirosRecebidos;

        /// <summary>
        /// Construtor
        /// </summary>
        public FormTabuleiro()
        {
            InitializeComponent();

            _tabuleiro = new Tabuleiro();
            _tirosDados = new Dictionary<Tiro, ResultadoDeTiro>();
            _tirosRecebidos = new List<Tiro>();
            pbTabuleiro.AllowDrop = true;
        }

        /// <summary>
        /// Obtém a picture box para um tipo de navio
        /// </summary>
        /// <param name="t">Tipo de navio</param>
        /// <returns>A picture box do tipo de navio dado</returns>
        private PictureBox GetPictureBoxParaTipoDeNavio(TipoDeNavio t)
        {
            switch (t)
            {
                case TipoDeNavio.PortaAvioes:
                    return pbPortaAvioes;
                case TipoDeNavio.Encouracado:
                    return pbEncouracado;
                case TipoDeNavio.Cruzador:
                    return pbCruzador;
                case TipoDeNavio.Destroier:
                    return pbDestroier;
                default:
                    return pbSubmarino;
            }
        }

        /// <summary>
        /// Obtém uma imagem para um tipo de navio
        /// </summary>
        /// <param name="t">Tipo de navio</param>
        /// <param name="d">Direção do navio</param>
        /// <returns>Uma imagem para o tipo de navio pedido</returns>
        private Image GetImagemParaTipoDeNavio(TipoDeNavio t, int d)
        {
            Bitmap bmp = new Bitmap(GetPictureBoxParaTipoDeNavio(t).Image);

            switch (d)
            {
                case 0:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 1:
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 2:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            Size size;
            switch (d)
            {
                case 1:
                case 3:
                    size = new Size(pbTabuleiro.Width / 10 * t.Tamanho(), pbTabuleiro.Height / 10);
                    break;
                default:
                    size = new Size(pbTabuleiro.Width / 10, pbTabuleiro.Height / 10 * t.Tamanho());
                    break;
            }

            return new Bitmap(bmp, size);
        }

        /// <summary>
        /// Desenho da picture box do tabuleiro
        /// </summary>
        private void pbTabuleiro_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float wColuna = (float)pbTabuleiro.Width / _tabuleiro.NumeroDeColunas,
                hLinha = (float)pbTabuleiro.Height / _tabuleiro.NumeroDeLinhas;

            using (Pen p = new Pen(Color.Black, 2))
            {
                for (int i = 1; i < _tabuleiro.NumeroDeLinhas; i++)
                    g.DrawLine(p, 0, i * hLinha, pbTabuleiro.Width, i * hLinha);

                for (int i = 1; i < _tabuleiro.NumeroDeColunas; i++)
                    g.DrawLine(p, i * wColuna, 0, i * wColuna, pbTabuleiro.Height);
            }

            foreach (KeyValuePair<int[], TipoDeNavio> navio in _tabuleiro.Navios)
            {
                int d = navio.Key[2];

                PointF pos;

                switch (d)
                {
                    case 1:
                        pos = new PointF(wColuna * (navio.Key[0] + 1 - navio.Value.Tamanho()), hLinha * navio.Key[1]);
                        break;
                    case 2:
                        pos = new PointF(wColuna * navio.Key[0], hLinha * (navio.Key[1] + 1 - navio.Value.Tamanho()));
                        break;
                    default:
                        pos = new PointF(wColuna * navio.Key[0], hLinha * navio.Key[1]);
                        break;
                }

                Image img = GetImagemParaTipoDeNavio(navio.Value, d);
                g.DrawImage(img, pos);
            }
        }

        /// <summary>
        /// Informações de ícone
        /// </summary>
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        /// <summary>
        /// Evento de mouse apertado em qualquer uma das pictureBox com imagem de navio
        /// </summary>
        private void pbNavio_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;

            Bitmap bmp = (Bitmap)pb.Image;
            _dragged = (TipoDeNavio)Enum.Parse(typeof(TipoDeNavio), (string)pb.Tag);
            CreateThumbnail(_dragged ?? TipoDeNavio.Submarino);

            DoDragDrop((string)pb.Tag, DragDropEffects.All);
        }

        /// <summary>
        /// Cria o thumbnail
        /// </summary>
        /// <param name="bmp">Bitmap para a imagem do cursor</param>
        void CreateThumbnail(TipoDeNavio t)
        {
            _thumb = (Bitmap)GetImagemParaTipoDeNavio(t, _dir);

            IntPtr ptr = _thumb.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = 0;
            tmp.yHotspot = 0;

            if (_dir == 1)
            {
                tmp.xHotspot = _thumb.Width;
                tmp.yHotspot = _thumb.Height / 2;
            }
            else if (_dir == 2)
            {
                tmp.xHotspot = _thumb.Width / 2;
                tmp.yHotspot = _thumb.Height;
            }
            else if (_dir == 3)
            {
                tmp.xHotspot = 0;
                tmp.yHotspot = _thumb.Height / 2;
            }
            else if (_dir == 0)
            {
                tmp.xHotspot = _thumb.Width / 2;
                tmp.yHotspot = 0;
            }

            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);

            _cursor = ptr;
        }

        /// <summary>
        /// Evento de DragDrop na picture box de tabuleiro
        /// </summary>
        private void pbTabuleiro_DragDrop(object sender, DragEventArgs e)
        {
            TipoDeNavio tipoDeNavio = _dragged ?? TipoDeNavio.Submarino;

            Point p = pbTabuleiro.PointToClient(new Point(e.X, e.Y));
            int x = p.X * _tabuleiro.NumeroDeColunas / pbTabuleiro.Width;
            int y = p.Y * _tabuleiro.NumeroDeLinhas / pbTabuleiro.Height;

            try
            {
                _tabuleiro.PosicionarNavio(tipoDeNavio, x, y, _dir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Eita nóis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (_tabuleiro.Contar(tipoDeNavio) == tipoDeNavio.Limite())
                GetPictureBoxParaTipoDeNavio(tipoDeNavio).Visible = false;

            pbTabuleiro.Invalidate();

            if (_tabuleiro.EstaCompleto())
            {
                DialogResult r = MessageBox.Show(this, "Tabuleiro completo, continuar?", "Batalha Naval", MessageBoxButtons.YesNo);

                if (r == DialogResult.Yes)
                {
                    Width -= tlpNavios.Width;
                    tlpNavios.Visible = false;
                    splitterLeft.Visible = false;

                    LoginDialog login = new LoginDialog();
                    r = login.ShowDialog(this);

                    if (r == DialogResult.OK)
                    {
                        _cliente = new ClienteP2P(login.User, _tabuleiro);
                        _cliente.OnClienteDisponivel += Cliente_OnClienteDisponivel;
                        _cliente.OnClienteRequisitandoConexao += Cliente_OnClienteRequisitandoConexao;
                        _cliente.OnClienteConectado += Cliente_OnClienteConectado;
                        _cliente.OnClienteDesconectado += Cliente_OnClienteDesconectado;
                        _cliente.OnDarTiro += Cliente_OnDarTiro;
                        _cliente.OnResultadoDeTiro += Cliente_OnResultadoDeTiro;
                        _cliente.OnTiroRecebido += Cliente_OnTiroRecebido;
                        _cliente.Iniciar();

                        panelConectar.Visible = true;
                        Width += panelConectar.Width;
                        splitterRight.Visible = true;

                        return;
                    }
                    else
                    {
                        Width += tlpNavios.Width;
                        tlpNavios.Visible = true;
                        splitterLeft.Visible = true;
                    }
                }

                _tabuleiro = new Tabuleiro();
                pbTabuleiro.Invalidate();

                foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                    GetPictureBoxParaTipoDeNavio(t).Visible = true;
            }
        }

        /// <summary>
        /// Recebeu um tiro
        /// </summary>
        /// <param name="t">Tiro recebido</param>
        private void Cliente_OnTiroRecebido(Tiro t)
        {
            MessageBox.Show("Tiro recebido em " + t.X + " " + t.Y);
            _tirosRecebidos.Add(t);
        }

        /// <summary>
        /// Recebeu o resultado de um tiro dado
        /// </summary>
        /// <param name="t">Tiro dado</param>
        /// <param name="resultado">Resultado do tiro</param>
        private void Cliente_OnResultadoDeTiro(Tiro t, ResultadoDeTiro resultado)
        {
            MessageBox.Show("Resultado: " + resultado);
            _tirosDados.Add(t, resultado);
        }

        /// <summary>
        /// Sinaliza que deve-se dar um tiro
        /// </summary>
        private void Cliente_OnDarTiro()
        {
            pbInimigo.Enabled = true;
            timeout.Start();
        }

        /// <summary>
        /// Evento chamado quando o cliente antes conectado se desconecta
        /// </summary>
        /// <param name="addr">Endereço do cliente desconectado</param>
        private void Cliente_OnClienteDesconectado(IPAddress addr)
        {
            _tabuleiro = new Tabuleiro();
            pbTabuleiro.Invalidate();

            pbInimigo.Visible = splitterTabuleiros.Visible = false;

            tlpNavios.Visible = true;
            _cliente = null;
        }

        /// <summary>
        /// Evento chamado quando um cliente se conecta a este
        /// </summary>
        /// <param name="addr">Endereço do cliente conectado</param>
        private void Cliente_OnClienteConectado(IPAddress addr)
        {
            Invoke(new Action(() =>
            {
                Width -= panelConectar.Width;
                panelConectar.Visible = false;
                splitterRight.Visible = false;
                splitterTabuleiros.Visible = true;
                pbInimigo.Visible = true;
            }));
        }

        /// <summary>
        /// Evento chamado quando um cliente quer se conectar com este
        /// </summary>
        /// <param name="addr">Endereço do cliente</param>
        /// <returns>True se deve aceitar a conexão e falso se não</returns>
        private bool Cliente_OnClienteRequisitandoConexao(IPAddress addr)
        {
            DialogResult r = DialogResult.No;
            Invoke(new Action(() => { 
                r = MessageBox.Show(this, string.Format("{0} ({1}) quer jogar com você. Aceitar?", _cliente.NomeRemoto, addr), "Batalha Naval", MessageBoxButtons.YesNo); 
            }));

            if (r == DialogResult.Yes)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Evento chamado quando um cliente está disponível para conexão
        /// </summary>
        /// <param name="addr">Endereço do cliente</param>
        private void Cliente_OnClienteDisponivel(System.Net.IPAddress addr)
        {
            if (InvokeRequired)
                Invoke(new Action(() => { Cliente_OnClienteDisponivel(addr); }));
            else if (!cbDisponiveis.Items.Contains(addr))
                cbDisponiveis.Items.Add(addr);
        }

        /// <summary>
        /// Evento de feedback do form (para o thumbnail)
        /// </summary>
        private void Form_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (_cursor == IntPtr.Zero) return;

            e.UseDefaultCursors = false;
            Cursor.Current = new Cursor(_cursor);
        }

        /// <summary>
        /// Evento de aceitação de dragdrop da picture box do tabuleiro
        /// </summary>
        private void pbTabuleiro_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Evento de mouse levantado no form
        /// </summary>
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            _cursor = IntPtr.Zero;
        }

        /// <summary>
        /// Clique no botão Conectar
        /// </summary>
        private void btnConectar_Click(object sender, EventArgs e)
        {
            btnConectar.Enabled = false;
            
            if (cbDisponiveis.SelectedIndex < 0)
            {
                btnConectar.Enabled = true;
                return;
            }
            
            IPAddress addr;

            try
            {
                addr = (IPAddress)cbDisponiveis.SelectedItem;
            }
            catch
            {
                MessageBox.Show(this, "Endereço inválido", "Batalha Naval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (!_cliente.SolicitarConexao(addr))
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(this, "O cliente remoto recusou a conexão =(", "Batalha Naval", MessageBoxButtons.OK);
                            btnConectar.Enabled = true;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show(this, "Não foi possível conectar ao cliente remoto. Tento novamente mais tarde.\r\n" + ex.Message, "Batalha Naval", MessageBoxButtons.OK);
                        cbDisponiveis.Items.RemoveAt(cbDisponiveis.SelectedIndex);
                        btnConectar.Enabled = true;
                    }));
                }
            });
        }

        /// <summary>
        /// Evento de tecla pressionada
        /// </summary>
        private void pbTabuleiro_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _dir = 1;
                    break;

                case Keys.Right:
                    _dir = 3;
                    break;

                case Keys.Up:
                    _dir = 2;
                    break;

                case Keys.Down:
                    _dir = 0;
                    break;
            }

            tlpNavios.Refresh();

            if (_cursor == IntPtr.Zero || _thumb == null)
                return;

            CreateThumbnail(_dragged ?? TipoDeNavio.Submarino);
            Cursor.Current = new Cursor(_cursor);
        }

        /// <summary>
        /// Cancela a operação de conexão e volta para a criação do tabuleiro
        /// </summary>
        private void btnVoltar_Click(object sender, EventArgs e)
        {
            _cliente.Close();
            _cliente = null;

            panelConectar.Visible = splitterRight.Visible = false;
            tlpNavios.Visible = splitterLeft.Visible = true;

            _tabuleiro = new Tabuleiro();
            pbTabuleiro.Invalidate();

            foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                GetPictureBoxParaTipoDeNavio(t).Visible = true;
        }

        /// <summary>
        /// Limpa o tabuleiro
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            _tabuleiro = new Tabuleiro();
            pbTabuleiro.Invalidate();

            foreach (TipoDeNavio t in (TipoDeNavio[])Enum.GetValues(typeof(TipoDeNavio)))
                GetPictureBoxParaTipoDeNavio(t).Visible = true;
        }

        /// <summary>
        /// Redesenha o tabuleiro do inimigo quando o mouse se move sobre ele
        /// </summary>
        private void pbInimigo_MouseMove(object sender, MouseEventArgs e)
        {
            pbInimigo.Invalidate();
        }

        /// <summary>
        /// Evento de desenho do tabuleiro inimigo
        /// </summary>
        private void pbInimigo_Paint(object sender, PaintEventArgs e)
        {
            Image cursor = new Bitmap(Properties.Resources.cursor, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10));
            Image hit = new Bitmap(Properties.Resources.acerto, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10));
            Image miss = new Bitmap(Properties.Resources.agua, new Size(pbInimigo.Width / 10, pbInimigo.Height / 10));
            
            foreach (KeyValuePair<Tiro, ResultadoDeTiro> pair in _tirosDados)
            {
                PointF pos = new PointF(pair.Key.X * pbInimigo.Width / 10, pair.Key.Y * pbInimigo.Height / 10);

                if (pair.Value.HasFlag(ResultadoDeTiro.Acertou))
                    e.Graphics.DrawImage(hit, pos);
                else
                    e.Graphics.DrawImage(miss, pos);
            }

            if (!pbInimigo.Enabled)
                return; 

            Point p = pbInimigo.PointToClient(Cursor.Position);
            int x = p.X * _tabuleiro.NumeroDeColunas / pbInimigo.Width;
            int y = p.Y * _tabuleiro.NumeroDeLinhas / pbInimigo.Height;

            e.Graphics.DrawImage(cursor, new PointF(x * pbInimigo.Width / 10, y * pbInimigo.Height / 10));
        }

        /// <summary>
        /// Timeout do tiro
        /// </summary>
        private void timeout_Tick(object sender, EventArgs e)
        {
            pbInimigo.Enabled = false;
        }

        /// <summary>
        /// Clique na picturebox do inimigo
        /// </summary>
        private void pbInimigo_MouseClick(object sender, MouseEventArgs e)
        {
            if (!pbInimigo.Enabled) return;

            Point p = pbInimigo.PointToClient(new Point(e.X, e.Y));
            int x = p.X * _tabuleiro.NumeroDeColunas / pbInimigo.Width;
            int y = p.Y * _tabuleiro.NumeroDeLinhas / pbInimigo.Height;

            _cliente.DarTiro(x, y);
            pbInimigo.Enabled = false;
        }
    }
}
