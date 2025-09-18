using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Robot.Common;


// Я вже вам здав цю лабу, проте ви дали мені вибір -
// або я додаю використання атррибитів і отримую 4 бали,
// або не додаю і отримую 2 бали. Але ви не встигли її переглянути =(



namespace ChornyiAndrii.RobotChallange.Test
{
    [TestClass]
    public class ChornyiAnndriiAlgorithmTests
    {
        private ChornyiAnndriiAlgorithm algorithm;

        [TestInitialize]
        public void Setup()
        {
            algorithm = new ChornyiAnndriiAlgorithm();
        }

        [TestMethod]
        [DataRow(10, 10, 11, 11, 300, typeof(CreateNewRobotCommand), DisplayName = "High Energy Near Station")]
        [DataRow(10, 10, 11, 11, 100, typeof(CollectEnergyCommand), DisplayName = "Low Energy Near Station")]
        [DataRow(0, 0, 10, 10, 100, typeof(MoveCommand), DisplayName = "Far From Station")]
        public void DoStep_ParameterizedTest(int robotX, int robotY, int stationX, int stationY, int energy, Type expectedType)
        {
            var robots = new List<Robot.Common.Robot>
            {
                new Robot.Common.Robot
                {
                    Position = new Position { X = robotX, Y = robotY },
                    Energy = energy,
                    OwnerName = "Chornyi Andrii"
                }
            };

            var stations = new List<EnergyStation>
            {
                new EnergyStation { Position = new Position { X = stationX, Y = stationY } }
            };

            var map = new Map { Stations = stations };
            var result = algorithm.DoStep(robots, 0, map);
            
            Assert.IsInstanceOfType(result, expectedType);
            
            if (expectedType == typeof(MoveCommand))
            {
                var moveCommand = (MoveCommand)result;
                Assert.AreEqual(1, moveCommand.NewPosition.X);
                Assert.AreEqual(1, moveCommand.NewPosition.Y);
            }
        }

        [TestMethod]
        public void DoStep_NoAvailableStations_ReturnsNull()
        {
            var robots = new List<Robot.Common.Robot>
            {
                new Robot.Common.Robot { Position = new Position { X = 0, Y = 0 }, OwnerName = "Chornyi Andrii" },
                new Robot.Common.Robot { Position = new Position { X = 10, Y = 10 }, OwnerName = "Chornyi Andrii" },
                new Robot.Common.Robot { Position = new Position { X = 11, Y = 11 }, OwnerName = "Chornyi Andrii" }
            };

            var stations = new List<EnergyStation>
            {
                new EnergyStation { Position = new Position { X = 10, Y = 10 } }
            };

            var map = new Map { Stations = stations };
            var result = algorithm.DoStep(robots, 0, map);
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow(10, 10, 10, 10, true, DisplayName = "Robot at Exact Station Position")]
        [DataRow(10, 10, 12, 12, true, DisplayName = "Robot Within Range")]
        [DataRow(10, 10, 13, 13, false, DisplayName = "Robot Outside Range")]
        public void IsNearStation_ParameterizedTest(int robotX, int robotY, int stationX, int stationY, bool expected)
        {
            var robot = new Robot.Common.Robot
            {
                Position = new Position { X = robotX, Y = robotY }
            };

            var stations = new List<EnergyStation>
            {
                new EnergyStation { Position = new Position { X = stationX, Y = stationY } }
            };

            var result = algorithm.IsNearStation(robot, stations);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(5, 5, 10, 10, 6, 6, DisplayName = "Move Right and Down")]
        [DataRow(5, 5, 5, 5, 5, 5, DisplayName = "Stay in Place")]
        [DataRow(5, 10, 10, 10, 6, 10, DisplayName = "Move Horizontally")]
        public void StepTowards_ParameterizedTest(int fromX, int fromY, int toX, int toY, int expectedX, int expectedY)
        {
            var from = new Position { X = fromX, Y = fromY };
            var to = new Position { X = toX, Y = toY };
            var result = algorithm.StepTowards(from, to);
            
            Assert.AreEqual(expectedX, result.X);
            Assert.AreEqual(expectedY, result.Y);
        }

        [TestMethod]
        public void FindTargetStation_NoRobotsNearStations_ReturnsClosestStation()
        {
            var movingRobot = new Robot.Common.Robot
            {
                Position = new Position { X = 0, Y = 0 },
                OwnerName = "Chornyi Andrii"
            };

            var stations = new List<EnergyStation>
            {
                new EnergyStation { Position = new Position { X = 5, Y = 5 } },
                new EnergyStation { Position = new Position { X = 3, Y = 3 } },
                new EnergyStation { Position = new Position { X = 10, Y = 10 } }
            };

            var allRobots = new List<Robot.Common.Robot> { movingRobot };

            var result = algorithm.FindTargetStation(movingRobot, stations, allRobots);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Position.X);
            Assert.AreEqual(3, result.Position.Y);
        }

        [TestMethod]
        public void FindTargetStation_MaxRobotsReached_ReturnsAlternativeStation()
        {
            var movingRobot = new Robot.Common.Robot
            {
                Position = new Position { X = 0, Y = 0 },
                OwnerName = "Chornyi Andrii"
            };

            var stations = new List<EnergyStation>
            {
                new EnergyStation { Position = new Position { X = 3, Y = 3 } },
                new EnergyStation { Position = new Position { X = 10, Y = 10 } }
            };

            var allRobots = new List<Robot.Common.Robot>
            {
                movingRobot,
                new Robot.Common.Robot { Position = new Position { X = 3, Y = 3 }, OwnerName = "Chornyi Andrii" },
                new Robot.Common.Robot { Position = new Position { X = 4, Y = 4 }, OwnerName = "Chornyi Andrii" }
            };

            var result = algorithm.FindTargetStation(movingRobot, stations, allRobots, maxRobotsPerStation: 2);

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Position.X);
            Assert.AreEqual(10, result.Position.Y);
        }

        [TestMethod]
        public void FindTargetStation_AllStationsOccupied_ReturnsNull()
        {
            var movingRobot = new Robot.Common.Robot
            {
                Position = new Position { X = 0, Y = 0 },
                OwnerName = "Chornyi Andrii"
            };

            var stations = new List<EnergyStation>
            {
                new EnergyStation { Position = new Position { X = 3, Y = 3 } }
            };

            var allRobots = new List<Robot.Common.Robot>
            {
                movingRobot,
                new Robot.Common.Robot { Position = new Position { X = 3, Y = 3 }, OwnerName = "Chornyi Andrii" },
                new Robot.Common.Robot { Position = new Position { X = 4, Y = 4 }, OwnerName = "Chornyi Andrii" }
            };

            var result = algorithm.FindTargetStation(movingRobot, stations, allRobots, maxRobotsPerStation: 2);

            Assert.IsNull(result);
        }
    }
}