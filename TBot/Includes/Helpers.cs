﻿using Tbot;
using Tbot.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Linq;
using System.IO;

namespace Tbot.Includes
{
    static class Helpers
    {
        public static void WriteLog(LogType type, LogSender sender, string message)
        {
            LogToConsole(type, sender, message);
            LogToFile(type, sender, message);
        }

        public static void LogToConsole(LogType type, LogSender sender, string message)
        {
            Console.ForegroundColor = type switch
            {
                LogType.Error => ConsoleColor.Red,
                LogType.Warning => ConsoleColor.Yellow,
                LogType.Info => ConsoleColor.Gray,
                LogType.Debug => ConsoleColor.White,
                _ => ConsoleColor.Gray
            };
            Console.WriteLine("[" + type.ToString() + "] " + "[" + sender.ToString() + "] " + "[" + DateTime.Now.ToString() + "] - " + message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void LogToFile(LogType type, LogSender sender, string message)
        {
            string path = Directory.GetCurrentDirectory() + "/log";
            DirectoryInfo dir = new(path);
            if (!dir.Exists)
                dir.Create();
            string fileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_TBot.log";
            try
            {
                StreamWriter file = new(path + "/" + fileName, true);
                file.WriteLine("[" + type.ToString() + "] " + "[" + sender.ToString() + "] " + "[" + DateTime.Now.ToString() + "] - " + message);
                file.Close();
            }
            catch (Exception) { }
        }
        public static void SetTitle(string content = "")
        {
            AssemblyName exeInfo = Assembly.GetExecutingAssembly().GetName();
            string info = exeInfo.Name + " v" + exeInfo.Version;
            if (content != "")
                Console.Title = content + " - " + info;
            else
                Console.Title = info;
            return;
        }

        public static void PlayAlarm()
        {
            Console.Beep();
            Thread.Sleep(1000);
            Console.Beep();
            Thread.Sleep(1000);
            Console.Beep();
            return;
        }

        public static int CalcRandomInterval(IntervalType type)
        {
            var rand = new Random();
            return type switch
            {
                IntervalType.LessThanASecond => rand.Next(500, 1000),
                IntervalType.AFewSeconds => rand.Next(5000, 15000),
                IntervalType.SomeSeconds => rand.Next(20000, 50000),
                IntervalType.AMinuteOrTwo => rand.Next(40000, 140000),
                IntervalType.AboutFiveMinutes => rand.Next(240000, 360000),
                IntervalType.AboutTenMinutes => rand.Next(540000, 720000),
                IntervalType.AboutAQuarterHour => rand.Next(840000, 960000),
                IntervalType.AboutHalfAnHour => rand.Next(1500000, 2100000),
                IntervalType.AboutAnHour => rand.Next(3000000, 42000000),
                _ => rand.Next(500, 1000),
            };
        }

        public static int CalcRandomInterval(int min, int max)
        {
            var rand = new Random();
            var minMillis = min * 60 * 1000;
            var maxMillis = max * 60 * 1000;
            return rand.Next(minMillis, maxMillis);
        }

        public static int CalcShipCapacity(Buildables buildable, int hyperspaceTech, Classes playerClass)
        {
            int baseCargo;
            int bonus = (hyperspaceTech * 5);
            switch (buildable)
            {
                case Buildables.SmallCargo:
                    baseCargo = 5000;
                    if (playerClass == Classes.Collector) bonus += 25;
                    break;
                case Buildables.LargeCargo:
                    baseCargo = 25000;
                    if (playerClass == Classes.Collector) bonus += 25;
                    break;
                case Buildables.LightFighter:
                    baseCargo = 50;
                    break;
                case Buildables.HeavyFighter:
                    baseCargo = 100;
                    break;
                case Buildables.Cruiser:
                    baseCargo = 800;
                    break;
                case Buildables.Battleship:
                    baseCargo = 1500;
                    break;
                case Buildables.ColonyShip:
                    baseCargo = 7500;
                    break;
                case Buildables.Recycler:
                    baseCargo = 20000;
                    if (playerClass == Classes.General) bonus += 25;
                    break;
                case Buildables.EspionageProbe:
                    baseCargo = 5;
                    break;
                case Buildables.Bomber:
                    baseCargo = 750;
                    break;
                case Buildables.Destroyer:
                    baseCargo = 2000;
                    break;
                case Buildables.Deathstar:
                    baseCargo = 1000000;
                    break;
                case Buildables.Battlecruiser:
                    baseCargo = 750;
                    break;
                case Buildables.Reaper:
                    baseCargo = 7000;
                    break;
                case Buildables.Pathfinder:
                    baseCargo = 12000;
                    if (playerClass == Classes.General) bonus += 25;
                    break;
                default:
                    return 0;
            }
            return baseCargo * (bonus + 100) / 100;
        }

        public static long CalcFleetCapacity(Ships fleet, int hyperspaceTech, Classes playerClass)
        {
            long total = 0;
            foreach (PropertyInfo prop in fleet.GetType().GetProperties())
            {
                long qty = (long)prop.GetValue(fleet, null);
                if (qty == 0) continue;
                if (Enum.TryParse<Buildables>(prop.Name, out Buildables buildable))
                {
                    int oneCargo = CalcShipCapacity(buildable, hyperspaceTech, playerClass);
                    total += oneCargo * qty;
                }
            }
            return total;
        }

        public static int CalcShipSpeed(Buildables buildable, Researches researches, Classes playerClass)
        {
            return CalcShipSpeed(buildable, researches.CombustionDrive, researches.ImpulseDrive, researches.HyperspaceDrive, playerClass);
        }

        public static int CalcShipSpeed(Buildables buildable, int combustionDrive, int impulseDrive, int hyperspaceDrive, Classes playerClass)
        {
            int baseSpeed;
            int bonus = combustionDrive;
            switch (buildable)
            {
                case Buildables.SmallCargo:
                    baseSpeed = 5000;
                    if (impulseDrive >= 5)
                    {
                        baseSpeed = 10000;
                        bonus = impulseDrive * 2;
                    }
                    if (playerClass == Classes.Collector) bonus += 10;
                    break;
                case Buildables.LargeCargo:
                    baseSpeed = 7500;
                    if (playerClass == Classes.Collector) bonus += 10;
                    break;
                case Buildables.LightFighter:
                    baseSpeed = 12500;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.HeavyFighter:
                    baseSpeed = 10000;
                    bonus = impulseDrive * 2;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.Cruiser:
                    baseSpeed = 15000;
                    bonus = impulseDrive * 2;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.Battleship:
                    baseSpeed = 10000;
                    bonus = hyperspaceDrive * 3;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.ColonyShip:
                    bonus = impulseDrive * 2;
                    baseSpeed = 2500;
                    break;
                case Buildables.Recycler:
                    baseSpeed = 2000;
                    if (impulseDrive >= 17)
                    {
                        baseSpeed = 4000;
                        bonus = impulseDrive * 2;
                    }
                    if (hyperspaceDrive >= 15)
                    {
                        baseSpeed = 6000;
                        bonus = hyperspaceDrive * 3;
                    }
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.EspionageProbe:
                    baseSpeed = 100000000;
                    break;
                case Buildables.Bomber:
                    baseSpeed = 4000;
                    bonus = impulseDrive * 2;
                    if (hyperspaceDrive >= 8)
                    {
                        baseSpeed = 5000;
                        bonus = hyperspaceDrive * 3;
                    }
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.Destroyer:
                    baseSpeed = 5000;
                    bonus = hyperspaceDrive * 3;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.Deathstar:
                    baseSpeed = 100;
                    bonus = hyperspaceDrive * 3;
                    break;
                case Buildables.Battlecruiser:
                    baseSpeed = 10000;
                    bonus = hyperspaceDrive * 3;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.Reaper:
                    baseSpeed = 10000;
                    bonus = hyperspaceDrive * 3;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                case Buildables.Pathfinder:
                    baseSpeed = 10000;
                    bonus = hyperspaceDrive * 3;
                    if (playerClass == Classes.General) bonus += 10;
                    break;
                default:
                    return 0;
            }
            return (int)Math.Round(((float)baseSpeed * ((float)bonus + 10) / 10), MidpointRounding.ToZero);
        }

        public static int CalcSlowestSpeed(Ships fleet, Researches researches, Classes playerClass)
        {
            return CalcSlowestSpeed(fleet, researches.CombustionDrive, researches.ImpulseDrive, researches.HyperspaceDrive, playerClass);
        }

        public static int CalcSlowestSpeed(Ships fleet, int combustionDrive, int impulseDrive, int hyperspaceDrive, Classes playerClass)
        {
            int lowest = int.MaxValue;
            foreach (PropertyInfo prop in fleet.GetType().GetProperties())
            {
                long qty = (long)prop.GetValue(fleet, null);
                
                if (qty == 0) continue;
                if (Enum.TryParse<Buildables>(prop.Name, out Buildables buildable))
                {
                    if (buildable == Buildables.SolarSatellite || buildable == Buildables.Crawler)
                        continue;
                    int speed = CalcShipSpeed(buildable, combustionDrive, impulseDrive, hyperspaceDrive, playerClass);
                    if (speed < lowest)
                        lowest = speed;
                }
            }
            return lowest;
        }        

        public static int CalcFleetSpeed(Ships fleet, Researches researches, Classes playerClass)
        {
            return CalcFleetSpeed(fleet, researches.CombustionDrive, researches.ImpulseDrive, researches.HyperspaceDrive, playerClass);
        }

        public static int CalcFleetSpeed(Ships fleet, int combustionDrive, int impulseDrive, int hyperspaceDrive, Classes playerClass)
        {
            int minSpeed = 0;
            foreach (PropertyInfo prop in fleet.GetType().GetProperties())
            {
                long qty = (long)prop.GetValue(fleet, null);
                if (qty == 0) continue;
                if (Enum.TryParse<Buildables>(prop.Name, out Buildables buildable))
                {
                    int thisSpeed = CalcShipSpeed(buildable, combustionDrive, impulseDrive, hyperspaceDrive, playerClass);
                    if (thisSpeed < minSpeed) minSpeed = thisSpeed;
                }
            }
            return minSpeed;
        }

        public static int CalcShipConsumption(Buildables buildable, Researches researches, ServerData serverData, Classes playerClass)
        {
            return CalcShipConsumption(buildable, researches.ImpulseDrive, researches.HyperspaceDrive, serverData.GlobalDeuteriumSaveFactor, playerClass);
        }

        public static int CalcShipConsumption(Buildables buildable, int impulseDrive, int hyperspaceDrive, double deuteriumSaveFactor, Classes playerClass)
        {
            int baseConsumption;
            switch (buildable)
            {
                case Buildables.SmallCargo:
                    baseConsumption = 20;
                    if (impulseDrive >= 5)
                        baseConsumption *= 2;
                    break;
                case Buildables.LargeCargo:
                    baseConsumption = 50;
                    break;
                case Buildables.LightFighter:
                    baseConsumption = 20;
                    break;
                case Buildables.HeavyFighter:
                    baseConsumption = 75;
                    break;
                case Buildables.Cruiser:
                    baseConsumption = 300;
                    break;
                case Buildables.Battleship:
                    baseConsumption = 500;
                    break;
                case Buildables.ColonyShip:
                    baseConsumption = 1000;
                    break;
                case Buildables.Recycler:
                    baseConsumption = 2000;
                    if (hyperspaceDrive >= 15)
                        baseConsumption *= 3;
                    else if (impulseDrive >= 17)
                        baseConsumption *= 2;                    
                    break;
                case Buildables.EspionageProbe:
                    baseConsumption = 1;
                    break;
                case Buildables.Bomber:
                    baseConsumption = 700;
                    if (hyperspaceDrive >= 8)
                        baseConsumption *= 3 / 2;
                    break;
                case Buildables.Destroyer:
                    baseConsumption = 1000;
                    break;
                case Buildables.Deathstar:
                    baseConsumption = 1;
                    break;
                case Buildables.Battlecruiser:
                    baseConsumption = 250;
                    break;
                case Buildables.Reaper:
                    baseConsumption = 1100;
                    break;
                case Buildables.Pathfinder:
                    baseConsumption = 300;
                    break;
                default:
                    return 0;
            }
            float fuelConsumption = (float)(deuteriumSaveFactor * baseConsumption);
            if (playerClass == Classes.General)
                fuelConsumption /= 2;
            return (int)Math.Round(fuelConsumption, MidpointRounding.ToZero);
        }

        public static int CalcFlightTime(Coordinate origin, Coordinate destination, Ships ships, decimal speed, Researches researches, ServerData serverData, Classes playerClass)
        {
            return CalcFlightTime(origin, destination, ships, speed, researches.CombustionDrive, researches.ImpulseDrive, researches.HyperspaceDrive, serverData.Galaxies, serverData.Systems, serverData.DonutGalaxy, serverData.DonutSystem, serverData.SpeedFleet, playerClass);
        }

        public static int CalcFlightTime(Coordinate origin, Coordinate destination, Ships ships, decimal speed, int combustionDrive, int impulseDrive, int hyperspaceDrive, int numberOfGalaxies, int numberOfSystems, bool donutGalaxies, bool donutSystems, int fleetSpeed, Classes playerClass)
        {
            int slowestShipSpeed = CalcSlowestSpeed(ships, combustionDrive, impulseDrive, hyperspaceDrive, playerClass);
            int distance = CalcDistance(origin, destination, numberOfGalaxies, numberOfSystems, donutGalaxies, donutSystems);
            return (int)Math.Round(((3500 / ((double)speed / 10) ) * (Math.Sqrt(distance * 10 / slowestShipSpeed) + 10) / fleetSpeed), MidpointRounding.AwayFromZero);
        }

        public static long CalcFuelConsumption(Coordinate origin, Coordinate destination, Ships ships, int flightTime, Researches researches, ServerData serverData, Classes playerClass)
        {
            return CalcFuelConsumption(origin, destination, ships, flightTime, researches.CombustionDrive, researches.ImpulseDrive, researches.HyperspaceDrive, serverData.Galaxies, serverData.Systems, serverData.DonutGalaxy, serverData.DonutSystem, serverData.SpeedFleet, serverData.GlobalDeuteriumSaveFactor, playerClass);
        }

        public static long CalcFuelConsumption(Coordinate origin, Coordinate destination, Ships ships, int flightTime, int combustionDrive, int impulseDrive, int hyperspaceDrive, int numberOfGalaxies, int numberOfSystems, bool donutGalaxies, bool donutSystems, int fleetSpeed, float deuteriumSaveFactor, Classes playerClass)
        {
            int distance = CalcDistance(origin, destination, numberOfGalaxies, numberOfSystems, donutGalaxies, donutSystems);
            float tempFuel = 0.0F;
            foreach (PropertyInfo prop in ships.GetType().GetProperties())
            {
                long qty = (long)prop.GetValue(ships, null);
                if (qty == 0) continue;
                if (Enum.TryParse<Buildables>(prop.Name, out Buildables buildable))
                {
                    float tempSpeed = 35000 / ((flightTime * fleetSpeed) - 10) * (float)Math.Sqrt(distance * 10 / CalcShipSpeed(buildable, combustionDrive, impulseDrive, hyperspaceDrive, playerClass));
                    int shipConsumption = CalcShipConsumption(buildable, impulseDrive, hyperspaceDrive, deuteriumSaveFactor, playerClass);
                    float thisFuel = (float)(shipConsumption * qty * distance) / 35000F * (float)Math.Pow(tempSpeed / 10 + 1, 2);
                    tempFuel += thisFuel;
                }
            }
            return (long)(1 + Math.Round(tempFuel, MidpointRounding.AwayFromZero));
        }

        public static Resources CalcMaxTransportableResources(Ships ships, Resources resources, int hyperspaceTech, Classes playerClass, long deuttoleave = 0, bool skipdeut = false)
        {
            var capacity = CalcFleetCapacity(ships, hyperspaceTech, playerClass);
            if (resources.Deuterium <= deuttoleave)
            {
                return new Resources { Deuterium = 0, Crystal = 0, Metal = 0 }; // don't transport if not enough deutereum left
            }

            else if (resources.TotalResources <= capacity)
            {
                if (skipdeut){ return new Resources {                                                Crystal = resources.Crystal, Metal = resources.Metal }; }
                else           return new Resources { Deuterium = resources.Deuterium - deuttoleave, Crystal = resources.Crystal, Metal = resources.Metal };
            }
            else
            {
                if (skipdeut)
                {
                    if (resources.Crystal > capacity)
                    {
                        return new Resources { Crystal = capacity };
                    }
                    else if (capacity >= resources.Crystal && capacity < (resources.TotalResources - resources.Deuterium))
                    {
                        return new Resources {  Crystal = resources.Crystal, Metal = (capacity - resources.Crystal) };
                    }
                    else return new Resources { Crystal = resources.Crystal, Metal = resources.Metal };

                }
                else
                {
                    if (resources.Deuterium - deuttoleave > capacity)
                    {
                        return new Resources { Deuterium = capacity };
                    }
                    else if (capacity >= resources.Deuterium - deuttoleave && capacity < (resources.Deuterium - deuttoleave + resources.Crystal))
                    {
                        return new Resources { Deuterium = resources.Deuterium - deuttoleave, Crystal = (capacity - resources.Deuterium + deuttoleave) };
                    }
                    else if (capacity >= (resources.Deuterium - deuttoleave + resources.Crystal) && capacity < resources.TotalResources)
                    {
                        return new Resources { Deuterium = resources.Deuterium - deuttoleave, Crystal = resources.Crystal, Metal = (capacity - resources.Deuterium + deuttoleave - resources.Crystal) };
                    }
                    else return resources;
                }
            }
        }

        public static long CalcShipNumberForPayload(Resources payload, Buildables buildable, int hyperspaceTech, Classes playerClass)
        {
            return (long)Math.Round(((float)payload.TotalResources / (float)CalcShipCapacity(buildable, hyperspaceTech, playerClass)), MidpointRounding.ToPositiveInfinity);
        }

        public static Ships CalcIdealExpeditionShips(Buildables buildable, int ecoSpeed, long topOnePoints, int hyperspaceTech, Classes playerClass)
        {
            var fleet = new Ships();

            int freightCap;
            if (topOnePoints < 10000)
                freightCap = 40000;
            else if (topOnePoints < 100000)
                freightCap = 500000;
            else if (topOnePoints < 1000000)
                freightCap = 1200000;
            else if (topOnePoints < 5000000)
                freightCap = 1800000;
            else if (topOnePoints < 25000000)
                freightCap = 2400000;
            else if (topOnePoints < 50000000)
                freightCap = 3000000;
            else if (topOnePoints < 75000000)
                freightCap = 3600000;
            else if (topOnePoints < 100000000)
                freightCap = 4200000;
            else
                freightCap = 5000000;

            if (playerClass == Classes.Discoverer)
                freightCap = freightCap * ecoSpeed * 3;
            else
                freightCap *= 2;

            int oneCargoCapacity = CalcShipCapacity(buildable, hyperspaceTech, playerClass);
            int cargoNumber = (int)Math.Round((float)freightCap / (float)oneCargoCapacity, MidpointRounding.ToPositiveInfinity);

            fleet = fleet.Add(buildable, cargoNumber);

            return fleet;
        }

        public static Buildables CalcMilitaryShipForExpedition(Ships fleet, int expeditionsNumber)
        {
            if (fleet.Reaper >= expeditionsNumber)
                return Buildables.Reaper;
            else if (fleet.Destroyer >= expeditionsNumber)
                return Buildables.Destroyer;
            else if (fleet.Bomber >= expeditionsNumber)
                return Buildables.Bomber;
            else if (fleet.Battlecruiser >= expeditionsNumber)
                return Buildables.Battlecruiser;
            else if (fleet.Battleship >= expeditionsNumber)
                return Buildables.Battleship;
            else if (fleet.Pathfinder >= expeditionsNumber)
                return Buildables.Pathfinder;
            else if (fleet.Cruiser >= expeditionsNumber)
                return Buildables.Cruiser;
            else if (fleet.HeavyFighter >= expeditionsNumber)
                return Buildables.HeavyFighter;
            else if (fleet.LightFighter >= expeditionsNumber)
                return Buildables.LightFighter;
            else
                return Buildables.Null;
        }

        public static Ships CalcExpeditionShips(Ships fleet, Buildables buildable, int expeditionsNumber, int ecoSpeed, long topOnePoints, int hyperspaceTech, Classes playerClass)
        {
            Ships ideal = CalcIdealExpeditionShips(buildable, ecoSpeed, topOnePoints, hyperspaceTech, playerClass);
            foreach (PropertyInfo prop in fleet.GetType().GetProperties())
            {
                if (prop.Name == buildable.ToString())
                {
                    long availableVal = (long)prop.GetValue(fleet);
                    long idealVal = (long)prop.GetValue(ideal);
                    if (availableVal < idealVal * expeditionsNumber)
                    {
                        long realVal = (long)Math.Round(((float)availableVal / (float)expeditionsNumber), MidpointRounding.AwayFromZero);
                        prop.SetValue(ideal, realVal);
                    }
                }
            }
            return ideal;
        }

        public static Ships CalcExpeditionShips(Ships fleet, Buildables buildable, int expeditionsNumber, ServerData serverdata, Researches researches, Classes playerClass)
        {
            return CalcExpeditionShips(fleet, buildable, expeditionsNumber, serverdata.Speed, serverdata.TopScore, researches.HyperspaceTechnology, playerClass);
        }

        public static bool MayAddShipToExpedition(Ships fleet, Buildables buildable, int expeditionsNumber)
        {
            foreach (PropertyInfo prop in fleet.GetType().GetProperties())
            {
                if (prop.Name == buildable.ToString())
                {
                    long availableVal = (long)prop.GetValue(fleet);
                    if (availableVal >= expeditionsNumber)
                        return true;
                }
            }
            return false;
        }

        public static Ships CalcFullExpeditionShips(Ships fleet, Buildables buildable, int expeditionsNumber, ServerData serverdata, Researches researches, Classes playerClass)
        {
            Ships oneExpeditionFleet = CalcExpeditionShips(fleet, buildable, expeditionsNumber, serverdata, researches, playerClass);

            if (MayAddShipToExpedition(fleet, Buildables.EspionageProbe, expeditionsNumber))
                oneExpeditionFleet.Add(Buildables.EspionageProbe, 1);

            Buildables militaryShip = CalcMilitaryShipForExpedition(fleet, expeditionsNumber);
            if (MayAddShipToExpedition(fleet, militaryShip, expeditionsNumber))
                oneExpeditionFleet.Add(militaryShip, 1);

            if (MayAddShipToExpedition(fleet, Buildables.Pathfinder, expeditionsNumber))
                if (oneExpeditionFleet.Pathfinder == 0)
                    oneExpeditionFleet.Add(Buildables.Pathfinder, 1);

            return oneExpeditionFleet;
        }

        public static int CalcDistance(Coordinate origin, Coordinate destination, int galaxiesNumber, int systemsNumber = 499, bool donutGalaxy = true, bool donutSystem = true)
        {
            if (origin.Galaxy != destination.Galaxy)
                return CalcGalaxyDistance(origin, destination, galaxiesNumber, donutGalaxy);

            if (origin.System != destination.System)
                return CalcSystemDistance(origin, destination, systemsNumber, donutSystem);

            if (origin.Position != destination.Position)
                return CalcPlanetDistance(origin, destination);

            return 5;
        }

        public static int CalcDistance(Coordinate origin, Coordinate destination, ServerData serverData)
        {
            return CalcDistance(origin, destination, serverData.Galaxies, serverData.Systems, serverData.DonutGalaxy, serverData.DonutSystem);
        }

        private static int CalcGalaxyDistance(Coordinate origin, Coordinate destination, int galaxiesNumber, bool donutGalaxy = true)
        {
            if (!donutGalaxy)
                return 2000 * Math.Abs(origin.Galaxy - destination.Galaxy);

            if (origin.Galaxy > destination.Galaxy)
                return 2000 * Math.Min((origin.Galaxy - destination.Galaxy), ((destination.Galaxy + galaxiesNumber) - origin.Galaxy));

            return 2000 * Math.Min((destination.Galaxy - origin.Galaxy), ((origin.Galaxy + galaxiesNumber) - destination.Galaxy));
        }

        private static int CalcSystemDistance(Coordinate origin, Coordinate destination, int systemsNumber, bool donutSystem = true)
        {
            if (!donutSystem)
                return 2700 + 95 * Math.Abs(origin.System - destination.System);

            if (origin.System > destination.System)
                return 2700 + 95 * Math.Min((origin.System - destination.System), ((destination.System + systemsNumber) - origin.System));

            return 2700 + 95 * Math.Min((destination.System - origin.System), ((origin.System + systemsNumber) - destination.System));

        }

        private static int CalcPlanetDistance(Coordinate origin, Coordinate destination)
        {
            return 1000 + 5 * Math.Abs(destination.Position - origin.Position);
        }

        public static long CalcMetalProduction(int level, int position, int speedFactor, float ratio = 1, int plasma = 0, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            int baseProd = position switch
            {
                6 => (int)Math.Round(30 + (30 * 0.17)),
                7 => (int)Math.Round(30 + (30 * 0.23)),
                8 => (int)Math.Round(30 + (30 * 0.35)),
                9 => (int)Math.Round(30 + (30 * 0.23)),
                10 => (int)Math.Round(30 + (30 * 0.17)),
                _ => 30,
            };
            baseProd *= speedFactor;
            int prod = (int)Math.Round((float)(baseProd * level * Math.Pow(1.1, level)));
            int plasmaProd = (int)Math.Round(prod * 0.01 * plasma);
            int geologistProd = 0;
            if (hasGeologist)
            {
                geologistProd = (int)Math.Round(prod * 0.1);
            }
            int staffProd = 0;
            if (hasStaff)
            {
                staffProd = (int)Math.Round(prod * 0.02);
            }
            int classProd = 0;
            if (playerClass == Classes.Collector)
            {
                classProd = (int)Math.Round(prod * 0.25);
            }
            return (long)Math.Round((prod + plasmaProd + geologistProd + staffProd + classProd) * ratio);
        }

        public static long CalcMetalProduction(Buildings buildings, int position, int speedFactor, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            if (researches == null) researches = new Researches() { PlasmaTechnology = 0 };
            return CalcMetalProduction(buildings.MetalMine, position, speedFactor, ratio, researches.PlasmaTechnology, playerClass, hasGeologist, hasStaff);
        }

        public static long CalcMetalProduction(Planet planet, int speedFactor, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            if (researches == null) researches = new Researches() { PlasmaTechnology = 0 };
            return CalcMetalProduction(planet.Buildings.MetalMine, planet.Coordinate.Position, speedFactor, ratio, researches.PlasmaTechnology, playerClass, hasGeologist, hasStaff);
        }

        public static long CalcCrystalProduction(int level, int position, int speedFactor, float ratio = 1, int plasma = 0, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            int baseProd = position switch
            {
                1 => (int)Math.Round(20 + (20 * 0.3)),
                2 => (int)Math.Round(20 + (20 * 0.2)),
                3 => (int)Math.Round(20 + (20 * 0.1)),
                _ => 20,
            };
            baseProd *= speedFactor;
            int prod = (int)Math.Round((float)(baseProd * level * Math.Pow(1.1, level)));
            int plasmaProd = (int)Math.Round(prod * 0.0066 * plasma);
            int geologistProd = 0;
            if (hasGeologist)
            {
                geologistProd = (int)Math.Round(prod * 0.1);
            }
            int staffProd = 0;
            if (hasStaff)
            {
                staffProd = (int)Math.Round(prod * 0.02);
            }
            int classProd = 0;
            if (playerClass == Classes.Collector)
            {
                classProd = (int)Math.Round(prod * 0.25);
            }
            return (long)Math.Round((prod + plasmaProd + geologistProd + staffProd + classProd) * ratio);
        }

        public static long CalcCrystalProduction(Buildings buildings, int position, int speedFactor, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            if (researches == null) researches = new Researches() { PlasmaTechnology = 0 };
            return CalcCrystalProduction(buildings.CrystalMine, position, speedFactor, ratio, researches.PlasmaTechnology, playerClass, hasGeologist, hasStaff);
        }

        public static long CalcCrystalProduction(Planet planet, int speedFactor, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            if (researches == null) researches = new Researches() { PlasmaTechnology = 0 };
            return CalcCrystalProduction(planet.Buildings.CrystalMine, planet.Coordinate.Position, speedFactor, ratio, researches.PlasmaTechnology, playerClass, hasGeologist, hasStaff);
        }

        public static long CalcDeuteriumProduction(int level, float temp, int speedFactor, float ratio = 1, int plasma = 0, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            int baseProd = 10 * speedFactor;
            int prod = (int)Math.Round((float)(baseProd * level * Math.Pow(1.1, level) * (1.05 + (0.01 * temp))));
            int plasmaProd = (int)Math.Round(prod * 0.0033 * plasma);
            int geologistProd = 0;
            if (hasGeologist)
            {
                geologistProd = (int)Math.Round(prod * 0.1);
            }
            int staffProd = 0;
            if (hasStaff)
            {
                staffProd = (int)Math.Round(prod * 0.02);
            }
            int classProd = 0;
            if (playerClass == Classes.Collector)
            {
                classProd = (int)Math.Round(prod * 0.25);
            }
            return (long)Math.Round((prod + plasmaProd + geologistProd + staffProd + classProd) * ratio);
        }

        public static long CalcDeuteriumProduction(Buildings buildings, Temperature temp, int speedFactor, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            if (researches == null) researches = new Researches() { PlasmaTechnology = 0 };
            return CalcDeuteriumProduction(buildings.CrystalMine, temp.Average, speedFactor, ratio, researches.PlasmaTechnology, playerClass, hasGeologist, hasStaff);
        }

        public static long CalcDeuteriumProduction(Planet planet, int speedFactor, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            if (researches == null) researches = new Researches() { PlasmaTechnology = 0 };
            return CalcDeuteriumProduction(planet.Buildings.CrystalMine, planet.Temperature.Average, speedFactor, ratio, researches.PlasmaTechnology, playerClass, hasGeologist, hasStaff);
        }

        public static Resources CalcPlanetHourlyProduction(Planet planet, int speedFactor, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            Resources hourlyProduction = new()
            {
                Metal = CalcMetalProduction(planet, speedFactor, ratio, researches, playerClass, hasGeologist, hasStaff),
                Crystal = CalcCrystalProduction(planet, speedFactor, ratio, researches, playerClass, hasGeologist, hasStaff),
                Deuterium = CalcDeuteriumProduction(planet, speedFactor, ratio, researches, playerClass, hasGeologist, hasStaff)
            };
            return hourlyProduction;
        }

        public static Resources CalcPrice(Buildables buildable, int level)
        {
            Resources output = new();

            switch (buildable)
            {
                case Buildables.MetalMine:
                    output.Metal = (long)(60 * Math.Pow(1.5, (level - 1)));
                    output.Crystal = (long)(15 * Math.Pow(1.5, (level - 1)));
                    /*Lorenzo 06/02/2021
                     * Added the calc for the energy needed
                     */
                    //MidpointRounding set to "ToNegativeInfinity" because
                    //in all cases that i try (metal 51 crystal 44) the result is always the lower integer
                    //Formula: 10 * Mine Level * (1.1 ^ Mine Level)
                    output.Energy = (long)Math.Round((10 * level * (Math.Pow(1.1, level))), 0, MidpointRounding.ToNegativeInfinity);
                    break;
                case Buildables.CrystalMine:
                    output.Metal = (long)(48 * Math.Pow(1.6, (level - 1)));
                    output.Crystal = (long)(24 * Math.Pow(1.6, (level - 1)));
                    /*Lorenzo 06/02/2021
                     * Added the calc for the energy needed
                     */
                    //MidpointRounding set to "ToNegativeInfinity" because
                    //in all cases that i try (metal 51 crystal 44) the result is always the lower integer
                    //Formula: 10 * Mine Level * (1.1 ^ Mine Level)
                    output.Energy = (long)Math.Round((10 * level * (Math.Pow(1.1, level))), 0, MidpointRounding.ToNegativeInfinity);
                    break;
                case Buildables.DeuteriumSynthesizer:
                    output.Metal = (long)(225 * Math.Pow(1.5, (level - 1)));
                    output.Crystal = (long)(75 * Math.Pow(1.5, (level - 1)));
                    /*Lorenzo 06/02/2021
                     * Added the calc for the energy needed
                     */
                    //MidpointRounding set to "ToNegativeInfinity" because
                    //in all cases that i try (metal 51 crystal 44) the result is always the lower integer
                    //Formula: 20 * Mine Level * (1.1 ^ Mine Level)
                    output.Energy = (long)Math.Round((20 * level * (Math.Pow(1.1, level))), 0, MidpointRounding.ToNegativeInfinity);
                    break;
                case Buildables.SolarPlant:
                    output.Metal = (long)(75 * Math.Pow(1.5, (level - 1)));
                    output.Crystal = (long)(30 * Math.Pow(1.5, (level - 1)));
                    break;
                case Buildables.FusionReactor:
                    output.Metal = (long)(1900 * Math.Pow(1.8, (level - 1)));
                    output.Crystal = (long)(360 * Math.Pow(1.8, (level - 1)));
                    output.Deuterium = (long)(180 * Math.Pow(1.8, (level - 1)));
                    break;
                case Buildables.MetalStorage:
                    output.Metal = (long)(500 * Math.Pow(2, level));
                    break;
                case Buildables.CrystalStorage:
                    output.Metal = (long)(500 * Math.Pow(2, level));
                    output.Crystal = (long)(250 * Math.Pow(2, level));
                    break;
                case Buildables.DeuteriumTank:
                    output.Metal = (long)(500 * Math.Pow(2, level));
                    output.Crystal = (long)(500 * Math.Pow(2, level));
                    break;
                case Buildables.ShieldedMetalDen:
                    break;
                case Buildables.UndergroundCrystalDen:
                    break;
                case Buildables.SeabedDeuteriumDen:
                    break;
                case Buildables.AllianceDepot:
                    output.Metal = (long)(20000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(40000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.RoboticsFactory:
                    output.Metal = (long)(400 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(120 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(200 * Math.Pow(2, level - 1));
                    break;
                case Buildables.Shipyard:
                    output.Metal = (long)(400 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(200 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(100 * Math.Pow(2, level - 1));
                    break;
                case Buildables.ResearchLab:
                    output.Metal = (long)(200 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(400 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(200 * Math.Pow(2, level - 1));
                    break;
                case Buildables.MissileSilo:
                    output.Metal = (long)(20000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(20000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(1000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.NaniteFactory:
                    output.Metal = (long)(1000000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(500000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(100000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.Terraformer:
                    output.Crystal = (long)(50000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(100000 * Math.Pow(2, level - 1));
                    output.Energy = (long)(1000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.SpaceDock:
                    break;
                case Buildables.LunarBase:
                    output.Metal = (long)(20000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(40000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(20000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.SensorPhalanx:
                    output.Metal = (long)(20000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(40000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(20000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.JumpGate:
                    output.Metal = (long)(2000000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(4000000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(2000000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.RocketLauncher:
                    output.Metal = (long)(2000 * level);
                    break;
                case Buildables.LightLaser:
                    output.Metal = (long)(1500 * level);
                    output.Crystal = (long)(500 * level);
                    break;
                case Buildables.HeavyLaser:
                    output.Metal = (long)(6000 * level);
                    output.Crystal = (long)(2000 * level);
                    break;
                case Buildables.GaussCannon:
                    output.Metal = (long)(20000 * level);
                    output.Crystal = (long)(15000 * level);
                    output.Deuterium = (long)(2000 * level);
                    break;
                case Buildables.IonCannon:
                    output.Metal = (long)(5000 * level);
                    output.Crystal = (long)(3000 * level);
                    break;
                case Buildables.PlasmaTurret:
                    output.Metal = (long)(50000 * level);
                    output.Crystal = (long)(50000 * level);
                    output.Deuterium = (long)(30000 * level);
                    break;
                case Buildables.SmallShieldDome:
                    output.Metal = (long)(10000 * level);
                    output.Crystal = (long)(10000 * level);
                    break;
                case Buildables.LargeShieldDome:
                    output.Metal = (long)(50000 * level);
                    output.Crystal = (long)(50000 * level);
                    break;
                case Buildables.AntiBallisticMissiles:
                    break;
                case Buildables.InterplanetaryMissiles:
                    break;
                case Buildables.SmallCargo:
                    output.Metal = (long)(2000 * level);
                    output.Crystal = (long)(2000 * level);
                    break;
                case Buildables.LargeCargo:
                    output.Metal = (long)(6000 * level);
                    output.Crystal = (long)(6000 * level);
                    break;
                case Buildables.LightFighter:
                    output.Metal = (long)(3000 * level);
                    output.Crystal = (long)(1000 * level);
                    break;
                case Buildables.HeavyFighter:
                    output.Metal = (long)(6000 * level);
                    output.Crystal = (long)(4000 * level);
                    break;
                case Buildables.Cruiser:
                    output.Metal = (long)(20000 * level);
                    output.Crystal = (long)(7000 * level);
                    output.Deuterium = (long)(2000 * level);
                    break;
                case Buildables.Battleship:
                    output.Metal = (long)(35000 * level);
                    output.Crystal = (long)(15000 * level);
                    break;
                case Buildables.ColonyShip:
                    output.Metal = (long)(10000 * level);
                    output.Crystal = (long)(20000 * level);
                    output.Deuterium = (long)(10000 * level);
                    break;
                case Buildables.Recycler:
                    output.Metal = (long)(10000 * level);
                    output.Crystal = (long)(6000 * level);
                    output.Deuterium = (long)(2000 * level);
                    break;
                case Buildables.EspionageProbe:
                    output.Crystal = (long)(1000 * level);
                    break;
                case Buildables.Bomber:
                    output.Metal = (long)(50000 * level);
                    output.Crystal = (long)(25000 * level);
                    output.Deuterium = (long)(15000 * level);
                    break;
                case Buildables.SolarSatellite:
                    output.Crystal = (long)(2000 * level);
                    output.Deuterium = (long)(500 * level);
                    break;
                case Buildables.Destroyer:
                    output.Metal = (long)(60000 * level);
                    output.Crystal = (long)(50000 * level);
                    output.Deuterium = (long)(15000 * level);
                    break;
                case Buildables.Deathstar:
                    output.Metal = (long)(5000000 * level);
                    output.Crystal = (long)(4000000 * level);
                    output.Deuterium = (long)(1000000 * level);
                    break;
                case Buildables.Battlecruiser:
                    output.Metal = (long)(30000 * level);
                    output.Crystal = (long)(40000 * level);
                    output.Deuterium = (long)(15000 * level);
                    break;
                case Buildables.Crawler:
                    break;
                case Buildables.Reaper:
                    output.Metal = (long)(85000 * level);
                    output.Crystal = (long)(55000 * level);
                    output.Deuterium = (long)(20000 * level);
                    break;
                case Buildables.Pathfinder:
                    output.Metal = (long)(8000 * level);
                    output.Crystal = (long)(15000 * level);
                    output.Deuterium = (long)(8000 * level);
                    break;
                case Buildables.EspionageTechnology:
                    output.Metal = (long)(200 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(1000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(200 * Math.Pow(2, level - 1));
                    break;
                case Buildables.ComputerTechnology:
                    output.Crystal = (long)(400 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(600 * Math.Pow(2, level - 1));
                    break;
                case Buildables.WeaponsTechnology:
                    output.Metal = (long)(800 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(200 * Math.Pow(2, level - 1));
                    break;
                case Buildables.ShieldingTechnology:
                    output.Metal = (long)(200 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(600 * Math.Pow(2, level - 1));
                    break;
                case Buildables.ArmourTechnology:
                    output.Metal = (long)(1000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.EnergyTechnology:
                    output.Crystal = (long)(800 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(400 * Math.Pow(2, level - 1));
                    break;
                case Buildables.HyperspaceTechnology:
                    output.Crystal = (long)(4000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(2000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.CombustionDrive:
                    output.Metal = (long)(400 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(600 * Math.Pow(2, level - 1));
                    break;
                case Buildables.ImpulseDrive:
                    output.Metal = (long)(2000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(4000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(600 * Math.Pow(2, level - 1));
                    break;
                case Buildables.HyperspaceDrive:
                    output.Metal = (long)(10000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(20000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(6000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.LaserTechnology:
                    output.Metal = (long)(200 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(100 * Math.Pow(2, level - 1));
                    break;
                case Buildables.IonTechnology:
                    output.Metal = (long)(1000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(300 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(100 * Math.Pow(2, level - 1));
                    break;
                case Buildables.PlasmaTechnology:
                    output.Metal = (long)(2000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(4000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(1000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.IntergalacticResearchNetwork:
                    output.Metal = (long)(240000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(400000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(160000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.Astrophysics:
                    output.Metal = (long)(4000 * Math.Pow(2, level - 1));
                    output.Crystal = (long)(8000 * Math.Pow(2, level - 1));
                    output.Deuterium = (long)(4000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.GravitonTechnology:
                    output.Energy = (long)(300000 * Math.Pow(2, level - 1));
                    break;
                case Buildables.Null:
                default:
                    break;
            }

            return output;
        }

        /*Tralla 12/2/2020
         * 
         * Added helper to calc delta
         * Hotfix to autominer energy builder
         */
        public static long GetRequiredEnergyDelta(Buildables buildable, int level)
        {
            if (buildable == Buildables.MetalMine || buildable == Buildables.CrystalMine || buildable == Buildables.DeuteriumSynthesizer)
            {
                if (level > 1)
                {
                    var prevLevelResources = CalcPrice(buildable, level - 1);
                    var thisLevelResources = CalcPrice(buildable, level);
                    return thisLevelResources.Energy - prevLevelResources.Energy;
                }
                else return CalcPrice(buildable, 1).Energy;
            }
            else return 0;
        }

        public static int GetNextLevel(Planet planet, Buildables buildable)
        {
            int output = 0;
            if (buildable == Buildables.SolarSatellite)
                output = CalcNeededSolarSatellites(planet);
            if (output == 0)
            {
                foreach (PropertyInfo prop in planet.Buildings.GetType().GetProperties())
                {
                    if (prop.Name == buildable.ToString())
                    {
                        output = (int)prop.GetValue(planet.Buildings) + 1;
                    }
                }
            }
            if (output == 0)
            {
                foreach (PropertyInfo prop in planet.Facilities.GetType().GetProperties())
                {
                    if (prop.Name == buildable.ToString())
                    {
                        output = (int)prop.GetValue(planet.Facilities) + 1;
                    }
                }
            }
            return output;
        }

        public static long CalcDepositCapacity(int level)
        {
            return 5000 * (long)(2.5 * Math.Pow(Math.E, (20 * level / 33)));
        }

        public static bool ShouldBuildMetalStorage(Planet planet, int maxLevel, int speedFactor, int hours = 12, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            long metalProduction = CalcMetalProduction(planet, speedFactor, ratio, researches, playerClass, hasGeologist, hasStaff);
            long metalCapacity = CalcDepositCapacity(planet.Buildings.MetalStorage);
            if (metalCapacity < hours * metalProduction && GetNextLevel(planet, Buildables.MetalStorage) < maxLevel)
                return true;
            else
                return false;
        }

        public static bool ShouldBuildCrystalStorage(Planet planet, int maxLevel, int speedFactor, int hours = 12, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            long crystalProduction = CalcCrystalProduction(planet, speedFactor, ratio, researches, playerClass, hasGeologist, hasStaff);
            long crystalCapacity = CalcDepositCapacity(planet.Buildings.CrystalStorage);
            if (crystalCapacity < hours * crystalProduction && GetNextLevel(planet, Buildables.CrystalStorage) < maxLevel)
                return true;
            else
                return false;
        }

        public static bool ShouldBuildDeuteriumTank(Planet planet, int maxLevel, int speedFactor, int hours = 12, float ratio = 1, Researches researches = null, Classes playerClass = Classes.NoClass, bool hasGeologist = false, bool hasStaff = false)
        {
            long deuteriumProduction = CalcDeuteriumProduction(planet, speedFactor, ratio, researches, playerClass, hasGeologist, hasStaff);
            long deuteriumCapacity = CalcDepositCapacity(planet.Buildings.DeuteriumTank);
            if (deuteriumCapacity < hours * deuteriumProduction && GetNextLevel(planet, Buildables.DeuteriumTank) < maxLevel)
                return true;
            else
                return false;
        }

        public static bool ShouldBuildEnergySource(Planet planet)
        {
            if (planet.Resources.Energy < 0)
                return true;
            else
                return false;
        }

        public static Buildables GetNextEnergySourceToBuild(Planet planet, int maxSolarPlant, int maxFusionReactor)
        {
            if (planet.Buildings.SolarPlant < maxSolarPlant)
                return Buildables.SolarPlant;
            if (planet.Buildings.FusionReactor < maxFusionReactor)
                return Buildables.FusionReactor;
            return Buildables.SolarSatellite;
        }

        public static int GetSolarSatelliteOutput(Planet planet)
        {
            return (int)Math.Round((planet.Temperature.Average + 160) / 6);
        }

        public static int CalcNeededSolarSatellites(Planet planet)
        {
            if (planet.Resources.Energy > 0)
                return 0;
            return (int)Math.Round((float)(Math.Abs(planet.Resources.Energy) / GetSolarSatelliteOutput(planet)), MidpointRounding.ToPositiveInfinity);
        }

        public static Buildables GetNextMineToBuild(Planet planet, int maxMetalMine = 100, int maxCrystalMine = 100, int maxDeuteriumSynthetizer = 100)
        {
            var mines = new List<Buildables> { Buildables.MetalMine, Buildables.CrystalMine, Buildables.DeuteriumSynthesizer };
            var dic = new Dictionary<Buildables, long>();
            foreach (var mine in mines)
            {
                if (mine == Buildables.MetalMine && GetNextLevel(planet, mine) > maxMetalMine)
                    continue;
                if (mine == Buildables.CrystalMine && GetNextLevel(planet, mine) > maxCrystalMine)
                    continue;
                if (mine == Buildables.DeuteriumSynthesizer && GetNextLevel(planet, mine) > maxDeuteriumSynthetizer)
                    continue;

                dic.Add(mine, CalcPrice(mine, GetNextLevel(planet, mine)).ConvertedDeuterium);
            }
            if (dic.Count == 0)
                return Buildables.Null;

            dic = dic.OrderBy(m => m.Value)
                .ToDictionary(m => m.Key, m => m.Value);
            return dic.FirstOrDefault().Key;
        }

        public static bool ShouldBuildRoboticFactory(Planet planet, int maxLevel = 10)
        {
            var nextMine = GetNextMineToBuild(planet);
            var nextMineLevel = GetNextLevel(planet, nextMine);
            var nextMinePrice = CalcPrice(nextMine, nextMineLevel);

            var nextRobotsLevel = GetNextLevel(planet, Buildables.RoboticsFactory);
            var nextRobotsPrice = CalcPrice(Buildables.RoboticsFactory, nextRobotsLevel);

            if (nextRobotsLevel < maxLevel && nextMinePrice.ConvertedDeuterium > nextRobotsPrice.ConvertedDeuterium)
                return true;
            else
                return false;
        }

        public static bool ShouldBuildShipyard(Planet planet, int maxLevel = 12)
        {
            var nextMine = GetNextMineToBuild(planet);
            var nextMineLevel = GetNextLevel(planet, nextMine);
            var nextMinePrice = CalcPrice(nextMine, nextMineLevel);

            var nextShipyardLevel = GetNextLevel(planet, Buildables.Shipyard);
            var nextShipyardPrice = CalcPrice(Buildables.Shipyard, nextShipyardLevel);

            if (nextShipyardLevel < maxLevel && nextMinePrice.ConvertedDeuterium > nextShipyardPrice.ConvertedDeuterium && planet.Facilities.RoboticsFactory >= 2)
                return true;
            else
                return false;
        }

        public static bool ShouldBuildNanites(Planet planet, int maxLevel = 10)
        {
            var nextMine = GetNextMineToBuild(planet);
            var nextMineLevel = GetNextLevel(planet, nextMine);
            var nextMinePrice = CalcPrice(nextMine, nextMineLevel);

            var nextNanitesLevel = GetNextLevel(planet, Buildables.NaniteFactory);
            var nextNanitesPrice = CalcPrice(Buildables.NaniteFactory, nextNanitesLevel);

            if (nextNanitesLevel < maxLevel && nextMinePrice.ConvertedDeuterium > nextNanitesPrice.ConvertedDeuterium && planet.Facilities.RoboticsFactory >= 10)
                return true;
            else
                return false;
        }
    }
}
