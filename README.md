# FXCM
1. Create FXCMHD.txt in C:\ with: 
    
    Line 1 = Account ID
    
    Line 2 = Password
    
    Line 3 = Symbol, e.g. AUD/USD
    
    Line 4 = Timeframe(for historical data downloader), e.g. M1, H4
    
    Line 5 = Start time for historical data downloader
    
    Line 6 = End time for historical data downloader
    
    Line 7 = Root directory of project
    
2. Run HistoricalDataDownloader to download historical data for given pairs, e.g. download 2016 minute data use the following FXCMHD.txt (Notice: change 14:00:00 to the local equivalent time of 17:00:00 EST)
    
    ![alt text](https://github.com/tianliwi/FXCM/blob/master/params.png)

3. Run BackTest to scan k1 and k2

4. Run DrawPNL to draw pnl curve, change k1 and k2 in FXCMBT.cs -> LoadBackTest
    
