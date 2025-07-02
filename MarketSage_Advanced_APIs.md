# MarketSage Advanced APIs Documentation

## Overview

This document covers advanced APIs and components in the MarketSage platform that extend the core functionality for data acquisition, indicator composition, backtesting, and execution engine management.

---

## Table of Contents

1. [CompoundIndicator System](#compound-indicator-system)
2. [Data Acquisition APIs](#data-acquisition-apis)
3. [Execution Engine](#execution-engine)
4. [Strategy Testing Framework](#strategy-testing-framework)
5. [Market Data Management](#market-data-management)
6. [Advanced Examples](#advanced-examples)

---

## Compound Indicator System

### CompoundIndicator Class

Combines multiple indicators to create composite trading signals using voting mechanisms.

```csharp
namespace MarketSage.Library
{
    public class CompoundIndicator
    {
        // Constructors
        public CompoundIndicator(ArrayList indicators, string name)
        public CompoundIndicator(IIndicator indicator)
        
        // Properties
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public string Version { get; }
        public int Period { get; set; }
        
        // Core Methods
        public string GetName()
        public bool IsReady()
        public int GetDirection()
        public bool IsBuy()
        public bool IsHold()
        public bool IsSell()
        public void AddData(TechnicalData data)
        public void Initialize()
        
        // Voting System Methods
        public int GetCommitee()            // Returns number of indicators
        public double Getconsensus()        // Returns consensus percentage
    }
}
```

**Key Features:**
- **Voting Mechanism**: Combines signals from multiple indicators
- **Consensus Building**: Determines overall signal based on majority vote
- **Signal Strength**: Provides consensus percentage for signal confidence

**Usage Examples:**

```csharp
// Create compound indicator with multiple technical indicators
var indicators = new ArrayList();
indicators.Add(new SMA(20));
indicators.Add(new RSI(14));
indicators.Add(new MACD(12, 26, 9));

var compound = new CompoundIndicator(indicators, "TrendFollowingSystem");
compound.Initialize();

// Feed data and get composite signals
foreach (var dataPoint in marketData)
{
    compound.AddData(dataPoint);
    
    if (compound.IsReady())
    {
        int direction = compound.GetDirection();  // -1, 0, 1
        double consensus = compound.Getconsensus(); // 0.0 to 1.0
        int committeeSize = compound.GetCommitee();
        
        Console.WriteLine($"Signal: {direction}, Consensus: {consensus:P}, Indicators: {committeeSize}");
        
        if (compound.IsBuy() && consensus > 0.6) // Require 60% consensus
        {
            // Execute buy with high confidence
        }
    }
}

// Single indicator compound (wrapper pattern)
var simpleCompound = new CompoundIndicator(new SMA(50));
```

---

## Data Acquisition APIs

### DataSource Class

Comprehensive data acquisition system supporting multiple data providers and formats.

```csharp
namespace MarketSage.Library
{
    public class DataSource
    {
        // Constructors
        public DataSource()
        public DataSource(string pathMarkets, string pathWork)
        public DataSource(string pathMarkets, string pathWork, string pathLogs, string market)
        
        // Market Symbol Management
        public void GetMarketSymbolsViaNASDAQ(string market)
        public void GetMarketSymbolsViaNASDAQTrader(string market)
        public void GetMarketSymbolsViaNASDAQTrader()
        public void GetIndexSymbolsViaNASDAQ(string index)
        public void GetIndexSymbolsViaNASDAQ()
        
        // Historical Data Retrieval
        public string GetHistoricMarketDataViaYAHOO(BackgroundWorker worker, DoWorkEventArgs e, string marketname)
        public string GetHistoricMarketDataViaYAHOO(string marketname)
        
        // Real-time Data (via OTFeed)
        public void Demo()  // OTFeed demo connection
        
        // Utility Methods
        public byte[] Decompress(byte[] body)
        
        // Properties
        public string _log { get; set; }  // Operation log
    }
}
```

**Data Sources Supported:**
- **NASDAQ**: Direct symbol and market data
- **Yahoo Finance**: Historical market data
- **OTFeed**: Real-time market data feed
- **CSV Files**: Custom data import

**Usage Examples:**

```csharp
// Initialize data source with paths
var dataSource = new DataSource("data/markets/", "data/work/", "logs/", "NASDAQ");

// Download market symbols from NASDAQ
dataSource.GetMarketSymbolsViaNASDAQTrader("NASDAQ");
dataSource.GetMarketSymbolsViaNASDAQTrader("NYSE");
dataSource.GetMarketSymbolsViaNASDAQTrader("AMEX");

// Get historical data from Yahoo Finance
string result = dataSource.GetHistoricMarketDataViaYAHOO("AAPL");

// Check operation log
Console.WriteLine(dataSource._log);

// Background worker example for large datasets
BackgroundWorker worker = new BackgroundWorker();
DoWorkEventArgs e = new DoWorkEventArgs(null);
string result = dataSource.GetHistoricMarketDataViaYAHOO(worker, e, "SPY");
```

**Supported Markets:**
- NASDAQ
- NYSE  
- AMEX
- OTCBB
- NASDAQ.FUNDS

---

## Execution Engine

### Engine Class

Core execution engine that orchestrates trading strategies, market data processing, and order execution.

```csharp
namespace MarketSage.Library
{
    public class Engine
    {
        // Constructors
        public Engine(double initialCashBalance, double commission)
        public Engine(double initialCashBalance, double commission, DateTime startDate, DateTime endDate)
        public Engine(Trader trader, DateTime startDate, DateTime endDate)
        
        // Properties
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Instrument Market { get; set; }
        public Observer Observer { get; set; }
        public Trader Trader { get; }
        
        // Strategy Management
        public void register(AbstractStrategy trader)    // Add strategy
        public void removeTraders()                      // Remove all strategies
        
        // Execution Control
        public void Start()                              // Initialize and start
        public void Stop()                               // Stop execution
        public void Run()                                // Run complete backtest
        public void Iterate()                            // Single iteration
        public void Tally(int count)                     // Update progress
        
        // Event Management
        public CalanderEvent GetEvent(DateTime date)     // Get calendar event type
    }
}
```

**Calendar Events:**
- `DAY_EVENT` - Daily processing
- `MONTH_EVENT` - Monthly processing  
- `YEAR_EVENT` - Annual processing

**Usage Examples:**

```csharp
// Create engine with initial parameters
var engine = new Engine(10000.0, 7.0, 
                       new DateTime(2020, 1, 1), 
                       new DateTime(2023, 12, 31));

// Add trading strategies
var strategy1 = new MyTrendStrategy();
var strategy2 = new MyMomentumStrategy();
engine.register(strategy1);
engine.register(strategy2);

// Set up market data
var instrument = new Instrument();
instrument.Name = "AAPL";
// Load historical data into instrument...
engine.Market = instrument;

// Set up observer for monitoring
var observer = new Observer();
engine.Observer = observer;

// Run complete backtest
engine.Start();
engine.Run();
engine.Stop();

// Manual iteration control
engine.Start();
while (engine.Market.HasNext())
{
    engine.Iterate();
    
    // Custom processing per iteration
    var currentDate = engine.Market.Date;
    var accountValue = engine.Trader.Account.AccountValue;
    Console.WriteLine($"{currentDate}: Account Value = {accountValue:C}");
}
engine.Stop();
```

---

## Strategy Testing Framework

### StrategyTester Class

Comprehensive backtesting framework with fitness calculation, progress reporting, and results management.

```csharp
namespace MarketSage.Library
{
    public class StrategyTester
    {
        // Constructors
        public StrategyTester()
        public StrategyTester(string pathLogs)
        
        // Core Backtesting Methods
        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, 
                           string log, Engine engine, Market market, 
                           ref string result, ref DataTable pool, 
                           string file, ref bool win)
                           
        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, 
                           string log, Engine engine, Market market, 
                           ref string result, ref DataTable pool, 
                           string file, ref bool win, double baseline)
                           
        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, 
                           string log, Engine engine, Market market, 
                           ref string result, ref DataTable pool, 
                           string file, ref bool win, bool disposition)
                           
        // Simplified Backtesting
        public void BackTest(Engine engine, Market market, ref string instrument, ref DataTable pool)
        public void BackTest(Engine engine, Market market, ref string instrument)
        public double BackTest(Engine engine, Market market, bool disposition, ref SortedList list)
        
        // Properties
        public ArrayList date { get; set; }              // Date series for charting
        public ArrayList value { get; set; }             // Value series for charting
        public Dictionary<int, GraphObj> graphObjs       // Chart annotations
    }
}
```

**Key Features:**
- **Multi-Instrument Testing**: Tests strategies across multiple instruments
- **Fitness Calculation**: Sophisticated scoring algorithm
- **Progress Reporting**: Background worker integration
- **Result Persistence**: Automatic saving of winning strategies
- **Performance Metrics**: Comprehensive statistics

**Usage Examples:**

```csharp
// Initialize strategy tester
var tester = new StrategyTester("logs/");

// Set up background worker for progress reporting
BackgroundWorker worker = new BackgroundWorker();
worker.WorkerReportsProgress = true;
worker.WorkerSupportsCancellation = true;
worker.ProgressChanged += (s, e) => {
    Console.WriteLine($"Progress: {e.ProgressPercentage}%");
};

// Create data tables for results
DataTable winnersPool = new DataTable();
winnersPool.Columns.Add("fitness", typeof(double));
winnersPool.Columns.Add("genome", typeof(string));
winnersPool.Columns.Add("profit", typeof(double));
winnersPool.Columns.Add("transactions", typeof(int));
winnersPool.Columns.Add("bars", typeof(double));
winnersPool.Columns.Add("inception", typeof(DateTime));

// Set up engine and market
var engine = new Engine(10000.0, 7.0);
var market = new Market(); // Load with instruments
var strategy = new TestStrategy();
engine.register(strategy);

// Run backtest
string result = "";
bool hasWinner = false;
DoWorkEventArgs args = new DoWorkEventArgs(null);

tester.BackTest(worker, args, "test.log", engine, market, 
               ref result, ref winnersPool, "winners.xml", ref hasWinner);

// Check results
if (hasWinner)
{
    Console.WriteLine("Found winning strategy!");
    Console.WriteLine(result);
    
    // Access performance data for charting
    var dates = tester.date;
    var values = tester.value;
    // Create performance chart...
}

// Set baseline requirement
double minimumProfit = 1000.0;
tester.BackTest(worker, args, "test.log", engine, market, 
               ref result, ref winnersPool, "winners.xml", 
               ref hasWinner, minimumProfit);
```

**Fitness Calculation:**
The fitness function considers multiple factors:
- Profit/Loss amount
- Number of transactions
- Time in market
- Risk-adjusted returns
- Consistency across instruments

---

## Market Data Management

### Market Class

Manages collections of instruments and provides data access methods.

```csharp
namespace MarketSage.Library
{
    public class Market
    {
        // Properties
        public ArrayList _instruments { get; set; }      // List of instrument symbols
        public bool random { get; set; }                 // Random instrument selection
        
        // Methods
        public ArrayList GetTechnicalData(string instrument)
        public void AddInstrument(string symbol)
        public void RemoveInstrument(string symbol)
        public int GetInstrumentCount()
    }
}
```

### Instrument Class (Extended)

```csharp
namespace MarketSage.Library
{
    public class Instrument
    {
        // Navigation
        public bool HasNext()
        public bool NextToLast()
        public void Next()
        public void Initialize()
        
        // Data Access
        public TechnicalData GetCurrent()
        public ArrayList TechnicalData { get; }
        
        // Properties
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; }
        public double AdjClose { get; }
        public double AdjHigh { get; }
        public double AdjLow { get; }
    }
}
```

---

## Advanced Examples

### Complete Strategy Development Workflow

```csharp
public class AdvancedTradingSystem
{
    public void DevelopAndTestStrategy()
    {
        // 1. Data Acquisition
        var dataSource = new DataSource("data/", "work/", "logs/");
        dataSource.GetMarketSymbolsViaNASDAQTrader("NASDAQ");
        dataSource.GetHistoricMarketDataViaYAHOO("QQQ");
        
        // 2. Create Compound Indicator System
        var trendIndicators = new ArrayList();
        trendIndicators.Add(new SMA(20));
        trendIndicators.Add(new SMA(50));
        trendIndicators.Add(new EMA(12));
        
        var momentumIndicators = new ArrayList();
        momentumIndicators.Add(new RSI(14));
        momentumIndicators.Add(new MACD(12, 26, 9));
        momentumIndicators.Add(new CCI(20));
        
        var entrySystem = new CompoundIndicator(trendIndicators, "TrendEntry");
        var exitSystem = new CompoundIndicator(momentumIndicators, "MomentumExit");
        
        // 3. Create Advanced Strategy
        var strategy = new AdvancedCompoundStrategy(entrySystem, exitSystem);
        
        // 4. Set up Testing Framework
        var engine = new Engine(50000.0, 7.0, 
                               new DateTime(2020, 1, 1), 
                               new DateTime(2023, 12, 31));
        engine.register(strategy);
        
        var market = new Market();
        // Load market with multiple instruments...
        
        var tester = new StrategyTester("logs/");
        
        // 5. Run Comprehensive Backtest
        BackgroundWorker worker = new BackgroundWorker();
        worker.WorkerReportsProgress = true;
        
        DataTable results = CreateResultsTable();
        string result = "";
        bool hasWinner = false;
        DoWorkEventArgs args = new DoWorkEventArgs(null);
        
        tester.BackTest(worker, args, "advanced_test.log", 
                       engine, market, ref result, ref results, 
                       "winners.xml", ref hasWinner, 5000.0); // $5K minimum
        
        // 6. Analyze Results
        if (hasWinner)
        {
            GeneratePerformanceReport(tester.date, tester.value);
            OptimizeParameters(results);
        }
    }
    
    private DataTable CreateResultsTable()
    {
        var table = new DataTable();
        table.Columns.Add("fitness", typeof(double));
        table.Columns.Add("genome", typeof(string));
        table.Columns.Add("profit", typeof(double));
        table.Columns.Add("transactions", typeof(int));
        table.Columns.Add("bars", typeof(double));
        table.Columns.Add("inception", typeof(DateTime));
        table.Columns.Add("sharpe_ratio", typeof(double));
        table.Columns.Add("max_drawdown", typeof(double));
        return table;
    }
}

public class AdvancedCompoundStrategy : AbstractStrategy
{
    private CompoundIndicator _entryCompound;
    private CompoundIndicator _exitCompound;
    private double _positionSizing = 0.25; // 25% of capital per trade
    
    public AdvancedCompoundStrategy(CompoundIndicator entry, CompoundIndicator exit)
    {
        _entryCompound = entry;
        _exitCompound = exit;
    }
    
    public override void Initialize()
    {
        _entryCompound.Initialize();
        _exitCompound.Initialize();
        EntryIndicatorName = _entryCompound.GetName();
        ExitIndicatorName = _exitCompound.GetName();
    }
    
    public override void Update()
    {
        var currentData = Market.GetCurrent();
        if (currentData == null) return;
        
        _entryCompound.AddData(currentData);
        _exitCompound.AddData(currentData);
        
        if (!_entryCompound.IsReady() || !_exitCompound.IsReady()) return;
        
        double position = GetPosition(Market.Name);
        double entryConsensus = _entryCompound.Getconsensus();
        double exitConsensus = _exitCompound.Getconsensus();
        
        // Entry logic with consensus threshold
        if (position == 0 && _entryCompound.IsBuy() && entryConsensus >= 0.7)
        {
            double cashToRisk = GetCash() * _positionSizing;
            AddCashOrder(Market.Name, cashToRisk);
        }
        
        // Exit logic with consensus threshold
        if (position > 0 && _exitCompound.IsSell() && exitConsensus >= 0.6)
        {
            AddQuantityOrder(Market.Name, -position);
        }
        
        // Risk management: Stop loss at 10%
        if (position > 0)
        {
            double unrealizedPnL = (currentData.AdjClose - GetLastTransaction().Amount / GetLastTransaction().Quantity) 
                                  * position;
            if (unrealizedPnL < -GetValue() * 0.1) // 10% stop loss
            {
                AddQuantityOrder(Market.Name, -position);
            }
        }
    }
}
```

### Multi-Strategy Portfolio Testing

```csharp
public class MultiStrategyTester
{
    public void TestPortfolioOfStrategies()
    {
        var strategies = new List<AbstractStrategy>
        {
            new TrendFollowingStrategy(),
            new MeanReversionStrategy(), 
            new MomentumStrategy(),
            new BreakoutStrategy()
        };
        
        var markets = new List<string> { "QQQ", "SPY", "IWM", "DIA" };
        var tester = new StrategyTester("portfolio_logs/");
        
        var portfolioResults = new DataTable();
        // Set up results table...
        
        foreach (var strategy in strategies)
        {
            foreach (var marketSymbol in markets)
            {
                var engine = new Engine(25000.0, 7.0); // Allocate $25K per strategy
                engine.register(strategy);
                
                var market = LoadMarketData(marketSymbol);
                engine.Market = market;
                
                // Test individual strategy on individual market
                string result = "";
                var strategyResults = new DataTable();
                tester.BackTest(engine, market, ref marketSymbol, ref strategyResults);
                
                // Aggregate results for portfolio analysis
                AggregateResults(portfolioResults, strategyResults, strategy.GetType().Name, marketSymbol);
            }
        }
        
        // Analyze portfolio performance
        AnalyzePortfolioResults(portfolioResults);
    }
}
```

---

## Best Practices for Advanced APIs

### 1. CompoundIndicator Usage
- Use consensus thresholds to filter weak signals
- Balance indicator diversity (trend, momentum, volume)
- Monitor individual indicator contributions
- Implement indicator weighting based on historical performance

### 2. Data Source Management
- Implement error handling for network failures
- Cache downloaded data to avoid repeated requests
- Validate data quality before use
- Use background workers for large data downloads

### 3. Engine Configuration
- Set appropriate date ranges for testing periods
- Use observers for detailed performance monitoring
- Implement progress reporting for long-running backtests
- Consider memory usage with large datasets

### 4. Strategy Testing
- Use multiple fitness criteria beyond just profit
- Implement walk-forward analysis
- Test across different market conditions
- Consider transaction costs and slippage

---

This advanced documentation provides the tools needed for sophisticated trading system development using the MarketSage platform's full capabilities.