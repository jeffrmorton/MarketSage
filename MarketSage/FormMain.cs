using System;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using MarketSage.Library;
using System.Web;
using ZedGraph;

/* MarketSage
   Copyright © 2008, 2009 Jeffrey Morton
 
   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
 
   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA */

namespace MarketSage
{
    public partial class FormMain : Form
    {
        private string _index = "";
        private int _quoteImportIteration = 0;
        private DateTime _agentQuotesStart;
        private DateTime _agentQuotesStop;
        string[] subdirectoryEntries;
        string regex = @"^.*\\";

        #region Global variables
        private bool _isShown = true;
        private PluginServices _pluginService;
        private StrategyTester _strategyTester = new StrategyTester(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs);
        private DataSource _dataSource = new DataSource(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryWork, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs, Properties.Settings.Default.GlobalMarket);
        private string _agentModelFileBaseLine = Properties.Settings.Default.GlobalMarket + "-BASELINE.txt";
        private double _agentModelHistoricBaseline = 0;
        private double _agentModel10YearBaseline = 0;
        private double _agentModel5YearBaseline = 0;
        private double _agentModel3YearBaseline = 0;
        private double _agentModel1YearBaseline = 0;
        private double _agentModel6MonthBaseline = 0;
        private double _agentModel3MonthBaseline = 0;
        private double _agentModel1MonthBaseline = 0;
        #endregion
        #region Data analysis variables
        // analyze
        private DataTable _datasetRisk;
        private Engine _engineRisk;
        private Market _marketRisk;
        private Observer _observerRisk;
        private int _counterAnalysis;
        private string _instrumentAnalysis;
        private string _instrumentNameAnalysis;
        private DateTime _startDateAnalyze;
        private DateTime _endDateAnalyze;
        private Instrument _instr;
        private string _reportRiskReward;
        //private string _reportDataQuality;
        private ArrayList _marketDateExceptions2;
        private ArrayList _holidaySchedule;
        private double[] _marketHistogram = new double[367];
        private int[] _marketHistogramCounter = new int[367];
        private double[] _marketHistogramAverage = new double[367];
        private double[] _marketMonthHistogram = new double[33];
        private int[] _marketMonthHistogramCounter = new int[33];
        private double[] _marketMonthHistogramAverage = new double[33];
        private double[] _marketLunarHistogram = new double[8];
        private int[] _marketLunarHistogramCounter = new int[8];
        private double[] _marketLunarHistogramAverage = new double[8];
        private double[] _marketWeekHistogram = new double[8];
        private int[] _marketWeekHistogramCounter = new int[8];
        private double[] _marketWeekHistogramAverage = new double[8];
        private BarItem myBar = new BarItem("Average Change");
        private LineItem myLine = new LineItem("Today");
        private BarItem myBarLunar = new BarItem("Average Change");
        private LineItem myLineLunar = new LineItem("Today");
        private string _agentModelSelectionSampleLength;
        private string _fileStageRiskReward = Properties.Settings.Default.GlobalMarket + "-STAGE-RISK-REWARD.xml";
        #endregion
        #region Modelling variables
        // model
        //private string _fileTargetEntry = Properties.Settings.Default.GlobalMarket + "-STRATEGY-LONG-TARGET.xml";
        private int _maxIndicators = 20;
        private SoundPlayer _soundPlayer = new SoundPlayer(MarketSage.Properties.Resources.Alert);
        public bool _isFinished;
        private int _counterNeural = 0;
        private Market _marketNeural;
        private Network bp;
        private TrainingSet trainingSet;
        //public const double STATE_FALSE = 0.2;
        //public const double STATE_TRUE = 0.8;
        // random chance
        private Engine _agentRandomChanceEngine;
        private bool _agentRandomChanceDisposition = true;  //  true = long; false = short;
        private Market _agentRandomChanceMarket;
        private Observer _agentRandomChanceObserver;
        private DateTime _agentRandomChanceStart;
        private DateTime _agentRandomChanceStop;
        private string _agentRandomChanceLog;
        private string _agentRandomChanceScore;
        private bool _agentRandomChanceWin;
        private DataTable _datasetModelSelectionMaxEntryQueue;
        private DataTable _datasetModelSelectionMaxExitQueue;
        private string _fileModelSelectionMaxEntryQueue = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-MAX-LONG-QUEUE.xml";
        private string _fileModelSelectionMaxExitQueue = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-MAX-SHORT-QUEUE.xml";
        // natural selection (historic)
        private Engine _agentNaturalSelectionISEngine;
        private bool _agentNaturalSelectionISDisposition = true;  //  true = long; false = short;
        private bool _agentNaturalSelectionISCombo = false;
        private Market _agentNaturalSelectionISMarket;
        private Observer _agentNaturalSelectionISObserver;
        private DateTime _agentNaturalSelectionISStart;
        private DateTime _agentNaturalSelectionISStop;
        private string _agentNaturalSelectionISLog;
        private string _agentNaturalSelectionISScore;
        private bool _agentNaturalSelectionISWin;
        private DataTable _datasetModelSelectionMaxEntryCache;
        private DataTable _datasetModelSelectionMaxExitCache;
        private DataTable _datasetModelSelectionMaxEntry;
        private DataTable _datasetModelSelectionMaxExit;
        private string _fileModelSelectionMaxEntryCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-MAX-LONG-CACHE.xml";
        private string _fileModelSelectionMaxExitCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-MAX-SHORT-CACHE.xml";
        private string _fileModelSelectionMaxEntry = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-MAX-LONG.xml";
        private string _fileModelSelectionMaxExit = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-MAX-SHORT.xml";
        private string _strategyChromosone;
        private ArrayList _agentNaturalSelectionQueue = new ArrayList();
        private ArrayList _agentModelGeneticQueueEntry = new ArrayList();
        private ArrayList _agentModelGeneticQueueExit = new ArrayList();
        // natural selection (sample)
        private Engine _agentNaturalSelectionOOSEngine;
        private bool _agentNaturalSelectionOSDisposition = true;  //  true = long; false = short;
        private Market _agentNaturalSelectionOOSMarket;
        private Observer _agentNaturalSelectionOOSObserver;
        private DateTime _agentNaturalSelectionOOSStart;
        private DateTime _agentNaturalSelectionOOSStop;
        private string _agentNaturalSelectionOOSLog;
        private string _agentNaturalSelectionOOSScore;
        private bool _agentNaturalSelectionOOSWin;
        private DataTable _datasetModelSelection10YearEntry;
        private DataTable _datasetModelSelection10YearExit;
        private DataTable _datasetModelSelection5YearEntry;
        private DataTable _datasetModelSelection5YearExit;
        private DataTable _datasetModelSelection3YearEntry;
        private DataTable _datasetModelSelection3YearExit;
        private DataTable _datasetModelSelection1YearEntry;
        private DataTable _datasetModelSelection1YearExit;
        private DataTable _datasetModelSelection10YearEntryCache;
        private DataTable _datasetModelSelection10YearExitCache;
        private DataTable _datasetModelSelection5YearEntryCache;
        private DataTable _datasetModelSelection5YearExitCache;
        private DataTable _datasetModelSelection3YearEntryCache;
        private DataTable _datasetModelSelection3YearExitCache;
        private DataTable _datasetModelSelection1YearEntryCache;
        private DataTable _datasetModelSelection1YearExitCache;

        private string _fileModelSelection10YearEntry = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-10YEAR-LONG.xml";
        private string _fileModelSelection10YearExit = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-10YEAR-SHORT.xml";
        private string _fileModelSelection10YearEntryCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-10YEAR-LONG-CACHE.xml";
        private string _fileModelSelection10YearExitCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-10YEAR-SHORT-CACHE.xml";

        private string _fileModelSelection5YearEntry = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-5YEAR-LONG.xml";
        private string _fileModelSelection5YearExit = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-5YEAR-SHORT.xml";
        private string _fileModelSelection5YearEntryCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-5YEAR-LONG-CACHE.xml";
        private string _fileModelSelection5YearExitCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-5YEAR-SHORT-CACHE.xml";

        private string _fileModelSelection3YearEntry = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-3YEAR-LONG.xml";
        private string _fileModelSelection3YearExit = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-3YEAR-SHORT.xml";
        private string _fileModelSelection3YearEntryCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-3YEAR-LONG-CACHE.xml";
        private string _fileModelSelection3YearExitCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-3YEAR-SHORT-CACHE.xml";

        private string _fileModelSelection1YearEntry = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-1YEAR-LONG.xml";
        private string _fileModelSelection1YearExit = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-1YEAR-SHORT.xml";
        private string _fileModelSelection1YearEntryCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-1YEAR-LONG-CACHE.xml";
        private string _fileModelSelection1YearExitCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-SELECTION-1YEAR-SHORT-CACHE.xml";

        private string _strategyChromosoneOOS;

        // genetic evolution
        private Engine _agentGeneticEvolutionEngine;
        private Market _agentGeneticEvolutionMarket;
        private Observer _agentGeneticEvolutionObserver;
        private DateTime _agentGeneticEvolutionStart;
        private DateTime _agentGeneticEvolutionStop;
        private string _agentGeneticEvolutionLog;
        private string _agentGeneticEvolutionScore;
        private bool _agentGeneticEvolutionWin;

        private DataTable _datasetModelGeneticMax;
        private DataTable _datasetModelGeneticMaxCache;

        private DataTable _datasetModelGeneticMaxQueue;


        private DataTable _datasetModelGenetic10Year;
        private DataTable _datasetModelGenetic10YearCache;
        private DataTable _datasetModelGenetic5Year;
        private DataTable _datasetModelGenetic5YearCache;
        private DataTable _datasetModelGenetic3Year;
        private DataTable _datasetModelGenetic3YearCache;
        private DataTable _datasetModelGenetic1Year;
        private DataTable _datasetModelGenetic1YearCache;
        private DataTable _datasetModelGenetic6Month;
        private DataTable _datasetModelGenetic6MonthCache;
        private DataTable _datasetModelGenetic3Month;
        private DataTable _datasetModelGenetic3MonthCache;
        private DataTable _datasetModelGenetic1Month;
        private DataTable _datasetModelGenetic1MonthCache;

        private string _fileModelGeneticMax = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-MAX.xml";
        private string _fileModelGeneticMaxCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-MAX-CACHE.xml";

        private string _fileModelGeneticMaxQueue = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-MAX-QUEUE.xml";


        private string _fileModelGenetic10Year = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-10YEAR.xml";
        private string _fileModelGenetic10YearCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-10YEAR-CACHE.xml";
        private string _fileModelGenetic5Year = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-5YEAR.xml";
        private string _fileModelGenetic5YearCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-5YEAR-CACHE.xml";
        private string _fileModelGenetic3Year = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-3YEAR.xml";
        private string _fileModelGenetic3YearCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-3YEAR-CACHE.xml";
        private string _fileModelGenetic1Year = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-1YEAR.xml";
        private string _fileModelGenetic1YearCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-1YEAR-CACHE.xml";
        private string _fileModelGenetic6Month = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-6MONTH.xml";
        private string _fileModelGenetic6MonthCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-6MONTH-CACHE.xml";
        private string _fileModelGenetic3Month = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-3MONTH.xml";
        private string _fileModelGenetic3MonthCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-3MONTH-CACHE.xml";
        private string _fileModelGenetic1Month = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-1MONTH.xml";
        private string _fileModelGenetic1MonthCache = Properties.Settings.Default.GlobalMarket + "-STRATEGY-GENETIC-1MONTH-CACHE.xml";

        private bool _flagSelectionSampleCombo = false;
        #endregion
        #region Forecasting Variables

        // A
        private int _buyRecommendationsPercentageOfMarketA;
        private int _sellRecommendationsPercentageOfMarketA;
        private DateTime _startDateForecastA;
        private DateTime _endDateForecastA;
        private DataTable _datasetRecommendationsBuyConsensusA;
        private DataTable _datasetRecommendationsSellConsensusA;
        private Engine _engineRecommendationsBuyA;
        private Engine _engineRecommendationsSellA;
        private Market _marketRecommendationsBuyA;
        private Market _marketRecommendationsSellA;
        private int _counterRecommendationsBuyA;
        private int _counterRecommendationsSellA;
        private string _instrumentRecommendationsBuySymbolA;
        private string _instrumentRecommendationsSellSymbolA;
        private string _instrumentRecommendationsBuyNameA;
        private string _instrumentRecommendationsSellNameA;
        private DataTable _datasetRecommendationsBuyA;
        private DataTable _datasetRecommendationsSellA;
        private string _fileRecommendationsBuyA = Properties.Settings.Default.GlobalMarket + "-RECOMMENDATIONS-A-BUY.xml";
        private string _fileRecommendationsSellA = Properties.Settings.Default.GlobalMarket + "-RECOMMENDATIONS-A-SELL.xml";
        private int _counterRecommendationsBuyStrategyRankA;
        private int _counterRecommendationsSellStrategyRankA;
        private string _strategyRecommendationsBuyStrategyNameA;
        private string _strategyRecommendationsSellStrategyNameA;
        private System.Collections.Hashtable _currentRecommendationsBuyInstrumentsA;
        private System.Collections.Hashtable _currentRecommendationsSellInstrumentsA;

        // B
        private int _buyRecommendationsPercentageOfMarketB;
        private int _sellRecommendationsPercentageOfMarketB;
        private DateTime _startDateForecastB;
        private DateTime _endDateForecastB;
        private DataTable _datasetRecommendationsBuyConsensusB;
        private DataTable _datasetRecommendationsSellConsensusB;
        private Engine _engineRecommendationsBuyB;
        private Engine _engineRecommendationsSellB;
        private Market _marketRecommendationsBuyB;
        private Market _marketRecommendationsSellB;
        private int _counterRecommendationsBuyB;
        private int _counterRecommendationsSellB;
        private string _instrumentRecommendationsBuySymbolB;
        private string _instrumentRecommendationsSellSymbolB;
        private string _instrumentRecommendationsBuyNameB;
        private string _instrumentRecommendationsSellNameB;
        private DataTable _datasetRecommendationsBuyB;
        private DataTable _datasetRecommendationsSellB;
        private string _fileRecommendationsBuyB = Properties.Settings.Default.GlobalMarket + "-RECOMMENDATIONS-B-BUY.xml";
        private string _fileRecommendationsSellB = Properties.Settings.Default.GlobalMarket + "-RECOMMENDATIONS-B-SELL.xml";
        private int _counterRecommendationsBuyStrategyRankB;
        private int _counterRecommendationsSellStrategyRankB;
        private string _strategyRecommendationsBuyStrategyNameB;
        private string _strategyRecommendationsSellStrategyNameB;
        private System.Collections.Hashtable _currentRecommendationsBuyInstrumentsB;
        private System.Collections.Hashtable _currentRecommendationsSellInstrumentsB;
        #endregion
        #region Trading variables
        private QuoteServices _quoteService = new QuoteServices(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs);
        private string _filePortfolioA = Properties.Settings.Default.GlobalMarket + "-PORTFOLIO-A.xml";
        private string _filePortfolioB = Properties.Settings.Default.GlobalMarket + "-PORTFOLIO-B.xml";
        private Portfolio _portfolioA;
        private Portfolio _portfolioB;
        private DataSet _datasetPortfolioA;
        private DataSet _datasetPortfolioB;
        #endregion
        #region Main
        public FormMain()
        {
            InitializeComponent();

            this.cpuCounter = new System.Diagnostics.PerformanceCounter();
            this.ramCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            this.cpuCounter.CategoryName = "Processor";
            this.cpuCounter.CounterName = "% Processor Time";
            this.cpuCounter.InstanceName = "_Total";


        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            //Properties.Settings.Default.AccountInitialBalance = 10000.00;
            Properties.Settings.Default.GlobalDirectoryRoot = "";

            // main
            this.Size = Properties.Settings.Default.FormMainSize;
            this.Location = Properties.Settings.Default.FormMainLocation;
            this.WindowState = Properties.Settings.Default.FormMainWindowState;
            if (Properties.Settings.Default.GlobalDirectoryRoot == "" || Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot) == false)
            {
                folderBrowserDialog1.Description = "Please select a root data folder.\r\nAll working data will be placed under this folder.";
                folderBrowserDialog1.ShowDialog();
                Properties.Settings.Default.GlobalDirectoryRoot = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.Save();
            }
            if (!Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData))
                Directory.CreateDirectory(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData);
            if (!Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs))
                Directory.CreateDirectory(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs);
            if (!Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets))
                Directory.CreateDirectory(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets);
            if (!Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryReports))
                Directory.CreateDirectory(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryReports);
            if (!Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryWork))
                Directory.CreateDirectory(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryWork);
            if (!Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryPlugins))
                Directory.CreateDirectory(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryPlugins);
            if (!Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules))
                Directory.CreateDirectory(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules);
            subdirectoryEntries = Directory.GetDirectories(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets);
            foreach (string subdirectory in subdirectoryEntries)
                comboBoxQuotes.Items.Add(Regex.Replace(subdirectory, regex, ""));
            if (comboBoxQuotes.Items.Count > 0)
                if (comboBoxQuotes.Items.Contains(Properties.Settings.Default.GlobalMarket))
                    comboBoxQuotes.Text = Properties.Settings.Default.GlobalMarket;
                else
                    comboBoxQuotes.SelectedIndex = 0;
            if (comboBoxIndexes.Items.Count > 0)
                comboBoxIndexes.SelectedIndex = 0;
            textBoxQuotesPasses.Text = _quoteImportIteration.ToString();
            _pluginService = new PluginServices();
            _pluginService.FindPlugins(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryPlugins);
            LoadSchedules();

            #region aquire
            #endregion
            #region model
            // model
            LoadBaseLineFile();
            bp = new Network(double.Parse(textBoxRate.Text), double.Parse(textBoxNoise.Text));
            trainingSet = new TrainingSet();
            this.numericUpDown1.Value = Properties.Settings.Default.randomMinIndicators;
            this.numericUpDown2.Value = Properties.Settings.Default.randomMaxIndicators;
            this.numericUpDown3.Value = Properties.Settings.Default.randomMinPeriod;
            this.numericUpDown4.Value = Properties.Settings.Default.randomMaxPeriod;
            this.numericUpDown5.Value = Properties.Settings.Default.FormStrategyEvolverPopulationConstraint;
            technicalChart.GraphPane.Title.Text = "";
            technicalChart.GraphPane.XAxis.Title.Text = "Time";
            technicalChart.GraphPane.YAxis.Title.Text = "Value";
            technicalChart.GraphPane.XAxis.Type = AxisType.DateAsOrdinal;
            technicalChart.GraphPane.Fill = new Fill(Color.Gray, Color.White, 90.0F);
            technicalChart.AxisChange();
            technicalChart.Invalidate();
            dateTimePickerModelRandomEarliest.Value = new DateTime(1900, 1, 1);
            dateTimePickerModelRandomLatest.Value = DateTime.Now;
            dateTimePicker3.Value = new DateTime(1900, 1, 1);
            dateTimePicker4.Value = DateTime.Now;
            dateTimePicker5.Value = new DateTime(1900, 1, 1);
            dateTimePicker6.Value = DateTime.Now;
            dateTimePicker7.Value = new DateTime(1900, 1, 1);
            dateTimePicker8.Value = DateTime.Now;
            comboBoxRandomChanceDisposition.SelectedIndex = 0;
            comboBoxNaturalSelectionHistoricDisposition.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBoxNaturalSelectionSampleDisposition.SelectedIndex = 3;
            comboBox1.SelectedIndex = 0;
            comboBoxCriteria.SelectedIndex = 0;
            LoadIndexes();
            InitializeAgents();
            #endregion
            #region forecast
            // forecast
            _currentRecommendationsBuyInstrumentsA = new System.Collections.Hashtable();
            _currentRecommendationsSellInstrumentsA = new System.Collections.Hashtable();
            _currentRecommendationsBuyInstrumentsB = new System.Collections.Hashtable();
            _currentRecommendationsSellInstrumentsB = new System.Collections.Hashtable();


            _datasetRecommendationsBuyA = new DataTable("recommendation");
            _datasetRecommendationsBuyA.Columns.Add("symbol", typeof(string));
            _datasetRecommendationsBuyA.Columns.Add("name", typeof(string));
            _datasetRecommendationsBuyA.Columns.Add("date", typeof(DateTime));
            _datasetRecommendationsBuyA.Columns.Add("return", typeof(double));
            dataGridRecommendationsBuy.DataSource = _datasetRecommendationsBuyA;
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyA))
                try
                {
                    _datasetRecommendationsBuyA.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyA);
                    _datasetRecommendationsBuyA = SupportClass.FilterSortDataTable(_datasetRecommendationsBuyA, "", "return", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            _datasetRecommendationsSellA = new DataTable("recommendation");
            _datasetRecommendationsSellA.Columns.Add("symbol", typeof(string));
            _datasetRecommendationsSellA.Columns.Add("name", typeof(string));
            _datasetRecommendationsSellA.Columns.Add("date", typeof(DateTime));
            dataGridRecommendationsSell.DataSource = _datasetRecommendationsSellA;
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellA))
                try
                {
                    _datasetRecommendationsSellA.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellA);
                    _datasetRecommendationsSellA = SupportClass.FilterSortDataTable(_datasetRecommendationsSellA, "", "date", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            _datasetRecommendationsBuyConsensusA = new DataTable("risk");
            _datasetRecommendationsBuyConsensusA.Columns.Add("instrument", typeof(string));
            _datasetRecommendationsBuyConsensusA.Columns.Add("name", typeof(string));
            _datasetRecommendationsBuyConsensusA.Columns.Add("date", typeof(DateTime));
            _datasetRecommendationsBuyConsensusA.Columns.Add("signal", typeof(int));
            _datasetRecommendationsBuyConsensusA.Columns.Add("consensus", typeof(double));

            _datasetRecommendationsSellConsensusA = new DataTable("risk");
            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = typeof(string);
            myDataColumn.ColumnName = "instrument";
            myDataColumn.Unique = true;
            _datasetRecommendationsSellConsensusA.Columns.Add(myDataColumn);
            _datasetRecommendationsSellConsensusA.Columns.Add("name", typeof(string));
            _datasetRecommendationsSellConsensusA.Columns.Add("date", typeof(DateTime));
            _datasetRecommendationsSellConsensusA.Columns.Add("signal", typeof(int));
            _datasetRecommendationsSellConsensusA.Columns.Add("consensus", typeof(double));

            _endDateForecastA = DateTime.Now;
            _startDateForecastA = _endDateForecastA.Subtract(new TimeSpan(Properties.Settings.Default.GlobalPeriod, 0, 0, 0, 0));



            _datasetRecommendationsBuyB = new DataTable("recommendation");
            _datasetRecommendationsBuyB.Columns.Add("symbol", typeof(string));
            _datasetRecommendationsBuyB.Columns.Add("name", typeof(string));
            _datasetRecommendationsBuyB.Columns.Add("date", typeof(DateTime));
            _datasetRecommendationsBuyB.Columns.Add("return", typeof(double));
            dataGrid24.DataSource = _datasetRecommendationsBuyB;
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyB))
                try
                {
                    _datasetRecommendationsBuyB.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyB);
                    _datasetRecommendationsBuyB = SupportClass.FilterSortDataTable(_datasetRecommendationsBuyB, "", "return", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            _datasetRecommendationsSellB = new DataTable("recommendation");
            _datasetRecommendationsSellB.Columns.Add("symbol", typeof(string));
            _datasetRecommendationsSellB.Columns.Add("name", typeof(string));
            _datasetRecommendationsSellB.Columns.Add("date", typeof(DateTime));
            dataGrid25.DataSource = _datasetRecommendationsSellB;
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellB))
                try
                {
                    _datasetRecommendationsSellB.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellB);
                    _datasetRecommendationsSellB = SupportClass.FilterSortDataTable(_datasetRecommendationsSellB, "", "date", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            _datasetRecommendationsBuyConsensusB = new DataTable("risk");
            _datasetRecommendationsBuyConsensusB.Columns.Add("instrument", typeof(string));
            _datasetRecommendationsBuyConsensusB.Columns.Add("name", typeof(string));
            _datasetRecommendationsBuyConsensusB.Columns.Add("date", typeof(DateTime));
            _datasetRecommendationsBuyConsensusB.Columns.Add("signal", typeof(int));
            _datasetRecommendationsBuyConsensusB.Columns.Add("consensus", typeof(double));

            _datasetRecommendationsSellConsensusB = new DataTable("risk");
            DataColumn myDataColumn2 = new DataColumn();
            myDataColumn2.DataType = typeof(string);
            myDataColumn2.ColumnName = "instrument";
            myDataColumn2.Unique = true;
            _datasetRecommendationsSellConsensusB.Columns.Add(myDataColumn2);
            _datasetRecommendationsSellConsensusB.Columns.Add("name", typeof(string));
            _datasetRecommendationsSellConsensusB.Columns.Add("date", typeof(DateTime));
            _datasetRecommendationsSellConsensusB.Columns.Add("signal", typeof(int));
            _datasetRecommendationsSellConsensusB.Columns.Add("consensus", typeof(double));

            _endDateForecastB = DateTime.Now;
            _startDateForecastB = _endDateForecastB.Subtract(new TimeSpan(Properties.Settings.Default.GlobalPeriod, 0, 0, 0, 0));


            dateTimePickerForecastEarliest.Value = _startDateForecastA;
            dateTimePickerForecastLatest.Value = _endDateForecastA;
            labelMarket.Text = Properties.Settings.Default.GlobalMarket;

            try
            {
                _portfolioA = new Portfolio(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _filePortfolioA, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs);
                _datasetPortfolioA = _portfolioA.GetDataSet();
                _datasetPortfolioA = SupportClass.FilterSortData(_datasetPortfolioA, 2, "", "gain", 1);
                _datasetPortfolioA = SupportClass.FilterSortData(_datasetPortfolioA, 3, "", "date", 1);
                dataGrid10.DataSource = _datasetPortfolioA.Tables["position"];
                dataGrid11.DataSource = _datasetPortfolioA.Tables["transaction"];
                textBox18.AppendText(DateTime.Now + " : Portfolio A loaded\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            _portfolioA.Update();
            try
            {
                _portfolioB = new Portfolio(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _filePortfolioB, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs);
                _datasetPortfolioB = _portfolioB.GetDataSet();
                _datasetPortfolioB = SupportClass.FilterSortData(_datasetPortfolioB, 2, "", "gain", 1);
                _datasetPortfolioB = SupportClass.FilterSortData(_datasetPortfolioB, 3, "", "date", 1);
                dataGrid26.DataSource = _datasetPortfolioB.Tables["position"];
                dataGrid27.DataSource = _datasetPortfolioB.Tables["transaction"];
                textBox18.AppendText(DateTime.Now + " : Portfolio B loaded\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            _portfolioB.Update();

            UpdatePortfolioDisplayA();
            UpdatePortfolioDisplayB();

            if (checkBoxUpdateLive.Checked == true)
            {
                backgroundWorkerUpdatePortfolio.RunWorkerAsync();
                timer1.Start();
                textBox18.AppendText(DateTime.Now + " : Live updating started\r\n");
            }
            #endregion
            #region analysis
            // analyze
            _marketDateExceptions2 = new ArrayList();
            _holidaySchedule = new ArrayList();
            string[] subdirectoryEntries2 = Directory.GetDirectories(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets);
            foreach (string subdirectory in subdirectoryEntries2)
                comboBoxMarket.Items.Add(Regex.Replace(subdirectory, @"^.*\\", ""));
            if (comboBoxMarket.Items.Contains(Properties.Settings.Default.GlobalMarket))
                comboBoxMarket.SelectedText = Properties.Settings.Default.GlobalMarket;

            _datasetRisk = new DataTable("risk");
            _datasetRisk.Columns.Add("instrument", typeof(string));
            _datasetRisk.Columns.Add("name", typeof(string));
            _datasetRisk.Columns.Add("sector", typeof(string));
            _datasetRisk.Columns.Add("industry", typeof(string));
            _datasetRisk.Columns.Add("peg_ratio", typeof(double));
            _datasetRisk.Columns.Add("price_earnings", typeof(string));
            _datasetRisk.Columns.Add("price_book", typeof(string));
            _datasetRisk.Columns.Add("price_sales", typeof(string));
            _datasetRisk.Columns.Add("beta", typeof(double));
            _datasetRisk.Columns.Add("start", typeof(DateTime));
            _datasetRisk.Columns.Add("end", typeof(DateTime));
            _datasetRisk.Columns.Add("up", typeof(int));
            _datasetRisk.Columns.Add("down", typeof(int));
            _datasetRisk.Columns.Add("total", typeof(int));
            _datasetRisk.Columns.Add("return", typeof(double));
            dataGridViewAnalyze.DataSource = _datasetRisk;

            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileStageRiskReward))
                try
                {
                    _datasetRisk.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileStageRiskReward);
                    _datasetRisk = SupportClass.FilterSortDataTable(_datasetRisk, "", "return", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            _startDateAnalyze = new DateTime(1900, 1, 1);
            _endDateAnalyze = DateTime.Now;
            _counterAnalysis = 0;
            _instrumentAnalysis = "";
            _instrumentNameAnalysis = "";
            _reportRiskReward = "";
            dateTimePickerAnalyzeEarliest.Value = _startDateAnalyze;
            dateTimePickerAnalyzeLatest.Value = _endDateAnalyze;
            for (int x = 0; x <= 366; x++)
            {
                _marketHistogramCounter[x] = 0;
                _marketHistogram[x] = 0;
            }
            for (int x = 0; x < 8; x++)
            {
                _marketLunarHistogramCounter[x] = 0;
                _marketLunarHistogram[x] = 0;
            }
            comboBoxMarket.Text = Properties.Settings.Default.GlobalMarket;
            #endregion
        }
        private void FormMain_MinimizeToTray(object sender, EventArgs e)
        {
            if (_isShown & this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.Visible = false;
                notifyIcon1.Visible = true;
                _isShown = false;
                timerMem.Stop();
                timerConnectionInfo.Stop();
            }
        }
        private void FormMain_Disposed(object sender, EventArgs e)
        {
            Properties.Settings.Default.FormMainWindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.FormMainSize = this.Size;
                Properties.Settings.Default.FormMainLocation = this.Location;
            }
            else
            {
                Properties.Settings.Default.FormMainSize = this.RestoreBounds.Size;
                Properties.Settings.Default.FormMainLocation = this.RestoreBounds.Location;
            }
            Properties.Settings.Default.FormStrategyEvolverRandomChanceChecked = checkBoxRandomChance.Checked;
            Properties.Settings.Default.FormStrategyEvolverNaturalSelectionChecked = checkBoxNaturalSelection.Checked;
            Properties.Settings.Default.FormStrategyEvolverGeneticEvolutionChecked = checkBoxGeneticEvolution.Checked;
            Properties.Settings.Default.FormStrategyEvolverNaturalSelectionOOSChecked = checkBox5.Checked;
            Properties.Settings.Default.Save();
            _portfolioA.Save();
            this.Dispose();
        }
        #endregion
        #region Form Events
        private void comboBoxNaturalSelectionSampleDisposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNaturalSelectionSampleDisposition.Text == "Combo")
                _flagSelectionSampleCombo = false;
            else
                _flagSelectionSampleCombo = true;
        }
        private void buttonRecommendationsBuy_Click(object sender, EventArgs e)
        {
            GetBuyRecommendationsA();

        }
        private void buttonRecommendationsSell_Click(object sender, EventArgs e)
        {
            GetSellRecommendationsA();
        }
        private void buttonUpdateReports_Click(object sender, EventArgs e)
        {
        }
        private void notifyIcon1_MouseDoubleClick(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            _isShown = true;
            timerMem.Start();
            timerConnectionInfo.Start();
        }
        private void buttonModelGeneticQueueSubmit_Click(object sender, EventArgs e)
        {
            if (textBoxModelGeneticQueueEntry.Text != "" && textBoxModelGeneticQueueExit.Text != "")
            {
                _agentModelGeneticQueueEntry.Add(textBoxModelGeneticQueueEntry.Text);
                _agentModelGeneticQueueExit.Add(textBoxModelGeneticQueueExit.Text);
                textBoxModelGeneticQueueEntry.Text = "";
                textBoxModelGeneticQueueExit.Text = "";
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBox5.Checked = false;
            if (comboBox2.Text != "")
            {
                switch (comboBox2.Text)
                {
                    case "10Y":
                        dateTimePicker5.Value = dateTimePicker6.Value.Subtract(new TimeSpan(10 * 365, 0, 0, 0));
                        break;
                    case "5Y":
                        dateTimePicker5.Value = dateTimePicker6.Value.Subtract(new TimeSpan(5 * 365, 0, 0, 0));
                        break;
                    case "3Y":
                        dateTimePicker5.Value = dateTimePicker6.Value.Subtract(new TimeSpan(3 * 365, 0, 0, 0));
                        break;
                    case "1Y":
                        dateTimePicker5.Value = dateTimePicker6.Value.Subtract(new TimeSpan(1 * 365, 0, 0, 0));
                        break;
                    case "6M":
                        dateTimePicker5.Value = dateTimePicker6.Value.Subtract(new TimeSpan(6 * 30, 0, 0, 0));
                        break;
                    case "3M":
                        dateTimePicker5.Value = dateTimePicker6.Value.Subtract(new TimeSpan(3 * 30, 0, 0, 0));
                        break;
                    case "1M":
                        dateTimePicker5.Value = dateTimePicker6.Value.Subtract(new TimeSpan(1 * 30, 0, 0, 0));
                        break;
                    default:
                        break;
                }
            }
            _agentModelSelectionSampleLength = comboBox2.Text;
        }
        private void buttonQuotesImport_Click(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(UpdateMarketData));
        }
        private void buttonQuotesStop_Click(object sender, EventArgs e)
        {
            buttonAcquireYahooStop.Enabled = !buttonAcquireYahooStop.Enabled;
            buttonAcquireYahooImport.Enabled = !buttonAcquireYahooImport.Enabled;
            backgroundWorkerQuotes.CancelAsync();
        }
        private void backgroundWorkerQuotes_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            _dataSource.GetHistoricMarketDataViaYAHOO(worker, e, _index);
        }
        private void backgroundWorkerQuotes_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBarQuotes.Value = e.ProgressPercentage;
        }
        private void backgroundWorkerQuotes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                buttonAcquireYahooStop.Enabled = !buttonAcquireYahooStop.Enabled;
                buttonAcquireYahooImport.Enabled = !buttonAcquireYahooImport.Enabled;
                textBoxDataAcquisitionConsole.AppendText(DateTime.Now + " : Operation produced error: " + e.Error.Message + "\r\n");
            }
            else if (e.Cancelled)
            {
                buttonAcquireYahooStop.Enabled = !buttonAcquireYahooStop.Enabled;
                buttonAcquireYahooImport.Enabled = !buttonAcquireYahooImport.Enabled;
                textBoxDataAcquisitionConsole.AppendText(DateTime.Now + " : Operation cancelled\r\n");
            }
            else
            {
                _quoteImportIteration++;
                textBoxQuotesPasses.Text = _quoteImportIteration.ToString();
                if (buttonAcquireYahooStop.Enabled == true && _quoteImportIteration < int.Parse(textBoxQuotesMaximum.Text))
                    backgroundWorkerQuotes.RunWorkerAsync();
                else
                {
                    buttonAcquireYahooStop.Enabled = !buttonAcquireYahooStop.Enabled;
                    buttonAcquireYahooImport.Enabled = !buttonAcquireYahooImport.Enabled;
                    textBoxDataAcquisitionConsole.AppendText(DateTime.Now + " : Operation completed sucessfully\r\n");
                }
            }
            _agentQuotesStop = DateTime.Now;
            double milli = _agentQuotesStop.Subtract(_agentQuotesStart).TotalMilliseconds;
            label1.Text = "Last Updated: " + _agentQuotesStop.ToString() + " : " + milli.ToString() + " ms = " + SupportClass.ConvertToMinutes(milli).ToString() + " min";
        }
        private void buttonIndexesImport_Click(object sender, EventArgs e)
        {
            //_index = comboBoxIndexes.Text;
            //backgroundWorkerIndexes.RunWorkerAsync();
        }
        private void backgroundWorkerIndexes_DoWork(object sender, DoWorkEventArgs e)
        {
            _index = Properties.Settings.Default.GlobalMarket;
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + _index + "\\"))
                File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + _index + "\\");
            Invoke(new MethodInvoker(_dataSource.GetIndexSymbolsViaNASDAQ));
            //GetMarketSymbolsViaNASDAQTrader(_index);
        }
        private void tabPageQuotes_Click(object sender, EventArgs e)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets);
            string regex = @"^.*\\";
            comboBoxQuotes.Items.Clear();
            foreach (string subdirectory in subdirectoryEntries)
                comboBoxQuotes.Items.Add(Regex.Replace(subdirectory, regex, ""));
            if (comboBoxQuotes.Items.Count > 0)
                comboBoxQuotes.SelectedIndex = 0;
            textBoxQuotesPasses.Text = _quoteImportIteration.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            UpdateIndex();
        }
        private void buttonQueue_Click(object sender, EventArgs e)
        {
            if (textBoxQueue.Text != "")
            {
                _agentNaturalSelectionQueue.Add(textBoxQueue.Text);
                textBoxQueue.Text = "";
            }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Minimum = numericUpDown1.Value;
            Properties.Settings.Default.randomMinIndicators = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = numericUpDown2.Value;
            Properties.Settings.Default.randomMaxIndicators = (int)numericUpDown2.Value;
            Properties.Settings.Default.Save();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown4.Minimum = numericUpDown3.Value;
            Properties.Settings.Default.randomMinPeriod = (int)numericUpDown3.Value;
            Properties.Settings.Default.Save();
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown3.Maximum = numericUpDown4.Value;
            Properties.Settings.Default.randomMaxPeriod = (int)numericUpDown4.Value;
            Properties.Settings.Default.Save();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FormStrategyEvolverPopulationConstraint = (int)numericUpDown5.Value;
            Properties.Settings.Default.Save();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            backgroundWorkerUpdatePortfolio.RunWorkerAsync();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void checkBoxRandomChance_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRandomChance.Checked == true)
            {
                if (checkBoxModelChanceRandom.Checked == false)
                {
                    if (_datasetRisk != null)
                    {
                        if (_datasetRisk.Rows[0].ItemArray[0] != null)
                        {
                            textBoxModelChanceSymbol.Text = _datasetRisk.Rows[0].ItemArray[0].ToString();
                            _agentRandomChanceEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePickerModelRandomEarliest.Value, dateTimePickerModelRandomLatest.Value);
                            _agentRandomChanceMarket = new Market();
                            _agentRandomChanceMarket.Set(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, _datasetRisk.Rows[0].ItemArray[0].ToString(), _datasetRisk.Rows[0].ItemArray[1].ToString());
                            _agentRandomChanceMarket.random = false;
                            _agentRandomChanceObserver = new Observer();
                            _agentRandomChanceEngine.Observer = _agentRandomChanceObserver;
                        }
                        else
                        {
                            checkBoxModelChanceRandom.Checked = true;
                            textBoxModelChanceSymbol.Text = "Random";
                            _agentRandomChanceEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePickerModelRandomEarliest.Value, dateTimePickerModelRandomLatest.Value);
                            _agentRandomChanceMarket = new Market();
                            _agentRandomChanceMarket.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                            _agentRandomChanceMarket.random = true;
                            _agentRandomChanceObserver = new Observer();
                            _agentRandomChanceEngine.Observer = _agentRandomChanceObserver;
                        }
                    }
                    else
                    {
                        checkBoxModelChanceRandom.Checked = true;
                        textBoxModelChanceSymbol.Text = "Random";
                        _agentRandomChanceEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePickerModelRandomEarliest.Value, dateTimePickerModelRandomLatest.Value);
                        _agentRandomChanceMarket = new Market();
                        _agentRandomChanceMarket.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                        _agentRandomChanceMarket.random = true;
                        _agentRandomChanceObserver = new Observer();
                        _agentRandomChanceEngine.Observer = _agentRandomChanceObserver;
                    }
                }
                else
                {
                    textBoxModelChanceSymbol.Text = "Random";
                    _agentRandomChanceEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePickerModelRandomEarliest.Value, dateTimePickerModelRandomLatest.Value);
                    _agentRandomChanceMarket = new Market();
                    _agentRandomChanceMarket.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    _agentRandomChanceMarket.random = true;
                    _agentRandomChanceObserver = new Observer();
                    _agentRandomChanceEngine.Observer = _agentRandomChanceObserver;
                }

                try
                {
                    textBoxRandomChance.AppendText(DateTime.Now + " : Engine instantiated\r\n");
                    switch (comboBoxRandomChanceDisposition.Text)
                    {
                        case "Long":
                            _agentRandomChanceDisposition = true;
                            break;
                        case "Short":
                            _agentRandomChanceDisposition = false;
                            break;
                        default:
                            break;
                    }
                    bool duplicate = true;
                    while (duplicate == true)
                    {
                        RandomStrategy strategy = new RandomStrategy(ref _pluginService, (int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value, _agentRandomChanceDisposition);
                        if (_agentRandomChanceDisposition == true)
                        {
                            if (!_datasetModelSelectionMaxEntry.Rows.Contains(strategy.EntryIndicatorName) && !_datasetModelSelectionMaxEntryQueue.Rows.Contains(strategy.EntryIndicatorName))
                            {
                                listBoxRandomChance.ForeColor = Color.DarkSeaGreen;
                                listBoxRandomChance.Items.Clear();
                                listBoxRandomChance.Items.Add(strategy.EntryIndicatorName);
                                duplicate = false;
                            }
                        }
                        else
                        {
                            if (!_datasetModelSelectionMaxExit.Rows.Contains(strategy.ExitIndicatorName) && !_datasetModelSelectionMaxExitQueue.Rows.Contains(strategy.ExitIndicatorName))
                            {
                                listBoxRandomChance.ForeColor = Color.IndianRed;
                                listBoxRandomChance.Items.Clear();
                                listBoxRandomChance.Items.Add(strategy.ExitIndicatorName);
                                duplicate = false;
                            }
                        }
                        _agentRandomChanceEngine.removeTraders();
                        _agentRandomChanceEngine.register(strategy);
                    }
                    backgroundWorkerRandomChance.RunWorkerAsync();
                    textBoxRandomChance.AppendText(DateTime.Now + " : Simulation started\r\n");
                    checkBoxRandomChance.Text = "Stop";
                    comboBoxRandomChanceDisposition.Enabled = false;
                }
                catch (Exception ex)
                {
                    checkBoxRandomChance.Checked = false;
                    progressBarRandomChance.Value = 0;
                    listBoxRandomChance.Items.Clear();
                    checkBoxRandomChance.Text = "Start";
                    comboBoxRandomChanceDisposition.Enabled = true;
                    textBoxRandomChance.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                }
            }
            else
            {
                backgroundWorkerRandomChance.CancelAsync();
            }
        }
        private void backgroundWorkerRandomChance_DoWork(object sender, DoWorkEventArgs e)
        {
            _agentRandomChanceStart = DateTime.Now;
            if (_agentRandomChanceDisposition == true)
            {
                _agentRandomChanceLog = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs + Properties.Settings.Default.GlobalMarket + "-RANDOM_CHANCE-ENTRY-" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                _strategyTester.BackTest(backgroundWorkerRandomChance, e, _agentRandomChanceLog, _agentRandomChanceEngine, _agentRandomChanceMarket, ref _agentRandomChanceScore, ref _datasetModelSelectionMaxEntryQueue, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryQueue, ref _agentRandomChanceWin, true);
            }
            else
            {
                _agentRandomChanceLog = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs + Properties.Settings.Default.GlobalMarket + "-RANDOM_CHANCE-EXIT-" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                _strategyTester.BackTest(backgroundWorkerRandomChance, e, _agentRandomChanceLog, _agentRandomChanceEngine, _agentRandomChanceMarket, ref _agentRandomChanceScore, ref _datasetModelSelectionMaxExitQueue, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitQueue, ref _agentRandomChanceWin, false);
            }
        }
        private void backgroundWorkerRandomChance_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBarRandomChance.Value = e.ProgressPercentage;
        }
        private void backgroundWorkerRandomChance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (e.Error != null)
            {
                checkBoxRandomChance.Checked = false;
                progressBarRandomChance.Value = 0;
                listBoxRandomChance.Items.Clear();
                checkBoxRandomChance.Text = "Start";
                comboBoxRandomChanceDisposition.Enabled = true;
                textBoxRandomChance.AppendText(DateTime.Now + " : Operation produced error: " + e.Error.Message + "\r\n");
            }
            else if (e.Cancelled)
            {
                checkBoxRandomChance.Checked = false;
                progressBarRandomChance.Value = 0;
                listBoxRandomChance.Items.Clear();
                checkBoxRandomChance.Text = "Start";
                comboBoxRandomChanceDisposition.Enabled = true;
                textBoxRandomChance.Clear();
                textBoxRandomChance.AppendText(DateTime.Now + " : Operation cancelled\r\n");
            }
            else
            {
                textBoxRandomChance.Clear();
                textBoxRandomChance.AppendText(_agentRandomChanceScore);
                textBoxRandomChance.AppendText(_agentRandomChanceObserver.GetReport());
                if (_agentRandomChanceDisposition == true)
                {
                    if (_datasetModelSelectionMaxEntryQueue.Rows.Count > 0)
                    {
                        _datasetModelSelectionMaxEntryQueue = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntryQueue, "", "fitness", 1);
                        //dataGridCacheEntry.DataSource = _datasetCacheEntry;
                        textBox7.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                        textBoxSelectionHistoricEntryQueueCount.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                        if (_datasetModelSelectionMaxEntryQueue.Rows.Count > (int)numericUpDown5.Value)
                        {
                            while (_datasetModelSelectionMaxEntryQueue.Rows.Count > (int)numericUpDown5.Value)
                            {
                                _datasetModelSelectionMaxEntryQueue.Rows.RemoveAt(_datasetModelSelectionMaxEntryQueue.Rows.Count - 1);
                                //dataGridCacheEntry.DataSource = _datasetCacheEntry;
                                textBox7.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                                textBoxSelectionHistoricEntryQueueCount.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                            }
                        }
                    }
                }
                else
                {
                    if (_datasetModelSelectionMaxExitQueue.Rows.Count > 0)
                    {
                        _datasetModelSelectionMaxExitQueue = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExitQueue, "", "fitness", 1);
                        //dataGridCacheExit.DataSource = _datasetCacheExit;
                        textBox8.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                        textBoxSelectionHistoricExitQueueCount.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                        if (_datasetModelSelectionMaxExitQueue.Rows.Count > (int)numericUpDown5.Value)
                        {
                            while (_datasetModelSelectionMaxExitQueue.Rows.Count > (int)numericUpDown5.Value)
                            {
                                _datasetModelSelectionMaxExitQueue.Rows.RemoveAt(_datasetModelSelectionMaxExitQueue.Rows.Count - 1);
                                //dataGridCacheExit.DataSource = _datasetCacheExit;
                                textBox8.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                                textBoxSelectionHistoricExitQueueCount.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                            }
                        }
                    }
                }

                LineItem line;
                double[] dateArray = (double[])_strategyTester.date.ToArray(typeof(double));
                double[] valueArray = (double[])_strategyTester.value.ToArray(typeof(double));
                technicalChart.GraphPane.CurveList.Clear();
                line = technicalChart.GraphPane.AddCurve(listBoxRandomChance.Items[0].ToString(), dateArray, valueArray, Color.Black, SymbolType.None);
                line.Line.Fill = new Fill(Color.White, Color.Black, 45F);
                /*
                Instrument instrument = new Instrument(_instrument, "", Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalSubDirectoryMarkets + "NASDAQ//" + _instrument + ".csv");
                StockPointList spl = new StockPointList();
                technicalChart.GraphPane.CurveList.Clear();
                for (int x = 0; x < instrument.TechnicalData.Count; x++)
                {
                    XDate xDate = new XDate(((TechnicalData)instrument.TechnicalData[x]).Date);
                    double date = xDate.XLDate;
                    double open = ((TechnicalData)instrument.TechnicalData[x]).AdjOpen;
                    double close = ((TechnicalData)instrument.TechnicalData[x]).AdjClose;
                    double high = ((TechnicalData)instrument.TechnicalData[x]).AdjHigh;
                    double low = ((TechnicalData)instrument.TechnicalData[x]).AdjLow;
                    double volume = ((TechnicalData)instrument.TechnicalData[x]).AdjVolume;
                    StockPt pt = new StockPt(date, high, low, open, close, volume);
                    spl.Add(pt);
                }
                JapaneseCandleStickItem myCurve = technicalChart.GraphPane.AddJapaneseCandleStick(_instrument, spl);
                myCurve.Stick.IsAutoSize = true;
                 */
                technicalChart.AxisChange();
                technicalChart.Invalidate();

                _agentRandomChanceStop = DateTime.Now;
                labelModelRandomChanceLast.Text = _agentRandomChanceStop.ToString() + " : " + _agentRandomChanceStop.Subtract(_agentRandomChanceStart).TotalMilliseconds.ToString() + " ms";
                if (checkBox1.Checked == true)
                    if (_agentRandomChanceWin == true)
                        _soundPlayer.Play();
                switch (comboBoxRandomChanceDisposition.Text)
                {
                    case "Long":
                        _agentRandomChanceDisposition = true;
                        break;
                    case "Short":
                        _agentRandomChanceDisposition = false;
                        break;
                    case "Rotate":
                        _agentRandomChanceDisposition = !_agentRandomChanceDisposition;
                        break;
                    default:
                        break;
                }
                bool duplicate = true;
                while (duplicate == true)
                {
                    RandomStrategy strategy = new RandomStrategy(ref _pluginService, (int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value, _agentRandomChanceDisposition);
                    if (_agentRandomChanceDisposition == true)
                    {
                        if (!_datasetModelSelectionMaxEntry.Rows.Contains(strategy.EntryIndicatorName) && !_datasetModelSelectionMaxEntryQueue.Rows.Contains(strategy.EntryIndicatorName))
                        {
                            listBoxRandomChance.ForeColor = Color.DarkSeaGreen;
                            listBoxRandomChance.Items.Clear();
                            listBoxRandomChance.Items.Add(strategy.EntryIndicatorName);
                            duplicate = false;
                        }
                    }
                    else
                    {
                        if (!_datasetModelSelectionMaxExit.Rows.Contains(strategy.ExitIndicatorName) && !_datasetModelSelectionMaxExitQueue.Rows.Contains(strategy.ExitIndicatorName))
                        {
                            listBoxRandomChance.ForeColor = Color.IndianRed;
                            listBoxRandomChance.Items.Clear();
                            listBoxRandomChance.Items.Add(strategy.ExitIndicatorName);
                            duplicate = false;
                        }
                    }
                    _agentRandomChanceEngine.removeTraders();
                    _agentRandomChanceEngine.register(strategy);
                }
                backgroundWorkerRandomChance.RunWorkerAsync();
            }
        }


        private void checkBoxNaturalSelectionIS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNaturalSelection.Checked == true)
            {
                try
                {
                    _agentNaturalSelectionISEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker3.Value, dateTimePicker4.Value);
                    _agentNaturalSelectionISMarket = new Market();
                    _agentNaturalSelectionISMarket.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    _agentNaturalSelectionISObserver = new Observer();
                    _agentNaturalSelectionISEngine.Observer = _agentNaturalSelectionISObserver;
                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Engine instantiated\r\n");
                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Initial balance: " + Properties.Settings.Default.AccountInitialBalance.ToString() + "\r\n");
                    switch (comboBoxNaturalSelectionHistoricDisposition.Text)
                    {
                        case "Long":
                            _agentNaturalSelectionISDisposition = true;
                            break;
                        case "Short":
                            _agentNaturalSelectionISDisposition = false;
                            break;
                        case "Combo":
                            _agentNaturalSelectionISCombo = true;
                            break;
                        default:
                            break;
                    }
                    if (_agentNaturalSelectionQueue.Count < 1)
                    {
                        if (comboBoxNaturalSelectionHistoricDisposition.Text == "Combo")
                        {
                            if (_datasetModelGeneticMaxQueue.Rows.Count > 0)
                            {
                                if (_agentModelHistoricBaseline == 0)
                                {
                                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker3.Value, dateTimePicker4.Value);
                                    Market market = new Market();
                                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                                    Observer observer = new Observer();
                                    engine.Observer = observer;
                                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Generating baseline\r\n");
                                    _agentModelHistoricBaseline = GetBaseLine(engine, market, 36514);
                                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Done (" + _agentModelHistoricBaseline.ToString() + ")\r\n");
                                }
                                _strategyChromosone = _datasetModelGeneticMaxQueue.Rows[0].ItemArray[1].ToString();
                                _agentNaturalSelectionISEngine.removeTraders();
                                GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, _strategyChromosone);
                                _agentNaturalSelectionISEngine.register(strategy);
                                listBoxNaturalSelection.Items.Clear();
                                listBoxNaturalSelection.ForeColor = Color.Blue;
                                listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName);
                                backgroundWorkerNaturalSelection.RunWorkerAsync();
                                textBoxNaturalSelection.AppendText(DateTime.Now + " : Simulation started\r\n");
                                checkBoxNaturalSelection.Text = "Stop";
                                comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                            }
                        }
                        else
                        {
                            _agentNaturalSelectionISCombo = false;
                            if (_agentNaturalSelectionISDisposition == true)
                            {
                                if (_datasetModelSelectionMaxEntryQueue.Rows.Count > 0)
                                {
                                    _strategyChromosone = _datasetModelSelectionMaxEntryQueue.Rows[0].ItemArray[1].ToString();
                                    _agentNaturalSelectionISEngine.removeTraders();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                    _agentNaturalSelectionISEngine.register(strategy);
                                    listBoxNaturalSelection.Items.Clear();
                                    listBoxNaturalSelection.ForeColor = Color.LimeGreen;
                                    listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName);
                                    backgroundWorkerNaturalSelection.RunWorkerAsync();
                                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Simulation started\r\n");
                                    checkBoxNaturalSelection.Text = "Stop";
                                    comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                }
                                else
                                {
                                    if (_datasetModelSelectionMaxExitQueue.Rows.Count > 0)
                                    {
                                        _agentNaturalSelectionISDisposition = false;
                                        _strategyChromosone = _datasetModelSelectionMaxExitQueue.Rows[0].ItemArray[1].ToString();
                                        _agentNaturalSelectionISEngine.removeTraders();
                                        TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                        _agentNaturalSelectionISEngine.register(strategy);
                                        listBoxNaturalSelection.Items.Clear();
                                        listBoxNaturalSelection.ForeColor = Color.Red;
                                        listBoxNaturalSelection.Items.Add(strategy.ExitIndicatorName);
                                        backgroundWorkerNaturalSelection.RunWorkerAsync();
                                        textBoxNaturalSelection.AppendText(DateTime.Now + " : Simulation started\r\n");
                                        checkBoxNaturalSelection.Text = "Stop";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                    }
                                    else
                                    {
                                        checkBoxNaturalSelection.Checked = false;
                                        progressBar1.Value = 0;
                                        listBoxNaturalSelection.Items.Clear();
                                        checkBoxNaturalSelection.Text = "Start";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                                        textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                        timer2.Enabled = true;
                                    }
                                }
                            }
                            else
                            {
                                if (_datasetModelSelectionMaxExitQueue.Rows.Count > 0)
                                {
                                    _strategyChromosone = _datasetModelSelectionMaxExitQueue.Rows[0].ItemArray[1].ToString();
                                    _agentNaturalSelectionISEngine.removeTraders();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                    _agentNaturalSelectionISEngine.register(strategy);
                                    listBoxNaturalSelection.Items.Clear();
                                    listBoxNaturalSelection.ForeColor = Color.Red;
                                    listBoxNaturalSelection.Items.Add(strategy.ExitIndicatorName);
                                    backgroundWorkerNaturalSelection.RunWorkerAsync();
                                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Simulation started\r\n");
                                    checkBoxNaturalSelection.Text = "Stop";
                                    comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                }
                                else
                                {
                                    if (_datasetModelSelectionMaxEntryQueue.Rows.Count > 0)
                                    {
                                        _agentNaturalSelectionISDisposition = true;
                                        _strategyChromosone = _datasetModelSelectionMaxEntryQueue.Rows[0].ItemArray[1].ToString();
                                        _agentNaturalSelectionISEngine.removeTraders();
                                        TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                        _agentNaturalSelectionISEngine.register(strategy);
                                        listBoxNaturalSelection.Items.Clear();
                                        listBoxNaturalSelection.ForeColor = Color.LimeGreen;
                                        listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName);
                                        backgroundWorkerNaturalSelection.RunWorkerAsync();
                                        textBoxNaturalSelection.AppendText(DateTime.Now + " : Simulation started\r\n");
                                        checkBoxNaturalSelection.Text = "Stop";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                    }
                                    else
                                    {
                                        checkBoxNaturalSelection.Checked = false;
                                        progressBar1.Value = 0;
                                        listBoxNaturalSelection.Items.Clear();
                                        checkBoxNaturalSelection.Text = "Start";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                                        textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                        timer2.Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _agentNaturalSelectionISEngine.removeTraders();
                        TestStrategy strategy = new TestStrategy(ref _pluginService, (string)_agentNaturalSelectionQueue[0], _agentNaturalSelectionISDisposition);
                        _agentNaturalSelectionQueue.RemoveAt(0);
                        _agentNaturalSelectionISEngine.register(strategy);
                        listBoxNaturalSelection.Items.Clear();
                        if (_agentNaturalSelectionISDisposition == true)
                        {
                            listBoxNaturalSelection.ForeColor = Color.LimeGreen;
                            listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName);
                        }
                        else
                        {
                            listBoxNaturalSelection.ForeColor = Color.Red;
                            listBoxNaturalSelection.Items.Add(strategy.ExitIndicatorName);
                        }
                        backgroundWorkerNaturalSelection.RunWorkerAsync();
                        textBoxNaturalSelection.AppendText(DateTime.Now + " : Simulation started\r\n");
                        checkBoxNaturalSelection.Text = "Stop";
                        comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    checkBoxNaturalSelection.Checked = false;
                    progressBar1.Value = 0;
                    listBoxNaturalSelection.Items.Clear();
                    checkBoxNaturalSelection.Text = "Start";
                    comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                }
            }
            else
            {
                backgroundWorkerNaturalSelection.CancelAsync();
            }
        }
        private void backgroundWorkerNaturalSelectionIS_DoWork(object sender, DoWorkEventArgs e)
        {
            _agentNaturalSelectionISStart = DateTime.Now;
            _agentNaturalSelectionISMarket.random = false;

            if (_agentNaturalSelectionISCombo == true)
            {
                _agentNaturalSelectionISLog = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs + Properties.Settings.Default.GlobalMarket + "-NATURAL_SELECTION-IS-COMBO-" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                _strategyTester.BackTest(backgroundWorkerNaturalSelection, e, _agentNaturalSelectionISLog, _agentNaturalSelectionISEngine, _agentNaturalSelectionISMarket, ref _agentNaturalSelectionISScore, ref _datasetModelGeneticMaxCache, ref _datasetModelGeneticMax, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMax, ref _agentNaturalSelectionISWin, true, _agentModelHistoricBaseline);
            }
            else
            {
                if (_agentNaturalSelectionISDisposition == true)
                {
                    _agentNaturalSelectionISLog = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs + Properties.Settings.Default.GlobalMarket + "-NATURAL_SELECTION-IS-ENTRY-" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    _strategyTester.BackTest(backgroundWorkerNaturalSelection, e, _agentNaturalSelectionISLog, _agentNaturalSelectionISEngine, _agentNaturalSelectionISMarket, ref _agentNaturalSelectionISScore, ref _datasetModelSelectionMaxEntryCache, ref _datasetModelSelectionMaxEntry, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntry, ref _agentNaturalSelectionISWin, true);
                }
                else
                {
                    _agentNaturalSelectionISLog = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs + Properties.Settings.Default.GlobalMarket + "-NATURAL_SELECTION-IS-EXIT-" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    _strategyTester.BackTest(backgroundWorkerNaturalSelection, e, _agentNaturalSelectionISLog, _agentNaturalSelectionISEngine, _agentNaturalSelectionISMarket, ref _agentNaturalSelectionISScore, ref _datasetModelSelectionMaxExitCache, ref _datasetModelSelectionMaxExit, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExit, ref _agentNaturalSelectionISWin, false);
                }
            }
        }
        private void backgroundWorkerNaturalSelectionIS_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }
        private void backgroundWorkerNaturalSelectionIS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (e.Error != null)
            {
                checkBoxNaturalSelection.Checked = false;
                progressBar1.Value = 0;
                listBoxNaturalSelection.Items.Clear();
                checkBoxNaturalSelection.Text = "Start";
                comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation produced error: " + e.Error.Message + "\r\n");
            }
            else if (e.Cancelled)
            {
                checkBoxNaturalSelection.Checked = false;
                progressBar1.Value = 0;
                listBoxNaturalSelection.Items.Clear();
                checkBoxNaturalSelection.Text = "Start";
                comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation cancelled\r\n");
            }
            else
            {
                textBoxNaturalSelection.AppendText(_agentNaturalSelectionISScore);
                _agentNaturalSelectionISStop = DateTime.Now;
                double milli = _agentNaturalSelectionISStop.Subtract(_agentNaturalSelectionISStart).TotalMilliseconds;
                label3.Text = _agentNaturalSelectionISStop.ToString() + " : " + milli.ToString() + " ms = " + SupportClass.ConvertToMinutes(milli).ToString() + " min";

                if (_agentNaturalSelectionISCombo == true)
                {
                    for (int x = 0; x < _datasetModelGeneticMaxQueue.Rows.Count; x++)
                    {
                        if (_datasetModelGeneticMaxQueue.Rows[x].ItemArray[1].ToString() == _strategyChromosone)
                        {
                            _datasetModelGeneticMaxQueue.Rows[x].Delete();
                        }
                    }
                    _datasetModelGeneticMaxQueue = SupportClass.FilterSortDataTable(_datasetModelGeneticMaxQueue, "", "fitness", 1);
                    _datasetModelGeneticMaxCache = SupportClass.FilterSortDataTable(_datasetModelGeneticMaxCache, "", "fitness", 1);
                    _datasetModelGeneticMax = SupportClass.FilterSortDataTable(_datasetModelGeneticMax, "", "fitness", 1);
                    dataGridModelGeneticMax.DataSource = _datasetModelGeneticMax;
                    FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxQueue, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    _datasetModelGeneticMaxQueue.WriteXml(fout);
                    fout.Close();
                    textBoxSelectionHistoricComboCount.Text = _datasetModelGeneticMax.Rows.Count.ToString();
                    textBoxSelectionHistoricComboQueueCount.Text = _datasetModelGeneticMaxQueue.Rows.Count.ToString();
                }
                else
                {
                    if (_agentNaturalSelectionISDisposition == true)
                    {
                        for (int x = 0; x < _datasetModelSelectionMaxEntryQueue.Rows.Count; x++)
                        {
                            if (_datasetModelSelectionMaxEntryQueue.Rows[x].ItemArray[1].ToString() == _strategyChromosone)
                            {
                                _datasetModelSelectionMaxEntryQueue.Rows[x].Delete();
                            }
                        }
                        _datasetModelSelectionMaxEntryQueue = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntryQueue, "", "fitness", 1);
                        _datasetModelSelectionMaxEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntryCache, "", "fitness", 1);
                        _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", "fitness", 1);
                        //dataGridCacheEntry.DataSource = _datasetCacheEntry;
                        //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                        dataGridModelSelectionMaxEntry.DataSource = _datasetModelSelectionMaxEntry;
                        FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryQueue, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        _datasetModelSelectionMaxEntryQueue.WriteXml(fout);
                        fout.Close();
                        textBox7.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                        textBoxSelectionHistoricEntryCount.Text = _datasetModelSelectionMaxEntry.Rows.Count.ToString();
                        textBoxSelectionHistoricEntryQueueCount.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                    }
                    else
                    {
                        for (int x = 0; x < _datasetModelSelectionMaxExitQueue.Rows.Count; x++)
                        {
                            if (_datasetModelSelectionMaxExitQueue.Rows[x].ItemArray[1].ToString() == _strategyChromosone)
                            {
                                _datasetModelSelectionMaxExitQueue.Rows[x].Delete();
                            }
                        }
                        _datasetModelSelectionMaxExitQueue = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExitQueue, "", "fitness", 1);
                        _datasetModelSelectionMaxExitCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExitCache, "", "fitness", 1);
                        _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", "fitness", 1);
                        //dataGridCacheExit.DataSource = _datasetCacheExit;
                        //dataGridHistoricCacheExit.DataSource = _datasetHistoricCacheExit;
                        dataGridModelSelectionMaxExit.DataSource = _datasetModelSelectionMaxExit;
                        FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitQueue, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        _datasetModelSelectionMaxExitQueue.WriteXml(fout);
                        fout.Close();
                        textBox8.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                        textBoxSelectionHistoricExitCount.Text = _datasetModelSelectionMaxExit.Rows.Count.ToString();
                        textBoxSelectionHistoricExitQueueCount.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                    }
                }
                _agentNaturalSelectionISEngine.removeTraders();
                switch (comboBoxNaturalSelectionHistoricDisposition.Text)
                {
                    case "Long":
                        _agentNaturalSelectionISDisposition = true;
                        break;
                    case "Short":
                        _agentNaturalSelectionISDisposition = false;
                        break;
                    case "Rotate":
                        _agentNaturalSelectionISDisposition = !_agentNaturalSelectionISDisposition;
                        break;
                    case "Combo":
                        _agentNaturalSelectionISCombo = true;
                        break;
                    default:
                        break;
                }
                try
                {
                    if (_agentNaturalSelectionQueue.Count < 1)
                    {
                        if (comboBoxNaturalSelectionHistoricDisposition.Text == "Combo")
                        {
                            if (_datasetModelGeneticMaxQueue.Rows.Count > 0)
                            {
                                _strategyChromosone = _datasetModelGeneticMaxQueue.Rows[0].ItemArray[1].ToString();
                                _agentNaturalSelectionISEngine.removeTraders();
                                GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, _strategyChromosone);
                                _agentNaturalSelectionISEngine.register(strategy);
                                listBoxNaturalSelection.Items.Clear();
                                listBoxNaturalSelection.ForeColor = Color.Blue;
                                listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName);
                                backgroundWorkerNaturalSelection.RunWorkerAsync();
                                checkBoxNaturalSelection.Text = "Stop";
                                comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                            }
                            else
                            {
                                checkBoxNaturalSelection.Checked = false;
                                progressBar1.Value = 0;
                                listBoxNaturalSelection.Items.Clear();
                                checkBoxNaturalSelection.Text = "Start";
                                comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                                textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                timer2.Enabled = true;
                            }
                        }
                        else
                        {
                            _agentNaturalSelectionISCombo = false;
                            if (_agentNaturalSelectionISDisposition == true)
                            {
                                if (_datasetModelSelectionMaxEntryQueue.Rows.Count > 0)
                                {
                                    _strategyChromosone = _datasetModelSelectionMaxEntryQueue.Rows[0].ItemArray[1].ToString();
                                    _agentNaturalSelectionISEngine.removeTraders();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                    _agentNaturalSelectionISEngine.register(strategy);
                                    listBoxNaturalSelection.Items.Clear();
                                    listBoxNaturalSelection.ForeColor = Color.LimeGreen;
                                    listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName);
                                    backgroundWorkerNaturalSelection.RunWorkerAsync();
                                    checkBoxNaturalSelection.Text = "Stop";
                                    comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                }
                                else
                                {
                                    if (_datasetModelSelectionMaxExitQueue.Rows.Count > 0)
                                    {
                                        _agentNaturalSelectionISDisposition = false;
                                        _strategyChromosone = _datasetModelSelectionMaxExitQueue.Rows[0].ItemArray[1].ToString();
                                        _agentNaturalSelectionISEngine.removeTraders();
                                        TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                        _agentNaturalSelectionISEngine.register(strategy);
                                        listBoxNaturalSelection.Items.Clear();
                                        listBoxNaturalSelection.ForeColor = Color.Red;
                                        listBoxNaturalSelection.Items.Add(strategy.ExitIndicatorName);
                                        backgroundWorkerNaturalSelection.RunWorkerAsync();
                                        checkBoxNaturalSelection.Text = "Stop";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                    }
                                    else
                                    {
                                        checkBoxNaturalSelection.Checked = false;
                                        progressBar1.Value = 0;
                                        listBoxNaturalSelection.Items.Clear();
                                        checkBoxNaturalSelection.Text = "Start";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                                        textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                        timer2.Enabled = true;
                                    }
                                }
                            }
                            else
                            {
                                if (_datasetModelSelectionMaxExitQueue.Rows.Count > 0)
                                {
                                    _strategyChromosone = _datasetModelSelectionMaxExitQueue.Rows[0].ItemArray[1].ToString();
                                    _agentNaturalSelectionISEngine.removeTraders();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                    _agentNaturalSelectionISEngine.register(strategy);
                                    listBoxNaturalSelection.Items.Clear();
                                    listBoxNaturalSelection.ForeColor = Color.Red;
                                    listBoxNaturalSelection.Items.Add(strategy.ExitIndicatorName);
                                    backgroundWorkerNaturalSelection.RunWorkerAsync();
                                    checkBoxNaturalSelection.Text = "Stop";
                                    comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                }
                                else
                                {
                                    if (_datasetModelSelectionMaxEntryQueue.Rows.Count > 0)
                                    {
                                        _agentNaturalSelectionISDisposition = true;
                                        _strategyChromosone = _datasetModelSelectionMaxEntryQueue.Rows[0].ItemArray[1].ToString();
                                        _agentNaturalSelectionISEngine.removeTraders();
                                        TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                                        _agentNaturalSelectionISEngine.register(strategy);
                                        listBoxNaturalSelection.Items.Clear();
                                        listBoxNaturalSelection.ForeColor = Color.LimeGreen;
                                        listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName);
                                        backgroundWorkerNaturalSelection.RunWorkerAsync();
                                        checkBoxNaturalSelection.Text = "Stop";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = false;
                                    }
                                    else
                                    {
                                        checkBoxNaturalSelection.Checked = false;
                                        progressBar1.Value = 0;
                                        listBoxNaturalSelection.Items.Clear();
                                        checkBoxNaturalSelection.Text = "Start";
                                        comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                                        textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                        timer2.Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _strategyChromosone = _agentNaturalSelectionQueue[0].ToString();
                        _agentNaturalSelectionQueue.RemoveAt(0);
                        TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosone, _agentNaturalSelectionISDisposition);
                        _agentNaturalSelectionISEngine.register(strategy);
                        listBoxNaturalSelection.Items.Clear();
                        if (_agentNaturalSelectionISDisposition == true)
                        {
                            listBoxNaturalSelection.ForeColor = Color.LimeGreen;
                            listBoxNaturalSelection.Items.Add(strategy.EntryIndicatorName);
                        }
                        else
                        {
                            listBoxNaturalSelection.ForeColor = Color.Red;
                            listBoxNaturalSelection.Items.Add(strategy.ExitIndicatorName);
                        }
                        backgroundWorkerNaturalSelection.RunWorkerAsync();
                    }
                }
                catch (Exception ex)
                {
                    checkBoxNaturalSelection.Checked = false;
                    progressBar1.Value = 0;
                    listBoxNaturalSelection.Items.Clear();
                    checkBoxNaturalSelection.Text = "Start";
                    comboBoxNaturalSelectionHistoricDisposition.Enabled = true;
                    textBoxNaturalSelection.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                }
            }
        }
        private void checkBoxNaturalSelectionOS_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                if (_agentModel10YearBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker6.Value.Subtract(new TimeSpan(10 * 365, 0, 0, 0)), dateTimePicker6.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBox5.AppendText(DateTime.Now + " : Generating 10 year baseline\r\n");
                    _agentModel10YearBaseline = GetBaseLine(engine, market, 3651);
                    textBox5.AppendText(DateTime.Now + " : Done (" + _agentModel10YearBaseline.ToString() + ")\r\n");
                }
                if (_agentModel5YearBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker6.Value.Subtract(new TimeSpan(5 * 365, 0, 0, 0)), dateTimePicker6.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBox5.AppendText(DateTime.Now + " : Generating 5 year baseline\r\n");
                    _agentModel5YearBaseline = GetBaseLine(engine, market, 1825);
                    textBox5.AppendText(DateTime.Now + " : Done (" + _agentModel5YearBaseline.ToString() + ")\r\n");
                }
                if (_agentModel3YearBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker6.Value.Subtract(new TimeSpan(3 * 365, 0, 0, 0)), dateTimePicker6.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBox5.AppendText(DateTime.Now + " : Generating 3 year baseline\r\n");
                    _agentModel3YearBaseline = GetBaseLine(engine, market, 1095);
                    textBox5.AppendText(DateTime.Now + " : Done (" + _agentModel3YearBaseline.ToString() + ")\r\n");
                }
                if (_agentModel1YearBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker6.Value.Subtract(new TimeSpan(1 * 365, 0, 0, 0)), dateTimePicker6.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBox5.AppendText(DateTime.Now + " : Generating 1 year baseline\r\n");
                    _agentModel1YearBaseline = GetBaseLine(engine, market, 365);
                    textBox5.AppendText(DateTime.Now + " : Done (" + _agentModel1YearBaseline.ToString() + ")\r\n");
                }
                if (_agentModel6MonthBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker6.Value.Subtract(new TimeSpan(6 * 30, 0, 0, 0)), dateTimePicker6.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBox5.AppendText(DateTime.Now + " : Generating 6 month baseline\r\n");
                    _agentModel6MonthBaseline = GetBaseLine(engine, market, 180);
                    textBox5.AppendText(DateTime.Now + " : Done (" + _agentModel6MonthBaseline.ToString() + ")\r\n");
                }
                if (_agentModel3MonthBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker6.Value.Subtract(new TimeSpan(3 * 30, 0, 0, 0)), dateTimePicker6.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBox5.AppendText(DateTime.Now + " : Generating 3 month baseline\r\n");
                    _agentModel3MonthBaseline = GetBaseLine(engine, market, 90);
                    textBox5.AppendText(DateTime.Now + " : Done (" + _agentModel3MonthBaseline.ToString() + ")\r\n");
                }
                if (_agentModel1MonthBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker6.Value.Subtract(new TimeSpan(1 * 30, 0, 0, 0)), dateTimePicker6.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBox5.AppendText(DateTime.Now + " : Generating 1 month baseline\r\n");
                    _agentModel1MonthBaseline = GetBaseLine(engine, market, 30);
                    textBox5.AppendText(DateTime.Now + " : Done (" + _agentModel1MonthBaseline.ToString() + ")\r\n");
                }
                if (_flagSelectionSampleCombo == true)
                {
                    try
                    {
                        _agentNaturalSelectionOOSEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker5.Value, dateTimePicker6.Value);
                        _agentNaturalSelectionOOSMarket = new Market();
                        _agentNaturalSelectionOOSMarket.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                        _agentNaturalSelectionOOSObserver = new Observer();
                        _agentNaturalSelectionOOSEngine.Observer = _agentNaturalSelectionOOSObserver;
                        textBox5.AppendText(DateTime.Now + " : Engine instantiated\r\n");
                        switch (comboBoxNaturalSelectionSampleDisposition.Text)
                        {
                            case "Long":
                                _agentNaturalSelectionOSDisposition = true;
                                break;
                            case "Short":
                                _agentNaturalSelectionOSDisposition = false;
                                break;
                            default:
                                break;
                        }
                        DataTable dt1;
                        DataTable dt2;
                        //if (_agentNaturalSelectionOSDisposition == true)
                        //{
                        if (_agentNaturalSelectionOSDisposition == true)
                        {
                            switch (comboBox2.Text)
                            {
                                case "10Y":
                                    dt1 = _datasetModelSelectionMaxEntryCache;
                                    dt2 = _datasetModelSelectionMaxExitCache;
                                    break;
                                case "5Y":
                                    dt1 = _datasetModelSelection10YearEntryCache;
                                    dt2 = _datasetModelSelection10YearExitCache;
                                    break;
                                case "3Y":
                                    dt1 = _datasetModelSelection5YearEntryCache;
                                    dt2 = _datasetModelSelection5YearExitCache;
                                    break;
                                case "1Y":
                                    dt1 = _datasetModelSelection3YearEntryCache;
                                    dt2 = _datasetModelSelection3YearExitCache;
                                    break;
                                default:
                                    dt1 = _datasetModelSelectionMaxEntryCache;
                                    dt2 = _datasetModelSelectionMaxExitCache;
                                    break;
                            }
                            if (dt1.Rows.Count > 0)
                            {
                                _strategyChromosoneOOS = dt1.Rows[0].ItemArray[1].ToString();
                                textBoxSelectioSampleCount.Text = dt1.Rows.Count.ToString();
                                _agentNaturalSelectionOOSEngine.removeTraders();
                                TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, true);
                                _agentNaturalSelectionOOSEngine.register(strategy);
                                listBoxNaturalSelectionSample.Items.Clear();
                                listBoxNaturalSelectionSample.ForeColor = Color.LimeGreen;
                                listBoxNaturalSelectionSample.Items.Add(strategy.EntryIndicatorName);
                                backgroundWorker1.RunWorkerAsync();
                                checkBox5.Text = "Stop";
                                comboBox2.Enabled = false;
                                comboBoxNaturalSelectionSampleDisposition.Enabled = false;
                                textBox5.AppendText(DateTime.Now + " : Simulation started\r\n");
                            }
                            else
                            {
                                if (dt2.Rows.Count > 0)
                                {
                                    _agentNaturalSelectionOSDisposition = false;
                                    _strategyChromosoneOOS = dt2.Rows[0].ItemArray[1].ToString();
                                    textBoxSelectioSampleCount.Text = dt2.Rows.Count.ToString();
                                    _agentNaturalSelectionOOSEngine.removeTraders();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, false);
                                    _agentNaturalSelectionOOSEngine.register(strategy);
                                    listBoxNaturalSelectionSample.Items.Clear();
                                    listBoxNaturalSelectionSample.ForeColor = Color.Red;
                                    listBoxNaturalSelectionSample.Items.Add(strategy.ExitIndicatorName);
                                    backgroundWorker1.RunWorkerAsync();
                                    checkBox5.Text = "Stop";
                                    comboBox2.Enabled = false;
                                    comboBoxNaturalSelectionSampleDisposition.Enabled = false;
                                    textBox5.AppendText(DateTime.Now + " : Simulation started\r\n");
                                }
                                else
                                {
                                    checkBox5.Checked = false;
                                    progressBar2.Value = 0;
                                    listBoxNaturalSelectionSample.Items.Clear();
                                    checkBox5.Text = "Start";
                                    comboBox2.Enabled = true;
                                    comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                                    textBox5.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                    timer1.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            switch (comboBox2.Text)
                            {
                                case "10Y":
                                    dt1 = _datasetModelSelectionMaxEntryCache;
                                    dt2 = _datasetModelSelectionMaxExitCache;
                                    break;
                                case "5Y":
                                    dt1 = _datasetModelSelection10YearEntryCache;
                                    dt2 = _datasetModelSelection10YearExitCache;
                                    break;
                                case "3Y":
                                    dt1 = _datasetModelSelection5YearEntryCache;
                                    dt2 = _datasetModelSelection5YearExitCache;
                                    break;
                                case "1Y":
                                    dt1 = _datasetModelSelection3YearEntryCache;
                                    dt2 = _datasetModelSelection3YearExitCache;
                                    break;
                                default:
                                    dt1 = _datasetModelSelectionMaxEntryCache;
                                    dt2 = _datasetModelSelectionMaxExitCache;
                                    break;
                            }
                            if (dt2.Rows.Count > 0)
                            {
                                _strategyChromosoneOOS = dt2.Rows[0].ItemArray[1].ToString();
                                textBoxSelectioSampleCount.Text = dt2.Rows.Count.ToString();
                                _agentNaturalSelectionOOSEngine.removeTraders();
                                TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, false);
                                _agentNaturalSelectionOOSEngine.register(strategy);
                                listBoxNaturalSelectionSample.Items.Clear();
                                listBoxNaturalSelectionSample.ForeColor = Color.Red;
                                listBoxNaturalSelectionSample.Items.Add(strategy.ExitIndicatorName);
                                backgroundWorker1.RunWorkerAsync();
                                checkBox5.Text = "Stop";
                                comboBox2.Enabled = false;
                                comboBoxNaturalSelectionSampleDisposition.Enabled = false;
                                textBox5.AppendText(DateTime.Now + " : Simulation started\r\n");
                            }
                            else
                            {
                                if (dt1.Rows.Count > 0)
                                {
                                    _agentNaturalSelectionOSDisposition = true;
                                    _strategyChromosoneOOS = dt1.Rows[0].ItemArray[1].ToString();
                                    textBoxSelectioSampleCount.Text = dt1.Rows.Count.ToString();
                                    _agentNaturalSelectionOOSEngine.removeTraders();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, true);
                                    _agentNaturalSelectionOOSEngine.register(strategy);
                                    listBoxNaturalSelectionSample.Items.Clear();
                                    listBoxNaturalSelectionSample.ForeColor = Color.LimeGreen;
                                    listBoxNaturalSelectionSample.Items.Add(strategy.EntryIndicatorName);
                                    backgroundWorker1.RunWorkerAsync();
                                    checkBox5.Text = "Stop";
                                    comboBox2.Enabled = false;
                                    comboBoxNaturalSelectionSampleDisposition.Enabled = false;
                                    textBox5.AppendText(DateTime.Now + " : Simulation started\r\n");
                                }
                                else
                                {
                                    checkBox5.Checked = false;
                                    progressBar2.Value = 0;
                                    listBoxNaturalSelectionSample.Items.Clear();
                                    checkBox5.Text = "Start";
                                    comboBox2.Enabled = true;
                                    comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                                    textBox5.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                    timer1.Enabled = true;
                                }
                            }
                        }
                        //}
                    }
                    catch (Exception ex)
                    {
                        checkBox5.Checked = false;
                        progressBar2.Value = 0;
                        listBoxNaturalSelectionSample.Items.Clear();
                        checkBox5.Text = "Start";
                        comboBox2.Enabled = true;
                        comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                        textBox5.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                    }
                }
                else
                {
                    try
                    {
                        _agentNaturalSelectionOOSEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker5.Value, dateTimePicker6.Value);
                        _agentNaturalSelectionOOSMarket = new Market();
                        _agentNaturalSelectionOOSMarket.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                        _agentNaturalSelectionOOSObserver = new Observer();
                        _agentNaturalSelectionOOSEngine.Observer = _agentNaturalSelectionOOSObserver;
                        textBox5.AppendText(DateTime.Now + " : Engine instantiated\r\n");
                        DataTable dt1;
                        switch (comboBox2.Text)
                        {
                            case "10Y":
                                dt1 = _datasetModelGeneticMaxCache;
                                break;
                            case "5Y":
                                dt1 = _datasetModelGenetic10YearCache;
                                break;
                            case "3Y":
                                dt1 = _datasetModelGenetic5YearCache;
                                break;
                            case "1Y":
                                dt1 = _datasetModelGenetic3YearCache;
                                break;
                            case "6M":
                                dt1 = _datasetModelGenetic1YearCache;
                                break;
                            case "3M":
                                dt1 = _datasetModelGenetic6MonthCache;
                                break;
                            case "1M":
                                dt1 = _datasetModelGenetic3MonthCache;
                                break;
                            default:
                                dt1 = _datasetModelGeneticMaxCache;
                                break;
                        }
                        if (dt1.Rows.Count > 0)
                        {
                            _strategyChromosoneOOS = dt1.Rows[0].ItemArray[1].ToString();
                            textBoxSelectioSampleCount.Text = dt1.Rows.Count.ToString();
                            _agentNaturalSelectionOOSEngine.removeTraders();
                            GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, _strategyChromosoneOOS);
                            _agentNaturalSelectionOOSEngine.register(strategy);
                            listBoxNaturalSelectionSample.Items.Clear();
                            listBoxNaturalSelectionSample.ForeColor = Color.Blue;
                            listBoxNaturalSelectionSample.Items.Add(strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName);
                            backgroundWorker1.RunWorkerAsync();
                            checkBox5.Text = "Stop";
                            comboBox2.Enabled = false;
                            comboBoxNaturalSelectionSampleDisposition.Enabled = false;
                            textBox5.AppendText(DateTime.Now + " : Simulation started\r\n");
                        }
                        else
                        {
                            checkBox5.Checked = false;
                            progressBar2.Value = 0;
                            listBoxNaturalSelectionSample.Items.Clear();
                            checkBox5.Text = "Start";
                            comboBox2.Enabled = true;
                            comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                            if (comboBox2.SelectedIndex != comboBox2.Items.Count - 1)
                            {
                                comboBox2.SelectedIndex = comboBox2.SelectedIndex + 1;
                                checkBox5.Checked = true;
                            }
                            else
                            {
                                if (_datasetModelGeneticMaxCache.Rows.Count == 0)
                                {
                                    Prune();
                                    textBox5.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                    timer1.Enabled = true;
                                }
                                else
                                {
                                    comboBox2.SelectedIndex = 0;
                                    checkBox5.Checked = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        checkBox5.Checked = false;
                        progressBar2.Value = 0;
                        listBoxNaturalSelectionSample.Items.Clear();
                        checkBox5.Text = "Start";
                        comboBox2.Enabled = true;
                        comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                        textBox5.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                    }
                }
            }
            else
            {
                backgroundWorker1.CancelAsync();
                textBoxSelectioSampleCount.Text = "";
            }
        }

        private void Prune()
        {
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Year))
            {
                if (_datasetModelGenetic1Year.Rows.Count > 0)
                {
                    for (int x = 0; x < _datasetModelGenetic1Year.Rows.Count; x++)
                    {
                        _datasetModelGeneticMaxQueue.ImportRow(_datasetModelGenetic1Year.Rows[x]);
                    }
                    _datasetModelGeneticMaxQueue.AcceptChanges();
                    _datasetModelGeneticMaxQueue = SupportClass.FilterSortDataTable(_datasetModelGeneticMaxQueue, "", "fitness", 1);
                    FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxQueue, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    _datasetModelGeneticMaxQueue.WriteXml(fout);
                    fout.Close();
                    textBoxSelectionHistoricComboQueueCount.Text = _datasetModelGeneticMaxQueue.Rows.Count.ToString();
                    if (checkBoxNaturalSelection.Checked == false)
                    {
                        comboBoxNaturalSelectionHistoricDisposition.SelectedIndex = 3;
                        checkBoxNaturalSelection.Checked = true;
                    }
                }
            }

            if (_datasetModelGenetic10Year.Rows.Count > 0)
            {
                ArrayList rowsToRemove = new ArrayList();
                for (int x = 0; x < _datasetModelGenetic10Year.Rows.Count; x++)
                {
                    DateTime inception = ((DateTime)(_datasetModelGenetic10Year.Rows[x].ItemArray[5]));
                    if (DateTime.Now.Subtract(inception).TotalDays > 7)
                    {
                        rowsToRemove.Add(_datasetModelGenetic10Year.Rows[x]);
                    }
                }
                for (int i = 0; i < rowsToRemove.Count; i++)
                {
                    _datasetModelGenetic10Year.Rows.Remove((DataRow)rowsToRemove[i]);
                }
                _datasetModelGenetic10Year.AcceptChanges();
                _datasetModelGenetic10Year = SupportClass.FilterSortDataTable(_datasetModelGenetic10Year, "", "fitness", 1);
                dataGridModelGenetic10Year.Invalidate();
                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10Year, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                _datasetModelGenetic10Year.WriteXml(fout);
                fout.Close();
            }



            /*
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10Year))
            {
                File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10Year);
                _datasetModelGenetic10Year.Clear();
                dataGridModelGenetic10Year.Invalidate();
            }
             */
            if (_datasetModelGenetic5Year.Rows.Count > 0)
            {
                ArrayList rowsToRemove = new ArrayList();
                for (int x = 0; x < _datasetModelGenetic5Year.Rows.Count; x++)
                {
                    DateTime inception = ((DateTime)(_datasetModelGenetic5Year.Rows[x].ItemArray[5]));
                    if (DateTime.Now.Subtract(inception).TotalDays > 7)
                    {
                        rowsToRemove.Add(_datasetModelGenetic5Year.Rows[x]);
                    }
                }
                for (int i = 0; i < rowsToRemove.Count; i++)
                {
                    _datasetModelGenetic5Year.Rows.Remove((DataRow)rowsToRemove[i]);
                }
                _datasetModelGenetic5Year.AcceptChanges();
                _datasetModelGenetic5Year = SupportClass.FilterSortDataTable(_datasetModelGenetic5Year, "", "fitness", 1);
                dataGridModelGenetic5Year.Invalidate();
                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5Year, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                _datasetModelGenetic5Year.WriteXml(fout);
                fout.Close();
            }



            /* 
           if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5Year))
           {
               File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5Year);
               _datasetModelGenetic5Year.Clear();
               dataGridModelGenetic5Year.Invalidate();
           }
            */
            if (_datasetModelGenetic3Year.Rows.Count > 0)
            {
                ArrayList rowsToRemove = new ArrayList();
                for (int x = 0; x < _datasetModelGenetic3Year.Rows.Count; x++)
                {
                    DateTime inception = ((DateTime)(_datasetModelGenetic3Year.Rows[x].ItemArray[5]));
                    if (DateTime.Now.Subtract(inception).TotalDays > 7)
                    {
                        rowsToRemove.Add(_datasetModelGenetic3Year.Rows[x]);
                    }
                }
                for (int i = 0; i < rowsToRemove.Count; i++)
                {
                    _datasetModelGenetic3Year.Rows.Remove((DataRow)rowsToRemove[i]);
                }
                _datasetModelGenetic3Year.AcceptChanges();
                _datasetModelGenetic3Year = SupportClass.FilterSortDataTable(_datasetModelGenetic3Year, "", "fitness", 1);
                dataGridModelGenetic3Year.Invalidate();
                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Year, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                _datasetModelGenetic3Year.WriteXml(fout);
                fout.Close();
            }


            /*
           if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Year))
           {
               File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Year);
               _datasetModelGenetic3Year.Clear();
               dataGridModelGenetic3Year.Invalidate();
           }
            */
            if (_datasetModelGenetic1Year.Rows.Count > 0)
            {
                ArrayList rowsToRemove = new ArrayList();
                for (int x = 0; x < _datasetModelGenetic1Year.Rows.Count; x++)
                {
                    DateTime inception = ((DateTime)(_datasetModelGenetic1Year.Rows[x].ItemArray[5]));
                    if (DateTime.Now.Subtract(inception).TotalDays > 7)
                    {
                        rowsToRemove.Add(_datasetModelGenetic1Year.Rows[x]);
                    }
                }
                for (int i = 0; i < rowsToRemove.Count; i++)
                {
                    _datasetModelGenetic1Year.Rows.Remove((DataRow)rowsToRemove[i]);
                }
                _datasetModelGenetic1Year.AcceptChanges();
                _datasetModelGenetic1Year = SupportClass.FilterSortDataTable(_datasetModelGenetic1Year, "", "fitness", 1);
                dataGridModelGenetic1Year.Invalidate();
                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Year, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                _datasetModelGenetic1Year.WriteXml(fout);
                fout.Close();
            }

            /* 
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Year))
            {
                File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Year);
                _datasetModelGenetic1Year.Clear();
                dataGridModelGenetic1Year.Invalidate();
            }
             */
            if (_datasetModelGenetic6Month.Rows.Count > 0)
            {
                ArrayList rowsToRemove = new ArrayList();
                for (int x = 0; x < _datasetModelGenetic6Month.Rows.Count; x++)
                {
                    DateTime inception = ((DateTime)(_datasetModelGenetic6Month.Rows[x].ItemArray[5]));
                    if (DateTime.Now.Subtract(inception).TotalDays > 7)
                    {
                        rowsToRemove.Add(_datasetModelGenetic6Month.Rows[x]);
                    }
                }
                for (int i = 0; i < rowsToRemove.Count; i++)
                {
                    _datasetModelGenetic6Month.Rows.Remove((DataRow)rowsToRemove[i]);
                }
                _datasetModelGenetic6Month.AcceptChanges();
                _datasetModelGenetic6Month = SupportClass.FilterSortDataTable(_datasetModelGenetic6Month, "", "fitness", 1);
                dataGridModelGenetic6Month.Invalidate();
                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6Month, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                _datasetModelGenetic6Month.WriteXml(fout);
                fout.Close();
            }



            /*
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6Month))
            {
                File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6Month);
                _datasetModelGenetic6Month.Clear();
                dataGridModelGenetic6Month.Invalidate();
            }
             */
            if (_datasetModelGenetic3Month.Rows.Count > 0)
            {
                ArrayList rowsToRemove = new ArrayList();
                for (int x = 0; x < _datasetModelGenetic3Month.Rows.Count; x++)
                {
                    DateTime inception = ((DateTime)(_datasetModelGenetic3Month.Rows[x].ItemArray[5]));
                    if (DateTime.Now.Subtract(inception).TotalDays > 7)
                    {
                        rowsToRemove.Add(_datasetModelGenetic3Month.Rows[x]);
                    }
                }
                for (int i = 0; i < rowsToRemove.Count; i++)
                {
                    _datasetModelGenetic3Month.Rows.Remove((DataRow)rowsToRemove[i]);
                }
                _datasetModelGenetic3Month.AcceptChanges();
                _datasetModelGenetic3Month = SupportClass.FilterSortDataTable(_datasetModelGenetic3Month, "", "fitness", 1);
                dataGridModelGenetic3Month.Invalidate();
                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Month, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                _datasetModelGenetic3Month.WriteXml(fout);
                fout.Close();
            }

            /*
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Month))
            {
                File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Month);
                _datasetModelGenetic3Month.Clear();
                dataGridModelGenetic3Month.Invalidate();
            }
             */
            if (_datasetModelGenetic1Month.Rows.Count > 0)
            {
                ArrayList rowsToRemove = new ArrayList();
                for (int x = 0; x < _datasetModelGenetic1Month.Rows.Count; x++)
                {
                    DateTime inception = ((DateTime)(_datasetModelGenetic1Month.Rows[x].ItemArray[5]));
                    if (DateTime.Now.Subtract(inception).TotalDays > 7)
                    {
                        rowsToRemove.Add(_datasetModelGenetic1Month.Rows[x]);
                    }
                }
                for (int i = 0; i < rowsToRemove.Count; i++)
                {
                    _datasetModelGenetic1Month.Rows.Remove((DataRow)rowsToRemove[i]);
                }
                _datasetModelGenetic1Month.AcceptChanges();
                _datasetModelGenetic1Month = SupportClass.FilterSortDataTable(_datasetModelGenetic1Month, "", "fitness", 1);
                dataGridModelGenetic1Month.Invalidate();
                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Month, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                _datasetModelGenetic1Month.WriteXml(fout);
                fout.Close();
            }
        }

        private void backgroundWorkerNaturalSelectionOS_DoWork(object sender, DoWorkEventArgs e)
        {
            _agentNaturalSelectionOOSStart = DateTime.Now;
            _agentNaturalSelectionOOSMarket.random = false;
            _agentNaturalSelectionOOSLog = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs + Properties.Settings.Default.GlobalMarket + "NATIURAL-SELECTION-LONG" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (_flagSelectionSampleCombo == true)
            {
                if (_agentNaturalSelectionOSDisposition == true)
                {
                    switch (_agentModelSelectionSampleLength)
                    {
                        case "10Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection10YearEntry, ref _datasetModelSelection10YearEntryCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearEntry, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearEntryCache, ref _agentNaturalSelectionOOSWin, true, _agentModel10YearBaseline);
                            break;
                        case "5Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection5YearEntry, ref _datasetModelSelection5YearEntryCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearEntry, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearEntryCache, ref _agentNaturalSelectionOOSWin, true, _agentModel5YearBaseline);
                            break;
                        case "3Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection3YearEntry, ref _datasetModelSelection3YearEntryCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearEntry, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearEntryCache, ref _agentNaturalSelectionOOSWin, true, _agentModel3YearBaseline);
                            break;
                        case "1Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection1YearEntry, ref _datasetModelSelection1YearEntryCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearEntry, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearEntryCache, ref _agentNaturalSelectionOOSWin, true, _agentModel1YearBaseline);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (_agentModelSelectionSampleLength)
                    {
                        case "10Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection10YearExit, ref _datasetModelSelection10YearExitCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearExit, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearExitCache, ref _agentNaturalSelectionOOSWin, false, _agentModel10YearBaseline);
                            break;
                        case "5Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection5YearExit, ref _datasetModelSelection5YearExitCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearExit, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearExitCache, ref _agentNaturalSelectionOOSWin, false, _agentModel5YearBaseline);
                            break;
                        case "3Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection3YearExit, ref _datasetModelSelection3YearExitCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearExit, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearExitCache, ref _agentNaturalSelectionOOSWin, false, _agentModel3YearBaseline);
                            break;
                        case "1Y":
                            _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelSelection1YearExit, ref _datasetModelSelection1YearExitCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearExit, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearExitCache, ref _agentNaturalSelectionOOSWin, false, _agentModel1YearBaseline);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                switch (_agentModelSelectionSampleLength)
                {
                    case "10Y":
                        _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelGenetic10Year, ref _datasetModelGenetic10YearCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10Year, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10YearCache, ref _agentNaturalSelectionOOSWin, true, _agentModel10YearBaseline);
                        break;
                    case "5Y":
                        _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelGenetic5Year, ref _datasetModelGenetic5YearCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5Year, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5YearCache, ref _agentNaturalSelectionOOSWin, true, _agentModel5YearBaseline);
                        break;
                    case "3Y":
                        _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelGenetic3Year, ref _datasetModelGenetic3YearCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Year, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3YearCache, ref _agentNaturalSelectionOOSWin, true, _agentModel3YearBaseline);
                        break;
                    case "1Y":
                        _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelGenetic1Year, ref _datasetModelGenetic1YearCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Year, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1YearCache, ref _agentNaturalSelectionOOSWin, true, _agentModel1YearBaseline);
                        break;
                    case "6M":
                        _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelGenetic6Month, ref _datasetModelGenetic6MonthCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6Month, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6MonthCache, ref _agentNaturalSelectionOOSWin, true, _agentModel6MonthBaseline);
                        break;
                    case "3M":
                        _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelGenetic3Month, ref _datasetModelGenetic3MonthCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Month, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3MonthCache, ref _agentNaturalSelectionOOSWin, true, _agentModel3MonthBaseline);
                        break;
                    case "1M":
                        _strategyTester.BackTest(backgroundWorker1, e, _agentNaturalSelectionOOSLog, _agentNaturalSelectionOOSEngine, _agentNaturalSelectionOOSMarket, ref _agentNaturalSelectionOOSScore, ref _datasetModelGenetic1Month, ref _datasetModelGenetic1MonthCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Month, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1MonthCache, ref _agentNaturalSelectionOOSWin, true, _agentModel1MonthBaseline);
                        break;
                    default:
                        break;
                }
            }
        }
        private void backgroundWorkerNaturalSelectionOS_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar2.Value = e.ProgressPercentage;
        }
        private void backgroundWorkerNaturalSelectionOS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (e.Error != null)
            {
                checkBox5.Checked = false;
                progressBar2.Value = 0;
                listBoxNaturalSelectionSample.Items.Clear();
                checkBox5.Text = "Start";
                comboBox2.Enabled = true;
                comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                textBox5.AppendText(DateTime.Now + " : Operation produced error: " + e.Error.Message + "\r\n");
            }
            else if (e.Cancelled)
            {
                checkBox5.Checked = false;
                progressBar2.Value = 0;
                listBoxNaturalSelectionSample.Items.Clear();
                checkBox5.Text = "Start";
                comboBox2.Enabled = true;
                comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                textBox5.AppendText(DateTime.Now + " : Operation cancelled\r\n");
            }
            else
            {
                textBox5.AppendText(_agentNaturalSelectionOOSScore);
                _agentNaturalSelectionOOSStop = DateTime.Now;
                double milli = _agentNaturalSelectionOOSStop.Subtract(_agentNaturalSelectionOOSStart).TotalMilliseconds;
                label4.Text = _agentNaturalSelectionOOSStop.ToString() + " : " + milli.ToString() + " ms = " + SupportClass.ConvertToMinutes(milli).ToString() + " min";
                if (_flagSelectionSampleCombo == true)
                {
                    if (_agentNaturalSelectionOSDisposition == true)
                    {
                        switch (comboBox2.Text)
                        {
                            case "10Y":
                                for (int x = 0; x < _datasetModelSelectionMaxEntryCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelectionMaxEntryCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelectionMaxEntryCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelectionMaxEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntryCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelectionMaxEntryCache.WriteXml(fout);
                                fout.Close();
                                break;
                            case "5Y":
                                for (int x = 0; x < _datasetModelSelection10YearEntryCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelection10YearEntryCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelection10YearEntryCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelection10YearEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelection10YearEntryCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearEntryCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelection10YearEntryCache.WriteXml(fout);
                                fout.Close();
                                break;
                            case "3Y":
                                for (int x = 0; x < _datasetModelSelection5YearEntryCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelection5YearEntryCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelection5YearEntryCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelection5YearEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelection5YearEntryCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearEntryCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelection5YearEntryCache.WriteXml(fout);
                                fout.Close();
                                break;
                            case "1Y":
                                for (int x = 0; x < _datasetModelSelection3YearEntryCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelection3YearEntryCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelection3YearEntryCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelection3YearEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelection3YearEntryCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearEntryCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelection3YearEntryCache.WriteXml(fout);
                                fout.Close();
                                break;
                            default:
                                for (int x = 0; x < _datasetModelSelectionMaxEntryCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelectionMaxEntryCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelectionMaxEntryCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelectionMaxEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntryCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelectionMaxEntryCache.WriteXml(fout);
                                fout.Close();
                                break;
                        }
                        switch (comboBox2.Text)
                        {
                            case "10Y":
                                _datasetModelSelection10YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection10YearEntry, "", "fitness", 1);
                                break;
                            case "5Y":
                                _datasetModelSelection5YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection5YearEntry, "", "fitness", 1);
                                break;
                            case "3Y":
                                _datasetModelSelection3YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection3YearEntry, "", "fitness", 1);
                                break;
                            case "1Y":
                                _datasetModelSelection1YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection1YearEntry, "", "fitness", 1);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (comboBox2.Text)
                        {
                            case "10Y":
                                for (int x = 0; x < _datasetModelSelectionMaxExitCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelectionMaxExitCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelectionMaxExitCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelectionMaxExitCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExitCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelectionMaxExitCache.WriteXml(fout);
                                fout.Close();
                                break;
                            case "5Y":
                                for (int x = 0; x < _datasetModelSelection10YearExitCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelection10YearExitCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelection10YearExitCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelection10YearExitCache = SupportClass.FilterSortDataTable(_datasetModelSelection10YearExitCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearExitCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelection10YearExitCache.WriteXml(fout);
                                fout.Close();
                                break;
                            case "3Y":
                                for (int x = 0; x < _datasetModelSelection5YearExitCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelection5YearExitCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelection5YearExitCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelection5YearExitCache = SupportClass.FilterSortDataTable(_datasetModelSelection5YearExitCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearExitCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelection5YearExitCache.WriteXml(fout);
                                fout.Close();
                                break;
                            case "1Y":
                                for (int x = 0; x < _datasetModelSelection3YearExitCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelection3YearExitCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelection3YearExitCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelection3YearExitCache = SupportClass.FilterSortDataTable(_datasetModelSelection3YearExitCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearExitCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelection3YearExitCache.WriteXml(fout);
                                fout.Close();
                                break;
                            default:
                                for (int x = 0; x < _datasetModelSelectionMaxExitCache.Rows.Count; x++)
                                {
                                    if (_datasetModelSelectionMaxExitCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                    {
                                        _datasetModelSelectionMaxExitCache.Rows[x].Delete();
                                    }
                                }
                                _datasetModelSelectionMaxExitCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExitCache, "", "fitness", 1);
                                //dataGridHistoricCacheEntry.DataSource = _datasetHistoricCacheEntry;
                                fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                _datasetModelSelectionMaxExitCache.WriteXml(fout);
                                fout.Close();
                                break;
                        }
                        switch (comboBox2.Text)
                        {
                            case "10Y":
                                _datasetModelSelection10YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection10YearExit, "", "fitness", 1);
                                break;
                            case "5Y":
                                _datasetModelSelection5YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection5YearExit, "", "fitness", 1);
                                break;
                            case "3Y":
                                _datasetModelSelection3YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection3YearExit, "", "fitness", 1);
                                break;
                            case "1Y":
                                _datasetModelSelection1YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection1YearExit, "", "fitness", 1);
                                break;
                            default:
                                break;
                        }
                    }
                    _agentNaturalSelectionOOSEngine.removeTraders();
                    switch (comboBoxNaturalSelectionSampleDisposition.Text)
                    {
                        case "Long":
                            _agentNaturalSelectionOSDisposition = true;
                            break;
                        case "Short":
                            _agentNaturalSelectionOSDisposition = false;
                            break;
                        case "Rotate":
                            _agentNaturalSelectionOSDisposition = !_agentNaturalSelectionOSDisposition;
                            break;
                        default:
                            break;
                    }
                    try
                    {
                        DataTable dt1;
                        DataTable dt2;
                        if (_agentNaturalSelectionOSDisposition == true)
                        {
                            switch (comboBox2.Text)
                            {
                                case "10Y":
                                    dt1 = _datasetModelSelectionMaxEntryCache;
                                    dt2 = _datasetModelSelectionMaxExitCache;
                                    break;
                                case "5Y":
                                    dt1 = _datasetModelSelection10YearEntryCache;
                                    dt2 = _datasetModelSelection10YearExitCache;
                                    break;
                                case "3Y":
                                    dt1 = _datasetModelSelection5YearEntryCache;
                                    dt2 = _datasetModelSelection5YearExitCache;
                                    break;
                                case "1Y":
                                    dt1 = _datasetModelSelection3YearEntryCache;
                                    dt2 = _datasetModelSelection3YearExitCache;
                                    break;
                                default:
                                    dt1 = _datasetModelSelectionMaxEntryCache;
                                    dt2 = _datasetModelSelectionMaxExitCache;
                                    break;
                            }
                            if (dt1.Rows.Count > 0)
                            {
                                _strategyChromosoneOOS = dt1.Rows[0].ItemArray[1].ToString();
                                textBoxSelectioSampleCount.Text = dt1.Rows.Count.ToString();
                                TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, true);
                                _agentNaturalSelectionOOSEngine.register(strategy);
                                listBoxNaturalSelectionSample.Items.Clear();
                                listBoxNaturalSelectionSample.ForeColor = Color.LimeGreen;
                                listBoxNaturalSelectionSample.Items.Add(strategy.EntryIndicatorName);
                                backgroundWorker1.RunWorkerAsync();
                            }
                            else
                            {
                                if (dt2.Rows.Count > 0)
                                {
                                    _agentNaturalSelectionOSDisposition = false;
                                    _strategyChromosoneOOS = dt2.Rows[0].ItemArray[1].ToString();
                                    textBoxSelectioSampleCount.Text = dt2.Rows.Count.ToString();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, false);
                                    _agentNaturalSelectionOOSEngine.register(strategy);
                                    listBoxNaturalSelectionSample.Items.Clear();
                                    listBoxNaturalSelectionSample.ForeColor = Color.Red;
                                    listBoxNaturalSelectionSample.Items.Add(strategy.ExitIndicatorName);
                                    backgroundWorker1.RunWorkerAsync();
                                }
                                else
                                {
                                    checkBox5.Checked = false;
                                    progressBar2.Value = 0;
                                    listBoxNaturalSelectionSample.Items.Clear();
                                    checkBox5.Text = "Start";
                                    comboBox2.Enabled = true;
                                    comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                                    textBox5.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                    timer1.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            if (_agentNaturalSelectionOSDisposition == false)
                            {
                                switch (comboBox2.Text)
                                {
                                    case "10Y":
                                        dt1 = _datasetModelSelectionMaxEntryCache;
                                        dt2 = _datasetModelSelectionMaxExitCache;
                                        break;
                                    case "5Y":
                                        dt1 = _datasetModelSelection10YearEntryCache;
                                        dt2 = _datasetModelSelection10YearExitCache;
                                        break;
                                    case "3Y":
                                        dt1 = _datasetModelSelection5YearEntryCache;
                                        dt2 = _datasetModelSelection5YearExitCache;
                                        break;
                                    case "1Y":
                                        dt1 = _datasetModelSelection3YearEntryCache;
                                        dt2 = _datasetModelSelection3YearExitCache;
                                        break;
                                    default:
                                        dt1 = _datasetModelSelectionMaxEntryCache;
                                        dt2 = _datasetModelSelectionMaxExitCache;
                                        break;
                                }
                                if (dt2.Rows.Count > 0)
                                {
                                    _strategyChromosoneOOS = dt2.Rows[0].ItemArray[1].ToString();
                                    textBoxSelectioSampleCount.Text = dt2.Rows.Count.ToString();
                                    TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, false);
                                    _agentNaturalSelectionOOSEngine.register(strategy);
                                    listBoxNaturalSelectionSample.Items.Clear();
                                    listBoxNaturalSelectionSample.ForeColor = Color.Red;
                                    listBoxNaturalSelectionSample.Items.Add(strategy.ExitIndicatorName);
                                    backgroundWorker1.RunWorkerAsync();
                                }
                                else
                                {
                                    if (dt1.Rows.Count > 0)
                                    {
                                        _agentNaturalSelectionOSDisposition = true;
                                        _strategyChromosoneOOS = dt1.Rows[0].ItemArray[1].ToString();
                                        textBoxSelectioSampleCount.Text = dt1.Rows.Count.ToString();
                                        TestStrategy strategy = new TestStrategy(ref _pluginService, _strategyChromosoneOOS, true);
                                        _agentNaturalSelectionOOSEngine.register(strategy);
                                        listBoxNaturalSelectionSample.Items.Clear();
                                        listBoxNaturalSelectionSample.ForeColor = Color.LimeGreen;
                                        listBoxNaturalSelectionSample.Items.Add(strategy.EntryIndicatorName);
                                        backgroundWorker1.RunWorkerAsync();
                                    }
                                    else
                                    {
                                        checkBox5.Checked = false;
                                        progressBar2.Value = 0;
                                        listBoxNaturalSelectionSample.Items.Clear();
                                        checkBox5.Text = "Start";
                                        comboBox2.Enabled = true;
                                        comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                                        textBox5.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                        timer1.Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        checkBox5.Checked = false;
                        progressBar2.Value = 0;
                        listBoxNaturalSelectionSample.Items.Clear();
                        checkBox5.Text = "Start";
                        comboBox2.Enabled = true;
                        comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                        textBox5.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                    }
                }
                else
                {
                    switch (comboBox2.Text)
                    {
                        case "10Y":
                            for (int x = 0; x < _datasetModelGeneticMaxCache.Rows.Count; x++)
                            {
                                if (_datasetModelGeneticMaxCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGeneticMaxCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGeneticMaxCache = SupportClass.FilterSortDataTable(_datasetModelGeneticMaxCache, "", "fitness", 1);
                            FileStream fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGeneticMaxCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic10Year = SupportClass.FilterSortDataTable(_datasetModelGenetic10Year, "", "fitness", 1);
                            dataGridModelGenetic10Year.DataSource = _datasetModelGenetic10Year;
                            break;
                        case "5Y":
                            for (int x = 0; x < _datasetModelGenetic10YearCache.Rows.Count; x++)
                            {
                                if (_datasetModelGenetic10YearCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGenetic10YearCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGenetic10YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic10YearCache, "", "fitness", 1);
                            fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10YearCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGenetic10YearCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic5Year = SupportClass.FilterSortDataTable(_datasetModelGenetic5Year, "", "fitness", 1);
                            dataGridModelGenetic5Year.DataSource = _datasetModelGenetic5Year;
                            break;
                        case "3Y":
                            for (int x = 0; x < _datasetModelGenetic5YearCache.Rows.Count; x++)
                            {
                                if (_datasetModelGenetic5YearCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGenetic5YearCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGenetic5YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic5YearCache, "", "fitness", 1);
                            fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5YearCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGenetic5YearCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic3Year = SupportClass.FilterSortDataTable(_datasetModelGenetic3Year, "", "fitness", 1);
                            dataGridModelGenetic3Year.DataSource = _datasetModelGenetic3Year;
                            break;
                        case "1Y":
                            for (int x = 0; x < _datasetModelGenetic3YearCache.Rows.Count; x++)
                            {
                                if (_datasetModelGenetic3YearCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGenetic3YearCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGenetic3YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic3YearCache, "", "fitness", 1);
                            fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3YearCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGenetic3YearCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic1Year = SupportClass.FilterSortDataTable(_datasetModelGenetic1Year, "", "fitness", 1);
                            dataGridModelGenetic1Year.DataSource = _datasetModelGenetic1Year;
                            break;
                        case "6M":
                            for (int x = 0; x < _datasetModelGenetic1YearCache.Rows.Count; x++)
                            {
                                if (_datasetModelGenetic1YearCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGenetic1YearCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGenetic1YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic1YearCache, "", "fitness", 1);
                            fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1YearCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGenetic1YearCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic6Month = SupportClass.FilterSortDataTable(_datasetModelGenetic6Month, "", "fitness", 1);
                            dataGridModelGenetic6Month.DataSource = _datasetModelGenetic6Month;
                            break;
                        case "3M":
                            for (int x = 0; x < _datasetModelGenetic6MonthCache.Rows.Count; x++)
                            {
                                if (_datasetModelGenetic6MonthCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGenetic6MonthCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGenetic6MonthCache = SupportClass.FilterSortDataTable(_datasetModelGenetic6MonthCache, "", "fitness", 1);
                            fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6MonthCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGenetic6MonthCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic3Month = SupportClass.FilterSortDataTable(_datasetModelGenetic3Month, "", "fitness", 1);
                            dataGridModelGenetic3Month.DataSource = _datasetModelGenetic3Month;
                            break;
                        case "1M":
                            for (int x = 0; x < _datasetModelGenetic3MonthCache.Rows.Count; x++)
                            {
                                if (_datasetModelGenetic3MonthCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGenetic3MonthCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGenetic3MonthCache = SupportClass.FilterSortDataTable(_datasetModelGenetic3MonthCache, "", "fitness", 1);
                            fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3MonthCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGenetic3MonthCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic1Month = SupportClass.FilterSortDataTable(_datasetModelGenetic1Month, "", "fitness", 1);
                            dataGridModelGenetic1Month.DataSource = _datasetModelGenetic1Month;
                            break;
                        default:
                            for (int x = 0; x < _datasetModelGeneticMaxCache.Rows.Count; x++)
                            {
                                if (_datasetModelGeneticMaxCache.Rows[x].ItemArray[1].ToString() == _strategyChromosoneOOS)
                                {
                                    _datasetModelGeneticMaxCache.Rows[x].Delete();
                                }
                            }
                            _datasetModelGeneticMaxCache = SupportClass.FilterSortDataTable(_datasetModelGeneticMaxCache, "", "fitness", 1);
                            fout = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxCache, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            _datasetModelGeneticMaxCache.WriteXml(fout);
                            fout.Close();
                            _datasetModelGenetic10Year = SupportClass.FilterSortDataTable(_datasetModelGenetic10Year, "", "fitness", 1);
                            dataGridModelGenetic10Year.DataSource = _datasetModelGenetic10Year;
                            break;
                    }
                    _agentNaturalSelectionOOSEngine.removeTraders();
                    try
                    {
                        DataTable dt1;
                        switch (comboBox2.Text)
                        {
                            case "10Y":
                                dt1 = _datasetModelGeneticMaxCache;
                                break;
                            case "5Y":
                                dt1 = _datasetModelGenetic10YearCache;
                                break;
                            case "3Y":
                                dt1 = _datasetModelGenetic5YearCache;
                                break;
                            case "1Y":
                                dt1 = _datasetModelGenetic3YearCache;
                                break;
                            case "6M":
                                dt1 = _datasetModelGenetic1YearCache;
                                break;
                            case "3M":
                                dt1 = _datasetModelGenetic6MonthCache;
                                break;
                            case "1M":
                                dt1 = _datasetModelGenetic3MonthCache;
                                break;
                            default:
                                dt1 = _datasetModelGeneticMaxCache;
                                break;
                        }
                        if (dt1.Rows.Count > 0)
                        {
                            _strategyChromosoneOOS = dt1.Rows[0].ItemArray[1].ToString();
                            textBoxSelectioSampleCount.Text = dt1.Rows.Count.ToString();
                            _agentNaturalSelectionOOSEngine.removeTraders();
                            GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, _strategyChromosoneOOS);
                            _agentNaturalSelectionOOSEngine.register(strategy);
                            listBoxNaturalSelectionSample.Items.Clear();
                            listBoxNaturalSelectionSample.ForeColor = Color.Blue;
                            listBoxNaturalSelectionSample.Items.Add(strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName);
                            backgroundWorker1.RunWorkerAsync();
                            checkBox5.Text = "Stop";
                            comboBox2.Enabled = false;
                            comboBoxNaturalSelectionSampleDisposition.Enabled = false;
                        }
                        else
                        {
                            checkBox5.Checked = false;
                            progressBar2.Value = 0;
                            listBoxNaturalSelectionSample.Items.Clear();
                            checkBox5.Text = "Start";
                            comboBox2.Enabled = true;
                            comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                            if (comboBox2.SelectedIndex != comboBox2.Items.Count - 1)
                            {
                                comboBox2.SelectedIndex = comboBox2.SelectedIndex + 1;
                                checkBox5.Checked = true;
                            }
                            else
                            {
                                if (_datasetModelGeneticMaxCache.Rows.Count == 0)
                                {
                                    Prune();
                                    textBox5.AppendText(DateTime.Now + " : Operation halted (retry @ " + DateTime.Now.AddMilliseconds(timer1.Interval).ToShortTimeString() + ")\r\n");
                                    timer1.Enabled = true;
                                }
                                else
                                {
                                    comboBox2.SelectedIndex = 0;
                                    checkBox5.Checked = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        checkBox5.Checked = false;
                        progressBar2.Value = 0;
                        listBoxNaturalSelectionSample.Items.Clear();
                        checkBox5.Text = "Start";
                        comboBox2.Enabled = true;
                        comboBoxNaturalSelectionSampleDisposition.Enabled = true;
                        textBox5.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                    }
                }
            }
        }
        private void checkBoxGeneticEvolution_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxGeneticEvolution.Checked == true)  // Start genetic evolution agent
            {
                if (_agentModelHistoricBaseline == 0)
                {
                    Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker3.Value, dateTimePicker4.Value);
                    Market market = new Market();
                    market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    Observer observer = new Observer();
                    engine.Observer = observer;
                    textBoxGeneticEvolution.AppendText(DateTime.Now + " : Generating baseline\r\n");
                    _agentModelHistoricBaseline = GetBaseLine(engine, market, 36514);
                    textBoxGeneticEvolution.AppendText(DateTime.Now + " : Done (" + _agentModelHistoricBaseline.ToString() + ")\r\n");
                }
                _agentGeneticEvolutionEngine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, dateTimePicker7.Value, dateTimePicker8.Value);
                _agentGeneticEvolutionMarket = new Market();
                _agentGeneticEvolutionMarket.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                _agentGeneticEvolutionObserver = new Observer();
                _agentGeneticEvolutionEngine.Observer = _agentGeneticEvolutionObserver;
                textBoxGeneticEvolution.AppendText(DateTime.Now + " : Engine instantiated\r\n");
                try
                {
                    if (_agentModelGeneticQueueEntry.Count < 1 && _agentModelGeneticQueueExit.Count < 1)
                    {
                        if ((int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0 && (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0)
                        {
                            if (checkBoxModelEvolutionRandom.Checked == true)
                            {
                                ArrayList entryPopulation = new ArrayList();
                                ArrayList exitPopulation = new ArrayList();
                                if (comboBoxCriteria.SelectedText != "")
                                {
                                    _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", comboBoxCriteria.SelectedText, 1);
                                    _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", comboBoxCriteria.SelectedText, 1);
                                }
                                for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                {
                                    entryPopulation.Add(_datasetModelSelectionMaxEntry.Rows[i].ItemArray[1].ToString());
                                }
                                for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                {
                                    exitPopulation.Add(_datasetModelSelectionMaxExit.Rows[i].ItemArray[1].ToString());
                                }
                                entryPopulation = SplitGenome(entryPopulation);
                                //entryPopulation = UpdateTargetDistribution(_datasetResultsOOSEntry, ref numericUpDown7, ref numericUpDown8);
                                exitPopulation = SplitGenome(exitPopulation);
                                //exitPopulation = UpdateTargetDistribution(_datasetResultsOOSExit, ref numericUpDown9, ref numericUpDown10);
                                bool duplicate = true;
                                while (duplicate == true)
                                {
                                    GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, entryPopulation, exitPopulation);
                                    if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                                    {
                                        listBoxGeneticEvolutionEntry.Items.Clear();
                                        listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                                        listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                                        listBoxGeneticEvolutionExit.Items.Clear();
                                        listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                                        listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                                        duplicate = false;
                                    }
                                    _agentGeneticEvolutionEngine.removeTraders();
                                    _agentGeneticEvolutionEngine.register(strategy);
                                }
                            }
                            else
                            {
                                bool duplicate = true;
                                Random rndEntry = new Random();
                                Random rndExit = new Random();
                                while (duplicate == true)
                                {
                                    int entry = rndEntry.Next((int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value) - 1);
                                    int exit = rndExit.Next((int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value) - 1);
                                    GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, _datasetModelSelectionMaxEntry.Rows[entry].ItemArray[1].ToString(), _datasetModelSelectionMaxExit.Rows[exit].ItemArray[1].ToString());
                                    if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                                    {
                                        listBoxGeneticEvolutionEntry.Items.Clear();
                                        listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                                        listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                                        listBoxGeneticEvolutionExit.Items.Clear();
                                        listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                                        listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                                        duplicate = false;
                                    }
                                    _agentGeneticEvolutionEngine.removeTraders();
                                    _agentGeneticEvolutionEngine.register(strategy);
                                }
                            }
                            backgroundWorkerGeneticEvolution.RunWorkerAsync();
                            textBoxGeneticEvolution.AppendText(DateTime.Now + " : Simulation started\r\n");
                            checkBoxGeneticEvolution.Text = "Stop";
                            comboBoxCriteria.Enabled = false;
                            numericUpDown6.Enabled = false;
                            checkBoxModelEvolutionRandom.Enabled = false;
                        }
                        else
                        {
                            checkBoxGeneticEvolution.Checked = false;
                            progressBar3.Value = 0;
                            listBoxGeneticEvolutionEntry.Items.Clear();
                            listBoxGeneticEvolutionExit.Items.Clear();
                            checkBoxGeneticEvolution.Text = "Start";
                            comboBoxCriteria.Enabled = true;
                            numericUpDown6.Enabled = true;
                            checkBoxModelEvolutionRandom.Enabled = true;
                            textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation halted\r\n");
                        }
                    }
                    else
                    {
                        GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, (string)_agentModelGeneticQueueEntry[0] + "&" + (string)_agentModelGeneticQueueExit[0]);
                        _agentModelGeneticQueueEntry.RemoveAt(0);
                        _agentModelGeneticQueueExit.RemoveAt(0);
                        if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                        {
                            listBoxGeneticEvolutionEntry.Items.Clear();
                            listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                            listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                            listBoxGeneticEvolutionExit.Items.Clear();
                            listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                            listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                            _agentGeneticEvolutionEngine.removeTraders();
                            _agentGeneticEvolutionEngine.register(strategy);
                            backgroundWorkerGeneticEvolution.RunWorkerAsync();
                            textBoxGeneticEvolution.AppendText(DateTime.Now + " : Simulation started\r\n");
                            checkBoxGeneticEvolution.Text = "Stop";
                            comboBoxCriteria.Enabled = false;
                            numericUpDown6.Enabled = false;
                            checkBoxModelEvolutionRandom.Enabled = false;
                        }
                        else
                        {
                            if ((int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0 && (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0)
                            {
                                ArrayList entryPopulation = new ArrayList();
                                ArrayList exitPopulation = new ArrayList();
                                if (comboBoxCriteria.SelectedText != "")
                                {
                                    _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", comboBoxCriteria.SelectedText, 1);
                                    _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", comboBoxCriteria.SelectedText, 1);
                                }
                                for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                {
                                    entryPopulation.Add(_datasetModelSelectionMaxEntry.Rows[i].ItemArray[1].ToString());
                                }
                                for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                {
                                    exitPopulation.Add(_datasetModelSelectionMaxExit.Rows[i].ItemArray[1].ToString());
                                }
                                entryPopulation = SplitGenome(entryPopulation);
                                //entryPopulation = UpdateTargetDistribution(_datasetResultsOOSEntry, ref numericUpDown7, ref numericUpDown8);
                                exitPopulation = SplitGenome(exitPopulation);
                                //exitPopulation = UpdateTargetDistribution(_datasetResultsOOSExit, ref numericUpDown9, ref numericUpDown10);
                                bool duplicate = true;
                                while (duplicate == true)
                                {
                                    strategy = new GeneticStrategy(ref _pluginService, entryPopulation, exitPopulation);
                                    if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                                    {
                                        listBoxGeneticEvolutionEntry.Items.Clear();
                                        listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                                        listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                                        listBoxGeneticEvolutionExit.Items.Clear();
                                        listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                                        listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                                        duplicate = false;
                                    }
                                    _agentGeneticEvolutionEngine.removeTraders();
                                    _agentGeneticEvolutionEngine.register(strategy);
                                }
                                backgroundWorkerGeneticEvolution.RunWorkerAsync();
                                textBoxGeneticEvolution.AppendText(DateTime.Now + " : Simulation started\r\n");
                                checkBoxGeneticEvolution.Text = "Stop";
                                comboBoxCriteria.Enabled = false;
                                numericUpDown6.Enabled = false;
                                checkBoxModelEvolutionRandom.Enabled = false;
                            }
                            else
                            {
                                checkBoxGeneticEvolution.Checked = false;
                                progressBar3.Value = 0;
                                listBoxGeneticEvolutionEntry.Items.Clear();
                                listBoxGeneticEvolutionExit.Items.Clear();
                                checkBoxGeneticEvolution.Text = "Start";
                                comboBoxCriteria.Enabled = true;
                                numericUpDown6.Enabled = true;
                                checkBoxModelEvolutionRandom.Enabled = true;
                                textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation halted\r\n");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    checkBoxGeneticEvolution.Checked = false;
                    progressBar3.Value = 0;
                    listBoxGeneticEvolutionEntry.Items.Clear();
                    listBoxGeneticEvolutionExit.Items.Clear();
                    checkBoxGeneticEvolution.Text = "Start";
                    comboBoxCriteria.Enabled = true;
                    numericUpDown6.Enabled = true;
                    checkBoxModelEvolutionRandom.Enabled = true;
                    textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                }
            }
            else
            {
                backgroundWorkerGeneticEvolution.CancelAsync();
            }
        }
        private void backgroundWorkerGeneticEvolution_DoWork(object sender, DoWorkEventArgs e)
        {
            _agentGeneticEvolutionStart = DateTime.Now;
            _agentGeneticEvolutionLog = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs + Properties.Settings.Default.GlobalMarket + "-GENETIC_EVOLUTION-ENTRY-" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            _agentGeneticEvolutionMarket.random = false;
            _strategyTester.BackTest(backgroundWorkerGeneticEvolution, e, _agentGeneticEvolutionLog, _agentGeneticEvolutionEngine, _agentGeneticEvolutionMarket, ref _agentGeneticEvolutionScore, ref _datasetModelGeneticMax, ref _datasetModelGeneticMaxCache, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMax, Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxCache, ref _agentGeneticEvolutionWin, false);
        }
        private void backgroundWorkerGeneticEvolution_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar3.Value = e.ProgressPercentage;
        }
        private void backgroundWorkerGeneticEvolution_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (e.Error != null)
            {
                checkBoxGeneticEvolution.Checked = false;
                progressBar3.Value = 0;
                listBoxGeneticEvolutionEntry.Items.Clear();
                listBoxGeneticEvolutionExit.Items.Clear();
                checkBoxGeneticEvolution.Text = "Start";
                comboBoxCriteria.Enabled = true;
                numericUpDown6.Enabled = true;
                checkBoxModelEvolutionRandom.Enabled = true;
                textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation produced error: " + e.Error.Message + "\r\n");
            }
            else if (e.Cancelled)
            {
                checkBoxGeneticEvolution.Checked = false;
                progressBar3.Value = 0;
                listBoxGeneticEvolutionEntry.Items.Clear();
                listBoxGeneticEvolutionExit.Items.Clear();
                checkBoxGeneticEvolution.Text = "Start";
                comboBoxCriteria.Enabled = true;
                numericUpDown6.Enabled = true;
                checkBoxModelEvolutionRandom.Enabled = true;
                textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation cancelled\r\n");
            }
            else
            {
                textBoxGeneticEvolution.AppendText(_agentGeneticEvolutionScore);
                _agentGeneticEvolutionStop = DateTime.Now;
                double milli = _agentGeneticEvolutionStop.Subtract(_agentGeneticEvolutionStart).TotalMilliseconds;
                label8.Text = _agentGeneticEvolutionStop.ToString() + " : " + milli.ToString() + " ms = " + SupportClass.ConvertToMinutes(milli).ToString() + " min";
                if (checkBox3.Checked == true)
                    if (_agentGeneticEvolutionWin == true)
                        _soundPlayer.Play();
                _datasetModelGeneticMax = SupportClass.FilterSortDataTable(_datasetModelGeneticMax, "", "fitness", 1);
                dataGridModelGeneticMax.DataSource = _datasetModelGeneticMax;
                textBoxSelectionHistoricComboCount.Text = _datasetModelGeneticMax.Rows.Count.ToString();
                if (comboBox2.Text == "10Y" && comboBoxNaturalSelectionSampleDisposition.Text == "Combo")
                    textBoxSelectioSampleCount.Text = _datasetModelGeneticMaxCache.Rows.Count.ToString();
                try
                {
                    if (_agentModelGeneticQueueEntry.Count < 1 && _agentModelGeneticQueueExit.Count < 1)
                    {
                        if ((int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0 && (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0)
                        {
                            if (checkBoxModelEvolutionRandom.Checked == true)
                            {
                                ArrayList entryPopulation = new ArrayList();
                                ArrayList exitPopulation = new ArrayList();
                                if (comboBoxCriteria.SelectedText != "")
                                {
                                    _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", comboBoxCriteria.SelectedText, 1);
                                    _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", comboBoxCriteria.SelectedText, 1);
                                }
                                for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                {
                                    entryPopulation.Add(_datasetModelSelectionMaxEntry.Rows[i].ItemArray[1].ToString());
                                }
                                for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                {
                                    exitPopulation.Add(_datasetModelSelectionMaxExit.Rows[i].ItemArray[1].ToString());
                                }
                                entryPopulation = SplitGenome(entryPopulation);
                                //entryPopulation = UpdateTargetDistribution(_datasetResultsOOSEntry, ref numericUpDown7, ref numericUpDown8);
                                exitPopulation = SplitGenome(exitPopulation);
                                //exitPopulation = UpdateTargetDistribution(_datasetResultsOOSExit, ref numericUpDown9, ref numericUpDown10);
                                bool duplicate = true;
                                while (duplicate == true)
                                {
                                    GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, entryPopulation, exitPopulation);
                                    if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                                    {
                                        listBoxGeneticEvolutionEntry.Items.Clear();
                                        listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                                        listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                                        listBoxGeneticEvolutionExit.Items.Clear();
                                        listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                                        listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                                        duplicate = false;
                                    }
                                    _agentGeneticEvolutionEngine.removeTraders();
                                    _agentGeneticEvolutionEngine.register(strategy);
                                }
                                backgroundWorkerGeneticEvolution.RunWorkerAsync();
                                checkBoxGeneticEvolution.Text = "Stop";
                                comboBoxCriteria.Enabled = false;
                                numericUpDown6.Enabled = false;
                                checkBoxModelEvolutionRandom.Enabled = false;
                            }
                            else
                            {
                                bool duplicate = true;
                                Random rndEntry = new Random();
                                Random rndExit = new Random();
                                while (duplicate == true)
                                {
                                    int entry = rndEntry.Next((int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value) - 1);
                                    int exit = rndExit.Next((int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value) - 1);
                                    GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, _datasetModelSelectionMaxEntry.Rows[entry].ItemArray[1].ToString(), _datasetModelSelectionMaxExit.Rows[exit].ItemArray[1].ToString());
                                    if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                                    {
                                        listBoxGeneticEvolutionEntry.Items.Clear();
                                        listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                                        listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                                        listBoxGeneticEvolutionExit.Items.Clear();
                                        listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                                        listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                                        duplicate = false;
                                    }
                                    _agentGeneticEvolutionEngine.removeTraders();
                                    _agentGeneticEvolutionEngine.register(strategy);
                                }
                                backgroundWorkerGeneticEvolution.RunWorkerAsync();
                                checkBoxGeneticEvolution.Text = "Stop";
                                comboBoxCriteria.Enabled = false;
                                numericUpDown6.Enabled = false;
                                checkBoxModelEvolutionRandom.Enabled = false;
                            }
                        }
                        else
                        {
                            checkBoxGeneticEvolution.Checked = false;
                            progressBar3.Value = 0;
                            listBoxGeneticEvolutionEntry.Items.Clear();
                            listBoxGeneticEvolutionExit.Items.Clear();
                            checkBoxGeneticEvolution.Text = "Start";
                            comboBoxCriteria.Enabled = true;
                            numericUpDown6.Enabled = true;
                            checkBoxModelEvolutionRandom.Enabled = true;
                            textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation halted\r\n");
                        }
                    }
                    else
                    {
                        GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, (string)_agentModelGeneticQueueEntry[0] + "&" + (string)_agentModelGeneticQueueExit[0]);
                        _agentModelGeneticQueueEntry.RemoveAt(0);
                        _agentModelGeneticQueueExit.RemoveAt(0);
                        if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                        {
                            listBoxGeneticEvolutionEntry.Items.Clear();
                            listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                            listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                            listBoxGeneticEvolutionExit.Items.Clear();
                            listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                            listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                            _agentGeneticEvolutionEngine.removeTraders();
                            _agentGeneticEvolutionEngine.register(strategy);
                            backgroundWorkerGeneticEvolution.RunWorkerAsync();
                            checkBoxGeneticEvolution.Text = "Stop";
                            comboBoxCriteria.Enabled = false;
                            numericUpDown6.Enabled = false;
                            checkBoxModelEvolutionRandom.Enabled = false;
                        }
                        else
                        {
                            if (_agentModelGeneticQueueEntry.Count < 1 && _agentModelGeneticQueueExit.Count < 1)
                            {
                                if ((int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0 && (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value) > 0)
                                {
                                    ArrayList entryPopulation = new ArrayList();
                                    ArrayList exitPopulation = new ArrayList();
                                    if (comboBoxCriteria.SelectedText != "")
                                    {
                                        _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", comboBoxCriteria.SelectedText, 1);
                                        _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", comboBoxCriteria.SelectedText, 1);
                                    }
                                    for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxEntry.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                    {
                                        entryPopulation.Add(_datasetModelSelectionMaxEntry.Rows[i].ItemArray[1].ToString());
                                    }
                                    for (int i = 0; i < (int)((double)(((double)_datasetModelSelectionMaxExit.Rows.Count / (double)100)) * (double)numericUpDown6.Value); i++)
                                    {
                                        exitPopulation.Add(_datasetModelSelectionMaxExit.Rows[i].ItemArray[1].ToString());
                                    }
                                    entryPopulation = SplitGenome(entryPopulation);
                                    //entryPopulation = UpdateTargetDistribution(_datasetResultsOOSEntry, ref numericUpDown7, ref numericUpDown8);
                                    exitPopulation = SplitGenome(exitPopulation);
                                    //exitPopulation = UpdateTargetDistribution(_datasetResultsOOSExit, ref numericUpDown9, ref numericUpDown10);
                                    bool duplicate = true;
                                    while (duplicate == true)
                                    {
                                        strategy = new GeneticStrategy(ref _pluginService, entryPopulation, exitPopulation);
                                        if (!_datasetModelGeneticMax.Rows.Contains(strategy.Genome))
                                        {
                                            listBoxGeneticEvolutionEntry.Items.Clear();
                                            listBoxGeneticEvolutionEntry.ForeColor = Color.LimeGreen;
                                            listBoxGeneticEvolutionEntry.Items.Add(strategy.EntryIndicatorName);
                                            listBoxGeneticEvolutionExit.Items.Clear();
                                            listBoxGeneticEvolutionExit.ForeColor = Color.Red;
                                            listBoxGeneticEvolutionExit.Items.Add(strategy.ExitIndicatorName);
                                            duplicate = false;
                                        }
                                        _agentGeneticEvolutionEngine.removeTraders();
                                        _agentGeneticEvolutionEngine.register(strategy);
                                    }
                                    backgroundWorkerGeneticEvolution.RunWorkerAsync();
                                    checkBoxGeneticEvolution.Text = "Stop";
                                    comboBoxCriteria.Enabled = false;
                                    numericUpDown6.Enabled = false;
                                    checkBoxModelEvolutionRandom.Enabled = false;
                                }
                                else
                                {
                                    checkBoxGeneticEvolution.Checked = false;
                                    progressBar3.Value = 0;
                                    listBoxGeneticEvolutionEntry.Items.Clear();
                                    listBoxGeneticEvolutionExit.Items.Clear();
                                    checkBoxGeneticEvolution.Text = "Start";
                                    comboBoxCriteria.Enabled = true;
                                    numericUpDown6.Enabled = true;
                                    checkBoxModelEvolutionRandom.Enabled = true;
                                    textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation halted\r\n");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    checkBoxGeneticEvolution.Checked = false;
                    progressBar3.Value = 0;
                    listBoxGeneticEvolutionEntry.Items.Clear();
                    listBoxGeneticEvolutionExit.Items.Clear();
                    checkBoxGeneticEvolution.Text = "Start";
                    comboBoxCriteria.Enabled = true;
                    numericUpDown6.Enabled = true;
                    checkBoxModelEvolutionRandom.Enabled = true;
                    textBoxGeneticEvolution.AppendText(DateTime.Now + " : Operation produced error: " + ex.Message + "\r\n");
                }
            }

            /*
                try
                {
                    _datasetResultsEntry = SupportClass.FilterSortDataTable(_datasetResultsEntry, "", "fitness", 1);
                    dataGridResultsEntry.DataSource = _datasetResultsEntry;
                    UpdateTargetDistribution();
                    if (_datasetResultsEntryTarget.Rows.Count >= 5)
                    {
                        ArrayList population = new ArrayList();
                        for (int i = 0; i < _datasetResultsEntryTarget.Rows.Count; i++)
                        {
                            population.Add(_datasetResultsEntryTarget.Rows[i].ItemArray[1].ToString());
                        }
                        population = SplitGenome(population);
                        GeneticStrategy strategy = new GeneticStrategy(population);
                        _agentGeneticEvolutionEngine.register(strategy);
                        listBoxGeneticEvolution.Items.Clear();
                        listBoxGeneticEvolution.Items.Add(strategy.EntryIndicatorName);
                        _agentGeneticEvolutionStop = DateTime.Now;
                        label5.Text = _agentGeneticEvolutionStop.ToString(_dateFormat) + " : " + _agentGeneticEvolutionStop.Subtract(_agentGeneticEvolutionStart).TotalMilliseconds.ToString() + "ms";
                        backgroundWorkerGeneticEvolution.RunWorkerAsync();
                    }
                    else
                    {
                        textBoxGeneticEvolution.AppendText(DateTime.Now + " : Population too small\r\n");
                        textBoxGeneticEvolution.AppendText(DateTime.Now + " : Simulation stopped\r\n");
                        checkBoxGeneticEvolution.Checked = false;
                    }
                }
                catch (Exception ex)
                {
                    checkBoxGeneticEvolution.Checked = false;
                    MessageBox.Show(ex.ToString());
                }
            }
            /*
            if (_datasetNaturalEntries.Rows.Count > 0)
                if (double.Parse(_datasetNaturalEntries.Rows[0].ItemArray[0].ToString()) > double.Parse(_datasetResults.Rows[_datasetResults.Rows.Count - 1].ItemArray[0].ToString()))
                {
                    DataRow newrow = _datasetResults.NewRow();
                    newrow["fitness"] = _datasetNaturalEntries.Rows[0].ItemArray[0];
                    newrow["strategy"] = _datasetNaturalEntries.Rows[0].ItemArray[1];
                    newrow["profit"] = _datasetNaturalEntries.Rows[0].ItemArray[2];
                    newrow["transactions"] = _datasetNaturalEntries.Rows[0].ItemArray[3];
                    newrow["bars"] = _datasetNaturalEntries.Rows[0].ItemArray[4];
                    _datasetResults.Rows.Add(newrow);
                    _datasetNaturalEntries.Rows[0].Delete();
                }
             */
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //_currentStrategyDistributionTrain = textBox2.Text;
        }

        private void backgroundWorkerRecommendationsSell_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            _instrumentRecommendationsSellSymbolA = _marketRecommendationsSellA._instruments[_counterRecommendationsSellA].ToString();
            _instrumentRecommendationsSellNameA = _marketRecommendationsSellA._instrumentNames[_counterRecommendationsSellA].ToString();
            _strategyTester.BackTest(_engineRecommendationsSellA, _marketRecommendationsSellA, ref _instrumentRecommendationsSellSymbolA);
        }
        private void backgroundWorkerRecommendationsSell_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GeneticStrategy trader = (GeneticStrategy)_engineRecommendationsSellA.Trader.Agents[0];
            textBoxRecommendationsSell.AppendText(DateTime.Now + " : " + _instrumentRecommendationsSellSymbolA + " : " + _instrumentRecommendationsSellNameA + " : " + trader.EntryIndicator.GetDirection() + " : " + trader.ExitIndicator.GetDirection() + "\r\n");
            if (trader.EntryIndicator.GetDirection() <= 0 && trader.ExitIndicator.GetDirection() == -1)
            {
                try
                {
                    DataRow newrow = _datasetRecommendationsSellConsensusA.NewRow();
                    newrow["instrument"] = _instrumentRecommendationsSellSymbolA;
                    newrow["name"] = _instrumentRecommendationsSellNameA;
                    newrow["date"] = ((TechnicalData)(trader.Market.TechnicalData[trader.Market.TechnicalData.Count - 1])).Date;
                    _datasetRecommendationsSellConsensusA.Rows.Add(newrow);
                }
                catch (Exception ex)
                {
                    textBoxRecommendationsSell.AppendText(DateTime.Now + " : " + ex.ToString() + " \r\n");
                }
            }
            if (_counterRecommendationsSellA < _marketRecommendationsSellA._instruments.Count - 1)
            {
                _counterRecommendationsSellA++;
                backgroundWorkerRecommendationsSellA.RunWorkerAsync();
            }
            else
            {
                if (_counterRecommendationsSellStrategyRankA == 0)
                    _sellRecommendationsPercentageOfMarketA = _datasetRecommendationsSellConsensusA.Rows.Count;
                if ((_counterRecommendationsSellStrategyRankA < (numericUpDownTradeIndicatorsSell.Value - 1)) && (_counterRecommendationsSellStrategyRankA < (_datasetModelGenetic1Month.Rows.Count - 1)) && (_datasetRecommendationsSellConsensusA.Rows.Count > 0))
                {
                    _currentRecommendationsSellInstrumentsA.Clear();
                    for (int x = 0; x < _datasetRecommendationsSellConsensusA.Rows.Count; x++)
                    {
                        _currentRecommendationsSellInstrumentsA.Add(_datasetRecommendationsSellConsensusA.Rows[x].ItemArray[0].ToString(), _datasetRecommendationsSellConsensusA.Rows[x].ItemArray[1].ToString());
                    }
                    _marketRecommendationsSellA.Set(_currentRecommendationsSellInstrumentsA);
                    _counterRecommendationsSellA = 0;
                    _counterRecommendationsSellStrategyRankA++;
                    _strategyRecommendationsSellStrategyNameA = _datasetModelGenetic1Month.Rows[_counterRecommendationsSellStrategyRankA].ItemArray[1].ToString();
                    textBoxRecommendationsSellStrategyName.Text = _strategyRecommendationsSellStrategyNameA;
                    textBoxRecommendationsSellStrategyRank.Text = _counterRecommendationsSellStrategyRankA.ToString();
                    trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsSellStrategyNameA);
                    _engineRecommendationsSellA.removeTraders();
                    _engineRecommendationsSellA.register(trader);
                    _datasetRecommendationsSellConsensusA.Rows.Clear();
                    textBoxRecommendationsSell.AppendText(DateTime.Now + " : ---\r\n");
                    backgroundWorkerRecommendationsSellA.RunWorkerAsync();
                }
                else
                {
                    for (int x = 0; x < _datasetRecommendationsSellConsensusA.Rows.Count; x++)
                    {
                        DataRow newrow = _datasetRecommendationsSellA.NewRow();
                        newrow["symbol"] = _datasetRecommendationsSellConsensusA.Rows[x].ItemArray[0].ToString();
                        newrow["name"] = _datasetRecommendationsSellConsensusA.Rows[x].ItemArray[1].ToString();
                        newrow["date"] = DateTime.Parse(_datasetRecommendationsSellConsensusA.Rows[x].ItemArray[2].ToString());
                        _datasetRecommendationsSellA.Rows.Add(newrow);
                    }
                    _datasetRecommendationsSellA = SupportClass.FilterSortDataTable(_datasetRecommendationsSellA, "", "date", 1);
                    dataGridRecommendationsSell.DataSource = _datasetRecommendationsSellA;
                    //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                    //{
                    textBoxRecommendationsSell.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsSellA + "\r\n");
                    try
                    {
                        FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellA, FileMode.Create, FileAccess.Write);
                        StreamWriter s = new StreamWriter(fs);
                        _datasetRecommendationsSellA.WriteXml(s);
                        s.Close();
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //}
                    textBoxRecommendationsSell.AppendText(DateTime.Now + " : " + _sellRecommendationsPercentageOfMarketA.ToString() + "\r\n");
                    textBoxRecommendationsSell.AppendText(DateTime.Now + " : Simulation completed\r\n");
                    buttonRecommendationsSell.Enabled = true;
                }
            }
        }

        private void backgroundWorkerRecommendationsBuy_DoWork(object sender, DoWorkEventArgs e)
        {
            _instrumentRecommendationsBuySymbolA = _marketRecommendationsBuyA._instruments[_counterRecommendationsBuyA].ToString();
            _instrumentRecommendationsBuyNameA = _marketRecommendationsBuyA._instrumentNames[_counterRecommendationsBuyA].ToString();
            _strategyTester.BackTest(_engineRecommendationsBuyA, _marketRecommendationsBuyA, ref _instrumentRecommendationsBuySymbolA);
        }

        private void backgroundWorkerRecommendationsBuy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            double minimum = (Properties.Settings.Default.BrokerCommision * 2) * 100;  //arbitrary BS
            GeneticStrategy trader = (GeneticStrategy)_engineRecommendationsBuyA.Trader.Agents[0];
            textBoxRecommendationsBuy.AppendText(DateTime.Now + " : " + _instrumentRecommendationsBuySymbolA + " : " + _instrumentRecommendationsBuyNameA + " : " + trader.EntryIndicator.GetDirection() + " : " + trader.ExitIndicator.GetDirection() + "\r\n");
            if (trader.EntryIndicator.GetDirection() == 1 && trader.ExitIndicator.GetDirection() >= 0)
            {
                DataRow newrow = _datasetRecommendationsBuyConsensusA.NewRow();
                newrow["instrument"] = _instrumentRecommendationsBuySymbolA;
                newrow["name"] = _instrumentRecommendationsBuyNameA;
                newrow["date"] = ((TechnicalData)(trader.Market.TechnicalData[trader.Market.TechnicalData.Count - 1])).Date;
                _datasetRecommendationsBuyConsensusA.Rows.Add(newrow);
            }
            if (_counterRecommendationsBuyA < _marketRecommendationsBuyA._instruments.Count - 1)
            {
                _counterRecommendationsBuyA++;
                backgroundWorkerRecommendationsBuyA.RunWorkerAsync();
            }
            else
            {
                if (_counterRecommendationsBuyStrategyRankA == 0)
                    _buyRecommendationsPercentageOfMarketA = _datasetRecommendationsBuyConsensusA.Rows.Count;
                double cash = (double.Parse(_datasetPortfolioA.Tables["account"].Rows[0].ItemArray[1].ToString()) - (Properties.Settings.Default.BrokerCommision * _datasetRecommendationsBuyConsensusA.Rows.Count)) / _datasetRecommendationsBuyConsensusA.Rows.Count;
                if ((_counterRecommendationsBuyStrategyRankA < (numericUpDownTradeIndicatorsBuy.Value - 1)) && (_counterRecommendationsBuyStrategyRankA < (_datasetModelGenetic1Month.Rows.Count - 1)) && (_datasetRecommendationsBuyConsensusA.Rows.Count > 0) && (cash < minimum))
                {
                    _currentRecommendationsBuyInstrumentsA.Clear();
                    for (int x = 0; x < _datasetRecommendationsBuyConsensusA.Rows.Count; x++)
                    {
                        _currentRecommendationsBuyInstrumentsA.Add(_datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[0].ToString(), _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[1].ToString());
                    }
                    _marketRecommendationsBuyA.Set(_currentRecommendationsBuyInstrumentsA);
                    _counterRecommendationsBuyA = 0;
                    _counterRecommendationsBuyStrategyRankA++;
                    _strategyRecommendationsBuyStrategyNameA = _datasetModelGenetic1Month.Rows[_counterRecommendationsBuyStrategyRankA].ItemArray[1].ToString();
                    textBoxRecommendationsBuyStrategyName.Text = _strategyRecommendationsBuyStrategyNameA;
                    textBoxRecommendationsBuyStrategyRank.Text = _counterRecommendationsBuyStrategyRankA.ToString();
                    trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsBuyStrategyNameA);
                    _engineRecommendationsBuyA.removeTraders();
                    _engineRecommendationsBuyA.register(trader);
                    _datasetRecommendationsBuyConsensusA.Rows.Clear();
                    textBoxRecommendationsBuy.AppendText(DateTime.Now + " : ---\r\n");
                    backgroundWorkerRecommendationsBuyA.RunWorkerAsync();
                }
                else
                {
                    for (int x = 0; x < _datasetRecommendationsBuyConsensusA.Rows.Count; x++)
                    {
                        textBoxRecommendationsBuy.AppendText(DateTime.Now + " : : " + _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[0].ToString() + "\r\n");
                        for (int y = 0; y < _datasetRisk.Rows.Count; y++)
                        {
                            if (_datasetRisk.Rows[y].ItemArray[0].ToString() == _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[0].ToString())
                            {
                                if (checkBox6.Checked == true)
                                {
                                    if ((double)_datasetRisk.Rows[y].ItemArray[4] <= 1 && (double)_datasetModelGenetic1Month.Rows[0].ItemArray[2] >= 0)
                                    {
                                        DataRow newrow = _datasetRecommendationsBuyA.NewRow();
                                        newrow["symbol"] = _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[0].ToString();
                                        newrow["name"] = _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[1].ToString();
                                        newrow["date"] = DateTime.Parse(_datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[2].ToString());
                                        newrow["return"] = _datasetRisk.Rows[y].ItemArray[14];
                                        _datasetRecommendationsBuyA.Rows.Add(newrow);
                                    }
                                    else
                                    {
                                        textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Constraining: " + _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[0].ToString() + "\r\n");
                                    }
                                }
                                else
                                {
                                    DataRow newrow = _datasetRecommendationsBuyA.NewRow();
                                    newrow["symbol"] = _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[0].ToString();
                                    newrow["name"] = _datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[1].ToString();
                                    newrow["date"] = DateTime.Parse(_datasetRecommendationsBuyConsensusA.Rows[x].ItemArray[2].ToString());
                                    newrow["return"] = _datasetRisk.Rows[y].ItemArray[14];
                                    _datasetRecommendationsBuyA.Rows.Add(newrow);
                                }
                            }
                        }
                    }
                    _datasetRecommendationsBuyA = SupportClass.FilterSortDataTable(_datasetRecommendationsBuyA, "", "return", 1);
                    dataGridRecommendationsBuy.DataSource = _datasetRecommendationsBuyA;
                    //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                    //{
                    textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsBuyA + "\r\n");
                    try
                    {
                        FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyA, FileMode.Create, FileAccess.Write);
                        StreamWriter s = new StreamWriter(fs);
                        _datasetRecommendationsBuyA.WriteXml(s);
                        s.Close();
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //}
                    textBoxRecommendationsBuy.AppendText(DateTime.Now + " : " + _buyRecommendationsPercentageOfMarketA.ToString() + "\r\n");
                    textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Simulation completed\r\n");
                    buttonRecommendationsBuy.Enabled = true;
                }
            }
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void PortfolioDataGrid_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int _dataRowIndexForContextMenu;
            Point pt = new Point(e.X, e.Y);
            DataGrid.HitTestInfo hti = PortfolioDataGrid.HitTest(pt);
            if (e.Button == MouseButtons.Right)
            {
                if (hti.Type == DataGrid.HitTestType.Cell)
                {
                    ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
                    PortfolioDataGrid.ContextMenuStrip = contextMenuStrip1;
                    contextMenuStrip1.Items.Add(_portfolioA.GetSymbolByRow(hti.Row) + " : " + _portfolioA.GetNameByRow(hti.Row));
                    _dataRowIndexForContextMenu = hti.Row;
                }
                else
                {
                    PortfolioDataGrid.ContextMenuStrip = null;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProcessRecommendationsA();
            ProcessRecommendationsB();
        }

        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {
            _portfolioA.Update();
            _portfolioB.Update();
        }

        private void backgroundWorker5_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdatePortfolioDisplayA();
            UpdatePortfolioDisplayB();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new FormAbout();
            form.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void backgroundWorkerMarketAnalysis_DoWork(object sender, DoWorkEventArgs e)
        {
            _instr = new Instrument(_marketRisk.GetTechnicalData(_marketRisk._instruments[_counterAnalysis].ToString()), _marketRisk._instruments[_counterAnalysis].ToString());
            _instrumentAnalysis = _marketRisk._instruments[_counterAnalysis].ToString();
            _instrumentNameAnalysis = _marketRisk._instrumentNames[_counterAnalysis].ToString();
            _strategyTester.BackTest(_engineRisk, _marketRisk, ref _instrumentAnalysis, ref _datasetRisk);
        }

        private void backgroundWorkerMarketAnalysis_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string temp = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + Properties.Settings.Default.GlobalMarket + "\\" + _instrumentAnalysis + "-industry.txt";
            StreamReader re = File.OpenText(temp);
            string sector = re.ReadLine();
            string industry = re.ReadLine();
            re.Close();
            temp = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + Properties.Settings.Default.GlobalMarket + "\\" + _instrumentAnalysis + "-fundamentals.txt";
            re = File.OpenText(temp);
            re.ReadLine();
            re.ReadLine();
            re.ReadLine();
            string pe = re.ReadLine();
            string peg = re.ReadLine();
            string ps = re.ReadLine();
            string pb = re.ReadLine();
            re.ReadLine();
            re.ReadLine();
            string be = re.ReadLine();
            re.Close();
            AbstractStrategy abstracttrader = (AbstractStrategy)_engineRisk.Trader.Agents[0];
            Statistics stats = _observerRisk.GetStatistics();
            textBox21.AppendText(DateTime.Now + " : " + _instrumentAnalysis + " : " + _observerRisk.GetStartDate().ToString() + " - " + _observerRisk.GetEndDate().ToString() + " : D " + stats.getDownPosDays().ToString() + " U " + stats.getUpPosDays().ToString() + " T " + stats.getDays().ToString() + " : Risk: " + stats.getMeanRisk().ToString() + " : PnL: " + abstracttrader.GetCumulativePnL().ToString() + "\r\n");
            DataRow newrow = _datasetRisk.NewRow();
            newrow["instrument"] = _instrumentAnalysis;
            newrow["name"] = _instrumentNameAnalysis;
            newrow["sector"] = sector;
            newrow["industry"] = industry;
            if (peg == "" || peg == "N/A")
                newrow["peg_ratio"] = 999;
            else
                newrow["peg_ratio"] = double.Parse(peg);
            newrow["price_earnings"] = pe;
            newrow["price_book"] = pb;
            newrow["price_sales"] = ps;
            newrow["beta"] = be;
            newrow["start"] = _observerRisk.GetStartDate();
            newrow["end"] = _observerRisk.GetEndDate();
            newrow["up"] = stats.getUpPosDays();
            newrow["down"] = stats.getDownPosDays();
            newrow["total"] = stats.getDays();
            newrow["return"] = (100 * ((abstracttrader.GetCumulativePnL() - abstracttrader.GetInitialBalance()) / abstracttrader.GetInitialBalance())) / (_observerRisk.GetEndDate().Subtract(_observerRisk.GetStartDate()).Days / 365);
            _datasetRisk.Rows.Add(newrow);
            _datasetRisk = SupportClass.FilterSortDataTable(_datasetRisk, "", "return", 1);
            dataGridViewAnalyze.DataSource = _datasetRisk;

            /*
            double[] temp = YearPeriodHistogram(_instr);
            for (int x = 1; x <= 366; x++)
            {
                _marketHistogramCounter[x]++;
                _marketHistogram[x] += temp[x];
                _marketHistogramAverage[x] = _marketHistogram[x] / _marketHistogramCounter[x];
                System.Windows.Forms.Application.DoEvents();
            }

            double[] temp1 = MonthPeriodHistogram(_instr);
            for (int x = 1; x <= 31; x++)
            {
                _marketMonthHistogramCounter[x]++;
                _marketMonthHistogram[x] += temp1[x];
                _marketMonthHistogramAverage[x] = _marketMonthHistogram[x] / _marketMonthHistogramCounter[x];
                System.Windows.Forms.Application.DoEvents();
            }

            double[] temp2 = LunarPhaseHistogram(_instr);
            for (int x2 = 0; x2 < 8; x2++)
            {
                _marketLunarHistogramCounter[x2]++;
                _marketLunarHistogram[x2] += temp2[x2];
                _marketLunarHistogramAverage[x2] = _marketLunarHistogram[x2] / _marketLunarHistogramCounter[x2];
                System.Windows.Forms.Application.DoEvents();
            }

            double[] temp3 = WeekPeriodHistogram(_instr);
            for (int x = 0; x <= 6; x++)
            {
                _marketWeekHistogramCounter[x]++;
                _marketWeekHistogram[x] += temp3[x];
                _marketWeekHistogramAverage[x] = _marketWeekHistogram[x] / _marketWeekHistogramCounter[x];
                System.Windows.Forms.Application.DoEvents();
            }
             */

            if (_counterAnalysis < _marketRisk._instruments.Count - 1)
            {
                _counterAnalysis++;
                backgroundWorkerMarketAnalysis.RunWorkerAsync();
            }
            else
            {
                /*
                CreateSolarChart(histogram);
                histogram.Invalidate();
                CreateMonthChart(zedGraphControl8);
                zedGraphControl8.Invalidate();
                CreateLunarChart(zedGraphControl7);
                zedGraphControl7.Invalidate();
                CreateWeekChart(zedGraphControl9);
                zedGraphControl9.Invalidate();
                 */
                _datasetRisk.WriteXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileStageRiskReward);
                if (checkBoxAnalyzeReport.Checked == true)
                {
                    try
                    {
                        FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryReports + _reportRiskReward, FileMode.Create, FileAccess.Write);
                        StreamWriter s = new StreamWriter(fs);
                        for (int i = 0; i < _datasetRisk.Rows.Count; i++)
                        {
                            s.Write(_datasetRisk.Rows[i].ItemArray[0].ToString() + ", " + _datasetRisk.Rows[i].ItemArray[1].ToString() + ", " + _datasetRisk.Rows[i].ItemArray[2].ToString() + ", " + _datasetRisk.Rows[i].ItemArray[3].ToString() + ", " + _datasetRisk.Rows[i].ItemArray[4].ToString() + ", " + _datasetRisk.Rows[i].ItemArray[5].ToString() + ", " + _datasetRisk.Rows[i].ItemArray[6].ToString() + ", " + _datasetRisk.Rows[i].ItemArray[7].ToString() + _datasetRisk.Rows[i].ItemArray[8].ToString() + "\r\n");
                        }
                        s.Close();
                        /*
                        FileStream fs2 = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryReports + comboBoxMarket.Text + "-Missing_Dates.txt", FileMode.Create, FileAccess.Write);
                        StreamWriter s2 = new StreamWriter(fs2);
                        _marketDateExceptions2.Sort();
                        for (int i = 0; i < _marketDateExceptions.Count; i++)
                        {
                            s2.Write(_marketDateExceptions[i].ToString() + "\r\n");
                        }
                        s2.Close();
                         */
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                textBox21.AppendText(DateTime.Now + " : Simulation completed\r\n");
                buttonAnalyzeRun.Enabled = true;
            }
        }

        private void buttonAnalyzeRun_Click(object sender, EventArgs e)
        {
            UpdateMarketAnalysis();
        }

        private void dateTimePickerAnalyzeEarliest_ValueChanged(object sender, EventArgs e)
        {
            _startDateAnalyze = dateTimePickerAnalyzeEarliest.Value;
            if (_engineRisk != null)
                _engineRisk.StartDate = _startDateAnalyze;
        }
        private void dateTimePickerAnalyzeLatest_ValueChanged(object sender, EventArgs e)
        {
            _endDateAnalyze = dateTimePickerAnalyzeLatest.Value;
            if (_engineRisk != null)
                _engineRisk.EndDate = _endDateAnalyze;
        }
        private void buttonNew_Click(object sender, EventArgs e)
        {
            bp.Initialize(int.Parse(textBoxInput.Text), int.Parse(textBoxHidden.Text), int.Parse(textBoxOutput.Text), true);
            textBox23.AppendText(DateTime.Now + " : Neural Network created - (" + bp._inputNeurons.ToString() + ", " + bp._hiddenNeurons.ToString() + ", " + bp._outputNeurons.ToString() + ", " + bp._stateNeurons.ToString() + ")\r\n");
            DisplayNeuralNetwork();
        }
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = ".nn";
            openFileDialog1.InitialDirectory = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData;
            openFileDialog1.ShowDialog();
            if (bp.Load(openFileDialog1.FileName))
            {
                textBox23.AppendText(DateTime.Now + " : Neural Network loaded - (" + bp._inputNeurons.ToString() + ", " + bp._hiddenNeurons.ToString() + ", " + bp._outputNeurons.ToString() + ", " + bp._stateNeurons.ToString() + ")\r\n");
                DisplayNeuralNetwork();
            }
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = ".nn";
            saveFileDialog1.InitialDirectory = Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData;
            saveFileDialog1.ShowDialog();
            if (bp.Save(saveFileDialog1.FileName))
            {
                textBox23.AppendText(DateTime.Now + " : Neural Network saved - (" + bp._inputNeurons.ToString() + ", " + bp._hiddenNeurons.ToString() + ", " + bp._outputNeurons.ToString() + ", " + bp._stateNeurons.ToString() + ")\r\n");
            }
        }

        private void buttonTrain_Click(object sender, EventArgs e)
        {
            _isFinished = false;
            buttonTrain.Enabled = false;
            _marketNeural = new Market();
            _marketNeural.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, false);
            _counterNeural = 0;
            backgroundWorkerNeural.RunWorkerAsync();
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            _isFinished = true;
            buttonTrain.Enabled = true;
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorkerNeural_DoWork(object sender, DoWorkEventArgs e)
        {
            trainingSet.ImportTimeSeries(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + Properties.Settings.Default.GlobalMarket + "\\" + _marketNeural._instruments[_counterNeural].ToString() + ".csv", 6, 0);
            bp.ClearStateUnits();
            bp.ClearResultGraph();
            Train(backgroundWorkerNeural, e);
        }

        private void backgroundWorkerNeural_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //txtBPerror.Text = System.Convert.ToString((int)(bp.BPerror * 10000000) / 10000000);
            textBoxError.Text = bp.BPerror.ToString();
            textBoxIterations.Text = System.Convert.ToString(bp.TrainingItterations);
            System.Windows.Forms.Application.DoEvents();
            Bitmap offScreenBmp = new Bitmap(pictureBoxGraph.Width, pictureBoxGraph.Height);
            for (int j = 0; j < bp._inputNeurons; j++)
            {
                showOutputGraph(ref offScreenBmp, j);
            }
            showOutputGraph(ref offScreenBmp, 2);
            pictureBoxGraph.Image = offScreenBmp;
            textBox23.AppendText(DateTime.Now + " : Training Set completed - " + _marketNeural._instruments[_counterNeural].ToString() + " (" + trainingSet.NoOfInstances.ToString() + ")\r\n");
            //this.progressBar4.Value = _counterNeural * (int)(100 / _marketNeural._instruments.Count);
            if (_counterNeural < _marketNeural._instruments.Count - 1)
            {
                _counterNeural++;
                backgroundWorkerNeural.RunWorkerAsync();
            }
            else
                DisplayNeuralNetwork();
        }

        private void backgroundWorkerNeural_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar4.Value = e.ProgressPercentage;
        }

        private void cmdPredict_Click_2(object sender, EventArgs e)
        {/*
            Bitmap offScreenBmp = new Bitmap(pictureBoxGraph.Width, pictureBoxGraph.Height);
            //testSet.ImportTimeSeries(_directoryMarket + "\\NASDAQ\\" + textBox5.Text + ".csv", (bp._inputNeurons), 1);
            bp.ClearStateUnits();
            bp.ClearResultGraph();
            for (int i = 0; i < testSet.NoOfInstances; i++)
            {
                bp.loadTrainingInstance(testSet.getInstance(i));
                bp.setRealValuesFromTrainingSet(testSet);
                bp.update();
                bp.storeOutputs();
            }
            //bp.ShowGrid(int.Parse(textBox9.Text), ref dataGrid7);
            for (int i = 0; i < bp._inputNeurons; i++)
            {
                //if ((bp.getShowDimension(i)))
                // {
                bp.showOutputGraph(ref offScreenBmp, i);
                // }
            }
            pictureBoxGraph.Image = offScreenBmp;
          */
        }

        private void button13_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox23.Text = openFileDialog1.FileName;
            trainingSet.ImportTimeSeries(textBox23.Text, 6, 0);
        }

        private void schedulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new FormSchedules();
            form.Show();
        }

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new FormPlugins();
            form.Show();
        }

        private void dateTimePickerForecastEarliest_ValueChanged(object sender, EventArgs e)
        {
            _startDateForecastA = dateTimePickerForecastEarliest.Value;
            //if (_engine != null)
            //    _engine.StartDate = _startDateForecast;
        }

        private void dateTimePickerForecastLatest_ValueChanged(object sender, EventArgs e)
        {
            _endDateForecastA = dateTimePickerForecastLatest.Value;
            //if (_engine != null)
            //    _engine.EndDate = _endDateForecast;
        }

        private void checkBoxUpdateLive_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUpdateLive.Checked == true)
            {
                backgroundWorkerUpdatePortfolio.RunWorkerAsync();
                timer1.Start();
                textBox18.AppendText(DateTime.Now + " : Live updating started\r\n");
            }
            else
            {
                timer1.Stop();
                textBox18.AppendText(DateTime.Now + " : Live updating stopped\r\n");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateBaseLineFile();
        }

        private void performanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            performanceToolStripMenuItem.Enabled = false;
            System.Threading.Thread myThread;
            myThread = new System.Threading.Thread(new System.Threading.ThreadStart(UpdateHistoricCharts));
            myThread.Start();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new ZedGraphChart();
            form.Show();
        }

        private void throttleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopAgents();
        }
        #endregion
        #region Initialization
        private void LoadIndexes()
        {
            InitializeTable(ref _datasetModelSelectionMaxEntryQueue);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryQueue))
            {
                try
                {
                    _datasetModelSelectionMaxEntryQueue.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryQueue);
                    textBox7.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                    _datasetModelSelectionMaxEntryQueue = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntryQueue, "", "fitness", 1);
                    if (_datasetModelSelectionMaxEntryQueue.Rows.Count > (int)numericUpDown5.Value)
                    {
                        while (_datasetModelSelectionMaxEntryQueue.Rows.Count > (int)numericUpDown5.Value)
                        {
                            _datasetModelSelectionMaxEntryQueue.Rows.RemoveAt(_datasetModelSelectionMaxEntryQueue.Rows.Count - 1);
                            textBox7.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelectionMaxExitQueue);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitQueue))
            {
                try
                {
                    _datasetModelSelectionMaxExitQueue.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitQueue);
                    textBox8.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                    _datasetModelSelectionMaxExitQueue = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExitQueue, "", "fitness", 1);
                    if (_datasetModelSelectionMaxExitQueue.Rows.Count > (int)numericUpDown5.Value)
                    {
                        while (_datasetModelSelectionMaxExitQueue.Rows.Count > (int)numericUpDown5.Value)
                        {
                            _datasetModelSelectionMaxExitQueue.Rows.RemoveAt(_datasetModelSelectionMaxExitQueue.Rows.Count - 1);
                            textBox8.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelSelectionMaxEntry);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntry))
            {
                try
                {
                    _datasetModelSelectionMaxEntry.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntry);
                    _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelSelectionMaxEntry.DataSource = _datasetModelSelectionMaxEntry;
            textBoxSelectionHistoricEntryCount.Text = _datasetModelSelectionMaxEntry.Rows.Count.ToString();
            textBoxSelectionHistoricEntryQueueCount.Text = _datasetModelSelectionMaxEntryQueue.Rows.Count.ToString();
            InitializeTable(ref _datasetModelSelectionMaxEntryCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryCache))
            {
                try
                {
                    _datasetModelSelectionMaxEntryCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxEntryCache);
                    _datasetModelSelectionMaxEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntryCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelSelectionMaxExit);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExit))
            {
                try
                {
                    _datasetModelSelectionMaxExit.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExit);
                    _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelSelectionMaxExit.DataSource = _datasetModelSelectionMaxExit;
            textBoxSelectionHistoricExitCount.Text = _datasetModelSelectionMaxExit.Rows.Count.ToString();
            textBoxSelectionHistoricExitQueueCount.Text = _datasetModelSelectionMaxExitQueue.Rows.Count.ToString();
            InitializeTable(ref _datasetModelSelectionMaxExitCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitCache))
            {
                try
                {
                    _datasetModelSelectionMaxExitCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelectionMaxExitCache);
                    _datasetModelSelectionMaxExitCache = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExitCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelSelection10YearEntry);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearEntry))
            {
                try
                {
                    _datasetModelSelection10YearEntry.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearEntry);
                    _datasetModelSelection10YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection10YearEntry, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection10YearEntryCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearEntryCache))
            {
                try
                {
                    _datasetModelSelection10YearEntryCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearEntryCache);
                    _datasetModelSelection10YearEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelection10YearEntryCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            InitializeTable(ref _datasetModelSelection10YearExit);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearExit))
            {
                try
                {
                    _datasetModelSelection10YearExit.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearExit);
                    _datasetModelSelection10YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection10YearExit, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection10YearExitCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearExitCache))
            {
                try
                {
                    _datasetModelSelection10YearExitCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection10YearExitCache);
                    _datasetModelSelection10YearExitCache = SupportClass.FilterSortDataTable(_datasetModelSelection10YearExitCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelSelection5YearEntry);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearEntry))
            {
                try
                {
                    _datasetModelSelection5YearEntry.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearEntry);
                    _datasetModelSelection5YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection5YearEntry, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection5YearEntryCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearEntryCache))
            {
                try
                {
                    _datasetModelSelection5YearEntryCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearEntryCache);
                    _datasetModelSelection5YearEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelection5YearEntryCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            InitializeTable(ref _datasetModelSelection5YearExit);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearExit))
            {
                try
                {
                    _datasetModelSelection5YearExit.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearExit);
                    _datasetModelSelection5YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection5YearExit, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection5YearExitCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearExitCache))
            {
                try
                {
                    _datasetModelSelection5YearExitCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection5YearExitCache);
                    _datasetModelSelection5YearExitCache = SupportClass.FilterSortDataTable(_datasetModelSelection5YearExitCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelSelection3YearEntry);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearEntry))
            {
                try
                {
                    _datasetModelSelection3YearEntry.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearEntry);
                    _datasetModelSelection3YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection3YearEntry, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection3YearEntryCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearEntryCache))
            {
                try
                {
                    _datasetModelSelection3YearEntryCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearEntryCache);
                    _datasetModelSelection3YearEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelection3YearEntryCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            InitializeTable(ref _datasetModelSelection3YearExit);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearExit))
            {
                try
                {
                    _datasetModelSelection3YearExit.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearExit);
                    _datasetModelSelection3YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection3YearExit, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection3YearExitCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearExitCache))
            {
                try
                {
                    _datasetModelSelection3YearExitCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection3YearExitCache);
                    _datasetModelSelection3YearExitCache = SupportClass.FilterSortDataTable(_datasetModelSelection3YearExitCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelSelection1YearEntry);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearEntry))
            {
                try
                {
                    _datasetModelSelection1YearEntry.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearEntry);
                    _datasetModelSelection1YearEntry = SupportClass.FilterSortDataTable(_datasetModelSelection1YearEntry, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection1YearEntryCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearEntryCache))
            {
                try
                {
                    _datasetModelSelection1YearEntryCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearEntryCache);
                    _datasetModelSelection1YearEntryCache = SupportClass.FilterSortDataTable(_datasetModelSelection1YearEntryCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            InitializeTable(ref _datasetModelSelection1YearExit);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearExit))
            {
                try
                {
                    _datasetModelSelection1YearExit.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearExit);
                    _datasetModelSelection1YearExit = SupportClass.FilterSortDataTable(_datasetModelSelection1YearExit, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelSelection1YearExitCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearExitCache))
            {
                try
                {
                    _datasetModelSelection1YearExitCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelSelection1YearExitCache);
                    _datasetModelSelection1YearExitCache = SupportClass.FilterSortDataTable(_datasetModelSelection1YearExitCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelGeneticMax);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMax))
            {
                try
                {
                    _datasetModelGeneticMax.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMax);
                    _datasetModelGeneticMax = SupportClass.FilterSortDataTable(_datasetModelGeneticMax, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGeneticMax.DataSource = _datasetModelGeneticMax;
            textBoxSelectionHistoricComboCount.Text = _datasetModelGeneticMax.Rows.Count.ToString();
            InitializeTable(ref _datasetModelGeneticMaxCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxCache))
            {
                try
                {
                    _datasetModelGeneticMaxCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxCache);
                    _datasetModelGeneticMaxCache = SupportClass.FilterSortDataTable(_datasetModelGeneticMaxCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            InitializeTable(ref _datasetModelGeneticMaxQueue);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxQueue))
            {
                try
                {
                    _datasetModelGeneticMaxQueue.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGeneticMaxQueue);
                    _datasetModelGeneticMaxQueue = SupportClass.FilterSortDataTable(_datasetModelGeneticMaxQueue, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            textBoxSelectionHistoricComboQueueCount.Text = _datasetModelGeneticMaxQueue.Rows.Count.ToString();

            InitializeTable(ref _datasetModelGenetic10Year);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10Year))
            {
                try
                {
                    _datasetModelGenetic10Year.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10Year);
                    _datasetModelGenetic10Year = SupportClass.FilterSortDataTable(_datasetModelGenetic10Year, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGenetic10Year.DataSource = _datasetModelGenetic10Year;
            InitializeTable(ref _datasetModelGenetic10YearCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10YearCache))
            {
                try
                {
                    _datasetModelGenetic10YearCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic10YearCache);
                    _datasetModelGenetic10YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic10YearCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelGenetic5Year);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5Year))
            {
                try
                {
                    _datasetModelGenetic5Year.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5Year);
                    _datasetModelGenetic5Year = SupportClass.FilterSortDataTable(_datasetModelGenetic5Year, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGenetic5Year.DataSource = _datasetModelGenetic5Year;
            InitializeTable(ref _datasetModelGenetic5YearCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5YearCache))
            {
                try
                {
                    _datasetModelGenetic5YearCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic5YearCache);
                    _datasetModelGenetic5YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic5YearCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelGenetic3Year);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Year))
            {
                try
                {
                    _datasetModelGenetic3Year.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Year);
                    _datasetModelGenetic3Year = SupportClass.FilterSortDataTable(_datasetModelGenetic3Year, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGenetic3Year.DataSource = _datasetModelGenetic3Year;
            InitializeTable(ref _datasetModelGenetic3YearCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3YearCache))
            {
                try
                {
                    _datasetModelGenetic3YearCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3YearCache);
                    _datasetModelGenetic3YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic3YearCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelGenetic1Year);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Year))
            {
                try
                {
                    _datasetModelGenetic1Year.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Year);
                    _datasetModelGenetic1Year = SupportClass.FilterSortDataTable(_datasetModelGenetic1Year, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGenetic1Year.DataSource = _datasetModelGenetic1Year;
            InitializeTable(ref _datasetModelGenetic1YearCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1YearCache))
            {
                try
                {
                    _datasetModelGenetic1YearCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1YearCache);
                    _datasetModelGenetic1YearCache = SupportClass.FilterSortDataTable(_datasetModelGenetic1YearCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelGenetic6Month);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6Month))
            {
                try
                {
                    _datasetModelGenetic6Month.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6Month);
                    _datasetModelGenetic6Month = SupportClass.FilterSortDataTable(_datasetModelGenetic6Month, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGenetic6Month.DataSource = _datasetModelGenetic6Month;
            InitializeTable(ref _datasetModelGenetic6MonthCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6MonthCache))
            {
                try
                {
                    _datasetModelGenetic6MonthCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic6MonthCache);
                    _datasetModelGenetic6MonthCache = SupportClass.FilterSortDataTable(_datasetModelGenetic6MonthCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelGenetic3Month);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Month))
            {
                try
                {
                    _datasetModelGenetic3Month.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3Month);
                    _datasetModelGenetic3Month = SupportClass.FilterSortDataTable(_datasetModelGenetic3Month, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGenetic3Month.DataSource = _datasetModelGenetic3Month;
            InitializeTable(ref _datasetModelGenetic3MonthCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3MonthCache))
            {
                try
                {
                    _datasetModelGenetic3MonthCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic3MonthCache);
                    _datasetModelGenetic3MonthCache = SupportClass.FilterSortDataTable(_datasetModelGenetic3MonthCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            InitializeTable(ref _datasetModelGenetic1Month);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Month))
            {
                try
                {
                    _datasetModelGenetic1Month.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1Month);
                    _datasetModelGenetic1Month = SupportClass.FilterSortDataTable(_datasetModelGenetic1Month, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dataGridModelGenetic1Month.DataSource = _datasetModelGenetic1Month;
            InitializeTable(ref _datasetModelGenetic1MonthCache);
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1MonthCache))
            {
                try
                {
                    _datasetModelGenetic1MonthCache.ReadXml(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileModelGenetic1MonthCache);
                    _datasetModelGenetic1MonthCache = SupportClass.FilterSortDataTable(_datasetModelGenetic1MonthCache, "", "fitness", 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void InitializeTable(ref DataTable table)
        {
            table = new DataTable("strategy");
            table.Columns.Add("fitness", typeof(double));
            table.Columns.Add("genome", typeof(string));
            table.Columns.Add("profit", typeof(double));
            table.Columns.Add("transactions", typeof(int));
            table.Columns.Add("bars", typeof(int));
            table.Columns.Add("inception", typeof(DateTime));
            table.PrimaryKey = new DataColumn[] { table.Columns["genome"] };
        }
        private void InitializeAgents()
        {
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + Properties.Settings.Default.GlobalMarket + "\\" + "!INDEX.xml"))
            {
                checkBoxRandomChance.Checked = Properties.Settings.Default.FormStrategyEvolverRandomChanceChecked;
                checkBoxNaturalSelection.Checked = Properties.Settings.Default.FormStrategyEvolverNaturalSelectionChecked;
                checkBoxGeneticEvolution.Checked = Properties.Settings.Default.FormStrategyEvolverGeneticEvolutionChecked;
                checkBox5.Checked = Properties.Settings.Default.FormStrategyEvolverNaturalSelectionOOSChecked;
            }
            else
            {
                checkBoxGeneticEvolution.Checked = false;
                checkBoxRandomChance.Checked = false;
                checkBoxNaturalSelection.Checked = false;
                checkBox5.Checked = false;
            }
        }
        #endregion
        #region Performance Monitoring
        // Performance Counter to measure CPU usage
        private System.Diagnostics.PerformanceCounter cpuCounter;
        // Performance Counter to measure memory usage
        private System.Diagnostics.PerformanceCounter ramCounter;

        [DllImport("wininet")]
        public static extern int InternetGetConnectedState(ref int lpdwFlags, int dwReserved);
        [DllImport("wininet")]
        public static extern int InternetAutodial(int dwFlags, int hwndParent);
        int nFirstTimeCheckConnection = 0;

        // available memory
        private float nFreeMemory;
        private float FreeMemory
        {
            get { return nFreeMemory; }
            set
            {
                nFreeMemory = value;
                this.statusBarPanelMem.Text = nFreeMemory + " Mb Available";
            }
        }

        // CPU usage
        private int nCPUUsage;
        private int CPUUsage
        {
            get { return nCPUUsage; }
            set
            {
                nCPUUsage = value;
                this.statusBarPanelCPU.Text = "CPU usage " + nCPUUsage + "%";
                try
                {
                    Icon icon = Icon.FromHandle(((Bitmap)imageListPercentage.Images[value / 10]).GetHicon());
                    this.statusBarPanelCPU.Icon = icon;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        private void timerMem_Tick(object sender, EventArgs e)
        {
            FreeMemory = ramCounter.NextValue();
            CPUUsage = (int)cpuCounter.NextValue();
        }
        private void timerConnectionInfo_Tick(object sender, EventArgs e)
        {
            ConnectionInfo();
        }
        void ConnectionInfo()
        {
            try
            {
                int nState = 0;
                if (InternetGetConnectedState(ref nState, 0) == 0)
                {
                    if (nFirstTimeCheckConnection++ == 0)
                        // ask for dial up or DSL connection
                        if (InternetAutodial(1, 0) != 0)
                            // check internet connection state again
                            InternetGetConnectedState(ref nState, 0);
                }
                if ((nState & 2) == 2 || (nState & 4) == 4)
                    // reset to reask for connection agina
                    nFirstTimeCheckConnection = 0;
            }
            catch
            {
            }
            this.statusBarPanelInfo.Text = InternetGetConnectedStateString();
        }
        string InternetGetConnectedStateString()
        {
            string strState = "";
            try
            {
                int nState = 0;
                // check internet connection state
                if (InternetGetConnectedState(ref nState, 0) == 0)
                    return "You are currently not connected to the internet";
                if ((nState & 1) == 1)
                    strState = "Modem connection";
                else if ((nState & 2) == 2)
                    strState = "LAN connection";
                else if ((nState & 4) == 4)
                    strState = "Proxy connection";
                else if ((nState & 8) == 8)
                    strState = "Modem is busy with a non-Internet connection";
                else if ((nState & 0x10) == 0x10)
                    strState = "Remote Access Server is installed";
                else if ((nState & 0x20) == 0x20)
                    return "Offline";
                else if ((nState & 0x40) == 0x40)
                    return "Internet connection is currently configured";

                // get current machine IP
                //IPHostEntry he = Dns.Resolve(Dns.GetHostName());
                IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());
                strState += ",  Machine IP: " + he.AddressList[2].ToString();
            }
            catch
            {
            }
            return strState;
        }
        #endregion
        #region Optimization
        private ArrayList SplitGenome(ArrayList parents)
        {
            ArrayList genes = new ArrayList();
            for (int i = 0; i < parents.Count; i++)
            {
                string[] dataArray1 = new string[_maxIndicators];
                Regex rex1 = new Regex(":(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                dataArray1 = rex1.Split(parents[i].ToString());
                for (int j = 0; j < dataArray1.Length; j++)
                {
                    if (dataArray1[j].ToString() != null)
                        genes.Add(dataArray1[j].ToString());
                }
            }
            return genes;
        }



        private Hashtable GetFitnessHashtable(DataTable data1)
        {
            Hashtable _simpleTable = new Hashtable();
            for (int x = data1.Rows.Count - 1; x >= 0; x--)
            {
                _simpleTable.Add(data1.Rows[x].ItemArray[0].ToString(), data1.Rows[x].ItemArray[1].ToString());
            }
            return _simpleTable;
        }
        private double[] GetFitnessArray(DataTable data1)
        {
            double[] result = new double[data1.Rows.Count];
            for (int x = 0; x < data1.Rows.Count; x++)
            {
                result[x] = double.Parse(data1.Rows[x].ItemArray[0].ToString());
            }
            return result;
        }
        #endregion

        public void UpdateMarketIndex(string market)
        {
            comboBoxIndexes.Text = market;
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + comboBoxIndexes.Text + "\\"))
                File.Delete(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + comboBoxIndexes.Text + "\\");
            backgroundWorkerIndexes.RunWorkerAsync();
        }
        public void UpdateMarketTechnicalData(string market)
        {
            _agentQuotesStart = DateTime.Now;
            comboBoxQuotes.Text = market;
            _quoteImportIteration = 0;
            textBoxQuotesPasses.Text = _quoteImportIteration.ToString();
            progressBarQuotes.Value = 0;
            buttonAcquireYahooStop.Enabled = false;
            buttonAcquireYahooImport.Enabled = false;
            textBoxDataAcquisitionConsole.AppendText(_dataSource.GetHistoricMarketDataViaYAHOO(comboBoxQuotes.Text));
            buttonAcquireYahooImport.Enabled = true;
        }
        public void UpdateMarketData()
        {
            _agentQuotesStart = DateTime.Now;
            comboBoxQuotes.Text = Properties.Settings.Default.GlobalMarket;
            _quoteImportIteration = 0;
            textBoxQuotesPasses.Text = _quoteImportIteration.ToString();
            progressBarQuotes.Value = 0;
            buttonAcquireYahooStop.Enabled = false;
            buttonAcquireYahooImport.Enabled = false;
            textBoxDataAcquisitionConsole.AppendText(_dataSource.GetHistoricMarketDataViaYAHOO(comboBoxQuotes.Text));
            buttonAcquireYahooImport.Enabled = true;
        }
        private void buttonAcquireOpenTickRun_Click(object sender, EventArgs e)
        {
            _dataSource.Demo();
        }

        #region Scheduler
        private void ScheduleCallBack(string scheduleName)
        {
            switch (scheduleName)
            {
                case "Indexes":
                    Invoke(new MethodInvoker(UpdateIndex));
                    break;
                case "Data":
                    Invoke(new MethodInvoker(UpdateMarketData));
                    break;
                case "Baseline":
                    Invoke(new MethodInvoker(UpdateBaseLineFile));
                    break;
                case "Analysis":
                    Invoke(new MethodInvoker(UpdateMarketAnalysis));
                    break;
                case "Forecast":
                    Invoke(new MethodInvoker(GetBuyRecommendationsA));
                    Invoke(new MethodInvoker(GetSellRecommendationsA));
                    Invoke(new MethodInvoker(GetBuyRecommendationsB));
                    Invoke(new MethodInvoker(GetSellRecommendationsB));
                    break;
                case "Process":
                    Invoke(new MethodInvoker(ProcessRecommendationsA));
                    Invoke(new MethodInvoker(ProcessRecommendationsB));
                    break;
                case "StartAgents":
                    Invoke(new MethodInvoker(StartAgents));
                    break;
                case "StopAgents":
                    Invoke(new MethodInvoker(StopAgents));
                    break;
                case "Reports":
                    Invoke(new MethodInvoker(UpdateReports));
                    break;
                case "Email":
                    Invoke(new MethodInvoker(EmailStatus));
                    break;
                case "Twitter":
                    Invoke(new MethodInvoker(TwitterStatus));
                    break;
                case "CleanUp":
                    Invoke(new MethodInvoker(CleanUpLogs));
                    break;
            }
        }
        private void LoadSchedules()
        {
            if (Directory.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules))
                foreach (string fileOn in Directory.GetFiles(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules, "*", SearchOption.AllDirectories))
                {
                    FileInfo file = new FileInfo(fileOn);
                    if (file.Extension.Equals(".xml"))
                    {
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                        XmlTextReader xr = new XmlTextReader(fileOn);
                        Schedule schedule = (Schedule)x.Deserialize(xr);
                        schedule.OnTrigger += new Invoke(ScheduleCallBack);
                        xr.Close();
                        while (schedule.NextInvokeTime.CompareTo(DateTime.Now) < 0)
                        {
                            schedule.CalculateNextInvokeTime();
                        }
                        Scheduler.AddSchedule(schedule);
                    }
                }
            /*
            int hours = 24 - DateTime.Now.Hour;
            DateTime indexes = DateTime.Now.Add(new TimeSpan(hours, 0, 0));
            DateTime technical = indexes.Add(new TimeSpan(1, 0, 0));
            DateTime baseline = technical.Add(new TimeSpan(1, 0, 0));
            DateTime analysis = baseline.Add(new TimeSpan(1, 0, 0));
            DateTime forecast = analysis.Add(new TimeSpan(1, 0, 0));
            DateTime process = forecast.Add(new TimeSpan(1, 0, 0));

            // Indexes
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-INDEXES.xml"))
            {
                System.Xml.Serialization.XmlSerializer x1 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextReader xw1 = new XmlTextReader(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-INDEXES.xml");
                Schedule schedule1 = (Schedule)x1.Deserialize(xw1);
                schedule1.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                xw1.Close();
                while (schedule1.NextInvokeTime.CompareTo(DateTime.Now) < 0)
                {
                    schedule1.CalculateNextInvokeTime();
                }
                MarketSage.Scheduler.AddSchedule(schedule1);
            }
            else
            {
                Schedule schedule1 = new Schedule("Indexes", indexes, ScheduleType.DAILY);
                schedule1.Path = "SCHEDULE-INDEXES.xml";
                schedule1.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                System.Xml.Serialization.XmlSerializer x1 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextWriter xw1 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + schedule1.Path, null);
                x1.Serialize(xw1, schedule1);
                xw1.Close();
                MarketSage.Scheduler.AddSchedule(schedule1);
            }

            // Technical data
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-DATA.xml"))
            {
                System.Xml.Serialization.XmlSerializer x2 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextReader xw2 = new XmlTextReader(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-DATA.xml");
                Schedule schedule2 = (Schedule)x2.Deserialize(xw2);
                schedule2.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                xw2.Close();
                while (schedule2.NextInvokeTime.CompareTo(DateTime.Now) < 0)
                {
                    schedule2.CalculateNextInvokeTime();
                }
                MarketSage.Scheduler.AddSchedule(schedule2);
            }
            else
            {
                Schedule schedule2 = new Schedule("Data", technical, ScheduleType.DAILY);
                schedule2.Path = "SCHEDULE-DATA.xml";
                schedule2.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                System.Xml.Serialization.XmlSerializer x2 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextWriter xw2 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + schedule2.Path, null);
                x2.Serialize(xw2, schedule2);
                xw2.Close();
                MarketSage.Scheduler.AddSchedule(schedule2);
            }

            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-BASELINE.xml"))
            {
                System.Xml.Serialization.XmlSerializer x6 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextReader xw6 = new XmlTextReader(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-BASELINE.xml");
                Schedule schedule6 = (Schedule)x6.Deserialize(xw6);
                schedule6.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                xw6.Close();
                while (schedule6.NextInvokeTime.CompareTo(DateTime.Now) < 0)
                {
                    schedule6.CalculateNextInvokeTime();
                }
                MarketSage.Scheduler.AddSchedule(schedule6);
            }
            else
            {
                Schedule schedule6 = new Schedule("Baseline", baseline, ScheduleType.DAILY);
                schedule6.Path = "SCHEDULE-BASELINE.xml";
                schedule6.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                System.Xml.Serialization.XmlSerializer x6 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextWriter xw6 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + schedule6.Path, null);
                x6.Serialize(xw6, schedule6);
                xw6.Close();
                MarketSage.Scheduler.AddSchedule(schedule6);
            }

            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-ANALYSIS.xml"))
            {
                System.Xml.Serialization.XmlSerializer x3 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextReader xw3 = new XmlTextReader(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-ANALYSIS.xml");
                Schedule schedule3 = (Schedule)x3.Deserialize(xw3);
                schedule3.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                xw3.Close();
                while (schedule3.NextInvokeTime.CompareTo(DateTime.Now) < 0)
                {
                    schedule3.CalculateNextInvokeTime();
                }
                MarketSage.Scheduler.AddSchedule(schedule3);
            }
            else
            {
                Schedule schedule3 = new Schedule("Analysis", analysis, ScheduleType.DAILY);
                schedule3.Path = "SCHEDULE-ANALYSIS.xml";
                schedule3.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                System.Xml.Serialization.XmlSerializer x3 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextWriter xw3 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + schedule3.Path, null);
                x3.Serialize(xw3, schedule3);
                xw3.Close();
                MarketSage.Scheduler.AddSchedule(schedule3);
            }

            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-FORECAST.xml"))
            {
                System.Xml.Serialization.XmlSerializer x4 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextReader xw4 = new XmlTextReader(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-FORECAST.xml");
                Schedule schedule4 = (Schedule)x4.Deserialize(xw4);
                schedule4.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                xw4.Close();
                while (schedule4.NextInvokeTime.CompareTo(DateTime.Now) < 0)
                {
                    schedule4.CalculateNextInvokeTime();
                }
                MarketSage.Scheduler.AddSchedule(schedule4);
            }
            else
            {
                Schedule schedule4 = new Schedule("Forecast", forecast, ScheduleType.DAILY);
                schedule4.Path = "SCHEDULE-FORECAST.xml";
                schedule4.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                System.Xml.Serialization.XmlSerializer x4 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextWriter xw4 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + schedule4.Path, null);
                x4.Serialize(xw4, schedule4);
                xw4.Close();
                MarketSage.Scheduler.AddSchedule(schedule4);
            }

            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-PROCESS.xml"))
            {
                System.Xml.Serialization.XmlSerializer x5 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextReader xw5 = new XmlTextReader(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + "SCHEDULE-PROCESS.xml");
                Schedule schedule5 = (Schedule)x5.Deserialize(xw5);
                schedule5.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                xw5.Close();
                while (schedule5.NextInvokeTime.CompareTo(DateTime.Now) < 0)
                {
                    schedule5.CalculateNextInvokeTime();
                }
                MarketSage.Scheduler.AddSchedule(schedule5);
            }
            else
            {
                Schedule schedule5 = new Schedule("Process", process, ScheduleType.DAILY);
                schedule5.Path = "SCHEDULE-PROCESS.xml";
                schedule5.OnTrigger += new MarketSage.Invoke(ScheduleCallBack);
                System.Xml.Serialization.XmlSerializer x5 = new System.Xml.Serialization.XmlSerializer(typeof(Schedule));
                XmlTextWriter xw5 = new XmlTextWriter(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectorySchedules + schedule5.Path, null);
                x5.Serialize(xw5, schedule5);
                xw5.Close();
                MarketSage.Scheduler.AddSchedule(schedule5);
            }
             */
        }

        private void UpdatePortfolioDisplayA()
        {
            _datasetPortfolioA = _portfolioA.GetDataSet();
            _datasetPortfolioA = SupportClass.FilterSortData(_datasetPortfolioA, 2, "", "gain", 1);
            label15.Text = "$" + _portfolioA.GetAccountValue().ToString("0.##");
            label18.Text = "$" + _portfolioA.GetTotalValue().ToString("0.##");
            if (_portfolioA.GetPnL() >= 0)
            {
                label17.ForeColor = Color.Black;
            }
            else
            {
                label17.ForeColor = Color.Red;
            }
            label17.Text = "$" + _portfolioA.GetPnL().ToString("0.##");
            if (_portfolioA.GetPnLPercentage() >= 0)
            {
                label21.ForeColor = Color.Black;
            }
            else
            {
                label21.ForeColor = Color.Red;
            }
            label21.Text = _portfolioA.GetPnLPercentage().ToString("0.##") + "%";
            dataGrid10.DataSource = _datasetPortfolioA.Tables["position"];
            dataGrid11.DataSource = _datasetPortfolioA.Tables["transaction"];
            label14.Text = "Last Updated: " + DateTime.Now;
        }
        private void UpdatePortfolioDisplayB()
        {
            _datasetPortfolioB = _portfolioB.GetDataSet();
            _datasetPortfolioB = SupportClass.FilterSortData(_datasetPortfolioB, 2, "", "gain", 1);
            label11.Text = "$" + _portfolioB.GetAccountValue().ToString("0.##");
            label24.Text = "$" + _portfolioB.GetTotalValue().ToString("0.##");
            if (_portfolioB.GetPnL() >= 0)
            {
                label23.ForeColor = Color.Black;
            }
            else
            {
                label23.ForeColor = Color.Red;
            }
            label23.Text = "$" + _portfolioB.GetPnL().ToString("0.##");
            if (_portfolioB.GetPnLPercentage() >= 0)
            {
                label9.ForeColor = Color.Black;
            }
            else
            {
                label9.ForeColor = Color.Red;
            }
            label9.Text = _portfolioB.GetPnLPercentage().ToString("0.##") + "%";
            dataGrid26.DataSource = _datasetPortfolioB.Tables["position"];
            dataGrid27.DataSource = _datasetPortfolioB.Tables["transaction"];
            ticker1.Text = "";
            for (int x = 0; x < _datasetPortfolioA.Tables["position"].Rows.Count; x++)
            {
                ticker1.Text += _datasetPortfolioA.Tables["position"].Rows[x].ItemArray[0].ToString() + "   " + _datasetPortfolioA.Tables["position"].Rows[x].ItemArray[6].ToString() + "       ";
            }
            label14.Text = "Last Updated: " + DateTime.Now;
        }

        private void UpdateIndex()
        {
            backgroundWorkerIndexes.RunWorkerAsync();
        }

        private void CleanUpLogs()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryLogs);
            TimeSpan customTSpan = new TimeSpan(7, 0, 0, 0);
            //Run through every file and delete if needed.
            //If no files then no problems. The program stops thats all.
            //==============================
            foreach (FileInfo f in dirInfo.GetFiles())
            {
                Console.WriteLine("{0,40}{1,10}{2,25}",
                    f.Name, f.Length, f.LastWriteTime);

                DateTime systemDt = DateTime.Now;
                DateTime fileDt = f.LastWriteTime;
                DateTime cpmTime;
                //IF SYSTEM TIME < FILE WRITE TIME - send a warning message
                //since anyway the file won't be deleted with the current logic
                if (f.LastWriteTime > systemDt)
                    Console.WriteLine("Some one messed the system clock or " +
                    "the file write time was in a different time zone!!!");

                cpmTime = fileDt + customTSpan;


                Console.WriteLine(cpmTime.ToLongDateString());
                Console.WriteLine(cpmTime.ToLongTimeString());

                //CHECKING IF THE FILE LIFE TIME IS MORE THAN THE
                //CURRENT SYSTEM TIME. IF YES FILE IS VALID
                if (DateTime.Compare(cpmTime, systemDt) > 0)
                    Console.WriteLine("Still Valid!");
                else    //CHECKING IF THE FILE LIFE TIME IS <= THE
                //CURRENT SYSTEM TIME. IF YES - FILE IS SET FOR DELETION
                {
                    Console.WriteLine("{0} file is being deleted!", f.Name);
                    f.Delete();
                    Console.WriteLine("{0} file has been deleted!", f.Name);
                }

                Console.WriteLine("\n");

            }

        }

        private void UpdateReports()
        {
            System.Threading.Thread myThread;
            myThread = new System.Threading.Thread(new System.Threading.ThreadStart(UpdateHistoricCharts));
            myThread.Start();
        }

        private void ProcessRecommendationsA()
        {
            //  Sell?
            if (_datasetRecommendationsSellA.Rows.Count > 0)
            {
                for (int x = 0; x < _datasetRecommendationsSellA.Rows.Count; x++)
                {
                    _datasetPortfolioA = _portfolioA.GetDataSet();
                    for (int y = 0; y < _datasetPortfolioA.Tables["position"].Rows.Count; y++)
                    {
                        if (_datasetRecommendationsSellA.Rows[x].ItemArray[0].ToString() == _datasetPortfolioA.Tables["position"].Rows[y].ItemArray[0].ToString())
                        {
                            string symbol = _datasetRecommendationsSellA.Rows[x].ItemArray[0].ToString();
                            string name = _datasetRecommendationsSellA.Rows[x].ItemArray[1].ToString();
                            double price = double.Parse(_quoteService.GetPrice(symbol));
                            _portfolioA.RemovePosition(symbol, price, Properties.Settings.Default.BrokerCommision);
                            textBox18.AppendText(DateTime.Now.ToString() + " : Sold " + " shares of " + symbol + " @ " + price.ToString() + " + 7.00\r\n");
                        }
                    }
                }
            }
            //  Buy?
            if (_datasetRecommendationsBuyA.Rows.Count > 0)
            {

                for (int x = 0; x < _datasetRecommendationsBuyA.Rows.Count; x++)
                {
                    _datasetPortfolioA = _portfolioA.GetDataSet();

                    int desired = (int)numericUpDown7.Value;
                    int current = _datasetPortfolioA.Tables["position"].Rows.Count;
                    int target = desired - current;
                    bool owned = false;

                    if (target <= 0)
                    {
                        double minimum = (Properties.Settings.Default.BrokerCommision * 2) * 100;  //arbitrary BS figuring on at least a 1% gain needed to cover overhead
                        double maximum = (double.Parse(_datasetPortfolioA.Tables["account"].Rows[0].ItemArray[1].ToString()) - (Properties.Settings.Default.BrokerCommision * _datasetRecommendationsBuyA.Rows.Count)) / (_datasetRecommendationsBuyA.Rows.Count - x);
                        string symbol = _datasetRecommendationsBuyA.Rows[x].ItemArray[0].ToString();
                        string name = _datasetRecommendationsBuyA.Rows[x].ItemArray[1].ToString();
                        double price = double.Parse(_quoteService.GetPrice(symbol));
                        double shares = Math.Floor(Math.Floor(maximum / price) / 1) * 1;  // no partial shares
                        if (minimum <= ((shares * price) + Properties.Settings.Default.BrokerCommision))
                        {
                            _portfolioA.AddPosition(symbol, name, shares, price, Properties.Settings.Default.BrokerCommision);
                            textBox18.AppendText(DateTime.Now.ToString() + " : Bought " + shares.ToString() + " shares of " + symbol + " @ " + price.ToString() + " + 7.00\r\n");
                        }
                    }
                    else
                    {
                        for (int y = 0; y < _datasetPortfolioA.Tables["position"].Rows.Count; y++)
                            if (_datasetRecommendationsBuyA.Rows[x].ItemArray[0].ToString() == _datasetPortfolioA.Tables["position"].Rows[y].ItemArray[0].ToString())
                                owned = true;
                        if (owned == false)
                        {
                            double minimum = (Properties.Settings.Default.BrokerCommision * 2) * 100;  //arbitrary BS figuring on at least a 1% gain needed to cover overhead
                            double maximum = (double.Parse(_datasetPortfolioA.Tables["account"].Rows[0].ItemArray[1].ToString()) - (Properties.Settings.Default.BrokerCommision * target)) / target;
                            string symbol = _datasetRecommendationsBuyA.Rows[x].ItemArray[0].ToString();
                            string name = _datasetRecommendationsBuyA.Rows[x].ItemArray[1].ToString();
                            double price = double.Parse(_quoteService.GetPrice(symbol));
                            double shares = Math.Floor(Math.Floor(maximum / price) / 1) * 1;  // no partial shares
                            if (minimum <= ((shares * price) + Properties.Settings.Default.BrokerCommision))
                            {
                                _portfolioA.AddPosition(symbol, name, shares, price, Properties.Settings.Default.BrokerCommision);
                                textBox18.AppendText(DateTime.Now.ToString() + " : Bought " + shares.ToString() + " shares of " + symbol + " @ " + price.ToString() + " + 7.00\r\n");
                            }
                        }
                    }
                }
            }

            UpdatePortfolioDisplayA();
        }
        private void ProcessRecommendationsB()
        {
            //  Sell?
            if (_datasetRecommendationsSellB.Rows.Count > 0)
            {
                for (int x = 0; x < _datasetRecommendationsSellB.Rows.Count; x++)
                {
                    _datasetPortfolioB = _portfolioB.GetDataSet();
                    for (int y = 0; y < _datasetPortfolioB.Tables["position"].Rows.Count; y++)
                    {
                        if (_datasetRecommendationsSellB.Rows[x].ItemArray[0].ToString() == _datasetPortfolioB.Tables["position"].Rows[y].ItemArray[0].ToString())
                        {
                            string symbol = _datasetRecommendationsSellB.Rows[x].ItemArray[0].ToString();
                            string name = _datasetRecommendationsSellB.Rows[x].ItemArray[1].ToString();
                            double price = double.Parse(_quoteService.GetPrice(symbol));
                            _portfolioB.RemovePosition(symbol, price, Properties.Settings.Default.BrokerCommision);
                            textBox18.AppendText(DateTime.Now.ToString() + " : Portfolio B sold " + " shares of " + symbol + " @ " + price.ToString() + " + 7.00\r\n");
                        }
                    }
                }
            }
            //  Buy?
            if (_datasetRecommendationsBuyB.Rows.Count > 0)
            {

                for (int x = 0; x < _datasetRecommendationsBuyB.Rows.Count; x++)
                {
                    _datasetPortfolioB = _portfolioB.GetDataSet();

                    int desired = (int)numericUpDown7.Value;
                    int current = _datasetPortfolioB.Tables["position"].Rows.Count;
                    int target = desired - current;
                    bool owned = false;

                    if (target <= 0)
                    {
                        double minimum = (Properties.Settings.Default.BrokerCommision * 2) * 100;  //arbitrary BS figuring on at least a 1% gain needed to cover overhead
                        double maximum = (double.Parse(_datasetPortfolioB.Tables["account"].Rows[0].ItemArray[1].ToString()) - (Properties.Settings.Default.BrokerCommision * _datasetRecommendationsBuyB.Rows.Count)) / (_datasetRecommendationsBuyB.Rows.Count - x);
                        string symbol = _datasetRecommendationsBuyB.Rows[x].ItemArray[0].ToString();
                        string name = _datasetRecommendationsBuyB.Rows[x].ItemArray[1].ToString();
                        double price = double.Parse(_quoteService.GetPrice(symbol));
                        double shares = Math.Floor(Math.Floor(maximum / price) / 1) * 1;  // no partial shares
                        if (minimum <= ((shares * price) + Properties.Settings.Default.BrokerCommision))
                        {
                            _portfolioB.AddPosition(symbol, name, shares, price, Properties.Settings.Default.BrokerCommision);
                            textBox18.AppendText(DateTime.Now.ToString() + " : Portfolio B bought " + shares.ToString() + " shares of " + symbol + " @ " + price.ToString() + " + 7.00\r\n");
                        }
                    }
                    else
                    {
                        for (int y = 0; y < _datasetPortfolioB.Tables["position"].Rows.Count; y++)
                            if (_datasetRecommendationsBuyB.Rows[x].ItemArray[0].ToString() == _datasetPortfolioB.Tables["position"].Rows[y].ItemArray[0].ToString())
                                owned = true;
                        if (owned == false)
                        {
                            double minimum = (Properties.Settings.Default.BrokerCommision * 2) * 100;  //arbitrary BS figuring on at least a 1% gain needed to cover overhead
                            double maximum = (double.Parse(_datasetPortfolioB.Tables["account"].Rows[0].ItemArray[1].ToString()) - (Properties.Settings.Default.BrokerCommision * target)) / target;
                            string symbol = _datasetRecommendationsBuyB.Rows[x].ItemArray[0].ToString();
                            string name = _datasetRecommendationsBuyB.Rows[x].ItemArray[1].ToString();
                            double price = double.Parse(_quoteService.GetPrice(symbol));
                            double shares = Math.Floor(Math.Floor(maximum / price) / 1) * 1;  // no partial shares
                            if (minimum <= ((shares * price) + Properties.Settings.Default.BrokerCommision))
                            {
                                _portfolioB.AddPosition(symbol, name, shares, price, Properties.Settings.Default.BrokerCommision);
                                textBox18.AppendText(DateTime.Now.ToString() + " : Portfolio B bought " + shares.ToString() + " shares of " + symbol + " @ " + price.ToString() + " + 7.00\r\n");
                            }
                        }
                    }
                }
            }

            UpdatePortfolioDisplayB();
        }

        private void StartAgents()
        {
            //checkBoxModelChanceRandom.Checked = false;
            if (checkBoxNaturalSelection.Checked == false)
                checkBoxNaturalSelection.Checked = true;
            if (checkBox5.Checked == false)
            {
                comboBox2.SelectedIndex = 0;
                checkBox5.Checked = true;
            }
            //checkBoxGeneticEvolution.Checked = false;
        }
        private void StopAgents()
        {
            if (checkBoxModelChanceRandom.Checked == true)
                checkBoxModelChanceRandom.Checked = false;
            if (checkBoxNaturalSelection.Checked == true)
                checkBoxNaturalSelection.Checked = false;
            if (checkBox5.Checked == true)
                checkBox5.Checked = false;
            if (checkBoxGeneticEvolution.Checked == true)
                checkBoxGeneticEvolution.Checked = false;
        }

        public void GetBuyRecommendationsA()
        {
            _currentRecommendationsBuyInstrumentsA.Clear();
            _datasetRecommendationsBuyA.Clear();
            dataGridRecommendationsBuy.DataSource = _datasetRecommendationsBuyA;
            _datasetRecommendationsBuyConsensusA.Clear();
            _counterRecommendationsBuyA = 0;
            _instrumentRecommendationsBuySymbolA = "";
            _instrumentRecommendationsBuyNameA = "";
            _counterRecommendationsBuyStrategyRankA = 0;
            if (_datasetModelGenetic1Month.Rows.Count > 0)
            {
                buttonRecommendationsBuy.Enabled = false;
                _strategyRecommendationsBuyStrategyNameA = _datasetModelGenetic1Month.Rows[_counterRecommendationsBuyStrategyRankA].ItemArray[1].ToString();
                textBoxRecommendationsBuyStrategyName.Text = _strategyRecommendationsBuyStrategyNameA;
                textBoxRecommendationsBuyStrategyRank.Text = _counterRecommendationsBuyStrategyRankA.ToString();
                try
                {
                    _engineRecommendationsBuyA = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, _startDateForecastA, _endDateForecastA);
                    _marketRecommendationsBuyA = new Market();
                    _marketRecommendationsBuyA.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    GeneticStrategy trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsBuyStrategyNameA);
                    _engineRecommendationsBuyA.register(trader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Trading simulation started\r\n");
                textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Market = " + Properties.Settings.Default.GlobalMarket + "\r\n");
                textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Date range = " + _startDateForecastA.ToString() + " - " + _endDateForecastA.ToString() + "\r\n");
                backgroundWorkerRecommendationsBuyA.RunWorkerAsync();
            }
            else
            {
                textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Minimum population not met.\r\n");
                //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                //{
                textBoxRecommendationsBuy.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsBuyA + "\r\n");
                try
                {
                    FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyA, FileMode.Create, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    _datasetRecommendationsBuyA.WriteXml(s);
                    s.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //}
            }
        }
        public void GetSellRecommendationsA()
        {
            _currentRecommendationsSellInstrumentsA.Clear();
            _datasetRecommendationsSellA.Clear();
            dataGridRecommendationsSell.DataSource = _datasetRecommendationsSellA;
            _datasetRecommendationsSellConsensusA.Clear();
            _counterRecommendationsSellA = 0;
            _instrumentRecommendationsSellSymbolA = "";
            _instrumentRecommendationsSellNameA = "";
            _counterRecommendationsSellStrategyRankA = 0;
            if (_datasetModelGenetic1Month.Rows.Count > 0)
            {

                buttonRecommendationsSell.Enabled = false;
                _strategyRecommendationsSellStrategyNameA = _datasetModelGenetic1Month.Rows[_counterRecommendationsSellStrategyRankA].ItemArray[1].ToString();
                textBoxRecommendationsSellStrategyName.Text = _strategyRecommendationsSellStrategyNameA;
                textBoxRecommendationsSellStrategyRank.Text = _counterRecommendationsSellStrategyRankA.ToString();
                try
                {
                    _engineRecommendationsSellA = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, _startDateForecastA, _endDateForecastA);
                    _marketRecommendationsSellA = new Market();
                    _marketRecommendationsSellA.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    GeneticStrategy trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsSellStrategyNameA);
                    _engineRecommendationsSellA.register(trader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                textBoxRecommendationsSell.AppendText(DateTime.Now + " : Trading simulation started\r\n");
                textBoxRecommendationsSell.AppendText(DateTime.Now + " : Market = " + Properties.Settings.Default.GlobalMarket + "\r\n");
                textBoxRecommendationsSell.AppendText(DateTime.Now + " : Date range = " + _startDateForecastA.ToString() + " - " + _endDateForecastA.ToString() + "\r\n");
                backgroundWorkerRecommendationsSellA.RunWorkerAsync();
            }
            else
            {
                textBoxRecommendationsSell.AppendText(DateTime.Now + " : Minimum population not met.\r\n");
                //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                //{
                textBoxRecommendationsSell.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsSellA + "\r\n");
                try
                {
                    FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellA, FileMode.Create, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    _datasetRecommendationsSellA.WriteXml(s);
                    s.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //}
            }
        }

        public void GetBuyRecommendationsB()
        {
            _currentRecommendationsBuyInstrumentsB.Clear();
            _datasetRecommendationsBuyB.Clear();
            dataGrid24.DataSource = _datasetRecommendationsBuyB;
            _datasetRecommendationsBuyConsensusB.Clear();
            _counterRecommendationsBuyB = 0;
            _instrumentRecommendationsBuySymbolB = "";
            _instrumentRecommendationsBuyNameB = "";
            _counterRecommendationsBuyStrategyRankB = 0;

            if (_datasetModelGenetic10Year.Rows.Count > 0 && _datasetModelGenetic5Year.Rows.Count > 0 && _datasetModelGenetic3Year.Rows.Count > 0 && _datasetModelGenetic1Year.Rows.Count > 0 && _datasetModelGenetic6Month.Rows.Count > 0 && _datasetModelGenetic3Month.Rows.Count > 0 && _datasetModelGenetic1Month.Rows.Count > 0)
            {
                button5.Enabled = false;
                _strategyRecommendationsBuyStrategyNameB = _datasetModelGenetic10Year.Rows[0].ItemArray[1].ToString();
                textBox4.Text = _strategyRecommendationsBuyStrategyNameB;
                textBox3.Text = _counterRecommendationsBuyStrategyRankB.ToString();
                try
                {
                    _engineRecommendationsBuyB = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, _startDateForecastB, _endDateForecastB);
                    _marketRecommendationsBuyB = new Market();
                    _marketRecommendationsBuyB.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    GeneticStrategy trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsBuyStrategyNameB);
                    _engineRecommendationsBuyB.register(trader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                textBox1.AppendText(DateTime.Now + " : Trading simulation started\r\n");
                textBox1.AppendText(DateTime.Now + " : Market = " + Properties.Settings.Default.GlobalMarket + "\r\n");
                textBox1.AppendText(DateTime.Now + " : Date range = " + _startDateForecastB.ToString() + " - " + _endDateForecastB.ToString() + "\r\n");
                backgroundWorkerRecommendationsBuyB.RunWorkerAsync();
            }
            else
            {
                textBox1.AppendText(DateTime.Now + " : Minimum population not met.\r\n");
                //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                //{
                textBox1.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsBuyB + "\r\n");
                try
                {
                    FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyB, FileMode.Create, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    _datasetRecommendationsBuyB.WriteXml(s);
                    s.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //}
            }
        }
        public void GetSellRecommendationsB()
        {
            _currentRecommendationsSellInstrumentsB.Clear();
            _datasetRecommendationsSellB.Clear();
            dataGrid25.DataSource = _datasetRecommendationsSellB;
            _datasetRecommendationsSellConsensusB.Clear();
            _counterRecommendationsSellB = 0;
            _instrumentRecommendationsSellSymbolB = "";
            _instrumentRecommendationsSellNameB = "";
            _counterRecommendationsSellStrategyRankB = 0;

            if (_datasetModelGenetic10Year.Rows.Count > 0 && _datasetModelGenetic5Year.Rows.Count > 0 && _datasetModelGenetic3Year.Rows.Count > 0 && _datasetModelGenetic1Year.Rows.Count > 0 && _datasetModelGenetic6Month.Rows.Count > 0 && _datasetModelGenetic3Month.Rows.Count > 0 && _datasetModelGenetic1Month.Rows.Count > 0)
            {

                button4.Enabled = false;
                _strategyRecommendationsSellStrategyNameB = _datasetModelGenetic10Year.Rows[0].ItemArray[1].ToString();
                textBox9.Text = _strategyRecommendationsSellStrategyNameB;
                textBox6.Text = _counterRecommendationsSellStrategyRankB.ToString();
                try
                {
                    _engineRecommendationsSellB = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, _startDateForecastB, _endDateForecastB);
                    _marketRecommendationsSellB = new Market();
                    _marketRecommendationsSellB.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                    GeneticStrategy trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsSellStrategyNameB);
                    _engineRecommendationsSellB.register(trader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                textBox2.AppendText(DateTime.Now + " : Trading simulation started\r\n");
                textBox2.AppendText(DateTime.Now + " : Market = " + Properties.Settings.Default.GlobalMarket + "\r\n");
                textBox2.AppendText(DateTime.Now + " : Date range = " + _startDateForecastB.ToString() + " - " + _endDateForecastB.ToString() + "\r\n");
                backgroundWorkerRecommendationsSellB.RunWorkerAsync();
            }
            else
            {
                textBox2.AppendText(DateTime.Now + " : Minimum population not met.\r\n");
                //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                //{
                textBox2.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsSellB + "\r\n");
                try
                {
                    FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellB, FileMode.Create, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    _datasetRecommendationsSellB.WriteXml(s);
                    s.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //}
            }
        }


        private void UpdateMarketAnalysis()
        {
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + comboBoxMarket.Text + "\\" + "!INDEX.xml"))
            {
                try
                {
                    _engineRisk = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, _startDateAnalyze, _endDateAnalyze);
                    _observerRisk = new Observer();
                    _engineRisk.Observer = _observerRisk;
                    _marketRisk = new Market();
                    _marketRisk.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, comboBoxMarket.Text, true);
                    if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + comboBoxMarket.Text + "\\" + "!EXCEPTIONS_DATES.txt"))
                    {
                        StreamReader sr = new StreamReader(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets + comboBoxMarket.Text + "\\" + "!EXCEPTIONS_DATES.txt");
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            _holidaySchedule.Add(DateTime.Parse(line));
                            line = sr.ReadLine();
                        }
                        sr.Close();
                    }
                    GeneticStrategy entryStrategy = new GeneticStrategy(ref _pluginService, "BuyandHold.36514", "BuyandHold.36514");
                    _engineRisk.register(entryStrategy);
                    textBox21.AppendText(DateTime.Now + " : Trading simulation started\r\n");
                    textBox21.AppendText(DateTime.Now + " : Market = " + comboBoxMarket.Text + "\r\n");
                    textBox21.AppendText(DateTime.Now + " : Date range = " + _startDateAnalyze.ToString() + " - " + _endDateAnalyze.ToString() + "\r\n");
                    _datasetRisk.Rows.Clear();
                    _counterAnalysis = 0;
                    //histogram.GraphPane.CurveList.Clear();
                    //zedGraphControl8.GraphPane.CurveList.Clear();
                    //zedGraphControl7.GraphPane.CurveList.Clear();
                    //zedGraphControl9.GraphPane.CurveList.Clear();
                    if (checkBoxAnalyzeReport.Checked == true)
                    {
                        _reportRiskReward = comboBoxMarket.Text + "-Risk_Reward.txt";
                    }
                    buttonAnalyzeRun.Enabled = false;
                    backgroundWorkerMarketAnalysis.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Market file " + comboBoxMarket.Text + "\\" + "!INDEX.xml" + " does not exist!");
            }
        }
        private void EmailStatus()
        {
            // Construct a new mail message and fill it with information from the form
            MailMessage mail = new MailMessage("jeffrmorton@cox.net", "jeffrey.raymond.morton@gmail.com");
            mail.Subject = "MarketSage Daily Status Report - " + DateTime.Now.ToShortDateString();        // put subject here	
            //if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalSubDirectoryImages + _timeCurrent.Year + "-" + _timeCurrent.Month + "-" + _timeCurrent.Day + " " + _timeCurrent.Hour + _timeCurrent.Minute + _timeCurrent.Second + ".bmp"))
            //    msg.Attachments.Add(new Attachment(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalSubDirectoryImages + _timeCurrent.Year + "-" + _timeCurrent.Month + "-" + _timeCurrent.Day + " " + _timeCurrent.Hour + _timeCurrent.Minute + _timeCurrent.Second + ".bmp"));
            string report = "";
            report += "Portfolio A\r\n";
            report += "ROI: " + _portfolioA.GetPnL().ToString() + "\r\n";
            report += "Positions: ";
            for (int x = 0; x < _datasetPortfolioA.Tables["position"].Rows.Count; x++)
            {
                report += _datasetPortfolioA.Tables["position"].Rows[x].ItemArray[0].ToString() + " ";
            }
            report += "\r\n";
            report += "Long Weight: ";
            report += _buyRecommendationsPercentageOfMarketA.ToString() + "%\r\n";
            report += "Long Recommendations: ";
            for (int x = 0; x < _datasetRecommendationsBuyA.Rows.Count; x++)
            {
                report += _datasetRecommendationsBuyA.Rows[x].ItemArray[0].ToString() + " ";
            }
            report += "\r\n";
            report += "Short Weight: ";
            report += _sellRecommendationsPercentageOfMarketA.ToString() + "%\r\n";
            report += "Short Recommendations: ";
            for (int x = 0; x < _datasetRecommendationsSellA.Rows.Count; x++)
            {
                report += _datasetRecommendationsSellA.Rows[x].ItemArray[0].ToString() + " ";
            }
            report += "\r\n";
            report += "Portfolio B\r\n";
            report += "ROI: " + _portfolioB.GetPnL().ToString() + "\r\n";
            report += "Positions: ";
            for (int x = 0; x < _datasetPortfolioB.Tables["position"].Rows.Count; x++)
            {
                report += _datasetPortfolioB.Tables["position"].Rows[x].ItemArray[0].ToString() + " ";
            }
            report += "\r\n";
            report += "Long Weight: ";
            report += _buyRecommendationsPercentageOfMarketB.ToString() + "%\r\n";
            report += "Long Recommendations: ";
            for (int x = 0; x < _datasetRecommendationsBuyB.Rows.Count; x++)
            {
                report += _datasetRecommendationsBuyB.Rows[x].ItemArray[0].ToString() + " ";
            }
            report += "\r\n";
            report += "Short Weight: ";
            report += _sellRecommendationsPercentageOfMarketB.ToString() + "%\r\n";
            report += "Short Recommendations: ";
            for (int x = 0; x < _datasetRecommendationsSellB.Rows.Count; x++)
            {
                report += _datasetRecommendationsSellB.Rows[x].ItemArray[0].ToString() + " ";
            }
            report += "\r\n";
            mail.Body = report;           // put body of email here
            SmtpClient clnt = new SmtpClient("smtp.east.cox.net", 25);
            clnt.DeliveryMethod = SmtpDeliveryMethod.Network;
            clnt.Send(mail);
        }

        private void TwitterStatus()
        {
            string username = "MarketSage";
            string password = "123456";
            string report = "";
            report += "Long: ";
            for (int x = 0; x < _datasetRecommendationsBuyB.Rows.Count; x++)
            {
                report += _datasetRecommendationsBuyB.Rows[x].ItemArray[0].ToString() + " ";
            }
            report += " - ";
            report += "Short: ";
            for (int x = 0; x < _datasetRecommendationsSellB.Rows.Count; x++)
            {
                report += _datasetRecommendationsSellB.Rows[x].ItemArray[0].ToString() + " ";
            }
            try
            {
                string uTop = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
                string url = "http://twitter.com/statuses/update.xml";
                string param = "status=" + report;
                byte[] Pdata = System.Text.Encoding.ASCII.GetBytes(param);
                ServicePointManager.Expect100Continue = false;
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                req.Method = "POST";
                req.Headers.Add("Authorization", "Basic " + uTop);
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = Pdata.Length;
                System.IO.Stream reqStream = req.GetRequestStream();
                reqStream.Write(Pdata, 0, Pdata.Length);
                reqStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error");
            }
        }

        #endregion
        #region Neural Network
        private void Train(BackgroundWorker worker, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            worker.ReportProgress(0);
            for (int i = 0; i < trainingSet.NoOfInstances; i++)
            {
                bp.loadTrainingInstance(trainingSet.getInstance(i));
                bp.update();
                bp.storeOutputs();
                bp.TrainingItterations = bp.TrainingItterations + 1;
                worker.ReportProgress((int)((i / trainingSet.NoOfInstances) * 100));
                if ((checkBoxShowState.Checked == true))
                {
                    pictureBoxInput.Image = showNeurons(ref pictureBoxInput, 0);
                    pictureBoxHiddenWeights.Image = showWeights(ref pictureBoxHiddenWeights, 1);
                    pictureBoxHidden.Image = showNeurons(ref pictureBoxHidden, 1);
                    pictureBoxStateWeights.Image = showNeurons(ref pictureBoxStateWeights, 2);
                    pictureBoxState.Image = showNeurons(ref pictureBoxState, 2);
                    pictureBoxOutputWeights.Image = showWeights(ref pictureBoxOutputWeights, 3);
                    pictureBoxOutput.Image = showNeurons(ref pictureBoxOutput, 3);
                    pictureBoxOutputDesired.Image = showNeurons(ref pictureBoxOutputDesired, 4);
                }
            }
        }
        private void DisplayNeuralNetwork()
        {
            pictureBoxInput.Image = showNeurons(ref pictureBoxInput, 0);
            pictureBoxHiddenWeights.Image = showWeights(ref pictureBoxHiddenWeights, 1);
            pictureBoxHidden.Image = showNeurons(ref pictureBoxHidden, 1);
            pictureBoxStateWeights.Image = showNeurons(ref pictureBoxStateWeights, 2);
            pictureBoxState.Image = showNeurons(ref pictureBoxState, 2);
            pictureBoxOutputWeights.Image = showWeights(ref pictureBoxOutputWeights, 3);
            pictureBoxOutput.Image = showNeurons(ref pictureBoxOutput, 3);
            pictureBoxOutputDesired.Image = showNeurons(ref pictureBoxOutputDesired, 4);
            textBoxInput.Text = bp._inputNeurons.ToString();
            textBoxHidden.Text = bp._hiddenNeurons.ToString();
            textBoxState.Text = bp._stateNeurons.ToString();
            textBoxOutput.Text = bp._outputNeurons.ToString();
            textBoxRate.Text = bp.learningRate.ToString();
            textBoxNoise.Text = bp.randomness.ToString();
            textBoxError.Text = bp.BPerror.ToString();
            textBoxIterations.Text = bp.TrainingItterations.ToString();
        }
        public Bitmap showNeurons(ref PictureBox canvas, int layer)
        {
            Bitmap offScreenBmp = new Bitmap(canvas.Width, canvas.Height);
            Graphics offScreenDC = Graphics.FromImage(offScreenBmp);
            float[] screenX = new float[2];
            float[] screenY = new float[2];
            double value = 0;
            Color c;
            int width = 0;
            double pos;
            double neg;
            System.Drawing.Brush br = new System.Drawing.SolidBrush(Color.FromArgb(0, 0, 0));
            // offScreenDC.FillRectangle(br, 0, 0, canvas.Width, canvas.Height);
            if (layer == 0)
            {
                width = bp._inputNeurons;
            }
            else if (layer == 1)
            {
                width = bp._hiddenNeurons;
            }
            else if (layer == 2)
            {
                width = bp._stateNeurons;
            }
            else if (layer == 3)
            {
                width = bp._outputNeurons;
            }
            else if (layer == 4)
            {
                width = bp._outputNeurons;
            }
            for (int x = 0; x < width; x++)
            {
                if (layer == 0)
                {
                    value = bp._input[x].value;
                }
                else if (layer == 1)
                {
                    value = bp._hidden[x].value;
                }
                else if (layer == 2)
                {
                    value = bp._state[x].value;
                }
                else if (layer == 3)
                {
                    value = bp._output[x].value;
                }
                else if (layer == 4)
                {
                    value = bp._output[x].desiredValue;
                }

                if ((value > 0))
                {
                    pos = value * 255;
                    neg = 0;
                }
                else
                {
                    pos = 0;
                    neg = System.Math.Abs(value * 255);
                }
                c = Color.FromArgb((int)neg, 0, (int)pos);
                System.Drawing.Pen pen = new System.Drawing.Pen(c);
                System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(c);
                //screenX[0] = (x / width) * canvas.Width;
                //screenY[0] = (y / height) * canvas.Height;
                //screenX[1] = ((x + 1) / width) * canvas.Width;
                //screenY[1] = ((y + 1) / height) * canvas.Height;
                screenX[0] = (canvas.Width / width) * x;
                screenY[0] = 0;
                //screenX[1] = ((x + 1) / width) * canvas.Width;
                screenX[1] = canvas.Width / width;
                screenY[1] = canvas.Height;
                offScreenDC.FillRectangle(brush, screenX[0], screenY[0], screenX[1], screenY[1]);
                //offScreenDC.DrawLine(pen, screenX[0], screenY[0], screenX[1], screenY[1]);
            }
            return offScreenBmp;
        }
        public Bitmap showWeights(ref PictureBox canvas, int layer)
        {
            Bitmap offScreenBmp = new Bitmap(canvas.Width, canvas.Height);
            Graphics offScreenDC = Graphics.FromImage(offScreenBmp);
            float[] screenX = new float[2];
            float[] screenY = new float[2];
            double value = 0;
            Color c;
            int width = 0;
            int height = 0;
            double pos;
            double neg;
            System.Drawing.Brush br = new System.Drawing.SolidBrush(Color.FromArgb(0, 0, 0));
            // offScreenDC.FillRectangle(br, 0, 0, canvas.Width, canvas.Height);
            if (layer == 1)
            {
                width = bp._inputNeurons + bp._stateNeurons;
                height = bp._hiddenNeurons;
            }
            else if (layer == 2)
            {
                width = bp._hiddenNeurons;
                height = bp._stateNeurons;
            }
            else if (layer == 3)
            {
                width = bp._hiddenNeurons;
                height = bp._outputNeurons;
            }
            if ((layer != 2))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (layer == 1)
                        {
                            value = bp._hidden[y].getWeight(x);
                        }
                        else if (layer == 2)
                        {
                            value = bp._state[y].getWeight(x);
                        }
                        else if (layer == 3)
                        {
                            value = bp._output[y].getWeight(x);
                        }
                        if ((value > 0))
                        {
                            if (value > 1)
                                pos = 255;
                            else
                                pos = value * 255;
                            neg = 0;
                        }
                        else
                        {
                            if (value < -1)
                                neg = 255;
                            else
                                neg = System.Math.Abs(value * 255);
                            pos = 0;
                        }
                        c = Color.FromArgb((int)neg, 0, (int)pos);
                        System.Drawing.Pen pen = new System.Drawing.Pen(c);
                        System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(c);
                        //screenX[0] = (x / width) * canvas.Width;
                        //screenY[0] = (y / height) * canvas.Height;
                        //screenX[1] = ((x + 1) / width) * canvas.Width;
                        //screenY[1] = ((y + 1) / height) * canvas.Height;
                        screenX[0] = (canvas.Width / width) * x;
                        screenY[0] = 0;
                        //screenX[1] = ((x + 1) / width) * canvas.Width;
                        screenX[1] = canvas.Width / width;
                        screenY[1] = canvas.Height;
                        offScreenDC.FillRectangle(brush, screenX[0], screenY[0], screenX[1], screenY[1]);
                        //offScreenDC.DrawLine(pen, screenX[0], screenY[0], screenX[1], screenY[1]);
                    }
                }
            }
            return offScreenBmp;
        }
        public void showOutputGraph(ref Bitmap offScreenBmp, int OutputIndex)
        {
            Pen pen = new Pen(Color.Black);
            Pen pen2 = new Pen(Color.Red);
            //Bitmap offScreenBmp = new Bitmap(canvas.Width, canvas.Height);
            //Bitmap offScreenBmp = canvas.Image.;
            Graphics offScreenDC = Graphics.FromImage(offScreenBmp);
            float screenX = 0;
            float screenY = 0;
            float screenY2 = 0;
            float prevScreenX = 0;
            float prevScreenY = 0;
            float prevScreenY2 = 0;
            /*
            if ((!(bp.IsMissing(clearGraph))))
            {
                if ((clearGraph() == true))
                {
                    canvas.Cls();
                }
            }
             */
            for (int i = 0; i < bp.tableGraphOutput.Rows.Count; i++)
            {
                screenX = (offScreenBmp.Width / (bp.tableGraphOutput.Rows.Count - 1)) * i;
                //MessageBox.Show(screenX.ToString());
                //screenX[0] = (offScreenBmp.Width / outputGraphPosition) * i + 1;
                //screenY = canvas.Height /2;
                screenY = (float)(double.Parse(bp.tableGraphOutput.Rows[i].ItemArray[OutputIndex].ToString())) * offScreenBmp.Height;
                screenY2 = (float)(double.Parse(bp.tableGraphDesired.Rows[i].ItemArray[OutputIndex].ToString())) * offScreenBmp.Height;
                //screenY = ((double)tableOutputGraph.Rows[i].ItemArray[OutputIndex] * canvas.Height) / canvas.Height;
                //screenY[0] = ((double)(1 - outputGraph[i, OutputIndex]) * canvas.Height) / canvas.Height;
                if ((i > 0))
                {
                    offScreenDC.DrawLine(pen, prevScreenX, prevScreenY, screenX, screenY);
                    offScreenDC.DrawLine(pen2, prevScreenX, prevScreenY2, screenX, screenY2);

                    //          canvas.Line (prevScreenX(0), prevScreenY(0))-(screenX(0), screenY(0)), RGB(0, 0, 255)
                }
                //screenY[1] = (double)(1 - outputGraph2[i, OutputIndex]) * canvas.Height;
                //f ((i > 0))
                //{
                //    offScreenDC.DrawLine(pen2, prevScreenX, prevScreenY[1], screenX, screenY[1]);
                //    canvas.Image = offScreenBmp;
                //}
                prevScreenX = screenX;
                prevScreenY = screenY;
                prevScreenY2 = screenY2;
            }
        }
        #endregion
        #region Baseline
        private void UpdateBaseLineFile()
        {
            Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision);
            Market market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            Observer observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating historic baseline\r\n");
            _agentModelHistoricBaseline = GetBaseLine(engine, market, 36514);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModelHistoricBaseline.ToString() + ")\r\n");

            engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(10 * 365, 0, 0, 0)), DateTime.Now);
            market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating 10 year baseline\r\n");
            _agentModel10YearBaseline = GetBaseLine(engine, market, 3651);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModel10YearBaseline.ToString() + ")\r\n");

            engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(5 * 365, 0, 0, 0)), DateTime.Now);
            market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating 5 year baseline\r\n");
            _agentModel5YearBaseline = GetBaseLine(engine, market, 1825);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModel5YearBaseline.ToString() + ")\r\n");

            engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(3 * 365, 0, 0, 0)), DateTime.Now);
            market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating 3 year baseline\r\n");
            _agentModel3YearBaseline = GetBaseLine(engine, market, 1095);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModel3YearBaseline.ToString() + ")\r\n");

            engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(1 * 365, 0, 0, 0)), DateTime.Now);
            market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating 1 year baseline\r\n");
            _agentModel1YearBaseline = GetBaseLine(engine, market, 365);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModel1YearBaseline.ToString() + ")\r\n");

            engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(6 * 30, 0, 0, 0)), DateTime.Now);
            market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating 6 month baseline\r\n");
            _agentModel6MonthBaseline = GetBaseLine(engine, market, 180);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModel6MonthBaseline.ToString() + ")\r\n");

            engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(3 * 30, 0, 0, 0)), DateTime.Now);
            market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating 3 month baseline\r\n");
            _agentModel3MonthBaseline = GetBaseLine(engine, market, 90);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModel3MonthBaseline.ToString() + ")\r\n");

            engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(1 * 30, 0, 0, 0)), DateTime.Now);
            market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            observer = new Observer();
            engine.Observer = observer;
            textBoxBaseline.AppendText(DateTime.Now + " : Generating 1 month baseline\r\n");
            _agentModel1MonthBaseline = GetBaseLine(engine, market, 30);
            textBoxBaseline.AppendText(DateTime.Now + " : Done (" + _agentModel1MonthBaseline.ToString() + ")\r\n");

            FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _agentModelFileBaseLine, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(_agentModelHistoricBaseline.ToString());
            sw.WriteLine(_agentModel10YearBaseline.ToString());
            sw.WriteLine(_agentModel5YearBaseline.ToString());
            sw.WriteLine(_agentModel3YearBaseline.ToString());
            sw.WriteLine(_agentModel1YearBaseline.ToString());
            sw.WriteLine(_agentModel6MonthBaseline.ToString());
            sw.WriteLine(_agentModel3MonthBaseline.ToString());
            sw.WriteLine(_agentModel1MonthBaseline.ToString());
            sw.Close();
            fs.Close();
        }
        private void LoadBaseLineFile()
        {
            if (File.Exists(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _agentModelFileBaseLine))
            {
                FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _agentModelFileBaseLine, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                _agentModelHistoricBaseline = double.Parse(sr.ReadLine());
                _agentModel10YearBaseline = double.Parse(sr.ReadLine());
                _agentModel5YearBaseline = double.Parse(sr.ReadLine());
                _agentModel3YearBaseline = double.Parse(sr.ReadLine());
                _agentModel1YearBaseline = double.Parse(sr.ReadLine());
                _agentModel6MonthBaseline = double.Parse(sr.ReadLine());
                _agentModel3MonthBaseline = double.Parse(sr.ReadLine());
                _agentModel1MonthBaseline = double.Parse(sr.ReadLine());
                sr.Close();
                fs.Close();
            }
        }
        private double GetBaseLine(Engine engine, Market market, int period)
        {
            GeneticStrategy strategy = new GeneticStrategy(ref _pluginService, "BuyandHold." + period.ToString(), "BuyandHold." + period.ToString());
            engine.register(strategy);
            SortedList list = new SortedList();
            return _strategyTester.BackTest(engine, market, false, ref list);
        }
        #endregion
        #region Charts
        private void UpdateHistoricCharts()
        {
            UpdateHistoricChart(fitnessGraph1Year, 1);
            UpdateHistoricChart(fitnessGraph3Year, 3);
            UpdateHistoricChart(fitnessGraph5Year, 5);
            UpdateHistoricChart(fitnessGraph10Year, 10);
            UpdateHistoricChart(fitnessGraphMax, 100);
        }
        private void UpdateHistoricChart(ZedGraphControl chart, int years)
        {
            chart.GraphPane.CurveList.Clear();
            chart.GraphPane.Title.Text = "";
            chart.GraphPane.XAxis.Title.Text = "Time";
            chart.GraphPane.YAxis.Title.Text = "Value";
            chart.GraphPane.XAxis.Type = AxisType.DateAsOrdinal;
            chart.GraphPane.Fill = new Fill(Color.Gray, Color.White, 90.0F);
            chart.AxisChange();
            chart.Invalidate();

            Engine engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
            Observer observer = new Observer();
            engine.Observer = observer;
            Market market = new Market();
            market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
            market.random = false;
            GeneticStrategy strategy2 = new GeneticStrategy(ref _pluginService, "BuyandHold." + (years * 365).ToString(), "BuyandHold." + (years * 365).ToString());
            engine.register(strategy2);
            SortedList list = new SortedList();
            _strategyTester.BackTest(engine, market, false, ref list);
            ArrayList date = new ArrayList();
            ArrayList value = new ArrayList();
            IDictionaryEnumerator ide = list.GetEnumerator();
            while (ide.MoveNext())
            {
                date.Add((double)ide.Key);
                value.Add((double)ide.Value);
            }
            double[] dateArray = (double[])date.ToArray(typeof(double));
            double[] valueArray = (double[])value.ToArray(typeof(double));
            LineItem line7;
            line7 = chart.GraphPane.AddCurve("Hold (Max)", dateArray, valueArray, Color.Black, SymbolType.None);
            line7.Line.Width = 3;
            //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            chart.AxisChange();
            chart.Invalidate();

            if (_datasetModelSelectionMaxEntry.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", "fitness", 1);
                TestStrategy strategy = new TestStrategy(ref _pluginService, _datasetModelSelectionMaxEntry.Rows[0].ItemArray[1].ToString(), true);
                engine.register(strategy);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, true, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line;
                line = chart.GraphPane.AddCurve("Bull (Max)", dateArray, valueArray, Color.Green, SymbolType.None);
                line.Line.Width = 3;
                //line.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }
            chart.AxisChange();
            chart.Invalidate();

            if (_datasetModelSelectionMaxExit.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", "fitness", 1);
                TestStrategy strategy = new TestStrategy(ref _pluginService, _datasetModelSelectionMaxExit.Rows[0].ItemArray[1].ToString(), false);
                engine.register(strategy);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line2;
                line2 = chart.GraphPane.AddCurve("Bear (Max)", dateArray, valueArray, Color.Red, SymbolType.None);
                line2.Line.Width = 3;
                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }
            chart.AxisChange();
            chart.Invalidate();
            /*
            if (_datasetModelSelectionMaxEntry.Rows.Count > 0 && _datasetModelSelectionMaxExit.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelSelectionMaxEntry = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxEntry, "", "fitness", 1);
                _datasetModelSelectionMaxExit = SupportClass.FilterSortDataTable(_datasetModelSelectionMaxExit, "", "fitness", 1);
                strategy2 = new GeneticStrategy(ref _pluginService, _datasetModelSelectionMaxEntry.Rows[0].ItemArray[1].ToString(), _datasetModelSelectionMaxExit.Rows[0].ItemArray[1].ToString());
                engine.register(strategy2);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line5;
                line5 = chart.GraphPane.AddCurve("Combined", dateArray, valueArray, Color.Blue, SymbolType.None);
                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }
            chart.AxisChange();
            chart.Invalidate();
            */
            if (_datasetModelGeneticMax.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelGeneticMax = SupportClass.FilterSortDataTable(_datasetModelGeneticMax, "", "fitness", 1);
                strategy2 = new GeneticStrategy(ref _pluginService, _datasetModelGeneticMax.Rows[0].ItemArray[1].ToString());
                engine.register(strategy2);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line10;
                line10 = chart.GraphPane.AddCurve("Fitness (Max)", dateArray, valueArray, Color.Cyan, SymbolType.None);
                line10.Line.Width = 3;
                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }
            chart.AxisChange();
            chart.Invalidate();

            if (_datasetModelGenetic10Year.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelGenetic10Year = SupportClass.FilterSortDataTable(_datasetModelGenetic10Year, "", "fitness", 1);
                strategy2 = new GeneticStrategy(ref _pluginService, _datasetModelGenetic10Year.Rows[0].ItemArray[1].ToString());
                engine.register(strategy2);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line10;
                line10 = chart.GraphPane.AddCurve("Fitness(10YR)", dateArray, valueArray, Color.Blue, SymbolType.None);
                line10.Line.Width = 3;

                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }
            chart.AxisChange();
            chart.Invalidate();

            if (_datasetModelGenetic5Year.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelGenetic5Year = SupportClass.FilterSortDataTable(_datasetModelGenetic5Year, "", "fitness", 1);
                strategy2 = new GeneticStrategy(ref _pluginService, _datasetModelGenetic5Year.Rows[0].ItemArray[1].ToString());
                engine.register(strategy2);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line10;
                line10 = chart.GraphPane.AddCurve("Fitness(5YR)", dateArray, valueArray, Color.Tomato, SymbolType.None);
                line10.Line.Width = 3;

                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }
            chart.AxisChange();
            chart.Invalidate();



            if (_datasetModelGenetic1Year.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelGenetic1Year = SupportClass.FilterSortDataTable(_datasetModelGenetic1Year, "", "fitness", 1);
                strategy2 = new GeneticStrategy(ref _pluginService, _datasetModelGenetic1Year.Rows[0].ItemArray[1].ToString());
                engine.register(strategy2);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line10;
                line10 = chart.GraphPane.AddCurve("Fitness (1YR)", dateArray, valueArray, Color.Gold, SymbolType.None);
                line10.Line.Width = 3;

                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }

            chart.AxisChange();
            chart.Invalidate();

            if (_datasetModelGenetic6Month.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelGenetic6Month = SupportClass.FilterSortDataTable(_datasetModelGenetic6Month, "", "profit", 1);
                strategy2 = new GeneticStrategy(ref _pluginService, _datasetModelGenetic6Month.Rows[0].ItemArray[1].ToString());
                engine.register(strategy2);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line10;
                line10 = chart.GraphPane.AddCurve("Fitness (6MT)", dateArray, valueArray, Color.ForestGreen, SymbolType.None);
                line10.Line.Width = 3;

                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }

            chart.AxisChange();
            chart.Invalidate();


            if (_datasetModelGenetic1Month.Rows.Count > 0)
            {
                engine = new Engine(Properties.Settings.Default.AccountInitialBalance, Properties.Settings.Default.BrokerCommision, DateTime.Now.Subtract(new TimeSpan(365 * years, 0, 0, 0)), DateTime.MaxValue);
                observer = new Observer();
                engine.Observer = observer;
                market = new Market();
                market.Load(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryMarkets, Properties.Settings.Default.GlobalMarket, true);
                market.random = false;
                _datasetModelGenetic1Month = SupportClass.FilterSortDataTable(_datasetModelGenetic1Month, "", "profit", 1);
                strategy2 = new GeneticStrategy(ref _pluginService, _datasetModelGenetic1Month.Rows[0].ItemArray[1].ToString());
                engine.register(strategy2);
                list = new SortedList();
                _strategyTester.BackTest(engine, market, false, ref list);
                date = new ArrayList();
                value = new ArrayList();
                ide = list.GetEnumerator();
                while (ide.MoveNext())
                {
                    date.Add((double)ide.Key);
                    value.Add((double)ide.Value);
                }
                dateArray = (double[])date.ToArray(typeof(double));
                valueArray = (double[])value.ToArray(typeof(double));
                LineItem line10;
                line10 = chart.GraphPane.AddCurve("Fitness (1MT)", dateArray, valueArray, Color.Silver, SymbolType.None); line10.Line.Width = 3;
                line10.Line.Width = 3;
                //line2.Line.Fill = new Fill(Color.White, Color.Black, 45F);
            }

            chart.AxisChange();
            chart.Invalidate();
            //Image image = chart.GetImage();
            //image.Save(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryReports + "Historic.bmp");
        }
        private double[] YearPeriodHistogram(Instrument instrument)
        {
            ArrayList _technicalData = instrument.TechnicalData;
            TechnicalData _yesterday = (TechnicalData)_technicalData[0];
            double[] _histogram = new double[367];

            for (int x = 0; x <= _technicalData.Count - 1; x++)
            {
                if (x > 0)
                {
                    _histogram[((TechnicalData)_technicalData[x]).Date.DayOfYear] = (_yesterday.AdjClose / 100) * (((TechnicalData)_technicalData[x]).AdjClose - _yesterday.AdjClose);
                    //_histogram[(int)(((TechnicalData)_technicalData[x]).Date.DayOfYear)] = ((TechnicalData)_technicalData[x]).Close - _yesterday.Close;
                }
                _yesterday = (TechnicalData)_technicalData[x];
                System.Windows.Forms.Application.DoEvents();
            }
            return (_histogram);
        }
        private double[] MonthPeriodHistogram(Instrument instrument)
        {
            ArrayList _technicalData = instrument.TechnicalData;
            TechnicalData _yesterday = (TechnicalData)_technicalData[0];
            double[] _histogram = new double[32];

            for (int x = 0; x <= _technicalData.Count - 1; x++)
            {
                if (x > 0)
                {
                    _histogram[((TechnicalData)_technicalData[x]).Date.Day] = (_yesterday.AdjClose / 100) * (((TechnicalData)_technicalData[x]).AdjClose - _yesterday.AdjClose);
                    //_histogram[(int)(((TechnicalData)_technicalData[x]).Date.Day)] = ((TechnicalData)_technicalData[x]).Close - _yesterday.Close;
                }
                _yesterday = (TechnicalData)_technicalData[x];
                System.Windows.Forms.Application.DoEvents();
            }
            return (_histogram);
        }
        private double[] WeekPeriodHistogram(Instrument instrument)
        {
            ArrayList _technicalData = instrument.TechnicalData;
            TechnicalData _yesterday = (TechnicalData)_technicalData[0];
            double[] _histogram = new double[8];

            for (int x = 0; x <= _technicalData.Count - 1; x++)
            {
                if (x > 0)
                {
                    _histogram[(int)((TechnicalData)_technicalData[x]).Date.DayOfWeek] = (_yesterday.AdjClose / 100) * (((TechnicalData)_technicalData[x]).AdjClose - _yesterday.AdjClose);
                    //_histogram[(int)(((TechnicalData)_technicalData[x]).Date.Day)] = ((TechnicalData)_technicalData[x]).Close - _yesterday.Close;
                }
                _yesterday = (TechnicalData)_technicalData[x];
                System.Windows.Forms.Application.DoEvents();
            }
            return (_histogram);
        }
        private double[] LunarPhaseHistogram(Instrument instrument)
        {
            ArrayList _technicalData = instrument.TechnicalData;
            TechnicalData _yesterday = (TechnicalData)_technicalData[0];
            double[] _histogram = new double[8];

            for (int x = 0; x <= _technicalData.Count - 1; x++)
            {
                if (x > 0)
                {
                    _histogram[SupportClass.GetLunarPhase(((TechnicalData)_technicalData[x]).Date)] = (_yesterday.AdjClose / 100) * (((TechnicalData)_technicalData[x]).AdjClose - _yesterday.AdjClose);
                    //_histogram[(int)(((TechnicalData)_technicalData[x]).Date.DayOfYear)] = ((TechnicalData)_technicalData[x]).Close - _yesterday.Close;
                }
                _yesterday = (TechnicalData)_technicalData[x];
                System.Windows.Forms.Application.DoEvents();
            }
            return (_histogram);
        }
        private void CreateSolarChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.Title.Text = "";
            // Set the title and axis labels
            myPane.Title.Text = comboBoxMarket.Text;
            myPane.XAxis.Title.Text = "Day of year";
            myPane.YAxis.Title.Text = "% Change";
            // Set the XAxis to ordinal type
            myPane.XAxis.Type = AxisType.LinearAsOrdinal;
            // Generate some curve data from the Sine function
            PointPairList list = new PointPairList();
            for (int i = 1; i <= 366; i++)
            {
                double x = i;
                double y = _marketHistogramAverage[i];
                list.Add(x, y);
                System.Windows.Forms.Application.DoEvents();
            }
            myBar = myPane.AddBar("", list, Color.Black);

            // Draw a box item to highlight a value range 
            BoxObj box = new BoxObj(DateTime.Now.DayOfYear, myPane.Chart.Rect.Top, 1, myPane.Chart.Rect.Bottom - myPane.Chart.Rect.Top);
            box.Fill = new Fill(Color.Red);
            // Use the BehindAxis zorder to draw the highlight beneath the grid lines 
            box.ZOrder = ZOrder.D_BehindAxis;
            myPane.GraphObjList.Add(box);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
               Color.Black, 45.0F);

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
        }
        private void CreateMonthChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.Title.Text = "";
            // Set the title and axis labels
            myPane.Title.Text = comboBoxMarket.Text;
            myPane.XAxis.Title.Text = "Day of month";
            myPane.YAxis.Title.Text = "% Change";
            // Set the XAxis to ordinal type
            myPane.XAxis.Type = AxisType.LinearAsOrdinal;
            // Generate some curve data from the Sine function
            PointPairList list = new PointPairList();
            for (int i = 1; i <= 32; i++)
            {
                double x = i;
                double y = _marketMonthHistogramAverage[i];
                list.Add(x, y);
                System.Windows.Forms.Application.DoEvents();
            }
            myBar = myPane.AddBar("", list, Color.Black);

            // Draw a box item to highlight a value range 
            BoxObj box = new BoxObj(DateTime.Now.Day, myPane.Chart.Rect.Top, 1, myPane.Chart.Rect.Bottom - myPane.Chart.Rect.Top);
            box.Fill = new Fill(Color.Red);
            // Use the BehindAxis zorder to draw the highlight beneath the grid lines 
            box.ZOrder = ZOrder.D_BehindAxis;
            myPane.GraphObjList.Add(box);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
               Color.Black, 45.0F);

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
        }
        private void CreateWeekChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.Title.Text = "";
            // Set the title and axis labels
            myPane.Title.Text = comboBoxMarket.Text;
            myPane.XAxis.Title.Text = "Day of week";
            myPane.YAxis.Title.Text = "% Change";
            // Set the XAxis to ordinal type
            myPane.XAxis.Type = AxisType.LinearAsOrdinal;
            // Generate some curve data from the Sine function
            PointPairList list = new PointPairList();
            for (int i = 1; i <= 7; i++)
            {
                double x = i;
                double y = _marketWeekHistogramAverage[i];
                list.Add(x, y);
                System.Windows.Forms.Application.DoEvents();
            }
            myBar = myPane.AddBar("", list, Color.Black);

            // Draw a box item to highlight a value range 
            BoxObj box = new BoxObj((int)DateTime.Now.DayOfWeek, myPane.Chart.Rect.Top, 1, myPane.Chart.Rect.Bottom - myPane.Chart.Rect.Top);
            box.Fill = new Fill(Color.Red);
            // Use the BehindAxis zorder to draw the highlight beneath the grid lines 
            box.ZOrder = ZOrder.D_BehindAxis;
            myPane.GraphObjList.Add(box);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
               Color.Black, 45.0F);

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
        }
        private void CreateLunarChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.Title.Text = "";
            // Set the title and axis labels
            myPane.Title.Text = comboBoxMarket.Text;
            myPane.XAxis.Title.Text = "Lunar phase";
            myPane.YAxis.Title.Text = "% Change";
            // Set the XAxis to ordinal type
            myPane.XAxis.Type = AxisType.LinearAsOrdinal;

            // Generate some curve data from the Sine function
            PointPairList list = new PointPairList();

            for (int i = 0; i < 8; i++)
            {
                double x = i;
                double y = _marketLunarHistogramAverage[i];
                list.Add(x, y);
                System.Windows.Forms.Application.DoEvents();
            }

            myBar = myPane.AddBar("", list, Color.Black);

            // Draw a box item to highlight a value range 
            BoxObj box = new BoxObj(SupportClass.GetLunarPhase(DateTime.Now), myPane.Chart.Rect.Top, 1, myPane.Chart.Rect.Bottom - myPane.Chart.Rect.Top);

            box.Fill = new Fill(Color.Red);
            // Use the BehindAxis zorder to draw the highlight beneath the grid lines 
            box.ZOrder = ZOrder.D_BehindAxis;
            myPane.GraphObjList.Add(box);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
               Color.Black, 45.0F);

            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
        }
        #endregion

        private void button5_Click(object sender, EventArgs e)
        {
            GetBuyRecommendationsB();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GetSellRecommendationsB();
        }

        private void backgroundWorkerRecommendationsBuyB_DoWork(object sender, DoWorkEventArgs e)
        {
            _instrumentRecommendationsBuySymbolB = _marketRecommendationsBuyB._instruments[_counterRecommendationsBuyB].ToString();
            _instrumentRecommendationsBuyNameB = _marketRecommendationsBuyB._instrumentNames[_counterRecommendationsBuyB].ToString();
            _strategyTester.BackTest(_engineRecommendationsBuyB, _marketRecommendationsBuyB, ref _instrumentRecommendationsBuySymbolB);
        }

        private void backgroundWorkerRecommendationsBuyB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //double minimum = (Properties.Settings.Default.BrokerCommision * 2) * 100;  //arbitrary BS
            GeneticStrategy trader = (GeneticStrategy)_engineRecommendationsBuyB.Trader.Agents[0];
            textBox1.AppendText(DateTime.Now + " : " + _instrumentRecommendationsBuySymbolB + " : " + _instrumentRecommendationsBuyNameB + " : " + trader.EntryIndicator.GetDirection() + " : " + trader.ExitIndicator.GetDirection() + "\r\n");
            if (trader.EntryIndicator.GetDirection() == 1 && trader.ExitIndicator.GetDirection() >= 0)
            {
                DataRow newrow = _datasetRecommendationsBuyConsensusB.NewRow();
                newrow["instrument"] = _instrumentRecommendationsBuySymbolB;
                newrow["name"] = _instrumentRecommendationsBuyNameB;
                newrow["date"] = ((TechnicalData)(trader.Market.TechnicalData[trader.Market.TechnicalData.Count - 1])).Date;
                _datasetRecommendationsBuyConsensusB.Rows.Add(newrow);
            }
            if (_counterRecommendationsBuyB < _marketRecommendationsBuyB._instruments.Count - 1)
            {
                _counterRecommendationsBuyB++;
                backgroundWorkerRecommendationsBuyB.RunWorkerAsync();
            }
            else
            {
                if (_counterRecommendationsBuyStrategyRankB == 0)
                    _buyRecommendationsPercentageOfMarketB = _datasetRecommendationsBuyConsensusB.Rows.Count;
                //double cash = (double.Parse(_datasetPortfolioB.Tables["account"].Rows[0].ItemArray[1].ToString()) - (Properties.Settings.Default.BrokerCommision * _datasetRecommendationsBuyConsensusB.Rows.Count)) / _datasetRecommendationsBuyConsensusB.Rows.Count;
                //if ((_counterRecommendationsBuyStrategyRankB < (numericUpDownTradeIndicatorsBuy.Value - 1)) && (_datasetRecommendationsBuyConsensusB.Rows.Count > 0) && (cash < minimum))
                if ((_counterRecommendationsBuyStrategyRankB < (numericUpDownTradeIndicatorsBuy.Value - 1)) && (_datasetRecommendationsBuyConsensusB.Rows.Count > 0))
                {
                    _currentRecommendationsBuyInstrumentsB.Clear();
                    for (int x = 0; x < _datasetRecommendationsBuyConsensusB.Rows.Count; x++)
                    {
                        _currentRecommendationsBuyInstrumentsB.Add(_datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[0].ToString(), _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[1].ToString());
                    }
                    _marketRecommendationsBuyB.Set(_currentRecommendationsBuyInstrumentsB);
                    _counterRecommendationsBuyB = 0;
                    _counterRecommendationsBuyStrategyRankB++;

                    switch (_counterRecommendationsBuyStrategyRankB)
                    {
                        case 1:
                            _strategyRecommendationsBuyStrategyNameB = _datasetModelGenetic5Year.Rows[0].ItemArray[1].ToString();
                            break;
                        case 2:
                            _strategyRecommendationsBuyStrategyNameB = _datasetModelGenetic3Year.Rows[0].ItemArray[1].ToString();
                            break;
                        case 3:
                            _strategyRecommendationsBuyStrategyNameB = _datasetModelGenetic1Year.Rows[0].ItemArray[1].ToString();
                            break;
                        case 4:
                            _strategyRecommendationsBuyStrategyNameB = _datasetModelGenetic6Month.Rows[0].ItemArray[1].ToString();
                            break;
                        case 5:
                            _strategyRecommendationsBuyStrategyNameB = _datasetModelGenetic3Month.Rows[0].ItemArray[1].ToString();
                            break;
                        case 6:
                            _strategyRecommendationsBuyStrategyNameB = _datasetModelGenetic1Month.Rows[0].ItemArray[1].ToString();
                            break;
                    }

                    textBox4.Text = _strategyRecommendationsBuyStrategyNameB;
                    textBox3.Text = _counterRecommendationsBuyStrategyRankB.ToString();
                    trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsBuyStrategyNameB);
                    _engineRecommendationsBuyB.removeTraders();
                    _engineRecommendationsBuyB.register(trader);
                    _datasetRecommendationsBuyConsensusB.Rows.Clear();
                    textBox1.AppendText(DateTime.Now + " : ---\r\n");
                    backgroundWorkerRecommendationsBuyB.RunWorkerAsync();
                }
                else
                {
                    for (int x = 0; x < _datasetRecommendationsBuyConsensusB.Rows.Count; x++)
                    {
                        textBox1.AppendText(DateTime.Now + " : : " + _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[0].ToString() + "\r\n");
                        for (int y = 0; y < _datasetRisk.Rows.Count; y++)
                        {
                            if (_datasetRisk.Rows[y].ItemArray[0].ToString() == _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[0].ToString())
                            {
                                if (checkBox6.Checked == true)
                                {
                                    if ((double)_datasetRisk.Rows[y].ItemArray[4] <= 1 && (double)_datasetModelGenetic1Month.Rows[0].ItemArray[2] >= 0)
                                    {
                                        DataRow newrow = _datasetRecommendationsBuyB.NewRow();
                                        newrow["symbol"] = _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[0].ToString();
                                        newrow["name"] = _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[1].ToString();
                                        newrow["date"] = DateTime.Parse(_datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[2].ToString());
                                        newrow["return"] = _datasetRisk.Rows[y].ItemArray[14];
                                        _datasetRecommendationsBuyB.Rows.Add(newrow);
                                    }
                                    else
                                    {
                                        textBox1.AppendText(DateTime.Now + " : Constraining: " + _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[0].ToString() + "\r\n");
                                    }
                                }
                                else
                                {
                                    DataRow newrow = _datasetRecommendationsBuyB.NewRow();
                                    newrow["symbol"] = _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[0].ToString();
                                    newrow["name"] = _datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[1].ToString();
                                    newrow["date"] = DateTime.Parse(_datasetRecommendationsBuyConsensusB.Rows[x].ItemArray[2].ToString());
                                    newrow["return"] = _datasetRisk.Rows[y].ItemArray[14];
                                    _datasetRecommendationsBuyB.Rows.Add(newrow);
                                }
                            }
                        }
                    }
                    _datasetRecommendationsBuyB = SupportClass.FilterSortDataTable(_datasetRecommendationsBuyB, "", "return", 1);
                    dataGrid24.DataSource = _datasetRecommendationsBuyB;
                    //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                    //{
                    textBox1.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsBuyB + "\r\n");
                    try
                    {
                        FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsBuyB, FileMode.Create, FileAccess.Write);
                        StreamWriter s = new StreamWriter(fs);
                        _datasetRecommendationsBuyB.WriteXml(s);
                        s.Close();
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //}
                    textBox1.AppendText(DateTime.Now + " : " + _buyRecommendationsPercentageOfMarketB.ToString() + "\r\n");
                    textBox1.AppendText(DateTime.Now + " : Simulation completed\r\n");
                    button5.Enabled = true;
                }
            }
        }

        private void backgroundWorkerRecommendationsSellB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            _instrumentRecommendationsSellSymbolB = _marketRecommendationsSellB._instruments[_counterRecommendationsSellB].ToString();
            _instrumentRecommendationsSellNameB = _marketRecommendationsSellB._instrumentNames[_counterRecommendationsSellB].ToString();
            _strategyTester.BackTest(_engineRecommendationsSellB, _marketRecommendationsSellB, ref _instrumentRecommendationsSellSymbolB);
        }
        private void backgroundWorkerRecommendationsSellB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GeneticStrategy trader = (GeneticStrategy)_engineRecommendationsSellB.Trader.Agents[0];
            textBox2.AppendText(DateTime.Now + " : " + _instrumentRecommendationsSellSymbolB + " : " + _instrumentRecommendationsSellNameB + " : " + trader.EntryIndicator.GetDirection() + " : " + trader.ExitIndicator.GetDirection() + "\r\n");
            if (trader.EntryIndicator.GetDirection() <= 0 && trader.ExitIndicator.GetDirection() == -1)
            {
                try
                {
                    DataRow newrow = _datasetRecommendationsSellConsensusB.NewRow();
                    newrow["instrument"] = _instrumentRecommendationsSellSymbolB;
                    newrow["name"] = _instrumentRecommendationsSellNameB;
                    newrow["date"] = ((TechnicalData)(trader.Market.TechnicalData[trader.Market.TechnicalData.Count - 1])).Date;
                    _datasetRecommendationsSellConsensusB.Rows.Add(newrow);
                }
                catch (Exception ex)
                {
                    textBox2.AppendText(DateTime.Now + " : " + ex.ToString() + " \r\n");
                }
            }
            if (_counterRecommendationsSellB < _marketRecommendationsSellB._instruments.Count - 1)
            {
                _counterRecommendationsSellB++;
                backgroundWorkerRecommendationsSellB.RunWorkerAsync();
            }
            else
            {
                if (_counterRecommendationsSellStrategyRankB == 0)
                    _sellRecommendationsPercentageOfMarketB = _datasetRecommendationsSellConsensusB.Rows.Count;


                if ((_counterRecommendationsSellStrategyRankB < (numericUpDownTradeIndicatorsSell.Value - 1)) && (_datasetRecommendationsSellConsensusB.Rows.Count > 0))
                {
                    _currentRecommendationsSellInstrumentsB.Clear();
                    for (int x = 0; x < _datasetRecommendationsSellConsensusB.Rows.Count; x++)
                    {
                        _currentRecommendationsSellInstrumentsB.Add(_datasetRecommendationsSellConsensusB.Rows[x].ItemArray[0].ToString(), _datasetRecommendationsSellConsensusB.Rows[x].ItemArray[1].ToString());
                    }
                    _marketRecommendationsSellB.Set(_currentRecommendationsSellInstrumentsB);
                    _counterRecommendationsSellB = 0;
                    _counterRecommendationsSellStrategyRankB++;

                    switch (_counterRecommendationsSellStrategyRankB)
                    {
                        case 1:
                            _strategyRecommendationsSellStrategyNameB = _datasetModelGenetic5Year.Rows[0].ItemArray[1].ToString();
                            break;
                        case 2:
                            _strategyRecommendationsSellStrategyNameB = _datasetModelGenetic3Year.Rows[0].ItemArray[1].ToString();
                            break;
                        case 3:
                            _strategyRecommendationsSellStrategyNameB = _datasetModelGenetic1Year.Rows[0].ItemArray[1].ToString();
                            break;
                        case 4:
                            _strategyRecommendationsSellStrategyNameB = _datasetModelGenetic6Month.Rows[0].ItemArray[1].ToString();
                            break;
                        case 5:
                            _strategyRecommendationsSellStrategyNameB = _datasetModelGenetic3Month.Rows[0].ItemArray[1].ToString();
                            break;
                        case 6:
                            _strategyRecommendationsSellStrategyNameB = _datasetModelGenetic1Month.Rows[0].ItemArray[1].ToString();
                            break;
                    }

                    textBox9.Text = _strategyRecommendationsSellStrategyNameB;
                    textBox6.Text = _counterRecommendationsSellStrategyRankB.ToString();
                    trader = new GeneticStrategy(ref _pluginService, _strategyRecommendationsSellStrategyNameB);
                    _engineRecommendationsSellB.removeTraders();
                    _engineRecommendationsSellB.register(trader);
                    _datasetRecommendationsSellConsensusB.Rows.Clear();
                    textBox2.AppendText(DateTime.Now + " : ---\r\n");
                    backgroundWorkerRecommendationsSellB.RunWorkerAsync();
                }
                else
                {
                    for (int x = 0; x < _datasetRecommendationsSellConsensusB.Rows.Count; x++)
                    {
                        DataRow newrow = _datasetRecommendationsSellB.NewRow();
                        newrow["symbol"] = _datasetRecommendationsSellConsensusB.Rows[x].ItemArray[0].ToString();
                        newrow["name"] = _datasetRecommendationsSellConsensusB.Rows[x].ItemArray[1].ToString();
                        newrow["date"] = DateTime.Parse(_datasetRecommendationsSellConsensusB.Rows[x].ItemArray[2].ToString());
                        _datasetRecommendationsSellB.Rows.Add(newrow);
                    }
                    _datasetRecommendationsSellB = SupportClass.FilterSortDataTable(_datasetRecommendationsSellB, "", "date", 1);
                    dataGrid25.DataSource = _datasetRecommendationsSellB;
                    //if (checkBoxForecastRecommendationsSaveReports.Checked == true)
                    //{
                    textBox2.AppendText(DateTime.Now + " : Generating report - " + _fileRecommendationsSellB + "\r\n");
                    try
                    {
                        FileStream fs = new FileStream(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryData + _fileRecommendationsSellB, FileMode.Create, FileAccess.Write);
                        StreamWriter s = new StreamWriter(fs);
                        _datasetRecommendationsSellB.WriteXml(s);
                        s.Close();
                        fs.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //}
                    textBox2.AppendText(DateTime.Now + " : " + _sellRecommendationsPercentageOfMarketB.ToString() + "\r\n");
                    textBox2.AppendText(DateTime.Now + " : Simulation completed\r\n");
                    button4.Enabled = true;
                }
            }
        }

        private void AddToRichTextBox(ref RichTextBox box, ref int count, string text)
        {
            if (count > 1000)
            {
                box.SelectionStart = 0;
                box.SelectionLength = box.Text.IndexOf("\n", 0) + 1;
                box.SelectedText = "";
            }
            else
                count++;
            box.AppendText(text);
            box.ScrollToCaret();
        }
        private void AddToTextBox(ref TextBox box, ref int count, string text)
        {
            if (count > 1000)
            {
                box.SelectionStart = 0;
                box.SelectionLength = box.Text.IndexOf("\n", 0) + 1;
                box.SelectedText = "";
            }
            else
                count++;
            box.AppendText(text);
            box.ScrollToCaret();
        }
    }
}