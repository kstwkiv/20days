using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
class TravelSummary
{
    public long lastEntryStation;
    public long lastExitStation;
    public long lastEntryTime;
    public long lastExitTime;
    public double totalFarePaid;
    public int totalTrips;
    public double averageFarePerTrip;

}

class Commuter
{
    public int cardNumber;
    public string commuteName;
    public string commuterType;
    public TravelSummary travelSummary; 
}

class Station
{
    public int stationId;
    public string stationName;
    public int Zone;
    public double latitude;
    public double longitude;
}

interface MetroOperations
{

    void issueCard(int cardNumber, String commuterName, String commuterType);

    bool tapIn(int cardNumber, int stationId, long epochTime);

    bool tapOut(int cardNumber, int stationId, long epochTime);

    Commuter getCommuterInfo(int cardNumber);

    List<Double> fareHistory(int cardNumber);

    Dictionary<String, Double> getZoneWiseRevenue(long startTime, long endTime);

    List<String> getFrequentRoute(int cardNumber);

    double getDailyPassSavings(int cardNumber, long date);

}

class Journey
{
    public int entryStationId;
    public long entryTime;
    public Journey(int entryStationId, long entryTime)
    {
        this.entryStationId = entryStationId;
        this.entryTime = entryTime;
    }
}

class completedJourney
{
    public int cardNumber;
    public int entryStationId;
    public int exitStationId;
    public long entryTime;
    public long exitTime;
    public double fare;

    public completedJourney(int cardNnumber,int entryStationId, int exitStationId, long entryTime,long exitTime,double fare)
    {
        this.cardNumber = cardNnumber;
        this.entryStationId = entryStationId;
        this.exitStationId = exitStationId;
        this.entryTime = entryTime;
        this.e
    }
}
