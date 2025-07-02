# MarketSage API Documentation

## Overview

MarketSage is a comprehensive trading platform built in C#/.NET that provides technical analysis, strategy backtesting, portfolio management, and plugin-based indicator system. The platform consists of several core modules:

- **MarketSage** - Main Windows Forms application
- **MarketSage.Library** - Core business logic and trading components
- **MarketSage.Plugin.Core** - Built-in technical indicators
- **MarketSage.Plugin.Experimental** - Experimental indicators

---

## Table of Contents

1. [Core Interfaces](#core-interfaces)
2. [Data Structures](#data-structures)
3. [Technical Indicators](#technical-indicators)
4. [Trading Components](#trading-components)
5. [Strategy Framework](#strategy-framework)
6. [Portfolio Management](#portfolio-management)
7. [Plugin System](#plugin-system)
8. [Reporting](#reporting)
9. [Usage Examples](#usage-examples)

---

## Core Interfaces

### IIndicator Interface

The base interface for all technical indicators in the system.

```csharp
namespace MarketSage.Library
{
    public interface IIndicator
    {
        // Properties
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
        
        // Methods
        string GetName();
        bool IsReady();
        int GetDirection();
        bool IsBuy();
        bool IsHold();
        bool IsSell();
        void AddData(TechnicalData data);
        void Initialize();
    }
}
```

**Properties:**
- `Name` - Human-readable name of the indicator
- `Description` - Description of what the indicator does
- `Author` - Author of the indicator
- `Version` - Version number

**Methods:**
- `GetName()` - Returns the technical name/identifier
- `IsReady()` - Returns true when indicator has enough data to generate signals
- `GetDirection()` - Returns signal direction (-1: sell, 0: hold, 1: buy)
- `IsBuy()` - Returns true when indicator signals buy
- `IsHold()` - Returns true when indicator signals hold
- `IsSell()` - Returns true when indicator signals sell
- `AddData(TechnicalData)` - Adds new market data to the indicator
- `Initialize()` - Initializes the indicator

---

## Data Structures

### TechnicalData Class

Core data structure representing market data for a single time period.

```csharp
namespace MarketSage.Library
{
    public class TechnicalData
    {
        public TechnicalData(DateTime date, double open, double high, 
                           double low, double close, long volume, double adjclose)
        
        // Properties
        public DateTime Date { get; }
        public double Open { get; }
        public double AdjOpen { get; }
        public double High { get; }
        public double AdjHigh { get; }
        public double Low { get; }
        public double AdjLow { get; }
        public double Close { get; }
        public double AdjClose { get; }
        public long Volume { get; }
        public long AdjVolume { get; }
    }
}
```

**Constructor:**
- `TechnicalData(date, open, high, low, close, volume, adjclose)` - Creates new market data entry

**Properties:**
- `Date` - Trading date
- `Open/AdjOpen` - Opening price (raw and adjusted)
- `High/AdjHigh` - High price (raw and adjusted)
- `Low/AdjLow` - Low price (raw and adjusted)  
- `Close/AdjClose` - Closing price (raw and adjusted)
- `Volume/AdjVolume` - Trading volume (raw and adjusted)

### Transaction Class

Represents a single trading transaction.

```csharp
namespace MarketSage.Library
{
    public class Transaction
    {
        public Transaction(DateTime date, string instrument, double quantity, double amount)
        
        // Properties
        public DateTime Date { get; }
        public string Instrument { get; }
        public double Quantity { get; }
        public double Amount { get; }
    }
}
```

### Order Class

Represents a trading order.

```csharp
namespace MarketSage.Library
{
    public class Order
    {
        public Order(string instrument, bool isQuantityOrder, double value)
        
        // Properties
        public string Instrument { get; }
        public bool IsQuantityOrder { get; }
        public double Value { get; }
    }
}
```

---

## Technical Indicators

### Simple Moving Average (SMA)

Calculates the simple moving average over a specified period.

```csharp
namespace MarketSage.Plugin
{
    public class SMA : IIndicator
    {
        public SMA()                    // Default 20-period SMA
        public SMA(int period)          // Custom period SMA
        
        public int Period { get; set; }
    }
}
```

**Usage:**
```csharp
// Create 20-period SMA
var sma20 = new SMA(20);
sma20.AddData(technicalData);
if (sma20.IsReady() && sma20.IsBuy()) {
    // Execute buy signal
}
```

### Exponential Moving Average (EMA)

Calculates exponential moving average with more weight on recent prices.

```csharp
namespace MarketSage.Plugin
{
    public class EMA : IIndicator
    {
        public EMA()
        public EMA(int period)
        
        public int Period { get; set; }
    }
}
```

### Relative Strength Index (RSI)

Momentum oscillator measuring speed and change of price movements.

```csharp
namespace MarketSage.Plugin
{
    public class RSI : IIndicator
    {
        public RSI()
        public RSI(int period)
        
        public int Period { get; set; }
    }
}
```

### Moving Average Convergence Divergence (MACD)

Trend-following momentum indicator.

```csharp
namespace MarketSage.Plugin
{
    public class MACD : IIndicator
    {
        public MACD()
        public MACD(int fastPeriod, int slowPeriod, int signalPeriod)
        
        public int FastPeriod { get; set; }
        public int SlowPeriod { get; set; }
        public int SignalPeriod { get; set; }
    }
}
```

### Other Available Indicators

**Trend Indicators:**
- `WMA` - Weighted Moving Average
- `LR` - Linear Regression
- `Aroon` - Aroon indicator
- `AroonO` - Aroon Oscillator

**Momentum Indicators:**
- `ROC` - Rate of Change
- `CCI` - Commodity Channel Index
- `WillR` - Williams %R

**Volume Indicators:**
- `OBV` - On Balance Volume
- `PVT` - Price Volume Trend
- `MFI` - Money Flow Index

**Volatility Indicators:**
- `ATR` - Average True Range
- `BB` - Bollinger Bands

**Oscillators:**
- `FORCE` - Force Index
- `BOP` - Balance of Power

---

## Trading Components

### Account Class

Manages trading account state, positions, and transactions.

```csharp
namespace MarketSage.Library
{
    public class Account
    {
        public Account(double balance, double commission)
        public Account(double balance, double commission, string filename)
        
        // Properties
        public double Cash { get; set; }
        public double Commission { get; set; }
        public double Basis { get; }
        public int DaysInPlay { get; }
        public double AccountValue { get; }
        public double PositionValue { get; }
        public double TransactionPnL { get; }
        public double CumulativePnL { get; }
        public double InitialBalance { get; }
        public int TransactionCount { get; }
        public double DaysHeld { get; }
        public IList Transactions { get; }
        public double StopLossPrice { get; }
        
        // Methods
        public double GetPosition(string instrument)
        public double GetPrice(string instrument)
        public double TradingBalance(Instrument instrument)
        public void Save()
        public void init()
    }
}
```

**Key Methods:**
- `GetPosition(instrument)` - Returns current position size for instrument
- `GetPrice(instrument)` - Returns current price for instrument
- `TradingBalance(instrument)` - Returns available trading balance (includes margin)
- `Save()` - Persists account state to file
- `init()` - Resets account to initial state

**Usage:**
```csharp
// Create account with $10000 initial balance and $7 commission
var account = new Account(10000.0, 7.0);

// Check current cash balance
double cash = account.Cash;

// Get current position in AAPL
double position = account.GetPosition("AAPL");

// Get account value including positions
double totalValue = account.AccountValue;
```

### Instrument Class

Represents a tradeable financial instrument.

```csharp
namespace MarketSage.Library
{
    public class Instrument
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; }
        public double AdjClose { get; }
        public double AdjHigh { get; }
        public double AdjLow { get; }
        
        public TechnicalData GetCurrent()
        public void AddData(TechnicalData data)
    }
}
```

### Trader Class

Handles order execution and trade management.

```csharp
namespace MarketSage.Library
{
    public class Trader
    {
        public Account Account { get; set; }
        public Instrument Instrument { get; set; }
        
        public void ExecuteOrders(ArrayList orders)
        public void ProcessOrder(Order order)
    }
}
```

---

## Strategy Framework

### AbstractStrategy Class

Base class for all trading strategies.

```csharp
namespace MarketSage.Library
{
    public abstract class AbstractStrategy
    {
        // Properties
        public CompoundIndicator EntryIndicator { get; set; }
        public CompoundIndicator ExitIndicator { get; set; }
        public Account Account { get; set; }
        public Instrument Market { get; set; }
        public string EntryIndicatorName { get; set; }
        public string ExitIndicatorName { get; set; }
        public string Genome { get; }
        
        // Methods
        public virtual ArrayList GetPendingOrders()
        public virtual ArrayList GetTickEvents()
        public virtual double GetCash()
        public virtual double GetDaysHeld()
        public virtual double GetValue()
        public virtual double GetPnL()
        public virtual double GetCumulativePnL()
        public virtual double GetInitialBalance()
        public virtual int GetTransactionCount()
        public virtual IList GetTransactions()
        public virtual double GetCommission()
        public virtual Transaction GetLastTransaction()
        public double GetPosition(string instrument)
        public double GetPrice(string instrument)
        public void AddQuantityOrder(string instrument, double quantity)
        public void AddCashOrder(string instrument, double amount)
        public virtual void Update()
        public abstract void Initialize()
    }
}
```

**Key Methods:**
- `AddQuantityOrder(instrument, quantity)` - Places order for specific quantity
- `AddCashOrder(instrument, amount)` - Places order for specific cash amount
- `Update()` - Called on each market tick to update strategy
- `Initialize()` - Must be implemented by concrete strategies

### Available Strategy Implementations

**RandomStrategy**
```csharp
public class RandomStrategy : AbstractStrategy
{
    public RandomStrategy()
    public override void Initialize()
}
```

**GeneticStrategy**
```csharp
public class GeneticStrategy : AbstractStrategy
{
    public GeneticStrategy()
    public override void Initialize()
}
```

**TestStrategy**
```csharp
public class TestStrategy : AbstractStrategy
{
    public TestStrategy()
    public override void Initialize()
}
```

**Usage Example:**
```csharp
// Create a simple strategy
public class MyStrategy : AbstractStrategy
{
    public override void Initialize()
    {
        // Set up entry and exit indicators
        EntryIndicator = new CompoundIndicator();
        EntryIndicator.AddIndicator(new SMA(20));
        
        ExitIndicator = new CompoundIndicator();
        ExitIndicator.AddIndicator(new RSI(14));
    }
    
    public override void Update()
    {
        base.Update(); // Call base implementation
        
        // Custom strategy logic here
        if (GetPosition(Market.Name) == 0 && EntryIndicator.IsBuy())
        {
            AddCashOrder(Market.Name, GetCash() * 0.5);
        }
    }
}
```

---

## Portfolio Management

### Portfolio Class

Manages multiple positions and provides portfolio-level analytics.

```csharp
namespace MarketSage.Library
{
    public class Portfolio
    {
        public Portfolio(double initialbalance, string filename, string directoryLogs)
        
        // Methods
        public void AddPosition(string symbol, string name, double shares, double price, double commission)
        public void RemovePosition(string symbol, double price, double commission)
        public double GetAccountValue()
        public double GetTotalValue()
        public double GetPnL()
        public double GetPnLPercentage()
        public string GetDJI()
        public string GetIXIC()
        public string GetGSPC()
        public string[] GetArrayOfInstruments()
        public string GetSymbolByRow(int index)
        public string GetNameByRow(int index)
        public DataSet GetDataSet()
        public void Save()
        public void Update()
    }
}
```

**Key Methods:**
- `AddPosition(symbol, name, shares, price, commission)` - Adds new position
- `RemovePosition(symbol, price, commission)` - Closes position
- `GetTotalValue()` - Returns total portfolio value
- `GetPnL()` - Returns profit/loss in dollars
- `GetPnLPercentage()` - Returns profit/loss as percentage
- `Update()` - Updates all positions with current market prices

**Usage:**
```csharp
// Create portfolio with $50,000 initial balance
var portfolio = new Portfolio(50000.0, "portfolio.xml", "logs/");

// Add position: 100 shares of AAPL at $150
portfolio.AddPosition("AAPL", "Apple Inc.", 100, 150.0, 7.0);

// Get current portfolio value
double totalValue = portfolio.GetTotalValue();
double pnl = portfolio.GetPnL();

// Update with current market prices
portfolio.Update();
```

---

## Plugin System

### PluginServices Class

Manages dynamic loading and discovery of indicator plugins.

```csharp
namespace MarketSage.Library
{
    public class PluginServices
    {
        public static PluginServices Instance { get; }
        public AvailablePlugins<IIndicator> AvailablePlugins { get; set; }
        
        public void FindPlugins()
        public void FindPlugins(string path)
        public void ClosePlugins()
    }
}
```

### AvailablePlugin Class

Represents a discovered plugin.

```csharp
namespace MarketSage.Library
{
    public class AvailablePlugin<T>
    {
        public string AssemblyPath { get; set; }
        public Type PlugType { get; set; }
        public string Name { get; set; }
        public T Instance { get; set; }
    }
}
```

**Usage:**
```csharp
// Discover plugins
var pluginService = PluginServices.Instance;
pluginService.FindPlugins();

// Create instance of a plugin
foreach (var plugin in pluginService.AvailablePlugins)
{
    if (plugin.Name == "SMA")
    {
        var smaInstance = (IIndicator)Activator.CreateInstance(plugin.PlugType);
        smaInstance.Initialize();
        break;
    }
}
```

---

## Reporting

### Available Report Classes

**AnnualReport**
```csharp
namespace MarketSage.Library
{
    public class AnnualReport
    {
        // Generates annual performance reports
    }
}
```

**DataQualityReport** 
```csharp
namespace MarketSage.Library
{
    public class DataQualityReport
    {
        // Analyzes data quality and completeness
    }
}
```

**TransactionsReport**
```csharp
namespace MarketSage.Library
{
    public class TransactionsReport
    {
        // Generates transaction history reports
    }
}
```

---

## Usage Examples

### Complete Trading System Example

```csharp
using MarketSage.Library;
using MarketSage.Plugin;

public class TradingExample
{
    public void RunBacktest()
    {
        // 1. Set up account
        var account = new Account(10000.0, 7.0);
        
        // 2. Create instrument
        var instrument = new Instrument();
        instrument.Name = "AAPL";
        
        // 3. Create strategy
        var strategy = new MyCustomStrategy();
        strategy.Account = account;
        strategy.Market = instrument;
        strategy.Initialize();
        
        // 4. Load historical data and run backtest
        var historicalData = LoadHistoricalData("AAPL");
        
        foreach (var dataPoint in historicalData)
        {
            instrument.AddData(dataPoint);
            strategy.Update();
            
            // Execute any pending orders
            var pendingOrders = strategy.GetPendingOrders();
            foreach (Order order in pendingOrders)
            {
                ExecuteOrder(account, order);
            }
        }
        
        // 5. Report results
        Console.WriteLine($"Final Account Value: {account.AccountValue:C}");
        Console.WriteLine($"Total P&L: {account.CumulativePnL:C}");
        Console.WriteLine($"Total Transactions: {account.TransactionCount}");
    }
}

public class MyCustomStrategy : AbstractStrategy
{
    private SMA sma20;
    private RSI rsi14;
    
    public override void Initialize()
    {
        sma20 = new SMA(20);
        rsi14 = new RSI(14);
    }
    
    public override void Update()
    {
        var currentData = Market.GetCurrent();
        if (currentData == null) return;
        
        sma20.AddData(currentData);
        rsi14.AddData(currentData);
        
        if (!sma20.IsReady() || !rsi14.IsReady()) return;
        
        double position = GetPosition(Market.Name);
        
        // Entry logic: Buy when price > SMA20 and RSI < 70
        if (position == 0 && currentData.AdjClose > sma20.GetValue() && rsi14.GetValue() < 70)
        {
            AddCashOrder(Market.Name, GetCash() * 0.5);
        }
        
        // Exit logic: Sell when RSI > 80 or price < SMA20
        if (position > 0 && (rsi14.GetValue() > 80 || currentData.AdjClose < sma20.GetValue()))
        {
            AddQuantityOrder(Market.Name, -position);
        }
    }
}
```

### Indicator Usage Example

```csharp
// Create multiple indicators
var sma20 = new SMA(20);
var ema12 = new EMA(12);
var rsi14 = new RSI(14);
var macd = new MACD(12, 26, 9);

// Feed data to indicators
foreach (var dataPoint in marketData)
{
    sma20.AddData(dataPoint);
    ema12.AddData(dataPoint);
    rsi14.AddData(dataPoint);
    macd.AddData(dataPoint);
    
    // Check if all indicators are ready
    if (sma20.IsReady() && ema12.IsReady() && rsi14.IsReady() && macd.IsReady())
    {
        // Generate trading signals based on multiple indicators
        bool bullishSignal = ema12.IsBuy() && macd.IsBuy() && rsi14.GetValue() < 70;
        bool bearishSignal = ema12.IsSell() || macd.IsSell() || rsi14.GetValue() > 80;
        
        if (bullishSignal)
        {
            Console.WriteLine("BUY SIGNAL");
        }
        else if (bearishSignal)
        {
            Console.WriteLine("SELL SIGNAL");
        }
    }
}
```

### Portfolio Management Example

```csharp
// Create and manage portfolio
var portfolio = new Portfolio(100000.0, "myportfolio.xml", "logs/");

// Add multiple positions
portfolio.AddPosition("AAPL", "Apple Inc.", 100, 150.0, 7.0);
portfolio.AddPosition("MSFT", "Microsoft", 50, 200.0, 7.0);
portfolio.AddPosition("GOOGL", "Alphabet", 25, 1500.0, 7.0);

// Monitor portfolio performance
portfolio.Update(); // Updates with current market prices

Console.WriteLine($"Portfolio Value: {portfolio.GetTotalValue():C}");
Console.WriteLine($"Cash Balance: {portfolio.GetAccountValue():C}");
Console.WriteLine($"P&L: {portfolio.GetPnL():C} ({portfolio.GetPnLPercentage():F2}%)");

// Get all holdings
string[] holdings = portfolio.GetArrayOfInstruments();
foreach (string symbol in holdings)
{
    Console.WriteLine($"Holding: {symbol}");
}
```

---

## Best Practices

### 1. Indicator Development
- Always implement the `IIndicator` interface
- Ensure `IsReady()` returns false until sufficient data is available
- Handle edge cases in signal generation
- Provide meaningful names and descriptions

### 2. Strategy Development
- Inherit from `AbstractStrategy`
- Implement proper risk management
- Use compound indicators for complex signals
- Test thoroughly with historical data

### 3. Error Handling
- Always check `IsReady()` before using indicator signals
- Validate market data before processing
- Handle exceptions in order execution

### 4. Performance Optimization
- Cache calculated values when possible
- Use efficient data structures for large datasets
- Minimize object creation in update loops

---

## API Reference Summary

### Core Classes
- `IIndicator` - Base interface for all indicators
- `TechnicalData` - Market data structure
- `AbstractStrategy` - Base class for trading strategies
- `Account` - Trading account management
- `Portfolio` - Portfolio management
- `PluginServices` - Plugin system management

### Key Namespaces
- `MarketSage.Library` - Core business logic
- `MarketSage.Plugin` - Technical indicators
- `MarketSage` - Main application

### Main Entry Point
```csharp
// Application entry point
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new FormMain());
}
```

This documentation provides a comprehensive overview of the MarketSage API. For specific implementation details, refer to the source code and inline documentation within each class.