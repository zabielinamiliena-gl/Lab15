using System;

namespace BukovynaAirport
{
    class Journal
    {
        private int minutes;

        public Journal(int startHour, int startMinute)
        {
            minutes = startHour * 60 + startMinute;
        }

        public void Write(string source, string message)
        {
            int hour = minutes / 60;
            int minute = minutes % 60;
            Console.WriteLine("[" + hour.ToString("00") + ":" + minute.ToString("00") + "] [" + source + "] " + message);
            minutes += 1;
        }
    }

    abstract class FlightService
    {
        public string Name { get; private set; }

        protected FlightService(string name)
        {
            Name = name;
        }

        public void Serve(Journal journal)
        {
            Accept(journal);
            Unload(journal);
            CleanCabin(journal);
            Prepare(journal);
            Load(journal);
            Release(journal);
        }

        private void Accept(Journal journal)
        {
            journal.Write(Name, "Етап: борт прийнято");
        }

        protected abstract void Unload(Journal journal);

        protected virtual void CleanCabin(Journal journal)
        {
            journal.Write(Name, "Етап: прибирання салону пропущено");
        }

        private void Prepare(Journal journal)
        {
            journal.Write(Name, "Етап: борт підготовлено до вильоту");
        }

        protected abstract void Load(Journal journal);

        private void Release(Journal journal)
        {
            journal.Write(Name, "Етап: борт випущено");
        }
    }

    class PassengerFlightService : FlightService
    {
        public PassengerFlightService(string flightNumber) : base("Пасажирський " + flightNumber) { }

        protected override void Unload(Journal journal)
        {
            journal.Write(Name, "Етап: вивантажено пасажирів і багаж");
        }

        protected override void CleanCabin(Journal journal)
        {
            journal.Write(Name, "Етап: салон прибрано");
        }

        protected override void Load(Journal journal)
        {
            journal.Write(Name, "Етап: завантажено пасажирів і багаж");
        }
    }

    class CargoFlightService : FlightService
    {
        public CargoFlightService(string flightNumber) : base("Вантажний " + flightNumber) { }

        protected override void Unload(Journal journal)
        {
            journal.Write(Name, "Етап: вивантажено вантаж");
        }

        protected override void Load(Journal journal)
        {
            journal.Write(Name, "Етап: завантажено вантаж");
        }
    }

    class CharterFlightService : FlightService
    {
        public CharterFlightService(string flightNumber) : base("Чартерний " + flightNumber) { }

        protected override void Unload(Journal journal)
        {
            journal.Write(Name, "Етап: вивантажено чартерну групу");
        }

        protected override void Load(Journal journal)
        {
            journal.Write(Name, "Етап: завантажено чартерну групу");
        }
    }

    class SanitaryFlightService : FlightService
    {
        public SanitaryFlightService(string flightNumber) : base("Санітарний " + flightNumber) { }

        protected override void Unload(Journal journal)
        {
            journal.Write(Name, "Етап: вивантажено пацієнтів і обладнання");
        }

        protected override void CleanCabin(Journal journal)
        {
            journal.Write(Name, "Етап: салон продезінфіковано");
        }

        protected override void Load(Journal journal)
        {
            journal.Write(Name, "Етап: завантажено пацієнтів і обладнання");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Situation2();
        }

        static void Situation2()
        {
            Journal journal = new Journal(10, 0);

            journal.Write("Сценарій", "Обслуговування пасажирського рейсу");
            new PassengerFlightService("PS-101").Serve(journal);

            journal.Write("Сценарій", "Обслуговування вантажного рейсу тією самою процедурою");
            new CargoFlightService("PS-201").Serve(journal);

            journal.Write("Сценарій", "Обслуговування чартерного рейсу тією самою процедурою");
            new CharterFlightService("PS-301").Serve(journal);

            journal.Write("Сценарій", "Санітарний рейс додано одним класом");
            new SanitaryFlightService("PS-401").Serve(journal);
        }
    }
}
