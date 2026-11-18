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

    class Flight
    {
        private FlightState state;

        public string Number { get; private set; }
        public string Gate { get; private set; }
        public Journal Journal { get; private set; }

        public Flight(string number, string gate, Journal journal)
        {
            Number = number;
            Gate = gate;
            Journal = journal;
            state = new ScheduledState();
            journal.Write(state.Name, "Рейс " + number + " створено, вихід " + gate);
        }

        public string StateName
        {
            get { return state.Name; }
        }

        public void SetState(FlightState next)
        {
            state = next;
        }

        public void SetGate(string gate)
        {
            Gate = gate;
        }

        public void OpenCheckIn()
        {
            state.OpenCheckIn(this);
        }

        public void RegisterPassenger(string passenger)
        {
            state.RegisterPassenger(this, passenger);
        }

        public void ChangeGate(string gate)
        {
            state.ChangeGate(this, gate);
        }

        public void StartBoarding()
        {
            state.StartBoarding(this);
        }

        public void Depart()
        {
            state.Depart(this);
        }

        public void Land()
        {
            state.Land(this);
        }

        public void Delay()
        {
            state.Delay(this);
        }

        public void ResumeCheckIn()
        {
            state.ResumeCheckIn(this);
        }

        public void Cancel()
        {
            state.Cancel(this);
        }

        public void ReturnToStand()
        {
            state.ReturnToStand(this);
        }
    }

    abstract class FlightState
    {
        public abstract string Name { get; }

        public virtual void OpenCheckIn(Flight flight)
        {
            Refuse(flight, "відкрити реєстрацію");
        }

        public virtual void RegisterPassenger(Flight flight, string passenger)
        {
            Refuse(flight, "зареєструвати пасажира " + passenger);
        }

        public virtual void ChangeGate(Flight flight, string gate)
        {
            Refuse(flight, "змінити вихід на " + gate);
        }

        public virtual void StartBoarding(Flight flight)
        {
            Refuse(flight, "почати посадку");
        }

        public virtual void Depart(Flight flight)
        {
            Refuse(flight, "випустити рейс");
        }

        public virtual void Land(Flight flight)
        {
            Refuse(flight, "зафіксувати приземлення");
        }

        public virtual void Delay(Flight flight)
        {
            Refuse(flight, "затримати рейс");
        }

        public virtual void ResumeCheckIn(Flight flight)
        {
            Refuse(flight, "відновити реєстрацію");
        }

        public virtual void Cancel(Flight flight)
        {
            Refuse(flight, "скасувати рейс");
        }

        public virtual void ReturnToStand(Flight flight)
        {
            Refuse(flight, "повернути борт на стоянку");
        }

        protected void Refuse(Flight flight, string action)
        {
            flight.Journal.Write(Name, "Відмова: не можна " + action + ". Стан: «" + Name + "»");
        }

        protected void Go(Flight flight, FlightState next)
        {
            flight.Journal.Write(Name, "Перехід: «" + Name + "» → «" + next.Name + "»");
            flight.SetState(next);
        }
    }

    class ScheduledState : FlightState
    {
        public override string Name { get { return "Заплановано"; } }

        public override void ChangeGate(Flight flight, string gate)
        {
            flight.SetGate(gate);
            flight.Journal.Write(Name, "Вихід рейсу " + flight.Number + " змінено на " + gate);
        }

        public override void OpenCheckIn(Flight flight)
        {
            Go(flight, new CheckInState());
        }

        public override void Cancel(Flight flight)
        {
            Go(flight, new CancelledState());
        }
    }

    class CheckInState : FlightState
    {
        public override string Name { get { return "Реєстрація"; } }

        public override void RegisterPassenger(Flight flight, string passenger)
        {
            flight.Journal.Write(Name, "Пасажира " + passenger + " зареєстровано на рейс " + flight.Number);
        }

        public override void ChangeGate(Flight flight, string gate)
        {
            flight.SetGate(gate);
            flight.Journal.Write(Name, "Вихід рейсу " + flight.Number + " змінено на " + gate);
        }

        public override void StartBoarding(Flight flight)
        {
            Go(flight, new BoardingState());
        }

        public override void Delay(Flight flight)
        {
            Go(flight, new DelayedState());
        }

        public override void Cancel(Flight flight)
        {
            Go(flight, new CancelledState());
        }
    }

    class DelayedState : FlightState
    {
        public override string Name { get { return "Затримано"; } }

        public override void ChangeGate(Flight flight, string gate)
        {
            flight.SetGate(gate);
            flight.Journal.Write(Name, "Вихід рейсу " + flight.Number + " змінено на " + gate);
        }

        public override void ResumeCheckIn(Flight flight)
        {
            Go(flight, new CheckInState());
        }

        public override void Cancel(Flight flight)
        {
            Go(flight, new CancelledState());
        }
    }

    class BoardingState : FlightState
    {
        public override string Name { get { return "Посадка"; } }

        public override void Depart(Flight flight)
        {
            Go(flight, new DepartedState());
        }

        public override void Cancel(Flight flight)
        {
            Go(flight, new CancelledState());
        }

        public override void ReturnToStand(Flight flight)
        {
            Go(flight, new ReturnToStandState());
        }
    }

    class DepartedState : FlightState
    {
        public override string Name { get { return "Вилетів"; } }

        public override void Land(Flight flight)
        {
            Go(flight, new LandedState());
        }
    }

    class LandedState : FlightState
    {
        public override string Name { get { return "Приземлився"; } }
    }

    class CancelledState : FlightState
    {
        public override string Name { get { return "Скасовано"; } }
    }

    class ReturnToStandState : FlightState
    {
        public override string Name { get { return "Повернення на стоянку"; } }

        public override void OpenCheckIn(Flight flight)
        {
            Go(flight, new CheckInState());
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Situation1();
        }

        static void Situation1()
        {
            Journal journal = new Journal(9, 0);

            journal.Write("Сценарій", "Рейс PS-101 від заплановано до приземлився");
            Flight ps101 = new Flight("PS-101", "A2", journal);
            ps101.ChangeGate("B12");
            ps101.OpenCheckIn();
            ps101.RegisterPassenger("Олена Коваль");
            ps101.Delay();
            ps101.RegisterPassenger("Іван Петренко");
            ps101.ResumeCheckIn();
            ps101.StartBoarding();
            ps101.ChangeGate("C3");
            ps101.Depart();
            ps101.RegisterPassenger("Марія Сидоренко");
            ps101.Cancel();
            ps101.Land();

            journal.Write("Сценарій", "Рейс PS-102 повертається на стоянку під час посадки");
            Flight ps102 = new Flight("PS-102", "A4", journal);
            ps102.OpenCheckIn();
            ps102.StartBoarding();
            ps102.ReturnToStand();

            journal.Write("Сценарій", "Рейс PS-103 скасовано до вильоту");
            Flight ps103 = new Flight("PS-103", "B2", journal);
            ps103.Cancel();
        }
    }
}
