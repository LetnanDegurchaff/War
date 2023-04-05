using System;
using System.Collections.Generic;

namespace CSLight
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Battlefield battlefield = new Battlefield();
            battlefield.Battle();
        }
    }

    class Battlefield
    {
        private Squad _squad1;
        private Squad _squad2;

        public Battlefield()
        {
            Random random = new Random();
            _squad1 = new Squad(random);
            _squad2 = new Squad(random);
        }

        public void Battle()
        {
            while (_squad1.GetCount() > 0 && _squad2.GetCount() > 0)
            {
                _squad1.Attack(_squad2);
                _squad2.RemoveDeadSoldiers();
                ShowSquadsStatus();

                _squad2.Attack(_squad1);
                _squad1.RemoveDeadSoldiers();
                ShowSquadsStatus();
            }

            ShowResultOfBattle();

            Console.Read();
        }

        public void ShowResultOfBattle()
        {
            if (_squad1.GetCount() == 0 && _squad2.GetCount() == 0)
            {
                Console.WriteLine("Ничья!");
            }
            else if (_squad2.GetCount() == 0)
            {
                Console.WriteLine("Победил первый отряд");
            }
            else if (_squad1.GetCount() == 0)
            {
                Console.WriteLine("Победил второй отряд");
            }
        }

        public void ShowSquadsStatus()
        {
            Console.Clear();

            Console.WriteLine("Первая команда\n");
            _squad1.ShowStatus();

            Console.WriteLine("\n\nВторая команда\n");
            _squad2.ShowStatus();
            Console.ReadLine();
        }
    }

    class Squad
    {
        private List<Soldier> _soldiers;
        private SoldierCreater _soldierCreater;
        private Random _random;

        public Squad(Random random)
        {
            _soldierCreater = new SoldierCreater();
            _soldiers = new List<Soldier>();
            _random = random;

            int soldiersCount = 15;
            Fill(soldiersCount);
        }

        //public void Attack(Squad enemySquad)
        //{
        //    if (GetCount() > 0 && enemySquad.GetCount() > 0)
        //        _soldiers[_random.Next(0, GetCount())].Attack(enemySquad.GetAttackedSoldiers());
        //}

        //public List<Soldier> GetAttackedSoldiers()
        //{
        //    return _soldiers;
        //}

        public void Attack(Squad enemySquad)
        {
            if (GetCount() > 0 && enemySquad.GetCount() > 0)
                _soldiers[_random.Next(0, GetCount())].Attack(enemySquad);
        }

        public void TakeDamage(int damage)
        {
            Random random = new Random();

            int numberOfAttackedSoldier = random.Next(0, _soldiers.Count);

            int soldiersCountFromLeft = numberOfAttackedSoldier;
            int soldiersCountFromRight = _soldiers.Count - numberOfAttackedSoldier - 1;

            int leftDamagedSpase = _soldiers[numberOfAttackedSoldier].GetCoordinates(soldiersCountFromLeft);

            for (int i = 1; i <= leftDamagedSpase; i++)
            {
                _soldiers[numberOfAttackedSoldier - i].TakeDamage(damage);
            }

            int rightDamagedSpase = _soldiers[numberOfAttackedSoldier].GetCoordinates(soldiersCountFromRight);

            for (int i = 1; i <= rightDamagedSpase; i++)
            {
                _soldiers[numberOfAttackedSoldier + i].TakeDamage(damage);
            }

            _soldiers[numberOfAttackedSoldier].TakeDamage(damage);
        }

        public void RemoveDeadSoldiers()
        {
            for (int i = _soldiers.Count - 1; i >= 0; i--)
            {
                if (_soldiers[i].Health <= 0)
                {
                    _soldiers.RemoveAt(i);
                }
            }
        }

        public void ShowStatus()
        {
            for (int i = 0; i < _soldiers.Count; i++)
            {
                Console.WriteLine($"{_soldiers[i].TypeOfEquipment} - {_soldiers[i].Health} HP");
            }
        }

        public int GetCount()
        {
            return _soldiers.Count;
        }

        private void Fill(int soldiersCount)
        {
            Random random = new Random();

            for (int i = 0; i < soldiersCount; i++)
            {
                _soldiers.Add(_soldierCreater.CreateRandomSoldier(random));
            }
        }
    }

    class SoldierCreater
    {
        public Soldier CreateRandomSoldier(Random random)
        {
            List<Soldier> soldiers = new List<Soldier>();

            soldiers.Add(CreateGranateLauncherGunner(random));
            soldiers.Add(CreateMachineGunner(random));
            soldiers.Add(CreateSniper(random));

            return soldiers[random.Next(0, soldiers.Count)];
        }

        public Soldier CreateGranateLauncherGunner(Random random)
        {
            string typeOfEquipment = "GranateLauncherGunner";
            int health = 4;
            int radiusOfDefeat = 3;
            int damage = 3;

            return new Soldier(typeOfEquipment, health, radiusOfDefeat, damage);
        }

        public Soldier CreateMachineGunner(Random random)
        {
            string typeOfEquipment = "MachineGunner";
            int health = 5;
            int radiusOfDefeat = 1;
            int damage = 1;

            return new Soldier(typeOfEquipment, health, radiusOfDefeat, damage);
        }

        public Soldier CreateSniper(Random random)
        {
            string typeOfEquipment = "Sniper";
            int health = 1;
            int radiusOfDefeat = 0;
            int damage = 6;

            return new Soldier(typeOfEquipment, health, radiusOfDefeat, damage);
        }
    }

    class Soldier
    {
        public Soldier(string typeOfEquipment, int health, int radiusOfDefeat, int damage)
        {
            TypeOfEquipment = typeOfEquipment;
            Health = health;
            RadiusOfDefeat = radiusOfDefeat;
            Damage = damage;
        }

        public string TypeOfEquipment { get; private set; }
        public int Health { get; private set; }
        public int RadiusOfDefeat { get; private set; }
        public int Damage { get; private set; }

        public void Attack(Squad enemySquad)
        {
            enemySquad.TakeDamage(Damage);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public int GetCoordinates(int soldiersCountFromSide)
        {
            if (RadiusOfDefeat > soldiersCountFromSide)
            {
                return soldiersCountFromSide;
            }
            else
            {
                return RadiusOfDefeat;
            }
        }
    }
}