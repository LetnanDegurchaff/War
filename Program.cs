using System;
using System.Collections.Generic;

namespace CSLight
{
    static class UserUtils
    {
        private static Random _random = new Random();

        public static int GenerateRandomNumber(int min, int max) =>
            _random.Next(min, max);
    }

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
        private Squad _squad1 = new Squad();
        private Squad _squad2 = new Squad();

        public void Battle()
        {
            while (_squad1.SoldiersCount > 0 && _squad2.SoldiersCount > 0)
            {
                _squad1.Attack(_squad2);
                _squad2.RemoveDeadSoldiers();
                ShowSquadsStatus();

                _squad2.Attack(_squad1);
                _squad1.RemoveDeadSoldiers();
                ShowSquadsStatus();
            }

            ShowBattleResults();

            Console.Read();
        }

        private void ShowSquadsStatus()
        {
            Console.Clear();

            Console.WriteLine("Первый отряд\n");
            _squad1.ShowStatus();

            Console.WriteLine("\n\nВторой отряд\n");
            _squad2.ShowStatus();
            Console.ReadLine();
        }

        private void ShowBattleResults()
        {
            if (_squad1.SoldiersCount == 0 && _squad2.SoldiersCount == 0)
            {
                Console.WriteLine("Ничья!");
            }
            else if (_squad2.SoldiersCount == 0)
            {
                Console.WriteLine("Победил первый отряд");
            }
            else
            {
                Console.WriteLine("Победил второй отряд");
            }
        }
    }

    interface IDamagebleReadOnlySquad
    {
        IReadOnlyList<IDamageble> GetAttackedSoldiers();
    }

    class Squad : IDamagebleReadOnlySquad
    {
        private SoldierCreator _soldierCreator = new SoldierCreator();
        private List<Soldier> _soldiers = new List<Soldier>();
        private int _soldiersCount = 15;

        public Squad()
        {
            for (int i = 0; i < _soldiersCount; i++)
            {
                _soldiers.Add(_soldierCreator.CreateRandomSoldier());
            }
        }

        public int SoldiersCount => _soldiers.Count;

        public IReadOnlyList<IDamageble> GetAttackedSoldiers() => _soldiers;
        

        public void Attack(IDamagebleReadOnlySquad enemySquad)
        {
            if (SoldiersCount > 0 && enemySquad.GetAttackedSoldiers().Count > 0)
                _soldiers[UserUtils.GenerateRandomNumber(0, SoldiersCount)]
                    .Attack(enemySquad.GetAttackedSoldiers());
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
                Console.WriteLine($"{_soldiers[i].GetType().Name} - {_soldiers[i].Health} HP");
            }
        }
    }

    #region Soldier

    interface IDamageble
    {
        void TakeDamage(int damage);
    }

    abstract class Soldier : IDamageble
    {
        public Soldier(int health, int damage)
        {
            (Health, Damage) = (health, damage);
        }

        public int Health { get; private set; }
        public int Damage { get; private set; }

        public virtual void Attack(IReadOnlyList<IDamageble> enemySoldiers)
        {
            enemySoldiers[UserUtils.GenerateRandomNumber(0, enemySoldiers.Count)]
                .TakeDamage(Damage);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }

    class GranateLauncherGunner : Soldier
    {
        private int _radiusOfDefeat = 3;

        public GranateLauncherGunner(int RadiusOfDefeat) : base(4, 3)
        {
            _radiusOfDefeat = RadiusOfDefeat;
        }

        public override void Attack(IReadOnlyList<IDamageble> enemySoldiers)
        {
            int numberOfAttackedSoldier = UserUtils.GenerateRandomNumber(0, enemySoldiers.Count);

            int soldiersCountFromLeft = numberOfAttackedSoldier;
            int soldiersCountFromRight = enemySoldiers.Count - numberOfAttackedSoldier - 1;

            int leftDamagedSpase = GetCoordinates(soldiersCountFromLeft);

            for (int i = 1; i <= leftDamagedSpase; i++)
            {
                enemySoldiers[numberOfAttackedSoldier - i].TakeDamage(Damage);
            }

            int rightDamagedSpase = GetCoordinates(soldiersCountFromRight);

            for (int i = 1; i <= rightDamagedSpase; i++)
            {
                enemySoldiers[numberOfAttackedSoldier + i].TakeDamage(Damage);
            }

            enemySoldiers[numberOfAttackedSoldier].TakeDamage(Damage);
        }

        private int GetCoordinates(int soldiersCountFromSide)
        {
            if (_radiusOfDefeat > soldiersCountFromSide)
            {
                return soldiersCountFromSide;
            }
            else
            {
                return _radiusOfDefeat;
            }
        }
    }

    class MachineGunner : Soldier
    {
        public MachineGunner() : base(5, 2) { }
    }

    class Sniper : Soldier
    {
        public Sniper() : base(1, 6) { }
    }

    #endregion Soldier

    class SoldierCreator
    {
        private Dictionary<Type, Func<Soldier>> _creatorsDictionary = new Dictionary<Type, Func<Soldier>>();
        private List<Type> _types = new List<Type>();

        public SoldierCreator()
        {
            _creatorsDictionary.Add(typeof(GranateLauncherGunner), CreateGranateLauncherGunner);
            _creatorsDictionary.Add(typeof(MachineGunner), CreateMachineGunner);
            _creatorsDictionary.Add(typeof(Sniper), CreateSniper);

            foreach (KeyValuePair<Type, Func<Soldier>> creator in _creatorsDictionary)
            {
                _types.Add(creator.Key);
            }
        }

        public Soldier CreateRandomSoldier()
        {
            Type randomSoldierType = _types[UserUtils.GenerateRandomNumber(0, _types.Count)];

            return _creatorsDictionary[randomSoldierType]();
        }

        private Soldier CreateGranateLauncherGunner()
        {
            int radiusOfDefeat = 3;

            return new GranateLauncherGunner(radiusOfDefeat);
        }

        private Soldier CreateMachineGunner() => new MachineGunner();

        private Soldier CreateSniper() => new Sniper();
    }
}