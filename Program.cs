using System;
using System.Collections.Generic;

namespace CSLight
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Battlefield battlefield = new Battlefield();
            battlefield.ToBattle();
        }
    }

    class Battlefield
    {
        private Creater _creater;
        private List<Soldier> _squad1;
        private List<Soldier> _squad2;

        public Battlefield()
        {
            _creater = new Creater();
            _squad1 = new List<Soldier>();
            _squad2 = new List<Soldier>();

            SoldiersCount = 15;
            FillSquad(_squad1, SoldiersCount);
            FillSquad(_squad2, SoldiersCount);
        }

        public int SoldiersCount { get; private set; }

        public void ToBattle()
        {
            while (_squad1.Count > 0 && _squad2.Count > 0)
            {
                Random random = new Random();

                _squad1[random.Next(0, _squad1.Count)].Attack(_squad2);
                _squad2[random.Next(0, _squad2.Count)].Attack(_squad1);

                ChangePosition();

                Console.Clear();
                ShowSquadsStatus();
            }

            if (_squad1.Count == 0 && _squad2.Count == 0)
            {
                Console.WriteLine("Ничья!");
            }
            else if (_squad2.Count == 0)
            {
                Console.WriteLine("Победил первый игрок");
            }
            else
            {
                Console.WriteLine("Победил второй игрок");
            }

            Console.Read();
        }

        public void ChangePosition()
        {
            for (int i = _squad1.Count - 1; i >= 0; i--)
            {
                if (_squad1[i].Health <= 0)
                {
                    _squad1.RemoveAt(i);
                }
            }

            for (int i = _squad2.Count - 1; i >= 0; i--)
            {
                if (_squad2[i].Health <= 0)
                {
                    _squad2.RemoveAt(i);
                }
            }
        }

        public void ShowSquadsStatus()
        {
            Console.WriteLine("Первая команда");

            for (int i = 0; i < _squad1.Count; i++)
            {
                Console.WriteLine($"{_squad1[i].TypeOfEquipment} - {_squad1[i].Health} HP");
            }

            Console.WriteLine("\nВторая команда");

            for (int i = 0; i < _squad2.Count; i++)
            {
                Console.WriteLine($"{_squad2[i].TypeOfEquipment} - {_squad2[i].Health} HP");
            }

            Console.ReadLine();
        }

        private void FillSquad(List<Soldier> squad, int soldiersCount)
        {
            Random random = new Random();

            for (int i = 0; i < soldiersCount; i++)
            {
                int typeOfSoldier = random.Next(0, 3);

                switch (typeOfSoldier)
                {
                    case 0:
                        squad.Add(_creater.CreateGranateLauncherGunner());
                        break;
                    case 1:
                        squad.Add(_creater.CreateMachineGunner());
                        break;
                    case 2:
                        squad.Add(_creater.CreateSniper());
                        break;
                }
            }
        }
    }

    class Creater
    {
        public Soldier CreateGranateLauncherGunner()
        {
            Random random = new Random();

            string typeOfEquipment = "GranateLauncherGunner";
            int health = 4;
            int radiusOfDefeat = 3;
            int damage = 3;

            return new Soldier(typeOfEquipment, health, radiusOfDefeat, damage);
        }

        public Soldier CreateMachineGunner()
        {
            Random random = new Random();

            string typeOfEquipment = "MachineGunner";
            int health = 5;
            int radiusOfDefeat = 1;
            int damage = 1;

            return new Soldier(typeOfEquipment, health, radiusOfDefeat, damage);
        }

        public Soldier CreateSniper()
        {
            Random random = new Random();

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

        public void Attack(List<Soldier> squad)
        {
            Random random = new Random();

            int numberOfAttackedSoldier = random.Next(0, squad.Count);

            int howMuchSoldiersInLeft = numberOfAttackedSoldier;
            int howMuchSoldiersInRight = squad.Count - numberOfAttackedSoldier - 1;

            if (RadiusOfDefeat > howMuchSoldiersInLeft)
            {
                int leftDamagedSpase = howMuchSoldiersInLeft;

                for (int i = 1; i <= leftDamagedSpase; i++)
                {
                    squad[numberOfAttackedSoldier - i].TakeDamage(Damage);
                }
            }
            else
            {
                int leftDamagedSpase = RadiusOfDefeat;

                for (int i = 1; i <= leftDamagedSpase; i++)
                {
                    squad[numberOfAttackedSoldier - i].TakeDamage(Damage);
                }
            }

            if (RadiusOfDefeat > howMuchSoldiersInRight)
            {
                int rightDamagedSpase = howMuchSoldiersInRight;

                for (int i = 1; i <= rightDamagedSpase; i++)
                {
                    squad[numberOfAttackedSoldier + i].TakeDamage(Damage);
                }
            }
            else
            {
                int rightDamagedSpase = RadiusOfDefeat;

                for (int i = 1; i <= rightDamagedSpase; i++)
                {
                    squad[numberOfAttackedSoldier + i].TakeDamage(Damage);
                }
            }

            squad[numberOfAttackedSoldier].TakeDamage(Damage);
        }

        public void TakeDamage(int damage)
        {
            Health -= Damage;
        }
    }
}