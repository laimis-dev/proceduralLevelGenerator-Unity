namespace Observer
{
    public interface IObserver
    {
        // Receive update from subject
        void UpdateObserver(string state);
    }

    public interface ISubject
    {
        // Attach an observer to the subject.
        void Attach(IObserver observer);

        // Detach an observer from the subject.
        void Detach(IObserver observer);

        // Notify all observers about an event.
        void Notify(string state);
    }
}