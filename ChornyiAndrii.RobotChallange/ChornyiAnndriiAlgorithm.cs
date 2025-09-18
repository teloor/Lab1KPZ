using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChornyiAndrii.RobotChallange
{
    public class ChornyiAnndriiAlgorithm : IRobotAlgorithm
    {
        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var myRobot = robots[robotToMoveIndex];

            // Якщо робот біля станції -> збирає енергію або створює нового
            if (IsNearStation(myRobot, map.Stations))
            {
                if (myRobot.Energy > 280 && robots.Count < 100)
                {
                    return new CreateNewRobotCommand();
                }

                return new CollectEnergyCommand();
            }

            // Інакше рухаємося до станції
            var target = FindTargetStation(myRobot, map.Stations, robots, maxRobotsPerStation: 2);
            if (target != null)
            {
                var step = StepTowards(myRobot.Position, target.Position);
                return new MoveCommand() { NewPosition = step };
            }

            return null;
        }


        public bool IsNearStation(Robot.Common.Robot robot, IList<EnergyStation> stations)
        {
            return stations.Any(s =>
                Math.Abs(s.Position.X - robot.Position.X) <= 2 &&
                Math.Abs(s.Position.Y - robot.Position.Y) <= 2);
        }

        // Пошук найближчої станції для робота з урахуванням ліміту на кількість роботів біля станції
        public EnergyStation FindTargetStation(
            Robot.Common.Robot movingRobot,          // робот, для якого шукаємо ціль
            IList<EnergyStation> stations,           // усі енергостанції на карті
            IList<Robot.Common.Robot> allRobots,     // усі роботи (свої і чужі)
            int maxRobotsPerStation = 2)             // максимум моїх роботів біля однієї станції
        {
            EnergyStation best = null;               // найкраща (найближча) станція
            int minDist = int.MaxValue;              // мінімальна відстань (початково дуже велика)

            foreach (var s in stations)              // перебираємо всі станції
            {
                // рахуємо, скільки моїх роботів вже стоїть в радіусі 2 клітинок від станції
                int nearMyRobots = allRobots
                    .Count(r => r.OwnerName == movingRobot.OwnerName &&
                                Math.Abs(r.Position.X - s.Position.X) <= 2 &&
                                Math.Abs(r.Position.Y - s.Position.Y) <= 2);

                // якщо біля станції вже достатньо моїх роботів → пропускаємо її
                if (nearMyRobots >= maxRobotsPerStation)
                    continue;

                // обчислюємо квадрат відстані до станції (дешевше, ніж справжня відстань з коренем)
                int d = (int)(Math.Pow(movingRobot.Position.X - s.Position.X, 2) +
                              Math.Pow(movingRobot.Position.Y - s.Position.Y, 2));

                // якщо ця станція ближча за попередні → запам'ятовуємо її
                if (d < minDist)
                {
                    minDist = d;
                    best = s;
                }
            }

            return best; // повертаємо найближчу станцію, що ще не переповнена
        }

        // Рух робота на один крок у напрямку до цільової точки
        public Position StepTowards(Position from, Position to)
        {
            var step = new Position() { X = from.X, Y = from.Y };

            // Рухаємося по X: вліво або вправо на 1 клітинку
            if (to.X > from.X) step.X++;
            else if (to.X < from.X) step.X--;

            // Рухаємося по Y: вгору або вниз на 1 клітинку
            if (to.Y > from.Y) step.Y++;
            else if (to.Y < from.Y) step.Y--;

            return step; // нова позиція на 1 крок ближче до цілі
        }



        public string Author
        {
            get { return "Chornyi Andrii"; }
        }

        public string Description
        {
            get { return "s"; }
        }
    }
}