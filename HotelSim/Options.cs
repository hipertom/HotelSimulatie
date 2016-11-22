using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace HotelSim
{
    public partial class Options : Form
    {
        // variabelen instantieren
        private int HteInSeconds;
        private int QueueDeathHTE;
        private int StairHTE;
        private int RoomCleaningHTE;
        private int MovieDurationHTE;
        private int EatTimeHTE;
        private string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HotelSimulatieInstellingen.Json");
        private string layoutDefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Hotel5.layout");

        public Options()
        {
            InitializeComponent();

            #region Opmaak van het form, panel, buttons en textboxen

            // velden gelijk zetten aan wat in de config staat
            // config uitlezen
            ConfigJsonModel config = new ConfigJsonModel();
            config.ReadConfigJson(configPath);
            // velden gelijk zetten
            if (config.HtesPerSecond != 0)
                textBoxHtePerSecond.Text = config.HtesPerSecond.ToString().Replace(",", ".");

            if (config.QueueDeathHTE != 0)
                textBoxQueueDeathHTE.Text = config.QueueDeathHTE.ToString().Replace(",", ".");

            if (config.StairDistanceHTE != 0)
                textBoxStairHTE.Text = config.StairDistanceHTE.ToString().Replace(",", ".");

            if (config.RoomCleaningHTE != 0)
                textBoxRoomCleaningHTE.Text = config.RoomCleaningHTE.ToString().Replace(",", ".");

            if (config.MovieDurationHTE != 0)
                textBoxMovieDuration.Text = config.MovieDurationHTE.ToString().Replace(",", ".");

            if (config.EatHTE != 0)
                textBoxEatHTE.Text = config.EatHTE.ToString().Replace(",", ".");

            // disable color change on hover van buttons
            buttonStart.FlatAppearance.MouseOverBackColor = buttonStart.BackColor;
            buttonBrowse.FlatAppearance.MouseOverBackColor = buttonBrowse.BackColor;


            // loop door alle text boxes en maak een list om bepaalde uit te sluiten!
            List<Control> exclude = new List<Control>();
            exclude.Add(textBoxFilePath);
            foreach (Control x in panelContainer.Controls) // loop door alle items in panel 
            {
                if (x is TextBox && !exclude.Contains(x)) // Controleren of huidige item in loop textbox is en niet in exclude staat
                {
                    ((TextBox)x).AutoSize = false;
                    ((TextBox)x).Size = new System.Drawing.Size(100, 18);
                }
            }

            #endregion

            // standaard path voor layout bestand zodat je in een keer door kan klikken als het bestand op je desktop staat
            textBoxFilePath.Text = layoutDefaultPath;

        }// end ctor

        private void buttonStart_Click(object sender, EventArgs e)
        {
            string path = textBoxFilePath.Text;
            List<LayoutJsonModel> layout = null;

            if (!string.IsNullOrWhiteSpace(path))
            {
                layout = ReadLayoutJson(path);
            }

            // check if is numeriek
            bool isNumeric1 = int.TryParse(textBoxHtePerSecond.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out HteInSeconds);
            bool isNumeric2 = int.TryParse(textBoxQueueDeathHTE.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out QueueDeathHTE);
            bool isNumeric3 = int.TryParse(textBoxStairHTE.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out StairHTE);
            bool isNumeric4 = int.TryParse(textBoxRoomCleaningHTE.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out RoomCleaningHTE);
            bool isNumeric5 = int.TryParse(textBoxMovieDuration.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out MovieDurationHTE);
            bool isNumeric6 = int.TryParse(textBoxEatHTE.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out EatTimeHTE);

            // check of waardes cijfers zijn en niet leeg
            if (!isNumeric1 || !isNumeric2 || !isNumeric3 || !isNumeric4 || !isNumeric5)
            {
                labelError.Text = "Één of meerdere velden zijn niet (correct) ingevuld";
            }
            else
            {
                // waardes moeten groter dan 0 zijn
                if (HteInSeconds <= 0 || QueueDeathHTE <= 0 || StairHTE <= 0 || RoomCleaningHTE <= 0 || MovieDurationHTE <= 0 || EatTimeHTE <= 0)
                {
                    labelError.Text = "Één of meerdere waardes zijn 0 of kleiner!";
                }
                else
                {
                    // of Json list is aan gemaakt
                    if (layout == null)
                    {
                        labelError.Text = "Er is geen (Json) bestand geselecteerd!";
                    }
                    else
                    {
                        // reset
                        labelError.Text = null;

                        // opslaan gegevens
                        ConfigJsonModel config = new ConfigJsonModel()
                        {
                            HtesPerSecond = HteInSeconds,
                            QueueDeathHTE = QueueDeathHTE,
                            RoomCleaningHTE = RoomCleaningHTE,
                            StairDistanceHTE = StairHTE,
                            MovieDurationHTE = MovieDurationHTE,
                            EatHTE = EatTimeHTE
                        };
                        config.WriteConfigJson(configPath);

                        // sluit huidige & open nieuw form
                        this.Hide();
                        var sim = new Simulation(layout, config);
                        sim.Closed += (s, args) => this.Close();
                        sim.Closed += (s, args) => HotelEvents.HotelEventManager.Stop();
                        sim.Show();
                    }
                }
            }
        }// end buttonStart_Click

        // wanneer je browsed naar file
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var path = openFileDialog.FileName;
                textBoxFilePath.Text = path;
            }
        }

        // Json uitlezen en dan een list van maken voor layout
        private List<LayoutJsonModel> ReadLayoutJson(string path)
        {
            try
            {
                StreamReader file = new StreamReader(path);
                string json = file.ReadToEnd();
                file.Close();
                return JsonConvert.DeserializeObject<List<LayoutJsonModel>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
